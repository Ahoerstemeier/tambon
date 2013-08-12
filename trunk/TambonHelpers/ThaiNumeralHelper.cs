using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.AHoerstemeier.Tambon
{
    public static class ThaiNumeralHelper
    {
        /// <summary>
        /// Translation table between Thai numeral and arabic numeral.
        /// </summary>
        internal static Dictionary<Char, Byte> ThaiNumerals = new Dictionary<char, byte>
        {
            {'๐',0},
            {'๑',1},
            {'๒',2},
            {'๓',3},
            {'๔',4},
            {'๕',5},
            {'๖',6},
            {'๗',7},
            {'๘',8},
            {'๙',9}
        };

        /// <summary>
        /// Replaces Thai numerals with their corresponding Arabian numeral.
        /// </summary>
        /// <param name="value">String to check.</param>
        /// <returns>String with numerals exchanged.</returns>
        public static String ReplaceThaiNumerals(String value)
        {
            string RetVal = String.Empty;

            if ( !String.IsNullOrEmpty(value) )
            {
                foreach ( char c in value )
                {
                    if ( ThaiNumerals.ContainsKey(c) )
                    {
                        RetVal = RetVal + ThaiNumerals[c].ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        RetVal = RetVal + c;
                    }
                }
            }
            return RetVal;
        }

        /// <summary>
        /// Replaces any Arabian numerals with the corresponding Thai numerals.
        /// </summary>
        /// <param name="value">String to be checked.</param>
        /// <returns>String with Thai numerals.</returns>
        internal static string UseThaiNumerals(string value)
        {
            string RetVal = String.Empty;

            if ( !String.IsNullOrEmpty(value) )
            {
                foreach ( Char c in value )
                {
                    if ( (c >= '0') | (c <= '9') )
                    {
                        Int32 numericValue = Convert.ToInt32(c) - Convert.ToInt32('0');
                        foreach ( KeyValuePair<Char, Byte> keyValuePair in ThaiNumerals )
                        {
                            if ( keyValuePair.Value == numericValue )
                            {
                                RetVal = RetVal + keyValuePair.Key;
                            }
                        }
                    }
                    else
                    {
                        RetVal = RetVal + c;
                    }
                }
            }
            return RetVal;
        }
    }
}