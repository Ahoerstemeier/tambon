﻿using System;

namespace De.AHoerstemeier.Tambon
{
    public partial class OtherIdentifier :IIsEmpty
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

        public Boolean ShouldSerializegeonames()
        {
            return !String.IsNullOrEmpty(geonames.value);
        }

        public Boolean ShouldSerializegetty()
        {
            return !String.IsNullOrEmpty(getty.value);
        }

        public Boolean ShouldSerializegoogleplace()
        {
            return !String.IsNullOrEmpty(googleplace.value);
        }

        public Boolean ShouldSerializegadm()
        {
            return !String.IsNullOrEmpty(gadm.value);
        }

        #endregion fixup serialization

        /// <inheritdoc/>
        public Boolean IsEmpty()
        {
            return !(ShouldSerializefips10() || ShouldSerializegnd() || ShouldSerializehasc() || ShouldSerializeiso3166() || ShouldSerializesalb() || ShouldSerializegeonames() || ShouldSerializegadm() || ShouldSerializegoogleplace() || ShouldSerializegetty() || ShouldSerializewoeid());
        }
    }
}