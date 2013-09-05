using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.AHoerstemeier.Tambon
{
    public partial class GazetteListFull : IGazetteEntries
    {
        public IEnumerable<GazetteEntry> AllGazetteEntries
        {
            get
            {
                var result = new List<GazetteEntry>();
                result.AddRange(this.entry);
                foreach ( var decadeList in this.decade )
                {
                    result.AddRange(decadeList.AllGazetteEntries);
                }
                foreach ( var yearList in this.year )
                {
                    result.AddRange(yearList.AllGazetteEntries);
                }
                return result;
            }
        }

        public IEnumerable<GazetteEntry> AllAboutGeocode(UInt32 geocode, Boolean includeSubEnties)
        {
            var result = new List<GazetteEntry>();
            result.AddRange(this.entry.Where(x => x.IsAboutGeocode(geocode, includeSubEnties)));
            foreach ( var decadeList in this.decade )
            {
                result.AddRange(decadeList.AllAboutGeocode(geocode, includeSubEnties));
            }
            foreach ( var yearList in this.year )
            {
                result.AddRange(yearList.AllAboutGeocode(geocode, includeSubEnties));
            }
            return result;
        }
    }
}