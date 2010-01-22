using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class GeoPoint : ICloneable
    {
        #region constants
        private double dScaleFactor = 0.9996; // scale factor, used as k0
        private double dCvtRad2Deg = 180.0 / Math.PI; // 57.2957795130823208767 ...
        private GeoDatum mDatum = GeoDatum.DatumWGS84();
        #endregion
        #region properties
        private double Altitude { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public GeoDatum Datum { get { return mDatum; } set { SetDatum(value); } }
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
        public GeoPoint(string iValue)
        {
            throw new NotImplementedException();
        }
        public GeoPoint(GeoPoint iValue)
        {
            Latitude = iValue.Latitude;
            Longitude = iValue.Longitude;
            Datum = (GeoDatum)iValue.Datum.Clone();
        }
        public GeoPoint(double iUTMNorthing, double iUTMEasting, string iUTMZone, GeoDatum iDatum)
        {
            double eccSquared = iDatum.Ellipsoid.ExcentricitySquared;
            double dEquatorialRadius = iDatum.Ellipsoid.SemiMajorAxis;

            // populate North/South
            char cZoneLetter = iUTMZone[iUTMZone.Length - 1];
            bool bNorthernHemisphere = (cZoneLetter >= 'N');

            string sZoneNum = iUTMZone.Substring(0, iUTMZone.Length - 1);
            int iZoneNumber = Convert.ToInt32(sZoneNum);

            double x = iUTMEasting - 500000.0; //remove 500,000 meter offset for longitude
            double y = iUTMNorthing;
            if (!bNorthernHemisphere)
            {
                // point is in southern hemisphere
                y -= 10000000.0; // remove 10,000,000 meter offset used for southern hemisphere
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
        protected void SetDatum(GeoDatum iNewDatum)
        {
            // Source http://home.hiwaay.net/~taylorc/bookshelf/math-science/geodesy/datum/transform/molodensky/
            double slat = Math.Sin(Latitude);
            double clat = Math.Cos(Latitude);
            double slon = Math.Sin(Longitude);
            double clon = Math.Cos(Longitude);
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

            Longitude = Longitude + dlon;
            Latitude = Latitude + dlat;
            Altitude = Altitude + dh;
            mDatum = iNewDatum;
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
        public void CalcUTM(out double oNorthing, out double oEasting, out string oZone)
        {
            throw new NotImplementedException();
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

        private static String CoordinateToString(string iFormat, double iValue)
        {
            String lResult = iFormat;

            lResult = lResult.Replace("%C", Math.Truncate(iValue).ToString());
            lResult = lResult.Replace("%D", Math.Truncate(Math.Abs(iValue)).ToString());
            lResult = lResult.Replace("%M", Math.Truncate((Math.Abs(iValue) * 60) % 60).ToString());
            lResult = lResult.Replace("%S", Math.Truncate((Math.Abs(iValue)*3600) % 60).ToString());

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
            String lLatitude = CoordinateToString(iFormat,Latitude);
            if (Latitude >= 0)
            {
                lLatitude = lLatitude.Replace("%H", "N");
            }
            else
            {
                lLatitude = lLatitude.Replace("%H", "S");
            }
            String lLongitude = CoordinateToString(iFormat, Longitude);
            if (Longitude >= 0)
            {
                lLongitude = lLongitude.Replace("%H", "E");
            }
            else
            {
                lLongitude = lLongitude.Replace("%H", "W");
            }

            String lResult = lLatitude + ' ' + lLongitude;
            lResult = lResult.Replace("%%","%");
            return lResult;
        }
        public String ToUTMString()
        {
            throw new NotImplementedException();
        }
        public String ToMGRSString()
        {
            throw new NotImplementedException();
        }

        public static GeoPoint ParseMGRS(String iValue)
        {
            String lValue = Helper.ReplaceThaiNumerals(iValue).ToUpperInvariant();
            // TODO

            // VE ๒๐๔๕๐๘ -> VE 204508 -> 48Q VE 204 508 -> 48Q 420400 1950800

            throw new NotImplementedException();
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
