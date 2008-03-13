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
        public String NewName { get; set; }
        public String OldEnglish { get; set; }
        public String NewEnglish { get; set; }
        #endregion
        internal override void DoLoad(XmlNode iNode)
        {
            base.DoLoad(iNode);
            if (iNode != null && iNode.Name.Equals(XmlLabel))
            {
                OldName = Helper.GetAttribute(iNode, "oldname");
                NewName = Helper.GetAttribute(iNode, "name");
                NewEnglish = Helper.GetAttributeOptionalString(iNode, "english");
                OldEnglish = Helper.GetAttributeOptionalString(iNode, "oldenglish");
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
                    NewName = iOtherRename.NewName;
                    OldEnglish = iOtherRename.OldEnglish;
                    NewEnglish = iOtherRename.NewEnglish;
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
            if (!String.IsNullOrEmpty(NewName))
            {
                iElement.SetAttribute("name", NewName);
            }
            if (!String.IsNullOrEmpty(NewEnglish))
            {
                iElement.SetAttribute("english", NewEnglish);
            }
        }
        
    }
}
