using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using System.IO;
using System.Windows.Forms;

namespace De.AHoerstemeier.Tambon
{
    class RoyalGazetteOnlineSearch
    {
        #region variables
        private const String SearchFormURL = "http://www.ratchakitcha.soc.go.th/RKJ/announce/search.jsp";
        private const String SearchPostURL = "http://www.ratchakitcha.soc.go.th/RKJ/announce/search_load_adv.jsp";
        private const String SearchPageURL = "http://www.ratchakitcha.soc.go.th/RKJ/announce/search_page_load.jsp";
        private const String BaseURL = "http://www.ratchakitcha.soc.go.th/RKJ/announce/";
        private const String ResponseDataURL = "parent.location.href=\"";

        private String mCookie = String.Empty;
        private String mDataUrl = String.Empty;
        private String mSearchKey = String.Empty;
        private Int32 mVolume = 0;
        private Int32 mNumberOfPages = 0;
        #endregion
        public event RoyalGazetteList.ProcessingFinished OnProcessingFinished;
        #region consts
        private static Dictionary<EntityModification, String> EntityModificationText = new Dictionary<EntityModification, string>
        {
            {EntityModification.Abolishment,"Abolish of %1%"},
            {EntityModification.AreaChange,"Change of area of %1%"},
            {EntityModification.Creation,"Creation of %1%"},
            {EntityModification.Rename,"Rename of %1%"},
            {EntityModification.StatusChange,"Change of status of %1%"}
        };
        public static Dictionary<EntityModification, Dictionary<EntityType, String>> SearchKeys = new Dictionary<EntityModification, Dictionary<EntityType, string>>
        {
            {
                EntityModification.Creation,new Dictionary<EntityType,String>
                {
                    {EntityType.KingAmphoe, "ตั้งเป็นกิ่งอำเภอ"},
                    {EntityType.Amphoe,"ตั้งเป็นอำเภอ"},
                    {EntityType.Thesaban,"จัดตั้ง เป็นเทศบาล หรือ องค์การบริหารส่วนตำบลเป็นเทศบาล"},
                    {EntityType.Sukhaphiban,"จัดตั้งสุขาภิบาล"},
                    {EntityType.Tambon,"ตั้งและกำหนดเขตตำบลในท้องที่อำเภอ หรือ ตั้งตำบลในจังหวัด หรือ ตั้งและเปลี่ยนแปลงเขตตำบล"},
                    {EntityType.TAO,"ตั้งองค์การบริหารส่วนตำบล หรือ รวมสภาตำบลกับองค์การบริหารส่วนตำบล"}
                }
            },
            {
                EntityModification.Abolishment,new Dictionary<EntityType,String>
                {
                    {EntityType.KingAmphoe,"ยุบกิ่งอำเภอ"},
                    {EntityType.Amphoe,"ยุบอำเภอ"},
                    {EntityType.Sukhaphiban,"ยุบสุขาภิบาล"},
                    {EntityType.Tambon,"ยุบตำบล หรือ ยุบและเปลี่ยนแปลงเขตตำบล"},
                    {EntityType.TAO,"ยุบองค์การบริหารส่วนตำบล หรือ ยุบรวมองค์การบริหารส่วนตำบล"},
                    {EntityType.TC,"ยุบรวมสภาตำบล"}
                }
             },
             {
                 EntityModification.Rename,new Dictionary<EntityType,String>
                 {
                     {EntityType.Amphoe,"เปลี่ยนชื่ออำเภอ หรือ เปลี่ยนนามอำเภอ หรือ เปลี่ยนแปลงชื่ออำเภอ"},
                     {EntityType.KingAmphoe,"เปลี่ยนชื่อกิ่งอำเภอ หรือ เปลี่ยนนามกิ่งอำเภอ หรือ เปลี่ยนแปลงชื่อกิ่งอำเภอ"},
                     {EntityType.Sukhaphiban,"เปลี่ยนชื่อสุขาภิบาล หรือ เปลี่ยนนามสุขาภิบาล หรือ เปลี่ยนแปลงชื่อสุขาภิบาล"},
                     {EntityType.Thesaban,"เปลี่ยนชื่อเทศบาล หรือ เปลี่ยนนามเทศบาล หรือ เปลี่ยนแปลงชื่อเทศบาล"},
                     {EntityType.Tambon,"เปลี่ยนชื่อตำบล หรือ เปลี่ยนนามตำบล หรือ เปลี่ยนแปลงชื่อตำบล หรือ เปลี่ยนแปลงแก้ไขชื่อตำบล"},
                     {EntityType.TAO,"เปลี่ยนชื่อองค์การบริหารส่วนตำบล"}
                 }
              },
              {
                  EntityModification.StatusChange,new Dictionary<EntityType,String>
                  {
                      {EntityType.Thesaban,"เปลี่ยนแปลงฐานะเทศบาล หรือ  พระราชกฤษฎีกาจัดตั้งเทศบาล"},
                      {EntityType.KingAmphoe,"พระราชกฤษฎีกาตั้งอำเภอ หรือ พระราชกฤษฎีกาจัดตั้งอำเภอ"}
                  }
               },
               {
                   EntityModification.AreaChange,new Dictionary<EntityType,String>
                   {
                       {EntityType.Amphoe,"เปลี่ยนแปลงเขตอำเภอ"},
                       {EntityType.KingAmphoe,"เปลี่ยนแปลงเขตกิ่งอำเภอ"},
                       {EntityType.Thesaban,"เปลี่ยนแปลงเขตเทศบาล หรือ การยุบรวมองค์การบริหารส่วนตำบลจันดีกับเทศบาล  หรือ ยุบรวมสภาตำบลกับเทศบาล"},
                       {EntityType.Sukhaphiban,"เปลี่ยนแปลงเขตสุขาภิบาล"},
                       {EntityType.Changwat,"เปลี่ยนแปลงเขตจังหวัด"}
                   }
                   }
        };

        private static Dictionary<ProtectedAreaTypes, String> SearchKeysProtectedAreas = new Dictionary<ProtectedAreaTypes, string>
        {
            {ProtectedAreaTypes.NonHuntingArea,"กำหนดเขตห้ามล่าสัตว์ป่า"},
            {ProtectedAreaTypes.WildlifeSanctuary,"เป็นเขตรักษาพันธุ์สัตว์ป่า"},
            {ProtectedAreaTypes.NationalPark,"เป็นอุทยานแห่งชาติ"},
            {ProtectedAreaTypes.HistoricalSite,"กำหนดเขตที่ดินโบราณสถาน"}
        };
        #endregion
        #region constructor
        public RoyalGazetteOnlineSearch()
        {
        }
        #endregion
        #region methods
        private void PerformRequest()
        {
            StringBuilder lRequestString = new StringBuilder();
            foreach (string s in new List<String> { "ก", "ง", "ข", "ค", "all" })
            {
                lRequestString.Append("chkType=" + MyURLEncode(s) + "&");
            }
            if (mVolume <= 0)
            {
                lRequestString.Append("txtBookNo=&");
            }
            else
            {
                //lRequestString.Append("txtBookNo=" + MyURLEncode(Helper.UseThaiNumerals(mVolume.ToString())) + "&");
                lRequestString.Append("txtBookNo=" + mVolume.ToString() + "&");
            }
            //request.Append("txtSection=&");
            //request.Append("txtFromDate=&");
            //request.Append("txtToDate=&");
            lRequestString.Append("chkSpecial=special&");
            lRequestString.Append("searchOption=adv&");
            lRequestString.Append("hidNowItem=txtTitle&");
            //request.Append("hidFieldSort=&");
            //request.Append("hidFieldSortText=&");
            lRequestString.Append("hidFieldList=" + MyURLEncode("txtTitle/txtBookNo/txtSection/txtFromDate/txtToDate/selDocGroup1") + "&");
            //request.Append("txtDetail=&");
            //request.Append("selDocGroup=&");
            //request.Append("selFromMonth=&");
            //request.Append("selFromYear=&");
            //request.Append("selToMonth=&");
            //request.Append("selToYear=&");
            lRequestString.Append("txtTitle=" + MyURLEncode(mSearchKey));

            mDataUrl = GetDataURL(0, lRequestString.ToString());
        }

        private String GetDataURL(Int32 iPage, String lRequestString)
        {
            WebClient lClient = new WebClient();
            String lSearchURL = String.Empty;
            if (iPage == 0)
            {
                lSearchURL = SearchPostURL;
            }
            else
            {
                lSearchURL = SearchPageURL;
                lClient.Headers.Add("Referer", "http://www.ratchakitcha.soc.go.th/RKJ/announce/search_result.jsp");
            }

            if (!String.IsNullOrEmpty(mCookie))
            {
                lClient.Headers.Add("Cookie", mCookie);
            }
            lClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.11) Gecko/20071127 Firefox/2.0.0.11");
            lClient.Headers.Add("Accept", "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5");
            lClient.Headers.Add("Accept-Language", "en-us,en;q=0.8,de;q=0.5,th;q=0.3");
            lClient.Headers.Add("Accept-Encoding", "gzip,deflate");
            lClient.Headers.Add("Accept-Charset", "UTF-8,*");
            Byte[] lResponseData = lClient.DownloadData(lSearchURL + "?" + lRequestString);
            String lCookie = lClient.ResponseHeaders.Get("Set-Cookie");
            if (!String.IsNullOrEmpty(lCookie))
            { 
                mCookie = lCookie; 
            }
            String response = Encoding.ASCII.GetString(lResponseData);
            Int32 lPosition = response.LastIndexOf(ResponseDataURL);
            String lDataURL = response.Substring(lPosition, response.Length - lPosition);
            lDataURL = lDataURL.Substring(ResponseDataURL.Length, lDataURL.Length - ResponseDataURL.Length);
            String retval = String.Empty;
            if (lDataURL.Contains("\";"))
            {
              retval = BaseURL + lDataURL.Substring(0,lDataURL.LastIndexOf("\";"));
            }
            else
            {
                retval = BaseURL + lDataURL.Substring(0, lDataURL.LastIndexOf("\"+")) + Helper.GetDateJavaScript(DateTime.Now).ToString()+"#";
            }
            return retval;
        }
        private System.IO.Stream DoDataDownload(Int32 iPage)
        {
            WebClient lClient = new WebClient();
            lClient.Encoding = Encoding.UTF8;
            if (iPage == 0)
            {
                lClient.Headers.Add("Referer", SearchFormURL);
            }
            else
            {
                lClient.Headers.Add("Referer", SearchPageURL);
            }
            if (!String.IsNullOrEmpty(mCookie))
            {
                lClient.Headers.Add("Cookie", mCookie);
            }
            lClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.11) Gecko/20071127 Firefox/2.0.0.11");
            lClient.Headers.Add("Accept", "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5");
            lClient.Headers.Add("Accept-Language", "en-us,en;q=0.8,de;q=0.5,th;q=0.3");
            lClient.Headers.Add("Accept-Encoding", "gzip,deflate");
            lClient.Headers.Add("Accept-Charset", "UTF-8,*");
            System.IO.Stream lStream = lClient.OpenRead(mDataUrl);
            return lStream;
        }
        private void PerformRequestPage(Int32 iPage)
        {
            
            StringBuilder lRequestString = new StringBuilder();
            //lRequestString.Append("hidlowerindex=1");
            //lRequestString.Append("hidupperindex=100");
            lRequestString.Append("txtNowpage="+iPage.ToString());
            mDataUrl = GetDataURL(iPage, lRequestString.ToString());
        }
        private string MyURLEncode(string value)
        {
            var lByteArray = Helper.ThaiEncoding.GetBytes(value);
            string s = HttpUtility.UrlEncode(lByteArray, 0, lByteArray.Length);
            return s;
        }

        private const string EntryStart = "        <td width=\"50\" align=\"center\" nowrap class=\"row4\">";
        private const string PageStart = "onkeypress=\"EnterPage()\"> จากทั้งหมด";
        private RoyalGazetteList DoParseStream(Stream iData)
        {
            var lReader = new System.IO.StreamReader(iData, Helper.ThaiEncoding);
            RoyalGazetteList retval = new RoyalGazetteList();
            retval.AddRange(DoParse(lReader));
            return retval;
            }
        private RoyalGazetteList DoParse(TextReader iReader)
        {
            RoyalGazetteList retval = new RoyalGazetteList();
            String lCurrentLine = String.Empty;
            int lDataState = -1;
            StringBuilder lEntryData = new StringBuilder();
            while ((lCurrentLine = iReader.ReadLine()) != null)
            {
                if (lCurrentLine.Contains(PageStart))
                {
                    String lTemp = lCurrentLine.Substring(lCurrentLine.LastIndexOf(PageStart)+PageStart.Length,3).Trim();
                    mNumberOfPages = Convert.ToInt32(lTemp);
                }
                else if (lCurrentLine.StartsWith(EntryStart))
                {
                    if (lEntryData.Length > 0)
                    {
                        var current = ParseSingeItem(lEntryData.ToString());
                        if (current != null)
                        {
                            retval.Add(current);
                        }
                        lEntryData.Remove(0, lEntryData.Length);
                    }
                    lDataState++;
                }
                else if (lDataState >= 0)
                {
                    lEntryData.Append(lCurrentLine.Trim() + " ");
                }
            }
            if (lEntryData.Length > 0)
            {
                retval.Add(ParseSingeItem(lEntryData.ToString()));
            }
            return retval;
        }

        private const string EntryVolumeorPage = "<td width=\"50\" align=\"center\" nowrap class=\"row2\">";
        private const string EntryIssue = "<td width=\"100\" align=\"center\" nowrap class=\"row3\">";
        private const string EntryDate = "<td width=\"150\" align=\"center\" nowrap class=\"row3\">";
        private const string EntryURL = "<a href=\"/DATA/PDF/";
        private const string EntryURLend = "\" target=\"_blank\"  class=\"topictitle\">";
        private const string ColumnEnd = "</td>";
        private const string EntryTitle = "menubar=no,location=no,scrollbars=auto,resizable');\"-->";
        private const string EntryTitleEnd = "</a></td>";
        private RoyalGazette ParseSingeItem(string iValue)
        {
            iValue = iValue.Replace("\t", "");
            RoyalGazette retval = null;
            Int32 position = iValue.IndexOf(EntryURL);
            if (position >= 0)
            {
                retval = new RoyalGazette();
                position = position + EntryURL.Length;
                Int32 position2 = iValue.IndexOf(EntryURLend);
                retval.URI = iValue.Substring(position, position2 - position);
                iValue = iValue.Substring(position2, iValue.Length - position2);
                position = iValue.IndexOf(EntryTitle) + EntryTitle.Length;
                position2 = iValue.IndexOf(EntryTitleEnd);
                retval.Title = iValue.Substring(position, position2 - position).Trim();
                iValue = iValue.Substring(position2, iValue.Length - position2);
                position = iValue.IndexOf(EntryVolumeorPage) + EntryVolumeorPage.Length;
                position2 = iValue.IndexOf(ColumnEnd, position);
                string volume = iValue.Substring(position, position2 - position);
                retval.Volume = Convert.ToInt32(Helper.ReplaceThaiNumerals(volume));
                iValue = iValue.Substring(position2, iValue.Length - position2);
                position = iValue.IndexOf(EntryIssue) + EntryIssue.Length;
                position2 = iValue.IndexOf(ColumnEnd, position);
                string Issue = Helper.ReplaceThaiNumerals(iValue.Substring(position, position2 - position).Trim());
                iValue = iValue.Substring(position2, iValue.Length - position2);
                retval.Issue = new RoyalGazetteIssue(Issue);
                position = iValue.IndexOf(EntryDate) + EntryDate.Length;
                position2 = iValue.IndexOf(ColumnEnd, position);
                string Date = iValue.Substring(position, position2 - position);
                retval.Publication = Helper.ParseThaiDate(Date);
                iValue = iValue.Substring(position2, iValue.Length - position2);
                position = iValue.IndexOf(EntryVolumeorPage) + EntryVolumeorPage.Length;
                position2 = iValue.IndexOf(ColumnEnd, position);
                string page = iValue.Substring(position, position2 - position);
                retval.PageInfo.Page = Convert.ToInt32(Helper.ReplaceThaiNumerals(page));
            }
            return retval;
        }

        public RoyalGazetteList DoGetList(String iSearchKey, Int32 iVolume)
        {
            mSearchKey = iSearchKey;
            mVolume = iVolume;
            mCookie = String.Empty;
            PerformRequest();
            System.IO.Stream lData = DoDataDownload(0);
            RoyalGazetteList retval = DoParseStream(lData);
            for (Int32 lPage = 2; lPage<=mNumberOfPages; lPage++)
            {
                PerformRequestPage(lPage);
                System.IO.Stream lDataPage = DoDataDownload(lPage);
                retval.AddRange(DoParseStream(lDataPage));
            }
            return retval;
        }
        protected RoyalGazetteList GetListDescription(string iSearchKey, int iVolume, string iDescription)
        {
            RoyalGazetteList retval = DoGetList(iSearchKey, iVolume);
            foreach (RoyalGazette lEntry in retval)
            {
                lEntry.Description = iDescription;
            }
            return retval;
        }
        public RoyalGazetteList SearchNews(DateTime iDate)
        {
            RoyalGazetteList retval = new RoyalGazetteList();
            retval.AddRange(SearchNewsRange(iDate, iDate));
            return retval;
        }
        public RoyalGazetteList SearchNewsRange(DateTime iBeginDate, DateTime iEndDate)
        {
            RoyalGazetteList retval = new RoyalGazetteList();
            var lProtecteAreaTypes = new List<ProtectedAreaTypes>();
            foreach (ProtectedAreaTypes lProtectedArea in Enum.GetValues(typeof(ProtectedAreaTypes)))
            {
                lProtecteAreaTypes.Add(lProtectedArea);
            }
            var lProtectedAreasList = SearchNewsProtectedAreas(iBeginDate, iEndDate, lProtecteAreaTypes);
            retval.AddRange(lProtectedAreasList);

            var lEntityTypes = new List<EntityType>();
            foreach (EntityType lEntityType in Enum.GetValues(typeof(EntityType)))
            {
                if (lEntityType != EntityType.Sukhaphiban)
                {
                    lEntityTypes.Add(lEntityType);
                }
            }
            var lEntityModifications = new List<EntityModification>();
            foreach (EntityModification lEntityModification in Enum.GetValues(typeof(EntityModification)))
            {
                lEntityModifications.Add(lEntityModification);
            }
            var lAdministrativeEntitiesList = SearchNewsRangeAdministrative(iBeginDate, iEndDate, lEntityTypes, lEntityModifications);
            retval.AddRange(lAdministrativeEntitiesList);
            return retval;
        }
        public RoyalGazetteList SearchNewsProtectedAreas(DateTime iBeginDate, DateTime iEndDate, List<ProtectedAreaTypes> iValues)
        {
            RoyalGazetteList retval = new RoyalGazetteList();
            Int32 iVolumeBegin = iBeginDate.Year - 2007 + 124;
            Int32 iVolumeEnd = iEndDate.Year - 2007 + 124;

            for (Int32 lVolume = iVolumeBegin; lVolume <= iVolumeEnd; lVolume++)
            {
                foreach (KeyValuePair<ProtectedAreaTypes, String> lKeyValuePair in SearchKeysProtectedAreas)
                {
                    if (iValues.Contains(lKeyValuePair.Key))
                    {
                        var lList = GetListDescription(lKeyValuePair.Value, lVolume, "Creation of " + lKeyValuePair.Key.ToString());
                        retval.AddRange(lList);
                    }
                }
            }
            return retval;
        }
        public RoyalGazetteList SearchNewsRangeAdministrative(DateTime iBeginDate, DateTime iEndDate, List<EntityType> iTypes, List<EntityModification> iModifications)
        {
            RoyalGazetteList retval = new RoyalGazetteList();
            Int32 iVolumeBegin = iBeginDate.Year - 2007 + 124;
            Int32 iVolumeEnd = iEndDate.Year - 2007 + 124;

            for (Int32 lVolume = iVolumeBegin; lVolume <= iVolumeEnd; lVolume++)
            {
                foreach (KeyValuePair<EntityModification, Dictionary<EntityType, String>> lOuterKeyValuePair in SearchKeys)
                {
                    if (iModifications.Contains(lOuterKeyValuePair.Key))
                    {
                        foreach (KeyValuePair<EntityType, String> lKeyValuePair in lOuterKeyValuePair.Value)
                        {
                            if (iTypes.Contains(lKeyValuePair.Key))
                            {
                                var lList = GetListDescription(lKeyValuePair.Value, lVolume, ModificationText(lOuterKeyValuePair.Key, lKeyValuePair.Key));
                                retval.AddRange(lList);
                            }
                        }
                    }
                }
            }
            return retval;
        }
        public RoyalGazetteList SearchString(DateTime iBeginDate, DateTime iEndDate, String iSearchKey)
        {
            RoyalGazetteList retval = new RoyalGazetteList();
            Int32 iVolumeBegin = iBeginDate.Year - 2007 + 124;
            Int32 iVolumeEnd = iEndDate.Year - 2007 + 124;

            for (Int32 lVolume = iVolumeBegin; lVolume <= iVolumeEnd; lVolume++)
            {
                var lList = GetListDescription(iSearchKey, lVolume, "");
                retval.AddRange(lList);
            }
            return retval;
        }

        private String ModificationText(EntityModification iModification, EntityType iEntityType)
        {
            String retval = EntityModificationText[iModification];
            retval = retval.Replace("%1%",iEntityType.ToString());
            return retval;
        }
        public void SearchNewsNow()
        {
            RoyalGazetteList lGazetteList = SearchNews(DateTime.Now);
            if (DateTime.Now.Month < 3)
            {
                // Check news from last year as well, in case something was added late
                lGazetteList.AddRange(SearchNews(DateTime.Now.AddYears(-1)));
            }
            if (OnProcessingFinished != null)
            {
                OnProcessingFinished(lGazetteList);
            }
        }

        #endregion
    }
}