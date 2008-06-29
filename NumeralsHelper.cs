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

        private static Dictionary<Char, Char> mOldPDFEncoding = new Dictionary<Char, Char>()
            {
              {'°','ก'},
              {'¢','ข'},
              // Kho Khwai ค
              {'ß','ง'},
              {'®','จ'},
              // vocals
              {'·','แ'},
              {'‚','โ'},
              {'„','ใ'},
              {'‡','เ'},
              {'“','า'},
              {'–','ะ'},    
              // numerals
              {'Ò','๑'},
              {'Ú','๒'},
              {'Û','๓'},
              {'Ù','๔'},
              {'ı','๕'},
              {'ˆ','๖'},
              {'˜','๗'},
              {'¯','๘'},
              {'˘','๙'},

              {'ª','ป'},
              {'∫','บ'},
              {'æ','พ'},
              {'π','น'},
              {'μ','ต'},
              {'À','ห'},
{'¥','ด'},
{'¬','ย'},
{'Σ','ท'},

              {'«','ว'},
              {'¡','ม'},
              {'≈','ล'},
              {'Õ','อ'},
              {'»','ศ'},
{'√','ร'}
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
