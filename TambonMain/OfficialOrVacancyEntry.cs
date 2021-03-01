using System;

namespace De.AHoerstemeier.Tambon
{
    public partial class OfficialOrVacancyEntry
    {
        /// <summary>
        /// Gets a timestamp for the current entry.
        /// </summary>
        public DateTime TimeStamp
        {
            get { return GetTimeStamp(); }
        }

        /// <summary>
        /// Calculates a timestamp for the current term or vacancy.
        /// </summary>
        /// <returns>Timestamp.</returns>

        protected virtual DateTime GetTimeStamp()
        {
            var result = DateTime.MinValue;
            if (beginSpecified)
            {
                result = begin;
            }
            else if (!String.IsNullOrEmpty(beginyear))
            {
                result = new DateTime(Convert.ToInt32(beginyear), 1, 1);
            }
            else if (endSpecified)
            {
                result = end;
            }
            else if (!String.IsNullOrEmpty(endyear))
            {
                result = new DateTime(Convert.ToInt32(endyear), 1, 1);
            }
            return result;
        }
    }
}
