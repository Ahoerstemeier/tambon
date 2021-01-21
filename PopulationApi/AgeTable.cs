using Newtonsoft.Json;
using System;

namespace PopulationApi
{
    public class AgeTable
    {

        [JsonProperty(PropertyName = "lstrLevel")]
        public Int32 Level { get; set; }

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
        /// Gets or sets the registrar code.
        /// </summary>
        /// <value>The registrar code.</value>
        /// <remarks>Two or four digit value.</remarks>
        [JsonProperty(PropertyName = "lsrcode")]
        public String RegistrarCode { get; set; }
    
        /// <summary>
        /// Gets or sets the district code.
        /// </summary>
        /// <value>The district code.</value>
        /// <remarks>Two digit value, not the full geocode.</remarks>

        [JsonProperty(PropertyName = "lsaa")]
        public Int32 AmphoeCode { get; set; }

        /// <summary>
        /// Gets or sets the subdistrict code.
        /// </summary>
        /// <value>The subdistrict code.</value>
        /// <remarks>Two digit value, not the full geocode.</remarks>
        [JsonProperty(PropertyName = "lstt")]
        public Int32 TambonCode { get; set; }

     
     

        /// <summary>
        /// Gets or sets the year and month.
        /// </summary>
        /// <value>The year and month.</value>
        /// <remarks>Value is two digit Thai year number, and two digit month number. 
        /// E.g. 6212 for December 2562 (2019).</remarks>
        [JsonProperty(PropertyName = "lsyymm")]
        public Int32 YearMonth { get; set; }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>The gender.</value>
        /// <remarks>1 means male, 2 means female.</remarks>
        [JsonProperty(PropertyName = "lsageSex")]
        public Int32 Gender { get; set; }

        /// <summary>
        /// Gets or sets the number for age 0.
        /// </summary>
        /// <value>The number for age 0.</value>
        [JsonProperty(PropertyName = "lsAge0")]
        public Int32 Age0 { get; set; }

        // ...

        /// <summary>
        /// Gets or sets the number for age 100.
        /// </summary>
        /// <value>The number for age 100.</value>
        [JsonProperty(PropertyName = "lsAge100")]
        public Int32 Age100 { get; set; }

        /// <summary>
        /// Gets or sets the number for age above 100.
        /// </summary>
        /// <value>The number for age above 100.</value>
        [JsonProperty(PropertyName = "lsAge101")]
        public Int32 AgeAbove100 { get; set; }

        /// <summary>
        /// Gets or sets the total number.
        /// </summary>
        /// <value>The total number.</value>
        [JsonProperty(PropertyName = "lsSumTotTot")]
        public Int32 Total { get; set; }

    }
}
