using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Interface to handle TIS1099 geocodes.
    /// </summary>
    internal interface IGeocode
    {
        /// <summary>
        /// Checks if this instance is about the entity identified by the <paramref name="geocode"/>.
        /// If <paramref name="includeSubEntities"/> is <c>true</c>,
        /// </summary>
        /// <param name="geocode">Geocode to check.</param>
        /// <param name="includeSubEntities">Toggles whether codes under <paramref name="geocode"/> are considered fitting as well.</param>
        /// <returns><c>true</c> if instance is about the code, <c>false</c> otherwise.</returns>
        Boolean IsAboutGeocode(UInt32 geocode, Boolean includeSubEntities);
    }
}