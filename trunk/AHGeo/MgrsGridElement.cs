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
        }
        private Int16 mDigits = UTMPoint.MinDigits;
        public Int16 Digits
        {
            get { return mDigits; }
        }
        private GeoDatum mDatum = GeoDatum.DatumWGS84();
        public GeoDatum Datum 
        { 
            get { return mDatum; } 
            set { mDatum = value; } 
        }
        private String mName = String.Empty;
        public String Name
        {
            get { return mName; }
            set { SetMgrs(value); }
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

        private Boolean SameZone(UTMPoint iPoint)
        { 
            GeoPoint lGeo = new GeoPoint(iPoint, mDatum);
            UTMPoint lRealUtm = lGeo.CalcUTM();
            Boolean lResult = (mCentralPoint.ZoneNumber == lRealUtm.ZoneNumber);
            return lResult;
        }
        private UTMPoint LimitToZone(UTMPoint iPoint)
        {
            UTMPoint lResult = new UTMPoint(iPoint);
            Int32 lMinEasting = 0;
            Int32 lMaxEasting = 0;
            if (iPoint.Easting < mCentralPoint.Easting)
            {
                lMinEasting = iPoint.Easting;
                lMaxEasting = lMinEasting + GridSize(mDigits);
            }
            else
            {
                lMaxEasting = iPoint.Easting;
                lMinEasting = lMaxEasting - GridSize(mDigits);
            }
            Int32 lLeftZone = 0;
            {
                UTMPoint lTemp = new UTMPoint(iPoint);
                lTemp.Easting = lMinEasting;
                GeoPoint lTempGeo = new GeoPoint(lTemp, mDatum);
                lTemp = lTempGeo.CalcUTM();
                lLeftZone = lTemp.ZoneNumber;
            }
            Int32 lDiff = lMaxEasting - lMinEasting;
            while (lDiff > 1)
            {
                Int32 lTempEasting = lMinEasting + lDiff / 2;
                lResult = new UTMPoint(iPoint);
                lResult.Easting = lTempEasting;
                GeoPoint lTempGeo = new GeoPoint(lResult, mDatum);
                UTMPoint lTemp = lTempGeo.CalcUTM();
                if (lTemp.ZoneNumber > lLeftZone)
                {
                    lMaxEasting = lTempEasting;
                }
                else
                {
                    lMinEasting = lTempEasting;
                }
                lDiff = lDiff / 2;
            }
            return lResult;
        }
        public UTMPoint NorthWestCorner()
        {
            String lValue = mCentralPoint.ToUTMString(mDigits);
            UTMPoint lResult = new UTMPoint(lValue);
            if (!SameZone(lResult))
            {
                lResult = LimitToZone(lResult);
            }
            return lResult;
        }
        public UTMPoint NorthEastCorner()
        {
            String lValue = mCentralPoint.ToUTMString(mDigits);
            UTMPoint lResult = new UTMPoint(lValue);
            lResult.Easting += GridSize(mDigits);
            if (!SameZone(lResult))
            {
                lResult = LimitToZone(lResult);
            }
            return lResult;
        }
        public UTMPoint SouthWestCorner()
        {
            String lValue = mCentralPoint.ToUTMString(mDigits);
            UTMPoint lResult = new UTMPoint(lValue);
            lResult.Northing += GridSize(mDigits);
            if (!SameZone(lResult))
            {
                lResult = LimitToZone(lResult);
            }
            return lResult;
        }
        public UTMPoint SouthEastCorner()
        {
            String lValue = mCentralPoint.ToUTMString(mDigits);
            UTMPoint lResult = new UTMPoint(lValue);
            lResult.Easting += GridSize(mDigits);
            lResult.Northing += GridSize(mDigits);
            if (!SameZone(lResult))
            {
                lResult = LimitToZone(lResult);
            }
            return lResult;
        }
        public UTMPoint ActualCentralPoint()
        {
            String lValue = mCentralPoint.ToUTMString(mDigits);
            UTMPoint lWest = new UTMPoint(lValue);
            lWest.Northing += GridSize(mDigits) / 2;
            UTMPoint lEast = new UTMPoint(lWest);
            lEast.Easting += GridSize(mDigits);
            if (!SameZone(lWest))
            {
                lWest = LimitToZone(lWest);
            }
            if (!SameZone(lEast))
            {
                lEast = LimitToZone(lEast);
            }
            UTMPoint lResult = new UTMPoint(lValue);
            lResult.Northing += GridSize(mDigits) / 2;
            lResult.Easting = (lWest.Easting + lEast.Easting) / 2;
            return lResult;
        }
        public Boolean IsValid()
        {
            Boolean lResult = SameZone(ActualCentralPoint());
            return lResult; 
        }

        public List<MgrsGridElement> SubGrids()
        {
            List<MgrsGridElement> lResult = new List<MgrsGridElement>();
            String lStart = mName.Substring(0, 5);
            String lNumbers = mName.Remove(0, 5);
            for (Int32 lSubEasting = 0; lSubEasting < 10; lSubEasting++)
            {
                for (Int32 lSubNorthing = 0; lSubNorthing < 10; lSubNorthing++)
                {
                    String lEastingString = lNumbers.Substring(0, mDigits-2) + lSubEasting.ToString();
                    String lNorthingString = lNumbers.Substring(mDigits-2, mDigits-2) + lSubNorthing.ToString();
                    String lName = lStart + lEastingString + lNorthingString;
                    MgrsGridElement lSubElement = new MgrsGridElement(lName);
                    lSubElement.Datum = this.Datum;
                    if (lSubElement.IsValid())
                    {
                        lResult.Add(lSubElement);
                    }
                }
            }
            return lResult;
        }

        private static void AddKmlStyle(KmlHelper iKmlWriter)
        {
            XmlNode lNode = iKmlWriter.AddStyle(mDefStyle);
            iKmlWriter.AddStylePoly(lNode, 2, false);
            iKmlWriter.AddIconStyle(lNode,new Uri("http://maps.google.com/mapfiles/kml/paddle/wht-blank.png"));
        }
        public void WriteToKml(KmlHelper iKmlWriter, XmlNode iNode)
        {
            XmlNode lNode = iNode;
            String lDescription = "MGRS Grid: " + mName;

            GeoPoint lPoint = new GeoPoint(ActualCentralPoint(),mDatum);
            // iKmlWriter.AddPoint(iNode, lPoint.Latitude, lPoint.Longitude, mName, mDefStyle, String.Empty, lDescription);
            List<GeoPoint> lBorder = new List<GeoPoint>();
            lBorder.Add(new GeoPoint(NorthWestCorner(),mDatum));
            lBorder.Add(new GeoPoint(NorthEastCorner(),mDatum));
            lBorder.Add(new GeoPoint(SouthEastCorner(),mDatum));
            lBorder.Add(new GeoPoint(SouthWestCorner(),mDatum));
            lBorder.Add(new GeoPoint(NorthWestCorner(),mDatum));
            iKmlWriter.AddPolygon(iNode, lBorder, mName, mDefStyle, lDescription, true);
        }
        public void ExportToKml(String iFilename)
        {
            KmlHelper lKmlWriter = StartKmlWriting();
            WriteToKml(lKmlWriter, lKmlWriter.DocumentNode);
            lKmlWriter.SaveToFile(iFilename);
        }
        public static KmlHelper StartKmlWriting()
        {
            KmlHelper lKmlWriter = new KmlHelper();
            AddKmlStyle(lKmlWriter);
            return lKmlWriter;
        }
        #endregion

        private void SetMgrs(String iValue)
        {
            String lMgrs = iValue.Replace(" ", "");
            Int16 lDigits = Convert.ToInt16((lMgrs.Length-1)/2);
            mCentralPoint = MakeCentral(UTMPoint.ParseMGRSString(iValue), lDigits);
            mDigits = lDigits;
            mName = iValue;
        }

        public MgrsGridElement(String iMgrs)
        {
            SetMgrs(iMgrs);
        }
    }
}
