using System;
using System.Collections.Generic;

namespace De.AHoerstemeier.Tambon
{
    internal interface IGazetteEntries
    {
        IEnumerable<GazetteEntry> AllAboutGeocode(UInt32 geocode, Boolean includeSubEnties);

        IEnumerable<GazetteEntry> AllGazetteEntries
        {
            get;
        }
    }
}