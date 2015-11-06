using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class GazetteCreate
    {
        /// <summary>
        /// Gets the geocodes of the entities from which the entity was split off.
        /// </summary>
        /// <returns>List of geocodes.</returns>
        public IEnumerable<UInt32> SplitFrom()
        {
            var parents = new List<UInt32>();
            foreach ( var item in Items )
            {
                if ( parentSpecified )
                {
                    parents.Add(parent);
                }
                var itemReassign = item as GazetteReassign;
                if ( itemReassign != null )
                {
                    if ( itemReassign.oldgeocodeSpecified )
                    {
                        parents.Add(itemReassign.oldgeocode / 100);
                    }
                    if ( itemReassign.oldownerSpecified )
                    {
                        parents.Add(itemReassign.oldowner);
                    }
                }
                var itemAreaChange = item as GazetteAreaChange;
                if ( itemAreaChange != null )
                {
                    if ( itemAreaChange.geocodeSpecified )
                    {
                        parents.Add(itemAreaChange.geocode);
                    }
                }
            }
            return parents.Distinct();
        }

        /// <summary>
        /// Converts the gazette operation into a entity history entry.
        /// </summary>
        /// <returns>Coresponding history entry.</returns>
        public override HistoryEntryBase ConvertToHistory()
        {
            var historyCreate = new HistoryCreate();
            historyCreate.splitfrom.AddRange(this.SplitFrom());
            historyCreate.type = this.type;
            Int32 subdivisions = 0;
            foreach ( var item in Items )
            {
                var itemReassign = item as GazetteReassign;
                if ( itemReassign != null )
                {
                    subdivisions++;
                    if ( itemReassign.typeSpecified )
                    {
                        historyCreate.subdivisiontype = itemReassign.type;
                        historyCreate.subdivisiontypeSpecified = true;
                    }
                }
            }
            if ( subdivisions > 0 )
            {
                historyCreate.subdivisions = subdivisions;
                historyCreate.subdivisionsSpecified = true;
            }
            return historyCreate;
        }
    }
}