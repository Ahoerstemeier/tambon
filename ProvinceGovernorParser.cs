using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class ProvinceGovernorParser
    {
        private Dictionary<String, List<EntityLeader>> mData = null;
        private const String mChangwatStart = ">จังหวัด";
        private const String mGovernorStart = ">ผู้ว่าราชการจังหวัด";
        private const String mViceGovernorStart = ">รองผู้ว่าราชการจังหวัด";
        private const String mTelephoneStart = " โทร ";
        private const String mMobileStart = "มือถือ";
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
        protected List<EntityLeader> ParseLeaders(String iValue)
        {
            List<EntityLeader> lResult = new List<EntityLeader>();
            Int32 lPos3 = iValue.IndexOf(mGovernorStart) + mGovernorStart.Length;
            Int32 lPos4 = iValue.IndexOf(mViceGovernorStart) + mViceGovernorStart.Length;
            lResult.AddRange(ParseNames(iValue.Substring(lPos3, lPos4 - lPos3 - mViceGovernorStart.Length), EntityLeaderType.Governor));
            lResult.AddRange(ParseNames(iValue.Substring(lPos4), EntityLeaderType.ViceGovernor));
            return lResult;
        }
        public void Parse(Stream iStream)
        {
            Dictionary<String, List<EntityLeader>> lResult = new Dictionary<String, List<EntityLeader>>();
            String lCurrentLine = String.Empty;
            var lReader = new StreamReader(iStream, Helper.ThaiEncoding);
            StringBuilder lEntryData = new StringBuilder();
            String lCurrentChangwat = String.Empty;
            String lCurrentData = String.Empty;
            while ((lCurrentLine = lReader.ReadLine()) != null)
            {
                String lLine = lCurrentLine.Replace("&nbsp;", " ");
                if (lLine.Contains(mChangwatStart))
                {
                    Int32 lPos1 = lLine.IndexOf(mChangwatStart) + mChangwatStart.Length;
                    Int32 lPos2 = lLine.IndexOf("<", lPos1);
                    if (!String.IsNullOrEmpty(lCurrentChangwat))
                    {
                        lCurrentData = lCurrentData + lLine.Substring(0, lPos1 - mChangwatStart.Length);
                        lResult.Add(lCurrentChangwat, ParseLeaders(lCurrentData));
                    }
                    lCurrentChangwat = lLine.Substring(lPos1, lPos2 - lPos1);
                    lLine = lLine.Substring(lPos2);
                    lCurrentData = lLine;
                }
                else if (!String.IsNullOrEmpty(lCurrentChangwat))
                {
                    lCurrentData = lCurrentData + lLine;
                }
            }
            if (!String.IsNullOrEmpty(lCurrentChangwat))
            {
                lResult.Add(lCurrentChangwat, ParseLeaders(lCurrentData));
            }
            mData = lResult;
        }
        public void ExportToXML(XmlNode iNode)
        {
            XmlDocument lXmlDocument = Helper.XmlDocumentFromNode(iNode);
            var lNode = (XmlElement)lXmlDocument.CreateNode("element", "governors", "");
            iNode.AppendChild(lNode);
            foreach (KeyValuePair<String, List<EntityLeader>> lEntry in mData)
            {
                var lNodeProvince = (XmlElement)lXmlDocument.CreateNode("element", "province", "");

                lNodeProvince.SetAttribute("geocode", Helper.GetGeocode(lEntry.Key).ToString());
                lNodeProvince.SetAttribute("name", lEntry.Key);
                lNode.AppendChild(lNodeProvince);

                var lNodeOfficials = (XmlElement)lXmlDocument.CreateNode("element", "officials", "");
                foreach (EntityLeader lEntityLeader in lEntry.Value)
                {
                    lEntityLeader.ExportToXML(lNodeOfficials);
                }
                lNodeProvince.AppendChild(lNodeOfficials);
            }
        }
        private List<EntityLeader> ParseNames(String iLine, EntityLeaderType iPosition)
        {
            List<EntityLeader> lResult = new List<EntityLeader>();
            // to split the string into lines with one leader data
            String lLine = iLine.Replace("<BR>", "\n").Replace("</P>", "\n");
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
    }
}
