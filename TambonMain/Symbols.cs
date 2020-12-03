using System;
using System.Linq;

namespace De.AHoerstemeier.Tambon
{
    public partial class Symbols: IIsEmpty
    {
        /// <inheritdoc/>
        public Boolean IsEmpty()
        {
            return
                !vision.Any() &&
                !slogan.Any() &&
                !mission.Any() &&
                !goal.Any() &&
                !emblem.Any() &&
                color == "unknown" &&
                !song.Any() &&
                String.IsNullOrEmpty(symbolbird) &&
                String.IsNullOrEmpty(symboltree) &&
                String.IsNullOrEmpty(symbolflower) &&
                String.IsNullOrEmpty(wateranimal);
        }
    }
}