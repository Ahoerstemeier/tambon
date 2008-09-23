using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    abstract class CreationStatistics : AnnouncementStatistics
    {
        #region properties
        private FrequencyCounter mCreationsPerAnnouncement = new FrequencyCounter();
        protected FrequencyCounter CreationsPerAnnouncement { get { return mCreationsPerAnnouncement; } }
        #endregion
        #region methods
        protected override void Clear()
        {
            base.Clear();
            mCreationsPerAnnouncement = new FrequencyCounter();
        }
        protected abstract Boolean ContentFitting(RoyalGazetteContent iContent);
        protected abstract void ProcessContent(RoyalGazetteContent iContent);
        protected override void ProcessAnnouncement(RoyalGazette iEntry)
        {
            Int32 lCount = 0;
            Int32 lProvinceGeocode = 0;
            foreach (RoyalGazetteContent lContent in iEntry.Content)
            {
                if (ContentFitting(lContent))
                {
                    lCount++;
                    ProcessContent(lContent);
                    lProvinceGeocode = lContent.Geocode;
                    while (lProvinceGeocode / 100 != 0)
                    {
                        lProvinceGeocode = lProvinceGeocode / 100;
                    }

                }
            }
            if (lCount > 0)
            {
                mNumberOfAnnouncements++;
                mCreationsPerAnnouncement.IncrementForCount(lCount, lProvinceGeocode);
            }
        }
        #endregion
    }
}
