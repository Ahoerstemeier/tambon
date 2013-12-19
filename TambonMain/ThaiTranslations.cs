using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.AHoerstemeier.Tambon
{
    internal static class ThaiTranslations
    {
        public static Dictionary<EntityType, String> EntityNamesThai = new Dictionary<EntityType, String>()
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
              {EntityType.PAO, "องค์การบริหารส่วนจังหวัด"},
              {EntityType.SaphaTambon, "สภาตำบล"},
              {EntityType.Phak, "ภาค"},
              {EntityType.KlumChangwat, "กลุ่มจังหวัด"},
              {EntityType.Constituency, "เขตเลือกตั้ง"}
            };

        public static Dictionary<EntityType, String> EntityAbbreviations = new Dictionary<EntityType, String>()
            {
              {EntityType.Changwat, "จ"},
              {EntityType.Amphoe, "อ" },
              {EntityType.Tambon, "ต"},
              {EntityType.ThesabanNakhon, "ทน"},
              {EntityType.ThesabanMueang, "ทม"},
              {EntityType.ThesabanTambon, "ทต"},
              {EntityType.KingAmphoe, "กิ่งอ"},
              {EntityType.TAO, "อบต"},
              {EntityType.PAO, "อบจ"},
              // Mueang, Khwaeng, Khet
            };

        public static Dictionary<EntityType, String> EntityNamesEnglish = new Dictionary<EntityType, String>()
            {
              {EntityType.Changwat, "Province"},
              {EntityType.Amphoe, "District" },
              {EntityType.Tambon, "Subdistrict"},
              {EntityType.Thesaban, "Municipality"},
              {EntityType.ThesabanNakhon, "City municipality"},
              {EntityType.ThesabanMueang, "Town municipality"},
              {EntityType.ThesabanTambon, "Subdistrict municipality"},
              //{EntityType.Mueang, "เมือง"},
              //{EntityType.SakhaTambon, "สาขาตำบล"},
              //{EntityType.SakhaKhwaeng, "สาขาแขวง"},
              //{EntityType.Sakha, "สาขา"},
              {EntityType.KingAmphoe, "Minor district"},
              {EntityType.Khet, "District"},
              {EntityType.Khwaeng, "Subdistrict"},
              //{EntityType.Bangkok, "กรุงเทพมหานคร"},
              {EntityType.Monthon, "Circle"},
              {EntityType.Sukhaphiban, "Sanitary district"},
              //{EntityType.SukhaphibanTambon, "สุขาภิบาลตำบล"},
              //{EntityType.SukhaphibanMueang, "สุขาภิบาลเมือง"},
              {EntityType.Chumchon, "Borough"},
              {EntityType.TAO, "Subdistrict administrative orgnization"},
              {EntityType.PAO, "Provincial administrative orgnization"},
              {EntityType.SaphaTambon, "Subdistrict council"},
              {EntityType.Phak, "Region"},
              {EntityType.KlumChangwat, "Province cluster"},
              {EntityType.Constituency, "Constituency"}
            };
    }
}