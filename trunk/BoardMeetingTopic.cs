using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    class BoardMeetingTopic
    {
        public DateTime Effective { get; set; }
        public RoyalGazette Gazette { get; set; }
        public RoyalGazetteContent Topic { get; set; }

        public TimeSpan TimeTillPublish()
        {
            TimeSpan RetVal = new TimeSpan(0);
            if (Gazette != null)
            {
                RetVal = Gazette.Publication - Effective;
            }
            return RetVal;
        }
        public void FindGazette()
        {
            foreach (RoyalGazette lGazette in Helper.GlobalGazetteList)
            {
                foreach (RoyalGazetteContent lGazetteContent in lGazette.Content)
                {
                    Boolean lFitting = false;
                    if (Topic.GetType() == typeof(RoyalGazetteContentStatus))
                    {
                        lFitting = (lGazetteContent.GetType() == typeof(RoyalGazetteContentConstituency));
                    }
//                    if (this.GetType() == typeof(RoyalGazetteContentRename))
//                    {
//                        lFitting = (lGazetteContent.GetType() == typeof(RoyalGazetteContentRename));
//                    }
                    if (lFitting)
                    {
                        if (Topic.Geocode != 0)
                        {
                            if (lGazetteContent.IsAboutGeocode(Topic.Geocode, false))
                            {
                                Gazette = lGazette;
                            }
                        }
                        if (Topic.TambonGeocode != 0)
                        {
                            if (lGazetteContent.IsAboutGeocode(Topic.TambonGeocode, false))
                            {
                                Gazette = lGazette;
                            }
                        }
                    }
                }
            }
        }
    }
}
