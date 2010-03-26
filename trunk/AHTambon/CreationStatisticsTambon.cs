using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public class CreationStatisticsTambon : CreationStatisticsCentralGovernment
    {
        #region properties
        private Int32 mMubanNumberEqual;
        private Int32 mMubanNumberChanged;
        #endregion
        #region constructor
        public CreationStatisticsTambon()
        {
            StartYear = 1883;
            EndYear = DateTime.Now.Year;
        }
        public CreationStatisticsTambon(Int32 iStartYear, Int32 iEndYear)
        {
            StartYear = iStartYear;
            EndYear = iEndYear;
        }
        #endregion
        #region methods
        protected override Boolean EntityFitting(EntityType iEntityType)
        {
            Boolean result = (iEntityType == EntityType.Tambon);
            return result;
        }
        protected override void Clear()
        {
            base.Clear();
            mMubanNumberEqual = 0;
            mMubanNumberChanged = 0;
        }
        protected override void ProcessContent(RoyalGazetteContent iContent)
        {
            base.ProcessContent(iContent);
            RoyalGazetteContentCreate lCreate = (RoyalGazetteContentCreate)iContent;

            List<Int32> lTargetMubanNumbers = new List<Int32>();
            foreach (RoyalGazetteContent lSubEntry in lCreate.SubEntries)
            {
                if (lSubEntry is RoyalGazetteContentReassign)
                {
                    RoyalGazetteContentReassign lReassign = (RoyalGazetteContentReassign)lSubEntry;
                    Int32 lTargetMubanCode = lSubEntry.Geocode % 100;
                    if (lTargetMubanCode == 0)
                    { }
                    else if (lTargetMubanNumbers.Contains(lTargetMubanCode))
                    {
                        ;  // This should no happen, probably mistake in XML
                    }
                    else
                    {
                        lTargetMubanNumbers.Add(lTargetMubanCode);
                    }
                    Int32 lOldMubanCode = lReassign.OldGeocode % 100;
                    if ((lTargetMubanCode != 0) & (lOldMubanCode != 0))
                    {
                        if (lTargetMubanCode == lOldMubanCode)
                        {
                            mMubanNumberEqual++;
                        }
                        else
                        {
                            mMubanNumberChanged++;
                        }
                    }

                         
                }
            }

        }

        protected void AppendMubanNumberChangeInfo(StringBuilder iBuilder)
        {
                Int32 mTotal = (mMubanNumberEqual+mMubanNumberChanged);
                if (mTotal > 0)
                {
                    Double mPercent = (100.0*mMubanNumberEqual) / mTotal;

                    iBuilder.AppendLine("Number of muban which kept number: " + 
                        mMubanNumberEqual.ToString() +
                        " out of " + 
                        mTotal.ToString() +
                        " (" + 
                        mPercent.ToString("0#.##") + 
                        "%)");
                    iBuilder.AppendLine();
                }
        }
        public override String Information()
        {
            StringBuilder lBuilder = new StringBuilder();
            AppendBasicInfo(lBuilder, "Tambon");
            AppendChangwatInfo(lBuilder, "Tambon");
            AppendSubEntitiesInfo(lBuilder, "Muban");
            AppendMubanNumberChangeInfo(lBuilder);
            AppendParentNumberInfo(lBuilder, "Tambon");
            
            String retval = lBuilder.ToString();
            return retval;
        }
        #endregion
    }
}