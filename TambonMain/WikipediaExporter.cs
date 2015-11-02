using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Wikibase;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Helper class for export to Wikipedia.
    /// </summary>
    public class WikipediaExporter
    {
        #region fields

        private IEnumerable<Entity> localGovernments;

        private Entity _baseEntity;
        private WikibaseApi _api;
        private WikiDataHelper _helper;

        #endregion fields

        #region properties

        /// <summary>
        /// Whether wikidata should be checked to include Wikipedia links.
        /// </summary>
        /// <value><c>true</c> to check Wikidata for links, <c>false</c> otherwise.</value>
        public Boolean CheckWikiData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the intended data source for the population.
        /// </summary>
        /// <value>Data source for the population.</value>
        public PopulationDataSourceType PopulationDataSource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the year of the population data.
        /// </summary>
        /// <value>The year of the population dsta.</value>
        public Int16 PopulationReferenceYear
        {
            get;
            set;
        }

        #endregion properties

        #region constructor

        /// <summary>
        /// Creates a new instance of <see cref="WikipediaExporter"/>.
        /// </summary>
        /// <param name="baseEntity">Entity representing the country.</param>
        /// <param name="localGovernments">Enumeration of the local governments.</param>
        /// <exception cref="ArgumentNullException"><paramref name="baseEntity"/> or <paramref name="localGovernments"/> is <c>null</c>.</exception>
        public WikipediaExporter(Entity baseEntity, IEnumerable<Entity> localGovernments)
        {
            if ( baseEntity == null )
            {
                throw new ArgumentNullException("baseEntity");
            }
            if ( localGovernments == null )
            {
                throw new ArgumentNullException("localGovernments");
            }

            _baseEntity = baseEntity;
            this.localGovernments = localGovernments;
            PopulationDataSource = PopulationDataSourceType.DOPA;
        }

        #endregion constructor

        #region public methods

        /// <summary>
        /// Creates the administration section for Wikipedia.
        /// </summary>
        /// <param name="entity">Entity to export.</param>
        /// <param name="language">Language to use.</param>
        /// <returns>Wikipedia text of administration section.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="entity"/> has wrong <see cref="Entity.type"/>.</exception>
        /// <exception cref="NotImplementedException"><paramref name="language"/> is not yet implemented.</exception>
        public String AmphoeToWikipedia(Entity entity, Language language)
        {
            if ( entity == null )
            {
                throw new ArgumentNullException("entity");
            }

            if ( !entity.type.IsCompatibleEntityType(EntityType.Amphoe) )
            {
                throw new ArgumentException(String.Format("Entity type {0} not compatible with Amphoe", entity.type));
            }

            String result;

            switch ( language )
            {
                case Language.German:
                    result = AmphoeToWikipediaGerman(entity);
                    break;

                case Language.English:
                    result = AmphoeToWikipediaEnglish(entity);
                    break;

                default:
                    throw new NotImplementedException(String.Format("Unsupported language {0}", language));
            }

            return result;
        }

        /// <summary>
        /// Creates an Wikipedia article stub for a Tambon.
        /// </summary>
        /// <param name="entity">Entity to export.</param>
        /// <param name="language">Language to use.</param>
        /// <returns>Wikipedia text.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="entity"/> has wrong <see cref="Entity.type"/>.</exception>
        /// <exception cref="NotImplementedException"><paramref name="language"/> is not yet implemented.</exception>
        public String TambonArticle(Entity entity, Language language)
        {
            if ( entity == null )
            {
                throw new ArgumentNullException("entity");
            }

            if ( !entity.type.IsCompatibleEntityType(EntityType.Tambon) )
            {
                throw new ArgumentException(String.Format("Entity type {0} not compatible with Tambon", entity.type));
            }
            if ( language != Language.English )
            {
                throw new NotImplementedException(String.Format("Unsupported language {0}", language));
            }

            var englishCulture = new CultureInfo("en-US");
            var province = _baseEntity.entity.FirstOrDefault(x => x.geocode == GeocodeHelper.ProvinceCode(entity.geocode));
            var amphoe = province.entity.FirstOrDefault(x => GeocodeHelper.IsBaseGeocode(x.geocode, entity.geocode));
            var muban = entity.entity.Where(x => x.type == EntityType.Muban && !x.IsObsolete);
            var lao = localGovernments.Where(x => x.LocalGovernmentAreaCoverage.Any(y => y.geocode == entity.geocode));
            var tempList = new List<Entity>() { province, amphoe };
            tempList.AddRange(muban);
            tempList.AddRange(lao);
            var links = RetrieveWikpediaLinks(tempList, language);
            var populationData = entity.population.FirstOrDefault(x => x.Year == PopulationReferenceYear && x.source == PopulationDataSourceType.DOPA);
            Int32 population = 0;
            if ( populationData != null )
            {
                population = populationData.TotalPopulation.total;
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("{{Infobox settlement");
            builder.AppendLine("<!--See the Table at Infobox Settlement for all fields and descriptions of usage-->");
            builder.AppendLine("<!-- Basic info  ---------------->");
            builder.AppendFormat(englishCulture, "|official_name          = {0}", entity.english);
            builder.AppendLine();
            builder.AppendFormat(englishCulture, "|native_name            = {0}", entity.name);
            builder.AppendLine();
            builder.AppendLine("|settlement_type        = [[Tambon]]");
            builder.AppendLine("|motto                  =");
            builder.AppendLine("<!-- Location ------------------>");
            builder.AppendLine("|subdivision_type      = Country ");
            builder.AppendLine("|subdivision_name      ={{flag|Thailand}} ");
            builder.AppendLine("|subdivision_type2      = [[Provinces of Thailand|Province]]");
            builder.AppendFormat(englishCulture, "|subdivision_name2      = {0}", WikiLink(links[province], province.english));
            builder.AppendLine();
            builder.AppendLine("|subdivision_type3      = [[Amphoe]]");
            builder.AppendFormat(englishCulture, "|subdivision_name3      = {0}", WikiLink(links[amphoe], amphoe.english));
            builder.AppendLine();
            builder.AppendLine("<!-- Politics ----------------->");
            builder.AppendLine("|established_title      = ");
            builder.AppendLine("|established_date       = ");
            builder.AppendLine("<!-- Area    --------------------->");
            builder.AppendLine("|area_total_km2           = ");
            builder.AppendLine("|area_water_km2           =");
            builder.AppendLine("<!-- Population   ----------------------->");
            builder.AppendFormat(englishCulture, "|population_as_of               = {0}", PopulationReferenceYear);
            builder.AppendLine();
            builder.AppendLine("|population_footnotes           = ");
            builder.AppendLine("|population_note                = ");
            builder.AppendFormat(englishCulture, "|population_total               = {0:#,###,###}", population);
            builder.AppendLine();
            builder.AppendLine("|population_density_km2         = ");
            builder.AppendLine("<!-- General information  --------------->");
            builder.AppendLine("|timezone               = [[Thailand Standard Time|TST]]");
            builder.AppendLine("|utc_offset             = +7");
            builder.AppendLine("|latd= |latm= |lats= |latNS=N");
            builder.AppendLine("|longd= |longm= |longs= |longEW=E");
            builder.AppendLine("|elevation_footnotes    =  ");
            builder.AppendLine("|elevation_m            = ");
            builder.AppendLine("<!-- Area/postal codes & others -------->");
            builder.AppendLine("|postal_code_type       = Postal code");
            builder.AppendLine("|postal_code            = {{#property:P281}}");
            builder.AppendLine("|area_code              = ");
            builder.AppendLine("|blank_name             = [[TIS 1099]]");
            builder.AppendLine("|blank_info             = {{#property:P1067}}");
            builder.AppendLine("|website                = ");
            builder.AppendLine("|footnotes              = ");
            builder.AppendLine("}}");
            builder.AppendLine();

            String nativeName;
            if ( !String.IsNullOrEmpty(entity.ipa) )
            {
                nativeName = String.Format(englishCulture, "{{{{lang-th|{0}}}}}; {{{{IPA-th|{1}|IPA}}}})", entity.name, entity.ipa);
            }
            else
            {
                nativeName = String.Format(englishCulture, "{{{{lang-th|{0}}}}})", entity.name);
            }

            builder.AppendFormat(englishCulture,
                "'''{0}''' ({{1}) is a ''[[tambon]]'' (subdistrict) of {2}, in {3}, [[Thailand]]. In {4} it had a total population of {5:#,###,###} people.{6}",
                entity.english,
                nativeName,
                WikiLink(links[amphoe], amphoe.english + " District"),
                WikiLink(links[province], province.english + " Province"),
                PopulationReferenceYear,
                population,
                PopulationDataDownloader.WikipediaReference(GeocodeHelper.ProvinceCode(entity.geocode), PopulationReferenceYear, language)
                );
            builder.AppendLine();
            builder.AppendLine();

            builder.AppendLine("==Administration==");
            builder.AppendLine("===Central administration===");
            if ( muban.Any() )
            {
                builder.AppendFormat("The ''tambon'' is subdivided into {0} administrative villages (''[[muban]]'').", muban.Count());
                builder.AppendLine();
                builder.AppendLine("{| class=\"wikitable sortable\"");
                builder.AppendLine("! No.");
                builder.AppendLine("! Name");
                builder.AppendLine("! Thai");
                foreach ( var mu in muban )
                {
                    builder.AppendLine("|-");
                    var muEnglish = mu.english;
                    if ( links.Keys.Contains(mu) )
                    {
                        muEnglish = WikiLink(links[mu], mu.english);
                    }
                    var muNumber = mu.geocode % 100;
                    if ( muNumber < 10 )
                    {
                        builder.AppendFormat("||{{{{0}}}}{0}.||{1}||{2}", muNumber, muEnglish, mu.name);
                    }
                    else
                    {
                        builder.AppendFormat("||{0}.||{1}||{2}", muNumber, mu.english, mu.name);
                    }
                    builder.AppendLine();
                }
                builder.AppendLine("|}");
                builder.AppendLine();
            }
            else
            {
                builder.AppendLine("The ''tambon'' has no administrative villages (''[[muban]]'').");
                builder.AppendLine();
            }
            if ( lao.Any() )
            {
                var enWikipediaLink = new Dictionary<EntityType, String>()
                {
                    {EntityType.ThesabanNakhon, "city (''[[Thesaban#City municipality|Thesaban Nakhon]]'')"},
                    {EntityType.ThesabanMueang, "town (''[[Thesaban#Town municipality|Thesaban Mueang]]'')"},
                    {EntityType.ThesabanTambon, "subdistrict municipality (''[[Thesaban#Subdistrict municipality|Thesaban Tambon]]'')"},
                    {EntityType.TAO, "[[Subdistrict administrative organization|subdistrict administrative organization (SAO)]]"},
                };

                builder.AppendLine("===Local administration===");
                var laoTupel = new List<Tuple<String, String, String>>();
                foreach ( var laoEntity in lao )
                {
                    var laoEnglish = laoEntity.english;
                    if ( links.Keys.Contains(laoEntity) )
                    {
                        laoEnglish = WikiLink(links[laoEntity], laoEnglish);
                    }
                    laoTupel.Add(new Tuple<String, String, String>(enWikipediaLink[laoEntity.type], laoEnglish, laoEntity.FullName));
                }

                if ( lao.Count() == 1 )
                {
                    var firstLao = laoTupel.First();

                    builder.AppendFormat("The whole area of the subdistrict is covered by the {0} {1} ({2})", firstLao.Item1, firstLao.Item2, firstLao.Item3);
                    builder.AppendLine();
                }
                else
                {
                    builder.AppendFormat("The area of the subdistrict is shared by {0} local governments.", lao.Count());
                    builder.AppendLine();
                    foreach ( var tupel in laoTupel )
                    {
                        builder.AppendFormat("*the {0} {1} ({2})", tupel.Item1, tupel.Item2, tupel.Item3);
                        builder.AppendLine();
                    }
                }
                builder.AppendLine();
            }

            builder.AppendLine("==References==");
            builder.AppendLine("{{reflist}}");
            builder.AppendLine();
            builder.AppendLine("==External links==");
            builder.AppendFormat("*[http://www.thaitambon.com/tambon{0} Thaitambon.com on {1}]", entity.geocode, entity.english);
            builder.AppendLine();
            // {{coord|19.0625|N|98.9396|E|source:wikidata-and-enwiki-cat-tree_region:TH|display=title}}
            builder.AppendLine();

            builder.AppendFormat("[[Category:Tambon of {0} Province]]", province.english);
            builder.AppendLine();
            builder.AppendFormat("[[Category:Populated places in {0} Province]]", province.english);
            builder.AppendLine();
            // {{ChiangMai-geo-stub}}

            return builder.ToString();
        }

        #endregion public methods

        #region private methods

        private delegate String CountAsString(Int32 count);

        private AmphoeDataForWikipediaExport CalculateAmphoeData(Entity entity, Language language)
        {
            if ( entity.type.IsCompatibleEntityType(EntityType.Amphoe) )
            {
                var result = new AmphoeDataForWikipediaExport();
                result.Province = _baseEntity.entity.FirstOrDefault(x => x.geocode == GeocodeHelper.ProvinceCode(entity.geocode));
                result.AllTambon.AddRange(entity.entity.Where(x => x.type.IsCompatibleEntityType(EntityType.Tambon) && !x.IsObsolete));
                result.LocalAdministrations.AddRange(entity.LocalGovernmentEntitiesOf(localGovernments).Where(x => !x.IsObsolete));

                var allEntities = result.AllTambon.ToList();
                allEntities.AddRange(result.LocalAdministrations);
                if ( CheckWikiData )
                {
                    foreach ( var keyValuePair in RetrieveWikpediaLinks(allEntities, language) )
                    {
                        result.WikipediaLinks[keyValuePair.Key] = keyValuePair.Value;
                    }
                }
                var counted = entity.CountAllSubdivisions(localGovernments);
                if ( !counted.ContainsKey(EntityType.Muban) )
                {
                    counted[EntityType.Muban] = 0;
                }
                foreach ( var keyValuePair in counted )
                {
                    result.CentralAdministrationCountByEntity[keyValuePair.Key] = keyValuePair.Value;
                }

                result.MaxPopulation = 0;
                foreach ( var tambon in result.AllTambon )
                {
                    var populationData = tambon.population.FirstOrDefault(x => x.Year == PopulationReferenceYear && x.source == PopulationDataSourceType.DOPA);
                    if ( populationData != null )
                    {
                        result.MaxPopulation = Math.Max(result.MaxPopulation, populationData.TotalPopulation.total);
                    }
                }

                foreach ( var keyValuePair in Entity.CountSubdivisions(result.LocalAdministrations) )
                {
                    result.LocalAdministrationCountByEntity[keyValuePair.Key] = keyValuePair.Value;
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates the administration section for German Wikipedia.
        /// </summary>
        /// <param name="entity">Entity to export.</param>
        /// <returns>German Wikipedia text of administration section.</returns>
        private String AmphoeToWikipediaGerman(Entity entity)
        {
            var numberStrings = new Dictionary<Int32, String>() {
                { 1, "eine" },
                { 2, "zwei" },
                { 3, "drei" },
                { 4, "vier" },
                { 5, "fünf" },
                { 6, "sechs" },
                { 7, "sieben" },
                { 8, "acht" },
                { 9, "neun" },
                { 10, "zehn" },
                { 11, "elf" },
                { 12, "zwölf" },
            };
            var wikipediaLink = new Dictionary<EntityType, String>()
            {
                {EntityType.ThesabanNakhon, "[[Thesaban#Großstadt|Thesaban Nakhon]]"},
                {EntityType.ThesabanMueang, "[[Thesaban#Stadt|Thesaban Mueang]]"},
                {EntityType.ThesabanTambon, "[[Thesaban#Kleinstadt|Thesaban Tambon]]"},
                {EntityType.TAO, "[[Verwaltungsgliederung Thailands#Tambon-Verwaltungsorganisationen|Tambon-Verwaltungsorganisationen]]"},
            };

            var amphoeData = CalculateAmphoeData(entity, Language.German);
            var germanCulture = new CultureInfo("de-DE");

            String headerBangkok = "== Verwaltung ==" + Environment.NewLine;
            String textBangkok = "Der Bezirk {0} ist in {1} ''[[Khwaeng]]'' („Unterbezirke“) eingeteilt." + Environment.NewLine + Environment.NewLine;
            String headerAmphoe = "== Verwaltung ==" + Environment.NewLine + "=== Provinzverwaltung ===" + Environment.NewLine;
            String textAmphoe = "Der Landkreis {0} ist in {1} ''[[Tambon]]'' („Unterbezirke“ oder „Gemeinden“) eingeteilt, die sich weiter in {2} ''[[Muban]]'' („Dörfer“) unterteilen." + Environment.NewLine + Environment.NewLine;
            String textAmphoeSingle = "Der Landkreis {0} ist in genau einen ''[[Tambon]]'' („Unterbezirk“ oder „Gemeinde“) eingeteilt, der sich weiter in {2} ''[[Muban]]'' („Dörfer“) unterteilt." + Environment.NewLine + Environment.NewLine;
            String tableHeaderAmphoe =
                "{{| class=\"wikitable\"" + Environment.NewLine +
                "! Nr." + Environment.NewLine +
                "! Name" + Environment.NewLine +
                "! Thai" + Environment.NewLine +
                "! Muban" + Environment.NewLine +
                "! Einw.{0}" + Environment.NewLine;
            String tableHeaderBangkok =
                "{{| class=\"wikitable\"" + Environment.NewLine +
                "! Nr." + Environment.NewLine +
                "! Name" + Environment.NewLine +
                "! Thai" + Environment.NewLine +
                "! Einw.{0}" + Environment.NewLine;
            String tableEntryAmphoe = "|-" + Environment.NewLine +
                "||{0}.||{1}||{{{{lang|th|{2}}}}}||{3}||{4}" + Environment.NewLine;
            String tableEntryBangkok = "|-" + Environment.NewLine +
                "||{0}.||{1}||{{{{lang|th|{2}}}}}||{4}" + Environment.NewLine;
            String tableFooter = "|}" + Environment.NewLine;

            String headerLocal = "=== Lokalverwaltung ===" + Environment.NewLine;
            String textLocalSingular = "Es gibt eine Kommune mit „{0}“-Status ''({1})'' im Landkreis:" + Environment.NewLine;
            String textLocalPlural = "Es gibt {0} Kommunen mit „{1}“-Status ''({2})'' im Landkreis:" + Environment.NewLine;
            String taoWithThesaban = "Außerdem gibt es {0} „[[Verwaltungsgliederung Thailands#Tambon-Verwaltungsorganisationen|Tambon-Verwaltungsorganisationen]]“ ({{{{lang|th|องค์การบริหารส่วนตำบล}}}} – Tambon Administrative Organizations, TAO)" + Environment.NewLine;
            String taoWithoutThesaban = "Im Landkreis gibt es {0} „[[Verwaltungsgliederung Thailands#Tambon-Verwaltungsorganisationen|Tambon-Verwaltungsorganisationen]]“ ({{{{lang|th|องค์การบริหารส่วนตำบล}}}} – Tambon Administrative Organizations, TAO)" + Environment.NewLine;
            String entryLocal = "* {0} (Thai: {{{{lang|th|{1}}}}})";
            String entryLocalCoverage = " bestehend aus {0}.";
            String entryLocalCoverageTwo = " bestehend aus {0} und {1}.";
            String tambonCompleteSingular = "dem kompletten Tambon {0}";
            String tambonPartiallySingular = "Teilen des Tambon {0}";
            String tambonCompletePlural = "den kompletten Tambon {0}";
            String tambonPartiallyPlural = "den Teilen der Tambon {0}";

            CountAsString countAsString = delegate(Int32 count)
            {
                String countAsStringResult;
                if ( !numberStrings.TryGetValue(count, out countAsStringResult) )
                {
                    countAsStringResult = count.ToString(germanCulture);
                }
                return countAsStringResult;
            };

            var result = String.Empty;
            if ( entity.type == EntityType.Khet )
            {
                result = headerBangkok +
                    String.Format(germanCulture, textBangkok, entity.english, countAsString(amphoeData.CentralAdministrationCountByEntity[EntityType.Khwaeng])) +
                    String.Format(germanCulture, tableHeaderBangkok, PopulationDataDownloader.WikipediaReference(GeocodeHelper.ProvinceCode(entity.geocode), PopulationReferenceYear, Language.German));
            }
            else if ( amphoeData.CentralAdministrationCountByEntity[EntityType.Tambon] == 1 )
            {
                result = headerAmphoe +
                    String.Format(germanCulture, textAmphoeSingle, entity.english, countAsString(amphoeData.CentralAdministrationCountByEntity[EntityType.Tambon]), countAsString(amphoeData.CentralAdministrationCountByEntity[EntityType.Muban])) +
                    String.Format(germanCulture, tableHeaderAmphoe, PopulationDataDownloader.WikipediaReference(GeocodeHelper.ProvinceCode(entity.geocode), PopulationReferenceYear, Language.German));
            }
            else
            {
                result = headerAmphoe +
                    String.Format(germanCulture, textAmphoe, entity.english, countAsString(amphoeData.CentralAdministrationCountByEntity[EntityType.Tambon]), countAsString(amphoeData.CentralAdministrationCountByEntity[EntityType.Muban])) +
                    String.Format(germanCulture, tableHeaderAmphoe, PopulationDataDownloader.WikipediaReference(GeocodeHelper.ProvinceCode(entity.geocode), PopulationReferenceYear, Language.German));
            }

            foreach ( var tambon in amphoeData.AllTambon )
            {
                if ( entity.type == EntityType.Khet )
                {
                    result += WikipediaTambonTableEntry(tambon, amphoeData, tableEntryBangkok, germanCulture);
                }
                else
                {
                    result += WikipediaTambonTableEntry(tambon, amphoeData, tableEntryAmphoe, germanCulture);
                }
            }
            result += tableFooter + Environment.NewLine;

            if ( amphoeData.LocalAdministrationCountByEntity.Any() )
            {
                result += headerLocal;
                var check = new List<EntityType>()
                {
                    EntityType.ThesabanNakhon,
                    EntityType.ThesabanMueang,
                    EntityType.ThesabanTambon,
                    EntityType.TAO,
                };
                foreach ( var entityType in check )
                {
                    Int32 count = 0;
                    if ( amphoeData.LocalAdministrationCountByEntity.TryGetValue(entityType, out count) )
                    {
                        if ( entityType == EntityType.TAO )
                        {
                            if ( amphoeData.LocalAdministrationCountByEntity.Keys.Count == 1 )
                            {
                                result += String.Format(germanCulture, taoWithoutThesaban, countAsString(count));
                            }
                            else
                            {
                                result += String.Format(germanCulture, taoWithThesaban, countAsString(count));
                            }
                        }
                        else
                        {
                            if ( count == 1 )
                            {
                                result += String.Format(germanCulture, textLocalSingular, entityType.Translate(Language.German), wikipediaLink[entityType]);
                            }
                            else
                            {
                                result += String.Format(germanCulture, textLocalPlural, countAsString(count), entityType.Translate(Language.German), wikipediaLink[entityType]);
                            }
                        }
                        foreach ( var localEntity in amphoeData.LocalAdministrations.Where(x => x.type == entityType) )
                        {
                            result += WikipediaLocalAdministrationTableEntry(
                                localEntity,
                                amphoeData,
                                entryLocal,
                                tambonCompleteSingular,
                                tambonCompletePlural,
                                tambonPartiallySingular,
                                tambonPartiallyPlural,
                                entryLocalCoverage,
                                entryLocalCoverageTwo,
                                germanCulture);
                        }
                        result += Environment.NewLine;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Creates the administration section for English Wikipedia.
        /// </summary>
        /// <param name="entity">Entity to export.</param>
        /// <returns>English Wikipedia text of administration section.</returns>
        private String AmphoeToWikipediaEnglish(Entity entity)
        {
            var englishCulture = new CultureInfo("en-US");
            var amphoeData = CalculateAmphoeData(entity, Language.English);

            String headerBangkok = "== Administration ==" + Environment.NewLine;
            String textBangkok = "The district {0} is subdivided into {1} subdistricts (''[[Khwaeng]]'')." + Environment.NewLine + Environment.NewLine;
            String headerAmphoe = "== Administration ==" + Environment.NewLine + "=== Central administration ===" + Environment.NewLine;
            String textAmphoe = "The district {0} is subdivided into {1} subdistricts (''[[Tambon]]''), which are further subdivided into {2} administrative villages (''[[Muban]]'')." + Environment.NewLine + Environment.NewLine;
            String textAmphoeSingleTambon = "The district {0} is subdivided into {1} subdistrict (''[[Tambon]]''), which is further subdivided into {2} administrative villages (''[[Muban]]'')." + Environment.NewLine + Environment.NewLine;
            String tableHeaderAmphoe =
                "{{| class=\"wikitable sortable\"" + Environment.NewLine +
                "! No." + Environment.NewLine +
                "! Name" + Environment.NewLine +
                "! Thai" + Environment.NewLine +
                "! Villages" + Environment.NewLine +
                "! [[Population|Pop.]]{0}" + Environment.NewLine;
            String tableHeaderBangkok =
                "{{| class=\"wikitable sortable\"" + Environment.NewLine +
                "! No." + Environment.NewLine +
                "! Name" + Environment.NewLine +
                "! Thai" + Environment.NewLine +
                "! [[Population|Pop.]]{0}" + Environment.NewLine;
            String tableEntryAmphoe = "|-" + Environment.NewLine +
                "||{0}.||{1}||{{{{lang|th|{2}}}}}||{3}||{4}" + Environment.NewLine;
            String tableEntryBangkok = "|-" + Environment.NewLine +
                "||{0}.||{1}||{{{{lang|th|{2}}}}}||{3}" + Environment.NewLine;
            String tableFooter = "|}" + Environment.NewLine;

            String headerLocal = "=== Local administration ===" + Environment.NewLine;
            String textLocalSingular = "There is one {0} in the district:" + Environment.NewLine;
            String textLocalPlural = "There are {0} {1} in the district:" + Environment.NewLine;
            String entryLocal = "* {0} (Thai: {{{{lang|th|{1}}}}})";
            String entryLocalCoverage = " consisting of {0}.";
            String entryLocalCoverageTwo = " consisting of {0} and {1}.";
            String tambonCompleteSingular = "the complete subdistrict {0}";
            String tambonPartiallySingular = "parts of the subdistrict {0}";
            String tambonCompletePlural = "the complete subdistrict {0}";
            String tambonPartiallyPlural = "parts of the subdistricts {0}";

            var enWikipediaLink = new Dictionary<EntityType, String>()
                {
                    {EntityType.ThesabanNakhon, "city (''[[Thesaban#City municipality|Thesaban Nakhon]]'')"},
                    {EntityType.ThesabanMueang, "town (''[[Thesaban#Town municipality|Thesaban Mueang]]'')"},
                    {EntityType.ThesabanTambon, "subdistrict municipality (''[[Thesaban#Subdistrict municipality|Thesaban Tambon]]'')"},
                    {EntityType.TAO, "[[Subdistrict administrative organization|subdistrict administrative organization (SAO)]]"},
                };
            var enWikipediaLinkPlural = new Dictionary<EntityType, String>()
                {
                    {EntityType.ThesabanNakhon, "cities (''[[Thesaban#City municipality|Thesaban Nakhon]]'')"},
                    {EntityType.ThesabanMueang, "towns (''[[Thesaban#Town municipality|Thesaban Mueang]]'')"},
                    {EntityType.ThesabanTambon, "subdistrict municipalities (''[[Thesaban#Subdistrict municipality|Thesaban Tambon]]'')"},
                    {EntityType.TAO, "[[Subdistrict administrative organization|subdistrict administrative organizations (SAO)]]"},
                };

            var result = String.Empty;
            if ( entity.type == EntityType.Khet )
            {
                result = headerBangkok +
                    String.Format(englishCulture, textBangkok, entity.english, amphoeData.CentralAdministrationCountByEntity[EntityType.Khwaeng]) +
                    String.Format(englishCulture, tableHeaderBangkok, PopulationDataDownloader.WikipediaReference(GeocodeHelper.ProvinceCode(entity.geocode), PopulationReferenceYear, Language.English));
            }
            else if ( amphoeData.CentralAdministrationCountByEntity[EntityType.Tambon] == 1 )
            {
                result = headerAmphoe +
                    String.Format(englishCulture, textAmphoeSingleTambon, entity.english, amphoeData.CentralAdministrationCountByEntity[EntityType.Tambon], amphoeData.CentralAdministrationCountByEntity[EntityType.Muban]) +
                    String.Format(englishCulture, tableHeaderAmphoe, PopulationDataDownloader.WikipediaReference(GeocodeHelper.ProvinceCode(entity.geocode), PopulationReferenceYear, Language.English));
            }
            else
            {
                result = headerAmphoe +
                    String.Format(englishCulture, textAmphoe, entity.english, amphoeData.CentralAdministrationCountByEntity[EntityType.Tambon], amphoeData.CentralAdministrationCountByEntity[EntityType.Muban]) +
                    String.Format(englishCulture, tableHeaderAmphoe, PopulationDataDownloader.WikipediaReference(GeocodeHelper.ProvinceCode(entity.geocode), PopulationReferenceYear, Language.English));
            }
            foreach ( var tambon in amphoeData.AllTambon )
            {
                if ( entity.type == EntityType.Khet )
                {
                    result += WikipediaTambonTableEntry(tambon, amphoeData, tableEntryBangkok, englishCulture);
                }
                else
                {
                    result += WikipediaTambonTableEntry(tambon, amphoeData, tableEntryAmphoe, englishCulture);
                }
            }
            result += tableFooter + Environment.NewLine;

            if ( amphoeData.LocalAdministrationCountByEntity.Any() )
            {
                result += headerLocal;
                var check = new List<EntityType>()
                {
                    EntityType.ThesabanNakhon,
                    EntityType.ThesabanMueang,
                    EntityType.ThesabanTambon,
                    EntityType.TAO,
                };
                foreach ( var entityType in check )
                {
                    Int32 count = 0;
                    if ( amphoeData.LocalAdministrationCountByEntity.TryGetValue(entityType, out count) )
                    {
                        if ( count == 1 )
                        {
                            result += String.Format(englishCulture, textLocalSingular, enWikipediaLink[entityType]);
                        }
                        else
                        {
                            result += String.Format(englishCulture, textLocalPlural, count, enWikipediaLinkPlural[entityType]);
                        }
                        foreach ( var localEntity in amphoeData.LocalAdministrations.Where(x => x.type == entityType) )
                        {
                            result += WikipediaLocalAdministrationTableEntry(
                                localEntity,
                                amphoeData,
                                entryLocal,
                                tambonCompleteSingular,
                                tambonCompletePlural,
                                tambonPartiallySingular,
                                tambonPartiallyPlural,
                                entryLocalCoverage,
                                entryLocalCoverageTwo,
                                englishCulture);
                        }
                        result += Environment.NewLine;
                    }
                }
            }
            return result;
        }

        private static String WikiLink(String link, String title)
        {
            if ( link == title )
            {
                return "[[" + title + "]]";
            }
            else
            {
                return "[[" + link + "|" + title + "]]";
            }
        }

        private String WikipediaTambonTableEntry(Entity tambon, AmphoeDataForWikipediaExport amphoeData, String format, CultureInfo culture)
        {
            var subCounted = tambon.CountAllSubdivisions(localGovernments);
            var muban = 0;
            if ( !subCounted.TryGetValue(EntityType.Muban, out muban) )
            {
                muban = 0;
            }
            var citizen = 0;
            var populationData = tambon.population.FirstOrDefault(x => x.Year == PopulationReferenceYear && x.source == PopulationDataSourceType.DOPA);
            if ( populationData != null )
            {
                citizen = populationData.TotalPopulation.total;
            }
            var geocodeString = (tambon.geocode % 100).ToString(culture);
            if ( tambon.geocode % 100 < 10 )
            {
                geocodeString = "{{0}}" + geocodeString;
            }
            String mubanString;
            if ( muban == 0 )
            {
                mubanString = "-";
            }
            else if ( muban < 10 )
            {
                mubanString = "{{0}}" + muban.ToString(culture);
            }
            else
            {
                mubanString = muban.ToString();
            }
            var citizenString = citizen.ToString("###,##0", culture);
            for ( int i = citizenString.Length ; i < amphoeData.MaxPopulation.ToString("###,##0", culture).Length ; i++ )
            {
                citizenString = "{{0}}" + citizenString;
            }
            var romanizedName = tambon.english;
            var link = String.Empty;
            if ( amphoeData.WikipediaLinks.TryGetValue(tambon, out link) )
            {
                romanizedName = WikiLink(link, romanizedName);
            }

            return String.Format(culture, format, geocodeString, romanizedName, tambon.name, mubanString, citizenString);
        }

        private static String WikipediaLocalAdministrationTableEntry(
            Entity localEntity,
            AmphoeDataForWikipediaExport amphoeData,
            String entryLocal,
            String tambonCompleteSingular,
            String tambonCompletePlural,
            String tambonPartiallySingular,
            String tambonPartiallyPlural,
            String entryLocalCoverageOne,
            String entryLocalCoverageTwo,
            CultureInfo culture)
        {
            var result = String.Empty;
            var english = localEntity.english;
            var link = String.Empty;
            if ( amphoeData.WikipediaLinks.TryGetValue(localEntity, out link) )
            {
                english = WikiLink(link, english);
            }
            result += String.Format(culture, entryLocal, english, localEntity.FullName);
            if ( localEntity.LocalGovernmentAreaCoverage.Any() )
            {
                var coverage = localEntity.LocalGovernmentAreaCoverage.GroupBy(x => x.coverage).Select(group => new
                {
                    Coverage = group.Key,
                    TambonCount = group.Count()
                });
                var textComplete = String.Empty;
                var textPartially = String.Empty;

                if ( coverage.Any(x => x.Coverage == CoverageType.completely) )
                {
                    var completeTambon = localEntity.LocalGovernmentAreaCoverage.
                        Where(x => x.coverage == CoverageType.completely).
                        Select(x => amphoeData.Province.FlatList().FirstOrDefault(y => y.geocode == x.geocode));
                    var tambonString = String.Join(", ", completeTambon.Select(x => x.english));
                    if ( coverage.First(x => x.Coverage == CoverageType.completely).TambonCount == 1 )
                    {
                        textComplete = String.Format(culture, tambonCompleteSingular, tambonString);
                    }
                    else
                    {
                        textComplete = String.Format(culture, tambonCompletePlural, tambonString);
                    }
                }
                if ( coverage.Any(x => x.Coverage == CoverageType.partially) )
                {
                    var completeTambon = localEntity.LocalGovernmentAreaCoverage.
                        Where(x => x.coverage == CoverageType.partially).
                        Select(x => amphoeData.Province.FlatList().FirstOrDefault(y => y.geocode == x.geocode));
                    var tambonString = String.Join(", ", completeTambon.Select(x => x.english));
                    if ( coverage.First(x => x.Coverage == CoverageType.partially).TambonCount == 1 )
                    {
                        textPartially = String.Format(culture, tambonPartiallySingular, tambonString);
                    }
                    else
                    {
                        textPartially = String.Format(culture, tambonPartiallyPlural, tambonString);
                    }
                }
                if ( !String.IsNullOrEmpty(textPartially) && !String.IsNullOrEmpty(textComplete) )
                {
                    result += String.Format(culture, entryLocalCoverageTwo, textComplete, textPartially);
                }
                else
                {
                    result += String.Format(culture, entryLocalCoverageOne, textComplete + textPartially);
                }
            }
            result += Environment.NewLine;
            return result;
        }

        private Dictionary<Entity, String> RetrieveWikpediaLinks(IEnumerable<Entity> entities, Language language)
        {
            var result = new Dictionary<Entity, String>();
            if ( _api == null )
            {
                _api = new WikibaseApi("https://www.wikidata.org", "TambonBot");
                _helper = new WikiDataHelper(_api);
            }
            foreach ( var entity in entities.Where(x => x.wiki != null && !String.IsNullOrEmpty(x.wiki.wikidata)) )
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if ( item != null )
                {
                    var links = item.getSitelinks();
                    String languageLink;
                    String wikiIdentifier = String.Empty;
                    switch ( language )
                    {
                        case Language.German:
                            wikiIdentifier = "dewiki";
                            break;

                        case Language.English:
                            wikiIdentifier = "enwiki";
                            break;

                        case Language.Thai:
                            wikiIdentifier = "thwiki";
                            break;
                    }
                    if ( item.getSitelinks().TryGetValue(wikiIdentifier, out languageLink) )
                    {
                        result[entity] = languageLink;
                    }
                }
            }
            return result;
        }

        #endregion private methods
    }

    internal class AmphoeDataForWikipediaExport
    {
        public Dictionary<Entity, String> WikipediaLinks
        {
            get;
            private set;
        }

        public Dictionary<EntityType, Int32> CentralAdministrationCountByEntity
        {
            get;
            private set;
        }

        public Dictionary<EntityType, Int32> LocalAdministrationCountByEntity
        {
            get;
            private set;
        }

        public Int32 MaxPopulation
        {
            get;
            set;
        }

        public List<Entity> AllTambon
        {
            get;
            private set;
        }

        public List<Entity> LocalAdministrations
        {
            get;
            private set;
        }

        public Entity Province
        {
            get;
            set;
        }

        public AmphoeDataForWikipediaExport()
        {
            WikipediaLinks = new Dictionary<Entity, String>();
            CentralAdministrationCountByEntity = new Dictionary<EntityType, Int32>();
            LocalAdministrationCountByEntity = new Dictionary<EntityType, Int32>();
            AllTambon = new List<Entity>();
            LocalAdministrations = new List<Entity>();
        }
    }
}