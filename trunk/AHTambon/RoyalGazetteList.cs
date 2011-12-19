using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.ServiceModel.Syndication;

namespace De.AHoerstemeier.Tambon
{
    public class RoyalGazetteList : List<RoyalGazette>
    {
        public delegate void ProcessingFinished(RoyalGazetteList data);
        public delegate void ProcessingFinishedFiltered(RoyalGazetteList data, Boolean filtered);

        private class URIComparer : IComparer<RoyalGazette>
        {
            public int Compare(RoyalGazette x, RoyalGazette y)
            {
                String lUriX = String.Empty;
                String lUriY = String.Empty;
                if ( x != null )
                {
                    lUriX = x.URI;
                }
                if ( y != null )
                {
                    lUriY = y.URI;
                }

                int lResult = lUriX.CompareTo(lUriY);
                return lResult;
            }
        }

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
                if ( !String.IsNullOrEmpty(iFromFile) && File.Exists(iFromFile) )
                {
                    lReader = new StreamReader(iFromFile);
                    lXmlDoc = new XmlDocument();
                    lXmlDoc.LoadXml(lReader.ReadToEnd());
                    RetVal = RoyalGazetteList.Load(lXmlDoc);
                }
            }
            finally
            {
                if ( lReader != null )
                {
                    lReader.Close();
                }
            }
            return RetVal;
        }
        internal static void ParseNode(XmlNode iNode, RoyalGazetteList ioList)
        {
            if ( iNode.HasChildNodes )
            {
                foreach ( XmlNode lChildNode in iNode.ChildNodes )
                {
                    if ( lChildNode.Name == "year" )
                    {
                        ParseNode(lChildNode, ioList);
                    }
                    else if ( lChildNode.Name == "decade" )
                    {
                        ParseNode(lChildNode, ioList);
                    }
                    else if ( lChildNode.Name == "month" )
                    {
                        ParseNode(lChildNode, ioList);
                    }
                    else if ( lChildNode.Name == "entry" )
                    {
                        ioList.Add(RoyalGazette.Load(lChildNode));
                    }

                }
            }

        }
        internal static RoyalGazetteList Load(XmlNode iNode)
        {
            RoyalGazetteList RetVal = null;

            if ( iNode != null )
            {
                foreach ( XmlNode lNode in iNode.ChildNodes )
                {
                    if ( lNode != null && lNode.Name.Equals("gazette") )
                    {
                        RetVal = new RoyalGazetteList();
                        ParseNode(lNode, RetVal);
                    }
                }
            }

            return RetVal;
        }
        public void MirrorAllToCache()
        {
            foreach ( RoyalGazette lEntry in this )
            {
                lEntry.MirrorToCache();
            }
        }
        public void SortByPublicationDate()
        {
            Sort(delegate(RoyalGazette x, RoyalGazette y) { return y.Publication.CompareTo(x.Publication); });
        }

        public RoyalGazetteList AllAboutEntity(Int32 iGeocode, Boolean iIncludeSubEntities)
        {
            var retval = new RoyalGazetteList();
            foreach ( RoyalGazette lEntry in this )
            {
                if ( lEntry.IsAboutGeocode(iGeocode, iIncludeSubEntities) )
                {
                    retval.Add(lEntry);
                }
            }
            return retval;
        }
        public void ExportToXML(XmlNode iNode)
        {
            XmlDocument lXmlDocument = TambonHelper.XmlDocumentFromNode(iNode);
            var lNode = (XmlElement)lXmlDocument.CreateNode("element", "gazette", "");
            iNode.AppendChild(lNode);
            foreach ( RoyalGazette lEntry in this )
            {
                lEntry.ExportToXML(lNode);
            }
        }
        public void ExportToRSS(String iFilename)
        {
            SyndicationFeed lFeed = new SyndicationFeed();
            lFeed.Title = new TextSyndicationContent("Royal Gazette");
            List<SyndicationItem> lItems = new List<SyndicationItem>();

            foreach ( RoyalGazette lEntry in this )
            {
                lItems.Add(lEntry.ToSyndicationItem());
            }
            lFeed.Items = lItems;

            XmlWriter lWriter = XmlWriter.Create(iFilename);
            lFeed.SaveAsAtom10(lWriter);
            lWriter.Flush();
        }
        public void SaveXML(string iFilename)
        {
            XmlDocument lXmlDocument = new XmlDocument();
            ExportToXML(lXmlDocument);
            lXmlDocument.Save(iFilename);
        }

        public RoyalGazetteList FilteredList(RoyalGazetteList iFilter)
        {
            var lResult = new RoyalGazetteList();
            foreach ( RoyalGazette lEntry in this )
            {
                if ( iFilter == null )
                {
                    lResult.Add(lEntry);
                }
                else if ( !iFilter.Contains(lEntry) )
                {
                    lResult.Add(lEntry);
                }
            }
            return lResult;
        }

        public RoyalGazetteList FindDuplicates()
        {
            RoyalGazetteList lResult = new RoyalGazetteList();
            RoyalGazetteList lTemp = new RoyalGazetteList();
            lTemp.AddRange(this);
            URIComparer lComparer = new URIComparer();
            lTemp.Sort(lComparer.Compare);
            int lIndex = 0;
            while ( lIndex < lTemp.Count )
            {
                RoyalGazette lEntry = lTemp[lIndex];
                lTemp.RemoveAt(lIndex);
                int lFound = lTemp.BinarySearch(lEntry, lComparer);
                if ( lFound >= 0 )
                {
                    lResult.Add(lEntry);
                }
            }
            return lResult;
        }

        #endregion

    }
}
