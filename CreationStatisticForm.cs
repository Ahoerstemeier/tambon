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
                Invoke(new RoyalGazetteList.ProcessingFinished(RoyalGazetteViewer.ShowGazetteDialog), new object[] { lStatistics.StrangeAnnouncements });
            }
        }

        private void DoCalculate(AnnouncementStatistics iStatistics)
        {
            iStatistics.Calculate();

            edtData.Text = iStatistics.Information();
        }
    }
}
