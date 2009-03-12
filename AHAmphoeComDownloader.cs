using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace De.AHoerstemeier.Tambon
{

    class AHAmphoeComDownloader
    {
        #region Constants
        private const String SearchStringAmphoeChangwat = "<font color=\"#0000FF\" face=\"MS Sans Serif\">";
        private const String SearchStringAmphoeChangwatEnd = "</font></b></td>";
        private const String SearchStringDataLineEnd = "</td>";
        private const String SearchStringDataLineEndTop = "</span></font></td>";
        private const String SearchStringDataCaptionTop = "<font color=\"#0066CC\">";
        private const String SearchStringChangwatSlogan = "คำขวัญจังหวัด";
        private const String SearchStringAmphoeSlogan = "คำขวัญอำเภอ";
        private const String SearchStringDistrictOffice = "ที่อยู่ที่ว่าการอำเภอ";
        private const String SearchStringTelephone = "หมายเลขโทรศัพท์";
        private const String SearchStringFax = "หมายเลขโทรสาร";
        private const String SearchStringWebsite = "เว็บไซต์อำเภอ";
        private const String SearchStringHistory = "1.ประวัติความเป็นมา";
        private const String SearchStringArea = "2.เนื้อที่/พื้นที่";
        private const String SearchStringClimate = "3.สภาพภูมิอากาศโดยทั่วไป";
        private const String SearchStringTambon = "1.ตำบล";
        private const String SearchStringThesaban = "3.เทศบาล";
        private const String SearchStringMuban = "2.หมู่บ้าน";
        private const String SearchStringTAO = "4.อบต";
        private const String SearchStringDataBottom = "<span lang=\"en-us\">";
        private static Dictionary<String, Int16> mProvinceIDs = new Dictionary<String, Int16>() {
{ "กระบี่",1},
{ "กาญจนบุรี",2},
{ "กาฬสินธุ์",3},
{ "กำแพงเพชร",4},
{ "ขอนแก่น",5},
{ "จันทบุรี",6},
{ "ฉะเชิงเทรา",7},
{ "ชลบุรี",8},
{ "ชัยนาท",9},
{ "ชัยภูมิ",10},
{ "ชุมพร",11},
{ "เชียงราย",12},
{ "เชียงใหม่",13},
{ "ตรัง",14},
{ "ตราด",15},
{ "ตาก",16},
{ "นครนายก",17},
{ "นครปฐม",18},
{ "นครพนม",19},
{ "นครราชสีมา",20},
{ "นครศรีธรรมราช",21},
{ "นครสวรรค์",22},
{ "นนทบุรี",23},
{ "นราธิวาส",24},
{ "น่าน",25},
{ "บุรีรัมย์",26},
{ "ปทุมธานี",27},
{ "ประจวบคีรีขันธ์",28},
{ "ปราจีนบุรี",29},
{ "ปัตตานี",30},
{ "พระนครศรีอยุธยา",31},
{ "พะเยา",32},
{ "พังงา",33},
{ "พัทลุง",34},
{ "พิจิตร",35},
{ "พิษณุโลก",36},
{ "เพชรบุรี",37},
{ "เพชรบูรณ์",38},
{ "แพร่",39},
{ "ภูเก็ต",40},
{ "มหาสารคาม",41},
{ "มุกดาหาร",42},
{ "แม่ฮ่องสอน",43},
{ "ยโสธร",44},
{ "ยะลา",45},
{ "ร้อยเอ็ด",46},
{ "ระนอง",47},
{ "ระยอง",48},
{ "ราชบุรี",49},
{ "ลพบุรี",50},
{ "ลำปาง",51},
{ "ลำพูน",52},
{ "เลย",53},
{ "ศรีสะเกษ",54},
{ "สกลนคร",55},
{ "สงขลา",56},
{ "สตูล",57},
{ "สมุทรปราการ",58},
{ "สมุทรสงคราม",59},
{ "สมุทรสาคร",60},
{ "สระแก้ว",61},
{ "สระบุรี",62},
{ "สิงห์บุรี",63},
{ "สุโขทัย",64},
{ "สุพรรณบุรี",65},
{ "สุราษฎร์ธานี",66},
{ "สุรินทร์",67},
{ "หนองคาย",68},
{ "หนองบัวลำภู",69},
{ "อ่างทอง",70},
{ "อำนาจเจริญ",71},
{ "อุดรธานี",72},
{ "อุตรดิตถ์",73},
{ "อุทัยธานี",74},
{ "อุบลราชธานี",75},
 };
        #endregion
        private Int16[][] CreateCodes()
        {
            Int16[][] retval = new Int16[76][];
            retval[1] = new Int16[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            retval[2] = new Int16[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
            retval[3] = new Int16[] { 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 851, 852, 853, 854 };
            retval[4] = new Int16[] { 36, 37, 38, 39, 40, 41, 42, 43, 44, 833, 834 };
            retval[5] = new Int16[] { 45, 46, 47, 48, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 855, 856, 857, 858, 859 };
            retval[6] = new Int16[] { 67, 68, 69, 70, 71, 72, 73, 74, 75, 76 };
            retval[7] = new Int16[] { 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87 };
            retval[8] = new Int16[] { 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98 };
            retval[9] = new Int16[] { 99, 100, 101, 102, 103, 104, 105, 106 };
            retval[10] = new Int16[] { 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 860 };
            retval[11] = new Int16[] { 122, 123, 124, 125, 126, 127, 128, 129 };
            retval[12] = new Int16[] { 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 835, 836 };
            retval[13] = new Int16[] { 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 837, 838 };
            retval[14] = new Int16[] { 168, 169, 170, 171, 172, 173, 174, 175, 176, 177 };
            retval[15] = new Int16[] { 178, 179, 180, 181, 182, 183, 184 };
            retval[16] = new Int16[] { 185, 186, 187, 188, 189, 190, 191, 192, 839 };
            retval[17] = new Int16[] { 193, 194, 195, 196 };
            retval[18] = new Int16[] { 197, 198, 199, 200, 201, 202, 203 };
            retval[19] = new Int16[] { 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 861 };
            retval[20] = new Int16[] { 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 862, 863, 864, 865, 866, 867 };
            retval[21] = new Int16[] { 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263 };
            retval[22] = new Int16[] { 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 840, 841 };
            retval[23] = new Int16[] { 277, 278, 279, 280, 281, 282 };
            retval[24] = new Int16[] { 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295 };
            retval[25] = new Int16[] { 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 842, 849 };
            retval[26] = new Int16[] { 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 868, 869 };
            retval[27] = new Int16[] { 330, 331, 332, 333, 334, 335, 336 };
            retval[28] = new Int16[] { 337, 338, 339, 340, 341, 342, 343, 344 };
            retval[29] = new Int16[] { 345, 346, 347, 348, 349, 350, 351 };
            retval[30] = new Int16[] { 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363 };
            retval[31] = new Int16[] { 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379 };
            retval[32] = new Int16[] { 380, 381, 382, 383, 384, 385, 386, 843, 844 };
            retval[33] = new Int16[] { 387, 388, 389, 390, 391, 392, 393, 394 };
            retval[34] = new Int16[] { 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405 };
            retval[35] = new Int16[] { 406, 407, 408, 409, 410, 411, 412, 413, 845, 846, 847, 850 };
            retval[36] = new Int16[] { 414, 415, 416, 417, 418, 419, 420, 421, 422 };
            retval[37] = new Int16[] { 423, 424, 425, 426, 427, 428, 429, 430 };
            retval[38] = new Int16[] { 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441 };
            retval[39] = new Int16[] { 442, 443, 444, 445, 446, 447, 448, 449 };
            retval[40] = new Int16[] { 450, 451, 452 };
            retval[41] = new Int16[] { 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 870, 871 };
            retval[42] = new Int16[] { 464, 465, 466, 467, 468, 469, 470 };
            retval[43] = new Int16[] { 471, 472, 473, 474, 475, 476, 477 };
            retval[44] = new Int16[] { 478, 479, 480, 481, 482, 483, 484, 485, 486 };
            retval[45] = new Int16[] { 487, 488, 489, 490, 491, 492, 493, 494 };
            retval[46] = new Int16[] { 495, 496, 497, 498, 499, 500, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 872, 873, 874 };
            retval[47] = new Int16[] { 512, 513, 514, 515, 516 };
            retval[48] = new Int16[] { 517, 518, 519, 520, 521, 522, 523, 524 };
            retval[49] = new Int16[] { 525, 526, 527, 528, 529, 530, 531, 532, 533, 534 };
            retval[50] = new Int16[] { 535, 536, 537, 538, 539, 540, 541, 542, 543, 544, 545 };
            retval[51] = new Int16[] { 546, 547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557, 558 };
            retval[52] = new Int16[] { 559, 560, 561, 562, 563, 564, 565, 848 };
            retval[53] = new Int16[] { 566, 567, 568, 569, 570, 571, 572, 573, 574, 575, 576, 577, 875, 876 };
            retval[54] = new Int16[] { 578, 579, 580, 581, 582, 583, 584, 585, 586, 587, 588, 589, 590, 591, 592, 593, 594, 595, 596, 597, 877, 878 };
            retval[55] = new Int16[] { 598, 599, 600, 601, 602, 603, 604, 605, 606, 607, 608, 609, 610, 611, 612, 613, 614, 615 };
            retval[56] = new Int16[] { 616, 617, 618, 619, 620, 621, 622, 623, 624, 625, 626, 627, 628, 629, 630, 631 };
            retval[57] = new Int16[] { 632, 633, 634, 635, 636, 637, 638 };
            retval[58] = new Int16[] { 639, 640, 641, 642, 643, 644 };
            retval[59] = new Int16[] { 645, 646, 647 };
            retval[60] = new Int16[] { 648, 649, 650 };
            retval[61] = new Int16[] { 651, 652, 653, 654, 655, 656, 657, 658, 659 };
            retval[62] = new Int16[] { 660, 661, 662, 663, 664, 665, 666, 667, 668, 669, 670, 671, 672 };
            retval[63] = new Int16[] { 673, 674, 675, 676, 677, 678 };
            retval[64] = new Int16[] { 679, 680, 681, 682, 683, 684, 685, 686, 687 };
            retval[65] = new Int16[] { 688, 689, 690, 691, 692, 693, 694, 695, 696, 697 };
            retval[66] = new Int16[] { 698, 699, 700, 701, 702, 703, 704, 705, 706, 707, 708, 709, 710, 711, 712, 713, 714, 715, 716 };
            retval[67] = new Int16[] { 717, 718, 719, 720, 721, 722, 723, 724, 725, 726, 727, 728, 729, 730, 731, 732, 733 };
            retval[68] = new Int16[] { 734, 735, 736, 737, 738, 739, 740, 741, 742, 743, 744, 745, 746, 747, 748, 749, 750 };
            retval[69] = new Int16[] { 751, 752, 753, 754, 755, 756 };
            retval[70] = new Int16[] { 757, 758, 759, 760, 761, 762, 763 };
            retval[71] = new Int16[] { 764, 765, 766, 767, 768, 769, 770 };
            retval[72] = new Int16[] { 771, 772, 773, 774, 775, 776, 777, 778, 779, 780, 781, 782, 783, 784, 785, 786, 787, 788, 789, 790 };
            retval[73] = new Int16[] { 791, 792, 793, 794, 795, 796, 797, 798, 799 };
            retval[74] = new Int16[] { 800, 801, 802, 803, 804, 805, 806, 807 };
            retval[75] = new Int16[] { 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 818, 819, 820, 821, 822, 823, 824, 825, 826, 827, 828, 829, 830, 831, 832 };
            return retval;
        }
        private Stream DoDownload(Int16 iProvinceID, Int16 iAmphoeID)
        {
            WebClient lWebClient = new WebClient();
            Stream lStream = lWebClient.OpenRead(URL(iProvinceID, iAmphoeID));
            return lStream;
        }

        private Uri URL(short iProvinceID, short iAmphoeID)
        {
            Uri retval = new Uri("http://www.amphoe.com/menu.php?mid=1&am=" + iAmphoeID.ToString() + "&pv=" + iProvinceID.ToString());
            return retval;
        }
        private String RemoveHTML(String iLine)
        {
            String lTempString = iLine.Replace("<br>", "\n");
            lTempString = lTempString.Replace("</td>", "");
            lTempString = lTempString.Replace("</span>", "");
            lTempString = lTempString.Replace("</font>", "");
            lTempString = lTempString.Replace("&nbsp;", " ");
            return lTempString;
        }
        private String ParseTopTableData(String iLine)
        {
            String lTempString = iLine.Substring(0, iLine.IndexOf(SearchStringDataLineEnd));
            lTempString = RemoveHTML(lTempString);
            Int32 lPos = lTempString.LastIndexOf('>') + 1;
            lTempString = lTempString.Substring(lPos);
            lTempString = lTempString.Trim();
            return lTempString;
        }
        private String ParseSecondDataTable(String iLine)
        {
            String lTempString = iLine.Substring(0, iLine.IndexOf(SearchStringDataLineEnd));
            lTempString = RemoveHTML(lTempString).Trim();
            Int32 lPos = lTempString.LastIndexOf('>') + 1;
            lTempString = lTempString.Substring(lPos);
            return lTempString;
        }
        private String TrimMultiLine(String iLine)
        {
            StringReader lReader = new StringReader(iLine);
            String retval = String.Empty;
            String lCurrentLine = String.Empty;
            while ((lCurrentLine = lReader.ReadLine()) != null)
            {
                retval = retval + lCurrentLine.Trim() + "\r\n";
            }
            return retval;
        }

        private void ParseAmphoeChangwatName(String iLine, AHAmphoeComData ioData)
        {
            Int32 lPos1 = iLine.IndexOf(Helper.EntityNames[EntityType.Amphoe]) + Helper.EntityNames[EntityType.Amphoe].Length;
            Int32 lPos2 = iLine.IndexOf("&nbsp;");
            ioData.AmphoeName = iLine.Substring(lPos1, lPos2 - lPos1);
            Int32 lPos3 = iLine.IndexOf(Helper.EntityNames[EntityType.Changwat]) + Helper.EntityNames[EntityType.Changwat].Length;
            Int32 lPos4 = iLine.IndexOf(SearchStringAmphoeChangwatEnd);
            ioData.ChangwatName = iLine.Substring(lPos3, lPos4 - lPos3);
        }
        private AHAmphoeComData Parse(Stream iStream)
        {
            AHAmphoeComData retval = new AHAmphoeComData();
            var lReader = new StreamReader(iStream, Helper.ThaiEncoding);
            String lCurrentLine = String.Empty;
            StringBuilder lEntryData = new StringBuilder();
            Int32 lDataState = 0;
            while ((lCurrentLine = lReader.ReadLine()) != null)
            {
                if (lCurrentLine.Contains(SearchStringAmphoeChangwat))
                {
                    ParseAmphoeChangwatName(lCurrentLine, retval);
                }
                else if (lCurrentLine.Contains(SearchStringDataCaptionTop))
                {
                    if (lCurrentLine.Contains(SearchStringChangwatSlogan))
                    {
                        lDataState = 1;
                    }
                    else if (lCurrentLine.Contains(SearchStringAmphoeSlogan))
                    {
                        lDataState = 2;
                    }
                    else if (lCurrentLine.Contains(SearchStringDistrictOffice))
                    {
                        lDataState = 3;
                    }
                    else if (lCurrentLine.Contains(SearchStringTelephone))
                    {
                        lDataState = 4;
                    }
                    else if (lCurrentLine.Contains(SearchStringFax))
                    {
                        lDataState = 5;
                    }
                    else if (lCurrentLine.Contains(SearchStringWebsite))
                    {
                        lDataState = 6;
                    }
                }
                else if (lCurrentLine.Contains(SearchStringHistory))
                {
                    lDataState = 7;
                }
                else if (lCurrentLine.Contains(SearchStringArea))
                {
                    lDataState = 8;
                }
                else if (lCurrentLine.Contains(SearchStringClimate))
                {
                    lDataState = 9;
                }
                else if (lCurrentLine.Contains(SearchStringTambon))
                {
                    retval.Tambon = ParseSubEntityNumber(lCurrentLine);
                }
                else if (lCurrentLine.Contains(SearchStringMuban))
                {
                    retval.Muban = ParseSubEntityNumber(lCurrentLine);
                }
                else if (lCurrentLine.Contains(SearchStringThesaban))
                {
                    retval.Thesaban = ParseSubEntityNumber(lCurrentLine);
                }
                else if (lCurrentLine.Contains(SearchStringTAO))
                {
                    retval.TAO = ParseSubEntityNumber(lCurrentLine);
                }
                else if (lCurrentLine.Contains(SearchStringDataLineEndTop))
                {
                    String lTempString = ParseTopTableData(lCurrentLine);
                    switch (lDataState)
                    {
                        case 1:
                            retval.ChangwatSlogan = lTempString;
                            break;
                        case 2:
                            retval.AmphoeSlogan = lTempString;
                            break;
                        case 3:
                            retval.DistrictOffice = lTempString;
                            break;
                        case 4:
                            retval.Telephone = lTempString;
                            break;
                        case 5:
                            retval.Fax = lTempString;
                            break;
                        case 6:
                            retval.Website = lTempString;
                            break;
                    }
                }
                else if (lCurrentLine.Contains(SearchStringDataBottom))
                {
                    String lTempString = lCurrentLine;

                    while (!lTempString.Contains(SearchStringDataLineEnd))
                    {
                        if ((lCurrentLine = lReader.ReadLine()) == null)
                        {
                            break;
                        }
                        lTempString = lTempString + lCurrentLine;
                    }
                    lTempString = ParseSecondDataTable(lTempString).Trim();
                    lTempString = TrimMultiLine(lTempString);
                    switch (lDataState)
                    {
                        case 7:
                            retval.History = lTempString;
                            lDataState = 0;
                            break;
                        case 8:
                            retval.Area = lTempString;
                            lDataState = 0;
                            break;
                        case 9:
                            retval.Climate = lTempString;
                            lDataState = 0;
                            break;
                    }
                }
            }
            return retval;
        }

        private Int32 ParseSubEntityNumber(String lCurrentLine)
        {
            String lTempString = lCurrentLine.Substring(lCurrentLine.IndexOf(SearchStringDataBottom) + SearchStringDataBottom.Length);
            lTempString = lTempString.Substring(0, lTempString.IndexOf('<')).Trim();
            Int32 retval = 0;
            try
            {
                retval = Convert.ToInt32(lTempString);
            }
            catch
            {
            }
            return retval;
        }

        public AHAmphoeComData DoIt(Int16 iProvinceID, Int16 iAmphoeID)
        {
            if (!File.Exists(CacheFileName(iAmphoeID)))
            {
                Stream lData = DoDownload(iProvinceID, iAmphoeID);
                SaveToCache(lData, iAmphoeID);
            }
            Stream mData = new FileStream(CacheFileName(iAmphoeID), FileMode.Open);
            var retval = Parse(mData);
            retval.AmphoeID = iAmphoeID;
            retval.ProvinceID = iProvinceID;
            return retval;
        }

        private void SaveToCache(Stream lData, Int16 iAmphoeID)
        {
            System.IO.Stream lFileStream = null;
            String lFileName = CacheFileName(iAmphoeID);
            Directory.CreateDirectory(CacheDirectory());
            try
            {
                try
                {
                    lFileStream = new FileStream(lFileName, FileMode.CreateNew);
                    Helper.StreamCopy(lData, lFileStream);
                    lFileStream.Flush();
                }
                finally
                {
                    lFileStream.Dispose();
                }
            }
            catch
            {
                File.Delete(lFileName);
            }

        }
        private String CacheDirectory()
        {
            String lDir = Path.Combine(AHGlobalSettings.HTMLCacheDir, "amphoe");
            return lDir;
        }

        private String CacheFileName(Int16 iAmphoeID)
        {
            String retval = Path.Combine(CacheDirectory(), "amphoe " + iAmphoeID.ToString() + ".html");
            return retval;
        }
        public List<AHAmphoeComData> DoItAll(Int32 iProvinceGeocode)
        {
            List<AHAmphoeComData> retval = new List<AHAmphoeComData>();
            var lCodes = CreateCodes();
            Int16 lProvinceID = IdFromGeocode(iProvinceGeocode);
            foreach (Int16 lValue in lCodes[lProvinceID])
            {
                retval.Add(DoIt(lProvinceID, lValue));
            }
            return retval;
        }

        private Int16 IdFromGeocode(int iProvinceGeocode)
        {
            Int16 lProvinceID = 0;
            XElement lGeocodeXML = XElement.Load("geocode.xml");

            var lQuery = from c in lGeocodeXML.Descendants("entity")
                         where ((Int32)(c.Attribute("geocode")) == iProvinceGeocode)
                         select c.Attribute("name");
            foreach (String s in lQuery)
            {
                lProvinceID = mProvinceIDs[s];
            }
            return lProvinceID;

        }
    }

}