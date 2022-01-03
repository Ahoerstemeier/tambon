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
            var councilTerm = x as CouncilTermOrVacancy;
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
        /// Gets an ordered enumeration of council terms.
        /// </summary>
        /// <value>An enumeration of council terms.</value>
        public IEnumerable<CouncilTerm> CouncilTerms
        {
            get
            {
                var result = new List<CouncilTerm>();
                foreach ( var item in Items )
                {
                    if (item is CouncilTerm term)
                    {
                        result.Add(term);
                    }
                }
                return result.OrderByDescending(x => x.begin);
            }
        }

        /// <summary>
        /// Gets an ordered enumeration of council terms and vacancies.
        /// </summary>
        /// <value>An enumeration of council terms and vacancies.</value>
        public IEnumerable<CouncilTermOrVacancy> CouncilTermsOrVacancies
        {
            get
            {
                var result = new List<CouncilTermOrVacancy>();
                foreach (var item in Items)
                {
                    if (item is CouncilTermOrVacancy term)
                    {
                        result.Add(term);
                    }
                }
                return result.OrderByDescending(x => x.begin);
            }
        }
    }
}