using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class RoyalGazetteContentAbolish:RoyalGazetteContent
    {
        internal const String XmlLabel = "abolish";
        #region properties
        public EntityType Status { get; set; }
        public Int32 NewParent { get; set; }
        public List<RoyalGazetteContent> SubEntities
        {
            get { return mSubEntities; }
        }
        #endregion
        internal override void DoLoad(XmlNode iNode)
        {
            base.DoLoad(iNode);
            if (iNode != null && iNode.Name.Equals(XmlLabel))
            {
                String s = TambonHelper.GetAttributeOptionalString(iNode, "type");
                if (!String.IsNullOrEmpty(s))
                {
                    Status = (EntityType)Enum.Parse(typeof(EntityType), s);
                }
                NewParent = TambonHelper.GetAttributeOptionalInt(iNode, "parent",0);
                foreach (XmlNode lNode in iNode.ChildNodes)
                {
                    if (lNode.Name == RoyalGazetteContentReassign.XmlLabel)
                    {
                        var lContent = new RoyalGazetteContentReassign();
                        lContent.DoLoad(lNode);
                        mSubEntities.Add(lContent);
                    }

                }
            }
        }
        protected override String GetXmlLabel()
        {
            return XmlLabel;
        }
        protected override void DoCopy(RoyalGazetteContent iOther)
        {
            if (iOther != null)
            {
                base.DoCopy(iOther);
                if (iOther is RoyalGazetteContentAbolish)
                {
                    RoyalGazetteContentAbolish iOtherAbolish = (RoyalGazetteContentAbolish)iOther;
                    Status = iOtherAbolish.Status;
                    NewParent = iOtherAbolish.NewParent;
                    Name = iOtherAbolish.Name;
                    English = iOtherAbolish.English;
                }
            }
        }
        override protected void WriteToXmlElement(XmlElement iElement)
        {
            base.WriteToXmlElement(iElement);
            if (Status != EntityType.Unknown)
            {
                iElement.SetAttribute("type", Status.ToString());
            }
            if (NewParent != 0)
            {
                iElement.SetAttribute("parent", NewParent.ToString());
            }
            if (!String.IsNullOrEmpty(Name))
            {
                iElement.SetAttribute("name", Name);
            }
            if (!String.IsNullOrEmpty(English))
            {
                iElement.SetAttribute("name", English);
            }
        }
    }
}
