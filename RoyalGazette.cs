using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace De.AHoerstemeier.Tambon
{
    internal class RoyalGazette : IComparable, ICloneable, IEquatable<RoyalGazette>,IGeocode
    {
        static String BaseURL = "http://www.ratchakitcha.soc.go.th/DATA/PDF/";
        #region variables
        private List<RoyalGazetteContent> mContent = new List<RoyalGazetteContent>();
        #endregion
        #region properties
        public String Description { get; set; }
        public String URI { get; set; }
        public String Title { get; set; }
        public String SubTitle { get; set; }
        public Int32 Volume { get; set; }
        public RoyalGazetteIssue Issue { get; set; }
        public RoyalGazettePageinfo PageInfo { get; set; }
        public DateTime Publication { get; set; }
        public DateTime Sign { get; set; }
        public DateTime Effective { get; set; }
        public List<RoyalGazetteContent> Content { get { return mContent; } }
        #endregion
        #region constructor
        public RoyalGazette()
        {
            Description = string.Empty;
            URI = string.Empty;
            Volume = 0;
            PageInfo = new RoyalGazettePageinfo();
            Issue = new RoyalGazetteIssue();
            Title = String.Empty;
            SubTitle = String.Empty;
            Publication = new DateTime();
            Sign = new DateTime();
            Effective = new DateTime();
        }
        public RoyalGazette(RoyalGazette value)
        {
            Description = value.Description;
            Volume = value.Volume;
            URI = value.URI;
            PageInfo = new RoyalGazettePageinfo(value.PageInfo);
            Issue = new RoyalGazetteIssue(value.Issue);
            Title = value.Title;
            SubTitle = value.SubTitle;
            Publication =value.Publication;
            Sign = value.Sign;
            Effective = value.Effective;
            foreach (RoyalGazetteContent entry in value.Content)
            {
                RoyalGazetteContent lNewContent = (RoyalGazetteContent)entry.Clone();
                Content.Add(lNewContent);
            }
        }
        #endregion
        #region methods
        protected string CacheFilename()
        {
            String lFilename = Path.Combine(AHGlobalSettings.PDFDir, URI);
            return lFilename;
        }
        protected string URL()
        {
            String retval = BaseURL + URI;
            return retval;
        }
        public Boolean Cached()
        {
            return File.Exists(CacheFilename());
        }
        public void MirrorToCache()
        {
            String lCacheFile = CacheFilename();
            if (!Cached())
            {
                System.IO.Stream lFileStream = null;
                try
                {
                    try
                    {
                        System.Net.WebClient lWebClient = new System.Net.WebClient();
                        System.IO.Stream WebStream = lWebClient.OpenRead(URL());
                        DirectoryInfo lDirInfo = new DirectoryInfo(@AHGlobalSettings.PDFDir);
                        string s = Path.GetDirectoryName(URI);
                        if (!String.IsNullOrEmpty(s))
                        {
                            lDirInfo.CreateSubdirectory(s);
                        }
                        lFileStream = new FileStream(lCacheFile, FileMode.CreateNew);
                        Helper.StreamCopy(WebStream, lFileStream);
                        lFileStream.Flush();
                    }
                    finally
                    {
                        lFileStream.Dispose();
                    }
                }
                catch
                {
                    if (File.Exists(lCacheFile))
                    {
                        File.Delete(lCacheFile);
                    }
                }
            }
        }
        public void RemoveFromCache()
        {
            if (Cached())
            {
                File.Delete(CacheFilename());
            }
        }
        
        public void ShowPDF()
        {
            MirrorToCache();
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            // TODO
            String lFilename = CacheFilename();
            if (File.Exists(lFilename))
            {
                p.StartInfo.FileName = lFilename;
                p.Start();
            }

        }
        internal static RoyalGazette Load(XmlNode iNode)
        {
            RoyalGazette RetVal = null;

            if (iNode != null && iNode.Name.Equals("entry"))
            {
                RetVal = new RoyalGazette();
                RetVal.Description = Helper.GetAttributeOptionalString(iNode, "description");
                RetVal.URI = Helper.GetAttribute(iNode, "uri");
                RetVal.Title = Helper.GetAttribute(iNode, "title");
                RetVal.SubTitle = Helper.GetAttributeOptionalString(iNode, "subtitle");
                RetVal.Volume = Convert.ToInt32(Helper.GetAttribute(iNode, "volume"));
                RetVal.Issue = new RoyalGazetteIssue(Helper.GetAttribute(iNode, "issue"));
                RetVal.PageInfo = new RoyalGazettePageinfo(Helper.GetAttribute(iNode, "page"));
                Int32 lPageEnd = Helper.GetAttributeOptionalInt(iNode, "pageend");
                if (lPageEnd != 0)
                {
                    RetVal.PageInfo.PageEnd = lPageEnd;
                }
                RetVal.Sign = Helper.GetAttributeOptionalDateTime(iNode, "sign");
                RetVal.Effective = Helper.GetAttributeOptionalDateTime(iNode, "effective");
                RetVal.Publication = Helper.GetAttributeDateTime(iNode, "publication");
                Int32 lEffectiveRelative = Helper.GetAttributeOptionalInt(iNode, "effectiveafter");
                if (lEffectiveRelative != 0)
                {
                    RetVal.Effective = RetVal.Publication.AddDays(lEffectiveRelative);
                }
                RetVal.LoadContents(iNode);
            }

            return RetVal;
        }
        public string Citation()
        {
            string retval = "{{cite journal|journal=Royal Gazette";
            if (Volume != 0)
            {
                retval = retval + "|volume=" + Volume.ToString();
            }
            if (!Issue.IsEmpty())
            {
                retval = retval + "|issue=" + Issue.ToString();
            }
            if (!PageInfo.IsEmpty())
            {
                retval = retval + "|pages=" + PageInfo.ToString();
            }
            retval = retval + "|title=" + Title;
            if (!String.IsNullOrEmpty(URI))
            {
                retval = retval + "|url=" + URL();
            }
            if (Publication != null)
            {
                retval = retval + "|date=" + Publication.ToString("MMMM d yyyy", Helper.CultureInfoUS);
            }
            retval = retval + "|language=Thai}}";
            return retval;
        }
        internal void ExportToXML(XmlNode iNode)
        {
            XmlDocument lXmlDocument = Helper.XmlDocumentFromNode(iNode);
            var lNewElement = (XmlElement)lXmlDocument.CreateNode("element", "entry", "");
            if (!String.IsNullOrEmpty(Description))
            {
                lNewElement.SetAttribute("description", Description);
            }
            lNewElement.SetAttribute("title", Title);
            if (!String.IsNullOrEmpty(SubTitle))
            {
                lNewElement.SetAttribute("subtitle", SubTitle);
            }
            if (Volume != 0)
            {
                lNewElement.SetAttribute("volume", Volume.ToString());
            }
            if (!PageInfo.IsEmpty())
            {
                lNewElement.SetAttribute("page", PageInfo.Page.ToString());
                if (PageInfo.PageEnd > PageInfo.Page)
                {
                    lNewElement.SetAttribute("pageend", PageInfo.PageEnd.ToString());
                }
            }
            if (!Issue.IsEmpty())
            {
                lNewElement.SetAttribute("issue", Issue.ToString());
            }
            if (!String.IsNullOrEmpty(URI))
            {
                lNewElement.SetAttribute("uri", URI);
            }
            
            if (Publication.Year > 1)
            {
                lNewElement.SetAttribute("publication", Publication.ToString("yyyy-MM-dd"));
            }
            if (Sign.Year > 1)
            {
                lNewElement.SetAttribute("sign", Sign.ToString("yyyy-MM-dd"));
            }
            if (Effective.Year > 1)
            {
                lNewElement.SetAttribute("effective", Effective.ToString("yyyy-MM-dd"));
            }
            iNode.AppendChild(lNewElement);
            foreach (RoyalGazetteContent lContent in Content)
            {
                lContent.ExportToXML(lNewElement);
            }
        }

        protected void LoadContents(XmlNode iNode)
        {
            foreach (XmlNode lNode in iNode.ChildNodes)
            {
                RoyalGazetteContent lContent = RoyalGazetteContent.CreateContentObject(lNode.Name);
                if (lContent != null)
                {
                    lContent.DoLoad(lNode);
                    Content.Add(lContent);
                }
            }
        }
        #endregion
        public override string ToString()
        {
            return Title;
        }

        #region IComparable Members

        public int CompareTo(object iOther)
        {
            if (iOther is RoyalGazette)
            {
                Int32 retval = 0;
                RoyalGazette value2 = (RoyalGazette)iOther;

                retval = Volume.CompareTo(value2.Volume);
                if (retval == 0)
                {
                    retval = Issue.CompareTo(value2.Issue);
                    if (retval == 0)
                    {
                        retval = PageInfo.CompareTo(value2.PageInfo);
                    }
                }
                return retval;
            }
            throw new InvalidCastException("Not a RoyalGazette");

        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new RoyalGazette(this);
        }

        #endregion

        #region IEquatable<RoyalGazette> Members

        public bool Equals(RoyalGazette iOther)
        {
            return (CompareTo(iOther) == 0);
        }

        #endregion

        #region IGeocode Members

        public bool IsAboutGeocode(int iGeocode, bool iIncludeSubEntities)
        {
            Boolean retval = false;
            foreach (RoyalGazetteContent element in mContent)
            {
                retval = (retval | element.IsAboutGeocode(iGeocode,iIncludeSubEntities));
            }

            return retval;
        }

        #endregion

    }
}
