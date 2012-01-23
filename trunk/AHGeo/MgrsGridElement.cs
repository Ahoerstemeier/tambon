using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Geo
{
    public class MgrsGridElement : GeoFrameBase
    {
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
            private set { _Digits = Math.Max(UtmPoint.MinimumDigits,value); }
        }
        public GeoDatum Datum { get; set; }
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
            GeoPoint geoPoint = new GeoPoint(point, Datum);
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
                GeoPoint tempGeoPoint = new GeoPoint(tempUtmPoint, Datum);
                tempUtmPoint = tempGeoPoint.CalcUTM();
                leftZone = tempUtmPoint.ZoneNumber;
            }
            Int32 eastingDiff = maxEasting - minEasting;
            while ( eastingDiff > 1 )
            {
                Int32 tempEasting = minEasting + eastingDiff / 2;
                result = new UtmPoint(point);
                result.Easting = tempEasting;
                GeoPoint tempGeoPoint = new GeoPoint(result, Datum);
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
        public UtmPoint NorthWestCornerUtm()
        {
            String value = CentralPoint.ToUtmString(_Digits);
            UtmPoint result = new UtmPoint(value);
            if ( !SameZone(result) )
            {
                result = LimitToZone(result);
            }
            return result;
        }
        public UtmPoint NorthEastCornerUtm()
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
        public UtmPoint SouthWestCornerUtm()
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
        public UtmPoint SouthEastCornerUtm()
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

        protected override GeoPoint GetNorthWestCorner()
        {
            return new GeoPoint(NorthWestCornerUtm(),Datum);
        }
        protected override GeoPoint GetNorthEastCorner()
        {
            return new GeoPoint(NorthEastCornerUtm(), Datum);
        }
        protected override GeoPoint GetSouthWestCorner()
        {
            return new GeoPoint(SouthWestCornerUtm(), Datum);
        }
        protected override GeoPoint GetSouthEastCorner()
        {
            return new GeoPoint(SouthEastCornerUtm(), Datum);
        }

        public List<MgrsGridElement> SubGrids()
        {
            List<MgrsGridElement> result = new List<MgrsGridElement>();
            String start = Name.Substring(0, 5);
            String numbers = Name.Remove(0, 5);
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

        #endregion

        public MgrsGridElement()
        {
            Datum = GeoDatum.DatumWGS84();
        }

        private void SetMgrs(String value)
        {
            String mgrs = value.Replace(" ", "");
            Int16 digits = Convert.ToInt16((mgrs.Length - 1) / 2);
            CentralPoint = MakeCentral(UtmPoint.ParseMgrsString(value), digits);
            _Digits = digits;
            Name = value;
        }

        public MgrsGridElement(String mgrs)
        {
            Datum = GeoDatum.DatumWGS84();
            SetMgrs(mgrs);
        }
    }
}
