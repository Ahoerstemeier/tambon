using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace De.AHoerstemeier.Tambon
{
    public partial class OfficialEntryBase
    {
        private const Int32 FirstYearWithElectedLocalOfficeHolder = 2003;

        /// <summary>
        /// Whether the <see cref="begin"/> and <see cref="end"/> dates of the term are sensible, i.e. end after begin.
        /// </summary>
        [XmlIgnore()]
        public Boolean TermDatesValid
        {
            get
            {
                var result =
                    (BeginYear < GlobalData.MaximumPossibleElectionYear) &
                    (EndYear < GlobalData.MaximumPossibleElectionYear);
                if ( EndYear > 0 )
                {
                    result = BeginYear <= EndYear;
                }

                if ( endFieldSpecified & beginFieldSpecified )
                    return end.CompareTo(begin) > 0;
                else
                    return true;
            }
        }

        /// <summary>
        /// Gets the year of term begin, either from <see cref="begin"/> or <see cref="beginyear"/>.
        /// </summary>
        public Int32 BeginYear
        {
            get
            {
                var beginYear = 0;
                if ( beginFieldSpecified )
                {
                    beginYear = begin.Year;
                }
                else if ( !String.IsNullOrEmpty(beginyear) )
                {
                    beginYear = Convert.ToInt32(beginyear);
                }
                return beginYear;
            }
        }

        /// <summary>
        /// Gets the year of term end, either from <see cref="end"/> or <see cref="endyear"/>.
        /// </summary>
        public Int32 EndYear
        {
            get
            {
                var endYear = 0;
                if ( endFieldSpecified )
                {
                    endYear = end.Year;
                }
                else if ( !String.IsNullOrEmpty(endyear) )
                {
                    endYear = Convert.ToInt32(endyear);
                }
                return endYear;
            }
        }

        /// <summary>
        /// Calculates whether the <see cref="begin"/> and <see cref="end"/> dates fit with the <paramref name="maximumTermLength"/> in years.
        /// </summary>
        /// <param name="maximumTermLength">Maximum length of term in years.</param>
        /// <returns><c>true</c> if term length is correct, <c>false</c> otherwise.</returns>
        public Boolean TermLengthValid(Byte maximumTermLength)
        {
            Boolean result = true;

            if ( endFieldSpecified & beginFieldSpecified )
            {
                if ( BeginYear > FirstYearWithElectedLocalOfficeHolder )
                {
                    var expectedEndTerm = begin.AddYears(maximumTermLength).AddDays(-1);
                    var compare = expectedEndTerm.CompareTo(end);
                    result = compare >= 0;

                    if ( endreason == OfficialEndType.EndOfTerm )
                        result = (compare == 0);
                }
            }
            return result;
        }

        /// <summary>
        /// Checks whether the official is still in office.
        /// </summary>
        /// <returns><c>true</c> if (presumably) still in office, <c>false</c> otherwise.</returns>
        public Boolean InOffice()
        {
            return (!endSpecified) && String.IsNullOrEmpty(endyear) && (endreason == OfficialEndType.Unknown);
        }
    }
}