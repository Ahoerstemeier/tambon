using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace De.AHoerstemeier.Tambon
{
    public class PopulationDataDownloader
    {
        public static String CacheDirectory
        {
            get;
            set;
        }

        public static String OutputDirectory
        {
            get;
            set;
        }

        #region fields

        private Boolean _anythingCached = false;
        private UInt32 _geocode = 0;

        #endregion fields

        #region properties

        public Entity Data
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the year for which is population data is done.
        /// </summary>
        public Int32 Year
        {
            get;
            private set;
        }

        #endregion properties

        #region constructor

        public PopulationDataDownloader(Int32 year, UInt32 geocode)
        {
            Year = year;
            _geocode = geocode;
            Int32 yearShort = Year + 543 - 2500;
            if ( (yearShort < 0) | (yearShort > 99) )
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

        private const String _tableEntryStart = "<td bgcolor=#fff9a4><font color=3617d2>";
        private const String _tableDataStart = "<td bgcolor=#ffffcb align=right><font color=0000ff>";
        private const String _tableBoldStart = "<b>";
        private const String _tableBoldEnd = "</b>";
        private const String _tableEntryEnd = "</font></td>";

        // private const String _urlBase = "http://www.dopa.go.th/xstat/";
        // private const String _urlBase = "http://203.113.86.149/xstat/";
        private const String _urlBase = "http://stat.dopa.go.th/xstat/";

        #endregion constants

        #region events

        public EventHandler<EventArgs> ProcessingFinished;

        private void OnProcessingFinished(EventArgs e)
        {
            if ( ProcessingFinished != null )
            {
                ProcessingFinished(this, e);
            }
        }

        #endregion events

        #region methods

        private void DownloadToCache(String filename)
        {
            Stream outputStream = null;
            try
            {
                WebClient webClient = new System.Net.WebClient();
                Stream inputStream = webClient.OpenRead(_urlBase + filename);

                outputStream = new FileStream(HtmlCacheFileName(filename), FileMode.CreateNew);
                inputStream.CopyTo(outputStream);
                outputStream.Flush();
            }
            finally
            {
                outputStream.Dispose();
            }
        }

        private String HtmlCacheFileName(String fileName)
        {
            String directory = Path.Combine(CacheDirectory, "DOPA");
            Directory.CreateDirectory(directory);
            String result = Path.Combine(directory, fileName);
            return result;
        }

        public String WikipediaReference()
        {
            String result = String.Format(CultureInfo.InvariantCulture,
                "<ref>{{cite web|url={0}|publisher=Department of Provincial Administration|title=Population statistics {1}}}</ref>",
                _urlBase + SourceFilename(1), Year);
            return result;
        }

        private Boolean IsCached(String fileName)
        {
            return File.Exists(HtmlCacheFileName(fileName));
        }

        private void ParseSingleFile(String filename, ref Entity currentSubEntry)
        {
            if ( !_anythingCached )
            {
                if ( !IsCached(filename) )
                {
                    DownloadToCache(filename);
                }
                else
                {
                    _anythingCached = true;
                }
            }
            var reader = new StreamReader(HtmlCacheFileName(filename), Encoding.GetEncoding(874));

            String currentLine = String.Empty;
            Entity currentEntry = null;
            Int32 dataState = 0;
            while ( (currentLine = reader.ReadLine()) != null )
            {
                #region parse name

                if ( currentLine.StartsWith(_tableEntryStart) )
                {
                    String value = StripTableHtmlFromLine(currentLine);
                    currentEntry = new Entity();
                    currentEntry.ParseName(value);
                    if ( Data == null )
                    {
                        Data = currentEntry;
                    }
                    else if ( (currentSubEntry == null) | currentEntry.type.IsSecondLevelAdministrativeUnit() )
                    {
                        Data.entity.Add(currentEntry);
                        currentSubEntry = currentEntry;
                    }
                    else
                    {
                        currentSubEntry.entity.Add(currentEntry);
                    }
                    dataState = 0;
                }

                #endregion parse name

                #region parse population data

                if ( currentLine.StartsWith(_tableDataStart) )
                {
                    if ( currentEntry == null )
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    String value = StripTableHtmlFromLine(currentLine);
                    if ( !String.IsNullOrEmpty(value) )
                    {
                        if (!currentEntry.population.Any())
                        {
                            var population = new PopulationData();
                            population.source = PopulationDataSourceType.DOPA;
                            population.year = Year.ToString(CultureInfo.InvariantCulture);
                            population.referencedate = new DateTime(Year - 1, 12, 31);
                            currentEntry.population.Add(population);
                        }
                        var dataPointType = PopulationDataType.total;
                        if (currentEntry.type.IsCompatibleEntityType(EntityType.Changwat) || currentEntry.type.IsCompatibleEntityType(EntityType.Amphoe))
                        {
                            dataPointType = PopulationDataType.total;
                        }
                        else if ( currentEntry.type.IsLocalGovernment() )
                        {
                            dataPointType = PopulationDataType.municipal;
                        }
                        else
                        {
                            dataPointType = PopulationDataType.nonmunicipal;
                        }
                        var dataPoint = currentEntry.population.First().data.FirstOrDefault(x => x.type==dataPointType);
                        if (dataPoint == null)
                        {
                            dataPoint = new HouseholdDataPoint();
                            dataPoint.type = dataPointType;
                            currentEntry.population.First().data.Add(dataPoint);
                        }
                        
                        switch ( dataState )
                        {
                            case 0:
                                dataPoint.male = Int32.Parse(value);
                                break;
                            case 1:
                                dataPoint.female = Int32.Parse(value);
                                break;
                            case 2:
                                dataPoint.total = Int32.Parse(value);
                                break;
                            case 3:
                                dataPoint.households = Int32.Parse(value);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    dataState++;
                }

                #endregion parse population data
            }
        }

        private String SourceFilename(Int16 page)
        {
            Int32 yearShort = Year + 543 - 2500;
            if ( (yearShort < 0) | (yearShort > 99) )
            {
                throw new ArgumentOutOfRangeException();
            }
            if ( (_geocode < 0) | (_geocode > 99) )
            {
                throw new ArgumentOutOfRangeException();
            }
            String result = String.Format(CultureInfo.InvariantCulture, "p{0:D2}{1:D2}_{2:D2}.html", yearShort, _geocode, page);
            return result;
        }

        protected void GetData()
        {
            Int16 count = 0;
            Entity currentSubEntity = null;
            try
            {
                while ( count < 99 )
                {
                    count++;
                    ParseSingleFile(SourceFilename(count), ref currentSubEntity);
                }
            }
            catch
            {
                // TODO: catch selectively the exception expected for HTTP not found/file not found
            }
            if ( Data != null )
            {
                Data.geocode = _geocode;
            }
        }

        private static String StripTableHtmlFromLine(String value)
        {
            string result = value;
            result = result.Replace(_tableDataStart, "");
            result = result.Replace(_tableEntryStart, "");
            result = result.Replace(_tableBoldStart, "");
            result = result.Replace(_tableBoldEnd, "");
            result = result.Replace(_tableEntryEnd, "");
            result = result.Replace(",", "");
            result = result.Trim();
            return result;
        }

        public static PopulationData Load(String fromFile)
        {
            PopulationData result = null;
            using ( var fileStream = new FileStream(fromFile, FileMode.Open, FileAccess.Read) )
            {
                result = XmlManager.XmlToEntity<PopulationData>(fileStream, new XmlSerializer(typeof(PopulationData)));
            }

            return result;
        }

        protected void GetGeocodes()
        {
            if ( Data != null )
            {
                var geocodes = GlobalData.GetGeocodeList(Data.geocode);
                // _invalidGeocodes = geocodes.InvalidGeocodeEntries();
                Data.SynchronizeGeocodes(geocodes);
            }
        }

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

        public void SaveXml()
        {
            var data = XmlManager.EntityToXml<Entity>(Data);
            using ( var fileStream = new StreamWriter(XmlExportFileName()) )
            {
                fileStream.WriteLine(data);
            }
        }

        public void ReOrderThesaban()
        {
            if (Data != null)
            {
                Data.ReorderThesaban();
            }
        }

        private void ProcessAllProvinces()
        {
            var data = new Entity();
            foreach ( var entry in GlobalData.Provinces )
            {
                var tempCalculator = new PopulationDataDownloader(Year, entry.geocode);
                tempCalculator.Process();
                data.entity.Add(tempCalculator.Data);
            }
            // data.ConsolidatePopulationData();
            Data = data;
        }

        private void ProcessProvince(UInt32 geocode)
        {
            GetData();
            // sort Tambon by Population, to avoid double entries in 2012 data to create big mistakes
            if ( Data != null )
            {
                foreach ( var amphoe in Data.entity )
                {
                    amphoe.entity.Sort(
                        delegate(Entity p1, Entity p2)
                        {
                            return -p1.population.First().data.First().total.CompareTo(p2.population.First().data.First().total);
                        });
                }
            }

            GetGeocodes();
            ReOrderThesaban();
        }

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

            OnProcessingFinished(new EventArgs());
        }

        public IEnumerable<Entity> EntitiesWithInvalidGeocode()
        {
            return Data.InvalidGeocodeEntries();
        }

        private IEnumerable<Tuple<Entity, UInt32>> EntitiesWithoutGeocode(Entity parent)
        {
            var result = new List<Tuple<Entity, UInt32>>();
            var allEntitiesWithoutGeocode = parent.entity.Where(x => x.geocode == 0);
            foreach ( var x in allEntitiesWithoutGeocode )
            {
                result.Add(Tuple.Create(x, parent.geocode));
            }
            foreach ( var subEntity in parent.entity )
            {
                result.AddRange(EntitiesWithoutGeocode(subEntity));
            }

            return result;
        }

        public IEnumerable<Tuple<Entity, UInt32>> EntitiesWithoutGeocode()
        {
            return EntitiesWithoutGeocode(Data);
        }

        #endregion methods
    }
}