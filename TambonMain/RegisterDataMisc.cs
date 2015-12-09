using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    partial class RegisterDataMisc
    {
        #region fixup serialization

        public bool IsEmpty()
        {
            return marriage == 0 && divorce == 0 && adoption == 0 && adoptiondissolution == 0 && familystatus == 0 && childacknowledge == 0;
        }

        #endregion fixup serialization
    }
}