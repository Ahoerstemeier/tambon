using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace De.AHoerstemeier.Tambon
{
    public class EntityCounter
    {
        #region properties
        private Dictionary<String, Int32> mNamesCount = new Dictionary<String, Int32>();
        private List<EntityType> mEntityTypes;
        public Int32 BaseGeocode { get; set; }
        private Int32 mNumberOfEntities;
        public Int32 NumberOfEntities { get { return mNumberOfEntities; } }
        #endregion

        #region constructor
        public EntityCounter(List<EntityType> iEntityTypes)
        {
            mEntityTypes = iEntityTypes;
        }
        #endregion

        #region methods
        public String CommonNames(Int32 iCutOff)
        {
            StringBuilder lBuilder = new StringBuilder();

            lBuilder.AppendLine("Total number: " + NumberOfEntities.ToString());

            List<KeyValuePair<String, Int32>> lSorted = new List<KeyValuePair<String, Int32>>();
            foreach (KeyValuePair<String, Int32> lKeyValuePair in mNamesCount)
            {
                String lName = lKeyValuePair.Key;
                lSorted.Add(lKeyValuePair);
            }
            lSorted.Sort(delegate(KeyValuePair<String, Int32> x, KeyValuePair<String, Int32> y) { return y.Value.CompareTo(x.Value); });
            Int32 lCount = 0;
            foreach (KeyValuePair<String, Int32> lKeyValuePair in lSorted)
            {
                lBuilder.AppendLine(lKeyValuePair.Key + " (" + lKeyValuePair.Value.ToString() + ") ");
                lCount++;
                if (lCount > iCutOff)
                {
                    break;
                }

            }

            String RetVal = lBuilder.ToString();
            return RetVal;
        }

        private List<PopulationDataEntry> LoadGeocodeLists()
        {
            var lList = new List<PopulationDataEntry>();
            foreach (PopulationDataEntry lEntry in TambonHelper.ProvinceGeocodes)
            {
                if (TambonHelper.IsBaseGeocode(BaseGeocode, lEntry.Geocode))
                {
                    PopulationData lEntities = TambonHelper.GetGeocodeList(lEntry.Geocode);
                    lList.AddRange(lEntities.Data.FlatList(mEntityTypes));
                }
            }
            return lList;
        }
        private List<PopulationDataEntry> NormalizeNames(List<PopulationDataEntry> iList)
        {
            List<PopulationDataEntry> lResult = new List<PopulationDataEntry>();
            foreach (PopulationDataEntry lEntry in iList)
            {
                if (lEntry.Type == EntityType.Muban)
                {
                    lEntry.Name = TambonHelper.StripBanOrChumchon(lEntry.Name);
                }
                if ((!lEntry.IsObsolete()) & (TambonHelper.IsBaseGeocode(BaseGeocode, lEntry.Geocode)))
                {
                    lResult.Add(lEntry);
                }
            }
            lResult.Sort(delegate(PopulationDataEntry p1, PopulationDataEntry p2) { return p1.Name.CompareTo(p2.Name); });
            return lResult;
        }
        private static Dictionary<String, Int32> DoCalculate(List<PopulationDataEntry> iList)
        {
            Dictionary<String, Int32> lResult = new Dictionary<String, Int32>();
            String lLastname = String.Empty;
            Int32 lCount = 0;
            foreach (PopulationDataEntry lEntry in iList)
            {
                if (lEntry.Name == lLastname)
                {
                    lCount++;
                }
                else
                {
                    if (!String.IsNullOrEmpty(lLastname))
                    {
                        lResult.Add(lLastname, lCount + 1);
                    }
                    lCount = 0;
                    lLastname = lEntry.Name;
                }
            }
            if (!String.IsNullOrEmpty(lLastname))
            {
                lResult.Add(lLastname, lCount + 1);
            }
            return lResult;
        }
        public void Calculate()
        {
            var lList = LoadGeocodeLists();
            Calculate(lList);
        }
        public void Calculate(List<PopulationDataEntry> iInputList)
        {
            var lList = NormalizeNames(iInputList);
            mNumberOfEntities = lList.Count;
            mNamesCount = DoCalculate(lList);
        }
        #endregion
    }
}
