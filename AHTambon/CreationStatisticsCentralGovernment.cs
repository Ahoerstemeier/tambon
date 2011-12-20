using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace De.AHoerstemeier.Tambon
{
    public abstract class CreationStatisticsCentralGovernment : CreationStatistics
    {
        #region properties
        protected FrequencyCounter mNumberOfSubEntities = new FrequencyCounter();
        protected FrequencyCounter mNumberOfParentEntities = new FrequencyCounter();
        private Dictionary<Int32, Int32> mCreationsPerParent = new Dictionary<Int32, Int32>();
        #endregion
        #region methods
        protected override void Clear()
        {
            base.Clear();
            mNumberOfSubEntities = new FrequencyCounter();
            mNumberOfParentEntities = new FrequencyCounter();
            mCreationsPerParent = new Dictionary<Int32, Int32>();
        }
        protected override void ProcessContent(RoyalGazetteContent iContent)
        {
            base.ProcessContent(iContent);
            RoyalGazetteContentCreate lCreate = (RoyalGazetteContentCreate)iContent;

            Int32 lParentGeocode = lCreate.Geocode / 100;

            if (!mCreationsPerParent.ContainsKey(lParentGeocode))
            {
               mCreationsPerParent.Add(lParentGeocode, 0);
            }
            mCreationsPerParent[lParentGeocode]++;
            
            Int32 lMaxSubEntityIndex = 0;
            List<Int32> lParentEntities = new List<Int32>();
            foreach (RoyalGazetteContent lSubEntry in lCreate.SubEntries)
            {
                if (lSubEntry is RoyalGazetteContentCreate)
                {
                    lMaxSubEntityIndex++;
                }
                if (lSubEntry is RoyalGazetteContentReassign)
                {
                    lMaxSubEntityIndex++;

                    RoyalGazetteContentReassign lReassign = (RoyalGazetteContentReassign)lSubEntry;
                    Int32 lParentEntityCode = lReassign.OldGeocode / 100;
                    if (!lParentEntities.Contains(lParentEntityCode))
                    {
                        lParentEntities.Add(lParentEntityCode);
                    }
                }
            }

            mNumberOfSubEntities.IncrementForCount(lMaxSubEntityIndex, lCreate.Geocode);
            if (lParentEntities.Any())
            {
                mNumberOfParentEntities.IncrementForCount(lParentEntities.Count, lCreate.Geocode);
            }
        }
        protected virtual void AppendBasicInfo(StringBuilder iBuilder)
        {
            String lEntityName = DisplayEntityName();
            iBuilder.AppendLine(NumberOfAnnouncements.ToString() + " Announcements");
            iBuilder.AppendLine(NumberOfCreations.ToString() + " "+lEntityName+" created");
            iBuilder.AppendLine("Creations per announcements: " + CreationsPerAnnouncement.MeanValue.ToString("F2", CultureInfo.InvariantCulture));
            iBuilder.AppendLine("Maximum creation per announcements: " + CreationsPerAnnouncement.MaxValue.ToString());
            iBuilder.AppendLine();
        }
        protected void AppendChangwatInfo(StringBuilder iBuilder)
        {
            String lEntityName = DisplayEntityName();
            if (NumberOfCreations > 0)
            {
                List<String> lChangwat = new List<String>();
                Int32 lMaxNumber = 1;
                foreach (PopulationDataEntry lEntry in TambonHelper.ProvinceGeocodes)
                {
                    if (mNumberOfCreationsPerChangwat[lEntry.Geocode] > lMaxNumber)
                    {
                        lMaxNumber = mNumberOfCreationsPerChangwat[lEntry.Geocode];
                        lChangwat.Clear();
                    }
                    if (mNumberOfCreationsPerChangwat[lEntry.Geocode] == lMaxNumber)
                    {
                        lChangwat.Add(lEntry.English);
                    }
                }
                iBuilder.AppendLine(lMaxNumber.ToString() + " "+lEntityName+" created in");
                foreach (String lName in lChangwat)
                {
                    iBuilder.AppendLine("* " + lName);
                }
                iBuilder.AppendLine();
            }
        }
        protected void AppendSubEntitiesInfo(StringBuilder iBuilder, String iSubEntityName)
        {
            String lEntityName = DisplayEntityName();
            if (NumberOfCreations > 0)
            {
                iBuilder.AppendLine("Most common number of " + iSubEntityName + ": " + mNumberOfSubEntities.MostCommonValue.ToString() + " (" + mNumberOfSubEntities.MostCommonValueCount.ToString() + " times)");
                iBuilder.AppendLine("Mean number of " + iSubEntityName + ": " + mNumberOfSubEntities.MeanValue.ToString("F2", CultureInfo.InvariantCulture));
                iBuilder.AppendLine("Standard deviation: " + mNumberOfSubEntities.StandardDeviation.ToString("F2", CultureInfo.InvariantCulture));
                iBuilder.AppendLine("Lowest number of " + iSubEntityName + ": " + mNumberOfSubEntities.MinValue.ToString());
                if (mNumberOfSubEntities.MinValue > 0)
                {
                    foreach (Int32 lGeocode in mNumberOfSubEntities.Data[mNumberOfSubEntities.MinValue])
                    {
                        iBuilder.Append(lGeocode.ToString() + ' ');
                    }
                    iBuilder.AppendLine();
                }
                iBuilder.AppendLine("Highest number of " + iSubEntityName + ": " + mNumberOfSubEntities.MaxValue.ToString());
                if (mNumberOfSubEntities.MaxValue > 0)
                {
                    foreach (Int32 lGeocode in mNumberOfSubEntities.Data[mNumberOfSubEntities.MaxValue])
                    {
                        iBuilder.Append(lGeocode.ToString() + ' ');
                    }
                    iBuilder.AppendLine();
                }
                iBuilder.AppendLine();
            }
        }
        protected void AppendParentNumberInfo(StringBuilder iBuilder)
        {
            String lParentName = DisplayEntityName();
            if (NumberOfCreations > 0)
            {
                iBuilder.AppendLine("Highest number of parent " + lParentName + ": " + mNumberOfParentEntities.MaxValue.ToString());
                if (mNumberOfParentEntities.MaxValue > 1)
                {
                    foreach (Int32 lGeocode in mNumberOfParentEntities.Data[mNumberOfParentEntities.MaxValue])
                    {
                        iBuilder.Append(lGeocode.ToString() + ' ');
                    }
                    iBuilder.AppendLine();
                }
                Int32[] lNumberOfParentEntities = new Int32[mNumberOfParentEntities.MaxValue + 1];
                foreach (KeyValuePair<Int32, List<Int32>> lParentEntity in mNumberOfParentEntities.Data)
                {
                    lNumberOfParentEntities[lParentEntity.Key] = lParentEntity.Value.Count;
                }
                for (Int32 i = 0; i <= mNumberOfParentEntities.MaxValue; i++)
                {
                    if (lNumberOfParentEntities[i] != 0)
                    {
                        iBuilder.AppendLine(i.ToString() + ": " + lNumberOfParentEntities[i].ToString());
                    }
                }
                iBuilder.AppendLine();
            }
        }
        protected void AppendParentFrequencyInfo(StringBuilder iBuilder, String iParentEntityName)
        {
            String lEntityName = DisplayEntityName();
            List<KeyValuePair<Int32, Int32>> lSortedParents = new List<KeyValuePair<Int32, Int32>>();
            lSortedParents.AddRange(mCreationsPerParent);
            lSortedParents.Sort(delegate(KeyValuePair<Int32, Int32> x, KeyValuePair<Int32, Int32> y) { return y.Value.CompareTo(x.Value); });
            if (lSortedParents.Any())
            {
                KeyValuePair<Int32, Int32> lFirst = lSortedParents.ElementAt(0);
                iBuilder.AppendLine("Most "+lEntityName+" created in one " + iParentEntityName + ": " + lFirst.Value.ToString());
                foreach (KeyValuePair<Int32, Int32> lEntry in lSortedParents.FindAll(
                    delegate(KeyValuePair<Int32, Int32> x) { return (x.Value == lFirst.Value); }))
                {
                    iBuilder.Append(lEntry.Key.ToString() + ' ');
                }
                iBuilder.Remove(iBuilder.Length - 1, 1);
            }
            iBuilder.AppendLine();
        }
        protected void AppendDayOfYearInfo(StringBuilder iBuilder)
        {
            String lEntityName = DisplayEntityName();
            DateTime lBaseDateTime = new DateTime(2004, 1, 1);
            List<KeyValuePair<Int32, Int32>> lSortedDays = new List<KeyValuePair<Int32, Int32>>();
            lSortedDays.AddRange(EffectiveDayOfYear);
            lSortedDays.Sort(delegate(KeyValuePair<Int32, Int32> x, KeyValuePair<Int32, Int32> y) { return y.Value.CompareTo(x.Value); });
            Int32 lCount = 0;
            if (lSortedDays.Any())
            {
                foreach (KeyValuePair<Int32, Int32> lData in lSortedDays)
                {
                    DateTime lCurrent = lBaseDateTime.AddDays(lData.Key-1);
                    iBuilder.AppendLine(lCurrent.ToString("MMM dd") + ": " + EffectiveDayOfYear[lData.Key].ToString() + " " + lEntityName + " created");
                    lCount++;
                    if (lCount > 10)
                    {
                        break;
                    }
                }
                iBuilder.AppendLine();
            }
        }
        protected abstract String DisplayEntityName();
        #endregion
    }
}
