using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class AgeTable
    {
        #region fixup serialization

        public Boolean ShouldSerializeunknown()
        {
            return unknown.female != 0 || unknown.male != 0 || unknown.total != 0;
        }

        #endregion fixup serialization
    }
}