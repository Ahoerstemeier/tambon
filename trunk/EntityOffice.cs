using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class EntityOffice : ICloneable
    {
        #region variables
        private List<Uri> mWebsites = new List<Uri>();

        private static Dictionary<OfficeType, String> OfficeKmlStyles = new Dictionary<OfficeType, String>()
            {
              {OfficeType.ProvinceHall, "http://maps.google.com/mapfiles/kml/paddle/blu-blank.png"},
              {OfficeType.PAOOffice,    "http://maps.google.com/mapfiles/kml/paddle/red-blank.png"},
              {OfficeType.DistrictOffice, "http://maps.google.com/mapfiles/kml/paddle/grn-blank.png"},
              {OfficeType.MunicipalityOffice, "http://maps.google.com/mapfiles/kml/paddle/ylw-blank.png"},
              {OfficeType.TAOOffice, "http://maps.google.com/mapfiles/kml/paddle/wht-blank.png"},
              {OfficeType.VillageHeadmanOffice, "http://maps.google.com/mapfiles/kml/paddle/pink-blank.png"},

              {OfficeType.DistrictMuseum, "http://maps.google.com/mapfiles/kml/shapes/museum.png"}
            };
        private static Dictionary<OfficeType, String> OfficeNameEnglish = new Dictionary<OfficeType, String>()
            {
              {OfficeType.ProvinceHall, "Province hall"},
              {OfficeType.PAOOffice,    "PAO office"},
              {OfficeType.DistrictOffice, "District office"},
              {OfficeType.MunicipalityOffice, "Municipality office"},
              {OfficeType.TAOOffice, "TAO office"},
              {OfficeType.VillageHeadmanOffice, "Village headman office"},

              {OfficeType.DistrictMuseum,"District museum"}
            };

        #endregion

        #region properties
        public EntityLeaderList OfficialsList { get; set; }
        public ThaiAddress Address { get; set; }
        public List<Uri> Websites { get { return mWebsites; } }
        public OfficeType Type { get; set; }
        public GeoPoint Location { get; set; }
        #endregion

        #region constructor
        public EntityOffice()
        {
        }
        public EntityOffice(EntityOffice iValue)
        {
            if (iValue.OfficialsList != null)
            {
                OfficialsList = (EntityLeaderList)iValue.OfficialsList.Clone();
            }
            if (iValue.Location != null)
            {
                Location = (GeoPoint)iValue.Location.Clone();
            }
            if (iValue.Address != null)
            {
                Address = (ThaiAddress)iValue.Address.Clone();
            }
            foreach (Uri lUri in iValue.Websites)
            {
                Websites.Add(lUri);
            }
            Type = iValue.Type;
        }
        #endregion

        #region methods
        internal virtual void WriteToXMLNode(XmlElement iNode)
        {
            iNode.SetAttribute("type", Type.ToString());

            if (Location != null)
            {
                Location.ExportToXML(iNode);
            }
            if (Address != null)
            {
                Address.ExportToXML(iNode);
            }

            XmlDocument lXmlDocument = Helper.XmlDocumentFromNode(iNode);
            foreach (Uri lUri in Websites)
            {
                var lNewElement = (XmlElement)lXmlDocument.CreateNode("element", "url", "");
                lNewElement.InnerText = lUri.ToString();
                iNode.AppendChild(lNewElement); 
            }
            if (OfficialsList != null)
            {
                OfficialsList.ExportToXML(iNode);
            }
        }
        public void ExportToXML(XmlNode iNode)
        {
            XmlDocument lXmlDocument = Helper.XmlDocumentFromNode(iNode);
            var lNewElement = (XmlElement)lXmlDocument.CreateNode("element", "office", "");
            iNode.AppendChild(lNewElement);
            WriteToXMLNode(lNewElement);
        }
        internal static EntityOffice Load(XmlNode iNode)
        {
            EntityOffice RetVal = null;

            if (iNode != null && iNode.Name.Equals("office"))
            {
                RetVal = new EntityOffice();

                string s = Helper.GetAttributeOptionalString(iNode, "type");
                if (!String.IsNullOrEmpty(s))
                {
                    RetVal.Type = (OfficeType)Enum.Parse(typeof(OfficeType), s);
                }
                if (iNode.HasChildNodes)
                {
                    foreach (XmlNode lChildNode in iNode.ChildNodes)
                    {
                        if (lChildNode.Name == "officials")
                        {
                            EntityLeaderList lOfficials = EntityLeaderList.Load(lChildNode);
                            RetVal.OfficialsList=lOfficials;
                        }
                        if (lChildNode.Name == "url")
                        {
                            RetVal.Websites.Add(new Uri(lChildNode.InnerText));
                        }
                        if (lChildNode.Name == "address")
                        {
                            RetVal.Address = ThaiAddress.Load(lChildNode);
                        }
                        if (lChildNode.Name == "geo:Point")
                        {
                            RetVal.Location = GeoPoint.Load(lChildNode);
                        }
                    }
                }
            }
            return RetVal;

        }

        internal static void AddKmlStyles(KmlHelper iKml)
        {
            foreach (KeyValuePair<OfficeType, String> lKeyValuePair in OfficeKmlStyles)
            {
                iKml.AddIconStyle(lKeyValuePair.Key.ToString(), new Uri(lKeyValuePair.Value));
            }
        }
        internal void AddToKml(KmlHelper iKml, XmlNode iNode, String lEntityName, String iDescription)
        {
            if (Location != null)
            {
                String lName = OfficeNameEnglish[Type] + ' ' + lEntityName;
                String lAddress = String.Empty;
                if (Address != null)
                {
                    lAddress = Address.ToString();
                }
                // ToDo: for Amphoe also amphoe.com URL to description
                String lDescription = iDescription;
                foreach (Uri lUri in this.Websites)
                {
                    lDescription = lDescription + "\n" + lUri.ToString();
                }
                iKml.AddPoint(iNode, Location.Latitude, Location.Longitude, lName, Type.ToString(),lAddress,lDescription);
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new EntityOffice(this);
        }

        #endregion
    }
}
