﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    class StatisticsAnnouncementDates : AnnouncementStatistics
    {
        #region properties
        private FrequencyCounter mDaysBetweenSignAndPublication = new FrequencyCounter();
        private FrequencyCounter mDaysBetweenPublicationAndEffective = new FrequencyCounter();
        private RoyalGazetteList mStrangeAnnouncements = new RoyalGazetteList();
        public RoyalGazetteList StrangeAnnouncements { get { return mStrangeAnnouncements; } }
        #endregion
        #region constructor
        public StatisticsAnnouncementDates()
        {
            StartYear = 1883;
            EndYear = DateTime.Now.Year;
        }
        public StatisticsAnnouncementDates(Int32 iStartYear, Int32 iEndYear)
        {
            StartYear = iStartYear;
            EndYear = iEndYear;
        }
        #endregion
        #region methods
        protected override void Clear()
        {
            base.Clear();
            mDaysBetweenPublicationAndEffective = new FrequencyCounter();
            mDaysBetweenSignAndPublication = new FrequencyCounter();
            mStrangeAnnouncements = new RoyalGazetteList();
        }
        protected override void ProcessAnnouncement(RoyalGazette iEntry)
        {
            Int32 lWarningOffsetDays = 345;
            mNumberOfAnnouncements++;
            if (iEntry.Publication.Year > 1)
            {
                if (iEntry.Effective.Year > 1)
                {
                    TimeSpan iTime = iEntry.Publication.Subtract(iEntry.Effective);
                    mDaysBetweenPublicationAndEffective.IncrementForCount(iTime.Days, 0);
                    if (Math.Abs(iTime.Days) > lWarningOffsetDays)
                    {
                        mStrangeAnnouncements.Add(iEntry);
                    }
                }
                if (iEntry.Sign.Year > 1)
                {
                    TimeSpan iTime = iEntry.Publication.Subtract(iEntry.Sign);
                    mDaysBetweenSignAndPublication.IncrementForCount(iTime.Days, 0);
                    if ((iTime.Days < 0) | (iTime.Days > lWarningOffsetDays))
                    {
                        if (!mStrangeAnnouncements.Contains(iEntry))
                        {
                            mStrangeAnnouncements.Add(iEntry);
                        }
                    }
                }
            }
        }
        public override String Information()
        {
            StringBuilder lBuilder = new StringBuilder();
            lBuilder.AppendLine(NumberOfAnnouncements.ToString() + " Announcements");
            lBuilder.AppendLine();

            lBuilder.AppendLine("Days between signature and publication");

            lBuilder.AppendLine("  Maximum: " + mDaysBetweenSignAndPublication.MaxValue.ToString());
            lBuilder.AppendLine("  Minimum: " + mDaysBetweenSignAndPublication.MinValue.ToString());
            lBuilder.AppendLine("  Mean: " + mDaysBetweenSignAndPublication.MeanValue.ToString());
            lBuilder.AppendLine("  Most common: " + mDaysBetweenSignAndPublication.MostCommonValue.ToString());
            lBuilder.AppendLine();

            lBuilder.AppendLine("Days between publication and becoming effective");

            lBuilder.AppendLine("  Maximum: " + mDaysBetweenPublicationAndEffective.MaxValue.ToString());
            lBuilder.AppendLine("  Minimum: " + mDaysBetweenPublicationAndEffective.MinValue.ToString());
            lBuilder.AppendLine("  Mean: " + mDaysBetweenPublicationAndEffective.MeanValue.ToString());
            lBuilder.AppendLine("  Most common: " + mDaysBetweenPublicationAndEffective.MostCommonValue.ToString());
            lBuilder.AppendLine();

            String retval = lBuilder.ToString();
            return retval;
        }
        #endregion
    }
}
