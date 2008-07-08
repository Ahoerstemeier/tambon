using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    class RoyalGazetteIssue : IComparable,ICloneable
    {
        static private List<Char> mIssueBookNames = new List<Char>() { 'ก', 'ง', 'ข', 'ค' };
        #region variables
        private String IssuePrefix;
        private String IssuePostfix;
        private Int32 IssueNumber = -1;
        private Char IssueBook = ' ';
        #endregion
        #region constructor
        public RoyalGazetteIssue(String value)
        {
            ParseIssue(value);
        }
        public RoyalGazetteIssue()
        {
        }
        public RoyalGazetteIssue(RoyalGazetteIssue value)
        {
            IssuePostfix = value.IssuePostfix;
            IssuePrefix = value.IssuePrefix;
            IssueNumber = value.IssueNumber;
            IssueBook = value.IssueBook;
        }
        #endregion
        #region methods
        public Boolean IsEmpty()
        {
            Boolean retval =
                (IssueBook != ' ') |
                (IssueNumber >= 0) |
                (!String.IsNullOrEmpty(IssuePostfix)) |
                (!String.IsNullOrEmpty(IssuePrefix));
            return !retval;
        }
        protected void ParseIssue(String iValue)
        {
            if (!String.IsNullOrEmpty(iValue))
            {
                String lTemp = iValue.Replace("เล่มที่","");

                // Standard format: [พิเศษ] [###] ก [ฉบับพิเศษ]
                foreach (String lSubString in lTemp.Split(new Char[] { ' ' }))
                {
                    if (String.IsNullOrEmpty(lSubString))
                    {
                        // do nothing, just a double space within the source string
                    }
                    else if (lSubString == "พิเศษ")
                    {
                        IssuePrefix = lSubString;
                    }
                    else if (lSubString == "ฉบับพิเศษ")
                    {
                        IssuePostfix = lSubString;
                    }
                    else if (lSubString == "ฉบับเพิ่มเติม")
                    {
                        IssuePostfix = lSubString;
                    }
                    else if (mIssueBookNames.Contains(lSubString[0]))
                    {
                        IssueBook = lSubString[0];
                    }
                    else
                    {
                        IssueNumber = Convert.ToInt32(lSubString);
                    }
                }
            }
        }
        public override String ToString()
        {
            String RetVal = String.Empty;
            if (!String.IsNullOrEmpty(IssuePrefix))
            {
                RetVal = IssuePrefix + " ";
            }
            if (IssueNumber >= 0)
            {
                RetVal = RetVal + IssueNumber.ToString() + " ";
            }
            RetVal = RetVal + IssueBook + " ";
            if (!String.IsNullOrEmpty(IssuePostfix))
            {
                RetVal = RetVal + IssuePostfix + " ";
            }
            RetVal = RetVal.Trim();
            return RetVal;
        }
        #endregion

        #region IComparable Members

        public int CompareTo(object iOther)
        {
            if (iOther is RoyalGazetteIssue)
            {
                Int32 retval = 0;
                RoyalGazetteIssue lOther = (RoyalGazetteIssue)iOther;
                retval = IssueBook.CompareTo(lOther.IssueBook);
                if (retval == 0)
                {
                    retval = IssueNumber.CompareTo(lOther.IssueNumber);
                    if (retval == 0)
                    {
                        retval = String.Compare(IssuePrefix, lOther.IssuePrefix);
                        if (retval == 0)
                        {
                            retval = String.Compare(IssuePostfix, lOther.IssuePostfix);
                        }
                    }
                }
                return retval;
            }
            throw new InvalidCastException("Not a RoyalGazetteIssue");
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new RoyalGazetteIssue(this);
        }

        #endregion
    }
}
