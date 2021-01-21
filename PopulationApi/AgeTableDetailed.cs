using Newtonsoft.Json;
using System;

namespace PopulationApi
{
    public class AgeTableDetailed:AgeTable
    {
        /// <summary>
        /// Gets or sets the nationality.
        /// </summary>
        /// <value>The gender.</value>
        /// <remarks>1 means male, 2 means female.</remarks>
        [JsonProperty(PropertyName = "lageNat")]
        public Int32 Nationality { get; set; }

        /// <summary>
        /// Gets or sets the statistics type.
        /// </summary>
        /// <value>The statistics type.</value>
        [JsonProperty(PropertyName = "lagePopSt")]
        public Int32 StatisticsType { get; set; }

        [JsonProperty(PropertyName = "lsAgethai")]
        public Int32 AgeThai { get; set; }

        [JsonProperty(PropertyName = "lstAgev0")]
        public Int32 AgeV0 { get; set; }
    }
}
