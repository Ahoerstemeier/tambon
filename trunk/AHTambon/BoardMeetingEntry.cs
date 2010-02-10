using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    public class BoardMeetingEntry
    {
        #region properties
        public String BoardNumber { get; set; }
        public int MeetingNumber { get; set; }
        public DateTime Date { get; set; }
        public Uri WebLink { get; set; }
        public List<BoardMeetingTopic> Contents { get; set; }
        #endregion
        #region constructors
        public BoardMeetingEntry()
        {
            Contents = new List<BoardMeetingTopic>();
        }
        #endregion
        #region methods
        internal static BoardMeetingEntry Load(XmlNode iNode)
        {
            BoardMeetingEntry RetVal = null;

            if (iNode != null && iNode.Name.Equals("boardmeeting"))
            {
                RetVal = new BoardMeetingEntry();
                RetVal.WebLink = new Uri(TambonHelper.GetAttribute(iNode, "url"));
                RetVal.Date = TambonHelper.GetAttributeDateTime(iNode, "date");
                RetVal.BoardNumber = TambonHelper.GetAttribute(iNode, "board");
                RetVal.MeetingNumber = Convert.ToInt32(TambonHelper.GetAttribute(iNode, "number"));

                RetVal.LoadContents(iNode);
            }

            return RetVal;
        }

        private void LoadContents(XmlNode iNode)
        {
            foreach (XmlNode lNode in iNode.ChildNodes)
            {
                RoyalGazetteContent lContent = RoyalGazetteContent.CreateContentObject(lNode.Name);
                if (lContent != null)
                {
                    lContent.DoLoad(lNode);
                    BoardMeetingTopic lTopic = new BoardMeetingTopic();
                    lTopic.Topic = lContent;
                    lTopic.Effective = TambonHelper.GetAttributeOptionalDateTime(lNode,"effective");
                    String s = TambonHelper.GetAttributeOptionalString(lNode, "type");
                    if (String.IsNullOrEmpty(s))
                    {
                        s = TambonHelper.GetAttributeOptionalString(lNode, "new");
                    }
                    if (!String.IsNullOrEmpty(s))
                    {
                        lTopic.Type = (EntityType)Enum.Parse(typeof(EntityType), s);
                    }
                    lTopic.FindGazette();
                    Contents.Add(lTopic);
                }
            }
        }
        #endregion
    }
}
