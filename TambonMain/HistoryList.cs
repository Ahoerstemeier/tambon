using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class HistoryList
    {
        /// <summary>
        /// Checks whether the history list indicates that the entity was of the given type at the given date.
        /// </summary>
        /// <param name="entityType">Entity type to check.</param>
        /// <param name="currentType">Entity type the entity has today.</param>
        /// <param name="dateToCheck">Date to check.</param>
        /// <returns><c>true</c> if history list confirms entity type, or history list empty and current type fits, <c>false</c> otherwise.</returns>
        public Boolean CheckTypeAtDate(EntityType entityType, EntityType currentType, DateTime dateToCheck)
        {
            if ( !Items.Any() )
            {
                // no history, so cannot probably already existing long before. Can only check if type fits.
                return entityType == currentType;
            }
            else
            {
                // ThenBy for the 2 special cases where Tambon was abolished and created again on same day
                var historiesToCheck = Items.Where(x => x.effectiveSpecified && x.effective < dateToCheck).OrderBy(y => y.effective).ThenBy(z => z is HistoryCreate);
                var calculatedType = currentType;
                if ( Items.Any(x => x is HistoryStatus) )
                {
                    calculatedType = (Items.OrderBy(x => x.effective).First(y => y is HistoryStatus) as HistoryStatus).old;
                }
                if ( Items.Any(x => x is HistoryCreate) )
                {
                    calculatedType = EntityType.Unknown;
                }
                foreach ( var history in historiesToCheck )
                {
                    var creation = history as HistoryCreate;
                    var changeType = history as HistoryStatus;
                    var abolish = history as HistoryAbolish;
                    if ( abolish != null )
                    {
                        calculatedType = EntityType.Unknown;
                    }
                    else if ( changeType != null )
                    {
                        calculatedType = changeType.@new;
                    }
                    else if ( creation != null )
                    {
                        calculatedType = creation.type;
                    }
                }
                return entityType == calculatedType;
            }
        }
    }
}