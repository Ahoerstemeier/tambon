using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class ThaiAddress
    {
        private Int32 mGeocode = 0;
        public String Changwat { get; set; }
        public String Amphoe { get; set; }
        public String Tambon { get; set; }
        public Int32 Muban { get; set; }
        public Int32 PostalCode { get; set; }
        public Int32 Geocode { get { return mGeocode; } }
        public String Street { get; set; }
        public String PlainValue { get; set; }

        const String SearchKeyMuban = "หมู่ ";
        const String SearchKeyMubanAlternative = "หมู่ที่";
        const String SearchKeyTambon = "ต.";
        const String SearchKeyPostalCode = "รหัสไปรษณีย์";

        internal void WriteToXmlElement(XmlElement iElement)
        {
            // TODO
        }
        private String TextAfter(String iValue, String iSearch)
        {
            String lTemp = lTemp = iValue.Substring(iValue.IndexOf(iSearch)+iSearch.Length).Trim();
            if (lTemp.Contains(' '))
            {
                lTemp = lTemp.Substring(0,lTemp.IndexOf(' '));
            }
            return lTemp;
        }
        internal void ParseString(String iValue)
        {
            PlainValue = iValue;
            // In kontessaban: ถ.ราษฎร์ดำรง หมู่ 4 ต.ปาย อำเภอ ปาย จังหวัด แม่ฮ่องสอน รหัสไปรษณีย์ 58130
            // in amphoe.com verschiedene: 
            // ถนนเพชรเกษม หมู่ที่ 7 ต.โคกโพธิ์ อ.โคกโพธิ์ จ.ปัตตานี
            // หมู่ที่ 1 ต.ตุยง อ.หนองจิก จ.ปัตตานี
            // aber auch unsinniges wie
            // ที่ว่าการอำเภอ เมืองปัตตานี จังหวัดปัตตานี oder gar ที่ว่าการอำเภอมายอ
            // TODO
            if (iValue.Contains(SearchKeyMuban))
            {
                String lTemp = iValue.Replace(SearchKeyMubanAlternative, SearchKeyMuban);
                lTemp = TextAfter(lTemp, SearchKeyMuban);
                lTemp = Helper.OnlyNumbers(lTemp);
                if (!String.IsNullOrEmpty(lTemp))
                {
                    Muban = Convert.ToInt32(lTemp);
                }
            }
            if (iValue.Contains(Helper.EntityNames[EntityType.Changwat]))
            {
                Changwat = TextAfter(iValue, Helper.EntityNames[EntityType.Changwat]);
            }
            if (iValue.Contains(Helper.EntityNames[EntityType.Amphoe]))
            {
                Amphoe = TextAfter(iValue, Helper.EntityNames[EntityType.Amphoe]);
            }
            if (iValue.Contains(SearchKeyTambon))
            {
                Tambon = TextAfter(iValue, SearchKeyTambon);
            }
            if (iValue.Contains(SearchKeyPostalCode))
            {
                String lTemp = TextAfter(iValue, SearchKeyPostalCode);
                lTemp = Helper.OnlyNumbers(lTemp);
                if (!String.IsNullOrEmpty(lTemp))
                {
                    PostalCode = Convert.ToInt32(lTemp);
                }            
            }
        }
        internal void ReadFromXml(XmlNode iNode)
        {
            // TODO
        }

        internal void CalcGeocode()
        {
            // Tambon eigentlich auch
            mGeocode = Helper.GetGeocode(Changwat, Amphoe, EntityType.Amphoe);
        }
    }
}
