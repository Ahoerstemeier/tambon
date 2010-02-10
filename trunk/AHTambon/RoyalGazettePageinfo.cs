using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public class RoyalGazettePageinfo:IComparable,ICloneable
    {
        public Int32 Page { get; set; }
        public Int32 PageEnd { get; set; }

        public override String ToString()
        {
            String retval = Page.ToString();
            if (PageEnd > Page)
            {
                retval = retval + "–" + PageEnd.ToString();
            }
            return retval;
        }
        public void ParseString(String iValue)
        {
            int lState = 0;
            foreach (String SubString in iValue.Split('-','–'))
            {
                switch (lState)
                {
                    case 0: 
                        Page = Convert.ToInt32(SubString);
                        break;
                    case 1: 
                        PageEnd = Convert.ToInt32(SubString);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Invalid page string " + iValue);
                }
                lState++;
            }
        }

        #region constructor
        public RoyalGazettePageinfo()
        {
            Page = 0;
            PageEnd = 0;
        }
        public RoyalGazettePageinfo(Int32 iPage)
        {
            Page = iPage;
            PageEnd = 0;
        }
        public RoyalGazettePageinfo(Int32 iPage, Int32 iPageEnd)
        {
            Page = iPage;
            PageEnd = iPageEnd;
        }
        public RoyalGazettePageinfo(RoyalGazettePageinfo iValue)
        {
            Page = iValue.Page;
            PageEnd = iValue.PageEnd;
        }
        public RoyalGazettePageinfo(String iValue)
        {
            ParseString(iValue);
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new RoyalGazettePageinfo(this);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object iOther)
        {
            if (iOther is RoyalGazettePageinfo)
            {
                RoyalGazettePageinfo lOther = (iOther as RoyalGazettePageinfo);
                Int32 retval = Page.CompareTo(lOther.Page);
                if (retval == 0)
                {
                    if ((PageEnd != 0) & (lOther.PageEnd != 0))
                    {
                        retval = PageEnd.CompareTo(lOther.PageEnd);
                    }
                }
                return retval;
            }
            throw new InvalidCastException("Not a RoyalGazettePageinfo");
        }

        #endregion

        internal bool IsEmpty()
        {
            return (Page == 0);
        }
    }
}
