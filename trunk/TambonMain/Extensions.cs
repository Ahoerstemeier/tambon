using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Extension methods for Tambon.
    /// </summary>
    public static class Extensions
    {
        private static IEnumerable<EntityType> _entityLocalGovernment = new List<EntityType>()
        {
                EntityType.Thesaban,
                EntityType.ThesabanNakhon,
                EntityType.ThesabanMueang,
                EntityType.ThesabanTambon,
                EntityType.Mueang,
                EntityType.Sukhaphiban,
                EntityType.TAO,
                EntityType.PAO,
        };

        // TODO: Move to resource and make it translateable there
        private static Dictionary<EntityType, Dictionary<Language, String>> _entityTranslations = new Dictionary<EntityType, Dictionary<Language, String>>()
        {
                {EntityType.Thesaban, new Dictionary<Language,String>(){
                    {Language.English,"municipality"},
                    {Language.German,"Stadt"},
                    {Language.Thai,"เทศบาล"}
                }},
                {EntityType.ThesabanTambon, new Dictionary<Language,String>(){
                    {Language.English,"subdistrict municipality"},
                    {Language.German,"Kleinstadt"},
                    {Language.Thai,"เทศบาลตำบล"}
                }},
                {EntityType.ThesabanMueang, new Dictionary<Language,String>(){
                    {Language.English,"town"},
                    {Language.German,"Kleinstadt"},
                    {Language.Thai,"เทศบาลเมือง"}
                }},
                {EntityType.ThesabanNakhon, new Dictionary<Language,String>(){
                    {Language.English,"city"},
                    {Language.German,"Großstadt"},
                    {Language.Thai,"เทศบาลนคร"}
                }},
                {EntityType.Muban, new Dictionary<Language,String>(){
                    {Language.English,"village"},
                    {Language.German,"Dorf"},
                    {Language.Thai,"หมู่บ้าน"}
                }},
                {EntityType.Tambon, new Dictionary<Language,String>(){
                    {Language.English,"subdistrict"},
                    {Language.German,"Kommune"},
                    {Language.Thai,"ตำบล"}
                }},
                {EntityType.Amphoe, new Dictionary<Language,String>(){
                    {Language.English,"district"},
                    {Language.German,"Kreis"},
                    {Language.Thai,"อำเภอ"}
                }},
                {EntityType.Changwat, new Dictionary<Language,String>(){
                    {Language.English,"province"},
                    {Language.German,"Provinz"},
                    {Language.Thai,"จังหวัด"}
                }},
        };

        /// <summary>
        /// Checks whether a entity type is a local government unit.
        /// </summary>
        /// <param name="type">Entity type.</param>
        /// <returns><c>true</c> if is a local government unit, <c>false</c> otherwise.</returns>
        /// <remarks>Local government units are <see cref="EntityType.Thesaban"/> (<see cref="EntityType.ThesabanNakhon"/>, <see cref="EntityType.ThesabanMueang"/>, <see cref="EntityType.ThesabanTambon"/>),
        /// <see cref="EntityType.Mueang"/>, <see cref="EntityType.TAO"/>, <see cref="EntityType.PAO"/> and <see cref="EntityType.Sukhaphiban"/>.
        /// <see cref="EntityType.Bangkok"/> is not included here.
        /// </remarks>
        static public Boolean IsThesaban(this EntityType type)
        {
            return _entityLocalGovernment.Contains(type);
        }

        private static IEnumerable<EntityType> _entitySakha = new List<EntityType>()
        {
                EntityType.Sakha,
                EntityType.SakhaTambon,
                EntityType.SakhaKhwaeng
        };

        /// <summary>
        /// Checks whether a entity type is one of the branch types.
        /// </summary>
        /// <param name="type">Entity type.</param>
        /// <returns><c>true</c> if is a branch administrative unit, <c>false</c> otherwise.</returns>
        /// <remarks>Branch administrative units are <see cref="EntityType.Sakha"/>, <see cref="EntityType.SakhaTambon"/> and <see cref="EntityType.SakhaKhwaeng"/>.
        /// </remarks>
        static public Boolean IsSakha(this EntityType type)
        {
            return _entityLocalGovernment.Contains(type);
        }

        private static IEnumerable<EntityType> _entityProvince = new List<EntityType>()
        {
                EntityType.Changwat,
                EntityType.Bangkok,
        };

        /// <summary>
        /// Checks whether a entity type is a first level administrative unit.
        /// </summary>
        /// <param name="type">Entity type.</param>
        /// <returns><c>true</c> if is a first level administrative unit, <c>false</c> otherwise.</returns>
        /// <remarks>First level administrative units are <see cref="EntityType.Changwat"/> and <see cref="EntityType.Bangkok"/>.
        /// </remarks>
        static public Boolean IsFirstLevelAdministrativeUnit(this EntityType type)
        {
            return _entityProvince.Contains(type);
        }

        private static IEnumerable<EntityType> _entityDistrict = new List<EntityType>()
        {
                EntityType.Amphoe,
                EntityType.KingAmphoe,
                EntityType.Khet
        };

        /// <summary>
        /// Checks whether a entity type is a second level administrative unit.
        /// </summary>
        /// <param name="type">Entity type.</param>
        /// <returns><c>true</c> if is a second level administrative unit, <c>false</c> otherwise.</returns>
        /// <remarks>Second level administrative units are <see cref="EntityType.Amphoe"/>, <see cref="EntityType.KingAmphoe"/> and <see cref="EntityType.Khet"/>,
        /// or those covered by <see cref="IsThesaban"/> or <see cref="IsSakha"/>.
        /// </remarks>
        static public Boolean IsSecondLevelAdministrativeUnit(this EntityType type)
        {
            return _entityDistrict.Contains(type) | type.IsThesaban() | type.IsSakha();
        }

        private static IEnumerable<EntityType> _entitySubDistrict = new List<EntityType>()
        {
                EntityType.Tambon,
                EntityType.Khwaeng
        };

        /// <summary>
        /// Checks whether a entity type is a third level administrative unit.
        /// </summary>
        /// <param name="type">Entity type.</param>
        /// <returns><c>true</c> if is a third level administrative unit, <c>false</c> otherwise.</returns>
        /// <remarks>Thrid level administrative units are <see cref="EntityType.Tambon"/> and <see cref="EntityType.Khwaeng"/>.</remarks>
        static public Boolean IsThirdLevelAdministrativeUnit(this EntityType type)
        {
            return _entitySubDistrict.Contains(type);
        }

        /// <summary>
        /// Gets whether two entity types are at the same administrative level.
        /// </summary>
        /// <param name="value">Entity type.</param>
        /// <param name="compare">Entity type to compare with.</param>
        /// <returns><c>true</c> if both types are at same administrative level or equal, <c>false otherwise.</c></returns>
        /// <remarks>The following combinations are tested.
        /// <list type="bullet">
        /// <item><description><see cref="EntityType.Bangkok"/> and <see cref="EntityType.Changwat"/>.</description></item>
        /// <item><description><see cref="EntityType.Amphoe"/>, <see cref="EntityType.KingAmphoe"/>, <see cref="EntityType.Khet"/> and <see cref="EntityType.Sakha"/>.</description></item>
        /// <item><description><see cref="EntityType.Tambon"/> and <see cref="EntityType.Khwaeng"/>.</description></item>
        /// <item><description><see cref="EntityType.Thesaban"/>, <see cref="EntityType.ThesabanTambn"/>, <see cref="EntityType.ThesabanMueanmg"/> and <see cref="EntityType.ThesabanNakhon"/>.</description></item>
        /// </list></remarks>
        public static Boolean IsCompatibleEntityType(this EntityType value, EntityType compare)
        {
            Boolean retval = false;
            switch ( value )
            {
                case EntityType.Bangkok:
                case EntityType.Changwat:
                    retval = (compare == EntityType.Changwat) | (compare == EntityType.Bangkok);
                    break;

                case EntityType.KingAmphoe:
                case EntityType.Khet:
                case EntityType.Sakha:
                case EntityType.Amphoe:
                    retval = (compare == EntityType.Amphoe) | (compare == EntityType.KingAmphoe) | (compare == EntityType.Khet) | (compare == EntityType.Sakha);
                    break;

                case EntityType.Khwaeng:
                case EntityType.Tambon:
                    retval = compare.IsThirdLevelAdministrativeUnit();
                    break;

                case EntityType.Thesaban:
                case EntityType.ThesabanNakhon:
                case EntityType.ThesabanMueang:
                case EntityType.ThesabanTambon:
                    retval = compare.IsThesaban();
                    break;

                default:
                    retval = (value == compare);
                    break;
            }
            return retval;
        }

        /// <summary>
        /// Checks whether the given council size fits to the administrative type.
        /// </summary>
        /// <param name="value">Type of entity.</param>
        /// <param name="size">Size of council.</param>
        /// <returns><c>true</c> if council size is valid, <c>false</c> otherwise.</returns>
        public static Boolean IsValidCouncilSize(this EntityType value, UInt32 size)
        {
            switch ( value )
            {
                case EntityType.TAO:
                    return (size % 2 == 0); // 2 councilors per Muban - this will also accept 0
                case EntityType.ThesabanTambon:
                    return (size == 12);
                case EntityType.ThesabanMueang:
                    return (size == 18);
                case EntityType.ThesabanNakhon:
                    return (size == 24);
                case EntityType.PAO:
                    return (size == 0) | ((size % 6 == 0) & (size >= 18) & (size <= 48));
                case EntityType.Bangkok:
                case EntityType.Khet:
                case EntityType.SaphaTambon:
                case EntityType.ProvinceCouncil:
                case EntityType.Sukhaphiban:
                case EntityType.Mueang:
                    return true;   // no way to calculate correct size of council
                default:
                    return false;  // no council for that kind of entity
            }
        }

        /// <summary>
        /// Gets the length of a term for the given entity type.
        /// </summary>
        /// <param name="value">Tyoe of entity.</param>
        /// <returns>Length of term in years, <c>0</c> if undefinied.</returns>
        public static Byte TermLength(this EntityType value)
        {
            switch ( value )
            {
                case EntityType.TAO:
                case EntityType.ThesabanTambon:
                case EntityType.ThesabanMueang:
                case EntityType.ThesabanNakhon:
                case EntityType.PAO:
                case EntityType.Bangkok:
                case EntityType.Khet:
                case EntityType.Mueang:
                    return 4;
                case EntityType.ProvinceCouncil:
                    return 5;
                case EntityType.SaphaTambon:
                case EntityType.Sukhaphiban:
                    return 0;  // don't know
                default:
                    return 0;  // no council for that kind of entity
            }
        }

        public static String Translate(this EntityType value, Language language)
        {
            var result = String.Empty;
            if ( _entityTranslations.ContainsKey(value) )
            {
                var subDictionary = _entityTranslations[value];
                if ( subDictionary.ContainsKey(language) )
                    result = subDictionary[language];
            }
            return result;
        }
    }
}