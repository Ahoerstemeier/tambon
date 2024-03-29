﻿using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using Wikibase;
using Wikibase.DataValues;

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

        public WikiDataTaskInfo SetContainsSubdivisionTask
        {
            get;
            private set;
        }

        public WikiDataTaskInfo SetLocatorMapTask
        {
            get;
            private set;
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
            _ = helper ?? throw new ArgumentNullException(nameof(helper));

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
            _availableTasks.Add(new WikiDataTaskInfo("Set IPA", SetIpa));
            _availableTasks.Add(new WikiDataTaskInfo("Set Thai abbreviation", SetThaiAbbreviation));
            _availableTasks.Add(new WikiDataTaskInfo("Set country", SetCountry));
            _availableTasks.Add(new WikiDataTaskInfo("Set is in administrative unit", SetIsInAdministrativeUnit));
            WikiDataTaskDelegate setInstanceOf = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetTypeOfAdministrativeUnit(entities, collisionInfo, overrideData);
            // _availableTasks.Add(new WikiDataTaskInfo("Set type of administrative unit", setTypeOfAdministrativeUnit));
            _availableTasks.Add(new WikiDataTaskInfo("Set instance of", setInstanceOf));
            _availableTasks.Add(new WikiDataTaskInfo("Set OpenStreetMap", SetOpenStreetMap));
            SetContainsSubdivisionTask = new WikiDataTaskInfo("Set ContainsSubdivisions", SetContainsSubdivisions);
            _availableTasks.Add(SetContainsSubdivisionTask);
            SetLocatorMapTask = new WikiDataTaskInfo("Set locator map", SetLocatorMap);
            _availableTasks.Add(SetLocatorMapTask);
            _availableTasks.Add(new WikiDataTaskInfo("Set TIS 1099", SetGeocode));
            _availableTasks.Add(new WikiDataTaskInfo("Set GND reference", SetGnd));
            _availableTasks.Add(new WikiDataTaskInfo("Set GNS-UFI reference", SetGNSUFI));
            _availableTasks.Add(new WikiDataTaskInfo("Set Facebook place id", SetFacebookPlaceId));
            _availableTasks.Add(new WikiDataTaskInfo("Set official website", SetOfficialWebsite));
            _availableTasks.Add(new WikiDataTaskInfo("Set WOEID reference", SetWoeid));
            _availableTasks.Add(new WikiDataTaskInfo("Set HASC reference", SetHASC));
            _availableTasks.Add(new WikiDataTaskInfo("Set GADM reference", SetGadm));
            _availableTasks.Add(new WikiDataTaskInfo("Set geonames reference", SetGeonames));
            _availableTasks.Add(new WikiDataTaskInfo("Set Postal code", SetPostalCode));
            _availableTasks.Add(new WikiDataTaskInfo("Set Location", SetLocation));
            WikiDataTaskDelegate setCensus2010 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.Census, 2010);
            _availableTasks.Add(new WikiDataTaskInfo("Set Census 2010", setCensus2010));
            WikiDataTaskDelegate setDopa2021 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.DOPA, 2021);
            _availableTasks.Add(new WikiDataTaskInfo("Set DOPA population 2021", setDopa2021));
            WikiDataTaskDelegate setDopa2020 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.DOPA, 2020);
            _availableTasks.Add(new WikiDataTaskInfo("Set DOPA population 2020", setDopa2020));
            WikiDataTaskDelegate setDopa2019 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.DOPA, 2019);
            _availableTasks.Add(new WikiDataTaskInfo("Set DOPA population 2019", setDopa2019));
            WikiDataTaskDelegate setDopa2018 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.DOPA, 2018);
            _availableTasks.Add(new WikiDataTaskInfo("Set DOPA population 2018", setDopa2018));
            WikiDataTaskDelegate setDopa2017 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.DOPA, 2017);
            //            _availableTasks.Add(new WikiDataTaskInfo("Set DOPA population 2017", setDopa2017));
            //            WikiDataTaskDelegate setDopa2016 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.DOPA, 2016);
            //            _availableTasks.Add(new WikiDataTaskInfo("Set DOPA population 2016", setDopa2016));
            WikiDataTaskDelegate setDopa2015 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.DOPA, 2015);
            _availableTasks.Add(new WikiDataTaskInfo("Set DOPA population 2015", setDopa2015));
            // WikiDataTaskDelegate setDopa2014 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.DOPA, 2014);
            // _availableTasks.Add(new WikiDataTaskInfo("Set DOPA population 2014", setDopa2014));
            WikiDataTaskDelegate setCensus2000 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.Census, 2000);
            _availableTasks.Add(new WikiDataTaskInfo("Set Census 2000", setCensus2000));
            WikiDataTaskDelegate setCensus1990 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.Census, 1990);
            _availableTasks.Add(new WikiDataTaskInfo("Set Census 1990", setCensus1990));
            WikiDataTaskDelegate setCensus1980 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.Census, 1980);
            _availableTasks.Add(new WikiDataTaskInfo("Set Census 1980", setCensus1980));
            WikiDataTaskDelegate setCensus1970 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.Census, 1970);
            _availableTasks.Add(new WikiDataTaskInfo("Set Census 1970", setCensus1970));
            WikiDataTaskDelegate setCensus1960 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.Census, 1960);
            _availableTasks.Add(new WikiDataTaskInfo("Set Census 1960", setCensus1960));
            WikiDataTaskDelegate setCensus1947 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.Census, 1947);
            _availableTasks.Add(new WikiDataTaskInfo("Set Census 1947", setCensus1947));
            WikiDataTaskDelegate setCensus1937 = (IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData) => SetPopulationData(entities, collisionInfo, overrideData, PopulationDataSourceType.Census, 1937);
            _availableTasks.Add(new WikiDataTaskInfo("Set Census 1937", setCensus1937));
            _availableTasks.Add(new WikiDataTaskInfo("Set Slogan", SetSlogan));
            _availableTasks.Add(new WikiDataTaskInfo("Set native label", SetNativeLabel));
            _availableTasks.Add(new WikiDataTaskInfo("Set official name", SetOfficialName));
            _availableTasks.Add(new WikiDataTaskInfo("Set bounding entities", SetShareBorderWith));
            _availableTasks.Add(new WikiDataTaskInfo("Set Inception", SetInception));
            _availableTasks.Add(new WikiDataTaskInfo("Set Described by Url", SetDescribedByUrl));
            _availableTasks.Add(new WikiDataTaskInfo("Set named after subdivision", SetNamedAfterSubdivision));
            _availableTasks.Add(new WikiDataTaskInfo("Set overlap", SetOverlap));
            _availableTasks.Add(new WikiDataTaskInfo("Cleanup population data", CleanupPopulationData));
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
            foreach (var entity in entities)
            {
                // Get an entity by searching for the id
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item != null)
                {
                    var links = item.getSitelinks();
                    if (!links.Any())
                    {
                        result["Orphan"]++;
                        if (collisionData != null)
                        {
                            collisionData.AppendFormat("Orphan: {0} - {1} ({2})", entity.wiki.wikidata, entity.english, entity.geocode);
                            collisionData.AppendLine();
                        }
                    }

                    foreach (var key in links.Keys)
                    {
                        if (!result.ContainsKey(key))
                        {
                            result[key] = 0;
                        }
                        result[key]++;
                    }
                }
                else
                {
                    result["Deleted"]++;
                    if (collisionData != null)
                    {
                        collisionData.AppendFormat("Deleted: {0} - {1} ({2})", entity.wiki.wikidata, entity.english, entity.geocode);
                        collisionData.AppendLine();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Create a new Wikidata item for the given entity and fills the basic properties.
        /// </summary>
        /// <param name="entity">Entity to need new item.</param>
        public void CreateItem(Entity entity)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            if (entity.wiki != null && !String.IsNullOrWhiteSpace(entity.wiki.wikidata))
            {
                throw new ArgumentException("Entity already has a Wikidata item");
            }

            var item = new Item(_helper.Api);
            item.setLabel("en", entity.english);
            item.setLabel("de", entity.english);
            item.setLabel("th", entity.FullName);
            item.setDescription("en", entity.GetWikiDataDescription(Language.English));
            item.setDescription("de", entity.GetWikiDataDescription(Language.German));
            item.setDescription("th", entity.GetWikiDataDescription(Language.Thai));
            item.save(_helper.GetItemCreateSaveSummary(item));
            if (entity.wiki == null)
            {
                entity.wiki = new WikiLocation();
            }
            entity.wiki.wikidata = item.id.PrefixedId.ToUpperInvariant();
            var items = new List<Entity>();
            items.Add(entity);
            var dummy = new StringBuilder();
            SetThaiAbbreviation(items, dummy, false);
            SetCountry(items, dummy, false);
            SetIsInAdministrativeUnit(items, dummy, false);
            SetTypeOfAdministrativeUnit(items, dummy, false);
            if (!entity.type.IsCompatibleEntityType(EntityType.Muban) && !entity.type.IsLocalGovernment())
            {
                SetGeocode(items, dummy, false);
            }
            SetPostalCode(items, dummy, false);
            SetLocation(items, dummy, false);
            SetNativeLabel(items, dummy, false);
            SetOfficialName(items, dummy, false);
            SetDescribedByUrl(items, dummy, false);
            SetNamedAfterSubdivision(items, dummy, false);
            SetOverlap(items, dummy, false);
        }

        /// <summary>
        /// Create a new Wikidata item for the category to the given entity and fills the basic properties.
        /// </summary>
        /// <param name="entity">Entity to need new item.</param>
        public void CreateCategory(Entity entity)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));
            if (entity.wiki == null || String.IsNullOrWhiteSpace(entity.wiki.wikidata))
            {
                throw new ArgumentException("Entity has no Wikidata item yet");
            }

            var item = new Item(_helper.Api);
            item.setLabel("en", "Category:" + entity.EnglishFullName);
            // item.setLabel("de", "Kategorie:" + entity.english);
            item.setLabel("th", "หมวดหมู่:" + entity.FullName);
            item.setDescription("en", "Wikimedia category page");
            item.setDescription("de", "Wikimedia-Kategorie");
            item.setDescription("th", "หน้าหมวดหมู่วิกิพีเดีย");
            item.save(_helper.GetItemCreateSaveSummary(item));

            Statement instanceStatement;
            _helper.CheckPropertyValue(item, WikiBase.PropertyIdInstanceOf, WikiBase.ItemWikimediaCategory, true, false, out instanceStatement);
            if (instanceStatement != null)
            {
                instanceStatement.save(_helper.GetClaimSaveEditSummary(instanceStatement));
            }

            Statement categoryStatement;
            _helper.CheckPropertyValue(item, WikiBase.PropertyIdTopicForCategory, entity.wiki.wikidata, true, false, out categoryStatement);
            if (categoryStatement != null)
            {
                categoryStatement.save(_helper.GetClaimSaveEditSummary(categoryStatement));
            }

            Statement linkStatement;
            var entityItem = _helper.GetWikiDataItemForEntity(entity);
            _helper.CheckPropertyValue(entityItem, WikiBase.PropertyIdCategoryForTopic, item.id.PrefixedId, true, false, out linkStatement);
            if (linkStatement != null)
            {
                linkStatement.save(_helper.GetClaimSaveEditSummary(linkStatement));
            }
        }

        #endregion public methods

        #region private methods

        private void SetDescription(Language language, IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            var languageCode = language.ToCode();
            ClearRunInfo();

            foreach (var entity in entities)
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var oldDescription = item.getDescription(languageCode);
                    var newDescription = entity.GetWikiDataDescription(language);

                    if (String.IsNullOrEmpty(oldDescription))
                    {
                        _runInfo[WikiDataState.NotSet]++;
                        item.setDescription(languageCode, newDescription);
                        item.save(String.Format("Added description [{0}]: {1}", languageCode, newDescription));
                    }
                    else if (oldDescription != newDescription)
                    {
                        _runInfo[WikiDataState.WrongValue]++;
                        if (collisionInfo != null)
                        {
                            collisionInfo.AppendFormat("{0}: {1} already has description [{2}] \"{3}\"", item.id, entity.english, languageCode, oldDescription);
                            collisionInfo.AppendLine();
                        }
                        if (overrideData)
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

        public String GetCommonsCategory(Entity entity)
        {
            var result = String.Empty;
            var item = _helper.GetWikiDataItemForEntity(entity);
            if (item != null)
            {
                result = _helper.GetStringClaim(item, WikiBase.PropertyIdCommonsCategory);
            }
            return result;
        }

        public Boolean CheckCommonsCategory(Entity entity)
        {
            var result = true;
            var commonsCategoryOnItem = String.Empty;
            var item = _helper.GetWikiDataItemForEntity(entity);
            if (item != null)
            {
                commonsCategoryOnItem = _helper.GetStringClaim(item, WikiBase.PropertyIdCommonsCategory);
                var categoryItem = _helper.GetItemClaim(item, WikiBase.PropertyIdCategoryForTopic);
                if (categoryItem != null)
                {
                    var commonsCategoryOnCategory = _helper.GetStringClaim(categoryItem, WikiBase.PropertyIdCommonsCategory);
                    if (!String.IsNullOrEmpty(commonsCategoryOnCategory))
                    {
                        result &= commonsCategoryOnItem == commonsCategoryOnCategory;
                    }
                    var siteLinks = categoryItem.getSitelinks();
                    var commonsLink = String.Empty;
                    if (siteLinks.ContainsKey(WikiBase.SiteLinkCommons))
                    {
                        commonsLink = categoryItem.getSitelink(WikiBase.SiteLinkCommons);
                    }
                    if (!String.IsNullOrEmpty(commonsCategoryOnItem))
                    {
                        result &= commonsLink == "Category:" + commonsCategoryOnItem;
                    }
                    else if (!String.IsNullOrEmpty(commonsLink))
                    {
                        result = false; // commons link in category, but no commons category property set on main item
                    }
                }
            }
            return result;
        }

        private void SetLabel(Language language, IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            var languageCode = language.ToCode();
            ClearRunInfo();

            foreach (var entity in entities)
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var oldLabel = item.getLabel(languageCode);
                    String newLabel;
                    if (language == Language.Thai)
                    {
                        newLabel = entity.FullName;
                    }
                    else
                    {
                        if (entity.type == EntityType.Chumchon)
                        {
                            newLabel = entity.english.StripBanOrChumchon();
                        }
                        else if (entity.type == EntityType.PAO)
                        {
                            newLabel = entity.english + " " + entity.type.Translate(language);
                        }
                        else
                        {
                            newLabel = entity.english;
                        }
                    }

                    if (String.IsNullOrEmpty(oldLabel))
                    {
                        _runInfo[WikiDataState.NotSet]++;
                        item.setLabel(languageCode, newLabel);
                        item.save(String.Format("Added label [{0}]: {1}", languageCode, newLabel));
                    }
                    else if (oldLabel != newLabel)
                    {
                        _runInfo[WikiDataState.WrongValue]++;
                        if (collisionInfo != null)
                        {
                            collisionInfo.AppendFormat("{0}: {1} already has label [{2}] \"{3}\"", item.id, entity.english, languageCode, oldLabel);
                            collisionInfo.AppendLine();
                        }
                        if (overrideData)
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

        private void SetThaiAbbreviation(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            var languageCode = Language.Thai.ToCode();
            ClearRunInfo();

            foreach (var entity in entities.Where(x => x.type != EntityType.Muban))
            {
                String newAlias = entity.AbbreviatedName;
                if (!String.IsNullOrEmpty(newAlias))
                {
                    var item = _helper.GetWikiDataItemForEntity(entity);
                    if (item == null)
                    {
                        _runInfo[WikiDataState.ItemNotFound]++;
                        collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                    }
                    else
                    {
                        var oldAliases = item.getAlias(languageCode);
                        if ((oldAliases == null) || !oldAliases.Contains(newAlias))
                        {
                            _runInfo[WikiDataState.NotSet]++;
                            item.addAlias(languageCode, newAlias);
                            item.save(String.Format("Added alias [{0}]: {1}", languageCode, newAlias));
                        }
                        else
                        {
                            _runInfo[WikiDataState.Valid]++;
                        }
                    }
                }
            }
        }

        private void ClearRunInfo()
        {
            _runInfo.Clear();
            foreach (var state in Enum.GetValues(typeof(WikiDataState)))
            {
                _runInfo[(WikiDataState)state] = 0;
            }
        }

        private void SetCountry(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities)
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.IsInCountryCorrect(item);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong country", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetIsInCountry(item, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set the <see cref="WikiBase.PropertyIdTerritoryIdentical"/> and <see cref="WikiBase.PropertyIdTerritoryOverlap"/> for
        /// <see cref="EntityType.Tambon"/> and the local government.
        /// </summary>
        /// <param name="entities">Entities to process.</param>
        /// <param name="collisionInfo">Collision info to return.</param>
        /// <param name="overrideData"><c>true</c> to override faulty data, <c>false</c> otherwise.</param>
        private void SetOverlap(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException("entities");
            ClearRunInfo();
            foreach (var entity in entities)
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.IsOverlapCorrect(item, entity);

                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong overlap", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statements = _helper.SetOverlap(item, entity, overrideData);
                        foreach (var statement in statements)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                }
            }
        }

        private void SetOpenStreetMap(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => x.wiki.openstreetmapSpecified))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.OpenStreetMapCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong OpenStreetMap id", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetOpenStreetMap(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                }
            }
        }

        private void SetGeocode(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities)
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.GeocodeCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong geocode id", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetGeocode(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                    // TODO: Sources
                }
            }
        }

        private void SetSlogan(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities)
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.SloganCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong slogan", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetSlogan(item, entity);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                            if ((statement != null) && entity.type.IsCompatibleEntityType(EntityType.Amphoe))
                            {
                                var source = AmphoeComHelper.AmphoeWebsite(entity.geocode);
                                if (source != null)
                                {
                                    var snak = new Snak(SnakType.Value, new EntityId(WikiBase.PropertyIdReferenceUrl), new StringValue(source.AbsoluteUri));
                                    var sloganReference = statement.CreateReferenceForSnak(snak);
                                    statement.AddReference(sloganReference);
                                    foreach (var reference in statement.References)
                                    {
                                        reference.Save(_helper.GetReferenceSaveEditSummary(reference));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets <see cref="WikiBase.PropertyIdNativeLabel"/> for the given <paramref name="entities"/>.
        /// </summary>
        /// <param name="entities">Entities to set.</param>
        /// <param name="collisionInfo">Container to fill with information on any problems.</param>
        /// <param name="overrideData"><c>true</c> to override wrong data, <c>false</c> otherwise.</param>
        /// <exception cref="ArgumentNullException"><paramref name="entities"/> is <c>null</c>.</exception>
        private void SetNativeLabel(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities)
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.NativeLabelCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong native label", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetNativeLabel(item, entity);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets <see cref="WikiBase.PropertyIdOfficialName"/> for the given <paramref name="entities"/>.
        /// </summary>
        /// <param name="entities">Entities to set.</param>
        /// <param name="collisionInfo">Container to fill with information on any problems.</param>
        /// <param name="overrideData"><c>true</c> to override wrong data, <c>false</c> otherwise.</param>
        /// <exception cref="ArgumentNullException"><paramref name="entities"/> is <c>null</c>.</exception>
        private void SetOfficialName(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities)
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.OfficialNameCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong official name", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetOfficialName(item, entity);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                }
            }
        }

        private void SetIpa(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => !String.IsNullOrWhiteSpace(x.ipa)))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.IpaCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong IPA", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetIpa(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));

                            _helper.AddLanguageOfWorkQualifier(statement);
                            foreach (var qualifier in statement.Qualifiers)
                            {
                                qualifier.Save(_helper.GetQualifierSaveEditSummary(qualifier));
                            }
                        }
                    }
                }
            }
        }

        private void SetGnd(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => !String.IsNullOrWhiteSpace(x.codes.gnd.value)))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.GndCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong GND id", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetGnd(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                    // TODO: Sources
                }
            }
        }

        private void SetWoeid(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => !String.IsNullOrWhiteSpace(x.codes.woeid.value)))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.WoeidCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong WOEID", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetWoeid(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }

                        var snak = new Snak(SnakType.Value, new EntityId(WikiBase.PropertyIdStatedIn), new EntityIdValue(new EntityId(WikiBase.ItemFlickrShapeFile)));
                        var woeidReference = statement.CreateReferenceForSnak(snak);
                        statement.AddReference(woeidReference);
                        foreach (var reference in statement.References)
                        {
                            reference.Save(_helper.GetReferenceSaveEditSummary(reference));
                        }
                    }
                }
            }
        }

        private void SetHASC(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => !String.IsNullOrWhiteSpace(x.codes.hasc.value)))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.HASCCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong HASC id", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetHASC(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                    // TODO: Sources
                }
            }
        }

        private void SetGadm(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => !String.IsNullOrWhiteSpace(x.codes.gadm.value)))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.GadmCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong GADM", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetGadm(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                }
            }
        }

        private void SetGeonames(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => !String.IsNullOrWhiteSpace(x.codes.geonames.value)))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.GeonamesCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong geonames", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetGeonames(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                }
            }
        }

        private void SetGNSUFI(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => !String.IsNullOrWhiteSpace(x.codes.gnsufi.value)))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.GNSUFICorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong GNS-UFI id", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetGNSUFI(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                    // TODO: Sources
                }
            }
        }

        private void SetFacebookPlaceId(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => x.office.Any(y => y.socialweb.facebook.Any(z => z.type == FacebookPageType.place))))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.FacebookPlaceIdCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong facebook place id", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetFacebookPlaceId(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                    // TODO: Sources
                }
            }
        }

        private void SetOfficialWebsite(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => x.office.Any(y => !String.IsNullOrEmpty(y.PreferredWebsite))))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.OfficialWebsiteCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong official website", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetOfficialWebsite(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                        _helper.AddWebsiteQualifiers(statement);
                        foreach (var qualifier in statement.Qualifiers)
                        {
                            qualifier.Save(_helper.GetQualifierSaveEditSummary(qualifier));
                        }
                    }

                }
            }
        }

        private void SetDescribedByUrl(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => x.type.IsCompatibleEntityType(EntityType.Tambon) || x.type.IsCompatibleEntityType(EntityType.Amphoe)))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.DescribedByUrlCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong described by URL", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetDescribedByUrl(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }

                        if (!statement.Qualifiers.Any(x => x.PropertyId.PrefixedId.Equals(WikiBase.PropertyIdLanguageOfWork, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            _helper.AddLanguageOfWorkQualifier(statement);
                            foreach (var qualifier in statement.Qualifiers)
                            {
                                qualifier.Save(_helper.GetQualifierSaveEditSummary(qualifier));
                            }
                        }
                    }
                    // TODO: Sources
                }
            }
        }

        private void SetNamedAfterSubdivision(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => x.NamedAfterEntity() != null))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.NamedAfterSubdivisionCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong named after subdivision", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetNamedAfterSubdivision(item, entity, overrideData);
                        if (statement != null)
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
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities)
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.IsInAdministrativeUnitCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong parent", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetIsInAdministrativeUnit(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                }
            }
        }

        private void SetTypeOfAdministrativeUnit(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => x.type != EntityType.Thesaban))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.TypeOfAdministrativeUnitCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong type", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetTypeOfAdministrativeUnit(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }

                        if (statement != null)
                        {
                            if (_helper.AddTypeOfAdministrativeQualifiersAndReferences(statement, entity.type, entity))
                            {
                                foreach (var qualifier in statement.Qualifiers)
                                {
                                    qualifier.Save(_helper.GetQualifierSaveEditSummary(qualifier));
                                }
                                foreach (var reference in statement.References)
                                {
                                    reference.Save(_helper.GetReferenceSaveEditSummary(reference));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetLocatorMap(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => x.type.IsCompatibleEntityType(EntityType.Amphoe)))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.LocatorMapCorrect(item, entity);
                    _runInfo[state]++;
                    if (state == WikiDataState.WrongValue)
                    {
                        collisionInfo.AppendFormat("{0}: {1} has wrong type", item.id, entity.english);
                        collisionInfo.AppendLine();
                    }
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetLocatorMap(item, entity, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                }
            }
        }

        private void SetContainsSubdivisions(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities)
            {
                if (entity.entity.Where(x => !x.IsObsolete && !x.type.IsLocalGovernment()).All(x => x.wiki != null && !String.IsNullOrEmpty(x.wiki.wikidata)))
                {
                    var item = _helper.GetWikiDataItemForEntity(entity);
                    if (item == null)
                    {
                        _runInfo[WikiDataState.ItemNotFound]++;
                        collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                    }
                    else
                    {
                        foreach (var subEntity in entity.entity.Where(x => !x.type.IsLocalGovernment()))
                        {
                            var state = _helper.ContainsSubdivisionsCorrect(item, entity, subEntity);
                            _runInfo[state]++;
                            if (state == WikiDataState.Incomplete)
                            {
                                var statement = _helper.SetContainsSubdivisions(item, entity, subEntity);
                                if (statement != null)
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
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => !x.IsObsolete && x.codes != null && x.codes.post != null && x.codes.post.value.Any()))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    foreach (var code in entity.codes.post.value)
                    {
                        var state = _helper.PostalCodeCorrect(item, entity, code);
                        _runInfo[state]++;
                        if (state == WikiDataState.Incomplete)
                        {
                            var statement = _helper.SetPostalCode(item, entity, code);
                            if (statement != null)
                            {
                                statement.save(_helper.GetClaimSaveEditSummary(statement));
                            }
                        }
                    }
                }
            }
        }

        private void SetShareBorderWith(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities.Where(x => !x.IsObsolete && x.area.bounding.Any()))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var allEntities = GlobalData.CompleteGeocodeList().FlatList();
                    foreach (var bounding in entity.area.bounding.Where(x => x.type == BoundaryType.land))
                    {
                        var boundingEntity = allEntities.FirstOrDefault(x => x.geocode == bounding.geocode);
                        if ((boundingEntity != null) && (boundingEntity.wiki != null) && (!String.IsNullOrEmpty(boundingEntity.wiki.wikidata)))
                        {
                            var state = _helper.BoundingEntityCorrect(item, entity, boundingEntity);
                            _runInfo[state]++;
                            if (state == WikiDataState.Incomplete)
                            {
                                var statement = _helper.SetBoundingEntity(item, entity, boundingEntity);
                                if (statement != null)
                                {
                                    statement.save(_helper.GetClaimSaveEditSummary(statement));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetPopulationData(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData, PopulationDataSourceType dataSource, Int16 year)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            GlobalData.LoadPopulationData(dataSource, year);
            // TODO - local governments are not calculated!
            foreach (var entity in entities.Where(x => x.population.Any(y => y.source == dataSource && y.Year == year)))
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var data = entity.population.First(y => y.source == dataSource && y.Year == year);

                    var state = _helper.PopulationDataCorrect(item, data);
                    _runInfo[state]++;
                    if (state != WikiDataState.Valid)
                    {
                        var statement = _helper.SetPopulationData(item, data, overrideData);
                        if (statement != null)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                            _helper.AddPopulationDataReferences(statement, data, entity);
                            foreach (var reference in statement.References)
                            {
                                reference.Save(_helper.GetReferenceSaveEditSummary(reference));
                            }

                            _helper.AddPopulationDataQualifiers(statement, data);
                            foreach (var qualifier in statement.Qualifiers)
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
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities)
            {
                Boolean hasValue = false;
                var office = entity.office.FirstOrDefault();
                if (office != null)
                {
                    hasValue = office.Point != null;
                }
                if (hasValue)
                {
                    var item = _helper.GetWikiDataItemForEntity(entity);
                    if (item == null)
                    {
                        _runInfo[WikiDataState.ItemNotFound]++;
                        collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                    }
                    else
                    {
                        var state = _helper.LocationCorrect(item, entity);
                        _runInfo[state]++;
                        if (state == WikiDataState.WrongValue)
                        {
                            var expected = new GeoCoordinate(Convert.ToDouble(office.Point.lat), Convert.ToDouble(office.Point.@long));
                            var actual = _helper.GetCoordinateValue(item, WikiBase.PropertyIdCoordinate);
                            var distance = expected.GetDistanceTo(actual);
                            collisionInfo.AppendFormat("{0}: {1} has wrong location, off by {2:0.###}km", item.id, entity.english, distance / 1000.0);
                            collisionInfo.AppendLine();
                        }
                        if (state != WikiDataState.Valid)
                        {
                            var statement = _helper.SetLocation(item, entity, overrideData);
                            if (statement != null)
                            {
                                statement.save(_helper.GetClaimSaveEditSummary(statement));
                            }
                            _helper.AddLocationQualifiers(statement, entity);
                            foreach (var qualifier in statement.Qualifiers)
                            {
                                qualifier.Save(_helper.GetQualifierSaveEditSummary(qualifier));
                            }

                        }
                    }
                }
            }
        }

        private void SetInception(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities)
            {
                var history = entity.history.Items.FirstOrDefault(x => x is HistoryCreate) as HistoryCreate;
                var hasValue = history != null && history.effectiveSpecified;
                if (hasValue)
                {
                    var item = _helper.GetWikiDataItemForEntity(entity);
                    if (item == null)
                    {
                        _runInfo[WikiDataState.ItemNotFound]++;
                        collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                    }
                    else
                    {
                        var state = _helper.InceptionCorrect(item, entity);
                        _runInfo[state]++;
                        if (state == WikiDataState.WrongValue)
                        {
                            collisionInfo.AppendFormat("{0}: {1} has wrong inception date", item.id, entity.english);
                            collisionInfo.AppendLine();
                        }
                        if (state != WikiDataState.Valid)
                        {
                            var statement = _helper.SetInception(item, entity, overrideData);
                            if (statement != null)
                            {
                                statement.save(_helper.GetClaimSaveEditSummary(statement));
                            }

                            var gazetteReference = history.Items.OfType<GazetteRelated>().FirstOrDefault();
                            if (gazetteReference != null)
                            {
                                var gazette = GlobalData.AllGazetteAnnouncements.FindAnnouncement(gazetteReference);
                                if (gazette != null)
                                {
                                    var snak = new Snak(SnakType.Value, new EntityId(WikiBase.PropertyIdReferenceUrl), new StringValue(gazette.DownloadUrl.AbsoluteUri));
                                    var urlReference = statement.CreateReferenceForSnak(snak);
                                    statement.AddReference(urlReference);
                                    foreach (var reference in statement.References)
                                    {
                                        reference.Save(_helper.GetReferenceSaveEditSummary(reference));
                                    }
                                }
                            }
                        }

                        // TODO: Reference!
                    }
                }
            }
        }

        private void CleanupPopulationData(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData)
        {
            _ = entities ?? throw new ArgumentNullException(nameof(entities));

            ClearRunInfo();
            foreach (var entity in entities)
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (item == null)
                {
                    _runInfo[WikiDataState.ItemNotFound]++;
                    collisionInfo.AppendFormat("{0}: {1} was deleted!", entity.wiki.wikidata, entity.english);
                }
                else
                {
                    var state = _helper.CheckPopulationData(item);
                    _runInfo[state]++;
                    if (state != WikiDataState.Valid)
                    {
                        var statements = _helper.CleanupPopulationData(item);
                        foreach (var statement in statements)
                        {
                            statement.save(_helper.GetClaimSaveEditSummary(statement));
                        }
                    }
                }
            }
        }

        #endregion private methods
    }
}