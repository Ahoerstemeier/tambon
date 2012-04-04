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
            foreach ( PopulationDataEntry lEntry in TambonHelper.ProvinceGeocodes )
            {
                cbxProvince.Items.Add(lEntry);
                if ( lEntry.Geocode == TambonHelper.PreferredProvinceGeocode )
                {
                    cbxProvince.SelectedItem = lEntry;
                }
            }

            cbxRegion.Items.Clear();
            foreach ( String lRegionScheme in TambonHelper.RegionSchemes() )
            {
                cbxRegion.Items.Add(lRegionScheme);
            }
            if ( cbxRegion.Items.Count > 0 )
            {
                cbxRegion.SelectedIndex = 0;
            }
        }

        private PopulationDataEntry GetPopulationDataFromCache(Int32 iYear)
        {
            PopulationDataEntry lData = null;
            if ( mDownloadedData.Keys.Contains(iYear) )
            {
                lData = mDownloadedData[iYear];
                lData = (PopulationDataEntry)lData.Clone();
            }
            return lData;
        }
        private PopulationDataEntry GetPopulationData(Int32 iYear)
        {
            PopulationDataEntry lData = GetPopulationDataFromCache(iYear);
            if ( lData == null )
            {
                PopulationData lDownloader = new PopulationData(iYear, 0);
                lDownloader.Process();
                lData = lDownloader.Data;
                StorePopulationDataToCache(lData, iYear);
            }
            return lData;
        }

        private void StorePopulationDataToCache(PopulationDataEntry iData, Int32 iYear)
        {
            mDownloadedData[iYear] = (PopulationDataEntry)iData.Clone();
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            Int32 lYear = Convert.ToInt32(edtYear.Value);
            Int32 lNumberOfConstituencies = Convert.ToInt32(edtNumberOfConstituencies.Value);
            Int32 lGeocode = 0;
            PopulationDataEntry lData = null;
            if ( rbxNational.Checked )
            {
                lData = GetPopulationData(lYear);
            }
            if ( rbxProvince.Checked )
            {
                var lProvince = (PopulationDataEntry)cbxProvince.SelectedItem;
                lGeocode = lProvince.Geocode;
                PopulationData lDownloader = new PopulationData(lYear, lGeocode);
                lDownloader.Process();

                lData = lDownloader.Data;
            }

            if ( rbxNational.Checked && chkBuengKan.Checked )
            {
                ModifyPopulationDataForBuengKan(lData);
            }

            Dictionary<PopulationDataEntry, Int32> lResult = ConstituencyCalculator.Calculate(lData, lYear, lNumberOfConstituencies);

            if ( chkRegions.Checked )
            {
                List<PopulationDataEntry> lRegions = TambonHelper.GetRegionBySchemeName(cbxRegion.Text);
                Dictionary<PopulationDataEntry, Int32> lRegionResult = new Dictionary<PopulationDataEntry, Int32>();
                foreach ( PopulationDataEntry lRegion in lRegions )
                {
                    Int32 lConstituencies = 0;
                    List<PopulationDataEntry> lSub = new List<PopulationDataEntry>();
                    foreach ( PopulationDataEntry lProvince in lRegion.SubEntities )
                    {
                        PopulationDataEntry lFound = lData.FindByCode(lProvince.Geocode);
                        if ( lFound != null )
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
            foreach ( KeyValuePair<PopulationDataEntry, Int32> lEntry in lResult )
            {
                Int32 lVotersPerSeat = 0;
                if ( lEntry.Value != 0 )
                {
                    lVotersPerSeat = lEntry.Key.Total / lEntry.Value;
                }
                lDisplayResult = lDisplayResult + lEntry.Key.English + " " + lEntry.Value.ToString() + " (" + lVotersPerSeat.ToString() + " per seat)" + Environment.NewLine;
            }
            txtData.Text = lDisplayResult;
            mLastCalculation = lResult;
            btnSaveCsv.Enabled = true;
        }

        private static void ModifyPopulationDataForBuengKan(PopulationDataEntry lData)
        {
            PopulationDataEntry lBuengKan = new PopulationDataEntry();
            lBuengKan.English = "Bueng Kan";
            lBuengKan.Geocode = 38;
            List<Int32> lBuengKanAmphoeCodes = new List<int>() { 4313, 4311, 4309, 4312, 4303, 4306, 4310, 4304 };
            PopulationDataEntry lNongKhai = lData.FindByCode(43);
            foreach ( Int32 lCode in lBuengKanAmphoeCodes )
            {
                PopulationDataEntry lEntry = lNongKhai.FindByCode(lCode);
                lBuengKan.SubEntities.Add(lEntry);
                lNongKhai.SubEntities.Remove(lEntry);
            }
            lNongKhai.CalculateNumbersFromSubEntities();
            lBuengKan.CalculateNumbersFromSubEntities();
            lData.SubEntities.Add(lBuengKan);
            lData.CalculateNumbersFromSubEntities();
            lData.SortSubEntitiesByGeocode();
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

            foreach ( KeyValuePair<PopulationDataEntry, Int32> lEntry in mLastCalculation )
            {
                Int32 lVotersPerSeat = 0;
                if ( lEntry.Value != 0 )
                {
                    lVotersPerSeat = lEntry.Key.Total / lEntry.Value;
                }

                lBuilder.AppendLine(lEntry.Key.English + "," + lEntry.Value.ToString() + "," + lVotersPerSeat.ToString());
            }

            SaveFileDialog lDlg = new SaveFileDialog();
            lDlg.Filter = "CSV Files|*.csv|All files|*.*";
            if ( lDlg.ShowDialog() == DialogResult.OK )
            {
                Stream lFileStream = new FileStream(lDlg.FileName, FileMode.CreateNew);
                StreamWriter lWriter = new StreamWriter(lFileStream);
                lWriter.Write(lBuilder.ToString());
                lWriter.Close();
            }

        }

        private void btnLoadConstituencyXml_Click(object sender, EventArgs e)
        {
            OpenFileDialog lDlg = new OpenFileDialog();
            lDlg.Filter = "XML Files|*.xml|All files|*.*";
            if ( lDlg.ShowDialog() == DialogResult.OK )
            {
                PopulationData lData = null;
                StreamReader lReader = null;
                try
                {

                    lReader = new StreamReader(lDlg.FileName);
                    XmlDocument lXmlDoc = new XmlDocument();
                    lXmlDoc.LoadXml(lReader.ReadToEnd());
                    foreach ( XmlNode lNode in lXmlDoc.ChildNodes )
                    {
                        if ( lNode.Name == "electiondata" )
                        {
                            lData = PopulationData.Load(lNode);
                        }
                    }
                }
                finally
                {
                    if ( lReader != null )
                    {
                        lReader.Dispose();
                    }
                }
                if ( (lData != null) && (lData.Data != null) )
                {
                    Int32 lYear = Convert.ToInt32(edtYear.Value);
                    PopulationDataEntry lDataEntry = GetPopulationData(lYear);
                    if ( rbxNational.Checked && chkBuengKan.Checked )
                    {
                        ModifyPopulationDataForBuengKan(lDataEntry);
                    }
                    var lList = lData.Data.FlatList(new List<EntityType>() { EntityType.Bangkok, EntityType.Changwat, EntityType.Amphoe, EntityType.KingAmphoe, EntityType.Khet });
                    foreach ( PopulationDataEntry lEntry in lList )
                    {
                        lEntry.CopyPopulationToConstituencies(lDataEntry);
                    }

                    ConstituencyStatisticsViewer lDialog = new ConstituencyStatisticsViewer(lData.Data);
                    lDialog.Show();
                }
            }
        }


    }
}

