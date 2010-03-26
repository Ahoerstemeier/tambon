using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class RoyalGazetteContentReassign:RoyalGazetteContent
    {
        internal const String XmlLabel = "reassign";

        #region properties
        public Int32 OldGeocode { get; set; }
        public Int32 OldParent { get; set; }
        #endregion

        #region methods
        internal override void DoLoad(XmlNode iNode)
        {
            base.DoLoad(iNode);
            if (iNode != null && iNode.Name.Equals(XmlLabel))
            {
                OldGeocode = TambonHelper.GetAttributeOptionalInt(iNode, "oldgeocode",0);
                OldParent = TambonHelper.GetAttributeOptionalInt(iNode, "oldparent",0);
            }
        }
        protected override void DoCopy(RoyalGazetteContent iOther)
        {
            if (iOther != null)
            {
                base.DoCopy(iOther);
                if (iOther is RoyalGazetteContentReassign)
                {
                    RoyalGazetteContentReassign iOtherReassign = (RoyalGazetteContentReassign)iOther;
                    OldGeocode = iOtherReassign.OldGeocode;
                    OldParent = iOtherReassign.OldParent;
                }
            }
        }
        override protected void WriteToXmlElement(XmlElement iElement)
        {
            base.WriteToXmlElement(iElement);
            if (OldGeocode != 0)
            {
                iElement.SetAttribute("oldgeocode", OldGeocode.ToString());
            }
            if (OldParent != 0)
            {
                iElement.SetAttribute("oldparent", OldParent.ToString());
            }
        }
        protected override String GetXmlLabel()
        {
            return XmlLabel;
        }
        #endregion

        #region IGeocode Members
        public override bool IsAboutGeocode(Int32 iGeocode, Boolean iIncludeSubEntities)
        {
            Boolean retval = TambonHelper.IsSameGeocode(iGeocode, Geocode, iIncludeSubEntities);
            retval = retval | TambonHelper.IsSameGeocode(iGeocode, OldGeocode, iIncludeSubEntities);
            return retval;
        }
        #endregion
    }
}
