using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    partial class WikiLocation
    {
        public Int32 NumericalWikiData
        {
            get
            {
                Int32 value = -1;
                if ( !String.IsNullOrWhiteSpace(wikidata) )
                {
                    try
                    {
                        var subString = wikidata.Remove(0, 1);
                        value = Convert.ToInt32(subString);
                    }
                    catch ( FormatException )
                    {
                    }
                }
                return value;
            }
        }
    }
}