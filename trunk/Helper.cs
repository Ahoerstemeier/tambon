using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Globalization;

namespace De.AHoerstemeier.Tambon
{
    internal class Helper
    {

        #region constants
        private const string PHOSO = "พ.ศ.";
        #endregion
        static public RoyalGazetteList GlobalGazetteList = new RoyalGazetteList();
        static public List<PopulationDataEntry> Geocodes = new List<PopulationDataEntry>();
        static public Encoding ThaiEncoding = Encoding.GetEncoding(874);
        static public CultureInfo CultureInfoUS = new CultureInfo("en-us");
        public static Dictionary<EntityType, String> EntityNames = new Dictionary<EntityType, string>()
            {
              {EntityType.Changwat, "จังหวัด"},
              {EntityType.Amphoe, "อำเภอ" },
              {EntityType.Tambon, "ตำบล"},
              {EntityType.Thesaban, "เทศบาล"},
              {EntityType.ThesabanNakhon, "เทศบาลนคร"},
              {EntityType.ThesabanMueang, "เทศบาลเมือง"},
              {EntityType.ThesabanTambon, "เทศบาลตำบล"},
              {EntityType.Mueang, "เมือง"},
              {EntityType.SakhaTambon, "สาขาตำบล"},
              {EntityType.SakhaKhwaeng, "สาขาแขวง"},
              {EntityType.Sakha, "สาขา"},
              {EntityType.KingAmphoe, "กิ่งอำเภอ"},
              {EntityType.Khet, "เขต"},
              {EntityType.Khwaeng, "แขวง"},
              {EntityType.Bangkok, "กรุงเทพมหานคร"},
              {EntityType.Monthon, "มณฑล"},
              {EntityType.Sukhaphiban, "สุขาภิบาล"},
              {EntityType.SukhaphibanTambon, "สุขาภิบาลตำบล"},
              {EntityType.SukhaphibanMueang, "สุขาภิบาลเมือง"},
              {EntityType.Chumchon, "ชุมชน"},
              {EntityType.TAO, "องค์การบริหารส่วนตำบล"},
              {EntityType.TC, "สภาตำบล"},
              {EntityType.Phak, "ภาค"}
            };
        internal static Dictionary<Char, Byte> ThaiNumerals = new Dictionary<char, byte> 
        {
            {'๐',0}, 
            {'๑',1}, 
            {'๒',2}, 
            {'๓',3}, 
            {'๔',4},
            {'๕',5}, 
            {'๖',6},
            {'๗',7},
            {'๘',8},
            {'๙',9}
        };
        public static Dictionary<String, Byte> ThaiMonthNames = new Dictionary<string, byte>
        {
            {"มกราคม",1},
            {"กุมภาพันธ์",2},
            {"มีนาคม",3},
            {"เมษายน",4},
            {"พฤษภาคม",5},
            {"มิถุนายน",6},
            {"กรกฎาคม",7},
            {"สิงหาคม",8},
            {"กันยายน",9},
            {"ตุลาคม",10},
            {"พฤศจิกายน",11},
            {"ธันวาคม",12}
        };
        private static List<EntityType> mEntityThesaban = new List<EntityType>() 
        { 
                EntityType.Thesaban, 
                EntityType.ThesabanNakhon, 
                EntityType.ThesabanMueang, 
                EntityType.ThesabanTambon, 
                EntityType.Mueang, 
                EntityType.Sukhaphiban,
                EntityType.TAO
        };
        static public List<EntityType> Thesaban
        {
            get { return mEntityThesaban; }
        }
        private static List<EntityType> mEntitySakha = new List<EntityType>() 
        { 
                EntityType.Sakha, 
                EntityType.SakhaTambon, 
                EntityType.SakhaKhwaeng 
        };
        static public List<EntityType> Sakha
        {
            get { return mEntitySakha; }
        }
        private static List<EntityType> mEntitySecondLevel = null;
        static public List<EntityType> SecondLevelEntity
        {
            get
            {
                if (mEntitySecondLevel == null)
                {
                    mEntitySecondLevel = CreateEntitySecondLevel();
                }
                return mEntitySecondLevel;
            }
        }
        static private List<EntityType> CreateEntitySecondLevel()
        {
            var retval = new List<EntityType>() 
            { 
                EntityType.Amphoe,
                EntityType.KingAmphoe,
                EntityType.Khet
            };
            retval.AddRange(Sakha);
            retval.AddRange(Thesaban);
            return retval;
        }
        public static EntityType ParseEntityType(string iValue)
        {
            EntityType retval = EntityType.Unknown;
            if (!String.IsNullOrEmpty(iValue))
            {
                foreach (KeyValuePair<EntityType, string> lKeyValuePair in EntityNames)
                {
                    if (iValue.StartsWith(lKeyValuePair.Value))
                    {
                        if (retval == EntityType.Unknown)
                        {
                            retval = lKeyValuePair.Key;
                        }
                        // special case - Sakha and SakhaTambon might make problems otherwise
                        else if (lKeyValuePair.Value.Length > EntityNames[retval].Length)
                        {
                            retval = lKeyValuePair.Key;
                        }
                    }

                }
            }
            if (retval == EntityType.Unknown)
            {
                retval = EntityType.Unknown;
            }
            return retval;
        }
        internal static Boolean IsCompatibleEntityType(EntityType iValue1, EntityType iValue2)
        {
            Boolean retval = false;
            switch (iValue1)
            {
                case EntityType.Bangkok:
                case EntityType.Changwat:
                    retval = (iValue2 == EntityType.Changwat) | (iValue2 == EntityType.Bangkok);
                    break;

                case EntityType.KingAmphoe:
                case EntityType.Khet:
                case EntityType.Sakha:
                case EntityType.Amphoe:
                    retval = (iValue2 == EntityType.Amphoe) | (iValue2 == EntityType.KingAmphoe) | (iValue2 == EntityType.Khet) | (iValue2 == EntityType.Sakha);
                    break;

                case EntityType.Khwaeng:
                case EntityType.Tambon:
                    retval = (iValue2 == EntityType.Khwaeng) | (iValue2 == EntityType.Tambon);
                    break;

                case EntityType.Thesaban:
                case EntityType.ThesabanNakhon:
                case EntityType.ThesabanMueang:
                case EntityType.ThesabanTambon:
                    retval = (iValue2 == EntityType.ThesabanMueang) | (iValue2 == EntityType.ThesabanNakhon) | (iValue2 == EntityType.ThesabanTambon) | (iValue2 == EntityType.Thesaban);
                    break;

                default:
                    retval = (iValue1 == iValue2);
                    break;
            }
            return retval;
        }

        // XML utilities
        public static XmlDocument XmlDocumentFromNode(XmlNode iNode)
        {
            XmlDocument retval = null;

            if (iNode is XmlDocument)
            {
                retval = (XmlDocument)iNode;
            }
            else
            {
                retval = iNode.OwnerDocument;
            }

            return retval;
        }
        public static Boolean HasAttribute(XmlNode iNode, String iAttributeName)
        {
            Boolean retval = false;
            if (iNode != null && iNode.Attributes != null)
            {
                foreach (XmlAttribute i in iNode.Attributes)
                {
                    retval = retval | (i.Name == iAttributeName);
                }
            }
            return retval;
        }
        public static String GetAttribute(XmlNode iNode, String iAttributeName)
        {
            String RetVal = String.Empty;
            if (iNode != null && iNode.Attributes != null && (iNode.Attributes.Count > 0) && !String.IsNullOrEmpty(iAttributeName))
            {
                RetVal = iNode.Attributes.GetNamedItem(iAttributeName).Value;
            }
            return RetVal;
        }
        public static String GetAttributeOptionalString(XmlNode iNode, String iAttributeName)
        {
            String RetVal = String.Empty;
            if (HasAttribute(iNode, iAttributeName))
            {
                RetVal = iNode.Attributes.GetNamedItem(iAttributeName).Value;
            }
            return RetVal;
        }
        public static Int32 GetAttributeOptionalInt(XmlNode iNode, String iAttributeName, Int32 iReplace)
        {
            Int32 RetVal = iReplace;
            if (HasAttribute(iNode, iAttributeName))
            {
                string s = iNode.Attributes.GetNamedItem(iAttributeName).Value;
                if (!String.IsNullOrEmpty(s))
                {
                    try
                    {
                        RetVal = Convert.ToInt32(s);
                    }
                    catch
                    {
                    }
                }
            }

            return RetVal;
        }
        public static DateTime GetAttributeOptionalDateTime(XmlNode iNode, String iAttributeName)
        {
            DateTime RetVal = new DateTime();
            if (HasAttribute(iNode, iAttributeName))
            {
                RetVal = Convert.ToDateTime(iNode.Attributes.GetNamedItem(iAttributeName).Value);
            }
            return RetVal;
        }
        internal static DateTime GetAttributeDateTime(XmlNode iNode, String iAttributeName)
        {
            DateTime RetVal = Convert.ToDateTime(iNode.Attributes.GetNamedItem(iAttributeName).Value);
            return RetVal;
        }

        public static void StreamCopy(Stream iInput, Stream ioOutput)
        {
            byte[] lBuffer = new byte[2048];
            int lRead = 0;

            do
            {
                lRead = iInput.Read(lBuffer, 0, lBuffer.Length);
                ioOutput.Write(lBuffer, 0, lRead);
            } while (lRead > 0);
        }
        public static bool IsNumeric(String iValue)
        {
            for (int i = 0; i < iValue.Length; i++)
            {
                if (!(Convert.ToInt32(iValue[i]) >= 48 && Convert.ToInt32(iValue[i]) <= 57))
                {
                    return false;
                }
            }
            return true;
        }

        internal static string ReplaceThaiNumerals(string iValue)
        {
            string RetVal = String.Empty;

            if (!String.IsNullOrEmpty(iValue))
            {
                foreach (char c in iValue)
                {
                    if (ThaiNumerals.ContainsKey(c))
                    {
                        RetVal = RetVal + ThaiNumerals[c].ToString();
                    }
                    else
                    {
                        RetVal = RetVal + c;
                    }
                }
            }
            return RetVal;
        }
        internal static string UseThaiNumerals(string iValue)
        {
            string RetVal = String.Empty;

            if (!String.IsNullOrEmpty(iValue))
            {
                foreach (Char c in iValue)
                {
                    if ((c >= '0') | (c <= '9'))
                    {
                        Int32 lValue = Convert.ToInt32(c) - Convert.ToInt32('0');
                        foreach (KeyValuePair<Char, Byte> lKeyValuePair in ThaiNumerals)
                        {
                            if (lKeyValuePair.Value == lValue)
                            {
                                RetVal = RetVal + lKeyValuePair.Key;
                            }
                        }
                    }
                    else
                    {
                        RetVal = RetVal + c;
                    }
                }
            }
            return RetVal;
        }

        internal static DateTime ParseThaiDate(string iValue)
        {
            String lMonthString = String.Empty;
            Int32 lMonth = 0;
            String lYearString = String.Empty;
            Int32 lYear = 0;
            Int32 lDay = 0;
            Int32 lPosition = 0;

            String lDate = ReplaceThaiNumerals(iValue);

            lPosition = lDate.IndexOf(' ');
            lDay = Convert.ToInt32(lDate.Substring(0, lPosition));
            lDate = lDate.Substring(lPosition + 1, lDate.Length - lPosition - 1);
            lPosition = lDate.IndexOf(' ');
            lMonthString = lDate.Substring(0, lPosition).Trim();
            lMonth = ThaiMonthNames[lMonthString];
            // TODO: Kamen da nicht auch welche mit KhoSo vor?
            lPosition = lDate.IndexOf(PHOSO) + PHOSO.Length;
            lYearString = lDate.Substring(lPosition, lDate.Length - lPosition);
            lYear = Convert.ToInt32(lYearString);

            if ((lYear < 2484) & (lMonth < 4))
            {
                lYear = lYear - 542;
            }
            else
            {
                lYear = lYear - 543;
            }
            return new DateTime(lYear, lMonth, lDay); ;
        }

        internal static Boolean IsSameGeocode(Int32 iGeocodeToFind, Int32 iGeocodeToCheck, Boolean iIncludeSubEntities)
        {
            Boolean retval = false;
            if (iIncludeSubEntities)
            {
                Int32 lGeocode = iGeocodeToCheck;
                while (lGeocode != 0)
                {
                    retval = (retval | (iGeocodeToFind == lGeocode));
                    lGeocode = lGeocode / 100;
                }
            }
            else
            {
                retval = (iGeocodeToFind == iGeocodeToCheck);
            }
            return retval;
        }

        internal static Int64 GetDateJavaScript(DateTime iValue)
        {
            // milliseconds since January 1, 1970
            TimeSpan lDifference = iValue.ToUniversalTime() - new DateTime(1970, 1, 1);
            Int64 retval = Convert.ToInt64(lDifference.TotalMilliseconds);
            return retval;
        }

        internal static String OnlyNumbers(String iValue)
        {
            String retval = String.Empty;
            foreach (Char c in iValue)
            {
                if ((c >= '0') && (c <= '9'))
                {
                    retval = retval + c;
                }
            }
            return retval;
        }
        internal static Int32 GetGeocode(String iChangwat)
        {
            XElement lGeocodeXML = XElement.Load("geocode.xml");
            Int32 lProvinceID = 0;
            var lQuery = from c in lGeocodeXML.Descendants("entity")
                         where (String)c.Attribute("name") == iChangwat
                         select (Int32)c.Attribute("geocode");

            foreach (Int32 lEntry in lQuery)
            {
                lProvinceID = lEntry;
            }
            return lProvinceID;
        }
        internal static Int32 GetGeocode(String iChangwat, String iName, EntityType iType)
        {
            Int32 lProvinceID = GetGeocode(iChangwat);
            Int32 lGeocode = 0;
            if (lProvinceID != 0)
            {
                String lSearchName = iName;
                if (lSearchName.Contains(" "))
                {
                    lSearchName = lSearchName.Substring(0, lSearchName.IndexOf(" "));
                }
                XElement lChangwatXML = XElement.Load("geocode\\geocode" + lProvinceID.ToString() + ".xml");
                var lAmphoeQuery = from c in lChangwatXML.Descendants("entity")
                                   where (
                                     ((String)c.Attribute("name") == lSearchName) &&
                                     Helper.IsCompatibleEntityType(
                                       (EntityType)Enum.Parse(typeof(EntityType), (String)c.Attribute("type"))
                                       , iType))
                                   select (Int32)c.Attribute("geocode");

                foreach (Int32 lEntry in lAmphoeQuery)
                {
                    lGeocode = lEntry;
                }
            }
            return lGeocode;
        }
        static public void LoadGeocodeList()
        {
            XElement lGeocodeXML = XElement.Load("geocode.xml");

            var lQuery = from c in lGeocodeXML.Descendants("entity")
                         orderby (string)c.Attribute("english")
                         select new PopulationDataEntry
                         {
                             Name = (string)c.Attribute("name"),
                             English = (string)c.Attribute("english"),
                             Type = (EntityType)Enum.Parse(typeof(EntityType), (string)c.Attribute("type")),
                             Geocode = (Int32)c.Attribute("geocode")
                         };

            Geocodes.Clear();
            Geocodes.AddRange(lQuery);
        }
    }
}
