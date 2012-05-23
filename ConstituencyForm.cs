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
        private Dictionary<Int32, PopulationDataEntry> _downloadedData = new Dictionary<int, PopulationDataEntry>();
        private Dictionary<PopulationDataEntry, Int32> _lastCalculation = null;

        public ConstituencyForm()
        {
            InitializeComponent();
        }

        private void rbxProvince_CheckedChanged(Object sender, EventArgs e)
        {
            cbxProvince.Enabled = rbxProvince.Checked;
        }

        private void ConstituencyForm_Load(Object sender, EventArgs e)
        {
            edtYear.Maximum = TambonHelper.PopulationStatisticMaxYear;
            edtYear.Minimum = TambonHelper.PopulationStatisticMinYear;
            edtYear.Value = edtYear.Maximum;

            TambonHelper.LoadGeocodeList();

            cbxProvince.Items.Clear();
            foreach ( PopulationDataEntry entry in TambonHelper.ProvinceGeocodes )
            {
                cbxProvince.Items.Add(entry);
                if ( entry.Geocode == TambonHelper.PreferredProvinceGeocode )
                {
                    cbxProvince.SelectedItem = entry;
                }
            }

            cbxRegion.Items.Clear();
            foreach ( String regionScheme in TambonHelper.RegionSchemes() )
            {
                cbxRegion.Items.Add(regionScheme);
            }
            if ( cbxRegion.Items.Count > 0 )
            {
                cbxRegion.SelectedIndex = 0;
            }
        }

        private PopulationDataEntry GetPopulationDataFromCache(Int32 year)
        {
            PopulationDataEntry data = null;
            if ( _downloadedData.Keys.Contains(year) )
            {
                data = _downloadedData[year];
                data = (PopulationDataEntry)data.Clone();
            }
            return data;
        }
        private PopulationDataEntry GetPopulationData(Int32 year)
        {
            PopulationDataEntry data = GetPopulationDataFromCache(year);
            if ( data == null )
            {
                PopulationData downloader = new PopulationData(year, 0);
                downloader.Process();
                data = downloader.Data;
                StorePopulationDataToCache(data, year);
            }
            return data;
        }

        private void StorePopulationDataToCache(PopulationDataEntry data, Int32 year)
        {
            _downloadedData[year] = (PopulationDataEntry)data.Clone();
        }

        private void btnCalc_Click(Object sender, EventArgs e)
        {
            Int32 year = Convert.ToInt32(edtYear.Value);
            Int32 numberOfConstituencies = Convert.ToInt32(edtNumberOfConstituencies.Value);
            Int32 geocode = 0;
            PopulationDataEntry data = null;
            if ( rbxNational.Checked )
            {
                data = GetPopulationData(year);
            }
            if ( rbxProvince.Checked )
            {
                var province = (PopulationDataEntry)cbxProvince.SelectedItem;
                geocode = province.Geocode;
                PopulationData downloader = new PopulationData(year, geocode);
                downloader.Process();

                data = downloader.Data;
            }

            if ( rbxNational.Checked && chkBuengKan.Checked )
            {
                ModifyPopulationDataForBuengKan(data);
            }
            data.SortSubEntitiesByEnglishName();

            Dictionary<PopulationDataEntry, Int32> result = ConstituencyCalculator.Calculate(data, year, numberOfConstituencies);

            if ( chkRegions.Checked )
            {
                List<PopulationDataEntry> regions = TambonHelper.GetRegionBySchemeName(cbxRegion.Text);
                Dictionary<PopulationDataEntry, Int32> regionResult = new Dictionary<PopulationDataEntry, Int32>();
                foreach ( PopulationDataEntry region in regions )
                {
                    Int32 constituencies = 0;
                    List<PopulationDataEntry> subList = new List<PopulationDataEntry>();
                    foreach ( PopulationDataEntry province in region.SubEntities )
                    {
                        PopulationDataEntry foundEntry = data.FindByCode(province.Geocode);
                        if ( foundEntry != null )
                        {
                            constituencies = constituencies + result[foundEntry];
                            subList.Add(foundEntry);
                        }
                    }
                    region.SubEntities.Clear();
                    region.SubEntities.AddRange(subList);
                    region.CalculateNumbersFromSubEntities();
                    regionResult.Add(region, constituencies);
                }
                result = regionResult;
            }

            String displayResult = String.Empty;
            foreach ( KeyValuePair<PopulationDataEntry, Int32> entry in result )
            {
                Int32 votersPerSeat = 0;
                if ( entry.Value != 0 )
                {
                    votersPerSeat = entry.Key.Total / entry.Value;
                }
                displayResult = displayResult +
                    String.Format("{0} {1} ({2} per seat)", entry.Key.English, entry.Value, votersPerSeat) + Environment.NewLine;
            }
            txtData.Text = displayResult;
            _lastCalculation = result;
            btnSaveCsv.Enabled = true;
        }

        private static void ModifyPopulationDataForBuengKan(PopulationDataEntry data)
        {
            PopulationDataEntry buengKan = data.FindByCode(38);
            if ( buengKan == null )
            {
                buengKan = new PopulationDataEntry();
                buengKan.English = "Bueng Kan";
                buengKan.Geocode = 38;
                List<Int32> buengKanAmphoeCodes = new List<int>() { 4313, 4311, 4309, 4312, 4303, 4306, 4310, 4304 };
                data.SubEntities.RemoveAll(p => p == null);
                PopulationDataEntry nongKhai = data.FindByCode(43);
                foreach ( Int32 code in buengKanAmphoeCodes )
                {
                    PopulationDataEntry entry = nongKhai.FindByCode(code);
                    buengKan.SubEntities.Add(entry);
                    nongKhai.SubEntities.Remove(entry);
                }
                nongKhai.CalculateNumbersFromSubEntities();
                buengKan.CalculateNumbersFromSubEntities();
                data.SubEntities.Add(buengKan);
                data.CalculateNumbersFromSubEntities();
            }
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
            Debug.Assert(_lastCalculation != null);

            StringBuilder lBuilder = new StringBuilder();

            foreach ( KeyValuePair<PopulationDataEntry, Int32> lEntry in _lastCalculation )
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

        private void btnLoadConstituencyXml_Click(Object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "XML Files|*.xml|All files|*.*";
            if ( openDialog.ShowDialog() == DialogResult.OK )
            {
                PopulationData data = null;
                StreamReader reader = null;
                try
                {

                    reader = new StreamReader(openDialog.FileName);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(reader.ReadToEnd());
                    foreach ( XmlNode node in xmlDoc.ChildNodes )
                    {
                        if ( node.Name == "electiondata" )
                        {
                            data = PopulationData.Load(node);
                        }
                    }
                }
                finally
                {
                    if ( reader != null )
                    {
                        reader.Dispose();
                    }
                }
                if ( (data != null) && (data.Data != null) )
                {
                    Int32 year = Convert.ToInt32(edtYear.Value);
                    PopulationDataEntry dataEntry = GetPopulationData(year);
                    if ( rbxNational.Checked && chkBuengKan.Checked )
                    {
                        ModifyPopulationDataForBuengKan(dataEntry);
                    }
                    dataEntry.SortSubEntitiesByEnglishName();

                    var entitie = data.Data.FlatList(new List<EntityType>() { EntityType.Bangkok, EntityType.Changwat, EntityType.Amphoe, EntityType.KingAmphoe, EntityType.Khet });
                    foreach ( PopulationDataEntry entry in entitie )
                    {
                        entry.CopyPopulationToConstituencies(dataEntry);
                    }

                    ConstituencyStatisticsViewer dialog = new ConstituencyStatisticsViewer(data.Data);
                    dialog.Show();
                }
            }
        }


    }
}

