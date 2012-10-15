using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class RoyalGazetteContentConstituency:RoyalGazetteContent
    {
        internal const String XmlLabel = "constituency";

        #region properties
        public EntityType Type
        {
            get;
            set;
        }
        #endregion

        #region constructor
        public RoyalGazetteContentConstituency()
        {
            Type = EntityType.Unknown;
        }
        public RoyalGazetteContentConstituency(RoyalGazetteContentConstituency other)
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
            }
        }
        override protected void WriteToXmlElement(XmlElement iElement)
        {
            base.WriteToXmlElement(iElement);
            if (Type != EntityType.Unknown)
            {
                iElement.SetAttribute("type", Type.ToString());
            }
        }
        protected override void DoCopy(RoyalGazetteContent iOther)
        {
            if (iOther != null)
            {
                base.DoCopy(iOther);
                if (iOther is RoyalGazetteContentConstituency)
                {
                    RoyalGazetteContentConstituency iOtherConstituency = (RoyalGazetteContentConstituency)iOther;
                    Type = iOtherConstituency.Type;
                }
            }
        }
        #endregion
    }
}
