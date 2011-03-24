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

namespace De.AHoerstemeier.Tambon
{
    public partial class ConstituencyForm : Form
    {
        private Dictionary<Int32, PopulationDataEntry> mDownloadedData = new Dictionary<int, PopulationDataEntry>();
        private Dictionary<PopulationDataEntry, Int32> mLastCalculation = null;

        public ConstituencyForm()
        {
            InitializeComponent();
        }

        private void rbxProvince_CheckedChanged(object sender, EventArgs e)
        {
            cbxProvince.Enabled = rbxProvince.Checked;
        }

        private void ConstituencyForm_Load(object sender, EventArgs e)
        {
            edtYear.Maximum = TambonHelper.PopulationStatisticMaxYear;
            edtYear.Value = edtYear.Maximum;

            TambonHelper.LoadGeocodeList();

            cbxProvince.Items.Clear();
            foreach (PopulationDataEntry lEntry in TambonHelper.Geocodes)
            {
                cbxProvince.Items.Add(lEntry);
                if (lEntry.Geocode == TambonHelper.PreferredProvinceGeocode)
                {
                    cbxProvince.SelectedItem = lEntry;
                }
            }

            cbxRegion.Items.Clear();
            foreach (String lRegionScheme in TambonHelper.RegionSchemes())
            {
                cbxRegion.Items.Add(lRegionScheme);
            }
            if (cbxRegion.Items.Count > 0)
            {
                cbxRegion.SelectedIndex = 0;
            }
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            Int32 lYear = Convert.ToInt32(edtYear.Value);
            Int32 lNumberOfConstituencies = Convert.ToInt32(edtNumberOfConstituencies.Value);
            Int32 lGeocode = 0;
            PopulationDataEntry lData = null;
            if (rbxNational.Checked)
            { 
                if (mDownloadedData.Keys.Contains(lYear))
                {
                    lData = mDownloadedData[lYear];
                    lData = (PopulationDataEntry)lData.Clone();
                }
            }
            if (rbxProvince.Checked)
            {
                var lProvince = (PopulationDataEntry)cbxProvince.SelectedItem;
                lGeocode = lProvince.Geocode;
            }
            if (lData == null)
            {
                PopulationData lDownloader = new PopulationData(lYear, lGeocode);
                lDownloader.Process();

                lData = lDownloader.Data;
            }
            if (rbxNational.Checked && (!mDownloadedData.Keys.Contains(lYear)))
            {
                mDownloadedData[lYear] = (PopulationDataEntry)lData.Clone();
            }

            if (rbxNational.Checked && chkBuengKan.Checked)
            {
                PopulationDataEntry lBuengKan = new PopulationDataEntry();
                lBuengKan.English = "Bueng Kan";
                lBuengKan.Geocode = 38;
                List<Int32> lBuengKanAmphoeCodes = new List<int>() { 4313, 4311, 4309, 4312, 4303, 4306, 4310, 4304 };
                PopulationDataEntry lNongKhai = lData.FindByCode(43);
                foreach (Int32 lCode in lBuengKanAmphoeCodes)
                {
                    PopulationDataEntry lEntry = lNongKhai.FindByCode(lCode);
                    lBuengKan.SubEntities.Add(lEntry);
                    lNongKhai.SubEntities.Remove(lEntry);
                }
                lNongKhai.CalculateNumbersFromSubEntities();
                lBuengKan.CalculateNumbersFromSubEntities();
                lData.SubEntities.Add(lBuengKan);
                lData.CalculateNumbersFromSubEntities();
                lData.SortSubEntities();
            }

            Dictionary<PopulationDataEntry, Int32> lResult = ConstituencyCalculator.Calculate(lData, lYear, lNumberOfConstituencies);

            if (chkRegions.Checked)
            {
                List<PopulationDataEntry> lRegions = TambonHelper.GetRegionBySchemeName(cbxRegion.Text);
                Dictionary<PopulationDataEntry, Int32> lRegionResult = new Dictionary<PopulationDataEntry, Int32>();
                foreach (PopulationDataEntry lRegion in lRegions)
                {
                    Int32 lConstituencies = 0;
                    List<PopulationDataEntry> lSub = new List<PopulationDataEntry>();
                    foreach (PopulationDataEntry lProvince in lRegion.SubEntities)
                    {
                        PopulationDataEntry lFound = lData.FindByCode(lProvince.Geocode);
                        if (lFound != null)
                        {
                            lConstituencies = lConstituencies + lResult[lFound];
                            lSub.Add(lFound);
                        }
                    }
                    lRegion.SubEntities.Clear();
                    lRegion.SubEntities.AddRange(lSub);
                    lRegion.CalculateNumbersFromSubEntities();
                    lRegionResult.Add(lRegion, lConstituencies);
                }
                lResult = lRegionResult;
            }

            String lDisplayResult = String.Empty;
            foreach (KeyValuePair<PopulationDataEntry, Int32> lEntry in lResult)
            {
                Int32 lVotersPerSeat = 0;
                if (lEntry.Value != 0)
                {
                    lVotersPerSeat = lEntry.Key.Total / lEntry.Value;
                }
                lDisplayResult = lDisplayResult + lEntry.Key.English + " " + lEntry.Value.ToString() + " (" + lVotersPerSeat.ToString() + " per seat)" + Environment.NewLine;
            }
            txtData.Text = lDisplayResult;
            mLastCalculation = lResult;
            btnSaveCsv.Enabled = true;
        }

        private void rbxNational_CheckedChanged(object sender, EventArgs e)
        {
            chkBuengKan.Enabled = rbxNational.Checked;
            chkRegions.Enabled = rbxNational.Checked;
        }

        private void chkRegions_Changed(object sender, EventArgs e)
        {
            cbxRegion.Enabled = chkRegions.Checked && chkRegions.Enabled;
        }

        private void btnSaveCsv_Click(object sender, EventArgs e)
        {
            Debug.Assert(mLastCalculation != null);

            StringBuilder lBuilder = new StringBuilder();

            foreach (KeyValuePair<PopulationDataEntry, Int32> lEntry in mLastCalculation)
            {
                Int32 lVotersPerSeat = 0;
                if (lEntry.Value != 0)
                {
                    lVotersPerSeat = lEntry.Key.Total / lEntry.Value;
                }

                lBuilder.AppendLine(lEntry.Key.English + "," + lEntry.Value.ToString() + "," + lVotersPerSeat.ToString());
            }

            SaveFileDialog lDlg = new SaveFileDialog();
            lDlg.Filter = "CSV Files|*.csv|All files|*.*";
            if (lDlg.ShowDialog() == DialogResult.OK)
            {
                Stream lFileStream = new FileStream(lDlg.FileName, FileMode.CreateNew);
                StreamWriter lWriter = new StreamWriter(lFileStream);
                lWriter.Write(lBuilder.ToString());
                lWriter.Close();
            }

        }

    }
}

