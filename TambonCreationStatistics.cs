using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    class TambonCreationStatistics
    {
        #region properties
        public Int32 StartYear { get; set; }
        public Int32 EndYear { get; set; }
        private Int32 mNumberOfAnnouncements;
        public Int32 NumberOfAnnouncements { get { return mNumberOfAnnouncements; } }
        private Int32 mNumberOfTambonCreations;
        public Int32 NumberOfTambonCreations { get { return mNumberOfTambonCreations; } }
        private Dictionary<Int32, List<Int32>> mNumberOfMuban = new Dictionary<int,List<int>>();
        public Dictionary<Int32, List<Int32>> NumberOfMuban { get { return mNumberOfMuban; } }
        private Int32[] mNumberOfTambonCreationsPerChangwat = new Int32[100];
        #endregion
        #region constructor
        public TambonCreationStatistics()
        {
            StartYear = 1883;
            EndYear = DateTime.Now.Year;
        }
        public TambonCreationStatistics(Int32 iStartYear, Int32 iEndYear)
        {
            StartYear = iStartYear;
            EndYear = iEndYear;
        }
        #endregion
        #region methods
        public void Calculate()
        { 
            mNumberOfMuban = new Dictionary<int,List<int>>();
            mNumberOfTambonCreationsPerChangwat = new Int32[100];
            mNumberOfTambonCreations = 0;
            mNumberOfAnnouncements = 0;

            foreach (RoyalGazette lEntry in Helper.GlobalGazetteList)
            {
                if ((lEntry.Publication.Year <= EndYear) && (lEntry.Publication.Year >= StartYear))
                {
                    Boolean lfound = false;
                    foreach (RoyalGazetteContent lContent in lEntry.Content)
                    {
                        if (lContent is RoyalGazetteContentCreate)
                        {
                            RoyalGazetteContentCreate lCreate = (RoyalGazetteContentCreate)lContent;
                            if (lCreate.Status == EntityType.Tambon)
                            {
                                lfound = true;                                
                                ProcessCreation(lCreate);
                            }
                        }
                    }
                    if (lfound)
                    {
                        mNumberOfAnnouncements++;
                    }
                }
            }
        }

        private void ProcessCreation(RoyalGazetteContentCreate lCreate)
        {
            mNumberOfTambonCreations++;

            Int32 lGeocode = lCreate.Geocode;
            while (lGeocode > 100)
            {
                lGeocode = lGeocode / 100;
            }
            mNumberOfTambonCreationsPerChangwat[lGeocode]++;

            // ToDo: Count Muban and fill list
        }
        #endregion

        public String Information()
        {
            StringBuilder lBuilder = new StringBuilder();
            lBuilder.AppendLine(NumberOfAnnouncements.ToString() + " Announcements");
            lBuilder.AppendLine(NumberOfTambonCreations.ToString() + " Tambon created");

            if (mNumberOfTambonCreations > 0)
            {
                List<String> lChangwat = new List<String>();
                Int32 lMaxNumber = 1;
                foreach (PopulationDataEntry lEntry in Helper.Geocodes)
                {
                    if (mNumberOfTambonCreationsPerChangwat[lEntry.Geocode] > lMaxNumber)
                    {
                        lMaxNumber = mNumberOfTambonCreationsPerChangwat[lEntry.Geocode];
                        lChangwat.Clear();
                    }
                    if (mNumberOfTambonCreationsPerChangwat[lEntry.Geocode] == lMaxNumber)
                    {
                        lChangwat.Add(lEntry.English);
                    }
                }
                lBuilder.AppendLine(lMaxNumber.ToString() + " Tambon created in");
                foreach (String lName in lChangwat)
                {
                    lBuilder.AppendLine("* " + lName);
                }
                lBuilder.AppendLine();
            
            // ToDo: The village data
            }

            String retval = lBuilder.ToString();
            return retval;
        }
    }
}
