using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class GazetteAreaChange
    {
        /// <summary>
        /// Converts the gazette operation into a entity history entry.
        /// </summary>
        /// <returns>Coresponding history entry.</returns>
        public override HistoryEntryBase ConvertToHistory()
        {
            var historyArea = new HistoryAreaChange();
            if ( this.oldareaSpecified )
            {
                historyArea.oldarea = this.oldarea;
                historyArea.oldareaSpecified = true;
            }
            if ( this.newareaSpecified )
            {
                historyArea.newarea = this.newarea;
                historyArea.newareaSpecified = true;
            }
            if ( this.typeSpecified )
            {
                historyArea.type = this.type;
                historyArea.typeSpecified = true;
            }
            return historyArea;
        }
    }
}