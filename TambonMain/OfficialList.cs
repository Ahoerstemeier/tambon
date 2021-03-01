using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    partial class OfficialList
    {
        private DateTime GetDateFromItem(Object x)
        {
            var term = x as OfficialEntryBase;
            if ( term != null )
            {
                return term.begin;
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
        /// Gets an ordered enumeration of official terms.
        /// </summary>
        /// <value>An enumeration of official terms.</value>
        public IEnumerable<OfficialEntryBase> OfficialTerms
        {
            get
            {
                var result = new List<OfficialEntryBase>();
                foreach ( var item in Items )
                {
                    var term = item as OfficialEntryBase;
                    if ( term != null )
                    {
                        result.Add(term);
                    }
                }
                return result.OrderByDescending(x => x.TimeStamp); 
            }
        }

        /// <summary>
        /// Gets an ordered enumeration of official terms and vacancies.
        /// </summary>
        /// <value>An enumeration of official terms and vacancies.</value>
        public IEnumerable<OfficialOrVacancyEntry> OfficialTermsOrVacancies
        {
            get
            {
                var result = new List<OfficialOrVacancyEntry>();
                foreach (var item in Items)
                {
                    var term = item as OfficialOrVacancyEntry;
                    if (term != null)
                    {
                        result.Add(term);
                    }
                }
                return result.OrderByDescending(x => x.TimeStamp);
            }
        }

        
    }
}