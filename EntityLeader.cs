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
        private String mName = String.Empty;
        public String Name { get { return mName; } set { SetName(value); } }
        public PersonTitle Title { get; set; }
        public String Telephone { get; set; }
        public String CellPhone { get; set; }
        public EntityLeaderType Position { get; set; }
        #endregion
        private void SetName(String iName)
        {
            mName = iName;
            foreach (KeyValuePair<String,PersonTitle> lEntry in Helper.PersonTitleStrings)
            {
                String lSearch = lEntry.Key;
                if (iName.StartsWith(lSearch))
                {
                    Title = lEntry.Value;
                    mName = iName.Remove(0, lSearch.Length);
                }
            }
            // TODO Strip persontitle and store it separately in Title property
        }
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
            if (!String.IsNullOrEmpty(Telephone))
            {
                lNewElement.SetAttribute("telephone", Telephone);
            }
            if (!String.IsNullOrEmpty(CellPhone))
            {
                lNewElement.SetAttribute("cellphone", CellPhone);
            }
            iNode.AppendChild(lNewElement);
        }
    }
}
