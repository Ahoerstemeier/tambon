using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    partial class CouncilList
    {
        private DateTime GetDateFromItem(Object x)
        {
            var councilTerm = x as CouncilTerm;
            if ( councilTerm != null )
            {
                return councilTerm.begin;
            }
            var electionDate = x as CanceledElection;
            if ( electionDate != null )
            {
                return electionDate.date;
            }
            return new DateTime(1900, 1, 1);
        }

        /// <summary>
        /// Sorts the items in the list by date.
        /// </summary>
        public void SortByDate()
        {
            Items.Sort((x, y) => GetDateFromItem(x).CompareTo(GetDateFromItem(y)));
        }

        /// <summary>
        /// Gets an enumeration of council terms.
        /// </summary>
        /// <value>An enumeration of council terms.</value>
        public IEnumerable<CouncilTerm> CouncilTerms
        {
            get
            {
                var result = new List<CouncilTerm>();
                foreach ( var item in Items )
                {
                    var term = item as CouncilTerm;
                    if ( term != null )
                    {
                        result.Add(term);
                    }
                }
                return result;
            }
        }
    }
}