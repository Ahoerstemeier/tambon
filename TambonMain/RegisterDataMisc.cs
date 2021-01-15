using System;


namespace De.AHoerstemeier.Tambon
{
    partial class RegisterDataMisc: IIsEmpty
    {
        #region fixup serialization

        /// <inheritdoc/>
        public bool IsEmpty()
        {
            return marriage == 0 && divorce == 0 && adoption == 0 && adoptiondissolution == 0 && familystatus == 0 && childacknowledge == 0;
        }

        #endregion fixup serialization
    }
}