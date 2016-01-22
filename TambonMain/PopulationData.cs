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
        #region fixup serialization

        public bool ShouldSerializeregister()
        {
            return register.change.Any() || !register.register.IsEmpty();
        }

        public Boolean ShouldSerializereference()
        {
            return reference.Any();
        }

        #endregion fixup serialization

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

        /// <summary>
        /// Gets the year.
        /// </summary>
        /// <value>The year.</value>
        /// <remarks>Same as <see cref="year"/>, which has wrong data type as XSD2Code cannot translate the XSD type year better.</remarks>
        public Int16 Year
        {
            get
            {
                return Convert.ToInt16(year, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Checks whether the parts sum up correctly.
        /// </summary>
        /// <returns><c>true</c> if all parts sum up correctly, <c>false</c> otherwise.</returns>
        public Boolean Verify()
        {
            Boolean result = true;

            foreach ( var entry in data )
            {
                result &= entry.Verify();
            }

            var municipal = data.FirstOrDefault(x => x.type == PopulationDataType.municipal);
            var rural = data.FirstOrDefault(x => x.type == PopulationDataType.nonmunicipal);
            if ( (municipal != null) && (rural != null) )
            {
                result &= TotalPopulation.VerifySum(municipal, rural);
            }
            var collectivehouseholds = data.FirstOrDefault(x => x.type == PopulationDataType.collectivehouseholds);
            var privatehouseholds = data.FirstOrDefault(x => x.type == PopulationDataType.privatehouseholds);
            if ( (collectivehouseholds != null) && (privatehouseholds != null) )
            {
                result &= TotalPopulation.VerifySum(collectivehouseholds, privatehouseholds);
            }
            var sanitary = data.FirstOrDefault(x => x.type == PopulationDataType.sanitary);
            var urbanSanitary = data.FirstOrDefault(x => x.type == PopulationDataType.urbansanitary);
            var ruralSanitary = data.FirstOrDefault(x => x.type == PopulationDataType.ruralsanitary);
            if ( (urbanSanitary != null) && (ruralSanitary != null) && (sanitary != null) )
            {
                result &= sanitary.VerifySum(collectivehouseholds, privatehouseholds);
            }

            var agricultural = data.FirstOrDefault(x => x.type == PopulationDataType.agricultural);
            var nonagricultural = data.FirstOrDefault(x => x.type == PopulationDataType.nonagricultural);
            if ( (agricultural != null) && (nonagricultural != null) )
            {
                result &= TotalPopulation.VerifySum(agricultural, nonagricultural);
            }
            return result;
        }

        /// <summary>
        /// Calculates the maximum deviation of the sum of the partial data point.
        /// </summary>
        /// <returns>Maximum deviation, <c>0</c> if all sum up correctly.</returns>
        public Int32 SumError()
        {
            Int32 maxError = 0;
            // DOPA data can contain more than one municipal entry with different geocodes
            PopulationDataPoint municipal = null;
            var municipalData = data.Where(x => x.type == PopulationDataType.municipal);
            if ( municipalData.Any() )
            {
                municipal = new PopulationDataPoint();
                foreach ( var dataPoint in municipalData )
                {
                    municipal.Add(dataPoint);
                }
            }
            // var municipal = data.FirstOrDefault(x => x.type == PopulationDataType.municipal);
            var rural = data.FirstOrDefault(x => x.type == PopulationDataType.nonmunicipal);
            if ( (municipal != null) && (rural != null) )
            {
                maxError = Math.Max(maxError, TotalPopulation.SumError(municipal, rural));
            }
            var collectivehouseholds = data.FirstOrDefault(x => x.type == PopulationDataType.collectivehouseholds);
            var privatehouseholds = data.FirstOrDefault(x => x.type == PopulationDataType.privatehouseholds);
            if ( (collectivehouseholds != null) && (privatehouseholds != null) )
            {
                maxError = Math.Max(maxError, TotalPopulation.SumError(collectivehouseholds, privatehouseholds));
            }
            var agricultural = data.FirstOrDefault(x => x.type == PopulationDataType.agricultural);
            var nonagricultural = data.FirstOrDefault(x => x.type == PopulationDataType.nonagricultural);
            if ( (agricultural != null) && (nonagricultural != null) )
            {
                maxError = Math.Max(maxError, TotalPopulation.SumError(agricultural, nonagricultural));
            }
            var sanitary = data.FirstOrDefault(x => x.type == PopulationDataType.sanitary);
            var urbanSanitary = data.FirstOrDefault(x => x.type == PopulationDataType.urbansanitary);
            var ruralSanitary = data.FirstOrDefault(x => x.type == PopulationDataType.ruralsanitary);
            if ( (urbanSanitary != null) && (ruralSanitary != null) && (sanitary != null) )
            {
                maxError = Math.Max(maxError, sanitary.SumError(urbanSanitary, ruralSanitary));
            }
            return maxError;
        }

        /// <summary>
        /// Adds the numbers of the data point to this.
        /// </summary>
        /// <param name="dataPoint">Data point to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dataPoint"/> is <c>null</c>.</exception>
        public void AddDataPoint(HouseholdDataPoint dataPoint)
        {
            if ( dataPoint == null )
            {
                throw new ArgumentNullException("dataPoint");
            }
            var target = data.FirstOrDefault(x => x.type == dataPoint.type);
            if ( target == null )
            {
                target = new HouseholdDataPoint();
                target.type = dataPoint.type;
                data.Add(target);
            }
            target.total += dataPoint.total;
            target.male += dataPoint.male;
            target.female += dataPoint.female;
            target.households += dataPoint.households;
        }
    }

    public partial class PopulationDataPoint
    {
        /// <summary>
        /// Adds the values from <paramref name="data"/> to self.
        /// </summary>
        /// <param name="data">Data to add.</param>
        public void Add(PopulationDataPoint data)
        {
            if ( data == null )
            {
                throw new ArgumentNullException("data");
            }
            this.female += data.female;
            this.male += data.male;
            this.total += data.total;
        }

        public Boolean IsEqual(PopulationDataPoint data)
        {
            if ( data == null )
            {
                throw new ArgumentNullException("data");
            }

            return
                (this.female == data.female) &&
                (this.male == data.male) &&
                (this.total == data.total);
        }

        /// <summary>
        /// Verifies if <see cref="male"/> and <see cref="female"/> sum up to <see cref="total"/>.
        /// </summary>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        public virtual Boolean Verify()
        {
            var sum = male + female;
            return (sum == total);
        }

        /// <summary>
        /// Deviation of sum <see cref="male"/> and <see cref="female"/> with <see cref="total"/>.
        /// </summary>
        /// <returns>Deviation of sum, <c>0</c> is sum is valid.</returns>
        public Int32 SumError()
        {
            var sum = male + female;
            return Math.Abs(sum - total);
        }

        /// <summary>
        /// Checks whether the two population data added together are equal with this.
        /// </summary>
        /// <param name="data1">First data point.</param>
        /// <param name="data2">Second data point.</param>
        /// <returns>Maximum deviation in one of the sum, <c>0</c> if equal.</returns>
        public Int32 SumError(PopulationDataPoint data1, PopulationDataPoint data2)
        {
            var sum = new PopulationDataPoint();
            if ( data1 != null )
            {
                sum.Add(data1);
            }
            if ( data2 != null )
            {
                sum.Add(data2);
            }
            return this.MaxDeviation(sum);
        }

        /// <summary>
        /// Checks the difference between this and <paramref name="compare"/>.
        /// </summary>
        /// <param name="compare">Data to compare with.</param>
        /// <returns>Maximum deviation between the two data points.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="compare"/> is <c>null</c>.</exception>
        public Int32 MaxDeviation(PopulationDataPoint compare)
        {
            if ( compare == null )
            {
                throw new ArgumentNullException("compare");
            }
            Int32 maleError = Math.Abs(this.male - compare.male);
            if ( (this.male == 0) || (compare.male == 0) )
            {
                maleError = 0;
            }
            Int32 femaleError = Math.Abs(this.female - compare.female);
            if ( (this.female == 0) || (compare.female == 0) )
            {
                femaleError = 0;
            }
            Int32 totalError = Math.Abs(this.total - compare.total);
            if ( (this.total == 0) || (compare.total == 0) )
            {
                totalError = 0;
            }
            return Math.Max(Math.Max(maleError, femaleError), totalError);
        }

        /// <summary>
        /// Checks whether the two population data added together are equal with this.
        /// </summary>
        /// <param name="data1">First data point.</param>
        /// <param name="data2">Second data point.</param>
        /// <returns><c>true</c> if equal, <c>false</c> otherwise.</returns>
        public Boolean VerifySum(PopulationDataPoint data1, PopulationDataPoint data2)
        {
            return this.SumError(data1, data2) == 0;
        }
    }

    public partial class HouseholdDataPoint
    {
        /// <summary>
        /// Verifies if <see cref="male"/> and <see cref="female"/> sum up to <see cref="total"/>,
        /// and <see cref="agetable"/> is valid if present.
        /// </summary>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        public override Boolean Verify()
        {
            var result = base.Verify();
            result &= AgeTableValid();
            return result;
        }

        /// <summary>
        /// Checks whether the <see cref="agetable"/> fits to self.
        /// </summary>
        /// <returns><c>true</c> if age table if valid, <c>false</c> otherwise.</returns>
        /// <remarks>Also returns <c>true</c> if there is no age table.</remarks>
        public Boolean AgeTableValid()
        {
            var result = true;
            if ( agetable != null && agetable.age.Any() )
            {
                var ageSum = new PopulationDataPoint();
                foreach ( var ageEntry in agetable.age )
                {
                    ageSum.Add(ageEntry);
                }
                ageSum.Add(agetable.unknown);
                result &= ageSum.IsEqual(this);
            }

            return result;
        }
    }
}