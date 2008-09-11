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
        private FrequencyCounter mCreationsPerAnnouncement = new FrequencyCounter();
        protected FrequencyCounter CreationsPerAnnouncement { get { return mCreationsPerAnnouncement; } }
        #endregion
        #region methods
        protected virtual void Clear()
        {
            mNumberOfAnnouncements = 0;
            mCreationsPerAnnouncement = new FrequencyCounter();
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
                    Int32 lCount = 0;
                    Int32 lProvinceGeocode = 0;
                    foreach (RoyalGazetteContent lContent in lEntry.Content)
                    {
                        if (ContentFitting(lContent))
                        {
                                lCount++;
                                ProcessContent(lContent);
                            lProvinceGeocode=lContent.Geocode;
                            while (lProvinceGeocode / 100 != 0)
                            {
                                lProvinceGeocode = lProvinceGeocode / 100;
                            }

                        }
                    }
                    if (lCount>0)
                    {
                        mNumberOfAnnouncements++;
                        mCreationsPerAnnouncement.IncrementForCount(lCount, lProvinceGeocode);
                    }
                }
            }
        }
        public abstract String Information();
        #endregion
    }
}
