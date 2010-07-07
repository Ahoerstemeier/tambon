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
        public static Dictionary<PopulationDataEntry, Int32> Calculate(Int32 iParentGeocode, Int32 iYear, Int32 NumberOfSeats)
        {
            Dictionary<PopulationDataEntry, Int32> retval = new Dictionary<PopulationDataEntry, Int32>();

            PopulationData lDownloader = new PopulationData(iYear, iParentGeocode);
            lDownloader.Process();
            Int32 lTotalPopulation = 0;
            foreach (PopulationDataEntry lEntry in lDownloader.Data.SubEntities)
            {
                retval.Add(lEntry, 0);
                lTotalPopulation += lEntry.Total;
            }
            double lDivisor = (1.0*NumberOfSeats) / (1.0*lTotalPopulation);
            Int32 lRemainingSeat = NumberOfSeats;
            Dictionary<PopulationDataEntry, double> lRemainder = new Dictionary<PopulationDataEntry, double>();
            foreach (PopulationDataEntry lEntry in lDownloader.Data.SubEntities)
            {
                double lSeats = lEntry.Total * lDivisor;
                Int32 lActualSeats = Math.Max(1, Convert.ToInt32(Math.Truncate(lSeats)));
                retval[lEntry] = lActualSeats;
                lRemainingSeat -= lActualSeats;
                double lRemainingValue = lSeats - lActualSeats;
                lRemainder.Add(lEntry, lRemainingValue);
            }

            List<PopulationDataEntry> lSortedRemainders = new List<PopulationDataEntry>();
            foreach (PopulationDataEntry lEntry in lDownloader.Data.SubEntities)
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
