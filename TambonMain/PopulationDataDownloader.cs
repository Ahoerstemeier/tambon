using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using MinimalJson;

namespace De.AHoerstemeier.Tambon
{
    // http://stat.dopa.go.th/stat/statnew/statTDD/datasource/showStatZone.php?statType=1&year=56&rcode=1001
    // {"aaData":[["00","ท้องถิ่นเขตพระนคร","27,279","29,405","56,684","19,257"],["10010100","แขวงพระบรมมหาราชวัง","2,695","1,921","4,616","1,304"],...
    // http://stat.dopa.go.th/stat/statnew/statTDD/views/showDistrictData.php?rcode=10&statType=1&year=56

    // http://stat.dopa.go.th/stat/statnew/statTDD/datasource/showStatDistrict.php?statType=1&year=56&rcode=10
    // {"aaData":[["00","กรุงเทพมหานคร","2,694,921","2,991,331","5,686,252","2,593,827"],["1001","ท้องถิ่นเขตพระนคร","27,279","29,405","56,684","19,257"],...
    // http://stat.dopa.go.th/stat/statnew/statTDD/views/showZoneData.php?rcode=1001&statType=1&year=56

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

        #region fields

        /// <summary>
        /// Geocode for which the data is to be downloaded.
        /// </summary>
        private UInt32 _geocode = 0;

        /// <summary>
        /// Year in Buddhist era, shortened to two digits.
        /// </summary>
        private Int32 _yearShort = 0;

        /// <summary>
        /// Whether the download should go down to Muban level.
        /// </summary>
        private Boolean _downloadMuban = false;

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
            if ( (_yearShort < 0) | (_yearShort > 99) )
            {
                throw new ArgumentOutOfRangeException();
            }
            if ( (_geocode < 0) | (_geocode > 99) )
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        private PopulationDataDownloader()
        {
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
        /// </remarks>
        private const String _urlShowAmphoe = "http://stat.dopa.go.th/stat/statnew/statTDD/views/showZoneData.php?statType={2}&year={0}&rcode={1}";

        /// <summary>
        /// URL to download the population data of an Amphoe in JSON format.
        /// </summary>
        /// <remarks>
        /// <para>{0} is expanded by the last two digits of the year in Buddhist era.</para>
        /// <para>{1} is expanded by the geocode (4 digits).</para>
        /// <para>{2} is expanded by the data type (see <see cref="DopaStatisticsType"/>).</para>
        /// </remarks>
        private const String _urlDataAmphoe = "http://stat.dopa.go.th/stat/statnew/statTDD/datasource/showStatZone.php?statType={2}&year={0}&rcode={1}";

        /// <summary>
        /// URL to show the population data of a province.
        /// </summary>
        /// <remarks>
        /// <para>{0} is expanded by the last two digits of the year in Buddhist era.</para>
        /// <para>{1} is expanded by the geocode (2 digits).</para>
        /// <para>{2} is expanded by the data type (see <see cref="DopaStatisticsType"/>).</para>
        /// </remarks>
        private const String _urlShowChangwat = "http://stat.dopa.go.th/stat/statnew/statTDD/views/showDistrictData.php?statType={2}&year={0}&rcode={1}";

        /// <summary>
        /// URL to download the population data of a province in JSON format.
        /// </summary>
        /// <remarks>
        /// <para>{0} is expanded by the last two digits of the year in Buddhist era.</para>
        /// <para>{1} is expanded by the geocode (2 digits).</para>
        /// <para>{2} is expanded by the data type (see <see cref="DopaStatisticsType"/>).</para>
        /// </remarks>
        private const String _urlDataChangwat = "http://stat.dopa.go.th/stat/statnew/statTDD/datasource/showStatDistrict.php?statType={2}&year={0}&rcode={1}";

        /// <summary>
        /// URL to download the population data of the country in JSON format.
        /// </summary>
        /// <remarks>
        /// <para>{0} is expanded by the last two digits of the year in Buddhist era.</para>
        /// <para>{1} is expanded by the data type (see <see cref="DopaStatisticsType"/>).</para>
        /// </remarks>
        private const String _urlDataCountry = "http://stat.dopa.go.th/stat/statnew/statTDD/datasource/showStatProvince.php?statType={1}&year={0}";

        /// <summary>
        /// URL to show the population data of a tambon.
        /// </summary>
        /// <remarks>
        /// <para>{0} is expanded by the last two digits of the year in Buddhist era.</para>
        /// <para>{1} is expanded by the geocode (8 digits, see <see cref="TambonGeocodeForDataRetrieval"/>).</para>
        /// <para>{2} is expanded by the data type (see <see cref="DopaStatisticsType"/>).</para>
        /// </remarks>
        private const String _urlShowTambon = "http://stat.dopa.go.th/stat/statnew/statTDD/views/showVillageData.php?rcode={1}&statType={2}&year={0}";

        /// <summary>
        /// URL to download the population data of a tambon.
        /// </summary>
        /// <remarks>
        /// <para>{0} is expanded by the last two digits of the year in Buddhist era.</para>
        /// <para>{1} is expanded by the geocode (8 digits, see <see cref="TambonGeocodeForDataRetrieval"/>).</para>
        /// <para>{2} is expanded by the data type (see <see cref="DopaStatisticsType"/>).</para>
        /// </remarks>
        private const String _urlDataTambon = "http://stat.dopa.go.th/stat/statnew/statTDD/datasource/showStatVillage.php?rcode={1}&statType={2}&year={0}";

        /// <summary>
        /// Maximum number of retries of a invalid JSon reply from the DOPA server.
        /// </summary>
        private Int32 maxRetry = 256;

        /// <summary>
        /// Translation dictionary from <see cref="DopaStatisticsType"/> to <see cref="PopulationChangeType"/>.
        /// </summary>
        private Dictionary<DopaStatisticsType, PopulationChangeType> _populationChangeType = new Dictionary<DopaStatisticsType, PopulationChangeType>()
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
            if ( ProcessingFinished != null )
            {
                ProcessingFinished(this, e);
            }
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
            switch ( language )
            {
                case Language.English:
                    result = String.Format(CultureInfo.InvariantCulture,
                        "<ref>{{{{cite web|url={0}|publisher=Department of Provincial Administration|title=Population statistics {1}|language=Thai|accessdate={2}}}}}</ref>",
                        String.Format(CultureInfo.InvariantCulture, _urlShowChangwat, _yearShort, geocode, (Int32)DopaStatisticsType.Population), year, DateTime.Now.ToString("yyyy-MM-dd"));
                    break;

                case Language.German:
                    result = String.Format(CultureInfo.InvariantCulture,
                        "<ref>{{{{cite web|url={0}|publisher=Department of Provincial Administration|title=Einwohnerstatistik {1}|language=Thai|accessdate={2}}}}}</ref>",
                        String.Format(CultureInfo.InvariantCulture, _urlShowChangwat, _yearShort, geocode, (Int32)DopaStatisticsType.Population), year, DateTime.Now.ToString("yyyy-MM-dd"));
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
            using ( var fileStream = new FileStream(fromFile, FileMode.Open, FileAccess.Read) )
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
            if ( Data != null )
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
        /// Calculates the geocode needed to retrieve the Tambon data from DOPA.
        /// </summary>
        /// <param name="geocode">Geocode of the Tambon (6 digits).</param>
        /// <param name="parentGeocode">Geocode of the parent (Amphoe or Thesaban, 4 digits)</param>
        /// <returns>Combined geocode.</returns>
        private UInt32 TambonGeocodeForDataRetrieval(UInt32 geocode, UInt32 parentGeocode)
        {
            return parentGeocode * 10000 + geocode % 10000;
        }

        /// <summary>
        /// Download the population data from DOPA.
        /// </summary>
        /// <param name="geocode">Geocode of the entity.</param>
        /// <param name="statisticsType">Statistics type.</param>
        /// <returns>JSON object returned from website.</returns>
        private JsonObject GetDataFromDopa(UInt32 geocode, DopaStatisticsType statisticsType)
        {
            JsonObject obj = null;
            String url;
            if ( geocode < 100 )
            {
                url = String.Format(CultureInfo.InvariantCulture, _urlDataChangwat, _yearShort, geocode, (Int32)statisticsType);
            }
            else if ( geocode < 10000 )
            {
                url = String.Format(CultureInfo.InvariantCulture, _urlDataAmphoe, _yearShort, geocode, (Int32)statisticsType);
            }
            else
            {
                url = String.Format(CultureInfo.InvariantCulture, _urlDataTambon, _yearShort, geocode, (Int32)statisticsType);
            }
            Int32 errorCount = 0;
            while ( obj == null )
            {
                try
                {
                    WebClient webClient = new System.Net.WebClient();
                    Stream inputStream = webClient.OpenRead(url);
                    String response = StreamToString(inputStream);

                    JsonValue result = JsonValue.readFrom(response);
                    if ( !result.isObject() )
                    {
                        return null;
                    }
                    obj = result.asObject();
                }
                catch
                {
                    errorCount++;
                }
                if ( errorCount > maxRetry )
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
            using ( StreamReader reader = new StreamReader(stream, Encoding.UTF8) )
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
            UInt32 changwatGeocode = GeocodeHelper.ProvinceCode(geocode);
            String url = String.Format(CultureInfo.InvariantCulture, _urlShowChangwat, year + 543 - 2500, changwatGeocode, (Int32)DopaStatisticsType.Population);
            return new Uri(url);
        }

        /// <summary>
        /// Parsing the JSON data returned from DOPA website.
        /// </summary>
        /// <param name="data">JSON data.</param>
        /// <returns>Entities with population data.</returns>
        private IEnumerable<Entity> ParsePopulationJson(JsonObject data)
        {
            var result = new List<Entity>();
            var actualData = data.get("aaData");
            if ( actualData != null )
            {
                var array = actualData.asArray();
                foreach ( JsonArray item in array )
                {
                    var parsedData = new List<String>();
                    foreach ( JsonValue dataPoint in item )
                    {
                        var strippedText = Regex.Replace(dataPoint.asString(), "<.*?>", string.Empty).Replace(",", String.Empty);
                        if ( strippedText == "-" )
                        {
                            strippedText = "0";
                        }
                        parsedData.Add(strippedText);
                    }
                    var firstLine = parsedData.First();
                    if ( !String.IsNullOrWhiteSpace(firstLine) && (firstLine != "00") )
                    {
                        Entity entity = new Entity();
                        entity.ParseName(parsedData.ElementAt(1).Replace("ท้องถิ่น", String.Empty).Trim());
                        entity.geocode = Convert.ToUInt32(firstLine, CultureInfo.InvariantCulture);
                        while ( entity.geocode % 100 == 0 )
                        {
                            entity.geocode = entity.geocode / 100;
                        }

                        PopulationData population = CreateEmptyPopulationEntry();
                        entity.population.Add(population);
                        HouseholdDataPoint householdDataPoint = new HouseholdDataPoint();
                        householdDataPoint.male = Convert.ToInt32(parsedData.ElementAt(2), CultureInfo.InvariantCulture);
                        householdDataPoint.female = Convert.ToInt32(parsedData.ElementAt(3), CultureInfo.InvariantCulture);
                        householdDataPoint.total = Convert.ToInt32(parsedData.ElementAt(4), CultureInfo.InvariantCulture);
                        householdDataPoint.households = Convert.ToInt32(parsedData.ElementAt(5), CultureInfo.InvariantCulture);
                        population.data.Add(householdDataPoint);
                        if ( (householdDataPoint.total > 0) || (householdDataPoint.households > 0) )
                        {
                            // occasionally there are empty entries, e.g. for 3117 includes an empty 311102
                            result.Add(entity);
                        }
                    }
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
            PopulationData population = new PopulationData();
            population.source = PopulationDataSourceType.DOPA;
            population.referencedate = new DateTime(Year, 12, 31);
            population.referencedateSpecified = true;
            population.year = Year.ToString(CultureInfo.InvariantCulture);
            return population;
        }

        private void GetProvinceData()
        {
            Data = new Entity();
            var populationJsonData = GetDataFromDopa(_geocode, DopaStatisticsType.Population);
            Data.entity.AddRange(ParsePopulationJson(populationJsonData));
            foreach ( var entity in Data.entity )
            {
                var subPopulationData = GetDataFromDopa(entity.geocode, DopaStatisticsType.Population);
                entity.entity.AddRange(ParsePopulationJson(subPopulationData));
                if ( _geocode > 10 && _downloadMuban )  // Bangkok has no Muban
                {
                    foreach ( var tambonEntity in entity.entity )
                    {
                        var mubanPopulationData = GetDataFromDopa(TambonGeocodeForDataRetrieval(tambonEntity.geocode, entity.geocode), DopaStatisticsType.Population);
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
            var jsonData = GetDataFromDopa(geocode, dataType);
            if ( populationData.register == null )
            {
                populationData.register = new RegisterData();
            }

            PopulationChangeEntry otherData = ParseAdditionalJson(jsonData);
            otherData.type = _populationChangeType[dataType];
            populationData.register.change.Add(otherData);
        }

        private PopulationChangeEntry ParseAdditionalJson(JsonObject data)
        {
            var result = new PopulationChangeEntry();

            var actualData = data.get("aaData");
            if ( actualData != null )
            {
                var array = actualData.asArray();
                foreach ( JsonArray item in array )
                {
                    var parsedData = new List<String>();
                    foreach ( JsonValue dataPoint in item )
                    {
                        var strippedText = Regex.Replace(dataPoint.asString(), "<.*?>", string.Empty).Replace(",", String.Empty);
                        if ( strippedText == "-" )
                        {
                            strippedText = "0";
                        }
                        parsedData.Add(strippedText);
                    }
                    var firstLine = parsedData.First();
                    if ( firstLine == "00" )
                    {
                        result.male = Convert.ToInt32(parsedData.ElementAt(2), CultureInfo.InvariantCulture);
                        result.female = Convert.ToInt32(parsedData.ElementAt(3), CultureInfo.InvariantCulture);
                        result.total = Convert.ToInt32(parsedData.ElementAt(4), CultureInfo.InvariantCulture);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Synchronizes the calculated data with the global geocode list.
        /// </summary>
        private void GetGeocodes()
        {
            if ( Data != null )
            {
                var geocodes = GlobalData.GetGeocodeList(Data.geocode);
                // _invalidGeocodes = geocodes.InvalidGeocodeEntries();
                foreach ( var amphoe in geocodes.entity.Where(x => x.type.IsCompatibleEntityType(EntityType.Amphoe)) )
                {
                    if ( !Data.entity.Any(x => GeocodeHelper.IsSameGeocode(x.geocode, amphoe.geocode, false)) )
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
            using ( var fileStream = new StreamWriter(XmlExportFileName()) )
            {
                fileStream.WriteLine(data);
            }
        }

        private void ReOrderThesaban()
        {
            if ( Data != null )
            {
                Data.ReorderThesaban();
            }
        }

        /// <summary>
        /// Gets the data for all provinces, used if <see cref="_geocode"/> is 0.
        /// </summary>
        private void ProcessAllProvinces()
        {
            Data = new Entity();
            Data.population = new List<PopulationData>();
            PopulationData populationData = CreateEmptyPopulationEntry();
            Data.population.Add(populationData);
            Data.type = EntityType.Country;
            Data.CopyBasicDataFrom(GlobalData.CountryEntity);

            foreach ( var entry in GlobalData.Provinces )
            {
                var tempCalculator = new PopulationDataDownloader(Year, entry.geocode);
                tempCalculator.Process();
                Data.entity.Add(tempCalculator.Data);
            }
            Data.CalculatePopulationFromSubEntities(Year, PopulationDataSourceType.DOPA);
        }

        private void ProcessProvince(UInt32 geocode)
        {
            GetProvinceData();
            // TODO: Special handling of Mu 0 (หมู่ที่ 0) entries
            GetGeocodes();
            // TODO: special handling of Muban and Thesaban, also to be done in ReOrderThesaban()?
            ReOrderThesaban();
            FixupWronglyPlacedAmphoe();
            var toRemove = new List<Entity>();
            foreach ( var entity in Data.FlatList() )
            {
                if ( entity.population.Any() )
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
            foreach ( var amphoe in Data.entity.Where(x => x.type.IsCompatibleEntityType(EntityType.Amphoe)) )
            {
                foreach ( var tambon in amphoe.entity.Where(x => !GeocodeHelper.IsBaseGeocode(amphoe.geocode, x.geocode)).ToList() )
                {
                    invalidTambon.Add(tambon);
                    amphoe.entity.Remove(tambon);
                }
            }
            foreach ( var tambon in invalidTambon )
            {
                var mainTambon = Data.FlatList().FirstOrDefault(x => GeocodeHelper.IsSameGeocode(x.geocode, tambon.geocode, false));
                if ( mainTambon != null )
                {
                    foreach ( var dataPoint in tambon.population.First().data )
                    {
                        mainTambon.population.First().AddDataPoint(dataPoint);
                    }
                }
            }
            var emptyAmphoe = Data.entity.Where(x => x.type.IsCompatibleEntityType(EntityType.Amphoe) && !x.entity.Any()).ToList();
            foreach ( var toRemove in emptyAmphoe )
            {
                Data.entity.Remove(toRemove);
            }
        }

        /// <summary>
        /// Starts the download of the data.
        /// </summary>
        public void Process()
        {
            if ( _geocode == 0 )
            {
                ProcessAllProvinces();
            }
            else
            {
                ProcessProvince(_geocode);
            }
            Data.SortByGeocodeRecursively();
            OnProcessingFinished(new EventArgs());
        }

        #endregion private methods
    }
}