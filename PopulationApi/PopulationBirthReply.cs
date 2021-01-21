using Newtonsoft.Json;
using System;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Birth reply.
    /// </summary>
    public class PopulationBirthReply : PopulationReplyBase
    {
        /// <summary>
        /// Gets or sets the total population number.
        /// </summary>
        /// <value>The total population number.</value>
        [JsonProperty(PropertyName = "lssumtotalAll")]
        public Int32 TotalNumber { get; set; }

        /// <summary>
        /// Gets or sets the female population number.
        /// </summary>
        /// <value>The female population number.</value>
        [JsonProperty(PropertyName = "lssumtotalGirl")]
        public Int32 Female { get; set; }

        [JsonProperty(PropertyName = "lssumtotalBoy")]
        /// <summary>
        /// Gets or sets the male population number.
        /// </summary>
        /// <value>The male population number.</value>
        public Int32 Male { get; set; }

    }


}
