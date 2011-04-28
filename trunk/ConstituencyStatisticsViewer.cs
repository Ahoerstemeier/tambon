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
            txtStatistics.Text = CalculateData(0);
        }

        private String CalculateData(Int32 iGeocode)
        {
            String lResult = String.Empty;
            FrequencyCounter lCounter = new FrequencyCounter();
            List<PopulationDataEntry> lList = null;
            if (iGeocode == 0)
            {

                lList = mData.FlatList(new List<EntityType>() { EntityType.Bangkok, EntityType.Changwat, EntityType.Amphoe, EntityType.KingAmphoe, EntityType.Khet });
            }
            else
            {
                lList = new List<PopulationDataEntry>();
                lList.Add(mData.FindByCode(iGeocode));
            }
            foreach (PopulationDataEntry lEntry in lList)
            {
                foreach (ConstituencyEntry lConstituency in lEntry.ConstituencyList)
                {
                    lCounter.IncrementForCount(lConstituency.Population(), lEntry.Geocode * 100 + lConstituency.Index);
                }
            }
            StringBuilder lBuilder = new StringBuilder();
            lBuilder.AppendLine("Number of constituencies: " + lCounter.NumberOfValues.ToString());
            lBuilder.AppendLine("Mean population per constituencies: " + Math.Round(lCounter.MeanValue).ToString());
            lBuilder.AppendLine("Maximum population: " + lCounter.MaxValue.ToString());
            foreach (var lEntry in lCounter.Data[lCounter.MaxValue])
            {
            lBuilder.AppendLine(" "+GetEntityConstituencyName(lEntry));
            }
            lBuilder.AppendLine("Minimum population: " + lCounter.MinValue.ToString());
            foreach (var lEntry in lCounter.Data[lCounter.MinValue])
            {
                lBuilder.AppendLine(" " + GetEntityConstituencyName(lEntry));
            }
            lResult = lBuilder.ToString();
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
    }
}
