using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Geo
{
    public class GeoPoint : ICloneable, IEquatable<GeoPoint>
    {
        #region constants
        private double dScaleFactor = 0.9996; // scale factor, used as k0
        private double dCvtRad2Deg = 180.0 / Math.PI; // 57.2957795130823208767 ...
        private GeoDatum mDatum = GeoDatum.DatumWGS84();
        private Int32 mGeoHashDefaultAccuracy = 9;
        private Int32 mMaidenheadDefaultAccuracy = 9;
        private PositionInRectangle mDefaultPositionInRectangle = PositionInRectangle.MiddleMiddle;
        #endregion

        #region properties
        public double Altitude { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public GeoDatum Datum { get { return mDatum; } set { SetDatum(value); } }
        public String GeoHash { get { return CalcGeoHash(mGeoHashDefaultAccuracy); } set { SetGeoHash(value); } }
        public String Maidenhead { get { return CalcMaidenhead(mMaidenheadDefaultAccuracy); } set { SetMaidenhead(value); } }

        private void SetMaidenhead(String iValue)
        {
            Double lLatitude = 0;
            Double lLongitude = 0;
            MaidenheadLocator.GeographicalCoordinatesByMaidenheadLocator(iValue, mDefaultPositionInRectangle, out lLatitude, out lLongitude);
            Datum = GeoDatum.DatumWGS84();
            Latitude = lLatitude;
            Longitude = lLongitude;
        }
        private String CalcMaidenhead(Int32 iPrecision)
        {

            String lResult = MaidenheadLocator.GetMaidenheadLocator(Latitude, Latitude, true, iPrecision);
            return lResult;
        }
        #endregion

        #region constructor
        public GeoPoint()
        {
        }
        public GeoPoint(double iLatitude, double iLongitude)
        {
            Latitude = iLatitude;
            Longitude = iLongitude;
        }
        public GeoPoint(double iLatitude, double iLongitude, double iAltitude, GeoDatum iDatum)
        {
            Latitude = iLatitude;
            Longitude = iLongitude;
            Altitude = iAltitude;
            mDatum = iDatum;
        }
        public GeoPoint(string iValue)
        {
            throw new NotImplementedException();
        }
        public GeoPoint(GeoPoint iValue)
        {
            Latitude = iValue.Latitude;
            Longitude = iValue.Longitude;
            Altitude = iValue.Altitude;
            mDatum = (GeoDatum)iValue.Datum.Clone();
        }
        public GeoPoint(UTMPoint iUTMPoint, GeoDatum iDatum)
        {
            double eccSquared = iDatum.Ellipsoid.ExcentricitySquared;
            double dEquatorialRadius = iDatum.Ellipsoid.SemiMajorAxis;

            bool bNorthernHemisphere = iUTMPoint.IsNorthernHemisphere;
            int iZoneNumber = iUTMPoint.ZoneNumber;

            double x = iUTMPoint.Easting - 500000.0; //remove 500,000 meter offset for longitude
            double y = iUTMPoint.Northing;
            if ( !bNorthernHemisphere )
            {
                // point is in southern hemisphere
                y = y - 10000000.0; // remove 10,000,000 meter offset used for southern hemisphere
            }

            double dLongOrigin = (iZoneNumber - 1) * 6 - 180 + 3; // +3 puts origin in middle of zone

            double eccPrimeSquared = (eccSquared) / (1 - eccSquared);

            double M = y / dScaleFactor;
            double mu = M / (dEquatorialRadius * (1 - eccSquared / 4 - 3 * eccSquared * eccSquared / 64 - 5 * eccSquared * eccSquared * eccSquared / 256));

            double e1 = (1 - Math.Sqrt(1 - eccSquared)) / (1 + Math.Sqrt(1 - eccSquared));
            // phi in radians
            double phi1Rad = mu + (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32) * Math.Sin(2 * mu)
                  + (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32) * Math.Sin(4 * mu)
                  + (151 * e1 * e1 * e1 / 96) * Math.Sin(6 * mu);
            // convert to degrees
            double phi1 = phi1Rad * dCvtRad2Deg;

            double N1 = dEquatorialRadius / Math.Sqrt(1 - eccSquared * Math.Sin(phi1Rad) * Math.Sin(phi1Rad));
            double T1 = Math.Tan(phi1Rad) * Math.Tan(phi1Rad);
            double C1 = eccPrimeSquared * Math.Cos(phi1Rad) * Math.Cos(phi1Rad);
            double R1 = dEquatorialRadius * (1 - eccSquared) / Math.Pow(1 - eccSquared * Math.Sin(phi1Rad) * Math.Sin(phi1Rad), 1.5);
            double D = x / (N1 * dScaleFactor);

            // phi in radians
            double dLat = phi1Rad - (N1 * Math.Tan(phi1Rad) / R1) * (D * D / 2 - (5 + 3 * T1 + 10 * C1 - 4 * C1 * C1 - 9 * eccPrimeSquared) * D * D * D * D / 24
                    + (61 + 90 * T1 + 298 * C1 + 45 * T1 * T1 - 252 * eccPrimeSquared - 3 * C1 * C1) * D * D * D * D * D * D / 720);
            // convert to degrees
            dLat = dLat * dCvtRad2Deg;

            // lon in radians
            double dLon = (D - (1 + 2 * T1 + C1) * D * D * D / 6 + (5 - 2 * C1 + 28 * T1 - 3 * C1 * C1 + 8 * eccPrimeSquared + 24 * T1 * T1)
                    * D * D * D * D * D / 120) / Math.Cos(phi1Rad);
            // convert to degrees
            dLon = dLongOrigin + dLon * dCvtRad2Deg;

            Longitude = dLon;
            Latitude = dLat;
            mDatum = iDatum;
        }
        #endregion

        #region methods
        public bool IsNorthernHemisphere()
        {
            return Latitude >= 0;
        }
        public bool IsWesternLongitude()
        {
            return Longitude < 0;
        }
        protected void SetDatum(GeoDatum iNewDatum)
        {
            // Source http://home.hiwaay.net/~taylorc/bookshelf/math-science/geodesy/datum/transform/molodensky/
            double LatRad = Latitude / dCvtRad2Deg;
            double LongRad = Longitude / dCvtRad2Deg;
            double slat = Math.Sin(LatRad);
            double clat = Math.Cos(LatRad);
            double slon = Math.Sin(LongRad);
            double clon = Math.Cos(LongRad);
            double from_a = Datum.Ellipsoid.SemiMajorAxis;
            double from_f = Datum.Ellipsoid.Flattening;
            double from_esq = Datum.Ellipsoid.ExcentricitySquared;
            double ssqlat = slat * slat;
            double adb = 1.0 / (1.0 - from_f);  // "a divided by b"
            double da = iNewDatum.Ellipsoid.SemiMajorAxis - Datum.Ellipsoid.SemiMajorAxis;
            double df = iNewDatum.Ellipsoid.Flattening - Datum.Ellipsoid.Flattening;
            double dx = -iNewDatum.deltaX + Datum.deltaX;
            double dy = -iNewDatum.deltaY + Datum.deltaY;
            double dz = -iNewDatum.deltaZ + Datum.deltaZ;

            double rn = from_a / Math.Sqrt(1.0 - from_esq * ssqlat);
            double rm = from_a * (1.0 - from_esq) / Math.Pow((1.0 - from_esq * ssqlat), 1.5);

            double dlat = (((((-dx * slat * clon - dy * slat * slon) + dz * clat)
                        + (da * ((rn * from_esq * slat * clat) / from_a)))
                    + (df * (rm * adb + rn / adb) * slat * clat)))
                / (rm + Altitude);

            double dlon = (-dx * slon + dy * clon) / ((rn + Altitude) * clat);

            double dh = (dx * clat * clon) + (dy * clat * slon) + (dz * slat)
                 - (da * (from_a / rn)) + ((df * rn * ssqlat) / adb);

            Longitude = Longitude + dlon * dCvtRad2Deg;
            Latitude = Latitude + dlat * dCvtRad2Deg;
            Altitude = Altitude + dh;
            mDatum = iNewDatum;
        }
        internal static void ShiftPositionInRectangle(ref Double ioLatitude, ref Double lLongitude, PositionInRectangle lPositionInRectangle, Double iHeight, Double iWidth)
        {
            switch ( lPositionInRectangle )
            {
                case PositionInRectangle.TopLeft:
                case PositionInRectangle.TopMiddle:
                case PositionInRectangle.TopRight:
                    ioLatitude += iHeight;
                    break;
            }

            switch ( lPositionInRectangle )
            {
                case PositionInRectangle.MiddleLeft:
                case PositionInRectangle.MiddleMiddle:
                case PositionInRectangle.MiddleRight:
                    ioLatitude += iHeight / 2;
                    break;
            }

            switch ( lPositionInRectangle )
            {
                case PositionInRectangle.TopRight:
                case PositionInRectangle.MiddleRight:
                case PositionInRectangle.BottomRight:
                    lLongitude += iWidth;
                    break;
            }

            switch ( lPositionInRectangle )
            {
                case PositionInRectangle.TopMiddle:
                case PositionInRectangle.MiddleMiddle:
                case PositionInRectangle.BottomMiddle:
                    lLongitude += iWidth / 2;
                    break;
            }
        }

        public void ExportToKml(XmlNode iNode)
        {
            XmlDocument lXmlDocument = Helper.XmlDocumentFromNode(iNode);
            var lNewElement = (XmlElement)lXmlDocument.CreateNode("element", "Point", "");
            iNode.AppendChild(lNewElement);

            var lCoordinatesElement = (XmlElement)lXmlDocument.CreateNode("element", "coordinates", "");
            lCoordinatesElement.InnerText =
                Latitude.ToString(Helper.CultureInfoUS) + ',' +
                Longitude.ToString(Helper.CultureInfoUS) + ",0";
            lNewElement.AppendChild(lCoordinatesElement);
        }
        public void ExportToXML(XmlElement iNode)
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
        public UTMPoint CalcUTM()
        {
            //converts lat/long to UTM coords.  Equations from USGS Bulletin 1532 
            //East Longitudes are positive, West longitudes are negative. 
            //North latitudes are positive, South latitudes are negative
            //Lat and Long are in decimal degrees

            double eccSquared = mDatum.Ellipsoid.ExcentricitySquared;
            double dEquatorialRadius = mDatum.Ellipsoid.SemiMajorAxis;

            double k0 = 0.9996;

            double LongOrigin;
            double eccPrimeSquared = (eccSquared) / (1 - eccSquared);

            //Make sure the longitude is between -180.00 .. 179.9
            double lLongitude = (Longitude + 180) - Math.Truncate((Longitude + 180) / 360) * 360 - 180; // -180.00 .. 179.9;

            double LatRad = Latitude / dCvtRad2Deg;
            double LongRad = Longitude / dCvtRad2Deg;

            Int32 ZoneNumber = (Int32)Math.Truncate((lLongitude + 180) / 6) + 1;

            if ( Latitude >= 56.0 && Latitude < 64.0 && lLongitude >= 3.0 && lLongitude < 12.0 )
            {
                ZoneNumber = 32; // larger zone for southern Norway
            }
            if ( Latitude >= 72.0 && Latitude < 84.0 )
            {
                // Special zones for Svalbard
                if ( lLongitude >= 0.0 && lLongitude < 9.0 )
                {
                    ZoneNumber = 31;
                }
                else if ( lLongitude >= 9.0 && lLongitude < 21.0 )
                {
                    ZoneNumber = 33;
                }
                else if ( lLongitude >= 21.0 && lLongitude < 33.0 )
                {
                    ZoneNumber = 35;
                }
                else if ( lLongitude >= 33.0 && lLongitude < 42.0 )
                {
                    ZoneNumber = 37;
                }
            }
            LongOrigin = (ZoneNumber - 1) * 6 - 180 + 3;  //+3 puts origin in middle of zone
            double LongOriginRad = LongOrigin / dCvtRad2Deg;

            double N = dEquatorialRadius / Math.Sqrt(1 - eccSquared * Math.Sin(LatRad) * Math.Sin(LatRad));
            double T = Math.Tan(LatRad) * Math.Tan(LatRad);
            double C = eccPrimeSquared * Math.Cos(LatRad) * Math.Cos(LatRad);
            double A = Math.Cos(LatRad) * (LongRad - LongOriginRad);
            double M = dEquatorialRadius * ((1 - eccSquared / 4 - 3 * eccSquared * eccSquared / 64 - 5 * eccSquared * eccSquared * eccSquared / 256) * LatRad
                - (3 * eccSquared / 8 + 3 * eccSquared * eccSquared / 32 + 45 * eccSquared * eccSquared * eccSquared / 1024) * Math.Sin(2 * LatRad)
                                    + (15 * eccSquared * eccSquared / 256 + 45 * eccSquared * eccSquared * eccSquared / 1024) * Math.Sin(4 * LatRad)
                                    - (35 * eccSquared * eccSquared * eccSquared / 3072) * Math.Sin(6 * LatRad));

            double UTMEasting = (double)(k0 * N * (A + (1 - T + C) * A * A * A / 6
                    + (5 - 18 * T + T * T + 72 * C - 58 * eccPrimeSquared) * A * A * A * A * A / 120)
                    + 500000.0);
            double UTMNorthing = (double)(k0 * (M + N * Math.Tan(LatRad) * (A * A / 2 + (5 - T + 9 * C + 4 * C * C) * A * A * A * A / 24
                 + (61 - 58 * T + T * T + 600 * C - 330 * eccPrimeSquared) * A * A * A * A * A * A / 720)));

            if ( Latitude < 0 )
            {
                UTMNorthing += 10000000.0; //10000000 meter offset for southern hemisphere
            }
            UTMPoint lResult = new UTMPoint(
                (Int32)Math.Truncate(UTMEasting),
                (Int32)Math.Truncate(UTMNorthing),
                ZoneNumber, Latitude >= 0);
            return lResult;
        }

        public static GeoPoint Load(XmlNode iNode)
        {
            GeoPoint RetVal = null;

            if ( iNode != null && iNode.Name.Equals("geo:Point") )
            {
                RetVal = new GeoPoint();

                if ( iNode.HasChildNodes )
                {
                    foreach ( XmlNode lChildNode in iNode.ChildNodes )
                    {
                        if ( lChildNode.Name == "geo:lat" )
                        {
                            String lLatitude = lChildNode.InnerText;
                            RetVal.Latitude = Convert.ToDouble(lLatitude, Helper.CultureInfoUS);
                        }
                        if ( lChildNode.Name == "geo:long" )
                        {
                            String lLongitude = lChildNode.InnerText;
                            RetVal.Longitude = Convert.ToDouble(lLongitude, Helper.CultureInfoUS);
                        }
                    }
                }
            }
            return RetVal;

        }

        private static String CoordinateToString(string iFormat, double iValue)
        {
            String lResult = iFormat;

            lResult = lResult.Replace("%C", Math.Truncate(iValue).ToString());
            lResult = lResult.Replace("%D", Math.Truncate(Math.Abs(iValue)).ToString());
            lResult = lResult.Replace("%M", Math.Truncate((Math.Abs(iValue) * 60) % 60).ToString());
            lResult = lResult.Replace("%S", Math.Truncate((Math.Abs(iValue) * 3600) % 60).ToString());

            // http://www.codeproject.com/KB/string/llstr.aspx
            // String.Format() ?

            // %C - integer co-ordinate, may be negative or positive
            // %c - decimal co-ordinate, the entire co-ordinate, may be negative or positive
            // %D - integer degrees, always positive
            // %M - integer degrees, always positive
            // %S - integer seconds, always positive, rounded
            // %d - decimal degrees, always positive
            // %m - decimal minutes, always positive
            // %s - decimal seconds, always positive
            return lResult;
        }
        public String ToString(string iFormat)
        {
            // %H - hemisphere - single character of N,S,E,W
            // %C - integer co-ordinate, may be negative or positive
            // %c - decimal co-ordinate, the entire co-ordinate, may be negative or positive
            // %D - integer degrees, always positive
            // %M - integer degrees, always positive
            // %S - integer seconds, always positive, rounded
            // %d - decimal degrees, always positive
            // %m - decimal minutes, always positive
            // %s - decimal seconds, always positive
            // %% - for %
            String lLatitude = CoordinateToString(iFormat, Latitude);
            if ( Latitude >= 0 )
            {
                lLatitude = lLatitude.Replace("%H", "N");
            }
            else
            {
                lLatitude = lLatitude.Replace("%H", "S");
            }
            String lLongitude = CoordinateToString(iFormat, Longitude);
            if ( Longitude >= 0 )
            {
                lLongitude = lLongitude.Replace("%H", "E");
            }
            else
            {
                lLongitude = lLongitude.Replace("%H", "W");
            }

            String lResult = lLatitude + ' ' + lLongitude;
            lResult = lResult.Replace("%%", "%");
            return lResult;
        }
        public override String ToString()
        {
            // TODO use ToString(Format) instead
            String lLatitude = Math.Abs(Latitude).ToString("#0.0000") + "° ";
            if ( IsNorthernHemisphere() )
            {
                lLatitude = lLatitude + 'N';
            }
            else
            {
                lLatitude = lLatitude + 'S';
            }
            String lLongitude = Math.Abs(Longitude).ToString("##0.0000") + "° ";
            if ( IsWesternLongitude() )
            {
                lLongitude = lLongitude + 'W';
            }
            else
            {
                lLongitude = lLongitude + 'E';
            }
            String lResult = lLatitude + ' ' + lLongitude;
            return lResult;
        }

        private String CalcGeoHash(Int32 iAccuracy)
        {
            return De.AHoerstemeier.Geo.GeoHash.EncodeGeoHash(this, iAccuracy);
        }
        private void SetGeoHash(String iValue)
        {
            GeoPoint lPoint = De.AHoerstemeier.Geo.GeoHash.DecodeGeoHash(iValue);
            this.Latitude = lPoint.Latitude;
            this.Longitude = lPoint.Longitude;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new GeoPoint(this);
        }

        #endregion
        #region IEquatable Members
        const double mAltitudeAccuracy = 0.01;  // 10 cm
        const double mDegreeAccuracy = 0.00005;  // ca. 0.5 ArcSecond
        public bool Equals(GeoPoint iObj)
        {
            double lAltitudeError = Math.Abs(iObj.Altitude - this.Altitude);
            double lLatitudeError = Math.Abs(iObj.Latitude - this.Latitude);
            double lLongitudeError = Math.Abs(iObj.Longitude - this.Longitude);
            bool lResult = (lAltitudeError < mAltitudeAccuracy)
                & (lLatitudeError < mDegreeAccuracy)
                & (lLongitudeError < mDegreeAccuracy)
                & (iObj.Datum.Equals(this.Datum));
            return lResult;
        }
        #endregion

    }
}
