using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public abstract class AnnouncementStatistics
    {
        #region properties

        /// <summary>
        /// Gets or sets the initial year to be checked.
        /// </summary>
        /// <value>The initial year.</value>
        public Int32 StartYear
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the final year to be checked.
        /// </summary>
        /// <value>The final year.</value>
        public Int32 EndYear
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of announcements.
        /// </summary>
        /// <value>The number of announcements.</value>
        public Int32 NumberOfAnnouncements
        {
            get;
            protected set;
        }

        #endregion properties

        #region methods

        /// <summary>
        /// Clears the <see cref="NumberOfAnnouncements"/>.
        /// </summary>
        protected virtual void Clear()
        {
            NumberOfAnnouncements = 0;
        }

        /// <summary>
        /// Checks whether the <see cref="GazetteEntry.publication">publication date</see> of <paramref name="entry"/> fits into the time range given by <see cref="StartYear"/> and <see cref="EndYear"/>.
        /// </summary>
        /// <param name="entry">Announcement to check.</param>
        /// <returns><c>true</c> if announcement publication date is in the time range, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entry"/> is <c>null</c>.</exception>
        protected virtual Boolean AnnouncementDateFitting(GazetteEntry entry)
        {
            if ( entry == null )
            {
                throw new ArgumentNullException("entry");
            }
            Boolean retval = ((entry.publication.Year <= EndYear) && (entry.publication.Year >= StartYear));
            return retval;
        }

        /// <summary>
        /// Calculates the actual detail statistics of the given <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry">Announcement to be checked.</param>
        protected abstract void ProcessAnnouncement(GazetteEntry entry);

        /// <summary>
        /// Calculates the statistics for all the given announcements.
        /// </summary>
        /// <param name="gazetteList">List of announcements.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gazetteList"/> is <c>null</c>.</exception>
        public void Calculate(GazetteList gazetteList)
        {
            if ( gazetteList == null )
            {
                throw new ArgumentNullException("gazetteList");
            }

            Calculate(gazetteList.AllGazetteEntries);
        }

        /// <summary>
        /// Calculates the statistics for all the given announcements.
        /// </summary>
        /// <param name="gazetteList">List of announcements.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gazetteList"/> is <c>null</c>.</exception>
        public void Calculate(IEnumerable<GazetteEntry> gazetteList)
        {
            if ( gazetteList == null )
            {
                throw new ArgumentNullException("gazetteList");
            }

            Clear();

            foreach ( var entry in gazetteList )
            {
                if ( AnnouncementDateFitting(entry) )
                {
                    ProcessAnnouncement(entry);
                }
            }
        }

        /// <summary>
        /// Gets a textual representation of the results.
        /// </summary>
        /// <returns>Textual representation of the results.</returns>
        public abstract String Information();

        #endregion methods
    }
}