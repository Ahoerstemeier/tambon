using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.AHoerstemeier.Tambon
{
    public static class TambonExtensions
    {
        /// <summary>
        /// Checks whether a string is a number.
        /// </summary>
        /// <param name="value">String to check.</param>
        /// <returns><c>true</c> if string is a number, <c>false</c> otherwise.</returns>
        public static Boolean IsNumeric(this String value)
        {
            if ( value == null )
            {
                throw new ArgumentNullException("value");
            }

            for ( int i = 0 ; i < value.Length ; i++ )
            {
                if ( !(Convert.ToInt32(value[i]) >= 48 && Convert.ToInt32(value[i]) <= 57) )
                {
                    return false;
                }
            }
            return !String.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Converts a string into a camel-cased string.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <returns>Camel-cased string.</returns>
        public static String ToCamelCase(this String value)
        {
            if ( value == null )
            {
                throw new ArgumentNullException("value");
            }

            var parts = value.Split(' ');
            var result = String.Empty;
            foreach ( var part in parts.Where(x => !String.IsNullOrWhiteSpace(x)) )
            {
                result += part.Substring(0, 1).ToUpperInvariant() + part.Substring(1);
            }
            return result;
        }

        /// <summary>
        /// Checks whether two Thai names can be considered identical as name of a Muban.
        /// </summary>
        /// <param name="value">Name of first Muban.</param>
        /// <param name="other">Name of second Muban.</param>
        /// <returns>True if identical, false otherwise.</returns>
        public static Boolean IsSameMubanName(this String value, String other)
        {
            Boolean result = (StripBanOrChumchon(value) == StripBanOrChumchon(other));
            return result;
        }

        /// <summary>
        /// Removes the word Ban (บ้าน) preceding the name.
        /// </summary>
        /// <param name="value">Name of a Muban.</param>
        /// <returns>Name without Ban.</returns>
        public static String StripBanOrChumchon(this String value)
        {
            if ( value == null )
            {
                throw new ArgumentNullException("value");
            }

            const String thaiStringBan = ThaiLanguageHelper.Ban;
            const String thaiStringChumchon = "ชุมชน";
            String retval = String.Empty;
            if ( value.StartsWith(thaiStringBan, StringComparison.Ordinal) )
            {
                retval = value.Remove(0, thaiStringBan.Length).Trim();
            }
            else if ( value.StartsWith(thaiStringChumchon, StringComparison.Ordinal) )
            {
                retval = value.Remove(0, thaiStringChumchon.Length).Trim();
            }
            else
            {
                retval = value;
            }

            const String englishStringBan = "Ban ";
            const String englishStringChumchon = "Chumchon ";
            if ( value.StartsWith(englishStringBan, StringComparison.Ordinal) )
            {
                retval = value.Remove(0, englishStringBan.Length).Trim();
            }
            else if ( value.StartsWith(englishStringChumchon, StringComparison.Ordinal) )
            {
                retval = value.Remove(0, englishStringChumchon.Length).Trim();
            }
            return retval;
        }
    }
}