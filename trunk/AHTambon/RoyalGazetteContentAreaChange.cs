using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class RoyalGazetteContentAreaChange:RoyalGazetteContent
    {
        internal const String XmlLabel = "areachange";

        #region properties
        public Single Area { get; set; }
        #endregion

        #region methods
        protected override String GetXmlLabel()
        {
            return XmlLabel;
        }
        internal override void DoLoad(XmlNode iNode)
        {
            base.DoLoad(iNode);
            if (iNode != null && iNode.Name.Equals(XmlLabel))
            {
                String AreaString = TambonHelper.GetAttributeOptionalString(iNode, "newarea");
                if (!String.IsNullOrEmpty(AreaString))
                {
                    Area = Convert.ToSingle(AreaString);
                }
            }
        }
        protected override void DoCopy(RoyalGazetteContent iOther)
        {
            if (iOther != null)
            {
                base.DoCopy(iOther);
                if (iOther is RoyalGazetteContentAreaChange)
                {
                    RoyalGazetteContentAreaChange iOtherArea = (RoyalGazetteContentAreaChange)iOther;
                    Area = iOtherArea.Area;
                }
            }
        }
        override protected void WriteToXmlElement(XmlElement iElement)
        {
            base.WriteToXmlElement(iElement);
            if (Area != 0)
            {
                iElement.SetAttribute("newarea", Area.ToString());
            }
        }
        #endregion
    }
}
