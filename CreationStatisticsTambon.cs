using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    class CreationStatisticsTambon : CreationStatisticsCentralGovernment
    {
        #region constructor
        public CreationStatisticsTambon()
        {
            StartYear = 1883;
            EndYear = DateTime.Now.Year;
        }
        public CreationStatisticsTambon(Int32 iStartYear, Int32 iEndYear)
        {
            StartYear = iStartYear;
            EndYear = iEndYear;
        }
        #endregion
        #region methods
        protected override Boolean EntityFitting(EntityType iEntityType)
        {
            Boolean result = (iEntityType == EntityType.Tambon);
            return result;
        }
        public override String Information()
        {
            StringBuilder lBuilder = new StringBuilder();
            AppendBasicInfo(lBuilder, "Tambon");
            AppendChangwatInfo(lBuilder, "Tambon");
            AppendSubEntitiesInfo(lBuilder, "Muban");
            AppendParentNumberInfo(lBuilder, "Tambon");
            
            String retval = lBuilder.ToString();
            return retval;
        }
        #endregion
    }
}