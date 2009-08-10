using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    public partial class PopulationDataView : Form
    {
        private PopulationData mData = null;
        internal event RoyalGazetteList.ProcessingFinished OnShowGazette;
        internal PopulationData Data
        {
            get { return mData; }
            set { SetData(value); }
        }
        private void SetData(PopulationData value)
        {
            mData = value;
            if (value != null)
            {
                if (mData.Data != null)
                {
                    this.Text = mData.Data.English + " "+mData.year.ToString();
                }
                PopulationDataToTreeView(mTreeviewData, mData);
            }
            var lEntitiesWithoutGeocode = mData.EntitiesWithoutGeocode();
            if (lEntitiesWithoutGeocode.Count > 0)
            {
                string s = "";
                foreach (PopulationDataEntry lEntry in lEntitiesWithoutGeocode)
                {
                    s = s + lEntry.Name + "\n";
                }
                MessageBox.Show(lEntitiesWithoutGeocode.Count.ToString() + " entities without geocode\n" + s);
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
            foreach (PopulationDataEntry lEntity in iData.SubEntities)
            {
                ListViewItem lListViewItem = mListviewData.Items.Add(lEntity.English);
                lListViewItem.SubItems.Add(lEntity.Name);
                lListViewItem.SubItems.Add(lEntity.Geocode.ToString());
                lListViewItem.SubItems.Add(lEntity.Total.ToString());
                lListViewItem.SubItems.Add(lEntity.SubNames(Helper.Thesaban).ToString());
            }
            mListviewData.EndUpdate();
        }

        private TreeNode PopulationDataEntryToTreeNode(PopulationDataEntry iData)
        {
            TreeNode retval = null;
            if (iData != null)
            {
                String lName = String.Empty;
                if (!String.IsNullOrEmpty(iData.English))
                {
                    lName = iData.English;
                }
                else
                {
                    lName = "(" + iData.Name + ")";
                }
                retval = new TreeNode(lName);
                retval.Tag = iData;
                foreach (PopulationDataEntry lEntity in iData.SubEntities)
                {
                    if (!Helper.Thesaban.Contains(lEntity.Type))
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
            if (iData != null)
            {
                TreeNode lNode = PopulationDataEntryToTreeNode(iData.Data);
                if (lNode != null)
                {
                    iTreeView.Nodes.Add(lNode);
                }
            }
            iTreeView.EndUpdate();
        }
        private void btnClipboardAmphoe_Click(object sender, EventArgs e)
        {
            var lData = CurrentSelectedEntity(sender);
            string lOutput = lData.WriteForWikipedia(mData.WikipediaReference());
            Clipboard.SetText(lOutput);
        }

        private void tv_data_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var lData = CurrentSelectedEntity(sender);
            PopulationDataEntryToListView(lData);
            btnClipboardAmphoe.Enabled = Helper.IsCompatibleEntityType(lData.Type, EntityType.Amphoe);
        }

        private PopulationDataEntry CurrentSelectedEntity(object sender)
        {
            var lSelectedNode = mTreeviewData.SelectedNode;
            var retval = (PopulationDataEntry)(lSelectedNode.Tag);
            return retval;
        }

        private void btnSaveXML_Click(object sender, EventArgs e)
        {
            XmlDocument lXmlDocument = new XmlDocument();
            mData.ExportToXML(lXmlDocument);
            lXmlDocument.Save(mData.XMLExportFileName());
        }

        private void btnGazette_Click(object sender, EventArgs e)
        {
            var lData = CurrentSelectedEntity(sender);
            if ((lData != null)&&(OnShowGazette != null))
            {
                var lList = Helper.GlobalGazetteList.AllAboutEntity(lData.Geocode, true);
                OnShowGazette(lList);
            }
        }
    }
}
