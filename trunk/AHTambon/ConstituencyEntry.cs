using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace De.AHoerstemeier.Tambon
{
    public class ConstituencyEntry : ICloneable
    {
        #region properties
        private List<PopulationDataEntry> lAdministrativeEntities = new List<PopulationDataEntry>();
        public List<PopulationDataEntry> AdministrativeEntities
        { get { return lAdministrativeEntities; } }
        private List<PopulationDataEntry> lExcludedAdministrativeEntities = new List<PopulationDataEntry>();
        public List<PopulationDataEntry> ExcludedAdministrativeEntities
        { get { return lExcludedAdministrativeEntities; } }

        public Int32 NumberOfSeats { get; set; }
        public Int32 Index { get; set; }
        #endregion

        #region constructor
        public ConstituencyEntry()
        {
            NumberOfSeats = 1;
        }
        public ConstituencyEntry(ConstituencyEntry value)
        {
            NumberOfSeats = value.NumberOfSeats;
            Index = value.Index;
            foreach (PopulationDataEntry lSubEntity in value.AdministrativeEntities)
            {
                AdministrativeEntities.Add((PopulationDataEntry)lSubEntity.Clone());
            }
            foreach (PopulationDataEntry lSubEntity in value.ExcludedAdministrativeEntities)
            {
                ExcludedAdministrativeEntities.Add((PopulationDataEntry)lSubEntity.Clone());
            }
        }
        #endregion

        #region methods
        public Int32 Population()
        {
            Int32 lResult = 0;
            foreach (PopulationDataEntry lEntry in AdministrativeEntities)
            {
                lResult += lEntry.Total;
            }
            foreach (PopulationDataEntry lEntry in ExcludedAdministrativeEntities)
            {
                lResult -= lEntry.Total;
            }
            return lResult;
        }

        internal static ConstituencyEntry Load(XmlNode iNode)
        {
            ConstituencyEntry RetVal = null;

            if (iNode != null && iNode.Name.Equals("constituency"))
            {
                RetVal = new ConstituencyEntry();
                RetVal.Index = TambonHelper.GetAttributeOptionalInt(iNode, "index",0);
                RetVal.NumberOfSeats = TambonHelper.GetAttributeOptionalInt(iNode, "numberofseats", 1);

                if (iNode.HasChildNodes)
                {
                    foreach (XmlNode lChildNode in iNode.ChildNodes)
                    {
                        if (lChildNode.Name == "include")
                        {
                            PopulationDataEntry lEntity = new PopulationDataEntry();
                            lEntity.Geocode = TambonHelper.GetAttributeOptionalInt(lChildNode,"geocode",0);
                            foreach (XmlNode lSubChildNode in lChildNode.ChildNodes)
                            {
                                if (lSubChildNode.Name == "exclude")
                                {
                                    PopulationDataEntry lExcludedEntity = new PopulationDataEntry();
                                    lExcludedEntity.Geocode = TambonHelper.GetAttributeOptionalInt(lSubChildNode, "geocode", 0);
                                    RetVal.ExcludedAdministrativeEntities.Add(lExcludedEntity);
                                }
                            }
                            RetVal.AdministrativeEntities.Add(lEntity);
                        }
                    }
                }
            }
            return RetVal;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new ConstituencyEntry(this);
        }

        #endregion

    }
}
