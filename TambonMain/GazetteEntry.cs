using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using De.AHoerstemeier.Tambon;

namespace De.AHoerstemeier.Tambon
{
    public partial class GazetteEntry : IGeocode
    {
        #region IGeocode Members

        public Boolean IsAboutGeocode(UInt32 geocode, Boolean includeSubEntities)
        {
            var result = false;
            foreach ( var entry in Items )
            {
                var toTest = entry as IGeocode;
                if ( toTest != null )
                {
                    result = result | toTest.IsAboutGeocode(geocode, includeSubEntities);
                }
            }
            return result;
        }

        #endregion IGeocode Members
    }
}