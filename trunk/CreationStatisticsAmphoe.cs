using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace De.AHoerstemeier.Tambon
{
    class CreationStatisticsAmphoe : CreationStatisticsCentralGovernment
    {
        #region properties
        private Int32 mNumberOfKingAmphoeCreations;
        public Int32 NumberOfKingAmphoeCreations { get { return mNumberOfKingAmphoeCreations; } }
        #endregion
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
        #endregion
        #region methods
        protected override void Clear()
        {
            base.Clear();
            mNumberOfKingAmphoeCreations = 0;
        }
        protected override Boolean EntityFitting(EntityType iEntityType)
        {
            Boolean result = (iEntityType == EntityType.Amphoe) | (iEntityType == EntityType.KingAmphoe);
            return result;
        }
        public override String Information()
        {
            StringBuilder lBuilder = new StringBuilder();
            AppendBasicInfo(lBuilder, "Amphoe");
            AppendChangwatInfo(lBuilder, "Amphoe");
            AppendSubEntitiesInfo(lBuilder, "Tambon");
            AppendParentNumberInfo(lBuilder, "Amphoe");

            String retval = lBuilder.ToString();
            return retval;
        }
        protected override void ProcessContent(RoyalGazetteContent iContent)
        {
            base.ProcessContent(iContent);
            RoyalGazetteContentCreate lCreate = (RoyalGazetteContentCreate)iContent;
            if (lCreate.Status == EntityType.KingAmphoe)
            {
                mNumberOfKingAmphoeCreations++;
            }
        }
        protected override void AppendBasicInfo(StringBuilder iBuilder, String iEntityName)
        {
            iBuilder.AppendLine(NumberOfAnnouncements.ToString() + " Announcements");
            iBuilder.AppendLine((NumberOfCreations-NumberOfKingAmphoeCreations).ToString() + " Amphoe created");
            iBuilder.AppendLine(NumberOfKingAmphoeCreations.ToString() + " King Amphoe created");
            iBuilder.AppendLine("Creations per announcements: " + CreationsPerAnnouncement.MeanValue.ToString("F2", CultureInfo.InvariantCulture));
            iBuilder.AppendLine("Maximum creation per announcements: " + CreationsPerAnnouncement.MaxValue.ToString());
            iBuilder.AppendLine();
        }
        #endregion
    }
}
