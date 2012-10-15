using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;

namespace De.AHoerstemeier.Tambon
{
    public partial class PopulationDataView : Form
    {
        private PopulationData mData = null;
        internal event RoyalGazetteProcessingFinishedHandler ShowGazette;
        internal PopulationData Data
        {
            get { return mData; }
            set { SetData(value); }
        }
        private void SetData(PopulationData value)
        {
            mData = value;
            if ( value != null )
            {
                if ( mData.Data != null )
                {
                    this.Text = mData.Data.English + " " + mData.Year.ToString();
                }
                PopulationDataToTreeView(mTreeviewData, mData);
            }
        }

        public PopulationDataView()
        {
            InitializeComponent();
        }

        private void PopulationDataEntryToListView(PopulationDataEntry iData)
        {
            mListviewData.BeginUpdate();
            mListviewData.Items.Clear();
            foreach ( PopulationDataEntry lEntity in iData.SubEntities )
            {
                ListViewItem lListViewItem = mListviewData.Items.Add(lEntity.English);
                lListViewItem.SubItems.Add(lEntity.Name);
                lListViewItem.SubItems.Add(lEntity.Geocode.ToString());
                lListViewItem.SubItems.Add(lEntity.Total.ToString());
                lListViewItem.SubItems.Add(lEntity.SubNames(EntityTypeHelper.Thesaban).ToString());
            }
            mListviewData.EndUpdate();
        }

        private TreeNode PopulationDataEntryToTreeNode(PopulationDataEntry iData)
        {
            TreeNode retval = null;
            if ( iData != null )
            {
                String lName = String.Empty;
                if ( !String.IsNullOrEmpty(iData.English) )
                {
                    lName = iData.English;
                }
                else
                {
                    lName = "(" + iData.Name + ")";
                }
                retval = new TreeNode(lName);
                retval.Tag = iData;
                foreach ( PopulationDataEntry lEntity in iData.SubEntities )
                {
                    if ( !EntityTypeHelper.Thesaban.Contains(lEntity.Type) )
                    {
                        retval.Nodes.Add(PopulationDataEntryToTreeNode(lEntity));
                    }
                }

            }
            return retval;

        }
        private void PopulationDataToTreeView(TreeView iTreeView, PopulationData iData)
        {
            iTreeView.BeginUpdate();
            iTreeView.Nodes.Clear();
            if ( iData != null )
            {
                TreeNode lNode = PopulationDataEntryToTreeNode(iData.Data);
                if ( lNode != null )
                {
                    iTreeView.Nodes.Add(lNode);
                }
            }
            iTreeView.EndUpdate();
        }
        private void btnClipboardAmphoe_Click(object sender, EventArgs e)
        {
            var lData = CurrentSelectedEntity(sender);
            String lOutput = lData.WriteForWikipedia(mData.WikipediaReference());
            if ( lOutput != String.Empty )
            {
                Clipboard.SetText(lOutput);
            }
        }

        private void tv_data_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var lData = CurrentSelectedEntity(sender);
            PopulationDataEntryToListView(lData);
            btnClipboardAmphoe.Enabled = lData.CanWriteForWikipedia();
            var entityCounter = lData.EntityTypeNumbers();
            String text = String.Empty;
            foreach ( KeyValuePair<EntityType, Int32> keyValuePair in entityCounter )
            {
                text = text + keyValuePair.Key.ToString() + " " + keyValuePair.Value.ToString(CultureInfo.InvariantCulture) + Environment.NewLine;
            }
            textBox1.Text = text;
        }

        private PopulationDataEntry CurrentSelectedEntity(object sender)
        {
            var lSelectedNode = mTreeviewData.SelectedNode;
            var retval = (PopulationDataEntry)(lSelectedNode.Tag);
            return retval;
        }

        private void btnSaveXML_Click(Object sender, EventArgs e)
        {
            XmlDocument lXmlDocument = new XmlDocument();
            mData.ExportToXML(lXmlDocument);
            lXmlDocument.Save(mData.XmlExportFileName());
        }

        private void btnGazette_Click(Object sender, EventArgs e)
        {
            var data = CurrentSelectedEntity(sender);
            if ( (data != null) && (ShowGazette != null) )
            {
                var list = TambonHelper.GlobalGazetteList.AllAboutEntity(data.Geocode, true);
                // Also check for old obsolete Geocodes!
                ShowGazette(this,new RoyalGazetteEventArgs(list));
            }
        }

        // ToDo: Change it to an event of GobalGazetteList
        private void PopulationDataView_Enter(object sender, EventArgs e)
        {
            btnGazette.Enabled = TambonHelper.GlobalGazetteList.Any();
        }
    }
}
