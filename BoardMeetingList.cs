using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    class BoardMeetingList : List<BoardMeetingEntry>
    {
        public static BoardMeetingList Load(String iFromFile)
        {
            StreamReader lReader = null;
            XmlDocument lXmlDoc = null;
            BoardMeetingList RetVal = null;
            try
            {
                if (!String.IsNullOrEmpty(iFromFile) && File.Exists(iFromFile))
                {
                    lReader = new StreamReader(iFromFile);
                    lXmlDoc = new XmlDocument();
                    lXmlDoc.LoadXml(lReader.ReadToEnd());
                    RetVal = BoardMeetingList.Load(lXmlDoc);
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
        internal static void ParseNode(XmlNode iNode, BoardMeetingList ioList)
        {
            if (iNode.HasChildNodes)
            {
                foreach (XmlNode lChildNode in iNode.ChildNodes)
                {
                    if (lChildNode.Name == "year")
                    {
                        ParseNode(lChildNode, ioList);
                    }
                    else if (lChildNode.Name == "boardmeeting")
                    {
                        ioList.Add(BoardMeetingEntry.Load(lChildNode));
                    }

                }
            }

        }
        internal static BoardMeetingList Load(XmlNode iNode)
        {
            BoardMeetingList RetVal = null;

            if (iNode != null)
            {
                foreach (XmlNode lNode in iNode.ChildNodes)
                {
                    if (lNode != null && lNode.Name.Equals("boardmeetings"))
                    {
                        RetVal = new BoardMeetingList();
                        ParseNode(lNode, RetVal);
                    }
                }
            }

            return RetVal;
        }

        public FrequencyCounter EffectiveDateTillPublication()
        {
            FrequencyCounter lCounter = new FrequencyCounter();
            foreach (BoardMeetingEntry lEntry in this)
            {
                foreach (BoardMeetingTopic lTopic in lEntry.Contents)
                {
                    if ((lTopic.Gazette != null) && (lTopic.Effective.Year>2000))
                    {
                        TimeSpan lDiff = lTopic.TimeTillPublish();
                        {
                            Int32 lGeocode = lTopic.Topic.Geocode;
                            if (lGeocode == 0)
                            {
                                lGeocode = lTopic.Topic.TambonGeocode;
                            }
                            lCounter.IncrementForCount(lDiff.Days, lGeocode);
                        }
                    }
                }
            }
            return lCounter;
        }

        public FrequencyCounter MeetingDateTillPublication()
        {
            FrequencyCounter lCounter = new FrequencyCounter();
            foreach (BoardMeetingEntry lEntry in this)
            {
                foreach (BoardMeetingTopic lTopic in lEntry.Contents)
                {
                    if (lTopic.Gazette != null)
                    {
                        TimeSpan lDiff = lTopic.Gazette.Publication - lEntry.Date;
                        {
                            Int32 lGeocode = lTopic.Topic.Geocode;
                            if (lGeocode == 0)
                            {
                                lGeocode = lTopic.Topic.TambonGeocode;
                            }
                            lCounter.IncrementForCount(lDiff.Days, lGeocode);
                        }
                    }
                }
            }
            return lCounter;
        }
    }
}
