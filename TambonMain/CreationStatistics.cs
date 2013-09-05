using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public abstract class CreationStatistics : AnnouncementStatistics
    {
        #region properties

        private FrequencyCounter _creationsPerAnnouncement = new FrequencyCounter();

        protected FrequencyCounter CreationsPerAnnouncement
        {
            get
            {
                return _creationsPerAnnouncement;
            }
        }

        private Int32 _numberOfCreations;

        public Int32 NumberOfCreations
        {
            get
            {
                return _numberOfCreations;
            }
        }

        protected Int32[] _numberOfCreationsPerChangwat = new Int32[100];

        private Dictionary<Int32, Int32> _effectiveDayOfYear = new Dictionary<int, int>();

        public Dictionary<Int32, Int32> EffectiveDayOfYear
        {
            get
            {
                return _effectiveDayOfYear;
            }
        }

        #endregion properties

        #region methods

        protected override void Clear()
        {
            base.Clear();
            _creationsPerAnnouncement = new FrequencyCounter();
            _numberOfCreationsPerChangwat = new Int32[100];
            _numberOfCreations = 0;
        }

        protected abstract Boolean EntityFitting(EntityType iEntityType);

        protected Boolean ContentFitting(GazetteOperationBase content)
        {
            Boolean result = false;
            GazetteCreate create = content as GazetteCreate;
            if ( create != null )
            {
                result = EntityFitting(create.type);
            }
            return result;
        }

        protected virtual void ProcessContent(GazetteCreate content)
        {
            _numberOfCreations++;

            UInt32 changwatGeocode = content.geocode;
            while ( changwatGeocode > 100 )
            {
                changwatGeocode = changwatGeocode / 100;
            }
            _numberOfCreationsPerChangwat[changwatGeocode]++;
        }

        protected override void ProcessAnnouncement(GazetteEntry entry)
        {
            Int32 count = 0;
            UInt32 provinceGeocode = 0;
            foreach ( var content in entry.Items )
            {
                var contentBase = content as GazetteOperationBase;
                if ( ContentFitting(contentBase) )
                {
                    count++;
                    ProcessContent(content as GazetteCreate);
                    provinceGeocode = contentBase.geocode;
                    while ( provinceGeocode / 100 != 0 )
                    {
                        provinceGeocode = provinceGeocode / 100;
                    }
                }
            }
            if ( count > 0 )
            {
                NumberOfAnnouncements++;
                _creationsPerAnnouncement.IncrementForCount(count, provinceGeocode);
                if ( entry.effective.Year > 1 )
                {
                    DateTime dummy = new DateTime(2004, entry.effective.Month, entry.effective.Day);
                    Int32 index = dummy.DayOfYear;
                    if ( !_effectiveDayOfYear.ContainsKey(index) )
                    {
                        _effectiveDayOfYear[index] = 0;
                    }
                    _effectiveDayOfYear[index]++;
                }
            }
        }

        #endregion methods
    }
}