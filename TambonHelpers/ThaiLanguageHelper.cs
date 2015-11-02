using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Helper class for Thai language, e.g. romanization.
    /// </summary>
    public static class ThaiLanguageHelper
    {
        /// <summary>
        /// Prefix for villages "Ban" (บ้าน) in Thai.
        /// </summary>
        public const String Ban = "บ้าน";

        /// <summary>
        /// Common suffixes for Thai names with their standard romanization.
        /// </summary>
        public static Dictionary<String, String> NameSuffixRomanizations = new Dictionary<String, String>()
        {
            {"เหนือ", "Nuea"},  //North
            {"ใต้", "Tai"}, // South
            {"พัฒนา", "Phatthana"}, // Development
            {"ใหม่", "Mai"},  // New
            {"ทอง", "Thong"}, // Gold
            {"น้อย","Noi"}, // Small
            {"ใน", "Nai"},  // within
            // less common ones
            { "สามัคคี", "Samakkhi" },  // Harmonious
            { "ใหม่พัฒนา", "Mai Phatthana"}, // New Development
            {"ตะวันออก", "Tawan Ok"}, // East
            {"ตะวันตก", "Tawan Tok"}, // West
            {"ออก", "Ok"}, // up
            {"ตก", "Tok"}, // down
            {"สอง", "Song"},  // second
            {"กลาง", "Klang"}, // Middle
            {"คำ", "Kham"},  // Word
            {"ใหญ่", "Yai"}, // Large
            {"เล็ก","Lek"},  // small
            {"เก่า", "Kao"}, // Old
            {"สันติสุข", "Santi Suk"},  // peace
            {"เจริญ", "Charoen"},  // growth
            {"ศรีเจริญ", "Si Charoen"},
            {"เจริญสุข","Charoen Suk"},
            {"บูรพา", "Burapha"},  // East
            {"สวรรค์", "Sawan"}, // Heaven
            {"หลวง", "Luang"},  // Big
            {"งาม", "Ngam"},     // Beautiful
            {"สมบูรณ์", "Sombun"}, // Complete
            {"สะอาด", "Sa-at"},  // clean
            {"นอก","Nok"},  // outside
            {"แดง","Daeng"},  // red
            {"ดง","Dong"},
            {"ไร","Rai"},  // Gain
            {"ราษฏร์","Rat"}, // people
            {"อรุณ","Arun"},  // dawn
            {"เรือ", "Ruea"},  // boat
            {"เฒ่า", "Thao"},  // old
            {"ยืน", "Yuen"},  // durable
            {"ยาง","Yang"},  // Rubber
            {"บน", "Bon"},  // upon
            {"อุดม", "Udom"},  // rich
            {"เดิม","Doem"},  // old
            {"บำรุง","Bamrung"}, // administrate
            {"เตียน","Tian"},
            {"เหลี่ยม","Liam"},
            {"คีรี","Khiri"},
            {"เด่น","Den"},  // notable
            {"สำนัก","Samnak"},  // office
            {"มงคล","Mongkhon"},  // dragon
            {"ศิริ","Siri"},
            {"ถาวร","Thawon"},  // permanent
            {"นคร", "Nakhon"},  // city
            {"สูง","Sung"},  // high
            {"ต่ำ","Tam"},  // low
            {"หัก","Hak"}, // less
            {"หนึ่ง","Nueng"},  // first, one
            {"ยาว","Yao"},  // Long
            {"รุ่งเรือง","Rung Rueang"},  // prosperous
            {"สำโรง","Samrong"},  // Sterculia foetida L.
            {"นิคม","Nikhom"},  // plantation
            {"บุรี","Buri"},  // town
            {"วัฒนา", "Watthana"}  // development
};

        /// <summary>
        /// Common suffixes for Thai names with their pronunciation in IPA.
        /// </summary>
        public static Dictionary<String, String> NameSuffixIpa = new Dictionary<String, String>()
        {
            {"เหนือ", "nɯ̌a"},  //North
            //{"ใต้", "Tai"}, // South
            //{"พัฒนา", "Phatthana"}, // Development
            // {"ใหม่", "mǎj"},  // New
            {"ทอง", "tʰɔ̄ːŋ"}, // Gold
            {"น้อย","nɔ́ːj"}, // Small
            // {"ใน", "Nai"},  // within
            // less common ones
            //{ "สามัคคี", "Samakkhi" },  // Harmonious
            //{ "ใหม่พัฒนา", "Mai Phatthana"}, // New Development
            //{"ตะวันออก", "Tawan Ok"}, // East
            //{"ตะวันตก", "Tawan Tok"}, // West
            //{"ออก", "Ok"}, // up
            //{"ตก", "Tok"}, // down
            //{"สอง", "Song"},  // second
            //{"กลาง", "Klang"}, // Middle
            //{"คำ", "Kham"},  // Word
            {"ใหญ่", "jàj"}, // Large
            //{"เล็ก","Lek"},  // small
            //{"เก่า", "Kao"}, // Old
            //{"สันติสุข", "Santi Suk"},  // peace
            {"เจริญ", "tɕā.rɤ̄ːn"},  // growth
            //{"ศรีเจริญ", "Si Charoen"},
            //{"เจริญสุข","Charoen Suk"},
            //{"บูรพา", "Burapha"},  // East
            //{"สวรรค์", "Sawan"}, // Heaven
            {"หลวง", "lǔaŋ"},  // Big
            //{"งาม", "Ngam"},     // Beautiful
            //{"สมบูรณ์", "Sombun"}, // Complete
            //{"สะอาด", "Sa-at"},  // clean
            //{"นอก","Nok"},  // outside
            {"แดง","dɛ̄ːŋ"},  // red
            //{"ดง","Dong"},
            //{"ไร","Rai"},  // Gain
            {"ราษฏร์","râːt"}, // people
            //{"อรุณ","Arun"},  // dawn
            //{"เรือ", "Ruea"},  // boat
            //{"เฒ่า", "Thao"},  // old
            //{"ยืน", "Yuen"},  // durable
            //{"ยาง","Yang"},  // Rubber
            //{"บน", "Bon"},  // upon
            //{"อุดม", "Udom"},  // rich
            //{"เดิม","Doem"},  // old
            //{"บำรุง","Bamrung"}, // administrate
            //{"เตียน","Tian"},
            //{"เหลี่ยม","Liam"},
            //{"คีรี","Khiri"},
            //{"เด่น","Den"},  // notable
            //{"สำนัก","Samnak"},  // office
            //{"มงคล","Mongkhon"},  // dragon
            //{"ศิริ","Siri"},
            //{"ถาวร","Thawon"},  // permanent
            {"นคร", "náʔkʰɔ̄ːn"},  // city
            {"สูง","sǔːŋ"},  // high
            //{"ต่ำ","Tam"},  // low
            //{"หัก","Hak"}, // less
            //{"หนึ่ง","Nueng"},  // first, one
            {"ยาว","jāːw"},  // Long
            //{"รุ่งเรือง","Rung Rueang"},  // prosperous
            //{"สำโรง","Samrong"},  // Sterculia foetida L.
            {"นิคม","ní.kʰōm"},  // plantation
            {"บุรี","būrīː"},  // town
            {"วัฒนา", "wát.tʰā.nāː"}  // development
        };

        /// <summary>
        /// Common prefixes for Thai names with their standard romanization.
        /// </summary>
        public static Dictionary<String, String> NamePrefixRomanizations = new Dictionary<String, String>()
        {
            {"ปากคลอง", "Pak Khlong"},  // Mouth of Canal
            {"คลอง", "Khlong"},  // Canal
            {"น้ำ","Nam"},  // Water (river)
            {"ปากน้ำ","Pak Nam"},  // River mouth
            {"แม่","Mae"},  // Mother
            {"วัง","Wang"},  // Palace
            {"หนอง","Nong"},  // Swamp
            {"หัว","Hua"},  // Head
            {"ตลาด","Talat"},  // Market
            {"ห้วย", "Huai"},  // Creek
            {"ดอน","Don"},  // Hill
            {"แหลม","Laem"},  // Cape
            {"ท่า","Tha"},  // position
            {"โคก","Khok"},  // mound
            {"บาง","Bang"},  // village
            {"นา","Na"},  // field
            {"ลาด","Lat"},  // slope
            {"ไผ่","Phai"},  // Bamboo
            {"วัด","Wat"},  // Temple
            {"พระ","Phra"}, // holy
            {"ศรี","Si"},
            {"โนน","Non"},
            {"โพธิ์","Pho"},
            {"หอม","Hom"},  // good smell
            {"บึง","Bueng"},  // swamp
            {"หลัก","Lak"},  // pillar
            {"ปาก","Pak"},  // mouth
            {"เกาะ","Ko"},  // Island
            {"ป่า","Pa"},  // forest
            {"มาบ","Map"},  // marshland
            {"อ่าง","Ang"},  // Basin
            {"หาด","Hat"},  // Beach
            {"สวน","Suan"},  // Garden
            {"อ่าว","Ao"},  // Bay
            {"ถ้ำ","Tham"},  // cave
            {"ดอย","Doi"},  // mountain
            {"ภู","Phu"},  // Hill
            {"ซับ","Sap"},  // absorb
            {"สัน","San"},  // crest
            {"โป่ง","Pong"},   // large
            {"ต้น","Ton"},   // Beginning
            {"เชียง","Chiang"}, //
            {"เหล่า","Lao"},
            {"ชัย","Chai"},
            {"โพรง","Phrong"},
            {"บ้าน","Ban"},  // house
            {"หมู่บ้าน","Mu Ban"},  // village
            {"คุย","Khui"},  // talk
            {"ตรอก","Trok"},  // lane
            {"หมื่น","Muen"},   // ten thousand
            {"บุ่ง","Bung"},  // Marsh
            {"กุด","Kut"},
            {"บัว","Bua"},  // Lotus
            {"ควน","Khuan"},
            {"หลัง","Lang"}, // Behind
            {"ถนน","Thanon"}, // Street
            {"ดวง","Duang"},  // disc
            {"ย่อย","Yoi"}, // minor, subordinate
            {"ซอย","Soi"},  // side-street
            {"เมือง","Mueang"}  // town
        };

        /// <summary>
        /// Common prefixes for Thai names with their pronunciation in IPA.
        /// </summary>
        public static Dictionary<String, String> NamePrefixIpa = new Dictionary<String, String>()
        {
            {"ปากคลอง", "pàːk kʰlɔ̄ːŋ"},  // Mouth of Canal
            {"คลอง", "kʰlɔ̄ːŋ"},  // Canal
            {"น้ำ","náːm"},  // Water (river)
            {"ปากน้ำ","pàːk náːm"},  // River mouth
            {"แม่","mɛ̂ː"},  // Mother
            {"วัง","wāŋ"},  // Palace
            {"หนอง","nɔ̌ːŋ"},  // Swamp
            //{"หัว","Hua"},  // Head
            //{"ตลาด","Talat"},  // Market
            {"ห้วย", "hûaj"},  // Creek
            {"ดอน","dɔ̄ːn"},  // Hill
            //{"แหลม","Laem"},  // Cape
            //{"ท่า","Tha"},  // position
            {"โคก","kʰôːk"},  // mound
            {"บาง","bāːŋ"},  // village
            {"นา","nāː"},  // field
            {"ลาด","lâːt"},  // slope
            //{"ไผ่","Phai"},  // Bamboo
            //{"วัด","Wat"},  // Temple
            {"พระ","pʰráʔ"}, // holy
            {"ศรี","sǐː"},
            //{"โนน","Non"},
            //{"โพธิ์","Pho"},
            //{"หอม","Hom"},  // good smell
            {"บึง","bɯ̄ŋ"},  // swamp
            {"หลัก","làk"},  // pillar
            {"ปาก","pàːk"},  // mouth
            {"เกาะ","kɔ̀ʔ"},  // Island
            {"ป่า","pàː"},  // forest
            //{"มาบ","Map"},  // marshland
            //{"อ่าง","Ang"},  // Basin
            //{"หาด","Hat"},  // Beach
            {"สวน","sǔan"},  // Garden
            //{"อ่าว","Ao"},  // Bay
            //{"ถ้ำ","Tham"},  // cave
            //{"ดอย","Doi"},  // mountain
            {"ภู","pʰūː"},  // Hill
            //{"ซับ","Sap"},  // absorb
            //{"สัน","San"},  // crest
            //{"โป่ง","Pong"},   // large
            //{"ต้น","Ton"},   // Beginning
            {"เชียง","tɕʰīaŋ"}, //
            //{"เหล่า","Lao"},
            //{"ชัย","Chai"},
            //{"โพรง","Phrong"},
            {"บ้าน","bâːn"},  // house
            //{"หมู่บ้าน","Mu Ban"},  // village
            //{"คุย","Khui"},  // talk
            //{"ตรอก","Trok"},  // lane
            //{"หมื่น","Muen"},   // ten thousand
            //{"บุ่ง","Bung"},  // Marsh
            //{"กุด","Kut"},
            {"บัว","būa"},  // Lotus
            //{"ควน","Khuan"},
            //{"หลัง","Lang"}, // Behind
            //{"ถนน","Thanon"}, // Street
            //{"ดวง","Duang"},  // disc
            //{"ย่อย","Yoi"}, // minor, subordinate
            //{"ซอย","Soi"},  // side-street
            {"เมือง","mɯ̄aŋ"}  // town
        };
    }
}