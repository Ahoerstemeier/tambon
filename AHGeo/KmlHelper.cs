using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Geo
{
    public class KmlHelper
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

        public XmlNode AddStyle(String iName)
        {            
            XmlNode lStyleNode = doc.CreateElement("Style");
            XmlAttribute lStyleAttribute = doc.CreateAttribute("id");
            lStyleAttribute.Value = iName;
            lStyleNode.Attributes.Append(lStyleAttribute);
            mDocumentNode.AppendChild(lStyleNode);
            return lStyleNode;
        }
        public void AddStylePoly(String iName, Int32 iLineWidth, Boolean iPolyFill)
        {
            XmlNode lStyleNode = AddStyle(iName);
            AddStylePoly(lStyleNode,iLineWidth,iPolyFill);
        }
        public void AddStylePoly(XmlNode iNode, Int32 iLineWidth, Boolean iPolyFill)
        {
            XmlNode lPolyStyleNode = doc.CreateElement("PolyStyle");
            iNode.AppendChild(lPolyStyleNode);

            XmlNode lLineNode = doc.CreateElement("LineStyle");
            lPolyStyleNode.AppendChild(lLineNode);
            XmlNode lLineWidthNode = doc.CreateElement("width");
            lLineWidthNode.InnerText = iLineWidth.ToString();
            //  <color>ff0000ff</color>
            
            XmlNode lFillNode = doc.CreateElement("fill");
            lFillNode.InnerText = Convert.ToInt32(iPolyFill).ToString();
            lPolyStyleNode.AppendChild(lFillNode);
        }
        public void AddIconStyle(String iName, Uri iIconUrl)
        {
            XmlNode lStyleNode = AddStyle(iName);
            AddIconStyle(lStyleNode, iIconUrl);
        }
        public void AddIconStyle(XmlNode iNode, Uri iIconUrl)
        {
            XmlNode lIconStyleNode = doc.CreateElement("IconStyle");
            iNode.AppendChild(lIconStyleNode);
            XmlNode lIconNode = doc.CreateElement("Icon");
            lIconStyleNode.AppendChild(lIconNode);
            XmlNode lIconHrefNode = doc.CreateElement("href");
            lIconHrefNode.AppendChild(doc.CreateTextNode(iIconUrl.ToString()));
            lIconNode.AppendChild(lIconHrefNode);
        }

        public void SaveToFile(String iFilename)
        {
            doc.Save(iFilename);
        }
        //this function can add a point to the map, if you want extend functionalities you have to create other functions for each google earth shapes ( polygon, line, etc... ) 
        public XmlNode AddPoint(XmlNode iNode, double iLatitude, double iLongitude, String iName, String iStyle, String iAddress, String iDescription)
        {
            XmlNode lPlacemarkNode = AddPlacemarkNode(iNode, iName, iStyle, iDescription);
            if (!String.IsNullOrEmpty(iAddress))
            {
                XmlNode lAddressNode = doc.CreateElement("address");
                lAddressNode.AppendChild(doc.CreateTextNode(iAddress));
                lPlacemarkNode.AppendChild(lAddressNode);
            }
            XmlNode lPointNode = doc.CreateElement("Point");
            lPlacemarkNode.AppendChild(lPointNode);
            XmlNode lCoordinateNode = doc.CreateElement("coordinates");
            lPointNode.AppendChild(lCoordinateNode);
            lCoordinateNode.AppendChild(doc.CreateTextNode(iLongitude.ToString(Helper.CultureInfoUS) + "," + iLatitude.ToString(Helper.CultureInfoUS)));
            return lPlacemarkNode;
        }

        private XmlNode AddPlacemarkNode(XmlNode iNode, String iName, String iStyle, String iDescription)
        {
            XmlNode lPlacemarkNode = doc.CreateElement("Placemark");
            iNode.AppendChild(lPlacemarkNode);
            XmlNode lNameNode = doc.CreateElement("name");
            lNameNode.AppendChild(doc.CreateTextNode(iName));
            lPlacemarkNode.AppendChild(lNameNode);

            XmlNode lStyleNode = doc.CreateElement("styleUrl");
            lStyleNode.AppendChild(doc.CreateTextNode('#' + iStyle));
            lPlacemarkNode.AppendChild(lStyleNode);
            if (!String.IsNullOrEmpty(iDescription))
            {
                XmlNode lDescriptionNode = doc.CreateElement("description");
                lDescriptionNode.AppendChild(doc.CreateTextNode(iDescription));
                lPlacemarkNode.AppendChild(lDescriptionNode);
            }
            return lPlacemarkNode;
        }
        public XmlNode AddPoint(double iLatitude, double iLongitude, String iName, String iStyle, String iAddress, String iDescription)
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
        public XmlNode AddPolygon(XmlNode iNode, List<GeoPoint> lBorder, String iName, String iStyle, String iDescription, Boolean iTessellate)
        {
            XmlNode lPlacemarkNode = AddPlacemarkNode(iNode, iName, iStyle, iDescription);
            XmlNode lPolygonNode = doc.CreateElement("Polygon");
            lPlacemarkNode.AppendChild(lPolygonNode);
            XmlNode lTessellateNode = doc.CreateElement("tesselate");
            lTessellateNode.InnerText = Convert.ToInt32(iTessellate).ToString();
            lPolygonNode.AppendChild(lTessellateNode);
            XmlNode lOuterBoundaryNode = doc.CreateElement("outerBoundaryIs");
            lPolygonNode.AppendChild(lOuterBoundaryNode);
            XmlNode lLinearRingNode = doc.CreateElement("LinearRing");
            lOuterBoundaryNode.AppendChild(lLinearRingNode);
            XmlNode lCoordinateNode = doc.CreateElement("coordinates");
            lLinearRingNode.AppendChild(lCoordinateNode);

            String lCoordinates = String.Empty;
            foreach (GeoPoint lPoint in lBorder)
            {
                lCoordinates +=
                    lPoint.Longitude.ToString(Helper.CultureInfoUS) + "," +
                    lPoint.Latitude.ToString(Helper.CultureInfoUS) + "," +
                    lPoint.Altitude.ToString(Helper.CultureInfoUS) + Environment.NewLine;
            }
            lCoordinateNode.InnerText = lCoordinates;

            return lPlacemarkNode;

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
