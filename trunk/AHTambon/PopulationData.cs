using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Linq;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Event arguments containing a <see cref="PopulationData"/>
    /// </summary>
    public class PopulationDataEventArgs:EventArgs
    {
        /// <summary>
        /// Gets the population data.
        /// </summary>
        /// <value>The population data.</value>
        public PopulationData PopulationData
        { get; private set; }
        /// <summary>
        /// Creates a new instance of <see cref="PopulationDataEventArgs"/>.
        /// </summary>
        /// <param name="data">Population data.</param>
        public PopulationDataEventArgs(PopulationData data)
        {
            PopulationData = data;
        }
    }
    /// <summary>
    /// Delegate 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ProcessingFinishedHandler(Object sender, PopulationDataEventArgs e);

    public class PopulationData
    {
        #region variables
        private Boolean _anythingCached = false;
        private Int32 _geocode = 0;
        private PopulationDataEntry _changwat = null;
        private PopulationDataEntry _currentSubEntry = null;
        private List<PopulationDataEntry> _thesaban = new List<PopulationDataEntry>();
        private List<PopulationDataEntry> _invalidGeocodes = new List<PopulationDataEntry>();
        #endregion
        #region properties
        /// <summary>
        /// Gets the year for which is population data is done.
        /// </summary>
        public Int32 Year
        {
            get; private set;
        }
        /// <summary>
        /// Gets the actual population data.
        /// </summary>
        /// <value>The population data.</value>
        public PopulationDataEntry Data
        {
            get { return _changwat; }
        }
        /// <summary>
        /// Gets the list of municipalities.
        /// </summary>
        /// <value>The municipalities.</value>
        public IEnumerable<PopulationDataEntry> Thesaban
        {
            get { return _thesaban; }
        }
        /// <summary>
        /// Thrown when the processing is finished and the data is calculated.
        /// </summary>
        public event ProcessingFinishedHandler ProcessingFinished;
        #endregion
        #region constructor
        public PopulationData(Int32 year, Int32 geocode)
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
        private PopulationData()
        {
        }
        /// <summary>
        /// Creates a new instance of <see cref="PopulationData"/> from a <see cref="PopulationDataEntry"/>.
        /// </summary>
        /// <param name="entry">Population data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="entry"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="entry"/> is not the data of a province-like entity.</exception>
        public PopulationData(PopulationDataEntry entry)
        {
            if ( entry == null )
            {
                throw new ArgumentNullException("entry");
            }
            if ( !EntityTypeHelper.IsCompatibleEntityType(entry.Type, EntityType.Changwat) )
            {
                throw new ArgumentException("entry", String.Format("{0} is an invalid type of base entry",entry.Type));
            }
            _geocode = entry.Geocode;
            _changwat = entry;
        }
        #endregion
        #region constants
        private const String _tableEntryStart = "<td bgcolor=#fff9a4><font color=3617d2>";
        private const String _tableDataStart = "<td bgcolor=#ffffcb align=right><font color=0000ff>";
        private const String _tableBoldStart = "<b>";
        private const String _tableBoldEnd = "</b>";
        private const String _tableEntryEnd = "</font></td>";
        // private const String _urlBase = "http://www.dopa.go.th/xstat/";
        // private const String _urlBase = "http://203.113.86.149/xstat/";
        private const String _urlBase = "http://stat.dopa.go.th/xstat/";

        #endregion

        #region methods
        private void Download(String filename)
        {
            Stream outputStream = null;
            try
            {
                WebClient webClient = new System.Net.WebClient();
                Stream inputStream = webClient.OpenRead(_urlBase + filename);

                outputStream = new FileStream(HtmlCacheFileName(filename), FileMode.CreateNew);
                TambonHelper.StreamCopy(inputStream, outputStream);
                outputStream.Flush();
            }
            finally
            {
                outputStream.Dispose();
            }
        }
        private String HtmlCacheFileName(String fileName)
        {
            String directory = Path.Combine(GlobalSettings.HTMLCacheDir, "DOPA");
            Directory.CreateDirectory(directory);
            String result = Path.Combine(directory, fileName);
            return result;
        }
        public String WikipediaReference()
        {
            StringBuilder lBuilder = new StringBuilder();
            lBuilder.Append("<ref>{{cite web|url=");
            lBuilder.Append(_urlBase + SourceFilename(1));
            lBuilder.Append("|publisher=Department of Provincial Administration");
            lBuilder.Append("|title=Population statistics ");
            lBuilder.Append(Year.ToString());
            lBuilder.Append("}}</ref>");
            String lResult = lBuilder.ToString();
            return lResult;
        }

        private Boolean IsCached(String fileName)
        {
            return File.Exists(HtmlCacheFileName(fileName));
        }
        private void ParseSingleFile(String filename)
        {
            if ( !_anythingCached )
            {
                if ( !IsCached(filename) )
                {
                    Download(filename);
                }
                else
                {
                    _anythingCached = true;
                }
            }
            var lReader = new System.IO.StreamReader(HtmlCacheFileName(filename), TambonHelper.ThaiEncoding);

            String lCurrentLine = String.Empty;
            PopulationDataEntry lCurrentEntry = null;
            int lDataState = 0;
            while ( (lCurrentLine = lReader.ReadLine()) != null )
            {
                #region parse name
                if ( lCurrentLine.StartsWith(_tableEntryStart) )
                {
                    String lValue = StripTableHtmlFromLine(lCurrentLine);
                    lCurrentEntry = new PopulationDataEntry();
                    lCurrentEntry.ParseName(lValue);
                    if ( _changwat == null )
                    {
                        _changwat = lCurrentEntry;
                    }
                    else if ( (_currentSubEntry == null) | (EntityTypeHelper.SecondLevelEntity.Contains(lCurrentEntry.Type)) )
                    {
                        _changwat.SubEntities.Add(lCurrentEntry);
                        _currentSubEntry = lCurrentEntry;
                    }
                    else
                    {
                        _currentSubEntry.SubEntities.Add(lCurrentEntry);
                    }
                    lDataState = 0;
                }
                #endregion
                #region parse population data
                if ( lCurrentLine.StartsWith(_tableDataStart) )
                {
                    if ( lCurrentEntry == null )
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    String lValue = StripTableHtmlFromLine(lCurrentLine);
                    if ( !String.IsNullOrEmpty(lValue) )
                    {
                        switch ( lDataState )
                        {
                            case 0:
                                lCurrentEntry.Male = Int32.Parse(lValue);
                                break;
                            case 1:
                                lCurrentEntry.Female = Int32.Parse(lValue);
                                break;
                            case 2:
                                lCurrentEntry.Total = Int32.Parse(lValue);
                                break;
                            case 3:
                                lCurrentEntry.Households = Int32.Parse(lValue);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    lDataState++;
                }
                #endregion

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
            StringBuilder builder = new StringBuilder();
            builder.Append('p');
            builder.Append(yearShort.ToString("D2"));
            builder.Append(_geocode.ToString("D2"));
            builder.Append('_');
            builder.Append(page.ToString("D2"));
            builder.Append(".html");
            return builder.ToString();
        }
        protected void GetData()
        {
            Int16 count = 0;
            try
            {
                while ( count < 99 )
                {
                    count++;
                    ParseSingleFile(SourceFilename(count));
                }
            }
            catch
            {
                // TODO: catch selectively the exception expected for HTTP not found/file not found
            }
            _currentSubEntry = null;
            if ( _changwat != null )
            {
                _changwat.Geocode = _geocode;
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
            StreamReader reader = null;
            XmlDocument xmlDoc = null;
            PopulationData result = null;
            try
            {
                if ( !String.IsNullOrEmpty(fromFile) && File.Exists(fromFile) )
                {
                    reader = new StreamReader(fromFile);
                    xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(reader.ReadToEnd());
                    result = PopulationData.Load(xmlDoc);
                }
            }
            finally
            {
                if ( reader != null )
                {
                    reader.Close();
                }
            }
            return result;
        }
        protected void DoLoad(XmlNode node)
        {
            if ( node.Name == "entity" )
            {
                _changwat = PopulationDataEntry.Load(node);
            }
            else if ( node.Name == "thesaban" )
            {
                LoadXmlThesaban(node);
            }
        }
        public static PopulationData Load(XmlNode node)
        {
            PopulationData result = null;

            if ( node != null )
            {
                result = new PopulationData();
                foreach ( XmlNode childNode in node.ChildNodes )
                {
                    if ( childNode.Name == "year" )
                    {
                        result.Year = Convert.ToInt32(childNode.Attributes.GetNamedItem("value"));
                        foreach ( XmlNode llNode in childNode.ChildNodes )
                            result.DoLoad(llNode);
                    }
                    else
                    {
                        result.DoLoad(childNode);
                    }
                }
            }
            if ( result._changwat != null )
            {
                result._geocode = result._changwat.Geocode;
            }
            return result;
        }

        private void LoadXmlThesaban(XmlNode node)
        {
            foreach ( XmlNode childNode in node.ChildNodes )
                if ( childNode.Name == "entity" )
                {
                    _thesaban.Add(PopulationDataEntry.Load(childNode));
                }
        }
        public void ExportToXML(XmlNode node)
        {
            XmlDocument xmlDocument = TambonHelper.XmlDocumentFromNode(node);
            var nodeYear = (XmlElement)xmlDocument.CreateNode("element", "year", "");
            node.AppendChild(nodeYear);
            nodeYear.SetAttribute("value", Year.ToString());
            Data.ExportToXML(nodeYear);
            var nodeThesaban = (XmlElement)xmlDocument.CreateNode("element", "thesaban", "");
            nodeYear.AppendChild(nodeThesaban);
            foreach ( PopulationDataEntry entity in _thesaban )
            {
                entity.ExportToXML(nodeThesaban);
            }
        }

        protected void GetGeocodes()
        {
            if ( _changwat != null )
            {
                PopulationData geocodes = TambonHelper.GetGeocodeList(_changwat.Geocode);
                _invalidGeocodes = geocodes.Data.InvalidGeocodeEntries();
                Data.GetCodes(geocodes.Data);
            }
        }
        public String XmlExportFileName()
        {
            String result = String.Empty;
            if ( _changwat != null )
            {
                String outputDirectory = Path.Combine(GlobalSettings.XMLOutputDir, "DOPA");
                Directory.CreateDirectory(outputDirectory);
                result = Path.Combine(outputDirectory, "population" + _changwat.Geocode.ToString("D2") + " " + Year.ToString() + ".XML");
            }
            return result;
        }
        public void SaveXml()
        {
            XmlDocument xmlDocument = new XmlDocument();
            ExportToXML(xmlDocument);
            xmlDocument.Save(XmlExportFileName());
        }

        public void ReOrderThesaban()
        {
            if ( _changwat != null )
            {
                foreach ( PopulationDataEntry entity in _changwat.SubEntities )
                {
                    if ( entity != null )
                    {
                        if ( EntityTypeHelper.Thesaban.Contains(entity.Type) | EntityTypeHelper.Sakha.Contains(entity.Type) )
                        {
                            _thesaban.Add(entity);
                        }
                    }
                }
                foreach ( PopulationDataEntry thesaban in _thesaban )
                {
                    _changwat.SubEntities.Remove(thesaban);
                }
                foreach ( PopulationDataEntry thesaban in _thesaban )
                {
                    if ( thesaban.SubEntities.Any() )
                    {
                        foreach ( PopulationDataEntry tambon in thesaban.SubEntities )
                        {
                            _changwat.AddTambonInThesabanToAmphoe(tambon, thesaban);
                        }
                    }
                }
                foreach ( PopulationDataEntry entity in _changwat.SubEntities )
                {
                    if ( entity != null )
                    {
                        entity.SortSubEntitiesByGeocode();
                    }
                }
            }
        }

        private void ProcessAllProvinces()
        {
            PopulationDataEntry data = new PopulationDataEntry();
            foreach ( PopulationDataEntry entry in TambonHelper.ProvinceGeocodes )
            {
                PopulationData tempCalculator = new PopulationData(Year, entry.Geocode);
                tempCalculator.Process();
                data.SubEntities.Add(tempCalculator.Data);
            }
            data.CalculateNumbersFromSubEntities();
            _changwat = data;
        }
        private void ProcessProvince(Int32 geocode)
        {
            GetData();
            // sort Tambon by Population, to avoid double entries in 2012 data to create big mistakes
            if (Data != null)
            {
                foreach (var amphoe in Data.SubEntities)
                {
                    amphoe.SubEntities.Sort(delegate(PopulationDataEntry p1, PopulationDataEntry p2) { return -p1.Total.CompareTo(p2.Total); });
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

            OnProcessingFinished(new PopulationDataEventArgs(this));
        }

        private void OnProcessingFinished(PopulationDataEventArgs e)
        {
            if ( ProcessingFinished != null )
            {
                ProcessingFinished(this, e);
            }
        }

        public List<PopulationDataEntry> EntitiesWithInvalidGeocode()
        {
            return _invalidGeocodes;
        }
        public List<Tuple<PopulationDataEntry,Int32>> EntitiesWithoutGeocode()
        {
            var result = new List<Tuple<PopulationDataEntry, Int32>>();
            if ( _changwat != null )
            {
                PopulationDataEntry.PopulationDataEntryEvent addToResult = delegate(PopulationDataEntry value, PopulationDataEntry parent)
                             {
                                 if (parent == null)
                                 {
                                     result.Add(Tuple.Create(value, 0));
                                 }
                                 else
                                 {
                                     result.Add(Tuple.Create(value, parent.Geocode));
                                 }
                             };
                _changwat.IterateEntitiesWithoutGeocode(addToResult,_changwat);
                foreach ( PopulationDataEntry thesaban in _thesaban )
                {
                    thesaban.IterateEntitiesWithoutGeocode(addToResult,_changwat);
                }
            }
            return result;
        }
        #endregion

    }
}
