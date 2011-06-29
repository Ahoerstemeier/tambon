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
        private Dictionary<PopulationDataEntry, List<PopulationDataEntry>> lExcludedAdministrativeEntities = new Dictionary<PopulationDataEntry, List<PopulationDataEntry>>();
        public Dictionary<PopulationDataEntry, List<PopulationDataEntry>> ExcludedAdministrativeEntities
        { get { return lExcludedAdministrativeEntities; } }
        private Dictionary<PopulationDataEntry, List<PopulationDataEntry>> lSubIncludedAdministrativeEntities = new Dictionary<PopulationDataEntry, List<PopulationDataEntry>>();
        public Dictionary<PopulationDataEntry, List<PopulationDataEntry>> SubIncludedAdministrativeEntities
        { get { return lSubIncludedAdministrativeEntities; } }

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
            foreach ( PopulationDataEntry lSubEntity in value.AdministrativeEntities )
            {
                AdministrativeEntities.Add((PopulationDataEntry)lSubEntity.Clone());
            }
            foreach ( var lKeyValuePair in value.ExcludedAdministrativeEntities )
            {
                ExcludedAdministrativeEntities[lKeyValuePair.Key] = new List<PopulationDataEntry>();
                foreach ( PopulationDataEntry lSubEntity in lKeyValuePair.Value )
                {
                    ExcludedAdministrativeEntities[lKeyValuePair.Key].Add((PopulationDataEntry)lSubEntity.Clone());
                }
            }
            foreach (var lKeyValuePair in value.SubIncludedAdministrativeEntities)
            {
                SubIncludedAdministrativeEntities[lKeyValuePair.Key] = new List<PopulationDataEntry>();
                foreach (PopulationDataEntry lSubEntity in lKeyValuePair.Value)
                {
                    SubIncludedAdministrativeEntities[lKeyValuePair.Key].Add((PopulationDataEntry)lSubEntity.Clone());
                }
            }
        }
        #endregion

        #region methods
        public Int32 Population()
        {
            Int32 lResult = 0;
            foreach ( PopulationDataEntry lEntry in AdministrativeEntities )
            {
                lResult += lEntry.Total;
            }
            foreach ( var lKeyValuePair in ExcludedAdministrativeEntities )
            {
                foreach ( PopulationDataEntry lEntry in lKeyValuePair.Value )
                {
                    lResult -= lEntry.Total;
                }
            }
            foreach (var lKeyValuePair in SubIncludedAdministrativeEntities)
            {
                foreach (PopulationDataEntry lEntry in lKeyValuePair.Value)
                {
                    lResult += lEntry.Total;
                }
            }
            return lResult;
        }

        internal static ConstituencyEntry Load(XmlNode iNode)
        {
            ConstituencyEntry RetVal = null;

            if ( iNode != null && iNode.Name.Equals("constituency") )
            {
                RetVal = new ConstituencyEntry();
                RetVal.Index = TambonHelper.GetAttributeOptionalInt(iNode, "index", 0);
                RetVal.NumberOfSeats = TambonHelper.GetAttributeOptionalInt(iNode, "numberofseats", 1);

                if ( iNode.HasChildNodes )
                {
                    foreach ( XmlNode lChildNode in iNode.ChildNodes )
                    {
                        if ( lChildNode.Name == "include" )
                        {
                            PopulationDataEntry lEntity = new PopulationDataEntry();
                            lEntity.Geocode = TambonHelper.GetAttributeOptionalInt(lChildNode, "geocode", 0);
                            foreach ( XmlNode lSubChildNode in lChildNode.ChildNodes )
                            {
                                if ( lSubChildNode.Name == "exclude" )
                                {
                                    PopulationDataEntry lExcludedEntity = new PopulationDataEntry();
                                    lExcludedEntity.Geocode = TambonHelper.GetAttributeOptionalInt(lSubChildNode, "geocode", 0);
                                    if ( !RetVal.ExcludedAdministrativeEntities.ContainsKey(lEntity) )
                                    {
                                        RetVal.ExcludedAdministrativeEntities[lEntity] = new List<PopulationDataEntry>();
                                    }
                                    RetVal.ExcludedAdministrativeEntities[lEntity].Add(lExcludedEntity);
                                }
                            }
                            RetVal.AdministrativeEntities.Add(lEntity);
                        }
                        if ( lChildNode.Name == "includesub" )
                        {
                            PopulationDataEntry lEntity = new PopulationDataEntry();
                            lEntity.Geocode = TambonHelper.GetAttributeOptionalInt(lChildNode, "geocode", 0);
                            foreach ( XmlNode lSubChildNode in lChildNode.ChildNodes )
                            {
                                if ( lSubChildNode.Name == "include" )
                                {
                                    PopulationDataEntry lIncludedEntity = new PopulationDataEntry();
                                    lIncludedEntity.Geocode = TambonHelper.GetAttributeOptionalInt(lSubChildNode, "geocode", 0);
                                    if ( !RetVal.SubIncludedAdministrativeEntities.ContainsKey(lEntity) )
                                    {
                                        RetVal.SubIncludedAdministrativeEntities[lEntity] = new List<PopulationDataEntry>();
                                    }
                                    RetVal.SubIncludedAdministrativeEntities[lEntity].Add(lIncludedEntity);
                                }
                            }
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
