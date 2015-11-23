using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class GazetteParentChange
    {
        /// <summary>
        /// Converts the gazette operation into a entity history entry.
        /// </summary>
        /// <returns>Corresponding history entry.</returns>
        public override HistoryEntryBase ConvertToHistory()
        {
            var historyReassign = new HistoryParentChange();
            historyReassign.newparent = this.newparent;
            historyReassign.oldparent = this.oldparent;
            if ( this.typeSpecified )
            {
                historyReassign.type = this.type;
                historyReassign.typeSpecified = true;
            }
            return historyReassign;
        }
    }
}