using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.AHoerstemeier.Tambon
{
    public partial class GazetteList : IGazetteEntries
    {
        public IEnumerable<GazetteEntry> AllGazetteEntries
        {
            get
            {
                return entry;
            }
        }

        public IEnumerable<GazetteEntry> AllAboutGeocode(UInt32 geocode, Boolean includeSubEnties)
        {
            var result = new List<GazetteEntry>();
            result.AddRange(this.entry.Where(x => x.IsAboutGeocode(geocode, includeSubEnties)));
            return result;
        }
    }

    public partial class GazetteListMonth : IGazetteEntries
    {
    }
}