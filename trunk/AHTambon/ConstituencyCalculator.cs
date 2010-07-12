using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public class ConstituencyCalculator
    {
        #region constructor
        public ConstituencyCalculator()
        { }
        #endregion

        #region methods
        public static Dictionary<PopulationDataEntry, Int32> Calculate(Int32 iParentGeocode, Int32 iYear, Int32 iNumberOfSeats)
        {
            Dictionary<PopulationDataEntry, Int32> retval = null;

            PopulationData lDownloader = new PopulationData(iYear, iParentGeocode);
            lDownloader.Process();
            retval = Calculate(lDownloader.Data, iYear, iNumberOfSeats);
            return retval;
        }
        public static Dictionary<PopulationDataEntry, Int32> Calculate(PopulationDataEntry iData, Int32 iYear, Int32 iNumberOfSeats)
        {
            Dictionary<PopulationDataEntry, Int32> retval = new Dictionary<PopulationDataEntry, Int32>();

            Int32 lTotalPopulation = 0;
            foreach (PopulationDataEntry lEntry in iData.SubEntities)
            {
                retval.Add(lEntry, 0);
                lTotalPopulation += lEntry.Total;
            }
            double lDivisor = (1.0*iNumberOfSeats) / (1.0*lTotalPopulation);
            Int32 lRemainingSeat = iNumberOfSeats;
            Dictionary<PopulationDataEntry, double> lRemainder = new Dictionary<PopulationDataEntry, double>();
            foreach (PopulationDataEntry lEntry in iData.SubEntities)
            {
                double lSeats = lEntry.Total * lDivisor;
                Int32 lActualSeats = Math.Max(1, Convert.ToInt32(Math.Truncate(lSeats)));
                retval[lEntry] = lActualSeats;
                lRemainingSeat -= lActualSeats;
                double lRemainingValue = lSeats - lActualSeats;
                lRemainder.Add(lEntry, lRemainingValue);
            }

            List<PopulationDataEntry> lSortedRemainders = new List<PopulationDataEntry>();
            foreach (PopulationDataEntry lEntry in iData.SubEntities)
            {
                lSortedRemainders.Add(lEntry);
            }
            lSortedRemainders.Sort(delegate(PopulationDataEntry p1, PopulationDataEntry p2) { return lRemainder[p2].CompareTo(lRemainder[p1]); });

            while (lRemainingSeat > 0)
            {
                PopulationDataEntry lFirst = lSortedRemainders.First();
                retval[lFirst] += 1;
                lRemainingSeat -= 1;
                lSortedRemainders.Remove(lFirst);
            }

            return retval;
        }
        #endregion
    }
}
