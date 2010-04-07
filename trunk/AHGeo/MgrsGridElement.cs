using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Geo
{
    public class MgrsGridElement
    {
        private const string mDefStyle = "White";

        #region properties
        private UTMPoint mCentralPoint = null;
        public UTMPoint CentralPoint
        {
            get { return mCentralPoint; }
            set { mCentralPoint = value; }
        }
        private Int16 mDigits = UTMPoint.MinDigits;
        public Int16 Digits
        {
            get { return mDigits; }
            set { mDigits = UTMPoint.MakeDigitValid(value); }
        }
        private GeoDatum mDatum = GeoDatum.DatumWGS84();
        public GeoDatum Datum 
        { 
            get { return mDatum; } 
            set { mDatum = value; } 
        }
        #endregion

        #region methods
        private static Int32 GridSize(Int16 iDigits)
        {
            Int16 lDigits = UTMPoint.MakeDigitValid(iDigits);
            Int32 lResult = Convert.ToInt32(Math.Pow(10, UTMPoint.MaxDigits - lDigits));
            return lResult;
        }
        private static UTMPoint MakeCentral(UTMPoint iPoint, Int16 iDigits)
        {
            String lValue = iPoint.ToUTMString(iDigits);
            UTMPoint lPoint = new UTMPoint(lValue);
            Double lDistance = 0.5*GridSize(iDigits);
            Double lEasting = lPoint.Easting + lDistance;
            Double lNorthing = lPoint.Northing + lDistance;
            UTMPoint lMiddlePoint = new UTMPoint(lEasting, lNorthing, lPoint.ZoneNumber, lPoint.IsNorthernHemisphere);
            return lMiddlePoint;
        }

        public override String ToString()
        {
            String lResult = String.Empty;
            lResult = CentralPoint.ToMGRSString(mDigits);
            return lResult;
        }
        public UTMPoint NorthWestCorner()
        {
            String lValue = mCentralPoint.ToUTMString(mDigits);
            UTMPoint lResult = new UTMPoint(lValue);
            return lResult;
        }
        public UTMPoint NorthEastCorner()
        {
            String lValue = mCentralPoint.ToUTMString(mDigits);
            UTMPoint lResult = new UTMPoint(lValue);
            lResult.Easting += GridSize(mDigits);
            return lResult;
        }
        public UTMPoint SouthWestCorner()
        {
            String lValue = mCentralPoint.ToUTMString(mDigits);
            UTMPoint lResult = new UTMPoint(lValue);
            lResult.Northing += GridSize(mDigits);
            return lResult;
        }
        public UTMPoint SouthEastCorner()
        {
            String lValue = mCentralPoint.ToUTMString(mDigits);
            UTMPoint lResult = new UTMPoint(lValue);
            lResult.Easting += GridSize(mDigits);
            lResult.Northing += GridSize(mDigits);
            return lResult;
        }

        private void AddKmlStyle(KmlHelper iKmlWriter)
        {
            XmlNode lNode = iKmlWriter.AddStyle(mDefStyle);
            iKmlWriter.AddStylePoly(lNode, 2, false);
            iKmlWriter.AddIconStyle(lNode,new Uri("http://maps.google.com/mapfiles/kml/paddle/wht-blank.png"));
        }
        public void WriteToKml(KmlHelper iKmlWriter, XmlNode iNode)
        {
            XmlNode lNode = iNode;
            String lDescription = "MGRS Grid: " + ToString();

            GeoPoint lPoint = new GeoPoint(mCentralPoint,mDatum);
            iKmlWriter.AddPoint(iNode, lPoint.Latitude, lPoint.Longitude, ToString(), mDefStyle, String.Empty, lDescription);
            List<GeoPoint> lBorder = new List<GeoPoint>();
            lBorder.Add(new GeoPoint(NorthWestCorner(),mDatum));
            lBorder.Add(new GeoPoint(NorthEastCorner(),mDatum));
            lBorder.Add(new GeoPoint(SouthEastCorner(),mDatum));
            lBorder.Add(new GeoPoint(SouthWestCorner(),mDatum));
            lBorder.Add(new GeoPoint(NorthWestCorner(),mDatum));
            iKmlWriter.AddPolygon(iNode, lBorder, ToString(), mDefStyle, lDescription, true);
        }
        public void ExportToKml(String iFilename)
        {
            KmlHelper lKmlWriter = new KmlHelper();
            AddKmlStyle(lKmlWriter);
            WriteToKml(lKmlWriter, lKmlWriter.DocumentNode);
            lKmlWriter.SaveToFile(iFilename);
        }
        #endregion

        public MgrsGridElement(String iUtm, Int16 iDigits)
        {
            CentralPoint = MakeCentral(UTMPoint.ParseMGRSString(iUtm), iDigits);
            Digits = iDigits;
        }
    }
}
