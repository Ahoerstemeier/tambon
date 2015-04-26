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
        /// <returns>Coresponding history entry.</returns>
        public override HistoryEntryBase ConvertToHistory()
        {
            var historyStatus = new HistoryStatus();
            historyStatus.old = this.old;
            historyStatus.@new = this.@new;
            return historyStatus;
        }
    }
}