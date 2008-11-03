using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;

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
        private Dictionary<Int32, Int32> mMubanCreationsInTambon = new Dictionary<Int32, Int32>();
        private Dictionary<Int32, Int32> mMubanCreationsInChangwat = new Dictionary<Int32, Int32>();
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
            mMubanCreationsInTambon = new Dictionary<Int32, Int32>();
            mMubanCreationsInChangwat = new Dictionary<Int32, Int32>();
        }
        protected override void ProcessContent(RoyalGazetteContent iContent)
        {
            RoyalGazetteContentCreate lCreate = (RoyalGazetteContentCreate)iContent;
            mNumberOfMubanCreations++;

            Int32 lMubanNumber = lCreate.Geocode % 100;
            Int32 lTambonGeocode = lCreate.Geocode / 100;
            if (lMubanNumber != lCreate.Geocode)
            {
                mHighestMubanNumber.IncrementForCount(lMubanNumber,lCreate.Geocode);
                if (!mMubanCreationsInTambon.ContainsKey(lTambonGeocode))
                {
                    mMubanCreationsInTambon.Add(lTambonGeocode, 0);
                }
                mMubanCreationsInTambon[lTambonGeocode]++;
                Int32 lChangwatGeocode = lTambonGeocode;
                while (lChangwatGeocode > 100)
                {
                    lChangwatGeocode = lChangwatGeocode / 100;
                }
                if (!mMubanCreationsInChangwat.ContainsKey(lChangwatGeocode))
                {
                    mMubanCreationsInChangwat.Add(lChangwatGeocode, 0);
                }
                mMubanCreationsInChangwat[lChangwatGeocode]++;
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
                        Debug.Assert(lTambonGeocode == (lSubEntry.Geocode / 100),"Parent muban as a different geocode");
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
                    retval=retval+lKeyValue.Value;
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
            lBuilder.AppendLine("Highest number of muban: " + mHighestMubanNumber.MaxValue.ToString());
            if (mHighestMubanNumber.MaxValue > 0)
            {
                foreach (Int32 lGeocode in mHighestMubanNumber.Data[mHighestMubanNumber.MaxValue])
                {
                    lBuilder.Append(lGeocode.ToString() + ' ');
                }
            }
            lBuilder.AppendLine();

            List<KeyValuePair<Int32, Int32>> lSortedTambon = new List<KeyValuePair<Int32,Int32>>();
            lSortedTambon.AddRange(mMubanCreationsInTambon);
            lSortedTambon.Sort(delegate(KeyValuePair<Int32, Int32> x, KeyValuePair<Int32, Int32> y) { return y.Value.CompareTo(x.Value); });
            if (lSortedTambon.Count > 0)
            {
                KeyValuePair<Int32, Int32> lFirst = lSortedTambon.ElementAt(0);
                lBuilder.AppendLine("Most muban created in one tambon: " + lFirst.Value.ToString());
                foreach (KeyValuePair<Int32, Int32> lEntry in lSortedTambon.FindAll(
                    delegate(KeyValuePair<Int32, Int32> x) { return (x.Value == lFirst.Value); }))
                {
                    lBuilder.Append(lEntry.Key.ToString() + ' ');
                }
                lBuilder.Remove(lBuilder.Length - 1, 1);
            }
            lBuilder.AppendLine();
            List<KeyValuePair<Int32, Int32>> lSortedChangwat = new List<KeyValuePair<Int32, Int32>>();
            lSortedChangwat.AddRange(mMubanCreationsInChangwat);
            lSortedChangwat.Sort(delegate(KeyValuePair<Int32, Int32> x, KeyValuePair<Int32, Int32> y) { return y.Value.CompareTo(x.Value); });
            if (lSortedChangwat.Count > 0)
            {
                KeyValuePair<Int32, Int32> lFirst = lSortedChangwat.ElementAt(0);
                lBuilder.AppendLine("Most muban created in province: " + lFirst.Value.ToString());
                foreach (KeyValuePair<Int32, Int32> lEntry in lSortedChangwat.FindAll(
                    delegate(KeyValuePair<Int32, Int32> x) { return (x.Value == lFirst.Value); }))
                {
                    PopulationDataEntry lGeocodeData = Helper.Geocodes.Find(delegate(PopulationDataEntry x) { return (x.Geocode == lEntry.Key); });
                    Debug.Assert(lGeocodeData != null, "Geocode not found");
                    lBuilder.Append(lGeocodeData.English + ", ");
                }
                lBuilder.Remove(lBuilder.Length - 2, 2);
            }
            lBuilder.AppendLine();
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

            lBuilder.Append("Other suffices: ");
            List<KeyValuePair<String, Int32>> lSortedSuffices = new List<KeyValuePair<String, Int32>>();
            foreach (KeyValuePair<String, Int32> lKeyValuePair in mNewNameSuffix)
            {
                String lName = Helper.ReplaceThaiNumerals(lKeyValuePair.Key);
                if (lStandardSuffices.Contains(lName))
                { }
                else if (String.IsNullOrEmpty(lKeyValuePair.Key))
                { }
                else if (Helper.IsNumeric(lName))
                { }
                else
                {
                    lSortedSuffices.Add(lKeyValuePair);
                }
            }
            lSortedSuffices.Sort(delegate(KeyValuePair<String, Int32> x, KeyValuePair<String, Int32> y) { return y.Value.CompareTo(x.Value); });
            foreach (KeyValuePair<String, Int32> lKeyValuePair in lSortedSuffices)
            {
                lBuilder.Append(lKeyValuePair.Key + " (" + lKeyValuePair.Value.ToString() + ") ");
            }
            lBuilder.AppendLine();

            String retval = lBuilder.ToString();
            return retval;
        }
        #endregion
    }
}
