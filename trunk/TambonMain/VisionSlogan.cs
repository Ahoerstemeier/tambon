using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class VisionSlogan
    {
        /// <summary>
        /// Gets the <see cref="Value"/> as a single lined string.
        /// </summary>
        /// <value>The value as a single lined string.</value>
        public String SingleLineValue
        {
            get
            {
                var multiLineSlogan = Value.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
                return String.Join(" ", multiLineSlogan).Trim();
            }
        }
    }
}