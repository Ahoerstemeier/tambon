using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    public class AmphoeComData
    {
        // TODO: DistrictOffice String -> ThaiAddress object
        #region properties
        public String AmphoeName { get; set; }
        public String ChangwatName { get; set; }
        public String ChangwatSlogan { get; set; }
        public String AmphoeSlogan { get; set; }
        public String DistrictOffice { get; set; }
        public Int32 ProvinceID { get; set;}
        public Int32 AmphoeID { get; set; }
        public String Telephone { get; set; }
        public String Fax { get; set; }
        public String Website { get; set; }
        public String History { get; set; }
        public String Area { get; set; }
        public String Climate { get; set; }
        public Int32 Tambon { get; set; }
        public Int32 Muban { get; set; }
        public Int32 Thesaban { get; set; }
        public Int32 TAO { get; set; }
        #endregion
        public Int32 Geocode()
        {
            Int32 iAmphoeID = TambonHelper.GetGeocode(ChangwatName, AmphoeName, EntityType.Amphoe);
            return iAmphoeID;
        }
        public void ExportToXML(XmlNode iNode)
        {
            XmlDocument lXmlDocument = TambonHelper.XmlDocumentFromNode(iNode);
            var lNewElement = (XmlElement)lXmlDocument.CreateNode("element", "entity", "");

            lNewElement.SetAttribute("geocode", Geocode().ToString());
            lNewElement.SetAttribute("name", AmphoeName);
            iNode.AppendChild(lNewElement);

            var lNodeAmphoeCom = (XmlElement)lXmlDocument.CreateNode("element", "amphoe.com", "");
            lNodeAmphoeCom.SetAttribute("amphoe", AmphoeID.ToString());
            lNodeAmphoeCom.SetAttribute("changwat", ProvinceID.ToString());
            lNewElement.AppendChild(lNodeAmphoeCom);

            var lNodeSlogan = (XmlElement)lXmlDocument.CreateNode("element", "slogan", "");
            lNodeSlogan.InnerText = AmphoeSlogan;
            lNewElement.AppendChild(lNodeSlogan);

            var lNodeAddress = (XmlElement)lXmlDocument.CreateNode("element", "address", "");
            lNodeAddress.InnerText = DistrictOffice;
            lNewElement.AppendChild(lNodeAddress);

            var lNodeHistory = (XmlElement)lXmlDocument.CreateNode("element", "history", "");
            lNodeHistory.InnerText = History;
            lNewElement.AppendChild(lNodeHistory);

            var lNodeClimate = (XmlElement)lXmlDocument.CreateNode("element", "climate", "");
            lNodeClimate.InnerText = Climate;
            lNewElement.AppendChild(lNodeClimate);

            var lNodeArea = (XmlElement)lXmlDocument.CreateNode("element", "area", "");
            lNodeArea.InnerText = Area;
            lNewElement.AppendChild(lNodeArea);

            var lNodeUrl = (XmlElement)lXmlDocument.CreateNode("element", "website", "");
            lNodeUrl.InnerText = Website;
            lNewElement.AppendChild(lNodeUrl);

            var lNodeTelephone = (XmlElement)lXmlDocument.CreateNode("element", "telephone", "");
            lNodeTelephone.InnerText = Telephone;
            lNewElement.AppendChild(lNodeTelephone);

            var lNodeFax = (XmlElement)lXmlDocument.CreateNode("element", "fax", "");
            lNodeFax.InnerText = Fax;
            lNewElement.AppendChild(lNodeFax);

            var lNodeSubEntities = (XmlElement)lXmlDocument.CreateNode("element", "subdivision", "");
            lNodeSubEntities.SetAttribute("tambon", Tambon.ToString());
            lNodeSubEntities.SetAttribute("thesaban", Thesaban.ToString());
            lNodeSubEntities.SetAttribute("muban", Muban.ToString());
            lNodeSubEntities.SetAttribute("TAO", TAO.ToString());
            lNewElement.AppendChild(lNodeSubEntities);
        }
    }
}
