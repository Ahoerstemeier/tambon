using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wikibase;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Delegate for the <see cref=" WikiDataBot"/>
    /// </summary>
    /// <param name="entities">Entities to process.</param>
    /// <param name="collisionInfo">Trace for items which need manual review.</param>
    /// <param name="overrideData"><c>true</c> if wrong statements should be corrected, <c>false</c> otherwise.</param>
    public delegate void WikiDataTaskDelegate(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData);

    /// <summary>
    /// List entry for the <see cref="WikiDataBot"/> tasks.
    /// </summary>
    public class WikiDataTaskInfo
    {
        /// <summary>
        /// Gets the description of the task.
        /// </summary>
        /// <value>The description of the task.</value>
        public String DisplayName
        {
            get;
            private set;
        }

        /// <summary>
        /// The delegate to be run for the task.
        /// </summary>
        /// <value>The delegate to be run.</value>
        public WikiDataTaskDelegate Task
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of <see cref="WikiDataTaskInfo"/>.
        /// </summary>
        /// <param name="displayName">Description of the task.</param>
        /// <param name="task">Delegate to be run.</param>
        public WikiDataTaskInfo(String displayName, WikiDataTaskDelegate task)
        {
            DisplayName = displayName;
            Task = task;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <returns>Display name.</returns>
        public override string ToString()
        {
            return DisplayName;
        }
    }

    /// <summary>
    /// Bot to edit the Thai administrative entities on WikiData.
    /// </summary>
    public class WikiDataBot
    {
        #region fields

        private Dictionary<Language, String> _languageCode;

        private List<WikiDataTaskInfo> _availableTasks;

        private WikiDataHelper _helper;

        private Dictionary<WikiDataState, Int32> _runInfo;

        #endregion fields

        #region properties

        /// <summary>
        /// Gets the available tasks.
        /// </summary>
        /// <value>Available tasks.</value>
        public IEnumerable<WikiDataTaskInfo> AvailableTasks
        {
            get
            {
                return _availableTasks;
            }
        }

        /// <summary>
        /// Gets the information of the previous run.
        /// </summary>
        /// <value>Information on previous run.</value>
        public Dictionary<WikiDataState, Int32> RunInfo
        {
            get
            {
                return _runInfo;
            }
        }

        #endregion properties

        #region constructor

        /// <summary>
        /// Creates a new instance of <see cref="WikiDataBot"/>.
        /// </summary>
        /// <param name="helper">Wiki data helper.</param>
        /// <exception cref="ArgumentNullException"><paramref name="helper"/> is <c>null</c>.</exception>
        public WikiDataBot(WikiDataHelper helper)
        {
            if ( helper == null )
            {
                throw new ArgumentNullException("helper");
            }

            _helper = helper;
            _runInfo = new Dictionary<WikiDataState, Int32>();

            _availableTasks = new List<WikiDataTaskInfo>();
            WikiDataTaskDelegate setDescriptionEnglish = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetDescription(Language.English, entities, collisionInfo, overrideData);
            WikiDataTaskDelegate setDescriptionThai = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetDescription(Language.Thai, entities, collisionInfo, overrideData);
            WikiDataTaskDelegate setDescriptionGerman = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetDescription(Language.German, entities, collisionInfo, overrideData);
            _availableTasks.Add(new WikiDataTaskInfo("Set description [en]", setDescriptionEnglish));
            _availableTasks.Add(new WikiDataTaskInfo("Set description [de]", setDescriptionGerman));
            _availableTasks.Add(new WikiDataTaskInfo("Set description [th]", setDescriptionThai));
            WikiDataTaskDelegate setLabelEnglish = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetLabel(Language.English, entities, collisionInfo, overrideData);
            WikiDataTaskDelegate setLabelGerman = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetLabel(Language.German, entities, collisionInfo, overrideData);
            WikiDataTaskDelegate setLabelThai = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetLabel(Language.Thai, entities, collisionInfo, overrideData);
            _availableTasks.Add(new WikiDataTaskInfo("Set label [en]", setLabelEnglish));
            _availableTasks.Add(new WikiDataTaskInfo("Set label [de]", setLabelGerman));
            _availableTasks.Add(new WikiDataTaskInfo("Set label [th]", setLabelThai));
            _availableTasks.Add(new WikiDataTaskInfo("Set country", SetCountry));
            _availableTasks.Add(new WikiDataTaskInfo("Set is in administrative unit", SetIsInAdministrativeUnit));
            WikiDataTaskDelegate setTypeOfAdministrativeUnit = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetTypeOfAdministrativeUnit(entities, collisionInfo, overrideData, false);
            WikiDataTaskDelegate setInstanceOf = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetTypeOfAdministrativeUnit(entities, collisionInfo, overrideData, true);
            _availableTasks.Add(new WikiDataTaskInfo("Set type of administrative unit", setTypeOfAdministrativeUnit));
            _availableTasks.Add(new WikiDataTaskInfo("Set instance of", setInstanceOf));
            _availableTasks.Add(new WikiDataTaskInfo("Set OpenStreetMap", SetOpenStreetMap));
            _availableTasks.Add(new WikiDataTaskInfo("Set ContainsSubdivisions", SetContainsSubdivisions));
            _availableTasks.Add(new WikiDataTaskInfo("Set TIS 1099", SetGeocode));
            _availableTasks.Add(new WikiDataTaskInfo("Set Postal code", SetPostalCode));
            _availableTasks.Add(new WikiDataTaskInfo("Set Location", SetLocation));
            WikiDataTaskDelegate setCensus2010 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.Census, 2010);
            _availableTasks.Add(new WikiDataTaskInfo("Set Census 2010", setCensus2010));

            _languageCode = new Dictionary<Language, String>()
            {
                {Language.English,"en"},
                {Language.German,"de"},
                {Language.Thai,"th"},
            };
        }

        #endregion constructor

        #region public methods

        /// <summary>
        /// Disconnects from WikiData server.
        /// </summary>
        public void LogOut()
        {
            _helper.Api.logout();
        }

        /// <summary>
        /// Counts the links to the various Wikimedia projects.
        /// </summary>
        /// <param name="entities">Entities to check.</param>
        /// <returns>Number of sitelinks by name of the wiki.</returns>
        public Dictionary<String, Int32> CountSiteLinks(IEnumerable<Entity> entities, StringBuilder collisionData)
        {
            var result = new Dictionary<String, Int32>();
            result["Orphan"] = 0;
            result["Deleted"] = 0;
            // Create a new EntityProvider instance and pass the api created above.
            EntityProvider entityProvider = new EntityProvider(_helper.Api);
            foreach ( var entity in entities )
            {
                // Get an entity by searching for the id
                var item = _helper.GetWikiDataItemForEntity(entity);
                if ( item != null )
                {
                    var links = item.getSitelinks();
                    if ( !links.Any() )
                    {
                        result["Orphan"]++;
                        if ( collisionData != null )
                        {
                            collisionData.AppendFormat("Orphan: {0} - {1} ({2})", entity.wiki.wikidata, entity.english, entity.geocode);
                            collisionData.AppendLine();
                        }
                    }

                    foreach ( var key in links.Keys )
                    {
                        if ( !result.ContainsKey(key) )
                        {
                            result[key] = 0;
                        }
                        result[key]++;
                    }
                }
                else
                {
                    result["Deleted"]++;
                    if ( collisionData != null )
                    {
                        collisionData.AppendFormat("Deleted: {0} - {1} ({2})", entity.wiki.wikidata, entity.english, entity.geocode);
                        collisionData.AppendLine();
                    }
                }
            }
            return result;
        }

        #endregion public methods

        #region private methods

        private void SetDescription(Language language, IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            if ( entities == null )
            {
                throw new ArgumentNullException("entities");
            }
            var languageCode = _languageCode[language];
            ClearRunInfo();

            foreach ( var entity in entities )
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if ( item == null )
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                }
                else
                {
                    var oldDescription = item.getDescription(languageCode);
                    var newDescription = entity.GetWikiDataDescription(language);

                    if ( String.IsNullOrEmpty(oldDescription) )
                    {
                        _runInfo[WikiDataState.NotSet]++;
                        item.setDescription(languageCode, newDescription);
                        item.save(String.Format("Added description [{0}]: {1}", languageCode, newDescription));
                    }
                    else if ( oldDescription != newDescription )
                    {
                        _runInfo[WikiDataState.WrongValue]++;
                        if ( collisionInfo != null )
                        {
                            collisionInfo.AppendFormat("{0}: {1} already has description [{2}] \"{3}\"", item.id, entity.english, languageCode, oldDescription);
                            collisionInfo.AppendLine();
                        }
                        if ( overrideData )
                        {
                            item.setDescription(languageCode, newDescription);
                            item.save(String.Format("Updated description [{0}]: {1}", languageCode, newDescription));
                        }
                    }
                    else
                    {
                        _runInfo[WikiDataState.Valid]++;
                    }
                }
            }
        }

        private void SetLabel(Language language, IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            if ( entities == null )
            {
                throw new ArgumentNullException("entities");
            }
            var languageCode = _languageCode[language];
            ClearRunInfo();

            foreach ( var entity in entities )
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if ( item == null )
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                }
                else
                {
                    var oldLabel = item.getLabel(languageCode);
                    String newLabel;
                    if ( language == Language.Thai )
                    {
                        newLabel = entity.FullName;
                    }
                    else
                    {
                        if ( entity.type == EntityType.Chumchon )
                        {
                            newLabel = entity.english.StripBanOrChumchon();
                        }
                        else
                        {
                            newLabel = entity.english;
                        }
                    }

                    if ( String.IsNullOrEmpty(oldLabel) )
                    {
                        _runInfo[WikiDataState.NotSet]++;
                        item.setLabel(languageCode, newLabel);
                        item.save(String.Format("Added label [{0}]: {1}", languageCode, newLabel));
                    }
                    else if ( oldLabel != newLabel )
                    {
                        _runInfo[WikiDataState.WrongValue]++;
                        if ( collisionInfo != null )
                        {
                            collisionInfo.AppendFormat("{0}: {1} already has label [{2}] \"{3}\"", item.id, entity.english, languageCode, oldLabel);
                            collisionInfo.AppendLine();
                        }
                        if ( overrideData )
                        {
                            item.setLabel(languageCode, newLabel);
                            item.save(String.Format("Updated label [{0}]: {1}", languageCode, newLabel));
                        }
                    }
                    else
                    {
                        _runInfo[WikiDataState.Valid]++;
                    }
                }
            }
        }

        private void ClearRunInfo()
        {
            _runInfo.Clear();
            foreach ( var state in Enum.GetValues(typeof(WikiDataState)) )
            {
                _runInfo[(WikiDataState)state] = 0;
            }
        }

        private void SetCountry(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            if ( entities == null )
            {
                throw new ArgumentNullException("entities");
            }
            ClearRunInfo();
            foreach ( var entity in entities )
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if ( item == null )
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                }
                else
                {
                    var state = _helper.IsInCountryCorrect(item);
                    _runInfo[state]++;
                    if ( state == WikiDataState.WrongValue )
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong country", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if ( state != WikiDataState.Valid )
                    {
                        var statement = _helper.SetIsInCountry(item, overrideData);
                        if ( statement != null )
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                }
            }
        }

        private void SetOpenStreetMap(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            if ( entities == null )
            {
                throw new ArgumentNullException("entities");
            }
            ClearRunInfo();
            foreach ( var entity in entities.Where(x => x.wiki.openstreetmapSpecified) )
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if ( item == null )
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                }
                else
                {
                    var state = _helper.OpenStreetMapCorrect(item, entity);
                    _runInfo[state]++;
                    if ( state == WikiDataState.WrongValue )
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong OpenStreetMap id", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if ( state != WikiDataState.Valid )
                    {
                        var statement = _helper.SetOpenStreetMap(item, entity, overrideData);
                        if ( statement != null )
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                }
            }
        }

        private void SetGeocode(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            if ( entities == null )
            {
                throw new ArgumentNullException("entities");
            }
            ClearRunInfo();
            foreach ( var entity in entities )
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if ( item == null )
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                }
                else
                {
                    var state = _helper.GeocodeCorrect(item, entity);
                    _runInfo[state]++;
                    if ( state == WikiDataState.WrongValue )
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong geocode id", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if ( state != WikiDataState.Valid )
                    {
                        var statement = _helper.SetGeocode(item, entity, overrideData);
                        if ( statement != null )
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                    // TODO: Sources
                }
            }
        }

        private void SetIsInAdministrativeUnit(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            if ( entities == null )
            {
                throw new ArgumentNullException("entities");
            }
            ClearRunInfo();
            foreach ( var entity in entities )
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if ( item == null )
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                }
                else
                {
                    var state = _helper.IsInAdministrativeUnitCorrect(item, entity);
                    _runInfo[state]++;
                    if ( state == WikiDataState.WrongValue )
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong parent", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if ( state != WikiDataState.Valid )
                    {
                        var statement = _helper.SetIsInAdministrativeUnit(item, entity, overrideData);
                        if ( statement != null )
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                }
            }
        }

        private void SetTypeOfAdministrativeUnit(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData, Boolean useInstanceOf)
        {
            if ( entities == null )
            {
                throw new ArgumentNullException("entities");
            }
            ClearRunInfo();
            foreach ( var entity in entities.Where(x => x.type != EntityType.Thesaban) )
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if ( item == null )
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                }
                else
                {
                    var state = _helper.TypeOfAdministrativeUnitCorrect(item, entity, useInstanceOf);
                    _runInfo[state]++;
                    if ( state == WikiDataState.WrongValue )
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong type", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if ( state != WikiDataState.Valid )
                    {
                        var statement = _helper.SetTypeOfAdministrativeUnit(item, entity, overrideData, useInstanceOf);
                        if ( statement != null )
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                }
            }
        }

        private void SetContainsSubdivisions(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            if ( entities == null )
            {
                throw new ArgumentNullException("entities");
            }
            ClearRunInfo();
            foreach ( var entity in entities )
            {
                if ( entity.entity.Where(x => !x.IsObsolete && !x.type.IsLocalGovernment()).All(x => x.wiki != null && !String.IsNullOrEmpty(x.wiki.wikidata)) )
                {
                    var item = _helper.GetWikiDataItemForEntity(entity);
                    if ( item == null )
                    {
                        _runInfo[WikiDataState.ItemNotFound]++;
                    }
                    else
                    {
                        foreach ( var subEntity in entity.entity.Where(x => !x.type.IsLocalGovernment()) )
                        {
                            var state = _helper.ContainsSubdivisionsCorrect(item, entity, subEntity);
                            _runInfo[state]++;
                            if ( state == WikiDataState.Incomplete )
                            {
                                var statement = _helper.SetContainsSubdivisions(item, entity, subEntity);
                                if ( statement != null )
                                {
                                    statement.save(_helper.GetClaimSaveEditSummary(statement));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetPostalCode(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            if ( entities == null )
            {
                throw new ArgumentNullException("entities");
            }
            ClearRunInfo();
            foreach ( var entity in entities.Where(x => !x.IsObsolete && x.codes != null && x.codes.post != null && x.codes.post.value.Any()) )
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if ( item == null )
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                }
                else
                {
                    foreach ( var code in entity.codes.post.value )
                    {
                        var state = _helper.PostalCodeCorrect(item, entity, code);
                        _runInfo[state]++;
                        if ( state == WikiDataState.Incomplete )
                        {
                            var statement = _helper.SetPostalCode(item, entity, code);
                            if ( statement != null )
                            {
                                statement.save(_helper.GetClaimSaveEditSummary(statement));
                            }
                        }
                    }
                }
            }
        }

        private void SetPopulationData(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData, PopulationDataSourceType dataSource, Int16 year)
        {
            if ( entities == null )
            {
                throw new ArgumentNullException("entities");
            }
            ClearRunInfo();
            foreach ( var entity in entities.Where(x => x.population.Any(y => y.source == dataSource && y.Year == year)) )
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if ( item == null )
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                }
                else
                {
                    var data = entity.population.First(y => y.source == dataSource && y.Year == year);

                    var state = _helper.PopulationDataCorrect(item, data);
                    _runInfo[state]++;
                    if ( state != WikiDataState.Valid )
                    {
                        var statement = _helper.SetPopulationData(item, data, overrideData);
                        if ( statement != null )
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                            _helper.AddPopulationDataReferences(statement, data);
                            foreach ( var reference in statement.References )
                            {
                                reference.Save(_helper.GetReferenceSaveEditSummary(reference));
                            }

                            _helper.AddPopulationDataQualifiers(statement, data);
                            foreach ( var qualifier in statement.Qualifiers )
                            {
                                qualifier.Save(_helper.GetQualifierSaveEditSummary(qualifier));
                            }
                        }
                    }
                }
            }
        }

        private void SetLocation(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            if ( entities == null )
            {
                throw new ArgumentNullException("entities");
            }
            ClearRunInfo();
            foreach ( var entity in entities )
            {
                Boolean hasValue = false;
                var office = entity.office.FirstOrDefault();
                if ( office != null )
                {
                    hasValue = office.Point != null;
                }
                if ( hasValue )
                {
                    var item = _helper.GetWikiDataItemForEntity(entity);
                    if ( item == null )
                    {
                        _runInfo[WikiDataState.ItemNotFound]++;
                    }
                    else
                    {
                        var state = _helper.LocationCorrect(item, entity);
                        _runInfo[state]++;
                        if ( state == WikiDataState.WrongValue )
                        {
                            collisionInfo.AppendFormat("{0}: {1} has wrong location", item.id, entity.english);
                            collisionInfo.AppendLine();
                        }
                        if ( state != WikiDataState.Valid )
                        {
                            var statement = _helper.SetLocation(item, entity, overrideData);
                            if ( statement != null )
                            {
                                statement.save(_helper.GetClaimSaveEditSummary(statement));
                            }
                        }
                    }
                }
            }
        }

        #endregion private methods
    }
}