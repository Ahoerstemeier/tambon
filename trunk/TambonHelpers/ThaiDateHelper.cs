using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.AHoerstemeier.Tambon
{
    public static class ThaiDateHelper
    {
        public static readonly Dictionary<String, Byte> ThaiMonthNames = new Dictionary<string, byte>
        {
            {"มกราคม",1},
            {"กุมภาพันธ์",2},
            {"มีนาคม",3},
            {"เมษายน",4},
            {"พฤษภาคม",5},
            {"มิถุนายน",6},
            {"กรกฎาคม",7},
            {"สิงหาคม",8},
            {"กันยายน",9},
            {"ตุลาคม",10},
            {"พฤศจิกายน",11},
            {"ธันวาคม",12}
        };

        public static readonly Dictionary<String, Byte> ThaiMonthAbbreviations = new Dictionary<string, byte>
        {
            {"ม.ค.",1},
            {"ก.พ.",2},
            {"มี.ค.",3},
            {"เม.ย.",4},
            {"พ.ค.",5},
            {"มิ.ย.",6},
            {"ก.ค.",7},
            {"สิ.ค.",8},
            {"ส.ค.",8},
            {"ก.ย.",9},
            {"ต.ค.",10},
            {"พ.ย.",11},
            {"ธ.ค.",12}
        };

        private const string BuddhistEra = "พ.ศ.";
        private const string ChristianEra = "ค.ศ.";
        private const string RattanakosinEra = "ร.ศ.";

        public static DateTime ParseThaiDate(String value)
        {
            String monthString = String.Empty;
            Int32 month = 0;
            String yearString = String.Empty;
            Int32 year = 0;
            Int32 day = 0;
            Int32 position = 0;

            String date = ThaiNumeralHelper.ReplaceThaiNumerals(value);

            position = date.IndexOf(' ');
            day = Convert.ToInt32(date.Substring(0, position), CultureInfo.InvariantCulture);
            date = date.Substring(position + 1, date.Length - position - 1);
            position = date.IndexOf(' ');
            monthString = date.Substring(0, position).Trim();
            month = ThaiMonthNames[monthString];
            // TODO: Kamen da nicht auch welche mit KhoSo vor?
            position = date.IndexOf(BuddhistEra, StringComparison.Ordinal) + BuddhistEra.Length;
            yearString = date.Substring(position, date.Length - position);
            year = Convert.ToInt32(yearString, CultureInfo.InvariantCulture);
            if ( year < 2100 )
            {
                year = year + 543;  // there are entries in KhoSo but with "พ.ศ." in the returned info
            }

            if ( (year < 2484) & (month < 4) )
            {
                year = year - 542;
            }
            else
            {
                year = year - 543;
            }
            return new DateTime(year, month, day);
        }
    }
}