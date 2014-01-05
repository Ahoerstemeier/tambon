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
            {EntityType.Khwaeng, "Q456876"},
            {EntityType.Chumchon,"Q15253857"},
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
        public const String PropertyIdReferenceUrl = "P854";
        public const String PropertyIdISO3166 = "P300";
        public const String PropertyIdGND = "P227";
        public const String PropertyIdFIPS10 = "P901";
        public const String PropertyIdDmoz = "P998";
        public const String PropertyIdThaiGeocode = "P1999";  // to be created
        public const String PropertyIdStatedIn = "P248";

        public const String ItemSourceTIS1099BE2535 = "Q15477441";
        public const String ItemSourceTIS1099BE2548 = "Q15477531";
        public const String ItemSourceCCAATT = "Q15477767";

        public const String ItemSandbox = "Q4115189";
    }
}