using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class KmlHelper
    {
        #region variables
        private XmlDocument doc = new XmlDocument();
        private XmlNode mDocumentNode;
        #endregion

        #region properties
        public XmlNode DocumentNode { get { return mDocumentNode; } }
        #endregion

        #region methods
        //this function create the frame of the document, it is standard 
        private void GenerateKml()
        {
            XmlNode lDocNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(lDocNode);

            XmlNode lKmlNode = doc.CreateElement("kml");
          
            XmlAttribute lXmlnsAttribute = doc.CreateAttribute("xmlns");
            lXmlnsAttribute.Value = "http://earth.google.com/kml/2.1";
            lKmlNode.Attributes.Append(lXmlnsAttribute);
            doc.AppendChild(lKmlNode);

            mDocumentNode = doc.CreateElement("Document");
            lKmlNode.AppendChild(mDocumentNode);
        }
 
        public void AddIconStyle(string iName,Uri iIconUrl)
        {
            XmlNode lStyleNode = doc.CreateElement("Style");
            XmlAttribute lStyleAttribute = doc.CreateAttribute("id");
            lStyleAttribute.Value = iName;
            lStyleNode.Attributes.Append(lStyleAttribute);
            mDocumentNode.AppendChild(lStyleNode);
            XmlNode lIconStyleNode  = doc.CreateElement("IconStyle");
            lStyleNode.AppendChild(lIconStyleNode);
            XmlNode lIconNode  = doc.CreateElement("Icon");
            lIconStyleNode.AppendChild(lIconNode);
            XmlNode lIconHrefNode  = doc.CreateElement("href");
            lIconHrefNode.AppendChild(doc.CreateTextNode(iIconUrl.ToString()));
            lIconNode.AppendChild(lIconHrefNode);
        }

        public void SaveToFile(String iFilename)
        {
            doc.Save(iFilename);
        }
        //this function can add a point to the map, if you want extend functionalities you have to create other functions for each google earth shapes ( polygon, line, etc... ) 
        public XmlNode AddPoint(XmlNode iNode, double iLatitude, double iLongitude, string iName, string iStyle, string iAddress, string iDescription)
        {
            XmlNode lPlacemarkNode = doc.CreateElement("Placemark");
            iNode.AppendChild(lPlacemarkNode);
            XmlNode lNameNode = doc.CreateElement("name");
            lNameNode.AppendChild(doc.CreateTextNode(iName));
            lPlacemarkNode.AppendChild(lNameNode);
            if (!String.IsNullOrEmpty(iAddress))
            {
                XmlNode lAddressNode = doc.CreateElement("address");
                lAddressNode.AppendChild(doc.CreateTextNode(iAddress));
                lPlacemarkNode.AppendChild(lAddressNode);
            }
            if (!String.IsNullOrEmpty(iDescription))
            {
                XmlNode lDescriptionNode = doc.CreateElement("description");
                lDescriptionNode.AppendChild(doc.CreateTextNode(iDescription));
                lPlacemarkNode.AppendChild(lDescriptionNode);
            }

            XmlNode lStyleNode = doc.CreateElement("styleUrl");
            lStyleNode.AppendChild(doc.CreateTextNode('#' + iStyle));
            lPlacemarkNode.AppendChild(lStyleNode);
            XmlNode lPointNode = doc.CreateElement("Point");
            lPlacemarkNode.AppendChild(lPointNode);
            XmlNode lCoordinateNode = doc.CreateElement("coordinates");
            lPointNode.AppendChild(lCoordinateNode);
            lCoordinateNode.AppendChild(doc.CreateTextNode(iLongitude.ToString(Helper.CultureInfoUS) + "," + iLatitude.ToString(Helper.CultureInfoUS)));
            return lPlacemarkNode;
        }
        public XmlNode AddPoint(double iLatitude, double iLongitude, string iName, string iStyle, string iAddress, string iDescription)
        {
            XmlNode RetVal = AddPoint(mDocumentNode, iLatitude, iLongitude, iName, iStyle,iAddress,iDescription);
            return RetVal;
        }
        public XmlNode AddFolder(XmlNode iNode, string iName, Boolean iOpen)
        {
            XmlNode lFolderNode = doc.CreateElement("Folder");
            iNode.AppendChild(lFolderNode);
            XmlNode lNameNode = doc.CreateElement("name");
            lNameNode.AppendChild(doc.CreateTextNode(iName));
            lFolderNode.AppendChild(lNameNode);
            XmlNode lOpenNode = doc.CreateElement("open");
            if (iOpen)
            {
                lOpenNode.AppendChild(doc.CreateTextNode("1"));
            }
            else
            {
                lOpenNode.AppendChild(doc.CreateTextNode("0"));
            }
            lFolderNode.AppendChild(lOpenNode);
            return lFolderNode; 
        }
        #endregion

        #region constructor
        public KmlHelper()
        {
            GenerateKml();
        }
        #endregion

    }

    // ToDo: Placemarks - <address>..</address>
    // ToDo: Placemarks - <description>...</description> - inside may be HTML hidden with CDATA
} 
