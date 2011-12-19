using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class RoyalGazetteContentAreaDefinition : RoyalGazetteContent
    {
        internal const String XmlLabel = "areadefinition";

        #region properties
        public Int32 NumberOfSubdivision { get; set; }
        #endregion

        #region methods
        protected override String GetXmlLabel()
        {
            return XmlLabel;
        }
        internal override void DoLoad(XmlNode node)
        {
            base.DoLoad(node);
            if ( node != null && node.Name.Equals(XmlLabel) )
            {
                String SubdivisionsString = TambonHelper.GetAttributeOptionalString(node, "subdivisions");
                if ( !String.IsNullOrEmpty(SubdivisionsString) )
                {
                    NumberOfSubdivision = Convert.ToInt32(SubdivisionsString);
                }
            }
        }
        protected override void DoCopy(RoyalGazetteContent other)
        {
            if ( other != null )
            {
                base.DoCopy(other);
                if ( other is RoyalGazetteContentAreaDefinition )
                {
                    RoyalGazetteContentAreaDefinition iOtherArea = (RoyalGazetteContentAreaDefinition)other;
                    NumberOfSubdivision = iOtherArea.NumberOfSubdivision;
                }
            }
        }
        override protected void WriteToXmlElement(XmlElement element)
        {
            base.WriteToXmlElement(element);
            if ( NumberOfSubdivision != 0 )
            {
                element.SetAttribute("subdivisions", NumberOfSubdivision.ToString());
            }
        }
        #endregion
    }
}
