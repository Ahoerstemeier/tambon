using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class RoyalGazetteContentCapital:RoyalGazetteContent
    {
        internal const String XmlLabel = "capital";
        #region properties
        public EntityType Type 
        {
            get;
            set;
        }
        public EntityType TypeCapital 
        {
            get;
            set;
        }
        public Int32 GeocodeCapital { get; set; }
        public Int32 GeocodeCapitalOld { get; set; }
        public String CapitalName { get; set; }
        public String CapitalNameOld { get; set; }
        public String CapitalEnglish { get; set; }
        public String CapitalEnglishOld { get; set; }
        #endregion

        #region constructor
        public RoyalGazetteContentCapital()
        {
            Type = EntityType.Unknown;
            TypeCapital = EntityType.Unknown;
        }
        public RoyalGazetteContentCapital(RoyalGazetteContentCapital other)
        {
            DoCopy(other);
        }

        #endregion

        #region methods
        protected override String GetXmlLabel()
        {
            return XmlLabel;
        }
        override internal void DoLoad(XmlNode iNode)
        {
            base.DoLoad(iNode);
            if (iNode != null && iNode.Name.Equals(XmlLabel))
            {
                string lTypeString = TambonHelper.GetAttributeOptionalString(iNode, "type");
                if (!String.IsNullOrEmpty(lTypeString))
                {
                    Type = (EntityType)Enum.Parse(typeof(EntityType), lTypeString);
                }
                lTypeString = TambonHelper.GetAttributeOptionalString(iNode, "capitaltype");
                if (!String.IsNullOrEmpty(lTypeString))
                {
                    TypeCapital = (EntityType)Enum.Parse(typeof(EntityType), lTypeString);
                }
                GeocodeCapital = TambonHelper.GetAttributeOptionalInt(iNode,"capitalgeocode",0);
                GeocodeCapitalOld = TambonHelper.GetAttributeOptionalInt(iNode, "oldcapitalgeocode", 0);
                CapitalName = TambonHelper.GetAttributeOptionalString(iNode, "capitalname");
                CapitalNameOld = TambonHelper.GetAttributeOptionalString(iNode,"oldcapitalname");
                CapitalEnglish = TambonHelper.GetAttributeOptionalString(iNode, "capitalenglish");
                CapitalEnglishOld = TambonHelper.GetAttributeOptionalString(iNode, "oldcapitalenglish");
            }
        }
        override protected void WriteToXmlElement(XmlElement iElement)
        {
            base.WriteToXmlElement(iElement);
            if (Type != EntityType.Unknown)
            {
                iElement.SetAttribute("type", Type.ToString());
            }
            if (TypeCapital != EntityType.Unknown)
            {
                iElement.SetAttribute("capitaltype", TypeCapital.ToString());
            }
            if (!String.IsNullOrEmpty(CapitalName))
            {
                iElement.SetAttribute("capitalname", CapitalName);
            }
            if (!String.IsNullOrEmpty(CapitalEnglish))
            {
                iElement.SetAttribute("capitalname", CapitalEnglish);
            }
            if (GeocodeCapital != 0)
            {
                iElement.SetAttribute("capitalgeocode", GeocodeCapital.ToString());
            }
            if (!String.IsNullOrEmpty(CapitalNameOld))
            {
                iElement.SetAttribute("capitalname", CapitalNameOld);
            }
            if (!String.IsNullOrEmpty(CapitalEnglishOld))
            {
                iElement.SetAttribute("capitalname", CapitalEnglishOld);
            }
            if (GeocodeCapitalOld != 0)
            {
                iElement.SetAttribute("capitalgeocode", GeocodeCapitalOld.ToString());
            }
        }
        protected override void DoCopy(RoyalGazetteContent iOther)
        {
            if (iOther != null)
            {
                base.DoCopy(iOther);
                if (iOther is RoyalGazetteContentCapital)
                {
                    RoyalGazetteContentCapital iOtherCapital = (RoyalGazetteContentCapital)iOther;
                    Type = iOtherCapital.Type;
                    TypeCapital = iOtherCapital.TypeCapital;
                    GeocodeCapital = iOtherCapital.GeocodeCapital;
                    GeocodeCapitalOld = iOtherCapital.GeocodeCapitalOld;
                    CapitalName = iOtherCapital.CapitalName;
                    CapitalNameOld = iOtherCapital.CapitalNameOld;
                    CapitalEnglish = iOtherCapital.CapitalEnglish;
                    CapitalEnglishOld = iOtherCapital.CapitalEnglishOld;
                }
            }
        }
        public override bool IsAboutGeocode(Int32 iGeocode, Boolean iIncludeSubEntities)
        {
            Boolean retval = TambonHelper.IsSameGeocode(iGeocode, Geocode, iIncludeSubEntities);
            retval = retval | TambonHelper.IsSameGeocode(iGeocode, GeocodeCapital, iIncludeSubEntities);
            retval = retval | TambonHelper.IsSameGeocode(iGeocode, GeocodeCapitalOld, iIncludeSubEntities);
            return retval;
        }
        #endregion
    }
}
