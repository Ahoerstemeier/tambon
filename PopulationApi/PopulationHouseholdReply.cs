using Newtonsoft.Json;
using System;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Population and household number reply.
    /// </summary>
    public class PopulationHouseholdReply: PopulationReplyBase
    {

        /// <summary>
        /// Gets or sets the total male population number.
        /// </summary>
        /// <value>The total male population number.</value>
        [JsonProperty(PropertyName = "lssumtotMale")]
        public Int32 TotalMale { get; set; }

        /// <summary>
        /// Gets or sets the total female population number.
        /// </summary>
        /// <value>The total female population number.</value>
        [JsonProperty(PropertyName = "lssumtotFemale")]
        public Int32 TotalFemale { get; set; }

        [JsonProperty(PropertyName = "lssumtotPop")]
        /// <summary>
        /// Gets or sets the total population number.
        /// </summary>
        /// <value>The total population number.</value>
        public Int32 TotalPopulation { get; set; }

        /// <summary>
        /// Gets or sets the number of households without termination date.
        /// </summary>
        /// <value>The number of households without termination date.</value>
        [JsonProperty(PropertyName = "lssumnotTermDate")]
        public Int32 HouseholdsWithoutTerminationDate { get; set; }

        /// <summary>
        /// Gets or sets the number of households with termination date.
        /// </summary>
        /// <value>The number of households with termination date.</value>
        [JsonProperty(PropertyName = "lssumtermDate")]
        public Int32 HouseholdsTerminationDate { get; set; }

        /// <summary>
        /// Gets or sets the total household number.
        /// </summary>
        /// <value>The total household number.</value>
        [JsonProperty(PropertyName = "lssumtotHouse")]
        public Int32 TotalHouseholds { get; set; }
    }


}
