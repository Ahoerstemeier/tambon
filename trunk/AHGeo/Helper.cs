using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Xml;

namespace De.AHoerstemeier.Geo
{
    public enum PositionInRectangle
    {
        TopLeft,
        TopMiddle,
        TopRight,
        BottomLeft,
        BottomMiddle,
        BottomRight,
        MiddleLeft,
        MiddleRight,
        MiddleMiddle,
    }

    internal class Helper
    {
        static public CultureInfo CultureInfoUS = new CultureInfo("en-us");
        internal static XmlDocument XmlDocumentFromNode(XmlNode node)
        {
            XmlDocument retval = null;

            if ( node is XmlDocument )
            {
                retval = (XmlDocument)node;
            }
            else
            {
                retval = node.OwnerDocument;
            }

            return retval;
        }
    }
}
