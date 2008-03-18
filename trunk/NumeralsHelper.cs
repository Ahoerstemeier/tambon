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
    }
}
