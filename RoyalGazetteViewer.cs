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
    public partial class RoyalGazetteViewer : Form
    {
        private DataTable mTable = new DataTable();
        private void CreateDataTable()
        {
            mTable.Columns.Add("Description", typeof(String));
            mTable.Columns.Add("Title", typeof(RoyalGazette));
            mTable.Columns.Add("Volume", typeof(Int32));
            mTable.Columns.Add("Issue", typeof(RoyalGazetteIssue));
            mTable.Columns.Add("Page", typeof(RoyalGazettePageinfo));
            mTable.Columns.Add("Signed", typeof(DateTime));
            mTable.Columns.Add("Publication", typeof(DateTime));
            mTable.Columns.Add("Effective", typeof(DateTime));
            mTable.Columns.Add("Gazette", typeof(String));
        }

        private void FillDataTable()
        {
            foreach (RoyalGazette lEntry in mData)
            {
                DataRow lRow = mTable.NewRow();
                lRow["Description"] = lEntry.Description;
                lRow["Title"] = lEntry;
                lRow["Volume"] = lEntry.Volume;
                lRow["Issue"] = lEntry.Issue;
                lRow["Page"] = lEntry.PageInfo;
                lRow["Publication"] = lEntry.Publication;
                lRow["Signed"] = lEntry.Sign;
                lRow["Effective"] = lEntry.Effective;
                lRow["Gazette"] = lEntry;
                mTable.Rows.Add(lRow);
            }
            grid.DataSource = mTable; 
        }

        public RoyalGazetteViewer()
        {
            InitializeComponent();
            CreateDataTable();
            InitializeGrid();
        }

        private RoyalGazetteList mData = null;
        internal RoyalGazetteList Data
        {
            get { return mData; }
            set { SetData(value); }
        }
        private void SetData(RoyalGazetteList value)
        {
            mData = value;
            FillDataTable();
            grid.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None).Visible = false;
        }

        private void InitializeGrid()
        {
            grid.ColumnHeadersBorderStyle =
                    DataGridViewHeaderBorderStyle.Single;
        }

        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridView lGrid = sender as DataGridView;
                DataGridViewRow lRow = lGrid.Rows[e.RowIndex];

                DataRowView lRowView = lRow.DataBoundItem as DataRowView;
                // Debug.Assert(lRowView != null);

                DataRow lDataRow = lRowView.Row;
                // Debug.Assert(lDataRow != null);

                RoyalGazette lGazette = lDataRow["Title"] as RoyalGazette;
                if (lGazette != null)
                {
                    lGazette.ShowPDF();
                }
            }
        }

        private void mnuShow_Click(object sender, EventArgs e)
        {
            foreach (RoyalGazette lGazette in CurrentSelection())
            {
                lGazette.ShowPDF();
            }
        }

        private void grid_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            contextMenuStrip1.Show(MousePosition);
        }
        private RoyalGazetteList CurrentSelection()
        {
            RoyalGazetteList retval = new RoyalGazetteList();
            foreach (DataGridViewRow lRow in grid.SelectedRows)
            {
                DataRowView lRowView = lRow.DataBoundItem as DataRowView;
                // Debug.Assert(lRowView != null);

                DataRow lDataRow = lRowView.Row;
                // Debug.Assert(lDataRow != null);

                RoyalGazette lGazette = lDataRow["Title"] as RoyalGazette;
                if (lGazette != null)
                {
                    retval.Add(lGazette);
                }
            }
            return retval;
        }

        private void mnuCitation_Click(object sender, EventArgs e)
        {
            String retval = String.Empty;
            foreach (RoyalGazette lGazette in CurrentSelection())
            {
                retval = retval + lGazette.Citation() + " ";
            }
            retval = retval.Trim();
            if (!String.IsNullOrEmpty(retval))
            {
                Clipboard.SetText(retval);
            }
        }
        internal static void ShowGazetteDialog(RoyalGazetteList iList)
        {
            var lDataForm = new RoyalGazetteViewer();
            lDataForm.Data = iList;
            lDataForm.Show();
        }

        internal static void ShowGazetteNewsDialog(RoyalGazetteList iList)
        {
            var lNewGazetteEntries = new RoyalGazetteList();
            foreach (RoyalGazette lEntry in iList)
            {
                if (Helper.GlobalGazetteList == null)
                {
                    lNewGazetteEntries.Add(lEntry);
                }
                else if (!Helper.GlobalGazetteList.Contains(lEntry))
                {
                    lNewGazetteEntries.Add(lEntry);
                }
            }
            if (lNewGazetteEntries.Count != 0)
            {
                ShowGazetteDialog(lNewGazetteEntries);
            }
        }

        private void mnuMirror_Click(object sender, EventArgs e)
        {
            foreach (RoyalGazette lGazette in CurrentSelection())
            {
                lGazette.MirrorToCache();
            }
        }

        private void mnuDeletePDF_Click(object sender, EventArgs e)
        {
            foreach (RoyalGazette lGazette in CurrentSelection())
            {
                lGazette.RemoveFromCache();
            }

        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (mData != null)
            {
                SaveFileDialog lDlg = new SaveFileDialog();
                lDlg.Filter = "XML Files|*.xml|All files|*.*";
                if (lDlg.ShowDialog() == DialogResult.OK)
                {
                    mData.SaveXML(/*"e:\\Thailand\\DOPA\\Gazette new.xml"*/ lDlg.FileName);
                }
            }
        }
    }
}
