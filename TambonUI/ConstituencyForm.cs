using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    public partial class ConstituencyForm : Form
    {
        private Dictionary<Entity, Int32> _lastCalculation = null;

        public ConstituencyForm()
        {
            InitializeComponent();
        }

        private void ConstituencyForm_Load(Object sender, EventArgs e)
        {
            edtYear.Maximum = GlobalData.PopulationStatisticMaxYear;
            edtYear.Minimum = GlobalData.PopulationStatisticMinYear;
            edtYear.Value = edtYear.Maximum;

            var provinces = GlobalData.Provinces;

            cbxProvince.Items.Clear();
            foreach ( var entry in provinces )
            {
                cbxProvince.Items.Add(entry);
                if ( entry.geocode == GlobalData.PreferredProvinceGeocode )
                {
                    cbxProvince.SelectedItem = entry;
                }
            }
        }

        private void btnCalc_Click(Object sender, EventArgs e)
        {
            Int16 year = Convert.ToInt16(edtYear.Value);
            Int16 numberOfConstituencies = Convert.ToInt16(edtNumberOfConstituencies.Value);
            UInt32 geocode = 0;
            var result = ConstituencyCalculator.Calculate(geocode, year, numberOfConstituencies);

            String displayResult = String.Empty;
            foreach ( KeyValuePair<Entity, Int32> entry in result )
            {
                Int32 votersPerSeat = 0;
                if ( entry.Value != 0 )
                {
                    votersPerSeat = entry.Key.GetPopulationDataPoint(PopulationDataSourceType.DOPA, year).total / entry.Value;
                }
                displayResult = displayResult +
                    String.Format("{0} {1} ({2} per seat)", entry.Key.english, entry.Value, votersPerSeat) + Environment.NewLine;
            }
            txtData.Text = displayResult;
            _lastCalculation = result;
            btnSaveCsv.Enabled = true;
        }

        private void btnSaveCsv_Click(Object sender, EventArgs e)
        {
            Debug.Assert(_lastCalculation != null);

            StringBuilder builder = new StringBuilder();
            Int16 year = Convert.ToInt16(edtYear.Value);

            foreach ( KeyValuePair<Entity, Int32> entry in _lastCalculation )
            {
                Int32 votersPerSeat = 0;
                if ( entry.Value != 0 )
                {
                    votersPerSeat = entry.Key.GetPopulationDataPoint(PopulationDataSourceType.DOPA, year).total / entry.Value;
                }

                builder.AppendLine(String.Format("{0},{1},{2}", entry.Key.english, entry.Value, votersPerSeat));
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "CSV Files|*.csv|All files|*.*";
            if ( dlg.ShowDialog() == DialogResult.OK )
            {
                using ( Stream fileStream = new FileStream(dlg.FileName, FileMode.CreateNew) )
                {
                    StreamWriter writer = new StreamWriter(fileStream);
                    writer.Write(builder.ToString());
                    writer.Close();
                };
            }
        }
    }
}