using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public class StatisticsAnnouncementDates : AnnouncementStatistics
    {
        #region properties

        private FrequencyCounter _daysBetweenSignAndPublication = new FrequencyCounter();
        private FrequencyCounter _daysBetweenPublicationAndEffective = new FrequencyCounter();

        public IEnumerable<GazetteEntry> StrangeAnnouncements
        {
            get
            {
                return _strangeAnnouncements;
            }
        }

        private List<GazetteEntry> _strangeAnnouncements = new List<GazetteEntry>();

        #endregion properties

        #region constructor

        public StatisticsAnnouncementDates()
        {
            StartYear = 1883;
            EndYear = DateTime.Now.Year;
        }

        public StatisticsAnnouncementDates(Int32 startYear, Int32 endYear)
        {
            StartYear = startYear;
            EndYear = endYear;
        }

        #endregion constructor

        #region methods

        protected override void Clear()
        {
            base.Clear();
            _daysBetweenPublicationAndEffective = new FrequencyCounter();
            _daysBetweenSignAndPublication = new FrequencyCounter();
            _strangeAnnouncements.Clear();
        }

        protected override void ProcessAnnouncement(GazetteEntry entry)
        {
            Int32 warningOffsetDays = 345;
            Boolean processed = false;
            if ( entry.publication.Year > 1 )
            {
                if ( entry.effective.Year > 1 )
                {
                    processed = true;
                    TimeSpan timeBetweenPublicationAndEffective = entry.publication.Subtract(entry.effective);
                    _daysBetweenPublicationAndEffective.IncrementForCount(timeBetweenPublicationAndEffective.Days, 0);
                    if ( Math.Abs(timeBetweenPublicationAndEffective.Days) > warningOffsetDays )
                    {
                        _strangeAnnouncements.Add(entry);
                    }
                }
                if ( entry.sign.Year > 1 )
                {
                    processed = true;
                    TimeSpan timeBetweenSignAndPublication = entry.publication.Subtract(entry.sign);
                    _daysBetweenSignAndPublication.IncrementForCount(timeBetweenSignAndPublication.Days, 0);
                    if ( (timeBetweenSignAndPublication.Days < 0) | (timeBetweenSignAndPublication.Days > warningOffsetDays) )
                    {
                        if ( !StrangeAnnouncements.Contains(entry) )
                        {
                            _strangeAnnouncements.Add(entry);
                        }
                    }
                }
                if ( processed )
                {
                    NumberOfAnnouncements++;
                }
            }
        }

        public override String Information()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(String.Format("{0} Announcements", NumberOfAnnouncements));
            builder.AppendLine();

            builder.AppendLine("Days between signature and publication");

            builder.AppendLine(String.Format("  Maximum: {0}", _daysBetweenSignAndPublication.MaxValue));
            builder.AppendLine(String.Format("  Minimum: {0}", _daysBetweenSignAndPublication.MinValue));
            builder.AppendLine(String.Format("  Median: {0}", _daysBetweenSignAndPublication.MeanValue));
            builder.AppendLine(String.Format("  Most common: {0}", _daysBetweenSignAndPublication.MostCommonValue));
            builder.AppendLine();

            builder.AppendLine("Days between publication and becoming effective");

            builder.AppendLine(String.Format("  Maximum: {0}", _daysBetweenPublicationAndEffective.MaxValue));
            builder.AppendLine(String.Format("  Minimum: {0}", _daysBetweenPublicationAndEffective.MinValue));
            builder.AppendLine(String.Format("  Median: {0}", _daysBetweenPublicationAndEffective.MeanValue));
            builder.AppendLine(String.Format("  Most common: {0}", _daysBetweenPublicationAndEffective.MostCommonValue));
            builder.AppendLine();

            String retval = builder.ToString();
            return retval;
        }

        #endregion methods
    }
}