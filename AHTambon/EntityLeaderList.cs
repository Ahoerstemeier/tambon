using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    public class EntityLeaderList : List<EntityLeader>, ICloneable
    {
        #region constructor
        public EntityLeaderList()
        {
        }
        public EntityLeaderList(EntityLeaderList iValue)
        {
            foreach (EntityLeader lLeader in iValue)
            {
                this.Add((EntityLeader)lLeader.Clone());
            }
        }
        #endregion

        #region properties
        public String Source { get; set; }
        #endregion

        #region methods
        public void ExportToXML(XmlElement iNode)
        {
            XmlDocument lXmlDocument = TambonHelper.XmlDocumentFromNode(iNode);
            var lNewElement = (XmlElement)lXmlDocument.CreateNode("element", "officials", "");
            if (!String.IsNullOrEmpty(Source))
            {
                lNewElement.SetAttribute("source", Source);
            }
            iNode.AppendChild(lNewElement);
            foreach (EntityLeader lEntry in this)
            {
                lEntry.ExportToXML(lNewElement);
            }
        }
        public static EntityLeaderList Load(XmlNode iNode)
        {
            EntityLeaderList RetVal = null;

            if (iNode != null && iNode.Name.Equals("officials"))
            {
                RetVal = new EntityLeaderList();
                RetVal.Source = TambonHelper.GetAttributeOptionalString(iNode, "source");

                if (iNode.HasChildNodes)
                {
                    foreach (XmlNode lChildNode in iNode.ChildNodes)
                    {
                        EntityLeader lCurrent = EntityLeader.Load(lChildNode);
                        if (lCurrent != null)
                        {
                            RetVal.Add(lCurrent);
                        }
                    }
                }
            }
            return RetVal;
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new EntityLeaderList(this);
        }

        #endregion
    }
}
