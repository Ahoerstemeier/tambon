using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace De.AHoerstemeier.Tambon
{
    class CreationStatisticsMuban:CreationStatistics
    {
        #region properties
        private Int32 mCreationsWithoutParentName;
        private Int32 mNumberOfMubanCreations;
        public Int32 NumberOfMubanCreations { get { return mNumberOfMubanCreations; } }
        private FrequencyCounter mHighestMubanNumber = new FrequencyCounter();
        private Dictionary<String,Int32> mNewNameSuffix = new Dictionary<string,Int32>();
        private Dictionary<String, Int32> mNewNamePrefix = new Dictionary<string, Int32>();
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
            mNewNamePrefix = new Dictionary<string, Int32>();
            mHighestMubanNumber = new FrequencyCounter();
            mCreationsWithoutParentName = 0;
        }
        protected override void ProcessContent(RoyalGazetteContent iContent)
        {
            RoyalGazetteContentCreate lCreate = (RoyalGazetteContentCreate)iContent;
            mNumberOfMubanCreations++;

            Int32 lMubanNumber = lCreate.Geocode % 100;
            if (lMubanNumber != lCreate.Geocode)
            {
                mHighestMubanNumber.IncrementForCount(lMubanNumber,lCreate.Geocode);
            }
            String lName = StripBan(lCreate.Name);
            if (!String.IsNullOrEmpty(lName))
            {
                String lParentName = String.Empty;
                foreach (RoyalGazetteContent lSubEntry in lCreate.SubEntities)
                {
                    if (lSubEntry is RoyalGazetteContentAreaChange)
                    {
                        lParentName = lSubEntry.Name;
                    }
                }
                lParentName = StripBan(lParentName);
                if (!String.IsNullOrEmpty(lParentName))
                {
                    if (lName.StartsWith(lParentName))
                    {
                        String lSuffix = lName.Remove(0, lParentName.Length).Trim();
                        if (mNewNameSuffix.ContainsKey(lSuffix))
                        {
                            mNewNameSuffix[lSuffix]++;
                        }
                        else
                        {
                            mNewNameSuffix.Add(lSuffix, 1);
                        }
                    }
                    if (lName.EndsWith(lParentName))
                    {
                        String lPrefix = lName.Replace(lParentName, "").Trim();

                        if (mNewNamePrefix.ContainsKey(lPrefix))
                        {
                            mNewNamePrefix[lPrefix]++;
                        }
                        else
                        {
                            mNewNamePrefix.Add(lPrefix, 1);
                        }
                    }
                }
                else
                {
                    mCreationsWithoutParentName++;
                }
            }
        }

        private String StripBan(String iName)
        {
            const String ThaiStringBan = "บ้าน";
            String retval = iName;
            if (iName.StartsWith(ThaiStringBan))
            {
                retval = iName.Remove(0, ThaiStringBan.Length).Trim();
            }
            return retval;
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
        protected Int32 PrefixFrequency(String iSuffix)
        {
            Int32 retval = 0;
            if (mNewNamePrefix.ContainsKey(iSuffix))
            {
                retval = mNewNamePrefix[iSuffix];
            }
            return retval;
        }
        protected Int32 SuffixFrequencyNumbers()
        {
            Int32 retval = 0;
            foreach (KeyValuePair<String, Int32> lKeyValue in mNewNameSuffix)
            {
                String lName = Helper.ReplaceThaiNumerals(lKeyValue.Key);
                if ((!String.IsNullOrEmpty(lName))&&(Helper.IsNumeric(lName)))
                {
                    retval++;
                }
            }
            return retval;
        }
        public override String Information()
        {
            StringBuilder lBuilder = new StringBuilder();
            lBuilder.AppendLine(NumberOfAnnouncements.ToString() + " Announcements");
            lBuilder.AppendLine(NumberOfMubanCreations.ToString() + " Muban created");
            lBuilder.AppendLine("Creations per announcements: " + CreationsPerAnnouncement.MeanValue.ToString("F2", CultureInfo.InvariantCulture));
            lBuilder.AppendLine("Maximum creation per announcements: " + CreationsPerAnnouncement.MaxValue.ToString());

            if (mCreationsWithoutParentName > 0)
            {
                lBuilder.AppendLine(mCreationsWithoutParentName.ToString() + " have no parent name");
            }
            lBuilder.AppendLine();
            lBuilder.AppendLine("Name equal: " + SuffixFrequency(String.Empty).ToString() + " times");
            List<String> lStandardSuffices = new List<String>() { "เหนือ", "ใต้", "พัฒนา", "ใหม่", "ทอง", "น้อย", "ใน"};
            foreach (String lSuffix in lStandardSuffices)
            {
                lBuilder.AppendLine("Suffix "+lSuffix+": "+SuffixFrequency(lSuffix).ToString()+" times");
            }
            lBuilder.AppendLine("Suffix with number:" + SuffixFrequencyNumbers().ToString() + " times");

            List<String> lStandardPrefixes = new List<String>() { "ใหม่" };
            foreach (String lPrefix in lStandardPrefixes)
            {
                lBuilder.AppendLine("Prefix " + lPrefix + ": " + PrefixFrequency(lPrefix).ToString() + " times");
            }

            lBuilder.AppendLine();
            lBuilder.AppendLine("Highest number of muban: " + mHighestMubanNumber.MaxValue.ToString());
            if (mHighestMubanNumber.MaxValue > 0)
            {
                foreach (Int32 lGeocode in mHighestMubanNumber.Data[mHighestMubanNumber.MaxValue])
                {
                    lBuilder.Append(lGeocode.ToString() + ' ');
                }
            }
            lBuilder.AppendLine();

            String retval = lBuilder.ToString();
            return retval;
        }
        #endregion
    }
}
