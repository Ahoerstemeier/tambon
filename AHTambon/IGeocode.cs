using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    interface IGeocode
    {
        Boolean IsAboutGeocode(Int32 iGeocode, Boolean iIncludeSubEntities);
    }
}
