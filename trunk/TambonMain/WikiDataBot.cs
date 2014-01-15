using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wikibase;

namespace De.AHoerstemeier.Tambon
{
    public delegate void WikiDataTaskDelegate(IEnumerable<Entity> entities, StringBuilder collisionInfo, Boolean overrideData);

    public class WikiDataTaskInfo
    {
        public String Description
        {
            get;
            private set;
        }

        public WikiDataTaskDelegate Task
        {
            get;
            private set;
        }

        public WikiDataTaskInfo(String description, WikiDataTaskDelegate task)
        {
            Description = description;
            Task = task;
        }

        public override string ToString()
        {
            return Description;
        }
    }

    public class WikiDataBot
    {
        private Dictionary<Language, String> _languageCode;

        private List<WikiDataTaskInfo> _availableTasks;

        public IEnumerable<WikiDataTaskInfo> AvailableTasks
        {
            get
            {
                return _availableTasks;
            }
        }

        private WikiDataHelper _helper;

        private Dictionary<WikiDataState, Int32> _runInfo;

        public Dictionary<WikiDataState, Int32> RunInfo
        {
            get
            {
                return _runInfo;
            }
        }

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
            _availableTasks.Add(new WikiDataTaskInfo("Set description [en]", setDescriptionEnglish));
            _availableTasks.Add(new WikiDataTaskInfo("Set description [th]", setDescriptionThai));
            _availableTasks.Add(new WikiDataTaskInfo("Set country", SetCountry));
            _availableTasks.Add(new WikiDataTaskInfo("Set is in administrative unit", SetIsInAdministrativeUnit));

            _languageCode = new Dictionary<Language, String>()
            {
                {Language.English,"en"},
                {Language.German,"de"},
                {Language.Thai,"th"},
            };
        }

        public void LogOut()
        {
            _helper.Api.logout();
        }

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
                var oldDescription = item.getDescription(languageCode);
                var newDescription = entity.GetWikiDataDescription(language);

                if ( String.IsNullOrEmpty(oldDescription) )
                {
                    item.setDescription(languageCode, newDescription);
                    item.save(String.Format("Added description [{0}]: {1}", languageCode, newDescription));
                }
                else if ( oldDescription != newDescription )
                {
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

        public Dictionary<String, Int32> CountSiteLinks(IEnumerable<Entity> entities)
        {
            var result = new Dictionary<String, Int32>();
            // Create a new EntityProvider instance and pass the api created above.
            EntityProvider entityProvider = new EntityProvider(_helper.Api);
            foreach ( var entity in entities )
            {
                // Get an entity by searching for the id
                var entityById = entityProvider.getEntityFromId(EntityId.newFromPrefixedId(entity.wiki.wikidata));
                var links = (entityById as Item).getSitelinks();
                foreach ( var key in links.Keys )
                {
                    if ( !result.ContainsKey(key) )
                        result[key] = 0;
                    result[key]++;
                }
            }
            return result;
        }
    }
}