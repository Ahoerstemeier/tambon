using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Geo
{
    public class UtmPoint : ICloneable, IEquatable<UtmPoint>
    {
        public const Int16 MaximumDigits = 7;
        public const Int16 MinimumDigits = 2;
        #region properties
        public Int32 Northing { get; set; }
        public Int32 Easting { get; set; }
        public Int32 ZoneNumber { get; set; }
        // public char ZoneBand { get; set; }
        public Boolean IsNorthernHemisphere { get; set; }
        #endregion

        #region constructor
        public UtmPoint()
        {
        }
        public UtmPoint(String value)
        {
            Assign(ParseUtmString(value));
        }
        public UtmPoint(UtmPoint value)
        {
            Assign(value);
        }
        public UtmPoint(Int32 easting, Int32 northing, Int32 zoneNumber, Boolean isNorthernHemisphere)
        {
            Northing = northing;
            Easting = easting;
            ZoneNumber = zoneNumber;
            IsNorthernHemisphere = isNorthernHemisphere;
        }
        public UtmPoint(Double easting, Double northing, Int32 zoneNumber, Boolean isNorthernHemisphere)
        {
            Northing = Convert.ToInt32(Math.Round(northing));
            Easting = Convert.ToInt32(Math.Round(easting));
            ZoneNumber = zoneNumber;
            IsNorthernHemisphere = isNorthernHemisphere;
        }
        #endregion

        #region constants
        private const String _ZoneCharacters = "CDEFGHJKLMNPQRSTUVWX";
        #endregion

        #region methods
        public char ZoneBand()
        {
            char zoneChar = ZoneBand(this.Northing, this.IsNorthernHemisphere);
            return zoneChar;
        }
        static char ZoneBand(Int32 northing, bool isNorthernHemisphere)
        {
            UtmPoint tempPoint = new UtmPoint(0, northing, 1, isNorthernHemisphere);
            GeoPoint geoPoint = new GeoPoint(tempPoint, GeoDatum.DatumWGS84());
            char zoneChar = UtmLetterDesignator(geoPoint.Latitude);
            return zoneChar;
        }
        public String ToUtmString(Int16 digits)
        {
            Int16 actualDigits = MakeDigitValid(digits);
            String northing = Northing.ToString("0000000");
            String easting = Easting.ToString("0000000");
            easting = easting.Substring(0, actualDigits);
            northing = northing.Substring(0, actualDigits);
            String result = ZoneNumber.ToString("00") + ZoneBand() + ' ' + easting + ' ' + northing;
            return result;
        }
        static public Int16 MakeDigitValid(Int16 digits)
        {
            Int16 result = Math.Min(MaximumDigits, Math.Max(MinimumDigits, digits));
            return result;
        }
        public String ToMgrsString(Int16 digits)
        {
            String result = String.Empty;
            Int16 actualDigits = MakeDigitValid(digits);
            String northing = Northing.ToString("0000000");
            String easting = Easting.ToString("0000000");
            String eastingLetters = MgrsEastingChars(ZoneNumber);
            Int32 eastingIdentifier = Convert.ToInt32(easting.Substring(0, 2)) % 8;
            if ( eastingIdentifier != 0 )
            {
                String eastingChar = eastingLetters.Substring(eastingIdentifier - 1, 1);
                Int32 northingIdentifier = Convert.ToInt32(northing.Substring(0, 1));
                northingIdentifier = (northingIdentifier % 2) * 10;
                northingIdentifier = northingIdentifier + Convert.ToInt32(northing.Substring(1, 1));

                String northingLetters = MgrsNorthingChars(ZoneNumber);
                String northingChar = northingLetters.Substring(northingIdentifier, 1);
                result =
                    ZoneNumber.ToString("00") +
                    ZoneBand() + ' ' +
                    eastingChar + northingChar + ' ' +
                    easting.Substring(2, actualDigits - 2) +
                    northing.Substring(2, actualDigits - 2);
            }
            return result;
        }
        public static UtmPoint ParseUtmString(String value)
        {
            String actualValue = value.ToUpperInvariant().Replace(" ", "");
            String zone = actualValue.Substring(0, 3);
            String numbers = actualValue.Remove(0, 3);
            Int32 digits = numbers.Length / 2;
            String eastingString = numbers.Substring(0, digits).PadRight(7, '0');
            String northingString = numbers.Substring(digits, digits).PadRight(7, '0');

            Int32 zoneNumber = Convert.ToInt32(zone.Substring(0, 2));
            Char zoneLetter = zone[2];

            Int32 northing = Convert.ToInt32(northingString);
            Int32 easting = Convert.ToInt32(eastingString);
            UtmPoint result = new UtmPoint(easting, northing, zoneNumber, MinNorthing(zoneLetter) >= 0);
            return result;
        }
        public static UtmPoint ParseMgrsString(String value)
        {
            String actualValue = value.ToUpperInvariant().Replace(" ", "");
            String zone = actualValue.Substring(0, 3);
            String eastingChar = actualValue.Substring(3, 1);
            String northingChar = actualValue.Substring(4, 1);
            String numbers = actualValue.Remove(0, 5);
            Int32 digits = numbers.Length / 2;
            String eastingString = numbers.Substring(0, digits).PadRight(5, '0');
            String northingString = numbers.Substring(digits, digits).PadRight(5, '0');

            Int32 zoneNumber = Convert.ToInt16(zone.Substring(0, 2));
            Char zoneLetter = zone[2];
            String eastingLetters = MgrsEastingChars(zoneNumber);
            Int32 eastingNumber = eastingLetters.IndexOf(eastingChar) + 1;
            eastingString = eastingNumber.ToString("00") + eastingString;
            String northingLetters = MgrsNorthingChars(zoneNumber);
            Int32 northingNumber = northingLetters.IndexOf(northingChar);
            Int32 minimumNorthing = MinNorthing(zoneLetter);

            Int32 temporaryNorthing = (minimumNorthing / 2000000) * 2000000 + northingNumber * 100000;
            if ( ZoneBand(temporaryNorthing, zoneLetter >= 'N') != zoneLetter )
            {
                temporaryNorthing = temporaryNorthing + 2000000;
            }

            Int32 northing = temporaryNorthing + Convert.ToInt32(northingString);
            Int32 easting = Convert.ToInt32(eastingString);
            UtmPoint result = new UtmPoint(easting, northing, zoneNumber, zoneLetter >= 'N');
            return result;
        }

        public static String MgrsNorthingChars(Int32 zoneNumber)
        {
            String northingLetters = String.Empty;
            switch ( zoneNumber % 2 )
            {
                case 0:
                    northingLetters = "FGHJKLMNPQRSTUVABCDE";
                    break;
                case 1:
                    northingLetters = "ABCDEFGHJKLMNPQRSTUV";
                    break;
            }
            return northingLetters;
        }
        private static String MgrsEastingChars(Int32 zoneNumber)
        {
            String eastingLetters = String.Empty;
            switch ( zoneNumber % 3 )
            {
                case 0:
                    eastingLetters = "STUVWXYZ";
                    break;
                case 1:
                    eastingLetters = "ABCDEFGH";
                    break;
                case 2:
                    eastingLetters = "JKLMNPQR";
                    break;
            }
            return eastingLetters;
        }
        private static Int32 MinNorthing(Char zoneChar)
        {
            switch ( zoneChar )
            {
                case 'C':
                    return 1100000;
                case 'D':
                    return 2000000;
                case 'E':
                    return 2800000;
                case 'F':
                    return 3700000;
                case 'G':
                    return 4600000;
                case 'H':
                    return 5500000;
                case 'J':
                    return 6400000;
                case 'K':
                    return 7300000;
                case 'L':
                    return 8200000;
                case 'M':
                    return 9100000;
                case 'N':
                    return 0;
                case 'P':
                    return 800000;
                case 'Q':
                    return 1700000;
                case 'R':
                    return 2600000;
                case 'S':
                    return 3500000;
                case 'T':
                    return 4400000;
                case 'U':
                    return 5300000;
                case 'V':
                    return 6200000;
                case 'W':
                    return 7000000;
                case 'X':
                    return 7900000;
            }
            throw new ArgumentOutOfRangeException(zoneChar.ToString() + " is invalid UTM zone letter");
        }
        private static Int32 UtmZoneToLatitude(Char zoneChar)
        {
            Int32 index = _ZoneCharacters.IndexOf(zoneChar);
            Int32 zoneBottom = -80 + 8 * index;
            return zoneBottom;
        }
        private static char UtmLetterDesignator(Double latitude)
        {
            Char letter;
            if ( latitude < 84.0 && latitude >= 72.0 )
            {
                // Special case: zone X is 12 degrees from north to south, not 8.
                letter = _ZoneCharacters[19];
            }
            else
            {
                letter = _ZoneCharacters[(int)((latitude + 80.0) / 8.0)];
            }

            return letter;
        }

        private void Assign(UtmPoint value)
        {
            Northing = value.Northing;
            Easting = value.Easting;
            ZoneNumber = value.ZoneNumber;
            IsNorthernHemisphere = value.IsNorthernHemisphere;
        }

        public override string ToString()
        {
            String result = ToUtmString(MaximumDigits);
            return result;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new UtmPoint(this);
        }

        #endregion
        #region IEquatable Members
        public bool Equals(UtmPoint value)
        {
            bool lResult = (value.Northing == this.Northing);
            lResult = lResult & (value.Easting == this.Easting);
            lResult = lResult & (value.ZoneNumber == this.ZoneNumber);
            lResult = lResult & (value.IsNorthernHemisphere == this.IsNorthernHemisphere);
            return lResult;
        }
        #endregion

    }
}
