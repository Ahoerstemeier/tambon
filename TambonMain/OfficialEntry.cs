using System;

namespace De.AHoerstemeier.Tambon
{
    public partial class OfficialEntry
    {
        /// <inheritdoc/>
        protected override DateTime GetTimeStamp()
        {
            var result = base.GetTimeStamp();
            if (result == DateTime.MinValue)
            {
                if (!String.IsNullOrEmpty(inoffice))
                {
                    result = new DateTime(Convert.ToInt32(inoffice), 1, 1);
                }
            }
            return result;
        }
    }
}
