using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Globalization;

namespace De.AHoerstemeier.Geo
{
    public class GeoPoint : ICloneable, IEquatable<GeoPoint>
    {
        #region constants
        private double dScaleFactor = 0.9996; // scale factor, used as k0
        private double _convertRadianToDegree = 180.0 / Math.PI; // 57.2957795130823208767 ...
        private GeoDatum _Datum = GeoDatum.DatumWGS84();
        private Int32 _GeoHashDefaultAccuracy = 9;
        private Int32 _MaidenheadDefaultAccuracy = 9;
        private PositionInRectangle _DefaultPositionInRectangle = PositionInRectangle.MiddleMiddle;
        #endregion

        #region properties
        public double Altitude { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public GeoDatum Datum { get { return _Datum; } set { SetDatum(value); } }
        public String GeoHash { get { return CalcGeoHash(_GeoHashDefaultAccuracy); } set { SetGeoHash(value); } }
        public String Maidenhead { get { return CalcMaidenhead(_MaidenheadDefaultAccuracy); } set { SetMaidenhead(value); } }

        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the GeoPoint class.
        /// </summary>
        public GeoPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the GeoPoint class.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when latitude is larger than ±90° or longitude is larger than ±180°</exception>
        /// <param name="latitude">Latitude in degrees.</param>
        /// <param name="longitude">Longitude in degrees.</param>
        public GeoPoint(double latitude, double longitude)
        {
            if ( Math.Abs(latitude) > 90 )
            {
                throw new ArgumentOutOfRangeException("latitude");
            }
            if ( Math.Abs(longitude) > 180 )
            {
                throw new ArgumentOutOfRangeException("longitude");
            }
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Initializes a new instance of the GeoPoint class.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when latitude is larger than ±90° or longitude is larger than ±180°.</exception>
        /// <exception cref="ArgumentNullException">Thrown when datum is null.</exception>
        /// <param name="latitude">Latitude in degrees.</param>
        /// <param name="longitude">Longitude in degrees.</param>
        /// <param name="altitude">Altitude in meter.</param>
        /// <param name="datum">Geographical datum.</param>
        public GeoPoint(double latitude, double longitude, double altitude, GeoDatum datum)
        {
            if ( Math.Abs(latitude) > 90 )
            {
                throw new ArgumentOutOfRangeException("latitude");
            }
            if ( Math.Abs(longitude) > 180 )
            {
                throw new ArgumentOutOfRangeException("longitude");
            }
            if ( datum == null )
            {
                throw new ArgumentNullException("datum");
            }

            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            _Datum = datum;
        }

        /// <summary>
        /// Initializes a new instance of the GeoPoint class.
        /// </summary>
        /// <param name="value">String representation of a geographical coordinate.</param>
        /// <exception cref="ArgumentException">Raise when the string could not be parsed.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when parsed latitude is larger than ±90° or longitude is larger than ±180°</exception>
        public GeoPoint(String value)
        {
            GeoPoint result = null;

            try
            {
                result = GeoPoint.ParseDegMinSec(value);
            }
            catch ( ArgumentException )
            {
            }

            if ( result == null )
            {
                try
                {
                    result = GeoPoint.ParseDegMin(value);
                }
                catch ( ArgumentException )
                {
                }
            }

            if ( result == null )
            {
                try
                {
                    result = GeoPoint.ParseDecimalDegree(value);
                }
                catch ( ArgumentException )
                {
                }
            }

            if ( result == null )
            {
                throw new ArgumentException("Cannot parse coordinate value " + value, "value");
            }

            Latitude = result.Latitude;
            Longitude = result.Longitude;
        }

        /// <summary>
        /// Initializes a new instance of the GeoPoint class.
        /// </summary>
        /// <param name="value">GeoPoint to copy data from.</param>
        public GeoPoint(GeoPoint value)
        {
            Latitude = value.Latitude;
            Longitude = value.Longitude;
            Altitude = value.Altitude;
            _Datum = (GeoDatum)value.Datum.Clone();
        }

        /// <summary>
        /// Initializes a new instance of the GeoPoint class.
        /// </summary>
        /// <param name="utmPoint">UTM coordinates.</param>
        /// <param name="datum">Geographical datum.</param>
        public GeoPoint(UtmPoint utmPoint, GeoDatum datum)
        {
            Double excentricitySquared = datum.Ellipsoid.ExcentricitySquared;
            Double equatorialRadius = datum.Ellipsoid.SemiMajorAxis;

            Boolean northernHemisphere = utmPoint.IsNorthernHemisphere;
            Int32 zoneNumber = utmPoint.ZoneNumber;

            Double x = utmPoint.Easting - 500000.0; //remove 500,000 meter offset for longitude
            Double y = utmPoint.Northing;
            if ( !northernHemisphere )
            {
                // point is in southern hemisphere
                y = y - 10000000.0; // remove 10,000,000 meter offset used for southern hemisphere
            }

            Double longOrigin = (zoneNumber - 1) * 6 - 180 + 3; // +3 puts origin in middle of zone

            Double excentricityPrimeSquared = (excentricitySquared) / (1 - excentricitySquared);

            Double M = y / dScaleFactor;
            Double mu = M / (equatorialRadius * (1 - excentricitySquared / 4 - 3 * excentricitySquared * excentricitySquared / 64 - 5 * excentricitySquared * excentricitySquared * excentricitySquared / 256));

            Double e1 = (1 - Math.Sqrt(1 - excentricitySquared)) / (1 + Math.Sqrt(1 - excentricitySquared));
            // phi in radians
            Double phi1Rad = mu + (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32) * Math.Sin(2 * mu)
                  + (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32) * Math.Sin(4 * mu)
                  + (151 * e1 * e1 * e1 / 96) * Math.Sin(6 * mu);
            // convert to degrees
            Double phi1 = phi1Rad * _convertRadianToDegree;

            Double N1 = equatorialRadius / Math.Sqrt(1 - excentricitySquared * Math.Sin(phi1Rad) * Math.Sin(phi1Rad));
            Double T1 = Math.Tan(phi1Rad) * Math.Tan(phi1Rad);
            Double C1 = excentricityPrimeSquared * Math.Cos(phi1Rad) * Math.Cos(phi1Rad);
            Double R1 = equatorialRadius * (1 - excentricitySquared) / Math.Pow(1 - excentricitySquared * Math.Sin(phi1Rad) * Math.Sin(phi1Rad), 1.5);
            Double D = x / (N1 * dScaleFactor);

            // phi in radians
            Double latitude = phi1Rad - (N1 * Math.Tan(phi1Rad) / R1) * (D * D / 2 - (5 + 3 * T1 + 10 * C1 - 4 * C1 * C1 - 9 * excentricityPrimeSquared) * D * D * D * D / 24
                    + (61 + 90 * T1 + 298 * C1 + 45 * T1 * T1 - 252 * excentricityPrimeSquared - 3 * C1 * C1) * D * D * D * D * D * D / 720);
            // convert to degrees
            latitude = latitude * _convertRadianToDegree;

            // lon in radians
            Double longitude = (D - (1 + 2 * T1 + C1) * D * D * D / 6 + (5 - 2 * C1 + 28 * T1 - 3 * C1 * C1 + 8 * excentricityPrimeSquared + 24 * T1 * T1)
                    * D * D * D * D * D / 120) / Math.Cos(phi1Rad);
            // convert to degrees
            longitude = longOrigin + longitude * _convertRadianToDegree;

            Longitude = longitude;
            Latitude = latitude;
            _Datum = datum;
        }
        #endregion

        #region methods
        /// <summary>
        /// Checks whether the point is on the northern hemisphere.
        /// </summary>
        /// <returns>True if on northern hemisphere or exactly at equator, false otherwise.</returns>
        public Boolean IsNorthernHemisphere()
        {
            return Latitude >= 0;
        }

        /// <summary>
        /// Checks whether the point is west of Greenwich.
        /// </summary>
        /// <returns>True if west of Greenwich or exactly on meridian, false otherwise.</returns>
        public Boolean IsWesternLongitude()
        {
            return Longitude <= 0;
        }

        private void SetDatum(GeoDatum newDatum)
        {
            // Source http://home.hiwaay.net/~taylorc/bookshelf/math-science/geodesy/datum/transform/molodensky/
            double LatRad = Latitude / _convertRadianToDegree;
            double LongRad = Longitude / _convertRadianToDegree;
            double slat = Math.Sin(LatRad);
            double clat = Math.Cos(LatRad);
            double slon = Math.Sin(LongRad);
            double clon = Math.Cos(LongRad);
            double from_a = Datum.Ellipsoid.SemiMajorAxis;
            double from_f = Datum.Ellipsoid.Flattening;
            double from_esq = Datum.Ellipsoid.ExcentricitySquared;
            double ssqlat = slat * slat;
            double adb = 1.0 / (1.0 - from_f);  // "a divided by b"
            double da = newDatum.Ellipsoid.SemiMajorAxis - Datum.Ellipsoid.SemiMajorAxis;
            double df = newDatum.Ellipsoid.Flattening - Datum.Ellipsoid.Flattening;
            double dx = -newDatum.DeltaX + Datum.DeltaX;
            double dy = -newDatum.DeltaY + Datum.DeltaY;
            double dz = -newDatum.DeltaZ + Datum.DeltaZ;

            double rn = from_a / Math.Sqrt(1.0 - from_esq * ssqlat);
            double rm = from_a * (1.0 - from_esq) / Math.Pow((1.0 - from_esq * ssqlat), 1.5);

            double dlat = (((((-dx * slat * clon - dy * slat * slon) + dz * clat)
                        + (da * ((rn * from_esq * slat * clat) / from_a)))
                    + (df * (rm * adb + rn / adb) * slat * clat)))
                / (rm + Altitude);

            double dlon = (-dx * slon + dy * clon) / ((rn + Altitude) * clat);

            double dh = (dx * clat * clon) + (dy * clat * slon) + (dz * slat)
                 - (da * (from_a / rn)) + ((df * rn * ssqlat) / adb);

            Longitude = Longitude + dlon * _convertRadianToDegree;
            Latitude = Latitude + dlat * _convertRadianToDegree;
            Altitude = Altitude + dh;
            _Datum = newDatum;
        }
        internal static void ShiftPositionInRectangle(ref Double latitude, ref Double longitude, PositionInRectangle positionInRectangle, Double height, Double width)
        {
            switch ( positionInRectangle )
            {
                case PositionInRectangle.TopLeft:
                case PositionInRectangle.TopMiddle:
                case PositionInRectangle.TopRight:
                    latitude += height;
                    break;
            }

            switch ( positionInRectangle )
            {
                case PositionInRectangle.MiddleLeft:
                case PositionInRectangle.MiddleMiddle:
                case PositionInRectangle.MiddleRight:
                    latitude += height / 2;
                    break;
            }

            switch ( positionInRectangle )
            {
                case PositionInRectangle.TopRight:
                case PositionInRectangle.MiddleRight:
                case PositionInRectangle.BottomRight:
                    longitude += width;
                    break;
            }

            switch ( positionInRectangle )
            {
                case PositionInRectangle.TopMiddle:
                case PositionInRectangle.MiddleMiddle:
                case PositionInRectangle.BottomMiddle:
                    longitude += width / 2;
                    break;
            }
        }

        public void ExportToKml(XmlNode node)
        {
            XmlDocument lXmlDocument = Helper.XmlDocumentFromNode(node);
            var lNewElement = (XmlElement)lXmlDocument.CreateNode("element", "Point", "");
            node.AppendChild(lNewElement);

            var lCoordinatesElement = (XmlElement)lXmlDocument.CreateNode("element", "coordinates", "");
            lCoordinatesElement.InnerText =
                Latitude.ToString(Helper.CultureInfoUS) + ',' +
                Longitude.ToString(Helper.CultureInfoUS) + ",0";
            lNewElement.AppendChild(lCoordinatesElement);
        }
        public void ExportToXML(XmlElement node)
        {
            XmlDocument lXmlDocument = Helper.XmlDocumentFromNode(node);
            var lNewElement = (XmlElement)lXmlDocument.CreateNode("element", "geo:Point", "");
            node.AppendChild(lNewElement);

            var lLatitudeElement = (XmlElement)lXmlDocument.CreateNode("element", "geo:lat", "");
            lLatitudeElement.InnerText = Latitude.ToString(Helper.CultureInfoUS);
            lNewElement.AppendChild(lLatitudeElement);

            var lLongitudeElement = (XmlElement)lXmlDocument.CreateNode("element", "get:long", "");
            lLongitudeElement.InnerText = Longitude.ToString(Helper.CultureInfoUS);
            lNewElement.AppendChild(lLongitudeElement);
        }
        public UtmPoint CalcUTM()
        {
            //converts lat/long to UTM coords.  Equations from USGS Bulletin 1532 
            //East Longitudes are positive, West longitudes are negative. 
            //North latitudes are positive, South latitudes are negative
            //Lat and Long are in decimal degrees

            double eccSquared = _Datum.Ellipsoid.ExcentricitySquared;
            double dEquatorialRadius = _Datum.Ellipsoid.SemiMajorAxis;

            double k0 = 0.9996;

            double LongOrigin;
            double eccPrimeSquared = (eccSquared) / (1 - eccSquared);

            //Make sure the longitude is between -180.00 .. 179.9
            double lLongitude = (Longitude + 180) - Math.Truncate((Longitude + 180) / 360) * 360 - 180; // -180.00 .. 179.9;

            double LatRad = Latitude / _convertRadianToDegree;
            double LongRad = Longitude / _convertRadianToDegree;

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
            double LongOriginRad = LongOrigin / _convertRadianToDegree;

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
            UtmPoint lResult = new UtmPoint(
                (Int32)Math.Truncate(UTMEasting),
                (Int32)Math.Truncate(UTMNorthing),
                ZoneNumber, Latitude >= 0);
            return lResult;
        }

        public static GeoPoint Load(XmlNode node)
        {
            GeoPoint RetVal = null;

            if ( node != null && node.Name.Equals("geo:Point") )
            {
                RetVal = new GeoPoint();

                if ( node.HasChildNodes )
                {
                    foreach ( XmlNode lChildNode in node.ChildNodes )
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

        private static String CoordinateToString(String format, Double value)
        {
            String lResult = format;

            lResult = lResult.Replace("%C", Math.Truncate(value).ToString());
            lResult = lResult.Replace("%D", Math.Truncate(Math.Abs(value)).ToString());
            lResult = lResult.Replace("%M", Math.Truncate((Math.Abs(value) * 60) % 60).ToString());
            lResult = lResult.Replace("%S", Math.Truncate((Math.Abs(value) * 3600) % 60).ToString());

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
        public String ToString(String format)
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
            String lLatitude = CoordinateToString(format, Latitude);
            if ( Latitude >= 0 )
            {
                lLatitude = lLatitude.Replace("%H", "N");
            }
            else
            {
                lLatitude = lLatitude.Replace("%H", "S");
            }
            String lLongitude = CoordinateToString(format, Longitude);
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

        private String CalcGeoHash(Int32 accuracy)
        {
            return De.AHoerstemeier.Geo.GeoHash.EncodeGeoHash(this, accuracy);
        }
        private void SetGeoHash(String value)
        {
            GeoPoint newPoint = De.AHoerstemeier.Geo.GeoHash.DecodeGeoHash(value);
            this.Latitude = newPoint.Latitude;
            this.Longitude = newPoint.Longitude;
        }

        private void SetMaidenhead(String value)
        {
            Double latitude = 0;
            Double longitude = 0;
            MaidenheadLocator.GeographicalCoordinatesByMaidenheadLocator(value, _DefaultPositionInRectangle, out latitude, out longitude);
            Datum = GeoDatum.DatumWGS84();
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
        private String CalcMaidenhead(Int32 precision)
        {
            String result = MaidenheadLocator.GetMaidenheadLocator(Latitude, Longitude, true, precision);
            return result;
        }

        private static GeoPoint ParseDegMinSec(String value)
        {
            Regex Parser = new Regex(@"([0-9]{1,3})[:|°]\s{0,}([0-9]{1,2})[:|']\s{0,}((?:\b[0-9]+(?:\.[0-9]*)?|\.[0-9]+\b))[""|\s]\s{0,}?([N|S|E|W])\s{0,}");

            value = value.Replace(',', '.');
            value = value.Trim();

            // Now parse using the regex parser
            MatchCollection matches = Parser.Matches(value);
            if ( matches.Count != 2 )
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "Lat/long value of '{0}' is not recognised", value));
            }

            Double latitude = 0.0;
            Double longitude = 0.0;

            foreach ( Match match in matches )
            {
                // Convert - adjust the sign if necessary
                Double deg = Double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                Double min = Double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                Double sec = Double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                Double result = deg + (min / 60) + (sec / 3600);
                if ( match.Groups[4].Success )
                {
                    Char ch = match.Groups[4].Value[0];
                    switch ( ch )
                    {
                        case 'S':
                            {
                                latitude = -result;
                                break;
                            }
                        case 'N':
                            {
                                latitude = result;
                                break;
                            }
                        case 'E':
                            {
                                longitude = result;
                                break;
                            }
                        case 'W':
                            {
                                longitude = -result;
                                break;
                            }
                    }
                }
            }
            return new GeoPoint(latitude, longitude);
        }
        private static GeoPoint ParseDegMin(String value)
        {
            Regex Parser = new Regex(@"([0-9]{1,3})[:|°|\s]\s{0,}((?:\b[0-9]+(?:\.[0-9]*)?|\.[0-9]+\b))'{0,1}\s{0,}?([N|S|E|W])\s{0,}");

            value = value.Replace(',', '.');
            value = value.Trim();

            // Now parse using the regex parser
            MatchCollection matches = Parser.Matches(value);
            if ( matches.Count != 2 )
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "Lat/long value of '{0}' is not recognised", value));
            }

            Double latitude = 0.0;
            Double longitude = 0.0;

            foreach ( Match match in matches )
            {
                // Convert - adjust the sign if necessary
                Double deg = Double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                Double min = Double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                Double result = deg + (min / 60);
                if ( match.Groups[3].Success )
                {
                    Char ch = match.Groups[3].Value[0];
                    switch ( ch )
                    {
                        case 'S':
                            {
                                latitude = -result;
                                break;
                            }
                        case 'N':
                            {
                                latitude = result;
                                break;
                            }
                        case 'E':
                            {
                                longitude = result;
                                break;
                            }
                        case 'W':
                            {
                                longitude = -result;
                                break;
                            }
                    }
                }
            }
            return new GeoPoint(latitude, longitude);
        }
        private static GeoPoint ParseDecimalDegree(String value)
        {
            Regex Parser = new Regex(@"((?:\b[0-9]+(?:\.[0-9]*)?|\.[0-9]+\b))°{0,1}\s{0,}?([N|S|E|W])\s{0,}");

            value = value.Replace(',', '.');
            value = value.Trim();

            // Now parse using the regex parser
            MatchCollection matches = Parser.Matches(value);
            if ( matches.Count != 2 )
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "Lat/long value of '{0}' is not recognised", value));
            }

            Double latitude = 0.0;
            Double longitude = 0.0;

            foreach ( Match match in matches )
            {
                // Convert - adjust the sign if necessary
                Double deg = Double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                Double result = deg;
                if ( match.Groups[2].Success )
                {
                    Char ch = match.Groups[2].Value[0];
                    switch ( ch )
                    {
                        case 'S':
                            {
                                latitude = -result;
                                break;
                            }
                        case 'N':
                            {
                                latitude = result;
                                break;
                            }
                        case 'E':
                            {
                                longitude = result;
                                break;
                            }
                        case 'W':
                            {
                                longitude = -result;
                                break;
                            }
                    }
                }
            }
            return new GeoPoint(latitude, longitude);
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new GeoPoint(this);
        }

        #endregion
        #region IEquatable Members
        const double _AltitudeAccuracy = 0.01;  // 10 cm
        const double _DegreeAccuracy = 0.00005;  // ca. 0.5 ArcSecond
        public bool Equals(GeoPoint value)
        {
            double altitudeError = Math.Abs(value.Altitude - this.Altitude);
            double latitudeError = Math.Abs(value.Latitude - this.Latitude);
            double longitudeError = Math.Abs(value.Longitude - this.Longitude);
            bool lResult = (altitudeError < _AltitudeAccuracy)
                & (latitudeError < _DegreeAccuracy)
                & (longitudeError < _DegreeAccuracy)
                & (value.Datum.Equals(this.Datum));
            return lResult;
        }
        #endregion

    }
}
