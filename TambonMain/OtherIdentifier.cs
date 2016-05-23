using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class OtherIdentifier
    {
        #region fixup serialization

        public Boolean ShouldSerializehasc()
        {
            return !String.IsNullOrEmpty(hasc.value);
        }

        public Boolean ShouldSerializeiso3166()
        {
            return !String.IsNullOrEmpty(iso3166.value);
        }

        public Boolean ShouldSerializefips10()
        {
            return !String.IsNullOrEmpty(fips10.value);
        }

        public Boolean ShouldSerializegnd()
        {
            return !String.IsNullOrEmpty(gnd.value);
        }

        public Boolean ShouldSerializesalb()
        {
            return !String.IsNullOrEmpty(salb.value);
        }

        public Boolean ShouldSerializewoeid()
        {
            return !String.IsNullOrEmpty(woeid.value);
        }

        public Boolean ShouldSerializegetty()
        {
            return !String.IsNullOrEmpty(getty.value);
        }

        #endregion fixup serialization

        public Boolean IsEmpty()
        {
            return !(ShouldSerializefips10() || ShouldSerializegnd() || ShouldSerializehasc() || ShouldSerializeiso3166() || ShouldSerializesalb());
        }
    }
}