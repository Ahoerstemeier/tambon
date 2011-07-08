using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Globalization;

namespace De.AHoerstemeier.Tambon
{
    public class EntityTypeHelper
    {
        public static Dictionary<EntityType, String> EntityNames = new Dictionary<EntityType, String>()
            {
              {EntityType.Changwat, "จังหวัด"},
              {EntityType.Amphoe, "อำเภอ" },
              {EntityType.Tambon, "ตำบล"},
              {EntityType.Thesaban, "เทศบาล"},
              {EntityType.ThesabanNakhon, "เทศบาลนคร"},
              {EntityType.ThesabanMueang, "เทศบาลเมือง"},
              {EntityType.ThesabanTambon, "เทศบาลตำบล"},
              {EntityType.Mueang, "เมือง"},
              {EntityType.SakhaTambon, "สาขาตำบล"},
              {EntityType.SakhaKhwaeng, "สาขาแขวง"},
              {EntityType.Sakha, "สาขา"},
              {EntityType.KingAmphoe, "กิ่งอำเภอ"},
              {EntityType.Khet, "เขต"},
              {EntityType.Khwaeng, "แขวง"},
              {EntityType.Bangkok, "กรุงเทพมหานคร"},
              {EntityType.Monthon, "มณฑล"},
              {EntityType.Sukhaphiban, "สุขาภิบาล"},
              {EntityType.SukhaphibanTambon, "สุขาภิบาลตำบล"},
              {EntityType.SukhaphibanMueang, "สุขาภิบาลเมือง"},
              {EntityType.Chumchon, "ชุมชน"},
              {EntityType.TAO, "องค์การบริหารส่วนตำบล"},
              {EntityType.TC, "สภาตำบล"},
              {EntityType.Phak, "ภาค"},
              {EntityType.KlumChangwat, "กลุ่มจังหวัด"},
              {EntityType.Constituency, "เขตเลือกตั้ง"}
            };
        private static List<EntityType> mEntityThesaban = new List<EntityType>() 
        { 
                EntityType.Thesaban, 
                EntityType.ThesabanNakhon, 
                EntityType.ThesabanMueang, 
                EntityType.ThesabanTambon, 
                EntityType.Mueang, 
                EntityType.Sukhaphiban,
                EntityType.TAO
        };
        static public List<EntityType> Thesaban
        {
            get { return mEntityThesaban; }
        }
        static private List<EntityType> CreateAllEntityLevels()
        {
            var retval = new List<EntityType>();
            foreach ( EntityType i in System.Enum.GetValues(typeof(EntityType)) )
            {
                retval.Add(i);
            }
            return retval;
        }
        private static List<EntityType> mAllEntityTypes = null;
        static public List<EntityType> AllEntityTypes
        {
            get
            {
                if ( mAllEntityTypes == null )
                {
                    mAllEntityTypes = CreateAllEntityLevels();
                }
                return mAllEntityTypes;
            }
        }
        private static List<EntityType> mEntitySakha = new List<EntityType>() 
        { 
                EntityType.Sakha, 
                EntityType.SakhaTambon, 
                EntityType.SakhaKhwaeng 
        };
        static public List<EntityType> Sakha
        {
            get { return mEntitySakha; }
        }
        private static List<EntityType> mEntitySecondLevel = null;
        static public List<EntityType> SecondLevelEntity
        {
            get
            {
                if ( mEntitySecondLevel == null )
                {
                    mEntitySecondLevel = CreateEntitySecondLevel();
                }
                return mEntitySecondLevel;
            }
        }
        static private List<EntityType> CreateEntitySecondLevel()
        {
            var retval = new List<EntityType>() 
            { 
                EntityType.Amphoe,
                EntityType.KingAmphoe,
                EntityType.Khet
            };
            retval.AddRange(Sakha);
            retval.AddRange(Thesaban);
            return retval;
        }
        public static EntityType ParseEntityType(string iValue)
        {
            EntityType retval = EntityType.Unknown;
            if ( !String.IsNullOrEmpty(iValue) )
            {
                foreach ( KeyValuePair<EntityType, string> lKeyValuePair in EntityNames )
                {
                    if ( iValue.StartsWith(lKeyValuePair.Value) )
                    {
                        if ( retval == EntityType.Unknown )
                        {
                            retval = lKeyValuePair.Key;
                        }
                        // special case - Sakha and SakhaTambon might make problems otherwise
                        else if ( lKeyValuePair.Value.Length > EntityNames[retval].Length )
                        {
                            retval = lKeyValuePair.Key;
                        }
                    }

                }
            }
            if ( retval == EntityType.Unknown )
            {
                retval = EntityType.Unknown;
            }
            return retval;
        }
        public static Boolean IsCompatibleEntityType(EntityType iValue1, EntityType iValue2)
        {
            Boolean retval = false;
            switch ( iValue1 )
            {
                case EntityType.Bangkok:
                case EntityType.Changwat:
                    retval = (iValue2 == EntityType.Changwat) | (iValue2 == EntityType.Bangkok);
                    break;

                case EntityType.KingAmphoe:
                case EntityType.Khet:
                case EntityType.Sakha:
                case EntityType.Amphoe:
                    retval = (iValue2 == EntityType.Amphoe) | (iValue2 == EntityType.KingAmphoe) | (iValue2 == EntityType.Khet) | (iValue2 == EntityType.Sakha);
                    break;

                case EntityType.Khwaeng:
                case EntityType.Tambon:
                    retval = (iValue2 == EntityType.Khwaeng) | (iValue2 == EntityType.Tambon);
                    break;

                case EntityType.Thesaban:
                case EntityType.ThesabanNakhon:
                case EntityType.ThesabanMueang:
                case EntityType.ThesabanTambon:
                    retval = (iValue2 == EntityType.ThesabanMueang) | (iValue2 == EntityType.ThesabanNakhon) | (iValue2 == EntityType.ThesabanTambon) | (iValue2 == EntityType.Thesaban);
                    break;

                default:
                    retval = (iValue1 == iValue2);
                    break;
            }
            return retval;
        }
    }
}