using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    abstract class RoyalGazetteContent : ICloneable, IGeocode
    {
        protected List<RoyalGazetteContent> mSubEntities = new List<RoyalGazetteContent>();
        #region properties
        public Int32 Geocode { get; set; }
        public Int32 Owner { get; set; }
        public String Name { get; set; }
        public String English { get; set; }
        #endregion
        #region methods
        virtual internal void DoLoad(XmlNode iNode)
        {
            if (iNode != null)
            {
                Geocode = Helper.GetAttributeOptionalInt(iNode, "geocode",0);
                Name = Helper.GetAttributeOptionalString(iNode, "name");
                English = Helper.GetAttributeOptionalString(iNode, "english");
                Owner = Helper.GetAttributeOptionalInt(iNode, "owner", 0);
                foreach (XmlNode lNode in iNode.ChildNodes)
                {
                    var lContent = RoyalGazetteContent.CreateContentObject(lNode.Name);
                    if (lContent != null)
                    {
                        lContent.DoLoad(lNode);
                        mSubEntities.Add(lContent);
                    }
                }
            }
        }

        protected virtual void DoCopy(RoyalGazetteContent iOther)
        {
            if (iOther != null)
            {
                Geocode = iOther.Geocode;
                Owner = iOther.Owner;
                Name = iOther.Name;
                English = iOther.English;
            }
            foreach (RoyalGazetteContent lContent in iOther.mSubEntities)
            {
                RoyalGazetteContent lNewContent = (RoyalGazetteContent)lContent.Clone();
                mSubEntities.Add(lNewContent);
            }
        }

        protected abstract String GetXmlLabel();
        internal void ExportToXML(XmlNode iNode)
        {
            XmlDocument lXmlDocument = Helper.XmlDocumentFromNode(iNode);
            var lNewElement = (XmlElement)lXmlDocument.CreateNode("element", GetXmlLabel(), "");
            WriteToXmlElement(lNewElement);
            iNode.AppendChild(lNewElement);
        }
        virtual protected void WriteToXmlElement(XmlElement iElement)
        {
            if (Geocode != 0)
            {
                iElement.SetAttribute("geocode", Geocode.ToString());
            }
            if (Owner != 0)
            {
                iElement.SetAttribute("owner", Owner.ToString());
            }
            if (!String.IsNullOrEmpty(Name))
            {
                iElement.SetAttribute("name", Name.ToString());
            }
            if (!String.IsNullOrEmpty(English))
            {
                iElement.SetAttribute("english", English.ToString());
            }
            foreach (RoyalGazetteContent lContent in mSubEntities)
            {
                lContent.ExportToXML(iElement);
            }

        }

        static internal RoyalGazetteContent CreateContentObject(String iName)
        {
            RoyalGazetteContent retval = null;
            switch (iName)
            {
                case RoyalGazetteContentRename.XmlLabel:
                    {
                        retval = new RoyalGazetteContentRename();
                        break;
                    }
                case RoyalGazetteContentStatus.XmlLabel:
                    {
                        retval = new RoyalGazetteContentStatus();
                        break;
                    }
                case RoyalGazetteContentCreate.XmlLabel:
                    {
                        retval = new RoyalGazetteContentCreate();
                        break;
                    }
                case RoyalGazetteContentAreaChange.XmlLabel:
                    {
                        retval = new RoyalGazetteContentAreaChange();
                        break;
                    }
                case RoyalGazetteContentReassign.XmlLabel:
                    {
                        retval = new RoyalGazetteContentReassign();
                        break;
                    }
                case RoyalGazetteContentAbolish.XmlLabel:
                    {
                        retval = new RoyalGazetteContentAbolish();
                        break;
                    }
                
            }
            return retval;
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            object lNewObject  = Activator.CreateInstance( this.GetType() );
            RoyalGazetteContent lNewContent = lNewObject as RoyalGazetteContent;
            lNewContent.DoCopy(this);
            return lNewContent;
        }

        #endregion

        #region IGeocode Members

        public virtual Boolean IsAboutGeocode(Int32 iGeocode, Boolean iIncludeSubEntities)
        {
            Boolean retval = Helper.IsSameGeocode(iGeocode, Geocode, iIncludeSubEntities);
            retval = retval | Helper.IsSameGeocode(iGeocode, Owner, iIncludeSubEntities);
            foreach (RoyalGazetteContent lContent in mSubEntities)
            {
                retval = retval | lContent.IsAboutGeocode(iGeocode, iIncludeSubEntities);
            }
            return retval;
        }

        #endregion
    }
}
