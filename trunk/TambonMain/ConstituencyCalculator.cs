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
        {
        }

        #endregion constructor

        #region methods

        public static Dictionary<Entity, Int32> Calculate(UInt32 parentGeocode, Int32 year, Int32 numberOfSeats)
        {
            Dictionary<Entity, Int32> result = null;

            var downloader = new PopulationDataDownloader(year, parentGeocode);
            downloader.Process();
            result = Calculate(downloader.Data, year, numberOfSeats);
            return result;
        }

        public static Dictionary<Entity, Int32> Calculate(Entity data, Int32 year, Int32 numberOfSeats)
        {
            if ( data == null )
            {
                throw new ArgumentNullException("data");
            }

            var result = new Dictionary<Entity, Int32>();

            Int32 totalPopulation = 0;
            foreach ( var entry in data.entity )
            {
                if ( entry != null )
                {
                    result.Add(entry, 0);
                    if ( entry.population.Any() )
                    {
                        totalPopulation += entry.population.First().TotalPopulation.total;
                    }
                }
            }
            Double divisor = (1.0 * numberOfSeats) / (1.0 * totalPopulation);
            Int32 remainingSeat = numberOfSeats;
            var remainder = new Dictionary<Entity, Double>();
            foreach ( var entry in data.entity )
            {
                if ( entry != null )
                {
                    if ( entry.population.Any() )
                    {
                        Double seats = entry.population.First().TotalPopulation.total * divisor;
                        Int32 actualSeats = Math.Max(1, Convert.ToInt32(Math.Truncate(seats)));
                        result[entry] = actualSeats;
                        remainingSeat -= actualSeats;
                        Double remainingValue = seats - actualSeats;
                        remainder.Add(entry, remainingValue);
                    }
                }
            }

            var sortedRemainders = new List<Entity>();
            foreach ( var entry in data.entity )
            {
                if ( entry != null )
                {
                    sortedRemainders.Add(entry);
                }
            }
            sortedRemainders.Sort(delegate(Entity p1, Entity p2)
            {
                return remainder[p2].CompareTo(remainder[p1]);
            });

            while ( remainingSeat > 0 )
            {
                Entity first = sortedRemainders.First();
                result[first] += 1;
                remainingSeat -= 1;
                sortedRemainders.Remove(first);
            }

            return result;
        }

        #endregion methods
    }
}