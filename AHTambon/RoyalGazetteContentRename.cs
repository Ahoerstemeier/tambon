using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class RoyalGazetteContentRename : RoyalGazetteContent
    {
        internal const String XmlLabel = "rename";

        #region properties
        public String OldName { get; set; }
        public String OldEnglish { get; set; }
        #endregion

        #region methods
        internal override void DoLoad(XmlNode iNode)
        {
            base.DoLoad(iNode);
            if (iNode != null && iNode.Name.Equals(XmlLabel))
            {
                OldName = TambonHelper.GetAttribute(iNode, "oldname");
                OldEnglish = TambonHelper.GetAttributeOptionalString(iNode, "oldenglish");
                Name = TambonHelper.GetAttribute(iNode, "name");
            }
        }
        protected override void DoCopy(RoyalGazetteContent iOther)
        {
            if (iOther != null)
            {
                base.DoCopy(iOther);
                if (iOther is RoyalGazetteContentRename)
                {
                    RoyalGazetteContentRename iOtherRename = (RoyalGazetteContentRename)iOther;
                    OldName = iOtherRename.OldName;
                    OldEnglish = iOtherRename.OldEnglish;
                }
            }
        }
        protected override String GetXmlLabel()
        {
            return XmlLabel;
        }
        override protected void WriteToXmlElement(XmlElement iElement)
        {
            base.WriteToXmlElement(iElement);
            if (!String.IsNullOrEmpty(OldName))
            {
                iElement.SetAttribute("oldname", OldName);
            }
            if (!String.IsNullOrEmpty(OldEnglish))
            {
                iElement.SetAttribute("oldenglish", OldEnglish);
            }
        }
        #endregion
    }
}
