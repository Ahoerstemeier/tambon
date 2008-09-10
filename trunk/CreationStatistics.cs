using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    abstract class CreationStatistics
    {
        #region properties
        public Int32 StartYear { get; set; }
        public Int32 EndYear { get; set; }
        private Int32 mNumberOfAnnouncements;
        public Int32 NumberOfAnnouncements { get { return mNumberOfAnnouncements; } }
        #endregion
        #region methods
        protected virtual void Clear()
        {
            mNumberOfAnnouncements = 0;
        }
        protected virtual Boolean AnnouncementDateFitting(RoyalGazette iEntry)
        {
            Boolean retval = ((iEntry.Publication.Year <= EndYear) && (iEntry.Publication.Year >= StartYear));
            return retval;
        }
        protected abstract Boolean ContentFitting(RoyalGazetteContent iContent);
        protected abstract void ProcessContent(RoyalGazetteContent iContent);
        public void Calculate()
        {
            Clear();

            foreach (RoyalGazette lEntry in Helper.GlobalGazetteList)
            {
                if (AnnouncementDateFitting(lEntry)) 
                {
                    Boolean lfound = false;
                    foreach (RoyalGazetteContent lContent in lEntry.Content)
                    {
                        if (ContentFitting(lContent))
                        {
                                lfound = true;
                                ProcessContent(lContent);
                        }
                    }
                    if (lfound)
                    {
                        mNumberOfAnnouncements++;
                    }
                }
            }
        }
        public abstract String Information();
        #endregion
    }
}
