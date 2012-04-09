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

        private PopulationDataEntry _baseEntry = null;
        internal PopulationDataEntry BaseEntry
        {
            get { return _baseEntry; }
            set { SetData(value); }
        }

        private void SetData(PopulationDataEntry value)
        {
            _baseEntry = value;
            UpdateList();
        }

        public PopulationByEntityTypeViewer()
        {
            InitializeComponent();
            UpdateEntityTypeCheckboxes();
        }

        private void UpdateEntityTypeCheckboxes()
        {
            chkAmphoe.Enabled = rbxAmphoeKhet.Checked;
            chkKhet.Enabled = rbxAmphoeKhet.Checked;
            chkTambon.Enabled = rbxTambonKhwaeng.Checked;
            chkKhwaeng.Enabled = rbxTambonKhwaeng.Checked;
            chkThesabanNakhon.Enabled = rbxThesaban.Checked;
            chkThesabanMueang.Enabled = rbxThesaban.Checked;
            chkThesabanTambon.Enabled = rbxThesaban.Checked;
        }
        private void UpdateList()
        {
            IEnumerable<PopulationDataEntry> list = CalculateList();
            PopulationDataEntry compare = FindCompare();
            List<Tuple<Int32, Int32, Double>> populationChanges = null;
            if (compare != null)
            {
                populationChanges = CalcPopulationChanges(list, compare).ToList();
            }
            FillListView(list, populationChanges);

            FrequencyCounter counter = new FrequencyCounter();
            foreach (var entry in list)
            {
                counter.IncrementForCount(entry.Total, entry.Geocode);
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Total population: " + counter.SumValue.ToString("##,###,##0"));
            builder.AppendLine("Number of entities: " + counter.NumberOfValues.ToString());
            builder.AppendLine("Mean population: " + counter.MeanValue.ToString("##,###,##0.0"));
            builder.AppendLine("Maximum population: " + counter.MaxValue.ToString("##,###,##0"));
            builder.AppendLine("Minimum population: " + counter.MinValue.ToString("##,###,##0"));

            if ((populationChanges != null) && (populationChanges.Any(x => x.Item2!=0)))
            {
                builder.AppendLine();
                populationChanges.Sort((x, y) => y.Item2.CompareTo(x.Item2));
                var winner = populationChanges.First();
                var winnerEntry = list.First(x => x.Geocode == winner.Item1);
                var looser = populationChanges.Last();
                var looserEntry = list.First(x => x.Geocode == looser.Item1);
                builder.AppendLine("Biggest winner: " + winner.Item2.ToString("##,###,##0")+" by "+winnerEntry.English+" ("+winner.Item1+")" );
                builder.AppendLine("Biggest looser: " + looser.Item2.ToString("##,###,##0") + " by " + looserEntry.English + " (" + looser.Item1 + ")");
            }
            if ((populationChanges != null) && (populationChanges.Any(x => x.Item2 != 0)))
            {
                builder.AppendLine();
                populationChanges.Sort((x, y) => y.Item3.CompareTo(x.Item3));
                var winner = populationChanges.First();
                var winnerEntry = list.First(x => x.Geocode == winner.Item1);
                var looser = populationChanges.Last();
                var looserEntry = list.First(x => x.Geocode == looser.Item1);
                builder.AppendLine("Biggest winner: " + winner.Item3.ToString("##0.00") + "% by " + winnerEntry.English + " (" + winner.Item1 + ")");
                builder.AppendLine("Biggest looser: " + looser.Item3.ToString("##0.00") + "% by " + looserEntry.English + " (" + looser.Item1 + ")");
            }

            txtStatistics.Text = builder.ToString();
        }

        private IEnumerable<Tuple<Int32, Int32, Double>> CalcPopulationChanges(IEnumerable<PopulationDataEntry> entityList, PopulationDataEntry compare)
        {
            List<Tuple<Int32, Int32, Double>> result = new List<Tuple<Int32, Int32, Double>>();
            if (entityList.Any() && (compare != null))
            {
                IEnumerable<PopulationDataEntry> thesabanList = null;
                if ((EntityTypeHelper.IsCompatibleEntityType(EntityType.Thesaban, entityList.First().Type)))
                {
                    thesabanList = compare.ThesabanList();
                }
                foreach (PopulationDataEntry entity in entityList)
                {
                    if (entity.Geocode != 0)
                    {
                        PopulationDataEntry compareEntry;
                        if (EntityTypeHelper.IsCompatibleEntityType(EntityType.Thesaban, entity.Type))
                        {
                            compareEntry = thesabanList.FirstOrDefault(x => x.Geocode == entity.Geocode);
                        }
                        else
                        {
                            compareEntry = compare.FindByCode(entity.Geocode);
                        }
                        if ((compareEntry != null) && (compareEntry.Total > 0))
                        {
                            Int32 populationChange = entity.Total - compareEntry.Total;
                            Double changePercent = 100.0 * populationChange / compareEntry.Total;
                            result.Add(Tuple.Create(entity.Geocode,populationChange,changePercent));
                        }
                    }
                }
            }
            return result;
        }

        private PopulationDataEntry FindCompare()
        {
            PopulationDataEntry result = null;
            if (chkCompare.Checked)
            {
                Int32 year = Convert.ToInt32(edtCompareYear.Value);
                result = TambonHelper.GetCountryPopulationData(year);
            }
            return result;
        }

        private IEnumerable<PopulationDataEntry> CalculateList()
        {
            List<PopulationDataEntry> list = new List<PopulationDataEntry>();
            List<EntityType> entities = new List<EntityType>();
            if (rbx_Changwat.Checked)
            {
                entities.Add(EntityType.Changwat);
                entities.Add(EntityType.Bangkok);
                list.AddRange(_baseEntry.FlatList(entities));
            }
            else if (rbxAmphoeKhet.Checked)
            {
                if (chkAmphoe.Checked)
                {
                    entities.Add(EntityType.Amphoe);
                }
                if (chkKhet.Checked)
                {
                    entities.Add(EntityType.Khet);
                }
                list.AddRange(_baseEntry.FlatList(entities));
            }
            else if (rbxTambonKhwaeng.Checked)
            {
                if (chkTambon.Checked)
                {
                    entities.Add(EntityType.Tambon);
                }
                if (chkKhwaeng.Checked)
                {
                    entities.Add(EntityType.Khwaeng);
                }
                list.AddRange(_baseEntry.FlatList(entities));
            }
            else if (rbxThesaban.Checked)
            {
                if (chkThesabanTambon.Checked)
                {
                    entities.Add(EntityType.ThesabanTambon);
                }
                if (chkThesabanMueang.Checked)
                {
                    entities.Add(EntityType.ThesabanMueang);
                }
                if (chkThesabanNakhon.Checked)
                {
                    entities.Add(EntityType.ThesabanNakhon);
                }

                foreach (PopulationDataEntry entity in _baseEntry.ThesabanList())
                {
                    if (entities.Contains(entity.Type))
                    {
                        list.Add(entity);
                    }
                }
            }

            list.Sort(delegate(PopulationDataEntry p1, PopulationDataEntry p2) { return (p2.Total.CompareTo(p1.Total)); });
            return list;
        }
        private void FillListView(IEnumerable<PopulationDataEntry> entityList, IEnumerable<Tuple<Int32, Int32, Double>> compare)
        {
            lvData.BeginUpdate();
            lvData.Items.Clear();
            if (entityList.Any())
            {
                foreach (PopulationDataEntry entity in entityList)
                {
                    ListViewItem listViewItem = lvData.Items.Add(entity.English);
                    listViewItem.SubItems.Add(entity.Name);
                    listViewItem.SubItems.Add(entity.Geocode.ToString());
                    listViewItem.SubItems.Add(entity.Total.ToString());
                    if (compare != null)
                    {
                        var compareEntry = compare.FirstOrDefault(x => x.Item1 == entity.Geocode);
                        if (compareEntry != null)
                        {
                            listViewItem.SubItems.Add(compareEntry.Item2.ToString());
                            listViewItem.SubItems.Add(compareEntry.Item3.ToString("##0.00"));
                        }
                    }
                }
            }
            lvData.EndUpdate();
        }

        private void chkEntity_CheckStateChanged(object sender, EventArgs e)
        {
            UpdateList();
        }
        private void rbxEntity_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEntityTypeCheckboxes();
            UpdateList();
        }

        private void btnExportCSV_Click(object sender, EventArgs e)
        {
            const Char csvCharacter = ',';
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "CSV Files|*.csv|All files|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter fileWriter = new StreamWriter(dialog.FileName);
                var list = CalculateList();
                var compare = FindCompare();
                foreach (PopulationDataEntry entity in list)
                {
                    fileWriter.Write(entity.Name);
                    fileWriter.Write(csvCharacter);
                    fileWriter.Write(entity.English);
                    fileWriter.Write(csvCharacter);
                    fileWriter.Write(entity.Geocode.ToString());
                    fileWriter.Write(csvCharacter);
                    fileWriter.Write(entity.Total.ToString());
                    if ((compare != null) && (entity.Geocode != 0))
                    {
                        PopulationDataEntry compareEntry = compare.FindByCode(entity.Geocode);
                        if ((compareEntry != null) && (compareEntry.Total > 0))
                        {
                            Int32 populationChange = entity.Total - compareEntry.Total;
                            Double changePercent = 100.0 * populationChange / compareEntry.Total;
                            fileWriter.Write(csvCharacter);
                            fileWriter.Write(populationChange.ToString());
                            fileWriter.Write(csvCharacter);
                            fileWriter.Write(changePercent.ToString("##0.##"));
                        }
                    }
                    fileWriter.WriteLine();
                }
                fileWriter.Close();
            }
        }

        private void PopulationByEntityTypeViewer_Load(object sender, EventArgs e)
        {
            edtCompareYear.Maximum = TambonHelper.PopulationStatisticMaxYear;
            edtCompareYear.Minimum = TambonHelper.PopulationStatisticMinYear;
            edtCompareYear.Value = edtCompareYear.Maximum;
            edtCompareYear.Enabled = chkCompare.Checked;
        }

        private void chkCompare_CheckedChanged(object sender, EventArgs e)
        {
            edtCompareYear.Enabled = chkCompare.Checked;
            UpdateList();
        }

        private void edtCompareYear_ValueChanged(object sender, EventArgs e)
        {
            UpdateList();
        }
    }
}
