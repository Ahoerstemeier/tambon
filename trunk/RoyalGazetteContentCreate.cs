using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class RoyalGazetteContentCreate : RoyalGazetteContent
    {
        internal const String XmlLabel = "create";
        #region properties
        public EntityType Status { get; set; }
        public Int32 Parent { get; set; }
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
                string s = Helper.GetAttribute(iNode, "type");
                if (!String.IsNullOrEmpty(s))
                {
                    Status = (EntityType)Enum.Parse(typeof(EntityType), s);
                }
                Parent = Helper.GetAttributeOptionalInt(iNode, "parent",0);

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
                if (iOther is RoyalGazetteContentCreate)
                {
                    RoyalGazetteContentCreate iOtherCreate = (RoyalGazetteContentCreate)iOther;
                    Status = iOtherCreate.Status;
                    Parent = iOtherCreate.Parent;
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
            if (Parent != 0)
            {
                iElement.SetAttribute("parent", Parent.ToString());
            }
        }
    }
}
