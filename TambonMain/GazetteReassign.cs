using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class GazetteReassign
    {
        /// <summary>
        /// Converts the gazette operation into a entity history entry.
        /// </summary>
        /// <returns>Coresponding history entry.</returns>
        public override HistoryEntryBase ConvertToHistory()
        {
            var historyReassign = new HistoryReassign();
            if ( this.oldgeocodeSpecified )
            {
                historyReassign.oldgeocode = this.oldgeocode;
                historyReassign.oldgeocodeSpecified = true;
            }
            if ( this.newownerSpecified )
            {
                historyReassign.newparent = this.newowner;
            }
            else if ( this.geocodeSpecified )
            {
                historyReassign.newparent = this.geocode / 100;
            }
            if ( this.oldownerSpecified )
            {
                historyReassign.oldparent = this.oldowner;
            }
            else
            {
                historyReassign.oldparent = this.oldgeocode / 100;
            }
            if ( this.typeSpecified )
            {
                historyReassign.type = this.type;
                historyReassign.typeSpecified = true;
            }
            return historyReassign;
        }
    }
}