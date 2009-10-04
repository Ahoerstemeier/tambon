using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class GeoPoint: ICloneable
    {
        #region properties
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        #endregion

        #region constructor
        public GeoPoint()
        {}
        public GeoPoint(float iLatitude, float iLongitude)
        {
            Latitude = iLatitude;
            Longitude = iLongitude;
        }
        public GeoPoint(GeoPoint iValue)
        {
            Latitude = iValue.Latitude;
            Longitude = iValue.Longitude;
        }
#endregion

        #region methods
        public void ExportToKml(XmlNode iNode)
        {
            XmlDocument lXmlDocument = Helper.XmlDocumentFromNode(iNode);
            var lNewElement = (XmlElement)lXmlDocument.CreateNode("element", "Point", "");
            iNode.AppendChild(lNewElement);

            var lCoordinatesElement = (XmlElement)lXmlDocument.CreateNode("element", "coordinates", "");
            lCoordinatesElement.InnerText = 
                Latitude.ToString(Helper.CultureInfoUS)+','+
                Longitude.ToString(Helper.CultureInfoUS)+",0";
            lNewElement.AppendChild(lCoordinatesElement);
        }
        internal void ExportToXML(XmlElement iNode)
        {
            XmlDocument lXmlDocument = Helper.XmlDocumentFromNode(iNode);
            var lNewElement = (XmlElement)lXmlDocument.CreateNode("element", "geo:Point", "");
            iNode.AppendChild(lNewElement);

            var lLatitudeElement = (XmlElement)lXmlDocument.CreateNode("element", "geo:lat", "");
            lLatitudeElement.InnerText = Latitude.ToString(Helper.CultureInfoUS);
            lNewElement.AppendChild(lLatitudeElement);

            var lLongitudeElement = (XmlElement)lXmlDocument.CreateNode("element", "get:long", "");
            lLongitudeElement.InnerText = Longitude.ToString(Helper.CultureInfoUS);
            lNewElement.AppendChild(lLongitudeElement);
        }

        internal static GeoPoint Load(XmlNode iNode)
        {
            GeoPoint RetVal = null;

            if (iNode != null && iNode.Name.Equals("geo:Point"))
            {
                RetVal = new GeoPoint();

                if (iNode.HasChildNodes)
                {
                    foreach (XmlNode lChildNode in iNode.ChildNodes)
                    {
                        if (lChildNode.Name == "geo:lat")
                        {
                            String lLatitude = lChildNode.InnerText;
                            RetVal.Latitude = Convert.ToDouble(lLatitude, Helper.CultureInfoUS);
                        }
                        if (lChildNode.Name == "geo:long")
                        {
                            String lLongitude = lChildNode.InnerText;
                            RetVal.Longitude = Convert.ToDouble(lLongitude, Helper.CultureInfoUS);
                        }
                    }
                }
            }
            return RetVal;

        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new GeoPoint(this);
        }

        #endregion

    }
}
