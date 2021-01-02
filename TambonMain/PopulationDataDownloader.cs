using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using MinimalJson;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Downloader of population data from DOPA.
    /// </summary>
    public class PopulationDataDownloader
    {
        /// <summary>
        /// Statistics data types provided by DOPA.
        /// </summary>
        private enum DopaStatisticsType
        {
            /// <summary>
            /// Population data - total, male, female, household.
            /// </summary>
            Population = 1,

            /// <summary>
            /// Household data.
            /// </summary>
            /// <remarks>Strange data, not linked in website. Male column is same value as number of households.</remarks>
            [Obsolete]
            Household = 2,

            /// <summary>
            /// Birth numbers.
            /// </summary>
            Birth = 3,

            /// <summary>
            /// Death numbers.
            /// </summary>
            Death = 4,

            /// <summary>
            /// Move out numbers.
            /// </summary>
            MoveOut = 5,

            /// <summary>
            /// Move in numbers.
            /// </summary>
            MoveIn = 6
        }

        /// <summary>
        /// Translation table from the statistics type to the code used in the URLs.
        /// </summary>
        private static readonly Dictionary<DopaStatisticsType, String> _statisticTypeNames = new Dictionary<DopaStatisticsType, String>()
        {
            { DopaStatisticsType.Population, "statpop" },
            { DopaStatisticsType.Birth, "statbirth" },
            { DopaStatisticsType.Death, "statdeath" },
            { DopaStatisticsType.MoveIn, "statmovein" },
            { DopaStatisticsType.MoveIn, "statmoveout" },
         };

        #region fields

        /// <summary>
        /// Geocode for which the data is to be downloaded.
        /// </summary>
        private readonly UInt32 _geocode = 0;

        /// <summary>
        /// Year in Buddhist era, shortened to two digits.
        /// </summary>
        private readonly Int32 _yearShort = 0;

        /// <summary>
        /// Whether the download should go down to Muban level.
        /// </summary>
        private readonly Boolean _downloadMuban = false;

        #endregion fields

        #region properties

        /// <summary>
        /// Gets the population data.
        /// </summary>
        /// <value>The data.</value>
        public Entity Data
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the year (common era) for which is population data is done.
        /// </summary>
        /// <value>The year.</value>
        public Int32 Year
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the directory to write the processed data as XML files.
        /// </summary>
        /// <value>The output directory.</value>
        public static String OutputDirectory
        {
            get;
            set;
        }

        #endregion properties

        #region constructor

        /// <summary>
        /// Creates a new instance of <see cref="PopulationDataDownloader"/>.
        /// </summary>
        /// <param name="year">Year (common era), must be between 1957 (BE 2500) and 2056 (BE 2599).</param>
        /// <param name="geocode">Province geocode. 0 means to download all of country.</param>
        public PopulationDataDownloader(Int32 year, UInt32 geocode)
        {
            Year = year;
            _geocode = geocode;
            _yearShort = Year + 543 - 2500;
            if ((_yearShort < 0) | (_yearShort > 99))
            {
                throw new ArgumentOutOfRangeException();
            }
            if ((_geocode < 0) | (_geocode > 99))
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        #endregion constructor

        #region constants

        /// <summary>
        /// URL to show the population data of an Amphoe.
        /// </summary>
        /// <remarks>
        /// <para>{0} is expanded by the last two digits of the year in Buddhist era.</para>
        /// <para>{1} is expanded by the geocode (4 digits).</para>
        /// <para>{2} is expanded by the data type (see <see cref="DopaStatisticsType"/>).</para>
        /// <para>{3} is expanded by the province geocode.</para>
        /// </remarks>
        private const String _urlShowAmphoe = "https://stat.bora.dopa.go.th/stat/statnew/statyear/#/TableTemplate4/Area/statpop?yymm={0}&&topic={2}&ccNo={2}&rcodeNo={1}";

        /// <summary>
        /// URL to show the population data of a province.
        /// </summary>
        /// <remarks>
        /// <para>{0} is expanded by the last two digits of the year in Buddhist era.</para>
        /// <para>{1} is expanded by the geocode (2 digits).</para>
        /// <para>{2} is expanded by the data type (see <see cref="DopaStatisticsType"/>).</para>
        /// </remarks>
        private const String _urlShowChangwat = "https://stat.bora.dopa.go.th/stat/statnew/statyear/#/TableTemplate3/Area/statpop?yymm={0}&topic={2}&ccNo={1}";

        /// <summary>
        /// URL to show the population data of a tambon.
        /// </summary>
        /// <remarks>
        /// <para>{0} is expanded by the last two digits of the year in Buddhist era.</para>
        /// <para>{1} is expanded by the registrar geocode (4 digits).
        /// <para>{2} is expanded by the data type (see <see cref="DopaStatisticsType"/>).</para>
        /// <para>{3} is expanded by the Tambon geocode (6 digits).
        /// </remarks>
        private const String _urlShowTambon = "https://stat.bora.dopa.go.th/stat/statnew/statyear/#/TableTemplate5/Area/statpop?yymm={0}&topic={2}&ccNo=11&rcodeNo={1}&ttNo={3}";

        /// <summary>
        /// Maximum number of retries of a invalid JSon reply from the DOPA server.
        /// </summary>
        private const Int32 maxRetry = 8;

        /// <summary>
        /// Translation dictionary from <see cref="DopaStatisticsType"/> to <see cref="PopulationChangeType"/>.
        /// </summary>
        private static readonly Dictionary<DopaStatisticsType, PopulationChangeType> _populationChangeType = new Dictionary<DopaStatisticsType, PopulationChangeType>()
        {
            { DopaStatisticsType.Birth, PopulationChangeType.birth },
            { DopaStatisticsType.Death, PopulationChangeType.death},
            { DopaStatisticsType.MoveIn, PopulationChangeType.movein },
            { DopaStatisticsType.MoveOut, PopulationChangeType.moveout },
        };

        #endregion constants

        #region events

        /// <summary>
        /// Occurs when the processing is completed.
        /// </summary>
        public EventHandler<EventArgs> ProcessingFinished;

        /// <summary>
        /// Throws the <see cref="ProcessingFinished"/> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        private void OnProcessingFinished(EventArgs e)
        {
            ProcessingFinished?.Invoke(this, e);
        }

        #endregion events

        #region public methods

        /// <summary>
        /// Gets the Wikipedia citation string for the current data element.
        /// </summary>
        /// <param name="geocode">Geocode of province.</param>
        /// <param name="year">Year (4 digit in western style).</param>
        /// <param name="language">Language.</param>
        /// <returns>Wikipedia reference string.</returns>
        public static String WikipediaReference(UInt32 geocode, Int32 year, Language language)
        {
            var _yearShort = (year + 543) % 100;
            String result = String.Empty;
            switch (language)
            {
                case Language.English:
                    result = String.Format(CultureInfo.InvariantCulture,
                        "<ref>{{{{cite web|url={0}|publisher=Department of Provincial Administration|title=Population statistics {1}|language=Thai|accessdate={2}}}}}</ref>",
                        String.Format(CultureInfo.InvariantCulture, _urlShowChangwat, _yearShort, geocode, _statisticTypeNames[DopaStatisticsType.Population]), year, DateTime.Now.ToString("yyyy-MM-dd"));
                    break;

                case Language.German:
                    result = String.Format(CultureInfo.InvariantCulture,
                        "<ref>{{{{cite web|url={0}|publisher=Department of Provincial Administration|title=Einwohnerstatistik {1}|language=Thai|accessdate={2}}}}}</ref>",
                        String.Format(CultureInfo.InvariantCulture, _urlShowChangwat, _yearShort, geocode, _statisticTypeNames[DopaStatisticsType.Population]), year, DateTime.Now.ToString("yyyy-MM-dd"));
                    break;

                case Language.Thai:
                    break;

                default:
                    break;
            }
            return result;
        }

        /// <summary>
        /// Gets the (english) Wikipedia citation string for the current data element.
        /// </summary>
        /// <returns>Wikipedia reference string.</returns>
        public String WikipediaReference()
        {
            return WikipediaReference(_geocode, Year, Language.English);
        }

        /// <summary>
        /// Loads population data from a XML file.
        /// </summary>
        /// <param name="fromFile">File name.</param>
        /// <returns>Population data.</returns>
        public static PopulationData Load(String fromFile)
        {
            PopulationData result = null;
            using (var fileStream = new FileStream(fromFile, FileMode.Open, FileAccess.Read))
            {
                result = XmlManager.XmlToEntity<PopulationData>(fileStream, new XmlSerializer(typeof(PopulationData)));
            }

            return result;
        }

        /// <summary>
        /// Gets the filename to which the data would be written.
        /// </summary>
        /// <returns>File name of generated file.</returns>
        public String XmlExportFileName()
        {
            String result = String.Empty;
            if (Data != null)
            {
                String outputDirectory = Path.Combine(OutputDirectory, "DOPA");
                Directory.CreateDirectory(outputDirectory);
                result = Path.Combine(outputDirectory, String.Format("population{0:D2} {1}.XML", Data.geocode, Year));
            }
            return result;
        }

        #endregion public methods

        #region private methods

        /// <summary>
        /// Download the population data from DOPA.
        /// </summary>
        /// <param name="geocode">Geocode of the entity.</param>
        /// <param name="statisticsType">Statistics type.</param>
        /// <returns>JSON array returned from website.</returns>
        private JsonArray GetDataFromDopa(UInt32 geocode, UInt32 registrarGeocode, DopaStatisticsType statisticsType)
        {
            JsonArray obj = null;
            var urlData = String.Empty;
            switch (statisticsType)
            {
                case DopaStatisticsType.Population:
                    urlData = "https://stat.bora.dopa.go.th/stat/statnew/connectSAPI/stat_forward.php?API=/api/statpophouse/v1/statpop/list?action={4}&yymmBegin={0}12&yymmEnd={0}12&statType=0&statSubType=999&subType=99&cc={1}&rcode={2}&tt={3}&mm=0";
                    break;
                case DopaStatisticsType.Birth:
                    urlData = "https://stat.bora.dopa.go.th/stat/statnew/connectSAPI/stat_forward.php?API=/api/stattranall/v1/statbirth/list?action=191&yymmBegin={0}12&yymmEnd={0}12&statType=0&statSubType=999&subType=99&cc={1}&rcode={2}&tt={3}&tt=0&mm=0";
                    break;
                case DopaStatisticsType.Death:
                    urlData = "https://stat.bora.dopa.go.th/stat/statnew/connectSAPI/stat_forward.php?API=/api/stattranall/v1/statdeath/list?action=191&yymmBegin={0}12&yymmEnd={0}12&statType=0&statSubType=999&subType=99&cc={1}&rcode={2}&tt={3}&mm=0";
                    break;
                case DopaStatisticsType.MoveIn:
                    urlData = "https://stat.bora.dopa.go.th/stat/statnew/connectSAPI/stat_forward.php?API=/api/stattranall/v1/statmovein/list?action=191&yymmBegin={0}12&yymmEnd={0}12&statType=1&statSubType=999&subType=99&cc={1}&rcode={2}&tt={3}&mm=0";
                    break;
                case DopaStatisticsType.MoveOut:
                    urlData = "https://stat.bora.dopa.go.th/stat/statnew/connectSAPI/stat_forward.php?API=/api/stattranall/v1/statmoveout/list?action=191&yymmBegin={0}12&yymmEnd={0}12&statType=1&statSubType=999&subType=99&region=3&cc={1}&rcode=0&tt=0&mm=0";
                    break;
            }

            String url;
            if (geocode < 100)
            {
                url = String.Format(CultureInfo.InvariantCulture, urlData, _yearShort, geocode, 0, 0, 193);
            }
            else if (geocode < 10000)
            {
                url = String.Format(CultureInfo.InvariantCulture, urlData, _yearShort, geocode / 100, geocode, 0, 194);
            }
            else
            {
                url = String.Format(CultureInfo.InvariantCulture, urlData, _yearShort, registrarGeocode / 100, registrarGeocode, geocode % 100, 195);
            }
            Int32 errorCount = 0;
            while (obj == null)
            {
                try
                {
                    var webClient = new System.Net.WebClient();
                    var inputStream = webClient.OpenRead(url);
                    var response = StreamToString(inputStream);

                    var result = JsonValue.readFrom(response);
                    if (!result.isArray())
                    {
                        return null;
                    }
                    obj = result.asArray();
                }
                catch
                {
                    errorCount++;
                }
                if (errorCount > maxRetry)
                {
                    throw new InvalidDataException(String.Format("Failed to get parseable json data for {0}", geocode));
                }
            }
            return obj;
        }

        /// <summary>
        /// Reads a stream into a string.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <returns>Content of stream as string.</returns>
        private static String StreamToString(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Gets the URL to display the population data of a province.
        /// </summary>
        /// <param name="year">Year number.</param>
        /// <param name="geocode">Geocode of the province.</param>
        /// <returns>URL to display the data.</returns>
        public static Uri GetDisplayUrl(Int32 year, UInt32 geocode)
        {
            var changwatGeocode = GeocodeHelper.ProvinceCode(geocode);
            var url = String.Format(CultureInfo.InvariantCulture, _urlShowChangwat, year + 543 - 2500, changwatGeocode, _statisticTypeNames[DopaStatisticsType.Population]);
            return new Uri(url);
        }

        /// <summary>
        /// Parsing the JSON data returned from DOPA website.
        /// </summary>
        /// <param name="data">JSON data.</param>
        /// <returns>Entities with population data.</returns>
        private IEnumerable<Entity> ParsePopulationJson(JsonArray data)
        {
            var result = new List<Entity>();
            foreach (JsonObject item in data)
            {
                Entity entity = new Entity();
                var baseGeocode = Convert.ToUInt32(item.get("lsrcode").asString().Replace("\"", "")); ;
                var mubanName = item.get("lsmmDesc").asString().Replace("\"", "").Trim();
                var changwat = Convert.ToUInt32(item.get("lscc").asInt());
                var amphoe = Convert.ToUInt32(item.get("lsaa").asInt());
                var tambon = Convert.ToUInt32(item.get("lstt").asInt());
                var muban = Convert.ToUInt32(item.get("lsmm").asInt());
                if (tambon > 0)
                {
                    // for tambon/muban, always need to start with amphoe, not with the thesaban rcode
                    baseGeocode = changwat * 100 + amphoe;
                }
                if (!String.IsNullOrEmpty(mubanName) && muban == 0)
                {
                    // Mu 0 occurs in JSON. Using magic 99 instead, then the stripping of final 00s will not fail
                    muban = 99;
                }
                entity.geocode = (baseGeocode * 100 + tambon) * 100 + muban;
                // normalize geocode by stripping final 00s
                while (entity.geocode % 100 == 0)
                {
                    entity.geocode = entity.geocode / 100;
                }

                PopulationData population = CreateEmptyPopulationEntry();
                entity.population.Add(population);
                HouseholdDataPoint householdDataPoint = new HouseholdDataPoint
                {
                    male = item.get("lssumtotMale").asInt(),
                    female = item.get("lssumtotFemale").asInt(),
                    total = item.get("lssumtotPop").asInt(),
                    households = item.get("lssumnotTermDate").asInt()
                };
                // oddly, the website displays the value "lssumnotTermDate" and not lssumtotHouse.
                // seems that lssumnotTermDate + lssumtermDate = lssumtotHouse
                // older API did also return the value now in lssumnotTermDate, so take that value for compatibility 
                population.data.Add(householdDataPoint);
                if ((householdDataPoint.total > 0) && (householdDataPoint.households > 0))
                {
                    // occasionally there are empty entries, e.g. for 3117 includes an empty 311102
                    result.Add(entity);
                }
            }
            return result;
        }

        /// <summary>
        /// Creates an empty population entry for the current download.
        /// </summary>
        /// <returns>Empty population data.</returns>
        private PopulationData CreateEmptyPopulationEntry()
        {
            PopulationData population = new PopulationData
            {
                source = PopulationDataSourceType.DOPA,
                referencedate = new DateTime(Year, 12, 31),
                referencedateSpecified = true,
                year = Year.ToString(CultureInfo.InvariantCulture)
            };
            return population;
        }

        private void GetProvinceData()
        {
            Data = new Entity();
            var populationJsonData = GetDataFromDopa(_geocode, _geocode, DopaStatisticsType.Population);
            Data.entity.AddRange(ParsePopulationJson(populationJsonData));
            foreach (var entity in Data.entity)
            {
                var subPopulationData = GetDataFromDopa(entity.geocode, entity.geocode, DopaStatisticsType.Population);
                entity.entity.AddRange(ParsePopulationJson(subPopulationData));
                if (_geocode > 10 && _downloadMuban)  // Bangkok has no Muban
                {
                    foreach (var tambonEntity in entity.entity)
                    {
                        var mubanPopulationData = GetDataFromDopa(tambonEntity.geocode, entity.geocode, DopaStatisticsType.Population);
                        tambonEntity.entity.AddRange(ParsePopulationJson(mubanPopulationData));
                    }
                }
            }
            Data.geocode = _geocode;
            Data.population = new List<PopulationData>();
            PopulationData populationData = CreateEmptyPopulationEntry();
            Data.population.Add(populationData);

            AddOtherData(populationData, _geocode, DopaStatisticsType.Birth);
            AddOtherData(populationData, _geocode, DopaStatisticsType.Death);
            AddOtherData(populationData, _geocode, DopaStatisticsType.MoveIn);
            AddOtherData(populationData, _geocode, DopaStatisticsType.MoveOut);
        }

        private void AddOtherData(PopulationData populationData, UInt32 geocode, DopaStatisticsType dataType)
        {
            var jsonData = GetDataFromDopa(geocode, geocode, dataType);
            if (populationData.register == null)
            {
                populationData.register = new RegisterData();
            }

            PopulationChangeEntry otherData = null;
            switch (dataType)
            {
                case DopaStatisticsType.Birth:
                    otherData = ParseAdditionalJsonBirth(jsonData);
                    break;
                case DopaStatisticsType.Death:
                    otherData = ParseAdditionalJson(jsonData);
                    break;
                case DopaStatisticsType.MoveIn:
                case DopaStatisticsType.MoveOut:
                    otherData = ParseAdditionalJson(jsonData);
                    break;
            }
            otherData.type = _populationChangeType[dataType];
            populationData.register.change.Add(otherData);
        }

        private PopulationChangeEntry ParseAdditionalJsonBirth(JsonArray data)
        {
            var result = new PopulationChangeEntry();

            foreach (JsonObject item in data)
            {
                result.male += item.get("lssumtotalBoy").asInt();
                result.female += item.get("lssumtotalGirl").asInt();
                result.total += item.get("lssumtotalAll").asInt();
            }

            return result;
        }

        private PopulationChangeEntry ParseAdditionalJson(JsonArray data)
        {
            var result = new PopulationChangeEntry();

            foreach (JsonObject item in data)
            {
                result.male += item.get("lssumtotMale").asInt();
                result.female += item.get("lssumtotFemale").asInt();
                result.total += item.get("lssumtotTot").asInt();
            }

            return result;
        }


        /// <summary>
        /// Synchronizes the calculated data with the global geocode list.
        /// </summary>
        private void GetGeocodes()
        {
            if (Data != null)
            {
                var geocodes = GlobalData.GetGeocodeList(Data.geocode);
                // _invalidGeocodes = geocodes.InvalidGeocodeEntries();
                foreach (var amphoe in geocodes.entity.Where(x => x.type.IsCompatibleEntityType(EntityType.Amphoe)))
                {
                    if (!Data.entity.Any(x => GeocodeHelper.IsSameGeocode(x.geocode, amphoe.geocode, false)))
                    {
                        // make sure all Amphoe will  be in the result list, in case of a Amphoe which has only Thesaban it will be missing in the DOPA data
                        var newAmphoe = new Entity();
                        newAmphoe.population.Add(CreateEmptyPopulationEntry());
                        newAmphoe.CopyBasicDataFrom(amphoe);
                        Data.entity.Add(newAmphoe);
                    }
                }
                Data.SynchronizeGeocodes(geocodes);
            }
        }

        /// <summary>
        /// Exports the data to a XML in the <see cref="OutputDirectory"/>.
        /// </summary>
        public void SaveXml()
        {
            var data = XmlManager.EntityToXml<Entity>(Data);
            using (var fileStream = new StreamWriter(XmlExportFileName()))
            {
                fileStream.WriteLine(data);
            }
        }

        private void ReOrderThesaban()
        {
            if (Data != null)
            {
                Data.ReorderThesaban();
            }
        }

        /// <summary>
        /// Gets the data for all provinces, used if <see cref="_geocode"/> is 0.
        /// </summary>
        private void ProcessAllProvinces()
        {
            PopulationData populationData = CreateEmptyPopulationEntry();
            Data = new Entity
            {
                population = new List<PopulationData>(){ populationData },
                type = EntityType.Country,
            };
            Data.CopyBasicDataFrom(GlobalData.CountryEntity);

            foreach (var entry in GlobalData.Provinces)
            {
                var tempCalculator = new PopulationDataDownloader(Year, entry.geocode);
                tempCalculator.Process();
                Data.entity.Add(tempCalculator.Data);
            }
            Data.CalculatePopulationFromSubEntities(Year, PopulationDataSourceType.DOPA);
        }

        private void ProcessProvince()
        {
            GetProvinceData();
            // TODO: Special handling of Mu 0 (หมู่ที่ 0) entries
            GetGeocodes();
            // TODO: special handling of Muban and Thesaban, also to be done in ReOrderThesaban()?
            ReOrderThesaban();
            FixupWronglyPlacedAmphoe();
            var toRemove = new List<Entity>();
            foreach (var entity in Data.FlatList())
            {
                if (entity.population.Any())
                {
                    entity.population.First().CalculateTotal();
                }
                else
                {
                    toRemove.Add(entity);
                }
            }
            Data.entity.RemoveAll(x => toRemove.Contains(x));
            Data.CalculatePopulationFromSubEntities(Year, PopulationDataSourceType.DOPA);
        }

        private void FixupWronglyPlacedAmphoe()
        {
            var invalidTambon = new List<Entity>();
            foreach (var amphoe in Data.entity.Where(x => x.type.IsCompatibleEntityType(EntityType.Amphoe)))
            {
                foreach (var tambon in amphoe.entity.Where(x => !GeocodeHelper.IsBaseGeocode(amphoe.geocode, x.geocode)).ToList())
                {
                    invalidTambon.Add(tambon);
                    amphoe.entity.Remove(tambon);
                }
            }
            foreach (var tambon in invalidTambon)
            {
                var mainTambon = Data.FlatList().FirstOrDefault(x => GeocodeHelper.IsSameGeocode(x.geocode, tambon.geocode, false));
                if (mainTambon != null)
                {
                    foreach (var dataPoint in tambon.population.First().data)
                    {
                        mainTambon.population.First().AddDataPoint(dataPoint);
                    }
                }
            }
            var emptyAmphoe = Data.entity.Where(x => x.type.IsCompatibleEntityType(EntityType.Amphoe) && !x.entity.Any()).ToList();
            foreach (var toRemove in emptyAmphoe)
            {
                Data.entity.Remove(toRemove);
            }
        }

        /// <summary>
        /// Starts the download of the data.
        /// </summary>
        public void Process()
        {
            if (_geocode == 0)
            {
                ProcessAllProvinces();
            }
            else
            {
                ProcessProvince();
            }
            Data.SortByGeocodeRecursively();
            OnProcessingFinished(new EventArgs());
        }

        #endregion private methods
    }
}