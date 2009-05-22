using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    class RoyalGazetteContentMention:RoyalGazetteContent
    {
        internal const String XmlLabel = "mention";
        protected override String GetXmlLabel()
        {
            return XmlLabel;
        }
    }
}
