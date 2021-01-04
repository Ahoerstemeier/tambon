﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
        /// <summary>
        /// Latest year for which the DOPA population statistics is available.
        /// </summary>
        public const Int32 PopulationStatisticMaxYear = 2020;

        /// <summary>
        /// Earliest year for which the DOPA population statistics is available.
        /// </summary>
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
                CountryEntity.wiki = new WikiLocation
                {
                    wikidata = WikiLocation.WikiDataItems[EntityType.Country]
                };
            }
        }

        /// <summary>
        /// Gets the entity for the whole country.
        /// </summary>
        /// <value>The country entity.</value>
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

        /// <summary>
        /// Cache to all gazette announcements.
        /// </summary>
        private static readonly GazetteList _allGazetteAnnouncements = new GazetteList();

        /// <summary>
        /// List of all provinces, without any of the sub-entities.
        /// </summary>
        public static IEnumerable<Entity> Provinces;

        private static readonly Dictionary<UInt32, Entity> _geocodeCache = new Dictionary<UInt32, Entity>();

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

        internal static String _baseXMLDirectory = Path.GetDirectoryName(Application.ExecutablePath);

        public static String BaseXMLDirectory
        {
            get
            {
                return _baseXMLDirectory;
            }
            set
            {
                _baseXMLDirectory = value;
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

        public static Entity LoadPopulationDataUnprocessed(PopulationDataSourceType source, Int16 year)
        {
            Entity result = null;
            if ( !GlobalData.CountryEntity.population.Any(x => x.Year == year && x.source == source) )
            {
                String filename = String.Empty;
                switch ( source )
                {
                    case PopulationDataSourceType.Census:
                        filename = BaseXMLDirectory + "\\population\\census{0}.xml";
                        break;

                    case PopulationDataSourceType.DOPA:
                        filename = BaseXMLDirectory + "\\population\\DOPA{0}.xml";
                        break;
                }
                filename = String.Format(CultureInfo.InvariantCulture, filename, year);
                if ( !string.IsNullOrWhiteSpace(filename) )
                {
                    result = LoadPopulationData(filename);
                }
            }
            return result;
        }

        public static void LoadPopulationData(PopulationDataSourceType source, Int16 year)
        {
            var populationData = LoadPopulationDataUnprocessed(source, year);
            if ( populationData != null )
            {
                MergePopulationData(populationData);
            }

            var geocodeToRecalculate = new List<UInt32>();
            var allEntities = GlobalData.CompleteGeocodeList().FlatList();
            foreach ( var item in allEntities.Where(x =>
                x.newgeocode.Any() &&
                x.population.Any(y => y.Year == year && y.source == source)).ToList() )
            {
                foreach ( var newGeocode in item.newgeocode )
                {
                    var newItem = allEntities.FirstOrDefault(x => x.geocode == newGeocode);
                    if ( newItem != null )
                    {
                        if ( !newItem.IsObsolete )
                        {
                            newItem.population.Add(item.population.First(y => y.Year == year && y.source == source));
                            geocodeToRecalculate.AddRange(GeocodeHelper.ParentGeocodes(newItem.geocode));
                        }
                    }
                }
                geocodeToRecalculate.AddRange(GeocodeHelper.ParentGeocodes(item.geocode));
            }
            if ( source == PopulationDataSourceType.Census )
            {
                // For DOPA need to be done with CalculateLocalGovernmentPopulation
                Entity.FillExplicitLocalGovernmentPopulation(allEntities.Where(x => x.type.IsLocalGovernment()).ToList(), allEntities, source, year);
            }

            foreach ( var recalculate in geocodeToRecalculate.Distinct() )
            {
                var entityToRecalculate = allEntities.FirstOrDefault(x => x.geocode == recalculate);
                if ( entityToRecalculate != null )
                {
                    var data = entityToRecalculate.population.FirstOrDefault(y => y.Year == year && y.source == source);
                    if ( data != null )
                    {
                        data.data.Clear();
                        foreach ( var subentity in entityToRecalculate.entity.Where(x => !x.IsObsolete) )
                        {
                            var subData = subentity.population.FirstOrDefault(y => y.Year == year && y.source == source);
                            if ( subData != null )
                            {
                                foreach ( var subDataPoint in subData.data )
                                {
                                    data.AddDataPoint(subDataPoint);
                                }
                            }
                        }
                        data.CalculateTotal();
                    }
                }
            }
        }

        private static void MergePopulationData(Entity data)
        {
            var allFlat = CompleteGeocodeList().FlatList().Where(x => !x.type.IsCompatibleEntityType(EntityType.Muban)).ToDictionary(x => x.geocode);
            var flat = data.FlatList();

            var dataPoints = flat.Where(x => x.population.Any()).ToList();
            foreach ( var dataPoint in dataPoints )
            {
                if (allFlat.TryGetValue(dataPoint.geocode, out Entity target))
                {
                    foreach (var populationEntry in dataPoint.population)
                    {
                        if (!target.population.Any(x => x.source == populationEntry.source && x.referencedate == populationEntry.referencedate))
                        {
                            target.population.Add(populationEntry);
                        }
                    }
                }
            }
        }

        private static Entity LoadPopulationData(String fileName)
        {
            Entity result = null;
            using ( var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read) )
            {
                result = XmlManager.XmlToEntity<Entity>(fileStream, new XmlSerializer(typeof(Entity)));
                var flat = result.FlatList();

                // propagate population references into the sub-entities
                foreach ( var entity in flat )
                {
                    if ( entity.population.Any() && entity.entity.Any() )
                    {
                        foreach ( var population in entity.population.Where(x => x.reference.Any()) )
                        {
                            foreach ( var subEntity in entity.entity )
                            {
                                foreach ( var target in subEntity.population.Where(x => !x.reference.Any() && x.source == population.source && x.year == population.year) )
                                {
                                    target.reference.AddRange(population.reference);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static void LoadPopulationData()
        {
            foreach ( String filename in Directory.EnumerateFiles(BaseXMLDirectory + "\\population\\") )
            {
                var data = LoadPopulationData(filename);
                MergePopulationData(data);
            }
        }

        public static Int32 MaximumPossibleElectionYear
        {
            get
            {
                return DateTime.Now.Year + 5;
            }
        }

        private static String pdfDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\PDF\\";

        /// <summary>
        /// Gets or sets the folder to store the local cached copies of the Royal Gazette announcement PDFs.
        /// </summary>
        public static String PdfDirectory
        {
            get
            {
                return pdfDirectory;
            }
            set
            {
                pdfDirectory = value;
            }
        }
    }
}