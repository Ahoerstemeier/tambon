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
    }
}