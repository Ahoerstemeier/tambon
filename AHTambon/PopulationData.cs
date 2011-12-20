using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Linq;

namespace De.AHoerstemeier.Tambon
{
    public class PopulationData
    {
        public delegate void ProcessingFinished(PopulationData data);
        #region variables
        private Boolean mAnyCached = false;
        private Int32 mYear = 0;
        private Int32 mGeocode = 0;
        private PopulationDataEntry mChangwat = null;
        private PopulationDataEntry mCurrentSubEntry = null;
        private List<PopulationDataEntry> mThesaban = new List<PopulationDataEntry>();
        private List<PopulationDataEntry> mInvalidGeocodes = new List<PopulationDataEntry>();
        #endregion
        #region properties
        public Int32 year
        {
            get { return mYear; }
        }
        public PopulationDataEntry Data
        {
            get { return mChangwat; }
        }
        public List<PopulationDataEntry> Thesaban
        {
            get { return mThesaban; }
        }
        public event ProcessingFinished OnProcessingFinished;
        #endregion
        #region constructor
        public PopulationData(Int32 iYear, Int32 iGeocode)
        {
            mYear = iYear;
            mGeocode = iGeocode;
            Int32 lYearShort = mYear + 543 - 2500;
            if ((lYearShort < 0) | (lYearShort > 99))
            {
                throw new ArgumentOutOfRangeException();
            }
            if ((mGeocode < 0) | (mGeocode > 99))
            {
                throw new ArgumentOutOfRangeException();
            }
        }
        private PopulationData()
        {
        }
        public PopulationData(PopulationDataEntry iEntry)
        {
            if (iEntry == null)
            {
                throw new ArgumentNullException("iEntry");
            }
            if (!EntityTypeHelper.IsCompatibleEntityType(iEntry.Type, EntityType.Changwat))
            {
                throw new ArgumentOutOfRangeException("iEntry", iEntry.Type, "Invalid type of base entry");
            }
            mGeocode = iEntry.Geocode;
            mChangwat = iEntry;
        }
        #endregion
        #region constants
        private const String TableEntryStart = "<td bgcolor=#fff9a4><font color=3617d2>";
        private const String TableDataStart = "<td bgcolor=#ffffcb align=right><font color=0000ff>";
        private const String TableBoldStart = "<b>";
        private const String TableBoldEnd = "</b>";
        private const String TableEntryEnd = "</font></td>";
        //private const String UrlBase = "http://www.dopa.go.th/xstat/";
        private const String UrlBase = "http://203.113.86.149/xstat/";

        #endregion

        #region methods
        private void Download(String iFilename)
        {
            System.IO.Stream lOutputStream = null;
            try
            {
                System.Net.WebClient lWebClient = new System.Net.WebClient();
                System.IO.Stream lInputStream = lWebClient.OpenRead(UrlBase + iFilename);

                lOutputStream = new FileStream(HtmlCacheFileName(iFilename), FileMode.CreateNew);
                TambonHelper.StreamCopy(lInputStream, lOutputStream);
                lOutputStream.Flush();
            }
            finally
            {
                lOutputStream.Dispose();
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
            lBuilder.Append(UrlBase+SourceFilename(1));
            lBuilder.Append("|publisher=Department of Provincial Administration");
            lBuilder.Append("|title=Population statistics ");
            lBuilder.Append(mYear.ToString());
            lBuilder.Append("}}</ref>");
            String lResult = lBuilder.ToString();
            return lResult;
        }

        private Boolean isCached(String fileName)
        {
            return File.Exists(HtmlCacheFileName(fileName));
        }
        private void ParseSingleFile(String filename)
        {
            if (!mAnyCached)
            {
                if (!isCached(filename))
                {
                    Download(filename);
                }
                else
                {
                    mAnyCached = true;
                }
            }
            var lReader = new System.IO.StreamReader(HtmlCacheFileName(filename), TambonHelper.ThaiEncoding);

            String lCurrentLine = String.Empty;
            PopulationDataEntry lCurrentEntry = null;
            int lDataState = 0;
            while ((lCurrentLine = lReader.ReadLine()) != null)
            {
                #region parse name
                if (lCurrentLine.StartsWith(TableEntryStart))
                {
                    String lValue = StripTableHtmlFromLine(lCurrentLine);
                    lCurrentEntry = new PopulationDataEntry();
                    lCurrentEntry.ParseName(lValue);
                    if (mChangwat == null)
                    {
                        mChangwat = lCurrentEntry;
                    }
                    else if ((mCurrentSubEntry == null) | (EntityTypeHelper.SecondLevelEntity.Contains(lCurrentEntry.Type)))
                    {
                        mChangwat.SubEntities.Add(lCurrentEntry);
                        mCurrentSubEntry = lCurrentEntry;
                    }
                    else
                    {
                        mCurrentSubEntry.SubEntities.Add(lCurrentEntry);
                    }
                    lDataState = 0;
                }
                #endregion
                #region parse population data
                if (lCurrentLine.StartsWith(TableDataStart))
                {
                    if (lCurrentEntry == null)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    String lValue = StripTableHtmlFromLine(lCurrentLine);
                    if (!String.IsNullOrEmpty(lValue))
                    {
                        switch (lDataState)
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
        private String SourceFilename(Int16 iPage)
        {
            Int32 lYearShort = mYear + 543 - 2500;
            if ((lYearShort < 0) | (lYearShort > 99))
            {
                throw new ArgumentOutOfRangeException();
            }
            if ((mGeocode < 0) | (mGeocode > 99))
            {
                throw new ArgumentOutOfRangeException();
            }
            StringBuilder lBuilder = new StringBuilder();
            lBuilder.Append('p');
            lBuilder.Append(lYearShort.ToString("D2"));
            lBuilder.Append(mGeocode.ToString("D2"));
            lBuilder.Append('_');
            lBuilder.Append(iPage.ToString("D2"));
            lBuilder.Append(".html");
            return lBuilder.ToString();
        }
        protected void GetData()
        {
            Int16 lCount = 0;
            try
            {
                while (lCount < 99)
                {
                    lCount++;
                    ParseSingleFile(SourceFilename(lCount));
                }
            }
            catch
            {
                // TODO: catch selectively the exception expected for HTTP not found/file not found
            }
            mCurrentSubEntry = null;
            if (mChangwat != null)
            {
                mChangwat.Geocode = mGeocode;
            }

        }
        private static String StripTableHtmlFromLine(String iValue)
        {
            string result = iValue;
            result = result.Replace(TableDataStart, "");
            result = result.Replace(TableEntryStart, "");
            result = result.Replace(TableBoldStart, "");
            result = result.Replace(TableBoldEnd, "");
            result = result.Replace(TableEntryEnd, "");
            result = result.Replace(",", "");
            result = result.Trim();
            return result;
        }
        public static PopulationData Load(String iFromFile)
        {
            StreamReader lReader = null;
            XmlDocument lXmlDoc = null;
            PopulationData RetVal = null;
            try
            {
                if (!String.IsNullOrEmpty(iFromFile) && File.Exists(iFromFile))
                {
                    lReader = new StreamReader(iFromFile);
                    lXmlDoc = new XmlDocument();
                    lXmlDoc.LoadXml(lReader.ReadToEnd());
                    RetVal = PopulationData.Load(lXmlDoc);
                }
            }
            finally
            {
                if (lReader != null)
                {
                    lReader.Close();
                }
            }
            return RetVal;
        }
        protected void DoLoad(XmlNode iNode)
        {
            if (iNode.Name == "entity")
            {
                mChangwat = PopulationDataEntry.Load(iNode);
            }
            else if (iNode.Name == "thesaban")
            {
                LoadXMLThesaban(iNode);
            }
        }
        public static PopulationData Load(XmlNode iNode)
        {
            PopulationData RetVal = null;

            if (iNode != null)
            {
                RetVal = new PopulationData();
                foreach (XmlNode lNode in iNode.ChildNodes)
                {
                    if (lNode.Name == "year")
                    {
                        RetVal.mYear = Convert.ToInt32(lNode.Attributes.GetNamedItem("value"));
                        foreach (XmlNode llNode in lNode.ChildNodes)
                            RetVal.DoLoad(llNode);
                    }
                    else 
                    {
                        RetVal.DoLoad(lNode);
                    }
                }
            }
            if (RetVal.mChangwat != null)
            {
                RetVal.mGeocode = RetVal.mChangwat.Geocode;
            }
            return RetVal;
        }

        private void LoadXMLThesaban(XmlNode iNode)
        {
            foreach (XmlNode lNode in iNode.ChildNodes)
                if (lNode.Name == "entity")
                {
                    mThesaban.Add(PopulationDataEntry.Load(lNode));
                }
        }
        public void ExportToXML(XmlNode iNode)
        {
            XmlDocument lXmlDocument = TambonHelper.XmlDocumentFromNode(iNode);
            var lNode = (XmlElement)lXmlDocument.CreateNode("element", "year", "");
            iNode.AppendChild(lNode);
            lNode.SetAttribute("value", mYear.ToString());
            Data.ExportToXML(lNode);
            var lNodeThesaban = (XmlElement)lXmlDocument.CreateNode("element", "thesaban", "");
            lNode.AppendChild(lNodeThesaban);
            foreach (PopulationDataEntry entity in mThesaban)
            {
                entity.ExportToXML(lNodeThesaban);
            }
        }

        protected void GetGeocodes()
        {
            if (mChangwat != null)
            {
                PopulationData lGeocodes = TambonHelper.GetGeocodeList(mChangwat.Geocode);
                mInvalidGeocodes = lGeocodes.Data.InvalidGeocodeEntries();
                Data.GetCodes(lGeocodes.Data);
            }
        }
        public String XMLExportFileName()
        {
            String retval = String.Empty;
            if (mChangwat != null)
            {
                String lDir = Path.Combine(GlobalSettings.XMLOutputDir, "DOPA");
                Directory.CreateDirectory(lDir);
                retval = Path.Combine(lDir,"population" + mChangwat.Geocode.ToString("D2") + " " + year.ToString() + ".XML");
            }
            return retval;
        }
        public void SaveXML()
        {
            XmlDocument lXmlDocument = new XmlDocument();
            ExportToXML(lXmlDocument);
            lXmlDocument.Save(XMLExportFileName());
        }

        public void ReOrderThesaban()
        {
            if (mChangwat != null)
            {
                foreach (PopulationDataEntry lEntity in mChangwat.SubEntities)
                {
                    if ( lEntity != null )
                    {
                        if ( EntityTypeHelper.Thesaban.Contains(lEntity.Type) | EntityTypeHelper.Sakha.Contains(lEntity.Type) )
                        {
                            mThesaban.Add(lEntity);
                        }
                    }
                }
                foreach (PopulationDataEntry lThesaban in mThesaban)
                {
                    mChangwat.SubEntities.Remove(lThesaban);
                }
                foreach (PopulationDataEntry lThesaban in mThesaban)
                {
                    if (lThesaban.SubEntities.Any())
                    {
                        foreach (PopulationDataEntry lTambon in lThesaban.SubEntities)
                        {
                            mChangwat.AddTambonInThesabanToAmphoe(lTambon, lThesaban);
                        }
                    }
                }
                foreach (PopulationDataEntry entity in mChangwat.SubEntities)
                {
                    if ( entity != null )
                    {
                        entity.SortSubEntities();
                    }
                }
            }
        }

        private void ProcessAllProvinces()
        {
            PopulationDataEntry lData = new PopulationDataEntry();
            foreach (PopulationDataEntry lEntry in TambonHelper.ProvinceGeocodes)
            {
                PopulationData lTempCalculator = new PopulationData(mYear, lEntry.Geocode);
                lTempCalculator.Process();
                lData.SubEntities.Add(lTempCalculator.Data);
            }
            lData.CalculateNumbersFromSubEntities();
            mChangwat = lData;
        }
        private void ProcessProvince(Int32 iGeocode)
        {
            GetData();
            GetGeocodes();
            ReOrderThesaban();
        }

        public void Process()
        {
            if (mGeocode==0)
            {
                ProcessAllProvinces();
            }
            else
            {
                ProcessProvince(mGeocode);
            }

            if (OnProcessingFinished != null)
            {
                OnProcessingFinished(this);
            }
        }

        public List<PopulationDataEntry> EntitiesWithInvalidGeocode()
        {
            return mInvalidGeocodes;
        }
        public List<PopulationDataEntry> EntitiesWithoutGeocode()
        {
            var retval = new List<PopulationDataEntry>();
            if (mChangwat != null)
            {
                PopulationDataEntry.PopulationDataEntryEvent del = delegate(PopulationDataEntry value)
                             {
                                 retval.Add(value);
                             };
                mChangwat.IterateEntitiesWithoutGeocode(del);
                foreach (PopulationDataEntry thesaban in mThesaban)
                {
                    thesaban.IterateEntitiesWithoutGeocode(del);
                }
            }
            return retval;
        }
        #endregion

    }
}
