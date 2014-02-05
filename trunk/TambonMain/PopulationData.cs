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

        public void CalculateTotal()
        {
            if ( !data.Any(x => x.type == PopulationDataType.total) )
            {
                var result = new HouseholdDataPoint();
                result.type = PopulationDataType.total;
                foreach ( var element in this.data.Where(x => x.type == PopulationDataType.municipal || x.type == PopulationDataType.nonmunicipal) )
                {
                    result.female += element.female;
                    result.male += element.male;
                    result.total += element.total;
                    result.households += element.households;
                }
                if ( result.total > 0 )
                {
                    data.Add(result);
                }
            }
        }

        public PopulationDataPoint TotalPopulation
        {
            get
            {
                var result = data.FirstOrDefault(x => x.type == PopulationDataType.total);
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