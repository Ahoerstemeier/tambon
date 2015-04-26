using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class GazetteRename
    {
        /// <summary>
        /// Converts the gazette operation into a entity history entry.
        /// </summary>
        /// <returns>Coresponding history entry.</returns>
        public override HistoryEntryBase ConvertToHistory()
        {
            var historyRename = new HistoryRename();
            historyRename.oldname = this.oldname;
            historyRename.oldenglish = this.oldenglish;
            historyRename.name = this.name;
            historyRename.english = this.english;
            return historyRename;
        }
    }
}