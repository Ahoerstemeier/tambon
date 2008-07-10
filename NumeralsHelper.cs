using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// TODO: Konvertierung des Fonts aus den aelteren PDFs in normales Encoding

namespace De.AHoerstemeier.Tambon
{
    public partial class NumeralsHelper : Form
    {
        public NumeralsHelper()
        {
            InitializeComponent();
        }

        private void NumeralsHelper_Load(object sender, EventArgs e)
        {

        }

        private void btnDoConvert_Click(object sender, EventArgs e)
        {
            String lValue = boxText.Text;
            lValue = Helper.ReplaceThaiNumerals(lValue);
            boxText.Text = lValue;
        }

        private Dictionary<Char, Char> mOldPDFEncoding = new Dictionary<Char, Char>()
            {
              // Letters: กขฃคฅฆงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรฤลฦวศษสหฬอฮ
              {'°','ก'},
              {'¢','ข'},
              // ฃ
              {'§','ค'},
              // ฅฆ
              {'ß','ง'},
              {'®','จ'},
              {'©','ฉ'},
              {'™','ช'},
              {'´','ซ'},
              {'≥','ฌ'},
              {'≠','ญ'},
              {'Æ','ฎ'},
              //ฏ
              {'∞','ฐ'},
              // ฑ
              {'≤','ฒ'},
              // ณ
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
              // ฤ
              {'≈','ล'},
              // ฦ
              {'«','ว'},
              {'»','ศ'},
              {'…','ษ'},
              {Convert.ToChar(0x00A0),'ส'},  // This one becomes 0x20 when copying it via clipboard
              {'À','ห'},
              // ฬ
              {'Õ','อ'},
              // ฮ
              
              // vocals
              {'–','ะ'},  
              {'—',Convert.ToChar(0x0E31)}, // อั
              {'í',Convert.ToChar(0x0E31)}, // อั (yes, again)
              {'“','า'},
              {'”',Convert.ToChar(0x0E33)},  // อำ
              {'‘',Convert.ToChar(0x0E34)},  // อิ
              {'’',Convert.ToChar(0x0E35)},  // อี
              {'ï',Convert.ToChar(0x0E35)},  // อี (yes, again)
              {'÷',Convert.ToChar(0x0E36)},  // อึ
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
              {'Ë',Convert.ToChar(0x0E48)}, // อ่
              {'à',Convert.ToChar(0x0E48)}, // อ่ (yes, again)
              {'É',Convert.ToChar(0x0E48)}, // อ่ (yes, again)
              {'ò',Convert.ToChar(0x0E48)}, // อ่ (yes, again)
              {'È',Convert.ToChar(0x0E49)}, // อ้ 
              {'ô',Convert.ToChar(0x0E49)}, // อ้ (yes, again)
              {'â',Convert.ToChar(0x0E49)}, // อ้ (yes, again)
              {'Ñ',Convert.ToChar(0x0E49)}, // อ้ (yes, again)
              {'ì',Convert.ToChar(0x0E4A)}, // อ๊
              {'ä',Convert.ToChar(0x0E4A)}, // อ๊ (yes, again)
              {'Ö',Convert.ToChar(0x0E4A)}, // อ๊ (yes, again)
              {'Ü',Convert.ToChar(0x0E4B)}, // อ๋ 
              {'å',Convert.ToChar(0x0E4C)}, // อ์
              {'Ï',Convert.ToChar(0x0E4C)}, // อ์
              
              {'Á',Convert.ToChar(0x0E47)}, //็อ็
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
              {'˘','๙'}
              
            };

        private void button1_Click(object sender, EventArgs e)
        {
            String lValue = boxText.Text;
            foreach (KeyValuePair<Char, Char> lKeyValuePair in mOldPDFEncoding)
            {
                lValue = lValue.Replace(lKeyValuePair.Key,lKeyValuePair.Value);
            }
            boxText.Text = lValue;
        }
    }
}
