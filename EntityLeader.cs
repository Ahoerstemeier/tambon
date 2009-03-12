using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class EntityLeader
    {
        #region properties
        public String Name { get; set; }
        public PersonTitle Title { get; set; }
        public String Telephone { get; set; }
        public String CellPhone { get; set; }
        public EntityLeaderType Position { get; set; }
        #endregion

        public void ExportToXML(System.Xml.XmlElement iNode)
        {
            XmlDocument lXmlDocument = Helper.XmlDocumentFromNode(iNode);
            var lNewElement = (XmlElement)lXmlDocument.CreateNode("element", "official", "");
            lNewElement.SetAttribute("title", Position.ToString());
            lNewElement.SetAttribute("name", Name);
            if (Title != PersonTitle.Unknown)
            {
                lNewElement.SetAttribute("nametitle", Title.ToString());
            }
            lNewElement.SetAttribute("telephone", Telephone);
            lNewElement.SetAttribute("cellphone", CellPhone);
            iNode.AppendChild(lNewElement);
        }
    }
}
