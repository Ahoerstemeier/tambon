using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace De.AHoerstemeier.Tambon
{
    public static class GlobalData
    {
        public const Int32 PopulationStatisticMaxYear = 2013;
        public const Int32 PopulationStatisticMinYear = 1993;

        /// <summary>
        /// Loads the global list of provinces.
        /// </summary>
        public static void LoadBasicGeocodeList()
        {
            var fileName = BaseXMLDirectory + "\\geocode.xml";
            using ( var filestream = new FileStream(fileName, FileMode.Open, FileAccess.Read) )
            {
                Entity geocodes = XmlManager.XmlToEntity<Entity>(filestream, new XmlSerializer(typeof(Entity)));
                var provinces = new List<Entity>();
                foreach ( var entity in geocodes.entity.Where(x => x.type.IsFirstLevelAdministrativeUnit() && !x.IsObsolete) )
                {
                    provinces.Add(entity);
                }
                provinces.Sort((x, y) => x.english.CompareTo(y.english));
                Provinces = provinces;
                geocodes.entity.Clear();
                CountryEntity = geocodes;
                CountryEntity.wiki = new WikiLocation();
                CountryEntity.wiki.wikidata = WikiLocation.WikiDataItems[EntityType.Country];
            }
        }

        public static Entity CountryEntity
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// List of all gazette announcements.
        /// </summary>
        public static GazetteList AllGazetteAnnouncements
        {
            get
            {
                return _allGazetteAnnouncements;
            }
        }

        private static GazetteList _allGazetteAnnouncements = new GazetteList();

        /// <summary>
        /// List of all provinces, without any of the sub-entities.
        /// </summary>
        public static IEnumerable<Entity> Provinces;

        static private Dictionary<UInt32, Entity> _geocodeCache = new Dictionary<UInt32, Entity>();

        /// <summary>
        /// Returns the tree of administrative subdivisions for a given province.
        /// </summary>
        /// <param name="provinceCode">TIS1099 code of the province.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref="provinceCode"/> does not refer to a valid province.</exception>
        /// <returns>Tree of subdivisions.</returns>
        /// <remarks>Internally caches a clone of the returned value, to load the file from disc only once.</remarks>
        static public Entity GetGeocodeList(UInt32 provinceCode)
        {
            Entity result = null;
            if ( !Provinces.Any(entry => entry.geocode == provinceCode) )
            {
                throw new ArgumentOutOfRangeException("provinceCode");
            }
            if ( _geocodeCache.Keys.Contains(provinceCode) )
            {
                result = _geocodeCache[provinceCode].Clone();
            }
            else
            {
                String fileName = GeocodeSourceFile(provinceCode);
                if ( File.Exists(fileName) )
                {
                    using ( var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read) )
                    {
                        result = XmlManager.XmlToEntity<Entity>(fileStream, new XmlSerializer(typeof(Entity)));
                    }
                    _geocodeCache.Add(provinceCode, result.Clone());
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the tree of administrative subdivisions for the whole country.
        /// </summary>
        /// <returns>Tree of subdivisions.</returns>
        static public Entity CompleteGeocodeList()
        {
            var result = CountryEntity.Clone();
            foreach ( var changwat in GlobalData.Provinces )
            {
                var actualChangwat = GetGeocodeList(changwat.geocode);
                result.entity.Add(actualChangwat);
            }
            return result;
        }

        internal static String _BaseXMLDirectory = Path.GetDirectoryName(Application.ExecutablePath);

        public static String BaseXMLDirectory
        {
            get
            {
                return _BaseXMLDirectory;
            }
            set
            {
                _BaseXMLDirectory = value;
            }
        }

        internal static String GeocodeXmlSourceDir()
        {
            String retval = BaseXMLDirectory + "\\geocode\\";
            return retval;
        }

        static private String GeocodeSourceFile(UInt32 provinceGeocode)
        {
            String filename = GeocodeXmlSourceDir() + String.Format("geocode{0:D2}.XML", provinceGeocode);
            return filename;
        }

        /// <summary>
        /// Gets the geocode of the province to be used as the default province.
        /// </summary>
        public static UInt32 PreferredProvinceGeocode
        {
            get
            {
                return 84;
            }
        }

        public static Entity LookupGeocode(UInt32 geocode)
        {
            var provinceCode = GeocodeHelper.ProvinceCode(geocode);
            var changwat = GetGeocodeList(provinceCode);
            var result = changwat.FlatList().FirstOrDefault(x => x.geocode == geocode);
            return result;
        }

        public static void LoadPopulationData()
        {
            var allFlat = CompleteGeocodeList().FlatList();
            foreach ( String file in Directory.EnumerateFiles(BaseXMLDirectory + "\\population\\") )
            {
                using ( var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read) )
                {
                    var data = XmlManager.XmlToEntity<Entity>(fileStream, new XmlSerializer(typeof(Entity)));
                    var flat = data.FlatList();
                    foreach ( var dataPoint in flat.Where(x => x.population.Any()) )
                    {
                        var target = allFlat.SingleOrDefault(x => x.geocode == dataPoint.geocode);
                        if ( target != null )
                        {
                            foreach ( var populationEntry in dataPoint.population )
                            {
                                if ( !target.population.Any(x => x.source == populationEntry.source && x.referencedate == populationEntry.referencedate) )
                                {
                                    target.population.Add(populationEntry);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static Int32 MaximumPossibleElectionYear
        {
            get
            {
                return DateTime.Now.Year + 5;
            }
        }
    }
}