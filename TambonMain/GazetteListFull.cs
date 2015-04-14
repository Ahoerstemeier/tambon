using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.AHoerstemeier.Tambon
{
    public partial class GazetteListFull : IGazetteEntries
    {
        /// <summary>
        /// Gets a flat list of all gazette entries.
        /// </summary>
        /// <value>Flat list of all gazette entries.</value>
        public IEnumerable<GazetteEntry> AllGazetteEntries
        {
            get
            {
                var result = new List<GazetteEntry>();
                result.AddRange(this.entry);
                foreach ( var decadeList in this.decade )
                {
                    result.AddRange(decadeList.AllGazetteEntries);
                }
                foreach ( var yearList in this.year )
                {
                    result.AddRange(yearList.AllGazetteEntries);
                }
                return result;
            }
        }

        /// <summary>
        /// Gets all gazette announcements related to the given geocode.
        /// </summary>
        /// <param name="geocode">Code to look for.</param>
        /// <param name="includeSubEnties"><c>true</c> to include announcements on sub-entities, <c>false</c> to only return exact matches.</param>
        /// <returns>All announcements related to the geocode.</returns>
        public IEnumerable<GazetteEntry> AllAboutGeocode(UInt32 geocode, Boolean includeSubEnties)
        {
            var result = new List<GazetteEntry>();
            result.AddRange(this.entry.Where(x => x.IsAboutGeocode(geocode, includeSubEnties)));
            foreach ( var decadeList in this.decade )
            {
                result.AddRange(decadeList.AllAboutGeocode(geocode, includeSubEnties));
            }
            foreach ( var yearList in this.year )
            {
                result.AddRange(yearList.AllAboutGeocode(geocode, includeSubEnties));
            }
            return result;
        }

        /// <summary>
        /// Searches for the first fitting announcement matching the given reference.
        /// </summary>
        /// <param name="gazetteReference">Gazette reference.</param>
        /// <returns>Gazette announcement, or <c>null</c> if nothing found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="gazetteReference"/> is <c>null</c>.</exception>
        public GazetteEntry FindAnnouncement(GazetteRelated gazetteReference)
        {
            if ( gazetteReference == null )
            {
                throw new ArgumentNullException("gazetteReference");
            }
            return AllGazetteEntries.FirstOrDefault(x => x.IsMatchWith(gazetteReference));
        }
    }
}