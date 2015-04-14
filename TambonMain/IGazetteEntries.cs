using System;
using System.Collections.Generic;

namespace De.AHoerstemeier.Tambon
{
    internal interface IGazetteEntries
    {
        /// <summary>
        /// Gets all gazette announcements related to the given geocode.
        /// </summary>
        /// <param name="geocode">Code to look for.</param>
        /// <param name="includeSubEnties"><c>true</c> to include announcements on sub-entities, <c>false</c> to only return exact matches.</param>
        /// <returns>All announcements related to the geocode.</returns>
        IEnumerable<GazetteEntry> AllAboutGeocode(UInt32 geocode, Boolean includeSubEnties);

        /// <summary>
        /// Gets a flat list of all gazette entries.
        /// </summary>
        /// <value>Flat list of all gazette entries.</value>
        IEnumerable<GazetteEntry> AllGazetteEntries
        {
            get;
        }

        /// <summary>
        /// Searches for the first fitting announcement matching the given reference.
        /// </summary>
        /// <param name="gazetteReference">Gazette reference.</param>
        /// <returns>Gazette announcement, or <c>null</c> if nothing found.</returns>
        GazetteEntry FindAnnouncement(GazetteRelated gazetteReference);
    }
}