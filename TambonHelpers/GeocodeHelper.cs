using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.AHoerstemeier.Tambon
{
    public static class GeocodeHelper
    {
        /// <summary>
        /// Default province.
        /// </summary>
        /// <remarks>Should be moved to UI settings.</remarks>
        static readonly public Int32 PreferredProvinceGeocode = 84;  // Surat Thani

        /// <summary>
        /// Checks whether two geocodes are identical, or in case the sub entities are included
        /// if <paramref name="geocodeToCheck"/> is below <paramref name="geocodeToFind"/>.
        /// </summary>
        /// <param name="geocodeToFind">Code to find.</param>
        /// <param name="geocodeToCheck">Code to check.</param>
        /// <param name="includeSubEntities">Toggles whether codes under the main code are considered fitting as well.</param>
        /// <returns>True if both codes fit together, false otherwise.</returns>
        public static Boolean IsSameGeocode(UInt32 geocodeToFind, UInt32 geocodeToCheck, Boolean includeSubEntities)
        {
            while ( geocodeToFind % 100 == 0 )
            {
                geocodeToFind = geocodeToFind / 100;
            }
            while ( geocodeToCheck % 100 == 0 )
            {
                geocodeToCheck = geocodeToCheck / 100;
            }
            Boolean result = false;
            if ( includeSubEntities )
            {
                result = IsBaseGeocode(geocodeToFind, geocodeToCheck);
            }
            else
            {
                result = (geocodeToFind == geocodeToCheck);
            }
            return result;
        }

        /// <summary>
        /// Checks whether the <paramref name="geocodeToCheck"/> is a code under <paramref name="baseGeocode"/>.
        /// A base code of zero means no check is done and true is returned.
        /// </summary>
        /// <param name="baseGeocode">Base code.</param>
        /// <param name="geocodeToCheck">Code to be checked to be under the base code.</param>
        /// <returns><c>true</c>c> if code is under base code, or base code is zero; <c>false</c> otherwise.</returns>
        public static Boolean IsBaseGeocode(UInt32 baseGeocode, UInt32 geocodeToCheck)
        {
            Boolean result = false;
            if ( baseGeocode == 0 )
            {
                result = true;
            }
            else if ( geocodeToCheck == 0 )
            {
                result = false;
            }
            else
            {
                Int32 level = 1;
                while ( baseGeocode < 1000000 )
                {
                    baseGeocode = baseGeocode * 100;
                    level = level * 100;
                }
                while ( geocodeToCheck < 1000000 )
                {
                    geocodeToCheck = geocodeToCheck * 100;
                }
                Int64 difference = geocodeToCheck - baseGeocode;

                result = (!(difference < 0)) & (difference < level);
            }
            return result;
        }

        /// <summary>
        /// Gets the geocode of the province in which the entity with the given geocode is located.
        /// </summary>
        /// <param name="geocode">Entity geocode.</param>
        /// <returns>Province geocode.</returns>
        public static UInt32 ProvinceCode(UInt32 geocode)
        {
            UInt32 result = geocode;
            while ( result > 100 )
            {
                result = result / 100;
            }
            return result;
        }

        /// <summary>
        /// Gets an enumeration of the geocode of all the parents of a given geocode.
        /// </summary>
        /// <param name="geocode">Entity geocode.</param>
        /// <returns>Enumeration of parent geocodes.</returns>
        public static IEnumerable<UInt32> ParentGeocodes(UInt32 geocode)
        {
            var result = new List<UInt32>();
            var value = geocode / 100;
            while ( value != 0 )
            {
                result.Add(value);
                value = value / 100;
            }
            return result;
        }

        /// <summary>
        /// Calculates the geocode level.
        /// </summary>
        /// <param name="geocode">Code to check.</param>
        /// <returns>Level of geocode.
        /// <list type="bullet">
        /// <item><term>0</term>
        /// <description>Country.</description>
        /// </item>
        /// <item><term>1</term>
        /// <description>1st level subdivision, i.e. province (or Bangkok).</description>
        /// </item>
        /// <item><term>2</term>
        /// <description>2nd level subdivision, i.e. district (Amphoe, King Amphoe, Khet), includes municipalities.</description>
        /// </item>
        /// <item><term>3</term>
        /// <description>3rd level subdivision, i.e. subdistrict (Tambon, Khwaeng).</description>
        /// </item>
        /// <item><term>4</term>
        /// <description>4th level subdivision, i.e. administrative villages and communities (Muban, Chumchon).</description>
        /// </item>
        /// </list>         /// </returns>
        public static Byte GeocodeLevel(UInt32 geocode)
        {
            Byte result = 0;
            var value = geocode;
            while ( value > 0 )
            {
                result++;
                value = value / 100;
            }
            return result;
        }
    }
}