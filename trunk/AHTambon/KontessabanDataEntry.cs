using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    public class KontessabanDataEntry : PopulationDataEntry
    {
        // TODO: Address String -> ThaiAddress object
        // TODO: Zugehörigen Geocode suchen (benötigt die geparste Addresse für Changwat-Namen)
        public Int32 KontessabanIndex { get; set; }
        protected ThaiAddress mAddress = new ThaiAddress();
        public ThaiAddress Address { get { return mAddress; } }
        public String FAX { get; set; }
        public String Website { get; set; }
        public String Telephone { get; set; }

        internal override void WriteToXMLNode(XmlElement iNode)
        {
            base.WriteToXMLNode(iNode);
            iNode.SetAttribute("parent", Address.Geocode.ToString());
            iNode.SetAttribute("TessabanIndex", KontessabanIndex.ToString());

            XmlDocument lXmlDocument = TambonHelper.XmlDocumentFromNode(iNode);
            var lNewElement = (XmlElement)lXmlDocument.CreateNode("element", "address", "");
            iNode.AppendChild(lNewElement);
            lNewElement.SetAttribute("Address", Address.PlainValue);
            lNewElement.SetAttribute("URL", Website);
            lNewElement.SetAttribute("Telephone", Telephone);
            lNewElement.SetAttribute("Fax", FAX);
        }

        internal void FinishEntry()
        {
            Address.CalcGeocode();
            Geocode = TambonHelper.GetGeocode(Address.Changwat, Name, EntityType.Thesaban);
        }
    }
}
