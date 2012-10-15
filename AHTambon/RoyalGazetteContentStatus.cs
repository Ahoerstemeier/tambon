using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class RoyalGazetteContentStatus : RoyalGazetteContent
    {
        internal const String XmlLabel = "status";
        #region properties
        public EntityType OldStatus 
        { 
            get;
            set;
        }
        public EntityType NewStatus 
        {
            get;
            set;
        }
        #endregion
        override internal void DoLoad(XmlNode iNode)
        {
            base.DoLoad(iNode);
            if (iNode != null && iNode.Name.Equals(XmlLabel))
            {
                string s = TambonHelper.GetAttribute(iNode, "old");
                if (!String.IsNullOrEmpty(s))
                {
                    OldStatus = (EntityType)Enum.Parse(typeof(EntityType), s);
                }
                s = TambonHelper.GetAttribute(iNode, "new");
                if (!String.IsNullOrEmpty(s))
                {
                    NewStatus = (EntityType)Enum.Parse(typeof(EntityType), s);
                }
            }
        }
        override protected void WriteToXmlElement(XmlElement iElement)
        {
            base.WriteToXmlElement(iElement);
            if (OldStatus != EntityType.Unknown)
            {
                iElement.SetAttribute("old", OldStatus.ToString());
            }
            if (NewStatus != EntityType.Unknown)
            {
                iElement.SetAttribute("new", NewStatus.ToString());
            }
        }
        #region constructor
        public RoyalGazetteContentStatus()
        {
        }
        public RoyalGazetteContentStatus(RoyalGazetteContentStatus other)
        {
            DoCopy(other);
        }

        #endregion
        protected override String GetXmlLabel()
        {
            return XmlLabel;
        }
        protected override void DoCopy(RoyalGazetteContent other)
        {
            if (other != null)
            {
                base.DoCopy(other);
                if (other is RoyalGazetteContentStatus)
                {
                    RoyalGazetteContentStatus iOtherStatus = (RoyalGazetteContentStatus)other;
                    OldStatus = iOtherStatus.OldStatus;
                    NewStatus = iOtherStatus.NewStatus;
                }
            }
        }

    }
}
