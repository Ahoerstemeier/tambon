using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    class CreationStatisticsMuban:CreationStatistics
    {
        #region properties
        private Int32 mNumberOfMubanCreations;
        public Int32 NumberOfMubanCreations { get { return mNumberOfMubanCreations; } }
        private List<Int32>[] mHighestMubanNumber = new List<Int32>[MAXIMUMMUBAN];
        private Dictionary<String,Int32> mNewNameSuffix = new Dictionary<string,Int32>();
        #endregion
        #region constructor
        public CreationStatisticsMuban()
        {
            StartYear = 1883;
            EndYear = DateTime.Now.Year;
        }
        public CreationStatisticsMuban(Int32 iStartYear, Int32 iEndYear)
        {
            StartYear = iStartYear;
            EndYear = iEndYear;
        }
        #endregion
        #region methods
        protected override void Clear()
        {
            base.Clear();
            mNumberOfMubanCreations = 0;
            mNewNameSuffix = new Dictionary<string, Int32>();
            mHighestMubanNumber = new List<Int32>[MAXIMUMMUBAN];
        }
        protected override void ProcessContent(RoyalGazetteContent iContent)
        {
            RoyalGazetteContentCreate lCreate = (RoyalGazetteContentCreate)iContent;
            mNumberOfMubanCreations++;

            Int32 lMubanNumber = lCreate.Geocode % 100;
            if (lMubanNumber != lCreate.Geocode)
            {
                if (mHighestMubanNumber[lMubanNumber] == null)
                {
                    mHighestMubanNumber[lMubanNumber] = new List<Int32>();
                }
                mHighestMubanNumber[lMubanNumber].Add(lCreate.Geocode);
            }
            if (!String.IsNullOrEmpty(lCreate.Name))
            {
                String lParentName = String.Empty;
                foreach (RoyalGazetteContent lSubEntry in lCreate.SubEntities)
                {
                    if (lSubEntry is RoyalGazetteContentAreaChange)
                    {
                        lParentName = lSubEntry.Name;
                    }
                }
                if (!String.IsNullOrEmpty(lParentName))
                {
                    if (lCreate.Name.StartsWith(lParentName))
                    {
                        String lSuffix = lCreate.Name.Remove(0, lParentName.Length);
                        if (mNewNameSuffix.ContainsKey(lSuffix))
                        {
                            mNewNameSuffix[lSuffix]++;
                        }
                        else
                        {
                            mNewNameSuffix.Add(lSuffix, 1);
                        }
                    }
                }
            }
        }

        protected override Boolean ContentFitting(RoyalGazetteContent iContent)
        {
            Boolean retval = false;
            if (iContent is RoyalGazetteContentCreate)
            {
                RoyalGazetteContentCreate lCreate = (RoyalGazetteContentCreate)iContent;
                retval = (lCreate.Status == EntityType.Muban);
            }
            return retval;
        }
        protected Int32 SuffixFrequency(String iSuffix)
        {
            Int32 retval = 0;
            if (mNewNameSuffix.ContainsKey(iSuffix))
            {
                retval = mNewNameSuffix[iSuffix];
            }
            return retval;
        }
        public override String Information()
        {
            StringBuilder lBuilder = new StringBuilder();
            lBuilder.AppendLine(NumberOfAnnouncements.ToString() + " Announcements");
            lBuilder.AppendLine(NumberOfMubanCreations.ToString() + " Muban created");
            lBuilder.AppendLine();
            lBuilder.AppendLine("Name equals " + SuffixFrequency(String.Empty).ToString() + " times");
            List<String> lStandardSuffices = new List<String>() { "เหนือ", "ใต้", "พัฒนา"};
            foreach (String lSuffix in lStandardSuffices)
            {
                lBuilder.AppendLine("Suffix "+lSuffix+": "+SuffixFrequency(lSuffix).ToString()+" times");
            }
            String retval = lBuilder.ToString();
            return retval;
        }
        #endregion
    }
}
