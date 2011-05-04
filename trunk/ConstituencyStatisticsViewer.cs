using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace De.AHoerstemeier.Tambon
{
    public partial class ConstituencyStatisticsViewer : Form
    {
        private PopulationDataEntry mData;

        public ConstituencyStatisticsViewer(PopulationDataEntry iData)
        {
            mData = iData;
            InitializeComponent();
            FillTreeView(TrvData);
            txtStatistics.Text = CalculateData(0);
        }

        private TreeNode PopulationDataEntryToTreeNode(PopulationDataEntry iData)
        {
            // ToDo: cleanup this CodeCopy from PopulationDataView.cs
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

        private void FillTreeView(TreeView iTreeView)
        {
            iTreeView.BeginUpdate();
            iTreeView.Nodes.Clear();
            if ( mData != null )
            {
                TreeNode lNode = PopulationDataEntryToTreeNode(mData);
                if ( lNode != null )
                {
                    iTreeView.Nodes.Add(lNode);
                }
            }
            iTreeView.EndUpdate();
            
        }

        private String CalculateData(Int32 iGeocode)
        {
            String lResult = String.Empty;
            PopulationDataEntry lEntry = null;
            if ( iGeocode == 0 )
            {
                lEntry = mData;
            }
            else
            {
                lEntry = mData.FindByCode(iGeocode);
            }
            if (lEntry != null)
            {
                List<PopulationDataEntry> lList = lEntry.FlatList(new List<EntityType>() { EntityType.Bangkok, EntityType.Changwat, EntityType.Amphoe, EntityType.KingAmphoe, EntityType.Khet });
                lList.Add(lEntry);
                FrequencyCounter lCounter = new FrequencyCounter();
                Int32 lSeats = 0;
                foreach (PopulationDataEntry lSubEntry in lList)
                {
                    foreach (ConstituencyEntry lConstituency in lSubEntry.ConstituencyList)
                    {
                        lCounter.IncrementForCount(lConstituency.Population() / lConstituency.NumberOfSeats, lSubEntry.Geocode * 100 + lConstituency.Index);
                        lSeats += lConstituency.NumberOfSeats;
                    }
                }
                StringBuilder lBuilder = new StringBuilder();
                lBuilder.AppendLine("Number of constituencies: " + lCounter.NumberOfValues.ToString());
                lBuilder.AppendLine("Number of seats: " + lSeats.ToString());
                if ( lCounter.NumberOfValues > 0 )
                {
                    lBuilder.AppendLine("Mean population per seat: " + Math.Round(lCounter.MeanValue).ToString());
                    lBuilder.AppendLine("Standard deviation: " + Math.Round(lCounter.StandardDeviation).ToString());
                    lBuilder.AppendLine("Maximum population per seat: " + lCounter.MaxValue.ToString());
                    foreach ( var lSubEntry in lCounter.Data[lCounter.MaxValue] )
                    {
                        lBuilder.AppendLine(" " + GetEntityConstituencyName(lSubEntry));
                    }
                    lBuilder.AppendLine("Minimum population per seat: " + lCounter.MinValue.ToString());
                    foreach ( var lSubEntry in lCounter.Data[lCounter.MinValue] )
                    {
                        lBuilder.AppendLine(" " + GetEntityConstituencyName(lSubEntry));
                    }
                }
                lResult = lBuilder.ToString();
            }
            return lResult;
        }

        private string GetEntityConstituencyName(Int32 iEntry)
        {
            Int32 lGeocode = iEntry / 100;
            Int32 lConstituency = iEntry % 100;
            PopulationDataEntry lEntry = mData.FindByCode(lGeocode);
            Debug.Assert(lEntry != null, "Code " + lGeocode.ToString() + " not found");
            String lResult = lEntry.English + " Constituency " + lConstituency.ToString();
            return lResult;
        }

        private void TrvData_AfterSelect(object sender, TreeViewEventArgs e)
        {
            PopulationDataEntry lEntry = (PopulationDataEntry)e.Node.Tag;
            if ( lEntry != null )
            {
                txtStatistics.Text = CalculateData(lEntry.Geocode);
            }
        }
    }
}
