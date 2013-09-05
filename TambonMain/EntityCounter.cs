using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public class EntityCounter
    {
        #region properties

        private Dictionary<String, Int32> _namesCount = new Dictionary<String, Int32>();
        private List<EntityType> _entityTypes;

        public UInt32 BaseGeocode
        {
            get;
            set;
        }

        private Int32 _numberOfEntities;

        public Int32 NumberOfEntities
        {
            get
            {
                return _numberOfEntities;
            }
        }

        #endregion properties

        #region constructor

        public EntityCounter(List<EntityType> entityTypes)
        {
            _entityTypes = entityTypes;
        }

        #endregion constructor

        #region methods

        public String CommonNames(Int32 cutOff)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Total number: " + NumberOfEntities.ToString());

            List<KeyValuePair<String, Int32>> sorted = new List<KeyValuePair<String, Int32>>();
            foreach ( var keyValuePair in _namesCount )
            {
                String name = keyValuePair.Key;
                sorted.Add(keyValuePair);
            }
            sorted.Sort(delegate(KeyValuePair<String, Int32> x, KeyValuePair<String, Int32> y)
            {
                return y.Value.CompareTo(x.Value);
            });
            Int32 count = 0;
            foreach ( var keyValuePair in sorted )
            {
                builder.AppendLine(keyValuePair.Key + " (" + keyValuePair.Value.ToString() + ") ");
                count++;
                if ( count > cutOff )
                {
                    break;
                }
            }

            String result = builder.ToString();
            return result;
        }

        private List<Entity> LoadGeocodeLists()
        {
            var result = new List<Entity>();
            foreach ( var entry in GlobalData.Provinces )
            {
                if ( GeocodeHelper.IsBaseGeocode(BaseGeocode, entry.geocode) )
                {
                    var entities = GlobalData.GetGeocodeList(entry.geocode);
                    var allEntities = entities.FlatList();
                    var allFittingEntities = allEntities.Where(x => _entityTypes.Contains(x.type));
                    result.AddRange(allFittingEntities);
                }
            }
            return result;
        }

        private List<String> NormalizeNameList(IEnumerable<Entity> entities)
        {
            var result = new List<String>();
            foreach ( var entry in entities )
            {
                var name = entry.name;
                if ( entry.type == EntityType.Muban )
                {
                    if ( !String.IsNullOrEmpty(entry.name) )
                    {
                        name = name.StripBanOrChumchon();
                    }
                }
                if ( (!entry.IsObsolete) & (GeocodeHelper.IsBaseGeocode(BaseGeocode, entry.geocode)) )
                {
                    result.Add(name);
                }
            }
            return result;
        }

        private static Dictionary<String, Int32> DoCalculate(IEnumerable<String> entityNames)
        {
            var result = new Dictionary<String, Int32>();
            var weightedList = entityNames.GroupBy(x => x)
                        .Select(g => new
                        {
                            Value = g.Key,
                            Count = g.Count()
                        })
                        .OrderByDescending(x => x.Count);
            foreach ( var entry in weightedList )
            {
                result.Add(entry.Value, entry.Count);
            }
            return result;
        }

        public void Calculate()
        {
            var entities = LoadGeocodeLists();
            Calculate(entities);
        }

        public void Calculate(IEnumerable<Entity> entities)
        {
            var processedEntities = NormalizeNameList(entities);
            _numberOfEntities = processedEntities.Count;
            _namesCount = DoCalculate(processedEntities);
        }

        #endregion methods
    }
}