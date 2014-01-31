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
        };

        public const String PropertyIdEntityType = "P132";
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
        public const String PropertyPopulation = "P1082";
        // public const String PropertyArea = "Pxxxx";

        // for qualifiers
        public const String PropertyPointInTime = "P585";

        public const String PropertyStartDate = "P580";
        public const String PropertyEndDate = "P582";

        // for sources
        public const String PropertyIdStatedIn = "P248";

        public const String PropertyIdReferenceUrl = "P854";

        // source statements for TIS 1099
        public const String ItemSourceTIS1099BE2535 = "Q15477441";

        public const String ItemSourceTIS1099BE2548 = "Q15477531";
        public const String ItemSourceCCAATT = "Q15477767";

        // source statements for population
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

        public const String ItemSandbox = "Q4115189";
    }
}