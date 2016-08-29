using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class Office
    {
        /// <summary>
        /// Gets the best fit from the <see cref=" url">list of websites</see>.
        /// </summary>
        /// <value>The preferred website URL.</value>
        public String PreferredWebsite
        {
            get
            {
                var urls = url.Where(x => x.status == MyUriStatus.unknown || x.status == MyUriStatus.online || x.status == MyUriStatus.inaccessible);
                urls = urls.Where(x => x.lastcheckedSpecified && (DateTime.Now - x.lastchecked).Days < 365);
                if ( urls.Any() )
                {
                    return urls.FirstOrDefault().Value;
                }
                else
                {
                    return String.Empty;
                }
            }
        }
    }
}