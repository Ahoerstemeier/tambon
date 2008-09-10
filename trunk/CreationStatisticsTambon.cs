using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    class CreationStatisticsTambon:CreationStatistics
    {
        #region properties
        private Int32 mNumberOfTambonCreations;
        public Int32 NumberOfTambonCreations { get { return mNumberOfTambonCreations; } }
        private List<Int32>[] mNumberOfMuban = new List<Int32>[MAXIMUMMUBAN];
        private Int32[] mNumberOfTambonCreationsPerChangwat = new Int32[100];
        #endregion
        #region constructor
        public CreationStatisticsTambon()
        {
            StartYear = 1883;
            EndYear = DateTime.Now.Year;
        }
        public CreationStatisticsTambon(Int32 iStartYear, Int32 iEndYear)
        {
            StartYear = iStartYear;
            EndYear = iEndYear;
        }
        #endregion
        #region methods
        protected override void Clear()
        {
            base.Clear();
            mNumberOfMuban = new List<Int32>[MAXIMUMMUBAN];
            mNumberOfTambonCreationsPerChangwat = new Int32[100];
            mNumberOfTambonCreations = 0;
        }
        protected override Boolean ContentFitting(RoyalGazetteContent iContent)
        { 
            Boolean retval = false;
            if (iContent is RoyalGazetteContentCreate)
            {
                RoyalGazetteContentCreate lCreate = (RoyalGazetteContentCreate)iContent;
                retval = (lCreate.Status == EntityType.Tambon);
            }
            return retval;
        }
        protected override void ProcessContent(RoyalGazetteContent iContent)
        {
            RoyalGazetteContentCreate lCreate = (RoyalGazetteContentCreate)iContent;
            mNumberOfTambonCreations++;

            Int32 lGeocode = lCreate.Geocode;
            while (lGeocode > 100)
            {
                lGeocode = lGeocode / 100;
            }
            mNumberOfTambonCreationsPerChangwat[lGeocode]++;

            Int32 lMaxMubanIndex = 0;
            foreach (RoyalGazetteContent lSubEntry in lCreate.SubEntities)
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
            mNumberOfMuban[lMaxMubanIndex].Add(lCreate.Geocode);
        }
        public override String Information()
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
        #endregion

    }
}