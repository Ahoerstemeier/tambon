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
        #endregion
    }
}
