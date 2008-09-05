using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    class TambonCreationStatistics
    {
        const Int32 MAXIMUMMUBAN = 50;
        #region properties
        public Int32 StartYear { get; set; }
        public Int32 EndYear { get; set; }
        private Int32 mNumberOfAnnouncements;
        public Int32 NumberOfAnnouncements { get { return mNumberOfAnnouncements; } }
        private Int32 mNumberOfTambonCreations;
        public Int32 NumberOfTambonCreations { get { return mNumberOfTambonCreations; } }
        private List<Int32>[] mNumberOfMuban = new List<Int32>[MAXIMUMMUBAN];
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
            mNumberOfMuban = new List<Int32>[MAXIMUMMUBAN];
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

        private void ProcessCreation(RoyalGazetteContentCreate iCreate)
        {
            mNumberOfTambonCreations++;

            Int32 lGeocode = iCreate.Geocode;
            while (lGeocode > 100)
            {
                lGeocode = lGeocode / 100;
            }
            mNumberOfTambonCreationsPerChangwat[lGeocode]++;

            Int32 lMaxMubanIndex = 0;
            foreach (RoyalGazetteContent lSubEntry in iCreate.SubEntities)
            {
                if (lSubEntry is RoyalGazetteContentReassign)
                {
                    Int32 lMubanCode = lSubEntry.Geocode % 100;
                    lMaxMubanIndex = Math.Max(lMaxMubanIndex, lMubanCode);
                }
            }

            if (mNumberOfMuban[lMaxMubanIndex] == null)
            {
                mNumberOfMuban[lMaxMubanIndex] = new List<Int32>();
            }
            mNumberOfMuban[lMaxMubanIndex].Add(iCreate.Geocode);
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

                Int32 lMostCommonNumber = 0;
                Int32 lMostCommonNumberValue = 0;
                Int32 lHighestNumber = 0;
                Int32 lLowestNumber = MAXIMUMMUBAN;
                Int32 lCount = 0;
                Int32 lSum = 0;

                for (int i = 1; i < MAXIMUMMUBAN; i++)
                {
                    if (mNumberOfMuban[i] != null)
                    {
                        Int32 lNumber = mNumberOfMuban[i].Count;
                        if (lNumber > 0)
                        {
                            lCount = lCount + lNumber;
                            lSum = lSum + lNumber * i;
                            lHighestNumber = i;
                            if (i < lLowestNumber)
                            {
                                lLowestNumber = i;
                            }
                            if (lNumber > lMostCommonNumberValue)
                            {
                                lMostCommonNumber = i;
                                lMostCommonNumberValue = lNumber;
                            }
                        }
                    }
                }
                if (lCount > 0)
                {
                    double lDeviation = 0;
                    double lMeanValue = (lSum * 1.0 / lCount);
                    for (int i = 1; i < MAXIMUMMUBAN; i++)
                    {
                        if (mNumberOfMuban[i] != null)
                        {
                            Int32 lNumber = mNumberOfMuban[i].Count;
                            if (lNumber > 0)
                            {
                                lDeviation = lDeviation + Math.Pow(i - lMeanValue, 2);
                            }
                        }
                    }
                    lDeviation = Math.Sqrt(lDeviation / lCount);
                    lBuilder.AppendLine("Most common number of muban: " + lMostCommonNumber.ToString() + " (" + lMostCommonNumberValue.ToString() + " times)");
                    lBuilder.AppendLine("Mean number of muban: " + lMeanValue.ToString());
                    lBuilder.AppendLine("Standard deviation: " + lDeviation.ToString());
                    lBuilder.AppendLine("Lowest number of muban: " + lLowestNumber.ToString());
                    foreach (Int32 lGeocode in mNumberOfMuban[lLowestNumber])
                    {
                        lBuilder.Append(lGeocode.ToString() + ' ');
                    }
                    lBuilder.AppendLine();
                    lBuilder.AppendLine("Highest number of muban: " + lHighestNumber.ToString());
                    foreach (Int32 lGeocode in mNumberOfMuban[lHighestNumber])
                    {
                        lBuilder.Append(lGeocode.ToString() + ' ');
                    }
                    lBuilder.AppendLine();
                }
            }

            String retval = lBuilder.ToString();
            return retval;
        }
    }
}