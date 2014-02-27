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
            if (!data.Any(x => x.type == PopulationDataType.total))
            {
                var result = new HouseholdDataPoint();
                result.type = PopulationDataType.total;
                foreach (var element in this.data.Where(x => x.type == PopulationDataType.municipal || x.type == PopulationDataType.nonmunicipal))
                {
                    result.female += element.female;
                    result.male += element.male;
                    result.total += element.total;
                    result.households += element.households;
                }
                if (result.total > 0)
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

        public Boolean Verify()
        {
            Boolean result = true;

            foreach (var entry in data)
            {
                result &= entry.Verify();
            }

            var municipal = data.FirstOrDefault(x => x.type == PopulationDataType.municipal);
            var rural = data.FirstOrDefault(x => x.type == PopulationDataType.nonmunicipal);
            if ((municipal != null) && (rural != null))
            { result &= TotalPopulation.VerifySum(municipal, rural); }
            var collectivehouseholds = data.FirstOrDefault(x => x.type == PopulationDataType.collectivehouseholds);
            var privatehouseholds = data.FirstOrDefault(x => x.type == PopulationDataType.privatehouseholds);
            if ((collectivehouseholds != null) && (privatehouseholds != null))
            { result &= TotalPopulation.VerifySum(collectivehouseholds, privatehouseholds); }
            var agricultural = data.FirstOrDefault(x => x.type == PopulationDataType.agricultural);
            var nonagricultural = data.FirstOrDefault(x => x.type == PopulationDataType.nonagricultural);
            if ((agricultural != null) && (nonagricultural != null))
            { result &= TotalPopulation.VerifySum(agricultural, nonagricultural); }
            return result;
        }
    }

    public partial class PopulationDataPoint
    {
        private void Add(PopulationDataPoint data)
        {
            this.female += data.female;
            this.male += data.male;
            this.total += data.total;
        }
        private Boolean IsEqual(PopulationDataPoint data)
        {
            return
            (this.female == data.female) &&
            (this.male == data.male) &&
            (this.total == data.total);
        }
        public  Boolean Verify()
        {
            var sum = male + female;
            return (sum == total);
        }

        public Boolean VerifySum(PopulationDataPoint data1, PopulationDataPoint data2)
        {
            var sum = new PopulationDataPoint();
            if (data1 != null)
            {
                sum.Add(data1);
            }
            if (data2 != null)
            {
                sum.Add(data2);
            }
            return this.IsEqual(sum);
        }
    }
}