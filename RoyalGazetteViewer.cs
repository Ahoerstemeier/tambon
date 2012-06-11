using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
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
            mTable.Columns.Add("SignedBy", typeof(String));
            mTable.Columns.Add("SignedByAs", typeof(GazetteSignPosition));
            mTable.Columns.Add("Gazette", typeof(String));
        }

        private void FillDataTable(RoyalGazetteList iData)
        {
            RoyalGazetteList lData = iData;
            if ( filterToolStripMenuItem.Checked )
            {
                lData = lData.FilteredList(TambonHelper.GlobalGazetteList);
            }

            mTable.Rows.Clear();
            foreach ( RoyalGazette lEntry in lData )
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
                lRow["SignedBy"] = lEntry.SignedBy;
                lRow["SignedByAs"] = lEntry.SignedByPosition;
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

        private Boolean mFiltered = false;
        internal Boolean Filtered
        {
            get { return mFiltered; }
            set { mFiltered = value; SetData(mData); }
        }
        private void SetData(RoyalGazetteList value)
        {
            if ( value != null )
            {
                mData = value;
                FillDataTable(mData);
                grid.Columns[mTable.Columns.Count - 1].Visible = false;
            }
        }

        private void InitializeGrid()
        {
            grid.ColumnHeadersBorderStyle =
                    DataGridViewHeaderBorderStyle.Single;
        }

        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if ( e.RowIndex >= 0 )
            {
                DataGridView lGrid = sender as DataGridView;
                DataGridViewRow lRow = lGrid.Rows[e.RowIndex];

                DataRowView lRowView = lRow.DataBoundItem as DataRowView;
                // Debug.Assert(lRowView != null);

                DataRow lDataRow = lRowView.Row;
                // Debug.Assert(lDataRow != null);

                RoyalGazette lGazette = lDataRow["Title"] as RoyalGazette;
                if ( lGazette != null )
                {
                    lGazette.ShowPDF();
                }
            }
        }

        private void mnuShow_Click(object sender, EventArgs e)
        {
            foreach ( RoyalGazette lGazette in CurrentSelection() )
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
            foreach ( DataGridViewRow lRow in grid.SelectedRows )
            {
                DataRowView lRowView = lRow.DataBoundItem as DataRowView;
                // Debug.Assert(lRowView != null);

                DataRow lDataRow = lRowView.Row;
                // Debug.Assert(lDataRow != null);

                RoyalGazette lGazette = lDataRow["Title"] as RoyalGazette;
                if ( lGazette != null )
                {
                    retval.Add(lGazette);
                }
            }
            return retval;
        }

        private void mnuCitation_Click(object sender, EventArgs e)
        {
            String retval = String.Empty;
            foreach ( RoyalGazette lGazette in CurrentSelection() )
            {
                retval = retval + lGazette.Citation() + " ";
            }
            retval = retval.Trim();
            if ( !String.IsNullOrEmpty(retval) )
            {
                Clipboard.SetText(retval);
            }
        }
        internal static void ShowGazetteDialog(RoyalGazetteList iList, Boolean iFiltered)
        {
            ShowGazetteDialog(iList, iFiltered, String.Empty);
        }
        internal static void ShowGazetteDialog(RoyalGazetteList iList, Boolean iFiltered, String iTitle)
        {
            var lDataForm = new RoyalGazetteViewer();
            lDataForm.Filtered = iFiltered;
            lDataForm.Data = iList;
            if ( !String.IsNullOrEmpty(iTitle) )
            {
                lDataForm.Text = iTitle;
            }
            lDataForm.Show();
        }

        internal static void ShowGazetteNewsDialog(RoyalGazetteList iList)
        {
            var lNewGazetteEntries = iList.FilteredList(TambonHelper.GlobalGazetteList);
            if ( lNewGazetteEntries.Count != 0 )
            {
                ShowGazetteDialog(iList, true);
            }
        }

        private void mnuMirror_Click(object sender, EventArgs e)
        {
            foreach ( RoyalGazette lGazette in CurrentSelection() )
            {
                lGazette.MirrorToCache();
            }
        }

        private void mnuDeletePDF_Click(object sender, EventArgs e)
        {
            foreach ( RoyalGazette lGazette in CurrentSelection() )
            {
                lGazette.RemoveFromCache();
            }
        }

        private void btnSaveXml_Click(object sender, EventArgs e)
        {
            if ( mData != null )
            {
                SaveFileDialog lDlg = new SaveFileDialog();
                lDlg.Filter = "XML Files|*.xml|All files|*.*";
                if ( lDlg.ShowDialog() == DialogResult.OK )
                {
                    mData.SaveXML(lDlg.FileName);
                }
            }
        }

        private void btnSaveRSS_Click(object sender, EventArgs e)
        {
            if ( mData != null )
            {
                SaveFileDialog lDlg = new SaveFileDialog();
                lDlg.Filter = "XML Files|*.xml|All files|*.*";
                if ( lDlg.ShowDialog() == DialogResult.OK )
                {
                    mData.ExportToRSS(lDlg.FileName);
                }
            }
        }

        private void xMLSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String retval = String.Empty;
            XmlDocument lXmlDocument = new XmlDocument();
            XmlNode lBaseNode = lXmlDocument;
            if ( CurrentSelection().Count > 1 )
            {
                lBaseNode = (XmlElement)lXmlDocument.CreateNode("element", "gazette", "");
                lXmlDocument.AppendChild(lBaseNode);
            }
            foreach ( RoyalGazette lGazette in CurrentSelection() )
            {
                lGazette.ExportToXML(lBaseNode);
            }
            retval = lXmlDocument.InnerXml;
            if ( !String.IsNullOrEmpty(retval) )
            {
                Clipboard.SetText(retval);
            }

        }

        private void pDFURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String retval = String.Empty;
            foreach ( RoyalGazette gazette in CurrentSelection() )
            {
                retval = retval + gazette.URL() + Environment.NewLine;
            }
            retval = retval.Substring(0, Math.Max(0, retval.Length - Environment.NewLine.Length));
            if ( !String.IsNullOrEmpty(retval) )
            {
                Clipboard.SetText(retval);
            }
        }

        private void filterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterToolStripMenuItem.Checked = !filterToolStripMenuItem.Checked;
            Filtered = filterToolStripMenuItem.Checked;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            Boolean lHasFilter = ((TambonHelper.GlobalGazetteList != null) && (TambonHelper.GlobalGazetteList.Count > 0));
            filterToolStripMenuItem.Visible = (lHasFilter);
            toolStripSeparator1.Visible = (lHasFilter);
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            RoyalGazetteList lList = mData.FindDuplicates();
            if ( lList.Count > 0 )
            {
                ShowGazetteDialog(lList, false, "Duplicate entries");
            }
        }
    }
}
