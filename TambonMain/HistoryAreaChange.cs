using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class HistoryAreaChange
    {
        #region fixup serialization

        public Boolean ShouldSerializechangewith()
        {
            return changewith.Any();
        }

        #endregion fixup serialization
    }
}