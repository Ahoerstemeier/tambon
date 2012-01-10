using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Geo
{
    public abstract class GeoFrameBase
    {
        #region properties
        public GeoPoint NorthWestCorner
        {
            get { return GetNorthWestCorner(); }
        }
        public GeoPoint SouthWestCorner
        {
            get { return GetSouthWestCorner(); }
        }
        public GeoPoint NorthEastCorner
        {
            get { return GetNorthEastCorner(); }
        }
        public GeoPoint SouthEastCorner
        {
            get { return GetSouthEastCorner(); }
        }
        public GeoPoint MiddlePoint 
        {
            get { return GetMiddlePoint(); }
        }

        public String Name { get; set; }
        #endregion

        #region private methods
        protected abstract GeoPoint GetNorthWestCorner();
        protected abstract GeoPoint GetNorthEastCorner();
        protected abstract GeoPoint GetSouthWestCorner();
        protected abstract GeoPoint GetSouthEastCorner();

        protected GeoPoint GetMiddlePoint()
        {
            var northWest = NorthWestCorner;
            var southEast = SouthEastCorner;
            Double latitude = northWest.Latitude + (southEast.Latitude - northWest.Latitude) / 2.0;
            Double longitude = northWest.Longitude + (southEast.Longitude - northWest.Longitude) / 2.0;
            GeoPoint result = new GeoPoint(latitude,longitude);
            return result;
        }
        #endregion

        #region KML export
        private const string _DefStyle = "Blue";
        private const UInt32 _DefColor = 0xffff0000;

        private static void AddKmlStyle(KmlHelper kmlWriter)
        {
            XmlNode node = kmlWriter.AddStyle(_DefStyle);
            kmlWriter.AddStylePoly(node, 2, _DefColor, false);
            kmlWriter.AddIconStyle(node, new Uri("http://maps.google.com/mapfiles/kml/paddle/wht-blank.png"));
        }
        public void WriteToKml(KmlHelper kmlWriter, XmlNode node, String description)
        {
            List<GeoPoint> border = new List<GeoPoint>();
            border.Add(NorthWestCorner);
            border.Add(NorthEastCorner);
            border.Add(SouthEastCorner);
            border.Add(SouthWestCorner);
            border.Add(NorthWestCorner);
            kmlWriter.AddPolygon(node, border, Name, _DefStyle, description, true);
        }
        public void ExportToKml(String fileName)
        {
            KmlHelper kmlWriter = StartKmlWriting();
            WriteToKml(kmlWriter, kmlWriter.DocumentNode, Name);
            kmlWriter.SaveToFile(fileName);
        }
        public static KmlHelper StartKmlWriting()
        {
            KmlHelper kmlWriter = new KmlHelper();
            AddKmlStyle(kmlWriter);
            return kmlWriter;
        }
        #endregion

        #region public methods
        public Boolean IsInside(GeoPoint point)
        {
            if ( point == null )
            {
                throw new ArgumentNullException("point");
            }

            GeoPoint a = NorthWestCorner;
            GeoPoint b = SouthWestCorner;
            GeoPoint c = NorthEastCorner;
            Double bax = b.Latitude - a.Latitude;
            Double bay = b.Longitude - a.Longitude;
            Double cax = c.Latitude - a.Latitude;
            Double cay = c.Longitude - a.Longitude;

            if ( (point.Latitude - a.Latitude) * bax + (point.Longitude - a.Longitude) * bay < 0.0 )
                return false;
            if ( (point.Latitude - b.Latitude) * bax + (point.Longitude - b.Longitude) * bay > 0.0 )
                return false;
            if ( (point.Latitude - a.Latitude) * cax + (point.Longitude - a.Longitude) * cay < 0.0 )
                return false;
            if ( (point.Latitude - c.Latitude) * cax + (point.Longitude - c.Longitude) * cay > 0.0 )
                return false;
            return true;

        }
        #endregion
    }
}
