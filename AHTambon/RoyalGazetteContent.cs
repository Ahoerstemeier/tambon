using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    public abstract class RoyalGazetteContent : ICloneable, IGeocode
    {
        protected List<RoyalGazetteContent> mSubEntries = new List<RoyalGazetteContent>();

        #region properties
        public Int32 Geocode { get; set; }
        public Int32 TambonGeocode { get; set; }
        public Int32 Owner { get; set; }
        public String Name { get; set; }
        public String English { get; set; }
        #endregion

        #region methods
        virtual internal void DoLoad(XmlNode iNode)
        {
            if ( iNode != null )
            {
                Geocode = TambonHelper.GetAttributeOptionalInt(iNode, "geocode", 0);
                TambonGeocode = TambonHelper.GetAttributeOptionalInt(iNode, "tambon", 0);
                Name = TambonHelper.GetAttributeOptionalString(iNode, "name");
                English = TambonHelper.GetAttributeOptionalString(iNode, "english");
                Owner = TambonHelper.GetAttributeOptionalInt(iNode, "owner", 0);
                foreach ( XmlNode lNode in iNode.ChildNodes )
                {
                    var lContent = RoyalGazetteContent.CreateContentObject(lNode.Name);
                    if ( lContent != null )
                    {
                        lContent.DoLoad(lNode);
                        mSubEntries.Add(lContent);
                    }
                }
            }
        }

        protected virtual void DoCopy(RoyalGazetteContent iOther)
        {
            if ( iOther != null )
            {
                Geocode = iOther.Geocode;
                TambonGeocode = iOther.TambonGeocode;
                Owner = iOther.Owner;
                Name = iOther.Name;
                English = iOther.English;
            }
            foreach ( RoyalGazetteContent lContent in iOther.mSubEntries )
            {
                RoyalGazetteContent lNewContent = (RoyalGazetteContent)lContent.Clone();
                mSubEntries.Add(lNewContent);
            }
        }

        protected abstract String GetXmlLabel();
        internal void ExportToXML(XmlNode iNode)
        {
            XmlDocument lXmlDocument = TambonHelper.XmlDocumentFromNode(iNode);
            var lNewElement = (XmlElement)lXmlDocument.CreateNode("element", GetXmlLabel(), "");
            WriteToXmlElement(lNewElement);
            iNode.AppendChild(lNewElement);
        }
        virtual protected void WriteToXmlElement(XmlElement iElement)
        {
            if ( Geocode != 0 )
            {
                iElement.SetAttribute("geocode", Geocode.ToString());
            }
            if ( TambonGeocode != 0 )
            {
                iElement.SetAttribute("tambon", TambonGeocode.ToString());
            }
            if ( Owner != 0 )
            {
                iElement.SetAttribute("owner", Owner.ToString());
            }
            if ( !String.IsNullOrEmpty(Name) )
            {
                iElement.SetAttribute("name", Name.ToString());
            }
            if ( !String.IsNullOrEmpty(English) )
            {
                iElement.SetAttribute("english", English.ToString());
            }
            foreach ( RoyalGazetteContent lContent in mSubEntries )
            {
                lContent.ExportToXML(iElement);
            }

        }

        static internal RoyalGazetteContent CreateContentObject(String iName)
        {
            RoyalGazetteContent retval = null;
            switch ( iName )
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
                case RoyalGazetteContentAreaDefinition.XmlLabel:
                    {
                        retval = new RoyalGazetteContentAreaDefinition();
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
                case RoyalGazetteContentConstituency.XmlLabel:
                    {
                        retval = new RoyalGazetteContentConstituency();
                        break;
                    }
                case RoyalGazetteContentMention.XmlLabel:
                    {
                        retval = new RoyalGazetteContentMention();
                        break;
                    }
                case RoyalGazetteContentCapital.XmlLabel:
                    {
                        retval = new RoyalGazetteContentCapital();
                        break;
                    }

            }
            return retval;
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            object lNewObject = Activator.CreateInstance(this.GetType());
            RoyalGazetteContent lNewContent = lNewObject as RoyalGazetteContent;
            lNewContent.DoCopy(this);
            return lNewContent;
        }

        #endregion

        #region IGeocode Members

        public virtual Boolean IsAboutGeocode(Int32 iGeocode, Boolean iIncludeSubEntities)
        {
            Boolean retval = TambonHelper.IsSameGeocode(iGeocode, Geocode, iIncludeSubEntities);
            retval = retval | TambonHelper.IsSameGeocode(iGeocode, Owner, iIncludeSubEntities);
            retval = retval | TambonHelper.IsSameGeocode(iGeocode, TambonGeocode, iIncludeSubEntities);
            foreach ( RoyalGazetteContent lContent in mSubEntries )
            {
                retval = retval | lContent.IsAboutGeocode(iGeocode, iIncludeSubEntities);
            }
            return retval;
        }

        #endregion
    }
}
