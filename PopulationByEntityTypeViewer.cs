using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace De.AHoerstemeier.Tambon
{
    public partial class PopulationByEntityTypeViewer : Form
    {
        // ToDo: Sort by Column
        // Export as CSV

        private PopulationDataEntry mBaseEntry = null;
        internal PopulationDataEntry BaseEntry
        {
            get { return mBaseEntry; }
            set { SetData(value); }
        }

        private void SetData(PopulationDataEntry value)
        {
            mBaseEntry = value;
            UpdateList();
        }

        public PopulationByEntityTypeViewer()
        {
            InitializeComponent();
            UpdateEntityTypeCheckboxes();
        }

        private void UpdateEntityTypeCheckboxes()
        {
            chk_Amphoe.Enabled = rbx_AmphoeKhet.Checked;
            chk_Khet.Enabled = rbx_AmphoeKhet.Checked;
            chk_Tambon.Enabled = rbx_TambonKhwaeng.Checked;
            chk_Khwaeng.Enabled = rbx_TambonKhwaeng.Checked;
            chk_ThesabanNakhon.Enabled = rbx_Thesaban.Checked;
            chk_ThesabanMueang.Enabled = rbx_Thesaban.Checked;
            chk_ThesabanTambon.Enabled = rbx_Thesaban.Checked;
        }
        private void UpdateList()
        {
            List<PopulationDataEntry> lList = CalculateList();
            FillListView(lList);

            FrequencyCounter lCounter = new FrequencyCounter();
            foreach (var lEntry in lList)
            {
                lCounter.IncrementForCount(lEntry.Total, lEntry.Geocode);
            }

            StringBuilder lBuilder = new StringBuilder();
            lBuilder.AppendLine("Total population: " + lCounter.SumValue.ToString("##,###,##0"));
            lBuilder.AppendLine("Number of entities: "+lCounter.NumberOfValues.ToString());
            lBuilder.AppendLine("Mean population: " + lCounter.MeanValue.ToString("##,###,##0.0"));
            lBuilder.AppendLine("Maximum population: " + lCounter.MaxValue.ToString("##,###,##0"));
            lBuilder.AppendLine("Minimum population: " + lCounter.MinValue.ToString("##,###,##0"));

            txtStatistics.Text = lBuilder.ToString();
        }

        private List<PopulationDataEntry> CalculateList()
        {
            List<PopulationDataEntry> lList = new List<PopulationDataEntry>();
            List<EntityType> lEntities = new List<EntityType>();
            if (rbx_Changwat.Checked)
            {
                lEntities.Add(EntityType.Changwat);
                lEntities.Add(EntityType.Bangkok);
                lList.AddRange(mBaseEntry.FlatList(lEntities));
            }
            else if (rbx_AmphoeKhet.Checked)
            {
                if (chk_Amphoe.Checked)
                {
                    lEntities.Add(EntityType.Amphoe);
                }
                if (chk_Khet.Checked)
                {
                    lEntities.Add(EntityType.Khet);
                }
                lList.AddRange(mBaseEntry.FlatList(lEntities));
            }
            else if (rbx_TambonKhwaeng.Checked)
            {
                if (chk_Tambon.Checked)
                {
                    lEntities.Add(EntityType.Tambon);
                }
                if (chk_Khwaeng.Checked)
                {
                    lEntities.Add(EntityType.Khwaeng);
                }
                lList.AddRange(mBaseEntry.FlatList(lEntities));
            }
            else if (rbx_Thesaban.Checked)
            {
                if (chk_ThesabanTambon.Checked)
                {
                    lEntities.Add(EntityType.ThesabanTambon);
                }
                if (chk_ThesabanMueang.Checked)
                {
                    lEntities.Add(EntityType.ThesabanMueang);
                }
                if (chk_ThesabanNakhon.Checked)
                {
                    lEntities.Add(EntityType.ThesabanNakhon);
                }

                foreach (PopulationDataEntry lEntity in mBaseEntry.ThesabanList())
                {
                    if (lEntities.Contains(lEntity.Type))
                    {
                        lList.Add(lEntity);
                    }
                }
            }

            lList.Sort(delegate(PopulationDataEntry p1, PopulationDataEntry p2) { return (p2.Total.CompareTo(p1.Total)); });
            return lList;
        }
        private void FillListView(List<PopulationDataEntry> iEntityList)
        {
            mListviewData.BeginUpdate();
            mListviewData.Items.Clear();
            foreach (PopulationDataEntry lEntity in iEntityList)
            {
                ListViewItem lListViewItem = mListviewData.Items.Add(lEntity.English);
                lListViewItem.SubItems.Add(lEntity.Name);
                lListViewItem.SubItems.Add(lEntity.Geocode.ToString());
                lListViewItem.SubItems.Add(lEntity.Total.ToString());
            }
            mListviewData.EndUpdate();
        }

        private void chk_Entity_CheckStateChanged(object sender, EventArgs e)
        {
            UpdateList();
        }
        private void rbx_Entity_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEntityTypeCheckboxes();
            UpdateList();
        }

        private void btnExportCSV_Click(object sender, EventArgs e)
        {
            const Char mCSVCharacter = ',';
            SaveFileDialog lDlg = new SaveFileDialog();
            lDlg.Filter = "CSV Files|*.csv|All files|*.*";
            if (lDlg.ShowDialog() == DialogResult.OK)
            {
                StreamWriter lFileWrite = new StreamWriter(lDlg.FileName);
                var lList = CalculateList();
                foreach (PopulationDataEntry lEntity in lList)
                {
                    ListViewItem lListViewItem = mListviewData.Items.Add(lEntity.English);
                    lFileWrite.Write(lEntity.Name);
                    lFileWrite.Write(mCSVCharacter);
                    lFileWrite.Write(lEntity.English);
                    lFileWrite.Write(mCSVCharacter);
                    lFileWrite.Write(lEntity.Geocode.ToString());
                    lFileWrite.Write(mCSVCharacter);
                    lFileWrite.WriteLine(lEntity.Total.ToString());
                }
                lFileWrite.Close();
            }
        }
    }
}
