using System;

namespace De.AHoerstemeier.Tambon
{
    public partial class OfficialVacancy
    {
        /// <inheritdoc/>
        protected override DateTime GetTimeStamp()
        {
            var result = base.GetTimeStamp();
            if (result == DateTime.MinValue)
            {
                if (!String.IsNullOrEmpty(year))
                {
                    result = new DateTime(Convert.ToInt32(year), 1, 1);
                }
            }
            return result;
        }
    }
}
