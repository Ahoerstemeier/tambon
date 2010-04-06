using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Geo
{
    public class UTMPoint : ICloneable, IEquatable<UTMPoint>
    {
        public const Int16 MaxDigits = 7;
        public const Int16 MinDigits = 2;
        #region properties
        public Int32 Northing { get; set; }
        public Int32 Easting { get; set; }
        public Int32 ZoneNumber { get; set; }
        // public char ZoneBand { get; set; }
        public Boolean IsNorthernHemisphere { get; set; }
        #endregion

        #region constructor
        public UTMPoint()
        {
        }
        public UTMPoint(string iValue)
        {
            Assign(ParseUTMString(iValue));
        }
        public UTMPoint(UTMPoint iValue)
        {
            Assign(iValue);
        }
        public UTMPoint(Int32 iEasting, Int32 iNorthing, Int32 iZoneNumber, Boolean iIsNorthernHemisphere)
        {
            Northing = iNorthing;
            Easting = iEasting;
            ZoneNumber = iZoneNumber;
            IsNorthernHemisphere = iIsNorthernHemisphere;
        }
        public UTMPoint(Double iEasting, Double iNorthing, Int32 iZoneNumber, Boolean iIsNorthernHemisphere)
        {
            Northing = Convert.ToInt32(Math.Round(iNorthing));
            Easting = Convert.ToInt32(Math.Round(iEasting));
            ZoneNumber = iZoneNumber;
            IsNorthernHemisphere = iIsNorthernHemisphere;
        }
        #endregion

        #region constants
        private const String cZoneCharacters = "CDEFGHJKLMNPQRSTUVWX";
        #endregion

        #region methods
        public char ZoneBand()
        {
            char lZoneChar = ZoneBand(this.Northing, this.IsNorthernHemisphere);
            return lZoneChar;
        }
        static char ZoneBand(Int32 iNorthing, bool iIsNorthernHemisphere)
        {
            UTMPoint lTemp = new UTMPoint(0, iNorthing, 1, iIsNorthernHemisphere);
            GeoPoint lGeoPoint = new GeoPoint(lTemp, GeoDatum.DatumWGS84());
            char lZoneChar = UTMLetterDesignator(lGeoPoint.Latitude);
            return lZoneChar;
        }
        public String ToUTMString(Int16 iDigits)
        {
            Int16 lDigits = MakeDigitValid(iDigits);
            String lNorthing = Northing.ToString("0000000");
            String lEasting = Easting.ToString("0000000");
            lEasting = lEasting.Substring(0, lDigits);
            lNorthing = lNorthing.Substring(0, lDigits);
            String lResult = ZoneNumber.ToString("00") + ZoneBand() + ' ' + lEasting + ' ' + lNorthing;
            return lResult;
        }
        static public Int16 MakeDigitValid(Int16 iDigits)
        {
            Int16 lDigits = Math.Min(MaxDigits, Math.Max(MinDigits, iDigits));
            return lDigits;
        }
        public String ToMGRSString(Int16 iDigits)
        {
            Int16 lDigits = MakeDigitValid(iDigits);
            String lNorthing = Northing.ToString("0000000");
            String lEasting = Easting.ToString("0000000");
            String lEastingLetters = MGRSEastingChars(ZoneNumber);
            Int32 lEastingIdentifier = Convert.ToInt32(lEasting.Substring(0, 2)) % 8;
            String lEastingChar = lEastingLetters.Substring(lEastingIdentifier-1, 1);
            Int32 lNorthingIdentifier = Convert.ToInt32(lNorthing.Substring(0, 1));
            lNorthingIdentifier = (lNorthingIdentifier % 2) * 10;
            lNorthingIdentifier = lNorthingIdentifier + Convert.ToInt32(lNorthing.Substring(1, 1));

            String lNorthingLetters = MGRSNorthingChars(ZoneNumber);
            String lNorthingChar = lNorthingLetters.Substring(lNorthingIdentifier, 1);
            String lResult =
                ZoneNumber.ToString("00") +
                ZoneBand() + ' ' +
                lEastingChar + lNorthingChar + ' ' +
                lEasting.Substring(2, lDigits - 2) +
                lNorthing.Substring(2,lDigits - 2);
            return lResult;
        }
        public static UTMPoint ParseUTMString(String iValue)
        {
            String lValue = iValue.ToUpperInvariant().Replace(" ", "");
            String lZone = lValue.Substring(0, 3);
            String lNumbers = lValue.Remove(0, 3);
            Int32 lDigits = lNumbers.Length / 2;
            String lEastingString = lNumbers.Substring(0, lDigits).PadRight(7,'0');
            String lNorthingString = lNumbers.Substring(lDigits, lDigits).PadRight(7, '0');

            Int32 lZoneNumber = Convert.ToInt32(lZone.Substring(0, 2));
            char lZoneLetter = lZone[2];

            Int32 lNorthing = Convert.ToInt32(lNorthingString);
            Int32 lEasting = Convert.ToInt32(lEastingString);
            UTMPoint lResult = new UTMPoint(lEasting, lNorthing, lZoneNumber, MinNorthing(lZoneLetter) >= 0);
            return lResult;
        }
        public static UTMPoint ParseMGRSString(String iValue)
        {
            String lValue = iValue.ToUpperInvariant().Replace(" ", "");
            String lZone = lValue.Substring(0, 3);
            String lEastingChar = lValue.Substring(3, 1);
            String lNorthingChar = lValue.Substring(4, 1);
            String lNumbers = lValue.Remove(0, 5);
            Int32 lDigits = lNumbers.Length / 2;
            String lEastingString = lNumbers.Substring(0, lDigits).PadRight(5,'0');
            String lNorthingString = lNumbers.Substring(lDigits, lDigits).PadRight(5, '0');

            Int32 lZoneNumber = Convert.ToInt16(lZone.Substring(0, 2));
            char lZoneLetter = lZone[2];
            String lEastingLetters = MGRSEastingChars(lZoneNumber);
            Int32 lEastingNumber = lEastingLetters.IndexOf(lEastingChar)+1;
            lEastingString = lEastingNumber.ToString("00") + lEastingString;
            String lNorthingLetters = MGRSNorthingChars(lZoneNumber);
            Int32 lNorthingNumber = lNorthingLetters.IndexOf(lNorthingChar);
            Int32 lMinNorthing = MinNorthing(lZoneLetter);

            Int32 lNorthingTemp = (lMinNorthing / 2000000) * 2000000 + lNorthingNumber * 100000;
            if (ZoneBand(lNorthingTemp, lZoneLetter >= 'N') != lZoneLetter)
            {
                lNorthingTemp = lNorthingTemp + 200000;
            }

            Int32 lNorthing = lNorthingTemp+Convert.ToInt32(lNorthingString);
            Int32 lEasting = Convert.ToInt32(lEastingString);
            UTMPoint lResult = new UTMPoint(lEasting, lNorthing, lZoneNumber, lZoneLetter > 'N');
            return lResult;
        }

        public static String MGRSNorthingChars(Int32 lZoneNumber)
        {
            String lNorthingLetters = String.Empty;
            switch (lZoneNumber % 2)
            {
                case 0:
                    lNorthingLetters = "FGHJKLMNPQRSTUVABCDE";
                    break;
                case 1:
                    lNorthingLetters = "ABCDEFGHJKLMNPQRSTUV";
                    break;
            }
            return lNorthingLetters;
        }
        private static String MGRSEastingChars(Int32 iZoneNumber)
        {
            String lEastingLetters = String.Empty;
            switch (iZoneNumber % 3)
            {
                case 0:
                    lEastingLetters = "STUVWXYZ";
                    break;
                case 1:
                    lEastingLetters = "ABCDEFGH";
                    break;
                case 2:
                    lEastingLetters = "JKLMNPQR";
                    break;
            }
            return lEastingLetters;
        }
        private static Int32 MinNorthing(char iZoneChar)
        {
            switch (iZoneChar)
            {
                case 'C': return 1100000;
                case 'D': return 2000000;
                case 'E': return 2800000;
                case 'F': return 3700000;
                case 'G': return 4600000;
                case 'H': return 5500000;
                case 'J': return 6400000;
                case 'K': return 7300000;
                case 'L': return 8200000;
                case 'M': return 9100000;
                case 'N': return 0;
                case 'P': return 800000;
                case 'Q': return 1700000;
                case 'R': return 2600000;
                case 'S': return 3500000;
                case 'T': return 4400000;
                case 'U': return 5300000;
                case 'V': return 6200000;
                case 'W': return 7000000;
                case 'X': return 7900000;
            }
            throw new ArgumentOutOfRangeException(iZoneChar.ToString()+" is invalid UTM zone letter");
        }
        private static Int32 UTMZoneToLatitude(char iZoneChar)
        {
            Int32 lIndex = cZoneCharacters.IndexOf(iZoneChar);
            Int32 lZoneBottom = -80 + 8 * lIndex;
            return lZoneBottom;
        }
        private static char UTMLetterDesignator(double iLatitude)
        {
            char lLetter;
            if (iLatitude < 84.0 && iLatitude >= 72.0)
            {
                // Special case: zone X is 12 degrees from north to south, not 8.
                lLetter = cZoneCharacters[19];
            }
            else
            {
                lLetter = cZoneCharacters[(int)((iLatitude + 80.0) / 8.0)];
            }

            return lLetter;
        }

        private void Assign(UTMPoint iValue)
        {
            Northing = iValue.Northing;
            Easting = iValue.Easting;
            ZoneNumber = iValue.ZoneNumber;
            IsNorthernHemisphere = iValue.IsNorthernHemisphere;
        }

        public override string ToString()
        {
            String lResult = ToUTMString(MaxDigits);
            return lResult;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new UTMPoint(this);
        }

        #endregion
        #region IEquatable Members
        public bool Equals(UTMPoint iObj)
        {
            bool lResult = (iObj.Northing == this.Northing);
            lResult = lResult & (iObj.Easting == this.Easting);
            lResult = lResult & (iObj.ZoneNumber == this.ZoneNumber);
            lResult = lResult & (iObj.IsNorthernHemisphere == this.IsNorthernHemisphere);
            return lResult;
        }
        #endregion

    }
}
