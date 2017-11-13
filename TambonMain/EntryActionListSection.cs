using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class EntryActionListSection : IGeocode
    {
        #region IGeocode Members

        /// <summary>
        /// Checks if this instance is about the entity identified by the <paramref name="geocode"/>.
        /// If <paramref name="includeSubEntities"/> is <c>true</c>,
        /// </summary>
        /// <param name="geocode">Geocode to check.</param>
        /// <param name="includeSubEntities">Toggles whether codes under <paramref name="geocode"/> are considered fitting as well.</param>
        /// <returns><c>true</c> if instance is about the code, <c>false</c> otherwise.</returns>
        public Boolean IsAboutGeocode(UInt32 geocode, Boolean includeSubEntities)
        {
            var result = false;
            foreach ( var entry in subsection )
            {
                var toTest = entry as IGeocode;
                if ( toTest != null )
                {
                    result = result | toTest.IsAboutGeocode(geocode, includeSubEntities);
                }
            }
            foreach ( var entry in Items )
            {
                var toTest = entry as IGeocode;
                if ( toTest != null )
                {
                    result = result | toTest.IsAboutGeocode(geocode, includeSubEntities);
                }
            }
            return result;
        }

        #endregion IGeocode Members
    }
}