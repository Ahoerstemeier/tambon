using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public class CreationStatisticsAmphoe : CreationStatisticsCentralGovernment
    {
        #region properties

        private Int32 _numberOfKingAmphoeCreations;

        public Int32 NumberOfKingAmphoeCreations
        {
            get
            {
                return _numberOfKingAmphoeCreations;
            }
        }

        #endregion properties

        #region constructor

        public CreationStatisticsAmphoe()
        {
            StartYear = 1883;
            EndYear = DateTime.Now.Year;
        }

        public CreationStatisticsAmphoe(Int32 iStartYear, Int32 iEndYear)
        {
            StartYear = iStartYear;
            EndYear = iEndYear;
        }

        #endregion constructor

        #region methods

        protected override void Clear()
        {
            base.Clear();
            _numberOfKingAmphoeCreations = 0;
        }

        protected override Boolean EntityFitting(EntityType iEntityType)
        {
            Boolean result = (iEntityType == EntityType.Amphoe) | (iEntityType == EntityType.KingAmphoe);
            return result;
        }

        public override String Information()
        {
            StringBuilder lBuilder = new StringBuilder();
            AppendBasicInfo(lBuilder);
            AppendChangwatInfo(lBuilder);
            AppendSubEntitiesInfo(lBuilder, "Tambon");
            AppendParentNumberInfo(lBuilder);
            AppendDayOfYearInfo(lBuilder);

            String retval = lBuilder.ToString();
            return retval;
        }

        protected override void ProcessContent(GazetteCreate content)
        {
            base.ProcessContent(content);
            if ( content.type == EntityType.KingAmphoe )
            {
                _numberOfKingAmphoeCreations++;
            }
        }

        protected override String DisplayEntityName()
        {
            return "Amphoe";
        }

        protected override void AppendBasicInfo(StringBuilder builder)
        {
            builder.AppendLine(String.Format("{0} Announcements", NumberOfAnnouncements));
            builder.AppendLine(String.Format("{0} Amphoe created", NumberOfCreations - NumberOfKingAmphoeCreations));
            builder.AppendLine(String.Format("{0} King Amphoe created", NumberOfKingAmphoeCreations));
            builder.AppendLine(String.Format("Creations per announcements: {0:F2}", CreationsPerAnnouncement.MeanValue));
            builder.AppendLine(String.Format("Maximum creation per announcements: {0}", CreationsPerAnnouncement.MaxValue));
            builder.AppendLine();
        }

        #endregion methods
    }
}