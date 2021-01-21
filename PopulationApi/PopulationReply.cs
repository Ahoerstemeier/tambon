using Newtonsoft.Json;
using System;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Population reply.
    /// </summary>
    /// <remarks>Used for death, move in and move out statistics.</remarks>
    public class PopulationReply : PopulationReplyBase
    {
        /// <summary>
        /// Gets or sets the total population number.
        /// </summary>
        /// <value>The total population number.</value>
        [JsonProperty(PropertyName = "lssumtotTot")]
        public Int32 TotalNumber { get; set; }

        /// <summary>
        /// Gets or sets the female population number.
        /// </summary>
        /// <value>The female population number.</value>
        [JsonProperty(PropertyName = "lssumtotFemale")]
        public Int32 Female { get; set; }

        [JsonProperty(PropertyName = "lssumtotMale")]
        /// <summary>
        /// Gets or sets the male population number.
        /// </summary>
        /// <value>The male population number.</value>
        public Int32 Male { get; set; }

    }


}
