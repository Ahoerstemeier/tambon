using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    class RoyalGazetteContentConstituency:RoyalGazetteContent
    {
        internal const String XmlLabel = "constituency";
        protected override String GetXmlLabel()
        {
            return XmlLabel;
        }
    }
}
