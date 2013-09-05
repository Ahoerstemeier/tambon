using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace De.AHoerstemeier.Tambon
{
    public partial class CouncilTerm
    {
        /// <summary>
        /// Gets or sets the size of the council.
        /// </summary>
        /// <value>The size of the council.</value>
        [XmlIgnore()]
        public UInt32 Size
        {
            get
            {
                return Convert.ToUInt32(size, CultureInfo.InvariantCulture);
            }
            set
            {
                if ( value < 0 )
                {
                    throw new ArgumentException("Size must be larger than 0");
                }
                size = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets or sets the size of the council at the end of its term.
        /// </summary>
        /// <value>The size of the council at the end of its term.</value>
        /// <remarks>If <see cref="finalsize"/> is not set, <see cref="Size"/> is returned.</remarks>
        [XmlIgnore()]
        public UInt32 FinalSize
        {
            get
            {
                if ( String.IsNullOrEmpty(finalsize) )
                {
                    return Size;
                }
                else
                {
                    return Convert.ToUInt32(finalsize, CultureInfo.InvariantCulture);
                }
            }
            set
            {
                if ( value < 0 )
                {
                    throw new ArgumentException("Size must be larger than 0");
                }
                finalsize = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Whether the <see cref="size"/> of the council fits with the council <see cref="type"/>.
        /// </summary>
        [XmlIgnore()]
        public Boolean CouncilSizeValid
        {
            get
            {
                return type.IsValidCouncilSize(Size);
            }
        }

        /// <summary>
        /// Whether the <see cref="begin"/> and <see cref="end"/> dates of the term are sensible, i.e. end after begin.
        /// </summary>
        [XmlIgnore()]
        public Boolean TermDatesValid
        {
            get
            {
                var result = (begin.Year > 1900) & (begin.Year < GlobalData.MaximumPossibleElectionYear);
                if ( endFieldSpecified )
                {
                    result &= (end.Year > 1900) & (end.Year < GlobalData.MaximumPossibleElectionYear);
                    result &= end.CompareTo(begin) > 0;
                }
                return result;
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
            if ( endFieldSpecified )
            {
                var expectedEndTerm = begin.AddYears(maximumTermLength);
                var compare = expectedEndTerm.CompareTo(end);
                result = compare > 0;
            }
            return result;
        }
    }
}