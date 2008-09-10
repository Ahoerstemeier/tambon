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
    public partial class CreationStatisticForm : Form
    {
        public CreationStatisticForm()
        {
            InitializeComponent();
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            CreationStatisticsTambon lTambonStatistics = new CreationStatisticsTambon((Int32)edtYearStart.Value, (Int32)edtYearEnd.Value);
            lTambonStatistics.Calculate();

            edtData.Text = lTambonStatistics.Information();
        }

        private void btnCalcMuban_Click(object sender, EventArgs e)
        {
            CreationStatisticsMuban lMubanStatistics = new CreationStatisticsMuban((Int32)edtYearStart.Value, (Int32)edtYearEnd.Value);
            lMubanStatistics.Calculate();

            edtData.Text = lMubanStatistics.Information();

        }
    }
}
