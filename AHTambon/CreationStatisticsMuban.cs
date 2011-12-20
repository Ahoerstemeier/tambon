using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace De.AHoerstemeier.Tambon
{
    public class CreationStatisticsMuban : CreationStatisticsCentralGovernment
    {
        #region properties
        private Int32 mCreationsWithoutParentName;
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
        protected override String DisplayEntityName()
        {
            return "Muban";
        }
        protected override void Clear()
        {
            base.Clear();
            mNewNameSuffix = new Dictionary<string, Int32>();
            mNewNamePrefix = new Dictionary<string, Int32>();
            mHighestMubanNumber = new FrequencyCounter();
            mCreationsWithoutParentName = 0;
        }
        protected override Boolean EntityFitting(EntityType iEntityType)
        {
            Boolean result = (iEntityType == EntityType.Muban);
            return result;
        }
        protected void ProcessContentForName(RoyalGazetteContentCreate iCreate)
        {
            Int32 lTambonGeocode = iCreate.Geocode / 100;
            String lName = TambonHelper.StripBanOrChumchon(iCreate.Name);
            if (!String.IsNullOrEmpty(lName))
            {
                String lParentName = String.Empty;
                foreach (RoyalGazetteContent lSubEntry in iCreate.SubEntries)
                {
                    if (lSubEntry is RoyalGazetteContentAreaChange)
                    {
                        lParentName = lSubEntry.Name;
                        Debug.Assert(lTambonGeocode == (lSubEntry.Geocode / 100), "Parent muban as a different geocode");
                    }
                }
                lParentName = TambonHelper.StripBanOrChumchon(lParentName);
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
        protected override void ProcessContent(RoyalGazetteContent iContent)
        {
            base.ProcessContent(iContent);
            RoyalGazetteContentCreate lCreate = (RoyalGazetteContentCreate)iContent;

            Int32 lMubanNumber = lCreate.Geocode % 100;
            if (lMubanNumber != lCreate.Geocode)
            {
                mHighestMubanNumber.IncrementForCount(lMubanNumber,lCreate.Geocode);
            }
            ProcessContentForName(lCreate);
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
                String lName = TambonHelper.ReplaceThaiNumerals(lKeyValue.Key);
                if ((!String.IsNullOrEmpty(lName))&&(TambonHelper.IsNumeric(lName)))
                {
                    retval=retval+lKeyValue.Value;
                }
            }
            return retval;
        }
        public override String Information()
        {
            StringBuilder lBuilder = new StringBuilder();
            AppendBasicInfo(lBuilder);
            AppendProblems(lBuilder);
            AppendChangwatInfo(lBuilder);
            AppendMubanNumberInfo(lBuilder);
            AppendParentFrequencyInfo(lBuilder,"Tambon");
            AppendChangwatInfo(lBuilder);
            AppendDayOfYearInfo(lBuilder);
            AppendNameInfo(lBuilder);

            String retval = lBuilder.ToString();
            return retval;
        }

        private void AppendProblems(StringBuilder iBuilder)
        {
            if (mCreationsWithoutParentName > 0)
            {
                iBuilder.AppendLine(mCreationsWithoutParentName.ToString() + " have no parent name");
            }
        }
        private void AppendMubanNumberInfo(StringBuilder iBuilder)
        {
            iBuilder.AppendLine("Highest number of muban: " + mHighestMubanNumber.MaxValue.ToString());
            if (mHighestMubanNumber.MaxValue > 0)
            {
                foreach (Int32 lGeocode in mHighestMubanNumber.Data[mHighestMubanNumber.MaxValue])
                {
                    iBuilder.Append(lGeocode.ToString() + ' ');
                }
            }
            iBuilder.AppendLine();
        }
        private void AppendNameInfo(StringBuilder iBuilder)
        {
            iBuilder.AppendLine("Name equal: " + SuffixFrequency(String.Empty).ToString() + " times");
            List<String> lStandardSuffices = new List<String>() { "เหนือ", "ใต้", "พัฒนา", "ใหม่", "ทอง", "น้อย", "ใน" };
            foreach (String lSuffix in lStandardSuffices)
            {
                iBuilder.AppendLine("Suffix " + lSuffix + ": " + SuffixFrequency(lSuffix).ToString() + " times");
            }
            iBuilder.AppendLine("Suffix with number:" + SuffixFrequencyNumbers().ToString() + " times");

            List<String> lStandardPrefixes = new List<String>() { "ใหม่" };
            foreach (String lPrefix in lStandardPrefixes)
            {
                iBuilder.AppendLine("Prefix " + lPrefix + ": " + PrefixFrequency(lPrefix).ToString() + " times");
            }

            iBuilder.AppendLine();

            iBuilder.Append("Other suffices: ");
            List<KeyValuePair<String, Int32>> lSortedSuffices = new List<KeyValuePair<String, Int32>>();
            foreach (KeyValuePair<String, Int32> lKeyValuePair in mNewNameSuffix)
            {
                String lName = TambonHelper.ReplaceThaiNumerals(lKeyValuePair.Key);
                if (lStandardSuffices.Contains(lName))
                { }
                else if (String.IsNullOrEmpty(lKeyValuePair.Key))
                { }
                else if (TambonHelper.IsNumeric(lName))
                { }
                else
                {
                    lSortedSuffices.Add(lKeyValuePair);
                }
            }
            lSortedSuffices.Sort(delegate(KeyValuePair<String, Int32> x, KeyValuePair<String, Int32> y) { return y.Value.CompareTo(x.Value); });
            foreach (KeyValuePair<String, Int32> lKeyValuePair in lSortedSuffices)
            {
                iBuilder.Append(lKeyValuePair.Key + " (" + lKeyValuePair.Value.ToString() + ") ");
            }
            iBuilder.AppendLine();
        }
        #endregion
    }
}
