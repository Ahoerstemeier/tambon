using Newtonsoft.Json;
using System;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Base population reply.
    /// </summary>
    public class PopulationReplyBase
    {
        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        /// <remarks>Apparently always 0?</remarks>
        [JsonProperty(PropertyName = "lstrLevel")]
        public Int32 Level { get; set; }

        /// <summary>
        /// Gets or sets the region code.
        /// </summary>
        /// <value>The region code.</value>
        /// <remarks>First digit of <see cref="ChangwatCode"/>, but usually 0.</remarks>
        [JsonProperty(PropertyName = "lsregion")]
        public Int32 Region { get; set; }

        /// <summary>
        /// Gets or sets the province code.
        /// </summary>
        /// <value>The province code.</value>
        /// <remarks>Two digit value, not the full geocode.</remarks>

        [JsonProperty(PropertyName = "lscc")]
        public Int32 ChangwatCode { get; set; }

        /// <summary>
        /// Gets or sets the province display name.
        /// </summary>
        /// <value>The province display name.</value>
        [JsonProperty(PropertyName = "lsccDesc")]
        public String ChangwatName { get; set; }

        /// <summary>
        /// Gets or sets the registrar code.
        /// </summary>
        /// <value>The registrar code.</value>
        /// <remarks>Two or four digit value.</remarks>
        [JsonProperty(PropertyName = "lsrcode")]
        public Int32 RegistrarCode { get; set; }

        /// <summary>
        /// Gets or sets the registrar display name.
        /// </summary>
        /// <value>The registrar display name.</value>
        [JsonProperty(PropertyName = "lsrcodeDesc")]
        public String RegistrarName { get; set; }

        /// <summary>
        /// Gets or sets the district code.
        /// </summary>
        /// <value>The district code.</value>
        /// <remarks>Two digit value, not the full geocode.</remarks>

        [JsonProperty(PropertyName = "lsaa")]
        public Int32 AmphoeCode { get; set; }

        /// <summary>
        /// Gets or sets the district display name.
        /// </summary>
        /// <value>The district display name.</value>
        [JsonProperty(PropertyName = "lsaaDesc")]
        public String AmphoeName { get; set; }

        /// <summary>
        /// Gets or sets the subdistrict code.
        /// </summary>
        /// <value>The subdistrict code.</value>
        /// <remarks>Two digit value, not the full geocode.</remarks>
        [JsonProperty(PropertyName = "lstt")]
        public Int32 TambonCode { get; set; }

        /// <summary>
        /// Gets or sets the subdistrict display name.
        /// </summary>
        /// <value>The subdistrict display name.</value>
        [JsonProperty(PropertyName = "lsttDesc")]
        public String TambonName { get; set; }

        /// <summary>
        /// Gets or sets the village code.
        /// </summary>
        /// <value>The village code.</value>
        /// <remarks>Two digit value, not the full geocode.</remarks>
        [JsonProperty(PropertyName = "lsmm")]
        public Int32 MubanCode { get; set; }

        /// <summary>
        /// Gets or sets the village display name.
        /// </summary>
        /// <value>The village display name.</value>
        [JsonProperty(PropertyName = "lsmmDesc")]
        public String MubanName { get; set; }

        /// <summary>
        /// Gets or sets the year and month.
        /// </summary>
        /// <value>The year and month.</value>
        /// <remarks>Value is two digit Thai year number, and two digit month number. 
        /// E.g. 6212 for December 2562 (2019).</remarks>
        [JsonProperty(PropertyName = "lsyymm")]
        public Int32 YearMonth { get; set; }

    }
}
