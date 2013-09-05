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
    public class RoyalGazetteEventArgs : EventArgs
    {
        public GazetteList Data
        {
            get;
            private set;
        }

        public RoyalGazetteEventArgs(GazetteList data)
            : base()
        {
            Data = data;
        }
    }

    public enum EntityModification
    {
        Creation,
        Abolishment,
        Rename,
        StatusChange,
        AreaChange,
        Constituency
    }

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

        public EventHandler<RoyalGazetteEventArgs> ProcessingFinished;

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

        public static Dictionary<EntityModification, Dictionary<ParkType, String>> SearchKeysProtectedAreas = new Dictionary<EntityModification, Dictionary<ParkType, String>>
        {
            {
                EntityModification.Creation,new Dictionary<ParkType,String>
                {
                    {ParkType.NonHuntingArea,"กำหนดเขตห้ามล่าสัตว์ป่า หรือ เป็นเขตห้ามล่าสัตว์ป่า"},
                    {ParkType.WildlifeSanctuary,"เป็นเขตรักษาพันธุ์สัตว์ป่า"},
                    {ParkType.NationalPark,"เป็นอุทยานแห่งชาติ หรือ เพิกถอนอุทยานแห่งชาติ"},
                    {ParkType.HistoricalSite,"กำหนดเขตที่ดินโบราณสถาน"},
                    {ParkType.NationalForest,"เป็นป่าสงวนแห่งชาติ หรือ กำหนดพื้นที่ป่าสงวนแห่งชาติ"}
                  }
               },
               {
                EntityModification.Abolishment,new Dictionary<ParkType,String>
                {
                    {ParkType.NationalPark,"เพิกถอนอุทยานแห่งชาติ"},  // actually it's normally an area change
                    {ParkType.WildlifeSanctuary,"เพิกถอนเขตรักษาพันธุ์สัตว์ป่า"}  // actually it's normally an area change
                  }
               },
               {
                EntityModification.AreaChange,new Dictionary<ParkType,String>
                {
                    {ParkType.NationalPark,"เปลี่ยนแปลงเขตอุทยานแห่งชาติ"},
                    {ParkType.WildlifeSanctuary,"เปลี่ยนแปลงเขตรักษาพันธุ์สัตว์ป่า"},
                    {ParkType.HistoricalSite,"แก้ไขเขตที่ดินโบราณสถาน"}
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
            foreach ( string s in new List<String> { "ก", "ง", "ข", "ค", "all" } )
            {
                requestString.Append("chkType=" + MyUrlEncode(s) + "&");
            }
            if ( _volume <= 0 )
            {
                requestString.Append("txtBookNo=&");
            }
            else
            {
                //lRequestString.Append("txtBookNo=" + MyURLEncode(Helper.UseThaiNumerals(mVolume.ToString())) + "&");
                requestString.Append("txtBookNo=" + _volume.ToString() + "&");
            }
            //request.Append("txtSection=&");
            //request.Append("txtFromDate=&");
            //request.Append("txtToDate=&");
            requestString.Append("chkSpecial=special&");
            requestString.Append("searchOption=adv&");
            requestString.Append("hidNowItem=txtTitle&");
            //request.Append("hidFieldSort=&");
            //request.Append("hidFieldSortText=&");
            requestString.Append("hidFieldList=" + MyUrlEncode("txtTitle/txtBookNo/txtSection/txtFromDate/txtToDate/selDocGroup1") + "&");
            //request.Append("txtDetail=&");
            //request.Append("selDocGroup=&");
            //request.Append("selFromMonth=&");
            //request.Append("selFromYear=&");
            //request.Append("selToMonth=&");
            //request.Append("selToYear=&");
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
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.11) Gecko/20071127 Firefox/2.0.0.11");
            client.Headers.Add("Accept", "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5");
            client.Headers.Add("Accept-Language", "en-us,en;q=0.8,de;q=0.5,th;q=0.3");
            client.Headers.Add("Accept-Encoding", "gzip,deflate");
            client.Headers.Add("Accept-Charset", "UTF-8,*");
            Byte[] lResponseData = client.DownloadData(searchUrl + "?" + requestString);
            String lCookie = client.ResponseHeaders.Get("Set-Cookie");
            if ( !String.IsNullOrEmpty(lCookie) )
            {
                _cookie = lCookie;
            }
            String response = Encoding.ASCII.GetString(lResponseData);
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
                    result = _baseUrl + dataUrl.Substring(0, dataUrl.LastIndexOf("\"+")) + GetDateJavaScript(DateTime.Now).ToString() + "#";
                }
            }
            return result;
        }

        /// <summary>
        /// Converts a datetime into the javascript datetime value, which is the milliseconds since January 1 1970.
        /// </summary>
        /// <param name="value">Date and time to be converted.</param>
        /// <returns>Javascript datetime value.</returns>
        private static Int64 GetDateJavaScript(DateTime value)
        {
            TimeSpan lDifference = value.ToUniversalTime() - new DateTime(1970, 1, 1);
            Int64 retval = Convert.ToInt64(lDifference.TotalMilliseconds);
            return retval;
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
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.11) Gecko/20071127 Firefox/2.0.0.11");
            client.Headers.Add("Accept", "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5");
            client.Headers.Add("Accept-Language", "en-us,en;q=0.8,de;q=0.5,th;q=0.3");
            client.Headers.Add("Accept-Encoding", "gzip,deflate");
            client.Headers.Add("Accept-Charset", "UTF-8,*");
            System.IO.Stream lStream = client.OpenRead(_dataUrl);
            return lStream;
        }

        private void PerformRequestPage(Int32 page)
        {
            StringBuilder requestString = new StringBuilder();
            //lRequestString.Append("hidlowerindex=1");
            //lRequestString.Append("hidupperindex=100");
            requestString.Append("txtNowpage=" + page.ToString());
            _dataUrl = GetDataUrl(page, requestString.ToString());
        }

        static private Encoding ThaiEncoding = Encoding.GetEncoding(874);

        private string MyUrlEncode(String value)
        {
            var lByteArray = ThaiEncoding.GetBytes(value);
            String result = HttpUtility.UrlEncode(lByteArray, 0, lByteArray.Length);
            return result;
        }

        private const string EntryStart = "        <td width=\"50\" align=\"center\" nowrap class=\"row4\">";
        private const string PageStart = "onkeypress=\"EnterPage()\"> จากทั้งหมด";

        private GazetteList DoParseStream(Stream data)
        {
            var reader = new System.IO.StreamReader(data, ThaiEncoding);
            GazetteList result = new GazetteList();
            result.entry.AddRange(DoParse(reader).entry);
            return result;
        }

        private GazetteList DoParse(TextReader reader)
        {
            GazetteList result = new GazetteList();
            String currentLine = String.Empty;
            int dataState = -1;
            StringBuilder entryData = new StringBuilder();
            while ( (currentLine = reader.ReadLine()) != null )
            {
                if ( currentLine.Contains(PageStart) )
                {
                    String lTemp = currentLine.Substring(currentLine.LastIndexOf(PageStart) + PageStart.Length, 3).Trim();
                    _numberOfPages = Convert.ToInt32(lTemp);
                }
                else if ( currentLine.StartsWith(EntryStart) )
                {
                    if ( entryData.Length > 0 )
                    {
                        var current = ParseSingeItem(entryData.ToString());
                        if ( current != null )
                        {
                            result.entry.Add(current);
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
            if ( entryData.Length > 0 )
            {
                var current = ParseSingeItem(entryData.ToString());
                if ( current != null )
                {
                    result.entry.Add(current);
                }
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

        private GazetteEntry ParseSingeItem(String value)
        {
            value = value.Replace("\t", "");
            GazetteEntry retval = null;
            Int32 position = value.IndexOf(EntryURL);
            if ( position >= 0 )
            {
                retval = new GazetteEntry();
                position = position + EntryURL.Length;
                Int32 position2 = value.IndexOf(EntryURLend);
                retval.uri = value.Substring(position, position2 - position);
                value = value.Substring(position2, value.Length - position2);
                position = value.IndexOf(EntryTitle) + EntryTitle.Length;
                position2 = value.IndexOf(EntryTitleEnd);
                retval.title = value.Substring(position, position2 - position).Trim();
                value = value.Substring(position2, value.Length - position2);
                position = value.IndexOf(EntryVolumeorPage) + EntryVolumeorPage.Length;
                position2 = value.IndexOf(ColumnEnd, position);
                string volume = value.Substring(position, position2 - position);
                retval.volume = Convert.ToByte(ThaiNumeralHelper.ReplaceThaiNumerals(volume));
                value = value.Substring(position2, value.Length - position2);
                position = value.IndexOf(EntryIssue) + EntryIssue.Length;
                position2 = value.IndexOf(ColumnEnd, position);
                retval.issue = ThaiNumeralHelper.ReplaceThaiNumerals(value.Substring(position, position2 - position).Trim());
                value = value.Substring(position2, value.Length - position2);
                position = value.IndexOf(EntryDate) + EntryDate.Length;
                position2 = value.IndexOf(ColumnEnd, position);
                string Date = value.Substring(position, position2 - position);
                retval.publication = ThaiDateHelper.ParseThaiDate(Date);
                value = value.Substring(position2, value.Length - position2);
                position = value.IndexOf(EntryVolumeorPage) + EntryVolumeorPage.Length;
                position2 = value.IndexOf(ColumnEnd, position);
                string page = value.Substring(position, position2 - position);
                retval.page = ThaiNumeralHelper.ReplaceThaiNumerals(page);

                if ( retval.title.Contains('[') && retval.title.EndsWith("]") )
                {
                    var beginSubTitle = retval.title.LastIndexOf('[');
                    retval.subtitle = retval.title.Substring(beginSubTitle + 1, retval.title.Length - beginSubTitle - 2).Trim();
                    retval.title = retval.title.Substring(0, beginSubTitle - 1).Trim();
                }
            }
            return retval;
        }

        public GazetteList DoGetList(String searchKey, Int32 volume)
        {
            _searchKey = searchKey;
            _volume = volume;
            _cookie = String.Empty;
            GazetteList result = null;
            try
            {
                PerformRequest();
                result = new GazetteList();
                if ( _dataUrl != String.Empty )
                {
                    Stream lData = DoDataDownload(0);
                    result = DoParseStream(lData);
                    for ( Int32 page = 2 ; page <= _numberOfPages ; page++ )
                    {
                        PerformRequestPage(page);
                        Stream lDataPage = DoDataDownload(page);
                        result.entry.AddRange(DoParseStream(lDataPage).entry);
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

        protected GazetteList GetListDescription(String searchKey, Int32 volume, String description)
        {
            GazetteList result = DoGetList(searchKey, volume);
            if ( result != null )
            {
                foreach ( var entry in result.entry )
                {
                    entry.description = description;
                }
            }
            return result;
        }

        public GazetteList SearchNews(DateTime date)
        {
            GazetteList result = new GazetteList();
            result.entry.AddRange(SearchNewsRange(date, date).entry);
            result.entry.Sort((x, y) => DateTime.Compare(x.publication, y.publication));
            return result;
        }

        public GazetteList SearchNewsRange(DateTime beginDate, DateTime endDate)
        {
            GazetteList result = new GazetteList();
            var protecteAreaTypes = new List<ParkType>();
            foreach ( ParkType protectedArea in Enum.GetValues(typeof(ParkType)) )
            {
                protecteAreaTypes.Add(protectedArea);
            }
            var protectedAreasList = SearchNewsProtectedAreas(beginDate, endDate, protecteAreaTypes);
            result.entry.AddRange(protectedAreasList.entry);

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
            result.entry.AddRange(administrativeEntitiesList.entry);
            result.entry.Sort((x, y) => DateTime.Compare(x.publication, y.publication));
            return result;
        }

        public GazetteList SearchNewsProtectedAreas(DateTime beginDate, DateTime endDate, IEnumerable<ParkType> values)
        {
            GazetteList result = new GazetteList();
            Int32 volumeBegin = beginDate.Year - 2007 + 124;
            Int32 volumeEnd = endDate.Year - 2007 + 124;

            for ( Int32 volume = volumeBegin ; volume <= volumeEnd ; volume++ )
            {
                foreach ( KeyValuePair<EntityModification, Dictionary<ParkType, String>> outerKeyValuePair in SearchKeysProtectedAreas )
                {
                    foreach ( KeyValuePair<ParkType, String> keyValuePair in outerKeyValuePair.Value )
                    {
                        if ( values.Contains(keyValuePair.Key) )
                        {
                            var list = GetListDescription(keyValuePair.Value, volume, ModificationText(outerKeyValuePair.Key, keyValuePair.Key));
                            if ( list != null )
                            {
                                result.entry.AddRange(list.entry);
                            }
                        }
                    }
                }
            }
            result.entry.Sort((x, y) => DateTime.Compare(x.publication, y.publication));
            return result;
        }

        public GazetteList SearchNewsRangeAdministrative(DateTime beginDate, DateTime endDate, IEnumerable<EntityType> types, IEnumerable<EntityModification> modifications)
        {
            GazetteList result = new GazetteList();
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
                                    result.entry.AddRange(list.entry);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public GazetteList SearchString(DateTime beginDate, DateTime endDate, String searchKey)
        {
            GazetteList result = new GazetteList();
            Int32 volumeBegin = beginDate.Year - 2007 + 124;
            Int32 volumeEnd = endDate.Year - 2007 + 124;

            for ( Int32 volume = volumeBegin ; volume <= volumeEnd ; volume++ )
            {
                var list = GetListDescription(searchKey, volume, "");
                if ( list != null )
                {
                    result.entry.AddRange(list.entry);
                }
            }
            return result;
        }

        private String ModificationText(EntityModification modification, EntityType entityType)
        {
            String result = String.Format(EntityModificationText[modification], entityType);
            return result;
        }

        private String ModificationText(EntityModification modification, ParkType protectedAreaType)
        {
            String result = String.Format(EntityModificationText[modification], protectedAreaType);
            return result;
        }

        public void SearchNewsNow()
        {
            GazetteList gazetteList = SearchNews(DateTime.Now);
            if ( DateTime.Now.Month == 1 )
            {
                // Check news from last year as well, in case something was added late
                gazetteList.entry.AddRange(SearchNews(DateTime.Now.AddYears(-1)).entry);
            }
            gazetteList.entry.Sort((x, y) => DateTime.Compare(x.publication, y.publication));
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