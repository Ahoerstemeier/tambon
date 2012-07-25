using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace De.AHoerstemeier.Tambon
{
    public partial class NumeralsTambonHelper : Form
    {
        public NumeralsTambonHelper()
        {
            InitializeComponent();
        }

        private void btnDoConvert_Click(object sender, EventArgs e)
        {
            String value = boxText.Text;
            value = TambonHelper.ReplaceThaiNumerals(value);
            boxText.Text = value;
        }

        private Dictionary<String, String> _MacPDFFixupSoSuea = new Dictionary<string, string>()
        {
              {" ะ","สะ"},  
              {" "+Convert.ToChar(0x0E31),"ส"+Convert.ToChar(0x0E31)},  // อั
              {" า","สา"},
              {" "+Convert.ToChar(0x0E33),"ส"+Convert.ToChar(0x0E33)},  // อำ
              {" "+Convert.ToChar(0x0E34),"ส"+Convert.ToChar(0x0E34)},  // อิ
              {" "+Convert.ToChar(0x0E35),"ส"+Convert.ToChar(0x0E35)},  // อี
              {" "+Convert.ToChar(0x0E36),"ส"+Convert.ToChar(0x0E36)},  // อึ
              {" "+Convert.ToChar(0x0E37),"ส"+Convert.ToChar(0x0E37)},  // อื
              {" "+Convert.ToChar(0x0E38),"ส"+Convert.ToChar(0x0E38)},  // อุ
              {" "+Convert.ToChar(0x0E39),"ส"+Convert.ToChar(0x0E39)},  // อู

              {"เ ","เส"},
              {"แ ","แส"},
              {"โ ","โส"},
              {"ใ ","ใส"},
              {"ไ ","ไส"},
              // tone marks
              {" "+Convert.ToChar(0x0E47),"ส"+Convert.ToChar(0x0E47)}, // อ็
              {" "+Convert.ToChar(0x0E48),"ส"+Convert.ToChar(0x0E48)}, // อ่
              {" "+Convert.ToChar(0x0E49),"ส"+Convert.ToChar(0x0E49)}, // อ้ 
              {" "+Convert.ToChar(0x0E4A),"ส"+Convert.ToChar(0x0E4A)}, // อ๊ 
              {" "+Convert.ToChar(0x0E4B),"ส"+Convert.ToChar(0x0E4B)}, // อ๋ 
              {" "+Convert.ToChar(0x0E4C),"ส"+Convert.ToChar(0x0E4C)}  // อ์
        };
        private Dictionary<Char, Char> _BrokenPDFEncoding = new Dictionary<Char, Char>()
        {
              {Convert.ToChar(0xF702),Convert.ToChar(0x0E35)},  // อี

              {Convert.ToChar(0xF705),Convert.ToChar(0x0E48)},  // อ่
              {Convert.ToChar(0xF706),Convert.ToChar(0x0E49)},  // อ้

              {Convert.ToChar(0xF708),Convert.ToChar(0x0E4B)},  // อ๋

              {Convert.ToChar(0xF70A),Convert.ToChar(0x0E48)},  // อ่
              {Convert.ToChar(0xF70B),Convert.ToChar(0x0E49)},  // อ้
              {Convert.ToChar(0xF70C),Convert.ToChar(0x0E4A)},  // อ๊

              {Convert.ToChar(0xF70E),Convert.ToChar(0x0E4C)},  // อ์

              {Convert.ToChar(0xF710),Convert.ToChar(0x0E31)},  // อั

              {Convert.ToChar(0xF712),Convert.ToChar(0x0E47)},  // อ็
              {Convert.ToChar(0xF713),Convert.ToChar(0x0E48)}   // อ่
        };
        private Dictionary<Char, Char> _MacPDFEncoding = new Dictionary<Char, Char>()
            {
              // Letters: กขฃคฅฆงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรฤลฦวศษสหฬอฮ
              {'°','ก'},
              {'¢','ข'},
              // ฃ
              {'§','ค'},
              // ฅ
              {'¶','ฆ'},
              {'ß','ง'},
              {'®','จ'},
              {'©','ฉ'},
              {'™','ช'},
              {'´','ซ'},
              // ฌ
              {'≠','ญ'},
              {'Æ','ฎ'},
              {'Ø','ฏ'},
              {'∞','ฐ'},
              {'±','ฑ'},
              {'≤','ฒ'},
              {'≥','ณ'},
              {'¥','ด'},
              {'μ','ต'},
              {'∂','ถ'},
              {'Σ','ท'},              
              {'Π','ธ'},
              {'π','น'},
              {'∫','บ'},
              {'ª','ป'},
              {'º','ผ'},
              {'Ω','ฝ'},
              {'æ','พ'},
              {'ø','ฟ'},
              {'¿','ภ'},
              {'¡','ม'},
              {'¬','ย'},
              {'√','ร'},
              {'ƒ','ฤ'},
              {'≈','ล'},
              // ฦ
              {'«','ว'},
              {'»','ศ'},
              {'…','ษ'},
              {Convert.ToChar(0x00A0),'ส'},  // This one becomes 0x20 when copying it via clipboard
              {'À','ห'},
              {'Ã','ฬ'},
              {'Õ','อ'},
              {'Œ','ฮ'},  // actually it becomes OE with clipboard
              
              // vocals
              {'–','ะ'},  
              {'—',Convert.ToChar(0x0E31)}, // อั
              {'í',Convert.ToChar(0x0E31)}, // อั (yes, again)
              {'“','า'},
              {'”',Convert.ToChar(0x0E33)},  // อำ
              {'‘',Convert.ToChar(0x0E34)},  // อิ
              {'î',Convert.ToChar(0x0E34)},  // อิ (yes, again)
              {'’',Convert.ToChar(0x0E35)},  // อี
              {'ï',Convert.ToChar(0x0E35)},  // อี (yes, again)
              {'÷',Convert.ToChar(0x0E36)},  // อึ
              {'ñ',Convert.ToChar(0x0E36)},  // อึ (yes, again)
              {'◊',Convert.ToChar(0x0E37)},  // อื
              {'ó',Convert.ToChar(0x0E37)},  // อื
              {'ÿ',Convert.ToChar(0x0E38)},  // อุ
              {'Ÿ',Convert.ToChar(0x0E39)},  // อู

              {'‡','เ'},
              {'·','แ'},
              {'‚','โ'},
              {'„','ใ'},
              {'‰','ไ'},
              // tone marks
              {'ì',Convert.ToChar(0x0E47)}, // อ็
              {'Á',Convert.ToChar(0x0E47)}, //็  อ็ (yes, again)
              {'Ë',Convert.ToChar(0x0E48)}, // อ่
              {'à',Convert.ToChar(0x0E48)}, // อ่ (yes, again)
              {'É',Convert.ToChar(0x0E48)}, // อ่ (yes, again)
              {'ò',Convert.ToChar(0x0E48)}, // อ่ (yes, again)
              {'È',Convert.ToChar(0x0E49)}, // อ้ 
              {'ô',Convert.ToChar(0x0E49)}, // อ้ (yes, again)
              {'â',Convert.ToChar(0x0E49)}, // อ้ (yes, again)
              {'Ñ',Convert.ToChar(0x0E49)}, // อ้ (yes, again)
              {'ä',Convert.ToChar(0x0E4A)}, // อ๊ 
              {'Ö',Convert.ToChar(0x0E4A)}, // อ๊ (yes, again)
              {'Í',Convert.ToChar(0x0E4A)}, // อ๊ (yes, again)
              {'Ü',Convert.ToChar(0x0E4B)}, // อ๋ 
              {'ã',Convert.ToChar(0x0E4B)}, // อ๋ (yes, again)
              {'Î',Convert.ToChar(0x0E4B)}, // อ๋ (yes, again)
              {'õ',Convert.ToChar(0x0E4B)}, // อ๋ (yes, again)
              {'å',Convert.ToChar(0x0E4C)}, // อ์
              {'Ï',Convert.ToChar(0x0E4C)}, // อ์ (yes, again)
              {'á',Convert.ToChar(0x0E4C)}, // อ์ (yes, again)
              // numerals
              {'','๐'},
              {'Ò','๑'},
              {'Ú','๒'},
              {'Û','๓'},
              {'Ù','๔'},
              {'ı','๕'},
              {'ˆ','๖'},
              {'˜','๗'},
              {'¯','๘'},
              {'˘','๙'},

              // special characters
              {'Ê','ๆ'},
              
              // non-Thai characters
              {'ç','“'},
              {'é','”'}
            };

        private void btnEncoding_Click(object sender, EventArgs e)
        {
            StringBuilder value = new StringBuilder(boxText.Text);
            foreach (KeyValuePair<Char, Char> lKeyValuePair in _MacPDFEncoding)
            {
                value = value.Replace(lKeyValuePair.Key,lKeyValuePair.Value);
            }
            value = value.Replace("OE", "ฮ");
            foreach (KeyValuePair<String,String> lKeyValuePair in _MacPDFFixupSoSuea)
            {
                value = value.Replace(lKeyValuePair.Key, lKeyValuePair.Value);
            }

            foreach (KeyValuePair<Char, Char> lKeyValuePair in _BrokenPDFEncoding)
            {
                value = value.Replace(lKeyValuePair.Key, lKeyValuePair.Value);
            }
            
            boxText.Text = value.ToString();
        }

        private void btnMonths_Click(object sender, EventArgs e)
        {
            StringBuilder value = new StringBuilder(boxText.Text);
            foreach (KeyValuePair<String,Byte> monthNameThai in TambonHelper.ThaiMonthNames)
            {
                DateTime month = new DateTime(2000, monthNameThai.Value, 1);
                String monthNameLocal = month.ToString("MMMM");
                value = value.Replace(monthNameThai.Key,monthNameLocal);
            }
            foreach (KeyValuePair<String, Byte> monthAbbreviation in TambonHelper.ThaiMonthAbbreviations)
            {
                DateTime month = new DateTime(2000, monthAbbreviation.Value, 1);
                String monthNameLocal = month.ToString("MMMM");
                value = value.Replace(monthAbbreviation.Key, monthNameLocal);
            }


            foreach (var subString in value.ToString().Split(new String[] {" ", Environment.NewLine, "\t"},StringSplitOptions.None))
            {
                if (TambonHelper.IsNumeric(subString))
                {
                    Int64 number = Convert.ToInt64(subString);
                    if ((number > 2400) && (number < 2600))
                    {
                        number -= 543;
                        value.Replace(subString, number.ToString());
                    }
                }
            }

            boxText.Text = value.ToString();
        }

        private void btnTitles_Click(object sender, EventArgs e)
        {
            StringBuilder value = new StringBuilder(boxText.Text);
            foreach (KeyValuePair<String, PersonTitle> lKeyValuePair in TambonHelper.PersonTitleStrings)
            {
                value = value.Replace(lKeyValuePair.Key, lKeyValuePair.Value.ToString()+" ");
            }
            boxText.Text = value.ToString();
        }

        private void btnInvert_Click(object sender, EventArgs e)
        {
            String value = boxText.Text;
            StringBuilder builder = new StringBuilder();
            foreach (String subString in value.Split('\n'))
            {
                builder.Insert(0, subString+'\n');
            };
            boxText.Text = builder.ToString();
        }
    }
}
