using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class HouseholdDataPoint
    {
        #region fixup serialization

        public bool ShouldSerializeagetable()
        {
            return agetable.age.Any();
        }

        #endregion fixup serialization

        /// <summary>
        /// Creates a new <see cref="HouseholdDataPoint"/> containing all values from <paramref name="dataPoint"/>.
        /// </summary>
        /// <param name="dataPoint">Data to copy.</param>
        public HouseholdDataPoint(HouseholdDataPoint dataPoint)
        {
            if ( dataPoint == null )
            {
                throw new ArgumentNullException("dataPoint");
            }
            this.male = dataPoint.male;
            this.female = dataPoint.female;
            this.households = dataPoint.households;
            this.total = dataPoint.total;
            this.geocode = dataPoint.geocode;
            this.type = dataPoint.type;
        }
    }
}