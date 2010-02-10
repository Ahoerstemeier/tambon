using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    public class ProvinceGovernorParser
    {
        #region variables
        private List<PopulationDataEntry> mData = null;
        private const String mChangwatStart = ">จังหวัด";
        private const String mGovernorStart = ">ผู้ว่าราชการจังหวัด";
        private const String mViceGovernorStart = ">รองผู้ว่าราชการจังหวัด";
        private const String mTelephoneStart = " โทร ";
        private const String mMobileStart = "มือถือ";
        private const String mPageEnd = "- BEGIN WEB STAT CODE -";
        #endregion
        #region constructor
        public ProvinceGovernorParser()
        { 
        }
        #endregion
        #region methods
        public void ParseUrl(String iUrl)
        {
            WebClient lWebClient = new WebClient();
            Stream mData = lWebClient.OpenRead(iUrl);
            Parse(mData);
        }
        public void ParseFile(String iFilename)
        {
            Stream mData = new FileStream(iFilename, FileMode.Open);
            Parse(mData);
        }
        protected EntityLeaderList ParseLeaders(String iValue)
        {
            EntityLeaderList lResult = new EntityLeaderList();
            Int32 lPos3 = iValue.IndexOf(mGovernorStart) + mGovernorStart.Length;
            Int32 lPos4 = iValue.IndexOf(mViceGovernorStart) + mViceGovernorStart.Length;
            lResult.AddRange(ParseNames(iValue.Substring(lPos3, lPos4 - lPos3 - mViceGovernorStart.Length), EntityLeaderType.Governor));
            lResult.AddRange(ParseNames(iValue.Substring(lPos4), EntityLeaderType.ViceGovernor));
            return lResult;
        }
        public void Parse(Stream iStream)
        {
            List<PopulationDataEntry> lResult = new List<PopulationDataEntry>();
            String lCurrentLine = String.Empty;
            var lReader = new StreamReader(iStream, TambonHelper.ThaiEncoding);
            StringBuilder lEntryData = new StringBuilder();
            String lCurrentChangwat = String.Empty;
            String lCurrentData = String.Empty;
            while ((lCurrentLine = lReader.ReadLine()) != null)
            {
                String lLine = lCurrentLine.Replace("&nbsp;", " ");
                while (lLine.Contains(mChangwatStart))
                {
                    Int32 lPos1 = lLine.IndexOf(mChangwatStart) + mChangwatStart.Length;
                    Int32 lPos2 = lLine.IndexOf("<", lPos1);
                    if (!String.IsNullOrEmpty(lCurrentChangwat))
                    {
                        lCurrentData = lCurrentData + Environment.NewLine + lLine.Substring(0, lPos1 - mChangwatStart.Length);
                        lResult.Add(new PopulationDataEntry(lCurrentChangwat, OfficeType.ProvinceHall, ParseLeaders(lCurrentData)));
                        lCurrentData = String.Empty;
                    }
                    lCurrentChangwat = lLine.Substring(lPos1, lPos2 - lPos1);
                    if (TambonHelper.ChangwatMisspellings.ContainsKey(lCurrentChangwat))
                    {
                        lCurrentChangwat = TambonHelper.ChangwatMisspellings[lCurrentChangwat];
                    }
                    lLine = lLine.Substring(lPos2);
                }
                if (lLine.Contains(mPageEnd))
                {
                    break;
                }
                if (!String.IsNullOrEmpty(lCurrentChangwat))
                {
                    lCurrentData = lCurrentData + Environment.NewLine + lLine;
                }
            }
            if (!String.IsNullOrEmpty(lCurrentChangwat))
            {
                lResult.Add(new PopulationDataEntry(lCurrentChangwat, OfficeType.ProvinceHall, ParseLeaders(lCurrentData)));
            }
            mData = lResult;
        }
        public void ExportToXML(XmlNode iNode)
        {
            XmlDocument lXmlDocument = TambonHelper.XmlDocumentFromNode(iNode);
            var lNode = (XmlElement)lXmlDocument.CreateNode("element", "governors", "");
            iNode.AppendChild(lNode);
            foreach (PopulationDataEntry lEntry in mData)
            {
                lEntry.ExportToXML(lNode);
            }
        }
        private List<EntityLeader> ParseNames(String iLine, EntityLeaderType iPosition)
        {
            List<EntityLeader> lResult = new List<EntityLeader>();
            // to split the string into lines with one leader data
            String lLine = iLine.Replace("<BR>", Environment.NewLine).Replace("</P>", Environment.NewLine);
            lLine = RemoveAllTags(lLine);
            StringReader lReader = new StringReader(lLine);
            String lCurrentLine = String.Empty;
            while ((lCurrentLine = lReader.ReadLine()) != null)
            {
                EntityLeader lCurrentEntry = new EntityLeader();
                lCurrentEntry.Position = iPosition;
                Int32 lPos1 = lCurrentLine.IndexOf(mTelephoneStart);
                Int32 lPos2 = lCurrentLine.IndexOf(mMobileStart);
                if (lPos2 >= 0)
                {
                    String lNumber = lCurrentLine.Substring(lPos2 + mMobileStart.Length).Trim();
                    // very last entry has text after the number
                    if (lNumber.Contains(' '))
                    {
                        lNumber = lNumber.Substring(0, lNumber.IndexOf(' '));
                    }
                    lCurrentEntry.CellPhone = lNumber;
                    lCurrentLine = lCurrentLine.Substring(0, lPos2);
                }
                if (lPos1 >= 0)
                {
                    lCurrentEntry.Telephone = lCurrentLine.Substring(lPos1 + mTelephoneStart.Length).Trim();
                    lCurrentLine = lCurrentLine.Substring(0, lPos1);
                }
                lCurrentEntry.Name = lCurrentLine.Trim();
                // The name may have more than whitespace in the middle
                while (lCurrentEntry.Name.Contains("  "))
                {
                    lCurrentEntry.Name = lCurrentEntry.Name.Replace("  ", " ");
                }
                if (!String.IsNullOrEmpty(lCurrentEntry.Name))
                {
                    lResult.Add(lCurrentEntry);
                }
            }
            return lResult;
        }

        private string RemoveAllTags(string iLine)
        {
            String lResult = String.Empty;
            String lLine = iLine;
            while (lLine.Contains('<'))
            {
                Int32 lPos1 = lLine.IndexOf('<');
                Int32 lPos2 = lLine.IndexOf('>');
                if (lPos1 > 0)
                {
                    lResult = lResult + lLine.Substring(0, lPos1);
                }
                if (lPos2 < 0)
                {
                    lLine = String.Empty;
                }
                else
                {
                    lLine = lLine.Substring(lPos2 + 1);
                }
            }
            lResult = lResult + lLine;
            return lResult;
        }
        public Dictionary<String, EntityLeader> NewGovernorsList()
        {
            Dictionary<String, EntityLeader> RetVal = new Dictionary<String, EntityLeader>();
            List<PopulationDataEntry> lFoundEntries = new List<PopulationDataEntry>();

            foreach (PopulationDataEntry lEntry in mData)
            { 
                lEntry.Geocode = TambonHelper.GetGeocode(lEntry.Name);
                String lFilename = TambonHelper.GeocodeSourceFile(lEntry.Geocode);
                PopulationData lData = PopulationData.Load(lFilename);
                lEntry.English = lData.Data.English;
                foreach (EntityOffice lOffice in lEntry.Offices)
                {
                    foreach (EntityLeader lLeader in lOffice.OfficialsList)
                    {
                        if (lLeader.Position == EntityLeaderType.Governor)
                        {
                            if (lData.Data.LeaderAlreadyInList(lLeader))
                            {
                                lFoundEntries.Add(lEntry);
                            }
                            else
                            {
                                RetVal.Add(lEntry.English,lLeader);
                            }
                        }
                    }
                }
            }
            foreach (PopulationDataEntry lEntry in lFoundEntries)
            {
                mData.Remove(lEntry);
            }
            return RetVal;
        }
        public String NewGovernorsText()
        { 
            StringBuilder lBuilder = new StringBuilder();
            foreach (KeyValuePair<String,EntityLeader> lEntry in NewGovernorsList())
            {
                lBuilder.AppendLine(lEntry.Key+" "+lEntry.Value.Name);
            }
            return lBuilder.ToString();
        }

        #endregion

    }
}
