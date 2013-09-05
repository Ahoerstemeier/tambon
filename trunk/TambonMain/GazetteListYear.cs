using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.AHoerstemeier.Tambon
{
    public partial class GazetteListYear : IGazetteEntries
    {
        public IEnumerable<GazetteEntry> AllGazetteEntries
        {
            get
            {
                var result = new List<GazetteEntry>();
                result.AddRange(this.entry);
                foreach ( var monthList in this.month )
                {
                    result.AddRange(monthList.AllGazetteEntries);
                }
                return result;
            }
        }

        public IEnumerable<GazetteEntry> AllAboutGeocode(UInt32 geocode, Boolean includeSubEnties)
        {
            var result = new List<GazetteEntry>();
            result.AddRange(this.entry.Where(x => x.IsAboutGeocode(geocode, includeSubEnties)));
            foreach ( var monthList in this.month )
            {
                result.AddRange(monthList.AllAboutGeocode(geocode, includeSubEnties));
            }
            return result;
        }
    }
}