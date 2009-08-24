using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace De.AHoerstemeier.Tambon
{
    class EntityCounter
    {
        private Dictionary<String, Int32>  mNamesCount = new Dictionary<String, Int32>();
        private List<EntityType> mEntityTypes;
        public Int32 BaseGeocode { get; set; }

        public EntityCounter(List<EntityType> iEntityTypes)
        {
            mEntityTypes = iEntityTypes;
        }
        public String CommonNames(Int32 iCutOff)
        {
            StringBuilder lBuilder = new StringBuilder();

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
    
        public void Calculate()
        {
            var lList = new List<PopulationDataEntry>();
            foreach (PopulationDataEntry lEntry in Helper.Geocodes)
            {
                String lFilename = Helper.GeocodeSourceFile(lEntry.Geocode);
                if (File.Exists(lFilename))
                {
                    PopulationData lEntities = PopulationData.Load(lFilename);
                    lList.AddRange(lEntities.Data.FlatList(mEntityTypes));
                }
            }
            lList.Sort(delegate(PopulationDataEntry p1, PopulationDataEntry p2) { return p1.Name.CompareTo(p2.Name); });
            String lLastname = String.Empty;
            Int32 lCount = 0;
            foreach (PopulationDataEntry lEntry in lList)
            {
                if (lEntry.NewGeocode.Count != 0)
                {
                }
                else if (lEntry.Name == lLastname)
                {
                    lCount++;
                }
                else
                {
                    if (!String.IsNullOrEmpty(lLastname))
                    {
                        mNamesCount.Add(lLastname, lCount + 1);
                    }
                    lCount = 0;
                    lLastname = lEntry.Name;
                }
            }
            if (!String.IsNullOrEmpty(lLastname))
            {
                mNamesCount.Add(lLastname, lCount + 1);
            }
        
        }
    }
}
