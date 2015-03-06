using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class WikiBase
    {
        /// <summary>
        /// Translation table from <see cref="EntityType"/> to the item at WikiData.
        /// </summary>
        public static Dictionary<EntityType, String> WikiDataItems = new Dictionary<EntityType, String>()
        {
            {EntityType.Changwat, "Q50198"},
            {EntityType.Amphoe, "Q475061"},
            {EntityType.KingAmphoe, "Q6519277"},
            {EntityType.Tambon, "Q1077097"},
            {EntityType.Muban, "Q1368879"},
            {EntityType.TAO, "Q15140073"},
            {EntityType.PAO, "Q13023500"},
            {EntityType.Thesaban, "Q1155688"},
            {EntityType.ThesabanTambon, "Q15141625"},
            {EntityType.ThesabanMueang, "Q13025342"},
            {EntityType.ThesabanNakhon, "Q15141632"},
            {EntityType.Sukhaphiban, "Q7635776"},
            {EntityType.Monthon, "Q1936511"},
            {EntityType.Country, "Q869"},
            {EntityType.Khet, "Q15634531"},
            {EntityType.Khwaeng, "Q456876"},
            {EntityType.Chumchon,"Q15253857"},
            {EntityType.SaphaTambon,"Q15695218"},
            {EntityType.KlumChangwat,"Q13012657"}
        };

        // public const String PropertyIdEntityType = "P132";
        public const String PropertyIdCountry = "P17";

        public const String PropertyIdIsInAdministrativeUnit = "P131";
        public const String PropertyIdCoordinate = "P625";
        public const String PropertyIdCapital = "P36";
        public const String PropertyIdContainsAdministrativeDivisions = "P150";
        public const String PropertyIdWebsite = "P856";
        public const String PropertyIdSharesBorderWith = "P47";
        public const String PropertyIdHeadOfGovernment = "P6";
        public const String PropertyIdPostalCode = "P281";
        public const String PropertyIdTwinCity = "P190";
        public const String PropertyIdOpenStreetMap = "P402";
        public const String PropertyIdLocationMap = "P242";
        public const String PropertyIdFreebaseIdentifier = "P646";
        public const String PropertyIdISO3166 = "P300";
        public const String PropertyIdGND = "P227";
        public const String PropertyIdFIPS10 = "P901";
        public const String PropertyIdDmoz = "P998";
        public const String PropertyIdThaiGeocode = "P1067";
        public const String PropertyIdInstanceOf = "P31";
        public const String PropertyIdPopulation = "P1082";
        public const String PropertyIdSocialMedia = "P553";
        public const String PropertyIdSocialMediaAddress = "P554";
        public const String PropertyIdFoundationalText = "P457";
        public const String PropertyIdMotto = "P1451";
        public const String PropertyIdNativeLabel = "P1705";
        public const String PropertyIdDeterminationMethod = "P459";

        // public const String PropertyIdArea = "Pxxxx";
        public const String PropertyIdCategoryCombinesTopic = "P971";

        public const String PropertyIdIsListOf = "P360";

        // for qualifiers
        public const String PropertyIdPointInTime = "P585";

        public const String PropertyIdStartDate = "P580";
        public const String PropertyIdEndDate = "P582";

        // for sources
        public const String PropertyIdStatedIn = "P248";

        public const String PropertyIdReferenceUrl = "P854";
        public const String PropertyIdPublisher = "P123";

        // source statements for TIS 1099
        public const String ItemSourceTIS1099BE2535 = "Q15477441";

        public const String ItemSourceTIS1099BE2548 = "Q15477531";
        public const String ItemSourceCCAATT = "Q15477767";
        public const String ItemDopa = "Q13012489";

        public const String ItemCensuses = "Q39825";
        public const String ItemRegistration = "Q15194024";

        // source statements for population
        public static Dictionary<Int16, String> ItemCensus = new Dictionary<Int16, String>()
        {
        {2010,ItemCensus2010},
        {2000,ItemCensus2000},
        {1990,ItemCensus1990},
        {1980,ItemCensus1980},
        {1970,ItemCensus1970},
        {1960,ItemCensus1960},
        {1947,ItemCensus1947},
        {1937,ItemCensus1937},
        {1929,ItemCensus1929},
        {1919,ItemCensus1919},
        {1909,ItemCensus1909},
        };

        public const String ItemCensus2010 = "Q15637207";

        public const String ItemCensus2000 = "Q15637213";
        public const String ItemCensus1990 = "Q15637229";
        public const String ItemCensus1980 = "Q15637237";
        public const String ItemCensus1970 = "Q15639176";
        public const String ItemCensus1960 = "Q15639232";
        public const String ItemCensus1947 = "Q15639300";
        public const String ItemCensus1937 = "Q15639324";
        public const String ItemCensus1929 = "Q15639341";
        public const String ItemCensus1919 = "Q15639367";
        public const String ItemCensus1909 = "Q15639395";

        public const String ItemSocialMediaTwitter = "Q918";
        public const String ItemSocialMediaFacebook = "Q335";
        public const String ItemSocialMediaGooglePlus = "Q356";
        public const String ItemSocialMediaFoursquare = "Q51709";

        public const String ItemSandbox = "Q4115189";
    }
}