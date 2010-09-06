using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace De.AHoerstemeier.Geo
{
    // Based on http://code.google.com/p/geospatialweb/source/browse/trunk/geohash/src/Geohash.java

    static internal class GeoHash
    {
        static char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8',
                        '9', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p',
                        'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        private static double GeoHashDecode(BitArray iBits, double iFloor, double iCeiling)
        {
            double iMiddle = 0;
            for (int i = 0; i < iBits.Length; i++)
            {
                iMiddle = (iFloor + iCeiling) / 2;
                if (iBits[i])
                {
                    iFloor = iMiddle;
                }
                else
                {
                    iCeiling = iMiddle;
                }
            }
            return iMiddle;
        }
        private static BitArray GeoHashEncode(double iValue, double iFloor, double iCeiling, Int32 iAccuracy)
        {
            Int32 lNumberOfBits = iAccuracy * 5;
            BitArray lResult = new BitArray(lNumberOfBits);
            for (int i = 0; i < lNumberOfBits; i++)
            {
                double lMiddle = (iFloor + iCeiling) / 2;
                if (iValue >= lMiddle)
                {
                    lResult[i] = true;
                    iFloor = lMiddle;
                }
                else
                {
                    lResult[i] = false;
                    iCeiling = lMiddle;
                }
            }
            return lResult;
        }

        private static String EncodeBase32(Int64 iValue)
        {
            char[] lBuf = new char[65];
            int lCharPos = 64;
            Boolean lNegative = (iValue < 0);
            if (!lNegative)
                iValue = -iValue;
            while (iValue <= -32)
            {
                lBuf[lCharPos--] = digits[(int)(-(iValue % 32))];
                iValue /= 32;
            }
            lBuf[lCharPos] = digits[(int)(-iValue)];

            if (lNegative)
                lBuf[--lCharPos] = '-';
            String lResult = new String(lBuf, lCharPos, (65 - lCharPos));
            return lResult;
        }

        internal static GeoPoint DecodeGeoHash(String iValue)
        {
            Int32 numbits = 6 * 5;
            StringBuilder lBuffer = new StringBuilder();
            //for (char c : geohash.toCharArray()) {
            //
            //                      int i = lookup.get(c) + 32;
            //                    lBuffer.Append( Int32.ToString(i, 2).substring(1) );
            //          }

            //        BitArray lonset = new BitArray();
            //      BitArray latset = new BitArray();

            //even bits
            int j = 0;
            for (int i = 0; i < numbits * 2; i += 2)
            {
                Boolean isSet = false;
                if (i < lBuffer.Length)
                    isSet = lBuffer[i] == '1';
                //            lonset.set(j++, isSet);
            }

            //odd bits
            j = 0;
            for (int i = 1; i < numbits * 2; i += 2)
            {
                Boolean isSet = false;
                if (i < lBuffer.Length)
                    isSet = lBuffer[i] == '1';
                // latset.set(j++, isSet);
            }

            //double lLongitude = GeoHashDecode(lonset, -180, 180);
            //double lLatitude = GeoHashDecode(latset, -90, 90);

            //GeoPoint lResult = new GeoPoint(lLatitude,lLongitude);

            GeoPoint lResult = null;
            return lResult;
        }

        internal static String EncodeGeoHash(GeoPoint iData, Int32 iAccuracy)
        {
            Int32 lNumBits = iAccuracy * 5;

            BitArray lLatitudeBits = GeoHashEncode(iData.Latitude, -90, 90, lNumBits);
            BitArray lLongitudeBits = GeoHashEncode(iData.Longitude, -180, 180, lNumBits);
            StringBuilder lBuffer = new StringBuilder();
            for (int i = 0; i < lNumBits; i++)
            {
                lBuffer.Append((lLatitudeBits[i]) ? '1' : '0');
                lBuffer.Append((lLongitudeBits[i]) ? '1' : '0');
            }
            String lResult = EncodeBase32(Convert.ToInt64(lBuffer.ToString(), 2));
            return lResult;
        }
    }
}
