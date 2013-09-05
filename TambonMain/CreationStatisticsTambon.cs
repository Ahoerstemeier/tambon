using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public class CreationStatisticsTambon : CreationStatisticsCentralGovernment
    {
        #region properties

        private Int32 _mubanNumberEqual;
        private Int32 _mubanNumberChanged;

        #endregion properties

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

        #endregion constructor

        #region methods

        protected override String DisplayEntityName()
        {
            return "Tambon";
        }

        protected override Boolean EntityFitting(EntityType iEntityType)
        {
            Boolean result = (iEntityType == EntityType.Tambon);
            return result;
        }

        protected override void Clear()
        {
            base.Clear();
            _mubanNumberEqual = 0;
            _mubanNumberChanged = 0;
        }

        protected override void ProcessContent(GazetteCreate content)
        {
            base.ProcessContent(content);

            List<UInt32> targetMubanNumbers = new List<UInt32>();
            foreach ( var subEntry in content.Items )
            {
                var reassign = subEntry as GazetteReassign;
                if ( reassign != null )
                {
                    UInt32 targetMubanCode = reassign.geocode % 100;
                    if ( targetMubanCode == 0 )
                    {
                    }
                    else if ( targetMubanNumbers.Contains(targetMubanCode) )
                    {
                        ;  // This should no happen, probably mistake in XML
                    }
                    else
                    {
                        targetMubanNumbers.Add(targetMubanCode);
                    }
                    UInt32 oldMubanCode = reassign.oldgeocode % 100;
                    if ( (targetMubanCode != 0) & (oldMubanCode != 0) )
                    {
                        if ( targetMubanCode == oldMubanCode )
                        {
                            _mubanNumberEqual++;
                        }
                        else
                        {
                            _mubanNumberChanged++;
                        }
                    }
                }
            }
        }

        protected void AppendMubanNumberChangeInfo(StringBuilder builder)
        {
            Int32 total = (_mubanNumberEqual + _mubanNumberChanged);
            if ( total > 0 )
            {
                Double percent = (100.0 * _mubanNumberEqual) / total;

                builder.AppendLine(String.Format("Number of muban which kept number: {0} out of {1} ({2:0#.##}%)", _mubanNumberEqual, total, percent));
                builder.AppendLine();
            }
        }

        public override String Information()
        {
            StringBuilder lBuilder = new StringBuilder();
            AppendBasicInfo(lBuilder);
            AppendChangwatInfo(lBuilder);
            AppendSubEntitiesInfo(lBuilder, "Muban");
            AppendMubanNumberChangeInfo(lBuilder);
            AppendParentNumberInfo(lBuilder);
            AppendDayOfYearInfo(lBuilder);

            String retval = lBuilder.ToString();
            return retval;
        }

        #endregion methods
    }
}