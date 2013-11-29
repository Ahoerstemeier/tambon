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
    }
}