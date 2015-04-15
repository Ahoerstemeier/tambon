using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class HistoryCreate
    {
        #region fixup serialization

        public Boolean ShouldSerializesplitfrom()
        {
            return splitfrom.Any();
        }

        #endregion fixup serialization
    }
}