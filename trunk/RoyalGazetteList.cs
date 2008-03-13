using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class RoyalGazetteList : List<RoyalGazette>
    {
        public delegate void ProcessingFinished(RoyalGazetteList data);
        #region constructor
        public RoyalGazetteList()
        {
        }
        #endregion
        #region methods
        public static RoyalGazetteList Load(String iFromFile)
        {
            StreamReader lReader = null;
            XmlDocument lXmlDoc = null;
            RoyalGazetteList RetVal = null;
            try
            {
                if (!String.IsNullOrEmpty(iFromFile) && File.Exists(iFromFile))
                {
                    lReader = new StreamReader(iFromFile);
                    lXmlDoc = new XmlDocument();
                    lXmlDoc.LoadXml(lReader.ReadToEnd());
                    RetVal = RoyalGazetteList.Load(lXmlDoc);
                }
            }
            finally
            {
                if (lReader != null)
                {
                    lReader.Close();
                }
            }
            return RetVal;
        }
        internal static void ParseNode(XmlNode iNode, RoyalGazetteList ioList)
        {
            if (iNode.HasChildNodes)
            {
                foreach (XmlNode lChildNode in iNode.ChildNodes)
                {
                    if (lChildNode.Name == "year")
                    {
                        ParseNode(lChildNode, ioList);
                    }
                    else if (lChildNode.Name == "decade")
                    {
                        ParseNode(lChildNode, ioList);
                    }
                    else if (lChildNode.Name == "entry")
                    {
                        ioList.Add(RoyalGazette.Load(lChildNode));
                    }

                }
            }

        }
        internal static RoyalGazetteList Load(XmlNode iNode)
        {
            RoyalGazetteList RetVal = null;

            if (iNode != null)
            {
                foreach (XmlNode lNode in iNode.ChildNodes)
                {
                    if (lNode != null && lNode.Name.Equals("gazette"))
                    {
                        RetVal = new RoyalGazetteList();
                        ParseNode(lNode,RetVal);
                    }
                }
            }

            return RetVal;
        }
        public void MirrorAllToCache()
        {
            foreach (RoyalGazette lEntry in this)
            {
                lEntry.MirrorToCache();
            }
        }
        public RoyalGazetteList AllAboutEntity(Int32 iGeocode, Boolean iIncludeSubEntities)
        {
            var retval = new RoyalGazetteList();
            foreach (RoyalGazette lEntry in this)
            {
                if (lEntry.IsAboutGeocode(iGeocode,iIncludeSubEntities))
                {
                    retval.Add(lEntry);
                }
            }
            return retval;
        }
        public void ExportToXML(XmlNode iNode)
        {
            XmlDocument lXmlDocument = Helper.XmlDocumentFromNode(iNode);
            var lNode = (XmlElement)lXmlDocument.CreateNode("element", "gazette", "");
            iNode.AppendChild(lNode);
            foreach (RoyalGazette lEntry in this)
            {
                lEntry.ExportToXML(lNode);
            }
        }
        public void SaveXML(string iFilename)
        {
            XmlDocument lXmlDocument = new XmlDocument();
            ExportToXML(lXmlDocument);
            lXmlDocument.Save(iFilename);
        }
        #endregion
    }
}
