using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using De.AHoerstemeier.Tambon;

namespace De.AHoerstemeier.Tambon
{
    public partial class GazetteEntry : IGeocode
    {
        #region IGeocode Members

        /// <summary>
        /// Checks if this instance is about the entity identified by the <paramref name="geocode"/>.
        /// If <paramref name="includeSubEntities"/> is <c>true</c>,
        /// </summary>
        /// <param name="geocode">Geocode to check.</param>
        /// <param name="includeSubEntities">Toggles whether codes under <paramref name="geocode"/> are considered fitting as well.</param>
        /// <returns><c>true</c> if instance is about the code, <c>false</c> otherwise.</returns>
        public Boolean IsAboutGeocode(UInt32 geocode, Boolean includeSubEntities)
        {
            var result = false;
            foreach ( var entry in Items )
            {
                var toTest = entry as IGeocode;
                if ( toTest != null )
                {
                    result = result | toTest.IsAboutGeocode(geocode, includeSubEntities);
                }
            }
            return result;
        }

        #endregion IGeocode Members

        /// <summary>
        /// Checks whether the announcement matches the given reference.
        /// </summary>
        /// <param name="gazetteReference">Gazette reference.</param>
        /// <returns><c>true</c> if matching the reference, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="gazetteReference"/> is <c>null</c>.</exception>
        public Boolean IsMatchWith(GazetteRelated gazetteReference)
        {
            if ( gazetteReference == null )
            {
                throw new ArgumentNullException("gazetteReference");
            }

            return
                gazetteReference.volume == this.volume &&
                gazetteReference.issue == this.issue &&
                gazetteReference.page == this.FirstPage &&
                gazetteReference.date == this.publication;
        }

        /// <summary>
        /// Gets the filename of the locally cached PDF file.
        /// </summary>
        public String LocalPdfFileName
        {
            get
            {
                return Path.Combine(GlobalData.PdfDirectory, uri.Replace("/", "\\"));
            }
        }

        /// <summary>
        /// Gets the URL of the announcement on the Royal Gazette webserver.
        /// </summary>
        public Uri DownloadUrl
        {
            get
            {
                return new Uri("http://www.ratchakitcha.soc.go.th/DATA/PDF/" + uri);
            }
        }

        /// <summary>
        /// Copies the PDF from the Royal Gazette webserver into the local cache.
        /// </summary>
        public void MirrorToCache()
        {
            String cacheFile = LocalPdfFileName;
            if ( !File.Exists(cacheFile) )
            {
                System.IO.Stream fileStream = null;
                try
                {
                    try
                    {
                        WebClient webClient = new System.Net.WebClient();
                        Stream webStream = webClient.OpenRead(DownloadUrl);
                        DirectoryInfo dirInfo = new DirectoryInfo(@GlobalData.PdfDirectory);
                        string s = Path.GetDirectoryName(uri);
                        if ( !String.IsNullOrEmpty(s) )
                        {
                            dirInfo.CreateSubdirectory(s);
                        }
                        Stream memoryStream = new MemoryStream();
                        BasicHelper.StreamCopy(webStream, memoryStream);
                        if ( memoryStream.Length > 0 )
                        {
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            fileStream = new FileStream(cacheFile, FileMode.CreateNew);
                            BasicHelper.StreamCopy(memoryStream, fileStream);
                            fileStream.Flush();
                        }
                    }
                    finally
                    {
                        if ( fileStream != null )
                        {
                            fileStream.Dispose();
                        }
                    }
                }
                catch
                {
                    if ( File.Exists(cacheFile) )
                    {
                        File.Delete(cacheFile);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the first page from <see cref="page"/>
        /// </summary>
        public UInt32 FirstPage
        {
            get
            {
                UInt32 startPage;
                UInt32 endPage;
                ParsePageString(page, out startPage, out endPage);
                return startPage;
            }
        }

        private void ParsePageString(String value, out UInt32 startPage, out UInt32 endPage)
        {
            Int32 state = 0;
            startPage = 0;
            endPage = 0;
            foreach ( String SubString in value.Split('-', '–') )
            {
                switch ( state )
                {
                    case 0:
                        startPage = Convert.ToUInt32(SubString);
                        break;
                    case 1:
                        endPage = Convert.ToUInt32(SubString);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Invalid page string " + value);
                }
                state++;
            }
            if ( endPage == 0 )
            {
                endPage = startPage;
            }
        }

        /// <summary>
        /// Gets a flat list of all the gazette operation entries within the gazette entry.
        /// </summary>
        public IEnumerable<GazetteOperationBase> GazetteOperations()
        {
            var result = new List<GazetteOperationBase>();
            foreach ( var item in Items )
            {
                var operationItem = item as GazetteOperationBase;
                if ( operationItem != null )
                {
                    result.AddRange(operationItem.GazetteOperations());
                }
            }
            return result;
        }

        /// <summary>
        /// Creates the reference for the Wikipedia in the given language.
        /// </summary>
        /// <param name="language">Requested language.</param>
        /// <returns>Reference wiki text.</returns>
        /// <exception cref="NotImplementedException"><paramref name="language"/> is not yet supported.</exception>
        public String WikipediaReference(Language language)
        {
            var result = String.Empty;
            switch ( language )
            {
                case Language.English:
                    result = "{{cite journal|journal=Royal Gazette";
                    if ( volume != 0 )
                    {
                        result += String.Format(CultureInfo.InvariantCulture, "|volume={0}", volume);
                    }
                    if ( !String.IsNullOrWhiteSpace(issue) )
                    {
                        result += String.Format("|issue={0}", issue);
                    }
                    if ( !String.IsNullOrEmpty(page) )
                    {
                        result += String.Format("|pages={0}", page);
                    }
                    result += String.Format("|title={0}", title);
                    if ( !String.IsNullOrEmpty(uri) )
                    {
                        result += String.Format("|url={0}", DownloadUrl);
                    }
                    if ( publication.Year > 1800 )
                    {
                        result += String.Format(CultureInfo.InvariantCulture, "|date={0:yyyy-MM-dd}", publication);
                    }
                    result += "|language=Thai}}";
                    break;
                default:
                    throw new NotImplementedException(String.Format("Language {0} not yet implemented", language));
            }

            return result;
        }
    }
}