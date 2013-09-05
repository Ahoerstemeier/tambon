using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public abstract class CreationStatisticsCentralGovernment : CreationStatistics
    {
        #region properties

        protected FrequencyCounter _numberOfSubEntities = new FrequencyCounter();
        protected FrequencyCounter _numberOfParentEntities = new FrequencyCounter();
        private Dictionary<UInt32, Int32> _creationsPerParent = new Dictionary<UInt32, Int32>();

        #endregion properties

        #region methods

        protected override void Clear()
        {
            base.Clear();
            _numberOfSubEntities = new FrequencyCounter();
            _numberOfParentEntities = new FrequencyCounter();
            _creationsPerParent = new Dictionary<UInt32, Int32>();
        }

        protected override void ProcessContent(GazetteCreate content)
        {
            base.ProcessContent(content);
            var create = content as GazetteCreate;

            UInt32 parentGeocode = create.geocode / 100;

            if ( !_creationsPerParent.ContainsKey(parentGeocode) )
            {
                _creationsPerParent.Add(parentGeocode, 0);
            }
            _creationsPerParent[parentGeocode]++;

            Int32 maxSubEntityIndex = 0;
            List<UInt32> parentEntities = new List<UInt32>();
            foreach ( GazetteOperationBase subEntry in create.Items )
            {
                var createSubEntry = subEntry as GazetteCreate;
                if ( createSubEntry != null )
                {
                    maxSubEntityIndex++;
                }
                var reassignSubEntry = subEntry as GazetteReassign;
                if ( reassignSubEntry != null )
                {
                    maxSubEntityIndex++;

                    UInt32 parentEntityCode = reassignSubEntry.oldgeocode / 100;
                    if ( !parentEntities.Contains(parentEntityCode) )
                    {
                        parentEntities.Add(parentEntityCode);
                    }
                }
            }

            _numberOfSubEntities.IncrementForCount(maxSubEntityIndex, create.geocode);
            if ( parentEntities.Any() )
            {
                _numberOfParentEntities.IncrementForCount(parentEntities.Count, create.geocode);
            }
        }

        protected virtual void AppendBasicInfo(StringBuilder builder)
        {
            String entityName = DisplayEntityName();
            builder.AppendLine(String.Format("{0} Announcements", NumberOfAnnouncements));
            builder.AppendLine(String.Format("{0} {1} created", NumberOfCreations, entityName));
            builder.AppendLine(String.Format("Creations per announcements: {0:F2}", CreationsPerAnnouncement.MeanValue));
            builder.AppendLine(String.Format("Maximum creation per announcements: {0}", CreationsPerAnnouncement.MaxValue));
            builder.AppendLine();
        }

        protected void AppendChangwatInfo(StringBuilder builder)
        {
            String entityName = DisplayEntityName();
            if ( NumberOfCreations > 0 )
            {
                List<String> changwat = new List<String>();
                Int32 maxNumber = 1;
                foreach ( var entry in GlobalData.Provinces )
                {
                    if ( _numberOfCreationsPerChangwat[entry.geocode] > maxNumber )
                    {
                        maxNumber = _numberOfCreationsPerChangwat[entry.geocode];
                        changwat.Clear();
                    }
                    if ( _numberOfCreationsPerChangwat[entry.geocode] == maxNumber )
                    {
                        changwat.Add(entry.english);
                    }
                }
                builder.AppendLine(String.Format("{0} {1} created in", maxNumber, entityName));
                foreach ( String name in changwat )
                {
                    builder.AppendLine(String.Format("* {0}", name));
                }
                builder.AppendLine();
            }
        }

        protected void AppendSubEntitiesInfo(StringBuilder builder, String subEntityName)
        {
            String entityName = DisplayEntityName();
            if ( NumberOfCreations > 0 )
            {
                builder.AppendLine(String.Format("Most common number of {0}: {1} times", subEntityName, _numberOfSubEntities.MostCommonValue));
                builder.AppendLine(String.Format("Mean number of {0}: {1:F2}", subEntityName, _numberOfSubEntities.MeanValue));
                builder.AppendLine(String.Format("Standard deviation: {0:F2}", _numberOfSubEntities.StandardDeviation));
                builder.AppendLine(String.Format("Lowest number of {0}: {1}", subEntityName, _numberOfSubEntities.MinValue));
                if ( _numberOfSubEntities.MinValue > 0 )
                {
                    foreach ( Int32 geocode in _numberOfSubEntities.Data[_numberOfSubEntities.MinValue] )
                    {
                        builder.Append(geocode.ToString() + ' ');
                    }
                    builder.AppendLine();
                }
                builder.AppendLine(String.Format("Highest number of {0}: {1}", subEntityName, _numberOfSubEntities.MaxValue));
                if ( _numberOfSubEntities.MaxValue > 0 )
                {
                    foreach ( Int32 geocode in _numberOfSubEntities.Data[_numberOfSubEntities.MaxValue] )
                    {
                        builder.Append(geocode.ToString() + ' ');
                    }
                    builder.AppendLine();
                }
                builder.AppendLine();
            }
        }

        protected void AppendParentNumberInfo(StringBuilder builder)
        {
            String parentName = DisplayEntityName();
            if ( NumberOfCreations > 0 )
            {
                builder.AppendLine(String.Format("Highest number of parent {0}: {1}", parentName, _numberOfParentEntities.MaxValue));
                if ( _numberOfParentEntities.MaxValue > 1 )
                {
                    foreach ( Int32 geocode in _numberOfParentEntities.Data[_numberOfParentEntities.MaxValue] )
                    {
                        builder.Append(geocode.ToString() + ' ');
                    }
                    builder.AppendLine();
                }
                Int32[] numberOfParentEntities = new Int32[_numberOfParentEntities.MaxValue + 1];
                foreach ( var parentEntity in _numberOfParentEntities.Data )
                {
                    numberOfParentEntities[parentEntity.Key] = parentEntity.Value.Count;
                }
                for ( Int32 i = 0 ; i <= _numberOfParentEntities.MaxValue ; i++ )
                {
                    if ( numberOfParentEntities[i] != 0 )
                    {
                        builder.AppendLine(i.ToString() + ": " + numberOfParentEntities[i].ToString());
                    }
                }
                builder.AppendLine();
            }
        }

        protected void AppendParentFrequencyInfo(StringBuilder builder, String parentEntityName)
        {
            String entityName = DisplayEntityName();
            var sortedParents = new List<KeyValuePair<UInt32, Int32>>();
            sortedParents.AddRange(_creationsPerParent);
            sortedParents.Sort(delegate(KeyValuePair<UInt32, Int32> x, KeyValuePair<UInt32, Int32> y)
            {
                return y.Value.CompareTo(x.Value);
            });
            if ( sortedParents.Any() )
            {
                var first = sortedParents.FirstOrDefault();
                builder.AppendLine(String.Format("Most {0} created in one {1}: {2}", entityName, parentEntityName, first.Value));
                foreach ( var entry in sortedParents.FindAll(
                    delegate(KeyValuePair<UInt32, Int32> x)
                    {
                        return (x.Value == first.Value);
                    }) )
                {
                    builder.Append(entry.Key.ToString() + ' ');
                }
                builder.Remove(builder.Length - 1, 1);
            }
            builder.AppendLine();
        }

        protected void AppendDayOfYearInfo(StringBuilder builder)
        {
            String entityName = DisplayEntityName();
            DateTime baseDateTime = new DateTime(2004, 1, 1);
            List<KeyValuePair<Int32, Int32>> sortedDays = new List<KeyValuePair<Int32, Int32>>();
            sortedDays.AddRange(EffectiveDayOfYear);
            sortedDays.Sort(delegate(KeyValuePair<Int32, Int32> x, KeyValuePair<Int32, Int32> y)
            {
                return y.Value.CompareTo(x.Value);
            });
            Int32 count = 0;
            if ( sortedDays.Any() )
            {
                foreach ( KeyValuePair<Int32, Int32> data in sortedDays )
                {
                    DateTime current = baseDateTime.AddDays(data.Key - 1);
                    builder.AppendLine(String.Format("{0:MMM dd}: {1} {2} created ", current, EffectiveDayOfYear[data.Key], entityName));
                    count++;
                    if ( count > 10 )
                    {
                        break;
                    }
                }
                builder.AppendLine();
            }
        }

        protected abstract String DisplayEntityName();

        #endregion methods
    }
}