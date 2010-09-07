using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace De.AHoerstemeier.Geo
{
    // Based on http://code.google.com/p/geospatialweb/source/browse/trunk/geohash/src/Geohash.java

    static internal class GeoHash
    {
        private static char[] mDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8',
                        '9', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p',
                        'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private static int mNumberOfBits = 6 * 5;
        private static Dictionary<char,Int32> mLookupTable = CreateLookup();

        private static Dictionary<char,Int32> CreateLookup()
        {
            Dictionary<char, Int32> lResult = new Dictionary<char, Int32>();
            int i = 0;
            foreach (char c in mDigits)
            {
                lResult[c]=i;
                i++;
            }
            return lResult;
        }

        private static double GeoHashDecode(BitArray iBits, double iFloor, double iCeiling)
        {
            double lMiddle = 0;
            double lFloor = iFloor;
            double lCeiling = iCeiling;
            for (int i = 0; i < iBits.Length; i++)
            {
                lMiddle = (lFloor + lCeiling) / 2;
                if (iBits[i])
                {
                    lFloor = lMiddle;
                }
                else
                {
                    lCeiling = lMiddle;
                }
            }
            return lMiddle;
        }
        private static BitArray GeoHashEncode(double iValue, double iFloor, double iCeiling)
        {
            BitArray lResult = new BitArray(mNumberOfBits);
            double lFloor = iFloor;
            double lCeiling = iCeiling;
            for (int i = 0; i < mNumberOfBits; i++)
            {
                double lMiddle = (lFloor + lCeiling) / 2;
                if (iValue >= lMiddle)
                {
                    lResult[i] = true;
                    lFloor = lMiddle;
                }
                else
                {
                    lResult[i] = false;
                    lCeiling = lMiddle;
                }
            }
            return lResult;
        }

        private static String EncodeBase32(String iBinaryString)
        {
            StringBuilder lBuffer = new StringBuilder();
            String lBinaryString = iBinaryString;
            while (lBinaryString.Length > 0)
            {
                String lCurrentBlock = lBinaryString.Substring(0, 5).PadLeft(5,'0');
                if (lBinaryString.Length > 5)
                {
                    lBinaryString = lBinaryString.Substring(5, lBinaryString.Length - 5);
                }
                else 
                { 
                    lBinaryString = String.Empty; 
                }
                Int32 lValue = Convert.ToInt32(lCurrentBlock, 2);
                lBuffer.Append(mDigits[lValue]);
            }

            String lResult = lBuffer.ToString();
            return lResult;
        }

        internal static GeoPoint DecodeGeoHash(String iValue)
        {
            StringBuilder lBuffer = new StringBuilder();
            foreach (char c in iValue)
            {
                if (!mLookupTable.ContainsKey(c))
                {
                    throw new ArgumentException("Invalid character " + c);
                }
                int i = mLookupTable[c] + 32;
                lBuffer.Append(Convert.ToString(i,2).Substring(1));
            }

            BitArray lonset = new BitArray(mNumberOfBits);
            BitArray latset = new BitArray(mNumberOfBits);

            //even bits
            int j = 0;
            for (int i = 0; i < mNumberOfBits * 2; i += 2)
            {
                Boolean isSet = false;
                if (i < lBuffer.Length)
                {
                    isSet = lBuffer[i] == '1';
                }
                lonset[j] = isSet;
                j++;
            }

            //odd bits
            j = 0;
            for (int i = 1; i < mNumberOfBits * 2; i += 2)
            {
                Boolean isSet = false;
                if (i < lBuffer.Length)
                {
                    isSet = lBuffer[i] == '1';
                }
                latset[j] = isSet;
                j++;
            }

            double lLongitude = GeoHashDecode(lonset, -180, 180);
            double lLatitude = GeoHashDecode(latset, -90, 90);

            GeoPoint lResult = new GeoPoint(lLatitude,lLongitude);

            return lResult;
        }

        internal static String EncodeGeoHash(GeoPoint iData, Int32 iAccuracy)
        {
            BitArray lLatitudeBits = GeoHashEncode(iData.Latitude, -90, 90);
            BitArray lLongitudeBits = GeoHashEncode(iData.Longitude, -180, 180);
            StringBuilder lBuffer = new StringBuilder();
            for (int i = 0; i < mNumberOfBits; i++)
            {
                lBuffer.Append((lLongitudeBits[i]) ? '1' : '0');
                lBuffer.Append((lLatitudeBits[i]) ? '1' : '0');
            }
            String lBinaryValue = lBuffer.ToString();
            String lResult = EncodeBase32(lBinaryValue);
            lResult = lResult.Substring(0, iAccuracy);
            return lResult;
        }
    }
}
