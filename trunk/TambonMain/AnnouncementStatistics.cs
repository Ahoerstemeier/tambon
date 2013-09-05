using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public abstract class AnnouncementStatistics
    {
        #region properties

        public Int32 StartYear
        {
            get;
            set;
        }

        public Int32 EndYear
        {
            get;
            set;
        }

        public Int32 NumberOfAnnouncements
        {
            get;
            protected set;
        }

        #endregion properties

        #region methods

        protected virtual void Clear()
        {
            NumberOfAnnouncements = 0;
        }

        protected virtual Boolean AnnouncementDateFitting(GazetteEntry entry)
        {
            Boolean retval = ((entry.publication.Year <= EndYear) && (entry.publication.Year >= StartYear));
            return retval;
        }

        protected abstract void ProcessAnnouncement(GazetteEntry entry);

        public void Calculate(GazetteList gazetteList)
        {
            if ( gazetteList == null )
            {
                throw new ArgumentNullException("gazetteList");
            }

            Calculate(gazetteList.AllGazetteEntries);
        }

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

        public abstract String Information();

        #endregion methods
    }
}