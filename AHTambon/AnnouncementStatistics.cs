using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public abstract class AnnouncementStatistics
    {
        #region properties
        public Int32 StartYear { get; set; }
        public Int32 EndYear { get; set; }
        public Int32 NumberOfAnnouncements { get; protected set; }
        #endregion
        #region methods
        protected virtual void Clear()
        {
            NumberOfAnnouncements = 0;
        }
        protected virtual Boolean AnnouncementDateFitting(RoyalGazette iEntry)
        {
            Boolean retval = ((iEntry.Publication.Year <= EndYear) && (iEntry.Publication.Year >= StartYear));
            return retval;
        }
        protected abstract void ProcessAnnouncement(RoyalGazette iEntry);
        public void Calculate()
        {
            Clear();

            foreach ( RoyalGazette lEntry in TambonHelper.GlobalGazetteList )
            {
                if ( AnnouncementDateFitting(lEntry) )
                {
                    ProcessAnnouncement(lEntry);
                }
            }
        }
        public abstract String Information();
        #endregion

    }
}
