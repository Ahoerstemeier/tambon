using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.AHoerstemeier.Tambon
{
    public partial class PopulationData
    {
        public PopulationData Clone()
        {
            return (PopulationData)(this.MemberwiseClone());
        }

        public PopulationDataPoint TotalPopulation
        {
            get
            {
                var result = new PopulationDataPoint();
                foreach ( var element in this.data )
                {
                    if ( element.female > 0 )
                        result.female = result.female + element.female;
                    if ( element.male > 0 )
                        result.male = result.male + element.male;
                    if ( element.total > 0 )
                        result.total = result.total + element.total;
                }
                return result;
            }
        }

        public void MergeIdenticalEntries()
        {
            // ToDo: Find duplicate entries with same data.type and merge content
        }

        public Int16 Year
        {
            get
            {
                return Convert.ToInt16(year, CultureInfo.InvariantCulture);
            }
        }
    }
}