using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class Symbols
    {
        /// <summary>
        /// Checks whether the class contains any data or not.
        /// </summary>
        /// <returns><c>true</c> if the class is effectively empty, <c>false</c> if there is anything set.</returns>
        public Boolean IsEmpty()
        {
            return
                !vision.Any() &&
                !slogan.Any() &&
                !mission.Any() &&
                !goal.Any() &&
                !emblem.Any() &&
                color == SymbolColor.unknown &&
                !song.Any() &&
                String.IsNullOrEmpty(symbolbird) &&
                String.IsNullOrEmpty(symboltree) &&
                String.IsNullOrEmpty(symbolflower);
        }
    }
}