using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class HistoryEntryBase
    {
        /// <summary>
        /// Adds the given gazette as a reference.
        /// </summary>
        /// <param name="gazette">Gazette entry to add.</param>
        public void AddGazetteReference(GazetteEntry gazette)
        {
            if ( gazette == null )
            {
                throw new ArgumentNullException("gazette");
            }

            if ( gazette.effectiveSpecified )
            {
                effective = gazette.effective;
                effectiveSpecified = true;
            }
            if ( gazette.effectiveafterSpecified )
            {
                effective = gazette.publication + new TimeSpan(gazette.effectiveafter, 0, 0, 0);
                effectiveSpecified = true;
            }
            if ( !effectiveSpecified )
            {
                // wild guess - using publication date as effective date
                effective = gazette.publication;
                effectiveSpecified = true;
            }
            status = ChangeStatus.Gazette;
            Items.Add(new GazetteRelated(gazette)
            {
                relation = GazetteRelation.Unknown
            });
        }
    }
}