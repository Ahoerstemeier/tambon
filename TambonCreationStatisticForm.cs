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
    public partial class TambonCreationStatisticForm : Form
    {
        public TambonCreationStatisticForm()
        {
            InitializeComponent();
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            TambonCreationStatistics lTambonStatistics = new TambonCreationStatistics((Int32)edtYearStart.Value, (Int32)edtYearEnd.Value);
            lTambonStatistics.Calculate();

            edtData.Text = lTambonStatistics.Information();
        }
    }
}
