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
        private XmlNode _DocumentNode;

        #endregion variables

        #region properties

        public XmlNode DocumentNode
        {
            get
            {
                return _DocumentNode;
            }
        }

        #endregion properties

        #region methods

        //this function create the frame of the document, it is standard
        private void GenerateKml()
        {
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlNode kmlNode = doc.CreateElement("kml");

            XmlAttribute xmlAttribute = doc.CreateAttribute("xmlns");
            xmlAttribute.Value = "http://earth.google.com/kml/2.1";
            kmlNode.Attributes.Append(xmlAttribute);
            doc.AppendChild(kmlNode);

            _DocumentNode = doc.CreateElement("Document");
            kmlNode.AppendChild(_DocumentNode);
        }

        public XmlNode AddStyle(String name)
        {
            XmlNode styleNode = doc.CreateElement("Style");
            XmlAttribute styleAttribute = doc.CreateAttribute("id");
            styleAttribute.Value = name;
            styleNode.Attributes.Append(styleAttribute);
            _DocumentNode.AppendChild(styleNode);
            return styleNode;
        }

        public void AddStylePoly(String name, Int32 lineWidth, UInt32 lineColor, Boolean polyFill)
        {
            XmlNode lStyleNode = AddStyle(name);
            AddStylePoly(lStyleNode, lineWidth, lineColor, polyFill);
        }

        public void AddStylePoly(XmlNode node, Int32 lineWidth, UInt32 lineColor, Boolean polyFill)
        {
            XmlNode polyStyleNode = doc.CreateElement("PolyStyle");
            node.AppendChild(polyStyleNode);

            XmlNode lineNode = doc.CreateElement("LineStyle");
            node.AppendChild(lineNode);
            XmlNode lineWidthNode = doc.CreateElement("width");
            lineWidthNode.InnerText = lineWidth.ToString();
            lineNode.AppendChild(lineWidthNode);
            XmlNode lineColorNode = doc.CreateElement("color");
            lineColorNode.InnerText = lineColor.ToString("X");
            lineNode.AppendChild(lineColorNode);

            XmlNode fillNode = doc.CreateElement("fill");
            fillNode.InnerText = Convert.ToInt32(polyFill).ToString();
            polyStyleNode.AppendChild(fillNode);
        }

        public void AddIconStyle(String name, Uri iconUrl)
        {
            XmlNode styleNode = AddStyle(name);
            AddIconStyle(styleNode, iconUrl);
        }

        public void AddIconStyle(XmlNode node, Uri iconUrl)
        {
            XmlNode iconStyleNode = doc.CreateElement("IconStyle");
            node.AppendChild(iconStyleNode);
            XmlNode iconNode = doc.CreateElement("Icon");
            iconStyleNode.AppendChild(iconNode);
            XmlNode iconHrefNode = doc.CreateElement("href");
            iconHrefNode.AppendChild(doc.CreateTextNode(iconUrl.ToString()));
            iconNode.AppendChild(iconHrefNode);
        }

        public void SaveToFile(String fileName)
        {
            doc.Save(fileName);
        }

        //this function can add a point to the map, if you want extend functionalities you have to create other functions for each Google earth shapes ( polygon, line, etc... )
        public XmlNode AddPoint(XmlNode node, Double latitude, Double longitude, String name, String style, String address, String description)
        {
            XmlNode placemarkNode = AddPlacemarkNode(node, name, style, description);
            if ( !String.IsNullOrEmpty(address) )
            {
                XmlNode addressNode = doc.CreateElement("address");
                addressNode.AppendChild(doc.CreateTextNode(address));
                placemarkNode.AppendChild(addressNode);
            }
            XmlNode pointNode = doc.CreateElement("Point");
            placemarkNode.AppendChild(pointNode);
            XmlNode coordinateNode = doc.CreateElement("coordinates");
            pointNode.AppendChild(coordinateNode);
            coordinateNode.AppendChild(doc.CreateTextNode(longitude.ToString(Helper.CultureInfoUS) + "," + latitude.ToString(Helper.CultureInfoUS)));
            return placemarkNode;
        }

        private XmlNode AddPlacemarkNode(XmlNode node, String name, String style, String description)
        {
            XmlNode placemarkNode = doc.CreateElement("Placemark");
            node.AppendChild(placemarkNode);
            XmlNode nameNode = doc.CreateElement("name");
            nameNode.AppendChild(doc.CreateTextNode(name));
            placemarkNode.AppendChild(nameNode);

            XmlNode styleNode = doc.CreateElement("styleUrl");
            styleNode.AppendChild(doc.CreateTextNode('#' + style));
            placemarkNode.AppendChild(styleNode);
            if ( !String.IsNullOrEmpty(description) )
            {
                XmlNode descriptionNode = doc.CreateElement("description");
                descriptionNode.AppendChild(doc.CreateTextNode(description));
                placemarkNode.AppendChild(descriptionNode);
            }
            return placemarkNode;
        }

        public XmlNode AddPoint(Double latitude, Double longitude, String name, String style, String address, String description)
        {
            XmlNode RetVal = AddPoint(_DocumentNode, latitude, longitude, name, style, address, description);
            return RetVal;
        }

        public XmlNode AddFolder(XmlNode node, String name, Boolean opened)
        {
            XmlNode folderNode = doc.CreateElement("Folder");
            node.AppendChild(folderNode);
            XmlNode nameNode = doc.CreateElement("name");
            nameNode.AppendChild(doc.CreateTextNode(name));
            folderNode.AppendChild(nameNode);
            XmlNode openNode = doc.CreateElement("open");
            if ( opened )
            {
                openNode.AppendChild(doc.CreateTextNode("1"));
            }
            else
            {
                openNode.AppendChild(doc.CreateTextNode("0"));
            }
            folderNode.AppendChild(openNode);
            return folderNode;
        }

        public XmlNode AddPolygon(XmlNode node, List<GeoPoint> border, String name, String style, String description, Boolean tessellate)
        {
            XmlNode placemarkNode = AddPlacemarkNode(node, name, style, description);
            XmlNode polygonNode = doc.CreateElement("Polygon");
            placemarkNode.AppendChild(polygonNode);
            XmlNode tessellateNode = doc.CreateElement("tesselate");
            tessellateNode.InnerText = Convert.ToInt32(tessellate).ToString();
            polygonNode.AppendChild(tessellateNode);
            XmlNode outerBoundaryNode = doc.CreateElement("outerBoundaryIs");
            polygonNode.AppendChild(outerBoundaryNode);
            XmlNode linearRingNode = doc.CreateElement("LinearRing");
            outerBoundaryNode.AppendChild(linearRingNode);
            XmlNode coordinateNode = doc.CreateElement("coordinates");
            linearRingNode.AppendChild(coordinateNode);

            String coordinates = String.Empty;
            foreach ( GeoPoint point in border )
            {
                coordinates +=
                    point.Longitude.ToString(Helper.CultureInfoUS) + "," +
                    point.Latitude.ToString(Helper.CultureInfoUS) + "," +
                    point.Altitude.ToString(Helper.CultureInfoUS) + Environment.NewLine;
            }
            coordinateNode.InnerText = coordinates;

            return placemarkNode;
        }

        #endregion methods

        #region constructor

        public KmlHelper()
        {
            GenerateKml();
        }

        #endregion constructor
    }

    // ToDo: Placemarks - <address>..</address>
    // ToDo: Placemarks - <description>...</description> - inside may be HTML hidden with CDATA
}