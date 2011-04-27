using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    class ConstituencyList:List<ConstituencyEntry>
    {
        public Int32 Population()
        {
            Int32 lResult = 0;
            foreach (ConstituencyEntry lEntry in this)
            {
                lResult += lEntry.Population();
            }
            return lResult;
        }
        public Int32 NumberOfSeats()
        {
            Int32 lResult = 0;
            foreach (ConstituencyEntry lEntry in this)
            {
                lResult += lEntry.NumberOfSeats;
            }
            return lResult;
        }
    }
}
