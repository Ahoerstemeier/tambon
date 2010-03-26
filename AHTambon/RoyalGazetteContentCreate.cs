using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    public class RoyalGazetteContentCreate : RoyalGazetteContent
    {
        internal const String XmlLabel = "create";

        #region properties
        private EntityType mType = EntityType.Unknown;
        public EntityType Type 
        {
            get { return mType; }
            set { mType = value; }
        }
        public Int32 Parent { get; set; }
        public List<RoyalGazetteContent> SubEntries
        {
            get { return mSubEntries; }
        }
        #endregion

        #region methods
        internal override void DoLoad(XmlNode iNode)
        {
            base.DoLoad(iNode);
            if (iNode != null && iNode.Name.Equals(XmlLabel))
            {
                string s = TambonHelper.GetAttribute(iNode, "type");
                if (!String.IsNullOrEmpty(s))
                {
                    Type = (EntityType)Enum.Parse(typeof(EntityType), s);
                }
                Parent = TambonHelper.GetAttributeOptionalInt(iNode, "parent",0);

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
                    Type = iOtherCreate.Type;
                    Parent = iOtherCreate.Parent;
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
            if (Parent != 0)
            {
                iElement.SetAttribute("parent", Parent.ToString());
            }
        }
        #endregion
    }
}
