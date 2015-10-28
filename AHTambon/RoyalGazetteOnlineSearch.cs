using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

// Other interesting search string: เป็นเขตปฏิรูปที่ดิน (area of land reform) - contains maps with Tambon boundaries

namespace De.AHoerstemeier.Tambon
{
    public class RoyalGazetteOnlineSearch
    {
        #region variables

        private const String _searchFormUrl = "http://www.ratchakitcha.soc.go.th/RKJ/announce/search.jsp";
        private const String _searchPostUrl = "http://www.ratchakitcha.soc.go.th/RKJ/announce/search_load_adv.jsp";
        private const String _searchPageUrl = "http://www.ratchakitcha.soc.go.th/RKJ/announce/search_page_load.jsp";
        private const String _baseUrl = "http://www.ratchakitcha.soc.go.th/RKJ/announce/";
        private const String _responseDataUrl = "parent.location.href=\"";

        private String _cookie = String.Empty;
        private String _dataUrl = String.Empty;
        private String _searchKey = String.Empty;
        private Int32 _volume = 0;
        private Int32 _numberOfPages = 0;

        #endregion variables

        public event RoyalGazetteProcessingFinishedHandler ProcessingFinished;

        #region consts

        private static Dictionary<EntityModification, String> EntityModificationText = new Dictionary<EntityModification, String>
        {
            {EntityModification.Abolishment,"Abolish of {0}"},
            {EntityModification.AreaChange,"Change of area of {0}"},
            {EntityModification.Creation,"Creation of {0}"},
            {EntityModification.Rename,"Rename of {0}"},
            {EntityModification.StatusChange,"Change of status of {0}"},
            {EntityModification.Constituency,"Constituencies of {0}"}
        };

        public static Dictionary<EntityModification, Dictionary<EntityType, String>> SearchKeys = new Dictionary<EntityModification, Dictionary<EntityType, String>>
        {
            {
                EntityModification.Creation,new Dictionary<EntityType,String>
                {
                    {EntityType.KingAmphoe, "ตั้งเป็นกิ่งอำเภอ"},
                    {EntityType.Amphoe,"ตั้งเป็นอำเภอ"},
                    {EntityType.Thesaban,"จัดตั้ง เป็นเทศบาล หรือ องค์การบริหารส่วนตำบลเป็นเทศบาล"},
                    {EntityType.Sukhaphiban,"จัดตั้งสุขาภิบาล"},
                    {EntityType.Tambon,"ตั้งและกำหนดเขตตำบล หรือ ตั้งตำบลในจังหวัด หรือ ตั้งและเปลี่ยนแปลงเขตตำบล"},
                    {EntityType.TAO,"ตั้งองค์การบริหารส่วนตำบล หรือ รวมสภาตำบลกับองค์การบริหารส่วนตำบล"},
                    {EntityType.Muban,"ตั้งหมู่บ้าน หรือ ตั้งและกำหนดเขตหมู่บ้าน หรือ ตั้งและกำหนดหมู่บ้าน"},
                    {EntityType.Phak,"การรวมจังหวัดยกขึ้นเป็นภาค"},
                    {EntityType.Khwaeng,"ตั้งแขวง"},
                    {EntityType.Khet,"ตั้งเขต กรุงเทพมหานคร"}
                }
            },
            {
                EntityModification.Abolishment,new Dictionary<EntityType,String>
                {
                    {EntityType.KingAmphoe,"ยุบกิ่งอำเภอ"},
                    {EntityType.Amphoe,"ยุบอำเภอ"},
                    {EntityType.Sukhaphiban,"ยุบสุขาภิบาล"},
                    {EntityType.Tambon,"ยุบตำบล หรือ ยุบและเปลี่ยนแปลงเขตตำบล"},
                    {EntityType.TAO,"ยุบองค์การบริหารส่วนตำบล หรือ ยุบรวมองค์การบริหารส่วนตำบล หรือ รวมองค์การบริหารส่วนตำบลกับ"},
                    {EntityType.SaphaTambon,"ยุบรวมสภาตำบล"}
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
                     {EntityType.TAO,"เปลี่ยนชื่อองค์การบริหารส่วนตำบล"},
                     {EntityType.Muban,"เปลี่ยนแปลงชื่อหมู่บ้าน"}
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
                       {EntityType.Amphoe,"เปลี่ยนแปลงเขตอำเภอ หรือ เปลี่ยนแปลงเขตต์อำเภอ"},
                       {EntityType.KingAmphoe,"เปลี่ยนแปลงเขตกิ่งอำเภอ"},
                       {EntityType.Thesaban,"เปลี่ยนแปลงเขตเทศบาล หรือ การยุบรวมองค์การบริหารส่วนตำบลจันดีกับเทศบาล  หรือ ยุบรวมสภาตำบลกับเทศบาล"},
                       {EntityType.Sukhaphiban,"เปลี่ยนแปลงเขตสุขาภิบาล"},
                       {EntityType.Changwat,"เปลี่ยนแปลงเขตจังหวัด"},
                       {EntityType.Tambon,"เปลี่ยนแปลงเขตตำบล หรือ กำหนดเขตตำบล  หรือ เปลี่ยนแปลงเขตต์ตำบล หรือ ปรับปรุงเขตตำบล"},
                       {EntityType.TAO,"เปลี่ยนแปลงเขตองค์การบริหารส่วนตำบล"},
                       {EntityType.Khwaeng,"เปลี่ยนแปลงพื้นที่แขวง"},
                       {EntityType.Phak,"เปลี่ยนแปลงเขตภาค"},
                       {EntityType.Khet,"เปลี่ยนแปลงพื้นที่เขต กรุงเทพมหานคร"},
                       {EntityType.Muban,"เปลี่ยนแปลงเขตท้องที่ และ หมู่บ้าน"}
                   }
             },
             {
                 EntityModification.Constituency,new Dictionary<EntityType,String>
                 {
                     {EntityType.PAO,"แบ่งเขตเลือกตั้งสมาชิกสภาองค์การบริหารส่วนจังหวัด หรือ เปลี่ยนแปลงแก้ไขเขตเลือกตั้งสมาชิกสภาองค์การบริหารส่วนจังหวัด"},
                     {EntityType.Thesaban,"การแบ่งเขตเลือกตั้งสมาชิกสภาเทศบาล หรือ เปลี่ยนแปลงแก้ไขเขตเลือกตั้งสมาชิกสภาเทศบาล"}
                 }
             }
        };

        public static Dictionary<EntityModification, Dictionary<ProtectedAreaTypes, String>> SearchKeysProtectedAreas = new Dictionary<EntityModification, Dictionary<ProtectedAreaTypes, String>>
        {
            {
                EntityModification.Creation,new Dictionary<ProtectedAreaTypes,String>
                {
                    {ProtectedAreaTypes.NonHuntingArea,"กำหนดเขตห้ามล่าสัตว์ป่า หรือ เป็นเขตห้ามล่าสัตว์ป่า"},
                    {ProtectedAreaTypes.WildlifeSanctuary,"เป็นเขตรักษาพันธุ์สัตว์ป่า"},
                    {ProtectedAreaTypes.NationalPark,"เป็นอุทยานแห่งชาติ หรือ เพิกถอนอุทยานแห่งชาติ"},
                    {ProtectedAreaTypes.HistoricalSite,"กำหนดเขตที่ดินโบราณสถาน"},
                    {ProtectedAreaTypes.NationalPreservedForest,"เป็นป่าสงวนแห่งชาติ หรือ กำหนดพื้นที่ป่าสงวนแห่งชาติ"}
                  }
               },
               {
                EntityModification.Abolishment,new Dictionary<ProtectedAreaTypes,String>
                {
                    {ProtectedAreaTypes.NationalPark,"เพิกถอนอุทยานแห่งชาติ"},  // actually it's normally an area change
                    {ProtectedAreaTypes.WildlifeSanctuary,"เพิกถอนเขตรักษาพันธุ์สัตว์ป่า"}  // actually it's normally an area change
                  }
               },
               {
                EntityModification.AreaChange,new Dictionary<ProtectedAreaTypes,String>
                {
                    {ProtectedAreaTypes.NationalPark,"เปลี่ยนแปลงเขตอุทยานแห่งชาติ"},
                    {ProtectedAreaTypes.WildlifeSanctuary,"เปลี่ยนแปลงเขตรักษาพันธุ์สัตว์ป่า"},
                    {ProtectedAreaTypes.HistoricalSite,"แก้ไขเขตที่ดินโบราณสถาน"}
                }
            }
        };

        #endregion consts

        #region constructor

        public RoyalGazetteOnlineSearch()
        {
        }

        #endregion constructor

        #region methods

        private void PerformRequest()
        {
            StringBuilder requestString = new StringBuilder();
            //foreach ( String s in new List<String> { "ก", "ง", "ข", "ค", "all" } )
            //{
            //    requestString.Append("chkType=" + MyUrlEncode(s) + "&");
            //}
            requestString.Append("chkType=all&");
            if ( _volume <= 0 )
            {
                requestString.Append("txtBookNo=&");
            }
            else
            {
                //lRequestString.Append("txtBookNo=" + MyURLEncode(Helper.UseThaiNumerals(mVolume.ToString())) + "&");
                requestString.Append("txtBookNo=" + _volume.ToString() + "&");
            }
            requestString.Append("txtSection=&");
            requestString.Append("txtFromDate=&");
            requestString.Append("txtToDate=&");
            requestString.Append("chkSpecial=special&");
            // requestString.Append("searchOption=adv&");
            requestString.Append("hidNowItem=txtTitle&");
            requestString.Append("hidFieldSort=&");
            requestString.Append("hidFieldSortText=&");
            // requestString.Append("hidFieldList=" + MyUrlEncode("txtSearchTitle/txtToDate/txtToDate1/txtToDate2") + "&");
            requestString.Append("hidFieldList=" + MyUrlEncode("txtTitle/txtBookNo/txtSection/txtFromDate/txtToDate/selDocGroup1") + "&");
            //request.Append("txtDetail=&");
            requestString.Append("selDocGroup1=&");
            requestString.Append("selFromDay=&");
            requestString.Append("selFromMonth=&");
            requestString.Append("selFromYear=&");
            requestString.Append("selToDay=31&");
            requestString.Append("selToMonth=&");
            requestString.Append("selToYear=&");
            requestString.Append("radSearchType=new&");
            requestString.Append("txtTitle=" + MyUrlEncode(_searchKey));

            _dataUrl = GetDataUrl(0, requestString.ToString());
        }

        private String GetDataUrl(Int32 page, String requestString)
        {
            WebClient client = new WebClient();
            String searchUrl = String.Empty;
            if ( page == 0 )
            {
                searchUrl = _searchPostUrl;
            }
            else
            {
                searchUrl = _searchPageUrl;
                client.Headers.Add("Referer", "http://www.ratchakitcha.soc.go.th/RKJ/announce/search_result.jsp");
            }

            if ( !String.IsNullOrEmpty(_cookie) )
            {
                client.Headers.Add("Cookie", _cookie);
            }
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0");
            client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            // client.Headers.Add("Accept-Encoding", "gzip,deflate");
            client.Headers.Add("Accept-Charset", "UTF-8,*");

            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String response = client.UploadString(searchUrl, requestString);

            // Byte[] lResponseData = client.DownloadData(searchUrl + "?" + requestString);
            String lCookie = client.ResponseHeaders.Get("Set-Cookie");
            if ( !String.IsNullOrEmpty(lCookie) )
            {
                _cookie = lCookie;
            }
            // String response = Encoding.ASCII.GetString(lResponseData);
            Int32 position = response.LastIndexOf(_responseDataUrl);
            String result = String.Empty;
            if ( position >= 0 )
            {
                String dataUrl = response.Substring(position, response.Length - position);
                dataUrl = dataUrl.Substring(_responseDataUrl.Length, dataUrl.Length - _responseDataUrl.Length);
                if ( dataUrl.Contains("\";") )
                {
                    result = _baseUrl + dataUrl.Substring(0, dataUrl.LastIndexOf("\";"));
                }
                else
                {
                    result = _baseUrl + dataUrl.Substring(0, dataUrl.LastIndexOf("\"+")) + TambonHelper.GetDateJavaScript(DateTime.Now).ToString() + "#";
                }
            }
            return result;
        }

        private Stream DoDataDownload(Int32 page)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            if ( page == 0 )
            {
                client.Headers.Add("Referer", _searchFormUrl);
            }
            else
            {
                client.Headers.Add("Referer", _searchPageUrl);
            }
            if ( !String.IsNullOrEmpty(_cookie) )
            {
                client.Headers.Add("Cookie", _cookie);
            }
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0");
            client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            // client.Headers.Add("Accept-Encoding", "gzip,deflate");
            client.Headers.Add("Accept-Charset", "UTF-8,*");
            System.IO.Stream receiveStream = client.OpenRead(_dataUrl);
            MemoryStream memory = new MemoryStream();
            TambonHelper.StreamCopy(receiveStream, memory);
            memory.Seek(0, 0);
            return memory;
        }

        private void PerformRequestPage(Int32 page)
        {
            StringBuilder requestString = new StringBuilder();
            //lRequestString.Append("hidlowerindex=1");
            //lRequestString.Append("hidupperindex=100");
            requestString.Append("txtNowpage=" + page.ToString());
            _dataUrl = GetDataUrl(page, requestString.ToString());
        }

        private string MyUrlEncode(String value)
        {
            var byteArray = TambonHelper.ThaiEncoding.GetBytes(value);
            String result = HttpUtility.UrlEncode(byteArray, 0, byteArray.Length);
            return result;
        }

        private const String EntryStart = "<td width=\"50\" align=\"center\" nowrap class=\"row4\">";

        private const String PageStart = "onkeypress=\"EnterPage()\"> จากทั้งหมด";

        private RoyalGazetteList DoParseStream(Stream data)
        {
            var reader = new System.IO.StreamReader(data, TambonHelper.ThaiEncoding);
            //var fileStream = new FileStream("c:\\temp\\out.htm", FileMode.OpenOrCreate);
            //TambonHelper.StreamCopy(data, fileStream);
            //fileStream.Close();
            RoyalGazetteList result = new RoyalGazetteList();
            result.AddRange(DoParse(reader));
            return result;
        }

        private RoyalGazetteList DoParse(TextReader reader)
        {
            RoyalGazetteList result = new RoyalGazetteList();
            String currentLine = String.Empty;
            Int32 dataState = -1;
            Int32 currentLineIndex = 0;
            StringBuilder entryData = new StringBuilder();
            try
            {
                while ( (currentLine = reader.ReadLine()) != null )
                {
                    currentLineIndex++;
                    currentLine = currentLine.Trim();
                    if ( !String.IsNullOrWhiteSpace(currentLine) )
                    {
                        if ( currentLine.Contains(PageStart) )
                        {
                            String temp = currentLine.Substring(currentLine.LastIndexOf(PageStart) + PageStart.Length, 3).Trim();
                            _numberOfPages = Convert.ToInt32(temp);
                        }
                        else if ( currentLine.StartsWith(EntryStart) )
                        {
                            if ( entryData.Length > 0 )
                            {
                                var current = ParseSingeItem(entryData.ToString());
                                if ( current != null )
                                {
                                    result.Add(current);
                                }
                                else
                                {
                                    dataState++;
                                    // failed to parse!
                                }
                                entryData.Remove(0, entryData.Length);
                            }
                            dataState++;
                        }
                        else if ( dataState >= 0 )
                        {
                            entryData.Append(currentLine.Trim() + " ");
                        }
                    }
                }
                if ( entryData.Length > 0 )
                {
                    var current = ParseSingeItem(entryData.ToString());
                    if ( current != null )
                    {
                        result.Add(current);
                    }
                }
            }
            catch ( IOException )
            {
            }

            return result;
        }

        private const string EntryVolumeorPage = "<td width=\"50\" align=\"center\" nowrap class=\"row2\">";
        private const string EntryIssue = "<td width=\"100\" align=\"center\" nowrap class=\"row3\">";
        private const string EntryDate = "<td width=\"150\" align=\"center\" nowrap class=\"row3\">";
        private const string EntryURL = "<a href=\"/DATA/PDF/";
        private const string EntryURLend = "\" target=\"_blank\"  class=\"topictitle\">";
        private const string ColumnEnd = "</td>";
        private const string EntryTitle = "menubar=no,location=no,scrollbars=auto,resizable');\"-->";
        private const string EntryTitleEnd = "</a></td>";

        private RoyalGazette ParseSingeItem(String value)
        {
            value = value.Replace("\t", "");
            RoyalGazette retval = null;
            Int32 position = value.IndexOf(EntryURL);
            if ( position >= 0 )
            {
                retval = new RoyalGazette();
                position = position + EntryURL.Length;
                Int32 position2 = value.IndexOf(EntryURLend);
                retval.URI = value.Substring(position, position2 - position);
                value = value.Substring(position2, value.Length - position2);
                position = value.IndexOf(EntryTitle) + EntryTitle.Length;
                position2 = value.IndexOf(EntryTitleEnd);
                retval.Title = value.Substring(position, position2 - position).Trim();
                value = value.Substring(position2, value.Length - position2);
                position = value.IndexOf(EntryVolumeorPage) + EntryVolumeorPage.Length;
                position2 = value.IndexOf(ColumnEnd, position);
                String volume = value.Substring(position, position2 - position);
                retval.Volume = Convert.ToInt32(TambonHelper.ReplaceThaiNumerals(volume));
                value = value.Substring(position2, value.Length - position2);
                position = value.IndexOf(EntryIssue) + EntryIssue.Length;
                position2 = value.IndexOf(ColumnEnd, position);
                String Issue = TambonHelper.ReplaceThaiNumerals(value.Substring(position, position2 - position).Trim());
                value = value.Substring(position2, value.Length - position2);
                retval.Issue = new RoyalGazetteIssue(Issue);
                position = value.IndexOf(EntryDate) + EntryDate.Length;
                position2 = value.IndexOf(ColumnEnd, position);
                String Date = value.Substring(position, position2 - position);
                retval.Publication = TambonHelper.ParseThaiDate(Date);
                value = value.Substring(position2, value.Length - position2);
                position = value.IndexOf(EntryVolumeorPage) + EntryVolumeorPage.Length;
                position2 = value.IndexOf(ColumnEnd, position);
                String page = value.Substring(position, position2 - position);
                if ( String.IsNullOrEmpty(page) )
                    retval.PageInfo.Page = 1;
                else
                    retval.PageInfo.Page = Convert.ToInt32(TambonHelper.ReplaceThaiNumerals(page));

                if ( retval.Title.Contains('[') && retval.Title.EndsWith("]") )
                {
                    var beginSubTitle = retval.Title.LastIndexOf('[');
                    retval.SubTitle = retval.Title.Substring(beginSubTitle + 1, retval.Title.Length - beginSubTitle - 2).Trim();
                    retval.Title = retval.Title.Substring(0, beginSubTitle - 1).Trim();
                }
            }
            return retval;
        }

        public RoyalGazetteList DoGetList(String searchKey, Int32 volume)
        {
            _searchKey = searchKey;
            _volume = volume;
            _cookie = String.Empty;
            RoyalGazetteList result = null;
            try
            {
                PerformRequest();
                result = new RoyalGazetteList();
                if ( _dataUrl != String.Empty )
                {
                    Stream receivedData = DoDataDownload(0);
                    result = DoParseStream(receivedData);
                    for ( Int32 page = 2 ; page <= _numberOfPages ; page++ )
                    {
                        PerformRequestPage(page);
                        Stream receivedDataPage = DoDataDownload(page);
                        result.AddRange(DoParseStream(receivedDataPage));
                    }
                }
            }
            catch ( WebException )
            {
                result = null;
                // TODO
            }
            return result;
        }

        protected RoyalGazetteList GetListDescription(String searchKey, Int32 volume, String description)
        {
            RoyalGazetteList result = DoGetList(searchKey, volume);
            if ( result != null )
            {
                foreach ( RoyalGazette entry in result )
                {
                    entry.Description = description;
                }
            }
            return result;
        }

        public RoyalGazetteList SearchNews(DateTime date)
        {
            RoyalGazetteList result = new RoyalGazetteList();
            result.AddRange(SearchNewsRange(date, date));
            result.SortByPublicationDate();
            return result;
        }

        public RoyalGazetteList SearchNewsRange(DateTime beginDate, DateTime endDate)
        {
            RoyalGazetteList result = new RoyalGazetteList();
            var protecteAreaTypes = new List<ProtectedAreaTypes>();
            foreach ( ProtectedAreaTypes protectedArea in Enum.GetValues(typeof(ProtectedAreaTypes)) )
            {
                protecteAreaTypes.Add(protectedArea);
            }
            var protectedAreasList = SearchNewsProtectedAreas(beginDate, endDate, protecteAreaTypes);
            result.AddRange(protectedAreasList);

            var entityTypes = new List<EntityType>();
            foreach ( EntityType entityType in Enum.GetValues(typeof(EntityType)) )
            {
                if ( entityType != EntityType.Sukhaphiban )
                {
                    entityTypes.Add(entityType);
                }
            }
            var entityModifications = new List<EntityModification>();
            foreach ( EntityModification entityModification in Enum.GetValues(typeof(EntityModification)) )
            {
                entityModifications.Add(entityModification);
            }
            var administrativeEntitiesList = SearchNewsRangeAdministrative(beginDate, endDate, entityTypes, entityModifications);
            result.AddRange(administrativeEntitiesList);
            result.SortByPublicationDate();
            return result;
        }

        public RoyalGazetteList SearchNewsProtectedAreas(DateTime beginDate, DateTime endDate, IEnumerable<ProtectedAreaTypes> values)
        {
            RoyalGazetteList result = new RoyalGazetteList();
            Int32 volumeBegin = beginDate.Year - 2007 + 124;
            Int32 volumeEnd = endDate.Year - 2007 + 124;

            for ( Int32 volume = volumeBegin ; volume <= volumeEnd ; volume++ )
            {
                foreach ( KeyValuePair<EntityModification, Dictionary<ProtectedAreaTypes, String>> outerKeyValuePair in SearchKeysProtectedAreas )
                {
                    foreach ( KeyValuePair<ProtectedAreaTypes, String> keyValuePair in outerKeyValuePair.Value )
                    {
                        if ( values.Contains(keyValuePair.Key) )
                        {
                            var list = GetListDescription(keyValuePair.Value, volume, ModificationText(outerKeyValuePair.Key, keyValuePair.Key));
                            if ( list != null )
                            {
                                result.AddRange(list);
                            }
                        }
                    }
                }
            }
            result.SortByPublicationDate();
            return result;
        }

        public RoyalGazetteList SearchNewsRangeAdministrative(DateTime beginDate, DateTime endDate, IEnumerable<EntityType> types, IEnumerable<EntityModification> modifications)
        {
            RoyalGazetteList result = new RoyalGazetteList();
            Int32 volumeBegin = beginDate.Year - 2007 + 124;
            Int32 volumeEnd = endDate.Year - 2007 + 124;

            for ( Int32 volume = volumeBegin ; volume <= volumeEnd ; volume++ )
            {
                foreach ( KeyValuePair<EntityModification, Dictionary<EntityType, String>> outerKeyValuePair in SearchKeys )
                {
                    if ( modifications.Contains(outerKeyValuePair.Key) )
                    {
                        foreach ( KeyValuePair<EntityType, String> keyValuePair in outerKeyValuePair.Value )
                        {
                            if ( types.Contains(keyValuePair.Key) )
                            {
                                var list = GetListDescription(keyValuePair.Value, volume, ModificationText(outerKeyValuePair.Key, keyValuePair.Key));
                                if ( list != null )
                                {
                                    result.AddRange(list);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public RoyalGazetteList SearchString(DateTime beginDate, DateTime endDate, String searchKey)
        {
            RoyalGazetteList result = new RoyalGazetteList();
            Int32 volumeBegin = beginDate.Year - 2007 + 124;
            Int32 volumeEnd = endDate.Year - 2007 + 124;

            for ( Int32 volume = volumeBegin ; volume <= volumeEnd ; volume++ )
            {
                var list = GetListDescription(searchKey, volume, "");
                if ( list != null )
                {
                    result.AddRange(list);
                }
            }
            result.SortByPublicationDate();
            return result;
        }

        private String ModificationText(EntityModification modification, EntityType entityType)
        {
            String result = String.Format(EntityModificationText[modification], entityType);
            return result;
        }

        private String ModificationText(EntityModification modification, ProtectedAreaTypes protectedAreaType)
        {
            String result = String.Format(EntityModificationText[modification], protectedAreaType);
            return result;
        }

        public void SearchNewsNow()
        {
            RoyalGazetteList gazetteList = SearchNews(DateTime.Now);
            if ( DateTime.Now.Month == 1 )
            {
                // Check news from last year as well, in case something was added late
                gazetteList.AddRange(SearchNews(DateTime.Now.AddYears(-1)));
            }
            gazetteList.SortByPublicationDate();
            OnProcessingFinished(new RoyalGazetteEventArgs(gazetteList));
        }

        private void OnProcessingFinished(RoyalGazetteEventArgs e)
        {
            if ( ProcessingFinished != null )
            {
                ProcessingFinished(this, e);
            }
        }

        #endregion methods
    }
}