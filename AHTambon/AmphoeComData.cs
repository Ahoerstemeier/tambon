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

        public String AmphoeName
        {
            get;
            set;
        }

        public String ChangwatName
        {
            get;
            set;
        }

        public String ChangwatSlogan
        {
            get;
            set;
        }

        public String AmphoeSlogan
        {
            get;
            set;
        }

        public String DistrictOffice
        {
            get;
            set;
        }

        public Int32 ProvinceID
        {
            get;
            set;
        }

        public Int32 AmphoeID
        {
            get;
            set;
        }

        public String Telephone
        {
            get;
            set;
        }

        public String Fax
        {
            get;
            set;
        }

        public String Website
        {
            get;
            set;
        }

        public String History
        {
            get;
            set;
        }

        public String Area
        {
            get;
            set;
        }

        public String Climate
        {
            get;
            set;
        }

        public Int32 Tambon
        {
            get;
            set;
        }

        public Int32 Muban
        {
            get;
            set;
        }

        public Int32 Thesaban
        {
            get;
            set;
        }

        public Int32 TAO
        {
            get;
            set;
        }

        #endregion properties

        public Int32 Geocode()
        {
            Int32 amphoeID = TambonHelper.GetGeocode(ChangwatName, AmphoeName, EntityType.Amphoe);
            return amphoeID;
        }

        public void ExportToXML(XmlNode node)
        {
            XmlDocument xmlDocument = TambonHelper.XmlDocumentFromNode(node);
            var newElement = (XmlElement)xmlDocument.CreateNode("element", "entity", "");

            newElement.SetAttribute("geocode", Geocode().ToString());
            newElement.SetAttribute("name", AmphoeName);
            node.AppendChild(newElement);

            var nodeAmphoeCom = (XmlElement)xmlDocument.CreateNode("element", "amphoe.com", "");
            nodeAmphoeCom.SetAttribute("amphoe", AmphoeID.ToString());
            nodeAmphoeCom.SetAttribute("changwat", ProvinceID.ToString());
            newElement.AppendChild(nodeAmphoeCom);

            var nodeSlogan = (XmlElement)xmlDocument.CreateNode("element", "slogan", "");
            nodeSlogan.InnerText = AmphoeSlogan;
            newElement.AppendChild(nodeSlogan);

            var nodeAddress = (XmlElement)xmlDocument.CreateNode("element", "address", "");
            nodeAddress.InnerText = DistrictOffice;
            newElement.AppendChild(nodeAddress);

            var nodeHistory = (XmlElement)xmlDocument.CreateNode("element", "history", "");
            nodeHistory.InnerText = History;
            newElement.AppendChild(nodeHistory);

            var nodeClimate = (XmlElement)xmlDocument.CreateNode("element", "climate", "");
            nodeClimate.InnerText = Climate;
            newElement.AppendChild(nodeClimate);

            var nodeArea = (XmlElement)xmlDocument.CreateNode("element", "area", "");
            nodeArea.InnerText = Area;
            newElement.AppendChild(nodeArea);

            var nodeUrl = (XmlElement)xmlDocument.CreateNode("element", "website", "");
            nodeUrl.InnerText = Website;
            newElement.AppendChild(nodeUrl);

            var nodeTelephone = (XmlElement)xmlDocument.CreateNode("element", "telephone", "");
            nodeTelephone.InnerText = Telephone;
            newElement.AppendChild(nodeTelephone);

            var nodeFax = (XmlElement)xmlDocument.CreateNode("element", "fax", "");
            nodeFax.InnerText = Fax;
            newElement.AppendChild(nodeFax);

            var nodeSubEntities = (XmlElement)xmlDocument.CreateNode("element", "subdivision", "");
            nodeSubEntities.SetAttribute("tambon", Tambon.ToString());
            nodeSubEntities.SetAttribute("thesaban", Thesaban.ToString());
            nodeSubEntities.SetAttribute("muban", Muban.ToString());
            nodeSubEntities.SetAttribute("TAO", TAO.ToString());
            newElement.AppendChild(nodeSubEntities);
        }
    }
}