using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace De.AHoerstemeier.Geo
{
    // Based on http://code.google.com/p/geospatialweb/source/browse/trunk/geohash/src/Geohash.java

    static internal class GeoHash
    {
        private static Char[] _Digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8',
                        '9', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p',
                        'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private static int _NumberOfBits = 6 * 5;
        private static Dictionary<Char,Int32> _LookupTable = CreateLookup();

        private static Dictionary<Char,Int32> CreateLookup()
        {
            Dictionary<Char, Int32> result = new Dictionary<char, Int32>();
            Int32 i = 0;
            foreach (Char c in _Digits)
            {
                result[c]=i;
                i++;
            }
            return result;
        }

        private static double GeoHashDecode(BitArray bits, double floorValue, double ceilingValue)
        {
            Double middle = 0;
            Double floor = floorValue;
            Double ceiling = ceilingValue;
            for (Int32 i = 0; i < bits.Length; i++)
            {
                middle = (floor + ceiling) / 2;
                if (bits[i])
                {
                    floor = middle;
                }
                else
                {
                    ceiling = middle;
                }
            }
            return middle;
        }
        private static BitArray GeoHashEncode(double value, double floorValue, double ceilingValue)
        {
            BitArray result = new BitArray(_NumberOfBits);
            Double floor = floorValue;
            Double ceiling = ceilingValue;
            for (Int32 i = 0; i < _NumberOfBits; i++)
            {
                Double middle = (floor + ceiling) / 2;
                if (value >= middle)
                {
                    result[i] = true;
                    floor = middle;
                }
                else
                {
                    result[i] = false;
                    ceiling = middle;
                }
            }
            return result;
        }

        private static String EncodeBase32(String binaryStringValue)
        {
            StringBuilder buffer = new StringBuilder();
            String binaryString = binaryStringValue;
            while (binaryString.Length > 0)
            {
                String currentBlock = binaryString.Substring(0, 5).PadLeft(5,'0');
                if (binaryString.Length > 5)
                {
                    binaryString = binaryString.Substring(5, binaryString.Length - 5);
                }
                else 
                { 
                    binaryString = String.Empty; 
                }
                Int32 value = Convert.ToInt32(currentBlock, 2);
                buffer.Append(_Digits[value]);
            }

            String result = buffer.ToString();
            return result;
        }

        internal static GeoPoint DecodeGeoHash(String value)
        {
            StringBuilder lBuffer = new StringBuilder();
            foreach (Char c in value)
            {
                if (!_LookupTable.ContainsKey(c))
                {
                    throw new ArgumentException("Invalid character " + c);
                }
                Int32 i = _LookupTable[c] + 32;
                lBuffer.Append(Convert.ToString(i,2).Substring(1));
            }

            BitArray lonset = new BitArray(_NumberOfBits);
            BitArray latset = new BitArray(_NumberOfBits);

            //even bits
            int j = 0;
            for (int i = 0; i < _NumberOfBits * 2; i += 2)
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
            for (int i = 1; i < _NumberOfBits * 2; i += 2)
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

        internal static String EncodeGeoHash(GeoPoint data, Int32 accuracy)
        {
            BitArray latitudeBits = GeoHashEncode(data.Latitude, -90, 90);
            BitArray longitudeBits = GeoHashEncode(data.Longitude, -180, 180);
            StringBuilder buffer = new StringBuilder();
            for (Int32 i = 0; i < _NumberOfBits; i++)
            {
                buffer.Append((longitudeBits[i]) ? '1' : '0');
                buffer.Append((latitudeBits[i]) ? '1' : '0');
            }
            String binaryValue = buffer.ToString();
            String result = EncodeBase32(binaryValue);
            result = result.Substring(0, accuracy);
            return result;
        }
    }
}
