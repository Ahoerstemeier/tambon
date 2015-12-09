using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    partial class RegisterData
    {
        #region fixup serialization

        public Boolean ShouldSerializeregister()
        {
            return !register.IsEmpty();
        }

        public Boolean ShouldSerializereference()
        {
            return reference.Any();
        }

        #endregion fixup serialization
    }
}