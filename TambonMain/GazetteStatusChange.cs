using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class GazetteStatusChange
    {
        /// <summary>
        /// Converts the gazette operation into a entity history entry.
        /// </summary>
        /// <returns>Corresponding history entry.</returns>
        public override HistoryEntryBase ConvertToHistory()
        {
            if ( this.old == EntityType.SaphaTambon && this.@new == EntityType.TAO )
            {
                var historyCreate = new HistoryCreate();
                historyCreate.type = this.@new;
                return historyCreate;
            }
            else
            {
                var historyStatus = new HistoryStatus();
                historyStatus.old = this.old;
                historyStatus.@new = this.@new;
                return historyStatus;
            }
        }
    }
}