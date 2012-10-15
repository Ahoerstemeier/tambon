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

        private void btnCalcTambon_Click(object sender, EventArgs e)
        {
            CreationStatisticsTambon lStatistics = new CreationStatisticsTambon((Int32)edtYearStart.Value, (Int32)edtYearEnd.Value);
            DoCalculate(lStatistics);
        }

        private void btnCalcMuban_Click(object sender, EventArgs e)
        {
            CreationStatisticsMuban lStatistics = new CreationStatisticsMuban((Int32)edtYearStart.Value, (Int32)edtYearEnd.Value);
            DoCalculate(lStatistics);
        }

        private void btnDates_Click(object sender, EventArgs e)
        {
            StatisticsAnnouncementDates lStatistics = new StatisticsAnnouncementDates((Int32)edtYearStart.Value, (Int32)edtYearEnd.Value);
            DoCalculate(lStatistics);
            if (lStatistics.StrangeAnnouncements.Count > 0)
            {
                Invoke(new Action(() => RoyalGazetteViewer.ShowGazetteDialog(lStatistics.StrangeAnnouncements,false)));
            }
        }

        private void DoCalculate(AnnouncementStatistics iStatistics)
        {
            iStatistics.Calculate();

            edtData.Text = iStatistics.Information();
        }

        private void btnCalcAmphoe_Click(object sender, EventArgs e)
        {
            CreationStatisticsAmphoe lStatistics = new CreationStatisticsAmphoe((Int32)edtYearStart.Value, (Int32)edtYearEnd.Value);
            DoCalculate(lStatistics);
        }

        private void CreationStatisticForm_Load(object sender, EventArgs e)
        {
            edtYearEnd.Maximum = DateTime.Now.Year;
            edtYearEnd.Value = edtYearEnd.Maximum;
            edtYearStart.Maximum = edtYearEnd.Maximum;
        }
    }
}
