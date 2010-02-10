using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace De.AHoerstemeier.Tambon
{
    public class KontessabanDownloader
    {
        const String URL = "http://www.kontessaban.com/tessaban/index.php";
        const String CacheFilename = "kontessaban list.htm";
        const String SearchKeyNumber = "ลำดับที่";
        const String SearchKeyName = "ชื่อ";
        const String SearchKeyAddress = "ที่ตั้ง";
        const String SearchKeyTelephone = "เบอร์โทรศัพท์ ";
        const String SearchKeyFax = "FAX";
        const String SearchKeyWebsite = "เว็บไซต์ ";

        private String DoDownload()
        {
            WebClient lWebClient = new WebClient();
            Stream lStream = lWebClient.OpenRead(URL);
            var lReader = new StreamReader(lStream, TambonHelper.ThaiEncoding);
            String lPlainWebsiteText = lReader.ReadToEnd();
            return lPlainWebsiteText;
        }
        private String DoLoadFromCache()
        {
            String lFilename = Path.Combine(GlobalSettings.HTMLCacheDir, CacheFilename);
            Stream lStream = new FileStream(lFilename, FileMode.Open);
            var lReader = new StreamReader(lStream, TambonHelper.ThaiEncoding);
            String lPlainWebsiteText = lReader.ReadToEnd();
            return lPlainWebsiteText;
        }
        private List<KontessabanDataEntry> DoParse(String iPlainText)
        {
            String lTempText = iPlainText.Replace("<br>", Environment.NewLine).Replace("<BR>", Environment.NewLine);
            var lParseReader = new StringReader(lTempText);
            String lCurrentLine = String.Empty;
            List<KontessabanDataEntry> lData = new List<KontessabanDataEntry>();
            KontessabanDataEntry lCurrent = null;
            while ((lCurrentLine = lParseReader.ReadLine()) != null)
            {
                if (lCurrent==null)
                {
                    lCurrent = new KontessabanDataEntry();
                }
                if (lCurrentLine.Contains(SearchKeyNumber))
                {
                    String lTemp = lCurrentLine.Substring(
                        lCurrentLine.IndexOf(SearchKeyNumber)+SearchKeyNumber.Length);
                    lTemp = lTemp.Replace("</b>", "").Replace("</B>", "");
                    lTemp = lTemp.Replace(",", "").Trim();
                    lTemp = lTemp.Substring(0, lTemp.IndexOf(' ')+1).Trim();
                    lCurrent.KontessabanIndex = Convert.ToInt32(lTemp);

                    String lName = lCurrentLine.Substring(lCurrentLine.IndexOf(SearchKeyName) + SearchKeyName.Length).Trim();
                    lCurrent.ParseName(lName);
                }
                if (lCurrentLine.StartsWith(SearchKeyAddress))
                {
                    String lTemp = lCurrentLine.Substring(SearchKeyAddress.Length).Trim();
                    lCurrent.Address.ParseString(lTemp);
                }
                if (lCurrentLine.StartsWith(SearchKeyWebsite))
                {
                    String lTemp = lCurrentLine.Substring(SearchKeyWebsite.Length).Trim();
                    lCurrent.Website = lTemp;

                    // this is the last data, a new entry will follow
                    lCurrent.FinishEntry();
                    lData.Add(lCurrent);
                    lCurrent = null;
                }
                if (lCurrentLine.StartsWith(SearchKeyTelephone))
                {
                    String lTemp = lCurrentLine.Substring(SearchKeyTelephone.Length).Trim();
                    lCurrent.Telephone = lTemp.Substring(0, lTemp.IndexOf(SearchKeyFax)).Trim();
                    lTemp = lTemp.Substring(lTemp.IndexOf(SearchKeyFax) + SearchKeyFax.Length);
                    lCurrent.FAX = lTemp.Trim();
                }
            }
            return lData;
        }
        public List<KontessabanDataEntry> DoIt()
        {
            String lData = String.Empty;
            String lFilename = Path.Combine(GlobalSettings.HTMLCacheDir, CacheFilename);
            if (File.Exists(lFilename))
            {
                lData = DoLoadFromCache();
            }
            else
            {
                lData = DoDownload();
                File.WriteAllText(lFilename, lData, TambonHelper.ThaiEncoding);
            }
            var lReturnData = DoParse(lData);
            return lReturnData;
        }
    }
}