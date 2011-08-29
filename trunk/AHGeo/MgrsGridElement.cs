using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Geo
{
    public class MgrsGridElement
    {
        private const string mDefStyle = "Blue";
        private const UInt32 mDefColor = 0xffff0000;

        #region properties
        public UtmPoint CentralPoint
        {
            get;
            private set;
        }
        private Int16 _Digits = UtmPoint.MinimumDigits;
        public Int16 Digits
        {
            get { return _Digits; }
            private set { _Digits = value; }
        }
        private GeoDatum mDatum = GeoDatum.DatumWGS84();
        public GeoDatum Datum
        {
            get { return mDatum; }
            set { mDatum = value; }
        }
        private String _Name = String.Empty;
        public String Name
        {
            get { return _Name; }
            set { SetMgrs(value); }
        }
        #endregion

        #region methods
        private static Int32 GridSize(Int16 digits)
        {
            Int16 actualDigits = UtmPoint.MakeDigitValid(digits);
            Int32 result = Convert.ToInt32(Math.Pow(10, UtmPoint.MaximumDigits - actualDigits));
            return result;
        }
        private static UtmPoint MakeCentral(UtmPoint point, Int16 digits)
        {
            String value = point.ToUtmString(digits);
            UtmPoint utmPoint = new UtmPoint(value);
            Double distance = 0.5 * GridSize(digits);
            Double easting = utmPoint.Easting + distance;
            Double northing = utmPoint.Northing + distance;
            UtmPoint middlePoint = new UtmPoint(easting, northing, utmPoint.ZoneNumber, utmPoint.IsNorthernHemisphere);
            return middlePoint;
        }

        private Boolean SameZone(UtmPoint point)
        {
            GeoPoint geoPoint = new GeoPoint(point, mDatum);
            UtmPoint realUtm = geoPoint.CalcUTM();
            Boolean result = (CentralPoint.ZoneNumber == realUtm.ZoneNumber);
            return result;
        }
        private UtmPoint LimitToZone(UtmPoint point)
        {
            UtmPoint result = new UtmPoint(point);
            Int32 minEasting = 0;
            Int32 maxEasting = 0;
            if ( point.Easting < CentralPoint.Easting )
            {
                minEasting = point.Easting;
                maxEasting = minEasting + GridSize(_Digits);
            }
            else
            {
                maxEasting = point.Easting;
                minEasting = maxEasting - GridSize(_Digits);
            }
            Int32 leftZone = 0;
            {
                UtmPoint tempUtmPoint = new UtmPoint(point);
                tempUtmPoint.Easting = minEasting;
                GeoPoint tempGeoPoint = new GeoPoint(tempUtmPoint, mDatum);
                tempUtmPoint = tempGeoPoint.CalcUTM();
                leftZone = tempUtmPoint.ZoneNumber;
            }
            Int32 eastingDiff = maxEasting - minEasting;
            while ( eastingDiff > 1 )
            {
                Int32 tempEasting = minEasting + eastingDiff / 2;
                result = new UtmPoint(point);
                result.Easting = tempEasting;
                GeoPoint tempGeoPoint = new GeoPoint(result, mDatum);
                UtmPoint empUtmPoint = tempGeoPoint.CalcUTM();
                if ( empUtmPoint.ZoneNumber > leftZone )
                {
                    maxEasting = tempEasting;
                }
                else
                {
                    minEasting = tempEasting;
                }
                eastingDiff = eastingDiff / 2;
            }
            return result;
        }
        public UtmPoint NorthWestCorner()
        {
            String value = CentralPoint.ToUtmString(_Digits);
            UtmPoint result = new UtmPoint(value);
            if ( !SameZone(result) )
            {
                result = LimitToZone(result);
            }
            return result;
        }
        public UtmPoint NorthEastCorner()
        {
            String value = CentralPoint.ToUtmString(_Digits);
            UtmPoint result = new UtmPoint(value);
            result.Easting += GridSize(_Digits);
            if ( !SameZone(result) )
            {
                result = LimitToZone(result);
            }
            return result;
        }
        public UtmPoint SouthWestCorner()
        {
            String value = CentralPoint.ToUtmString(_Digits);
            UtmPoint result = new UtmPoint(value);
            result.Northing += GridSize(_Digits);
            if ( !SameZone(result) )
            {
                result = LimitToZone(result);
            }
            return result;
        }
        public UtmPoint SouthEastCorner()
        {
            String value = CentralPoint.ToUtmString(_Digits);
            UtmPoint result = new UtmPoint(value);
            result.Easting += GridSize(_Digits);
            result.Northing += GridSize(_Digits);
            if ( !SameZone(result) )
            {
                result = LimitToZone(result);
            }
            return result;
        }
        public UtmPoint ActualCentralPoint()
        {
            String value = CentralPoint.ToUtmString(_Digits);
            UtmPoint west = new UtmPoint(value);
            west.Northing += GridSize(_Digits) / 2;
            UtmPoint east = new UtmPoint(west);
            east.Easting += GridSize(_Digits);
            if ( !SameZone(west) )
            {
                west = LimitToZone(west);
            }
            if ( !SameZone(east) )
            {
                east = LimitToZone(east);
            }
            UtmPoint result = new UtmPoint(value);
            result.Northing += GridSize(_Digits) / 2;
            result.Easting = (west.Easting + east.Easting) / 2;
            return result;
        }
        public Boolean IsValid()
        {
            Boolean result = SameZone(ActualCentralPoint());
            return result;
        }

        public List<MgrsGridElement> SubGrids()
        {
            List<MgrsGridElement> result = new List<MgrsGridElement>();
            String start = _Name.Substring(0, 5);
            String numbers = _Name.Remove(0, 5);
            for ( Int32 subEasting = 0 ; subEasting < 10 ; subEasting++ )
            {
                for ( Int32 subNorthing = 0 ; subNorthing < 10 ; subNorthing++ )
                {
                    String eastingString = numbers.Substring(0, _Digits - 2) + subEasting.ToString();
                    String northingString = numbers.Substring(_Digits - 2, _Digits - 2) + subNorthing.ToString();
                    String name = start + eastingString + northingString;
                    MgrsGridElement subElement = new MgrsGridElement(name);
                    subElement.Datum = this.Datum;
                    if ( subElement.IsValid() )
                    {
                        result.Add(subElement);
                    }
                }
            }
            return result;
        }

        private static void AddKmlStyle(KmlHelper kmlWriter)
        {
            XmlNode node = kmlWriter.AddStyle(mDefStyle);
            kmlWriter.AddStylePoly(node, 2, mDefColor, false);
            kmlWriter.AddIconStyle(node, new Uri("http://maps.google.com/mapfiles/kml/paddle/wht-blank.png"));
        }
        public void WriteToKml(KmlHelper kmlWriter, XmlNode node)
        {
            String description = "MGRS Grid: " + _Name;

            GeoPoint point = new GeoPoint(ActualCentralPoint(), mDatum);
            // iKmlWriter.AddPoint(iNode, lPoint.Latitude, lPoint.Longitude, mName, mDefStyle, String.Empty, lDescription);
            List<GeoPoint> border = new List<GeoPoint>();
            border.Add(new GeoPoint(NorthWestCorner(), mDatum));
            border.Add(new GeoPoint(NorthEastCorner(), mDatum));
            border.Add(new GeoPoint(SouthEastCorner(), mDatum));
            border.Add(new GeoPoint(SouthWestCorner(), mDatum));
            border.Add(new GeoPoint(NorthWestCorner(), mDatum));
            kmlWriter.AddPolygon(node, border, _Name, mDefStyle, description, true);
        }
        public void ExportToKml(String fileName)
        {
            KmlHelper kmlWriter = StartKmlWriting();
            WriteToKml(kmlWriter, kmlWriter.DocumentNode);
            kmlWriter.SaveToFile(fileName);
        }
        public static KmlHelper StartKmlWriting()
        {
            KmlHelper kmlWriter = new KmlHelper();
            AddKmlStyle(kmlWriter);
            return kmlWriter;
        }
        #endregion

        private void SetMgrs(String value)
        {
            String mgrs = value.Replace(" ", "");
            Int16 digits = Convert.ToInt16((mgrs.Length - 1) / 2);
            CentralPoint = MakeCentral(UtmPoint.ParseMgrsString(value), digits);
            _Digits = digits;
            _Name = value;
        }

        public MgrsGridElement(String mgrs)
        {
            SetMgrs(mgrs);
        }
    }
}
