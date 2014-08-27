using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    partial class GazetteRelated
    {
        /// <summary>
        /// Creates a new instance of <see cref="GazetteRelated"/> linking an Gazette entry.
        /// </summary>
        /// <param name="entry">Gazette entry to which the link should be made.</param>
        public GazetteRelated(GazetteEntry entry)
        {
            if ( entry == null )
            {
                throw new ArgumentNullException("entry");
            }

            this.relationField = GazetteRelation.Mention;
            this.date = entry.publication;
            this.dateSpecified = true;
            this.issue = entry.issue;
            this.page = entry.FirstPage;
            this.volume = entry.volume;
        }
    }
}