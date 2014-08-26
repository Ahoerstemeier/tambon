using System;
using System.Collections.Generic;
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
    }
}