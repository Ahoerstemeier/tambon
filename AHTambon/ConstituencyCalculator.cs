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
        public static Dictionary<PopulationDataEntry, Int32> Calculate(Int32 parentGeocode, Int32 year, Int32 numberOfSeats)
        {
            Dictionary<PopulationDataEntry, Int32> retval = null;

            PopulationData downloader = new PopulationData(year, parentGeocode);
            downloader.Process();
            retval = Calculate(downloader.Data, year, numberOfSeats);
            return retval;
        }
        public static Dictionary<PopulationDataEntry, Int32> Calculate(PopulationDataEntry data, Int32 year, Int32 numberOfSeats)
        {
            Dictionary<PopulationDataEntry, Int32> result = new Dictionary<PopulationDataEntry, Int32>();

            Int32 totalPopulation = 0;
            foreach (PopulationDataEntry entry in data.SubEntities)
            {
                if (entry != null)
                {
                    result.Add(entry, 0);
                    totalPopulation += entry.Total;
                }
            }
            double divisor = (1.0*numberOfSeats) / (1.0*totalPopulation);
            Int32 remainingSeat = numberOfSeats;
            Dictionary<PopulationDataEntry, double> remainder = new Dictionary<PopulationDataEntry, double>();
            foreach (PopulationDataEntry entry in data.SubEntities)
            {
                if (entry != null)
                {
                    double seats = entry.Total * divisor;
                    Int32 actualSeats = Math.Max(1, Convert.ToInt32(Math.Truncate(seats)));
                    result[entry] = actualSeats;
                    remainingSeat -= actualSeats;
                    double remainingValue = seats - actualSeats;
                    remainder.Add(entry, remainingValue);
                }
            }

            List<PopulationDataEntry> sortedRemainders = new List<PopulationDataEntry>();
            foreach (PopulationDataEntry entry in data.SubEntities)
            {
                if (entry != null)
                {
                    sortedRemainders.Add(entry);
                }
            }
            sortedRemainders.Sort(delegate(PopulationDataEntry p1, PopulationDataEntry p2) { return remainder[p2].CompareTo(remainder[p1]); });

            while (remainingSeat > 0)
            {
                PopulationDataEntry first = sortedRemainders.First();
                result[first] += 1;
                remainingSeat -= 1;
                sortedRemainders.Remove(first);
            }

            return result;
        }
        #endregion
    }
}
