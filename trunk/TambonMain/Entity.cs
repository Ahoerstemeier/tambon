using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.AHoerstemeier.Tambon
{
    public partial class Entity
    {
        public Entity Clone()
        {
            // Don't I need a deep value copy?
            var newEntity = (Entity)(this.MemberwiseClone());
            newEntity.entity = new List<Entity>();
            foreach ( var subEntity in this.entity )
            {
                newEntity.entity.Add(subEntity.Clone());
            }
            return newEntity;
        }

        private ICollection<Entity> _thesaban = new List<Entity>();

        /// <summary>
        /// Gets whether the entity at the given geocode is active/valid.
        /// </summary>
        public Boolean IsObsolete
        {
            get
            {
                return obsolete | !String.IsNullOrEmpty(newgeocode);
            }
        }

        public IEnumerable<Entity> Thesaban
        {
            get
            {
                return _thesaban;
            }
        }

        public void ReorderThesaban()
        {
            foreach ( var subEntity in entity )
            {
                if ( subEntity != null )
                {
                    if ( subEntity.type.IsThesaban() | subEntity.type.IsSakha() )
                    {
                        _thesaban.Add(subEntity);
                    }
                }
            }
            foreach ( var thesaban in _thesaban )
            {
                entity.Remove(thesaban);
            }
            foreach ( var thesaban in _thesaban )
            {
                if ( thesaban.entity.Any() )
                {
                    foreach ( var tambon in thesaban.entity )
                    {
                        AddTambonInThesabanToAmphoe(tambon, thesaban);
                    }
                }
            }
            foreach ( var subEntity in entity )
            {
                if ( subEntity != null )
                {
                    subEntity.entity.Sort((x, y) => x.geocode.CompareTo(y.geocode));
                }
            }
        }

        internal void AddTambonInThesabanToAmphoe(Entity tambon, Entity thesaban)
        {
            var allSubEntities = entity.SelectMany(x => x.entity);
            var mainTambon = allSubEntities.FirstOrDefault(x => (x.geocode == tambon.geocode) & (x.type == EntityType.Tambon));
            var mainAmphoe = allSubEntities.FirstOrDefault(x => (x.geocode == tambon.geocode / 100));
            if ( mainTambon == null )
            {
                if ( mainAmphoe != null )
                {
                    mainTambon = (Entity)tambon.MemberwiseClone();
                    mainAmphoe.entity.Add(mainTambon);
                }
            }
            else
            {
                mainTambon.population.First().data.AddRange(tambon.population.First().data);
            }
            if ( mainAmphoe != null )
            {
                mainAmphoe.population.First().data.AddRange(tambon.population.First().data);
            }
            if ( mainTambon != null )
            {
                var newEntity = new Entity()
                {
                    geocode = thesaban.geocode,
                    name = thesaban.name,
                    english = thesaban.english,
                    type = thesaban.type
                };
                var populationData = mainTambon.population.First();
                newEntity.population.Add(populationData.Clone());
                mainTambon.entity.Add(newEntity);
            }
        }

        internal void ParseName(String value)
        {
            if ( !String.IsNullOrEmpty(value) )
            {
                foreach ( var abbreviation in ThaiTranslations.EntityAbbreviations )
                {
                    value = value.Replace(abbreviation.Value + ".", ThaiTranslations.EntityNamesThai[abbreviation.Key]);  // especially the ThesabanTambon occurs sometimes
                }
                EntityType entityType = EntityType.Unknown;
                foreach ( var entityTypeName in ThaiTranslations.EntityNamesThai )
                {
                    if ( value.StartsWith(entityTypeName.Value) )
                    {
                        entityType = entityTypeName.Key;
                    }
                }
                if ( (entityType == EntityType.Unknown) | (entityType == EntityType.Bangkok) )
                {
                    name = value;
                }
                else
                {
                    name = value.Replace(ThaiTranslations.EntityNamesThai[entityType], "");
                }
                if ( entityType.IsSakha() )
                {
                    // Some pages have the syntax "Name AmphoeName" with the word อำเภอ, others without
                    //Int32 pos = Name.IndexOf(Helper.EntityNames[EntityType.Amphoe]);
                    //if (pos > 0)
                    //{
                    //    mName = mName.Remove(pos - 1);
                    //}
                    Int32 pos = name.IndexOf(" ");
                    if ( pos > 0 )
                    {
                        name = name.Remove(pos);
                    }
                }
                obsolete = name.Contains("*");
                name = name.Replace("*", "");
                if ( name.StartsWith(".") )
                {
                    // Mistake in DOPA population statistic for Buriram 2005, a leading "."
                    name = name.Substring(1, name.Length - 1);
                }
                name = name.Trim();
                type = entityType;
            }
        }

        public IEnumerable<Entity> InvalidGeocodeEntries()
        {
            var result = new List<Entity>();

            foreach ( var subEntity in entity )
            {
                if ( !GeocodeHelper.IsBaseGeocode(this.geocode, subEntity.geocode) )
                {
                    result.Add(subEntity);
                }

                Int32 entitiesWithSameCode = 0;
                foreach ( var subEntityForCount in entity )
                {
                    if ( subEntityForCount.geocode == subEntity.geocode )
                    {
                        entitiesWithSameCode++;
                    }
                }
                if ( entitiesWithSameCode > 1 )
                {
                    result.Add(subEntity);
                }

                result.AddRange(subEntity.InvalidGeocodeEntries());
            }
            return result;
        }

        internal void CopyBasicDataFrom(Entity source)
        {
            geocode = source.geocode;
            english = source.english;
            name = source.name;
        }

        internal IEnumerable<String> OldNames
        {
            get
            {
                var result = new List<String>();
                foreach ( var item in history.Items )
                {
                    var itemRename = item as HistoryRename;
                    if ( itemRename != null )
                    {
                        result.Add(itemRename.oldname);
                    }
                }
                return result;
            }
        }

        private Boolean SameNameAndType(String findName, EntityType findType)
        {
            return (name == findName) & (type.IsCompatibleEntityType(findType));
        }

        private Entity FindByNameAndType(String findName, EntityType findType, Boolean allowOldNames)
        {
            Entity result = null;
            result = FindByNameAndType(findName, findType, allowOldNames, false, 0);
            if ( result == null )
            {
                result = FindByNameAndType(findName, findType, allowOldNames, true, 0);
            }
            return result;
        }

        private Entity FindByNameAndType(String findName, EntityType findType, Boolean allowOldNames, Boolean allowObsolete, Int32 startPosition)
        {
            Entity retval = null;
            this.entity.Sort((x, y) => x.geocode.CompareTo(y.geocode));

            foreach ( var subEntity in entity )
            {
                if ( subEntity.SameNameAndType(findName, findType) )
                {
                    if ( (!subEntity.obsolete) | allowObsolete )
                    {
                        startPosition--;
                        if ( startPosition < 0 )
                        {
                            retval = subEntity;
                            break;
                        }
                    }
                }
                if ( allowOldNames & (subEntity.OldNames.Contains(findName)) & (subEntity.type.IsCompatibleEntityType(findType)) )
                {
                    if ( (!subEntity.obsolete) | allowObsolete )
                    {
                        startPosition--;
                        if ( startPosition < 0 )
                        {
                            retval = subEntity;
                            break;
                        }
                    }
                }
            }
            return retval;
        }

        internal void SynchronizeGeocodes(Entity geocodeSource)
        {
            var missedEntities = new List<Entity>();

            if ( geocodeSource != null )
            {
                // this == geocodeSource => copy directly from source
                if ( ((name == geocodeSource.name) | (geocodeSource.OldNames.Contains(name))) & (type.IsCompatibleEntityType(geocodeSource.type)) )
                {
                    CopyBasicDataFrom(geocodeSource);
                }

                foreach ( var subEntity in entity )
                {
                    // find number of sub entities with same name and type
                    Int32 position = 0;
                    if ( subEntity.type != EntityType.Muban )
                    {
                        foreach ( var find in entity )
                        {
                            if ( find == subEntity )
                            {
                                break;
                            }
                            if ( find.SameNameAndType(subEntity.name, subEntity.type) )
                            {
                                position++;
                            }
                        }
                    }

                    Entity sourceEntity = null;
                    if ( subEntity.type == EntityType.Muban )
                    {
                        sourceEntity = sourceEntity.entity.FirstOrDefault(x => x.geocode == subEntity.geocode);
                    }
                    else
                    {
                        sourceEntity = geocodeSource.FindByNameAndType(subEntity.name, subEntity.type, false, false, position);
                        if ( sourceEntity == null )
                        {
                            sourceEntity = geocodeSource.FindByNameAndType(subEntity.name, subEntity.type, true, false, position);
                        }
                    }
                    if ( sourceEntity != null )
                    {
                        subEntity.SynchronizeGeocodes(sourceEntity);
                    }
                    else
                    {
                        // Problem!
                    }

                    if ( subEntity.type.IsThesaban() | subEntity.type.IsSakha() )
                    {
                        foreach ( UInt32 parentCode in subEntity.parent )
                        {
                            Entity parentEntity = geocodeSource.entity.FirstOrDefault(x => x.geocode == parentCode);
                            if ( parentEntity != null )
                            {
                                subEntity.SynchronizeGeocodes(parentEntity);
                                Entity sourceValue = this.entity.FirstOrDefault(x => x.geocode == parentCode);
                                if ( sourceValue == null )
                                {
                                    var newEntry = (Entity)parentEntity.MemberwiseClone();
                                    newEntry.entity.Clear();
                                    Boolean found = false;
                                    foreach ( var compare in missedEntities )
                                    {
                                        found = found | (compare.geocode == newEntry.geocode);
                                    }
                                    if ( !found )
                                    {
                                        missedEntities.Add(newEntry);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach ( var newEntry in missedEntities )
            {
                var parent = this.entity.FirstOrDefault(x => x.geocode == newEntry.geocode / 100);
                if ( parent != null )
                {
                    parent.entity.Add(newEntry);
                }
                parent.entity.Sort((x, y) => x.geocode.CompareTo(y.geocode));
            }
        }

        internal void ConsolidatePopulationData()
        {
            foreach ( var data in population )
            {
                data.MergeIdenticalEntries();
            }
        }

        /// <summary>
        /// Gets the display name of the entity.
        /// </summary>
        /// <returns>The display name of the entity.</returns>
        /// <remarks>Always returns <see cref="Entity.english"/>.</remarks>
        public override string ToString()
        {
            return english;
        }

        public IEnumerable<Entity> FlatList()
        {
            var result = new List<Entity>();
            result.Add(this);
            foreach ( var subEntity in entity )
            {
                result.AddRange(subEntity.FlatList());
            }
            return result;
        }

        private IEnumerable<OfficeType> _officesWithElectedOfficials = new List<OfficeType>() { OfficeType.MunicipalityOffice, OfficeType.PAOOffice, OfficeType.TAOOffice };

        private IEnumerable<EntityTermEnd> OfficialElectionsPending()
        {
            var result = new List<EntityTermEnd>();
            foreach ( var officeEntry in office )
            {
                if ( (!officeEntry.obsolete) && _officesWithElectedOfficials.Contains(officeEntry.type) )
                {
                    officeEntry.officials.Items.Sort((x, y) => x.begin.CompareTo(y.begin));
                    var term = officeEntry.officials.Items.LastOrDefault();
                    if ( term != null )
                    // foreach ( var term in office.council )
                    {
                        DateTime termEnd;
                        if ( term.endSpecified )
                        {
                            termEnd = term.end;
                        }
                        else
                        {
                            termEnd = term.begin.AddYears(4).AddDays(-1);
                        }
                        if ( (termEnd.CompareTo(DateTime.Now) <= 0) )
                        {
                            result.Add(new EntityTermEnd(this, null, term));
                        }
                    }
                    else
                    {
                        // no Official list at all, but there should be one...
                        result.Add(new EntityTermEnd(this, null, new OfficialEntryUnnamed()));
                    }
                }
            }
            return result;
        }

        private IEnumerable<EntityTermEnd> CouncilElectionsPending()
        {
            var result = new List<EntityTermEnd>();
            foreach ( var officeEntry in office )
            {
                if ( !officeEntry.obsolete )
                {
                    officeEntry.council.Sort((x, y) => x.begin.CompareTo(y.begin));
                    var term = officeEntry.council.LastOrDefault();
                    if ( term != null )
                    // foreach ( var term in office.council )
                    {
                        DateTime termEnd;
                        if ( term.endSpecified )
                        {
                            termEnd = term.end;
                        }
                        else
                        {
                            termEnd = term.begin.AddYears(4).AddDays(-1);
                        }
                        if ( (termEnd.CompareTo(DateTime.Now) <= 0) )
                        {
                            result.Add(new EntityTermEnd(this, term, null));
                        }
                    }
                }
            }
            return result;
        }

        private IEnumerable<EntityTermEnd> OfficialTermsEndInTimeSpan(DateTime begin, DateTime end)
        {
            var result = new List<EntityTermEnd>();
            foreach ( var officeEntry in office )
            {
                officeEntry.officials.Items.Sort((x, y) => x.begin.CompareTo(y.begin));
                var term = officeEntry.officials.Items.LastOrDefault();
                if ( term != null )
                // foreach ( var term in office.council )
                {
                    DateTime termEnd;
                    if ( term.endSpecified )
                    {
                        termEnd = term.end;
                    }
                    else
                    {
                        termEnd = term.begin.AddYears(4).AddDays(-1);
                    }
                    if ( (termEnd.CompareTo(begin) >= 0) & (termEnd.CompareTo(end) <= 0) )
                    {
                        result.Add(new EntityTermEnd(this, null, term));
                    }
                }
            }
            return result;
        }

        private IEnumerable<EntityTermEnd> CouncilTermsEndInTimeSpan(DateTime begin, DateTime end)
        {
            var result = new List<EntityTermEnd>();
            foreach ( var officeEntry in office )
            {
                officeEntry.council.Sort((x, y) => x.begin.CompareTo(y.begin));
                var term = officeEntry.council.LastOrDefault();
                if ( term != null )
                // foreach ( var term in office.council )
                {
                    DateTime termEnd;
                    if ( term.endSpecified )
                    {
                        termEnd = term.end;
                    }
                    else
                    {
                        termEnd = term.begin.AddYears(4).AddDays(-1);
                    }
                    if ( (termEnd.CompareTo(begin) >= 0) & (termEnd.CompareTo(end) <= 0) )
                    {
                        result.Add(new EntityTermEnd(this, term, null));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Calculates list of entities which have a council ending their term within the given timespan.
        /// </summary>
        /// <param name="begin">Begin of timespan.</param>
        /// <param name="end">End of timespan.</param>
        /// <returns>List of entities with council term ends.</returns>
        public IEnumerable<EntityTermEnd> EntitiesWithCouncilTermEndInTimeSpan(DateTime begin, DateTime end)
        {
            var result = new List<EntityTermEnd>();
            foreach ( var item in FlatList() )
            {
                result.AddRange(item.CouncilTermsEndInTimeSpan(begin, end));
            }

            return result;
        }

        /// <summary>
        /// Calculates list of entities which have a council ending their term within the given <paramref name="year"/>.
        /// </summary>
        /// <param name="year">Year to look for.</param>
        /// <returns>List of entities with council term ends.</returns>
        public IEnumerable<EntityTermEnd> EntitiesWithCouncilTermEndInYear(Int32 year)
        {
            return EntitiesWithCouncilTermEndInTimeSpan(new DateTime(year, 1, 1), new DateTime(year, 12, 31));
        }

        /// <summary>
        /// Calculates list of entities which have a official ending their term within the given timespan.
        /// </summary>
        /// <param name="begin">Begin of timespan.</param>
        /// <param name="end">End of timespan.</param>
        /// <returns>List of entities with official term ends.</returns>
        public IEnumerable<EntityTermEnd> EntitiesWithOfficialTermEndInTimeSpan(DateTime begin, DateTime end)
        {
            var result = new List<EntityTermEnd>();
            foreach ( var item in FlatList() )
            {
                result.AddRange(item.OfficialTermsEndInTimeSpan(begin, end));
            }

            return result;
        }

        /// <summary>
        /// Calculates list of entities which have a official ending their term within the given <paramref name="year"/>.
        /// </summary>
        /// <param name="year">Year to look for.</param>
        /// <returns>List of entities with official term ends.</returns>
        public IEnumerable<EntityTermEnd> EntitiesWithOfficialTermEndInYear(Int32 year)
        {
            return EntitiesWithOfficialTermEndInTimeSpan(new DateTime(year, 1, 1), new DateTime(year, 12, 31));
        }

        /// <summary>
        /// Calculates list of entities which have no elected council currently.
        /// </summary>
        /// <returns>List of entities without elected council.</returns>
        public IEnumerable<EntityTermEnd> EntitiesWithCouncilElectionPending()
        {
            var result = new List<EntityTermEnd>();
            foreach ( var item in FlatList() )
            {
                result.AddRange(item.CouncilElectionsPending());
            }

            return result;
        }

        /// <summary>
        /// Calculates list of entities which have no elected official currently.
        /// </summary>
        /// <returns>List of entities without elected official.</returns>
        public IEnumerable<EntityTermEnd> EntitiesWithOfficialElectionPending()
        {
            var result = new List<EntityTermEnd>();
            foreach ( var item in FlatList() )
            {
                result.AddRange(item.OfficialElectionsPending());
            }

            return result;
        }

        /// <summary>
        /// Gets the description ready to be set to WikiData.
        /// </summary>
        /// <param name="language">Language.</param>
        /// <returns>Description of the entity.</returns>
        public String GetWikiDataDescription(Language language)
        {
            var allEntities = GlobalData.CompleteGeocodeList().FlatList();
            var typeValue = this.type.Translate(language);
            var expanded = String.Empty;  // 0 = type, 1 = hierarchy
            var expandedTopLevel = String.Empty;  // 0 = type
            var hierarchy = String.Empty;
            var hierarchyExpand = String.Empty;  // 0 = name, 1 = type
            switch ( language )
            {
                case Language.English:
                    expanded = "{0} in {1}Thailand";
                    expandedTopLevel = "{0} of Thailand";
                    hierarchyExpand = "{0} {1}, ";
                    break;
                case Language.German:
                    expanded = "{0} in {1}Thailand";
                    expandedTopLevel = "{0} in Thailand";
                    hierarchyExpand = "{1} {0}, ";
                    break;
                case Language.Thai:
                    expanded = "{0}ใน{1}ประเทศไทย";
                    expandedTopLevel = "{0}ในประเทศไทย";
                    hierarchyExpand = "{1}{0} ";
                    break;
            }
            var currentGeocode = geocode;
            while ( currentGeocode / 100 != 0 )
            {
                currentGeocode = currentGeocode / 100;
                var parentEntity = allEntities.First(x => x.geocode == currentGeocode);
                var parentType = parentEntity.type.Translate(language);
                if ( language == Language.Thai )
                    hierarchy += String.Format(hierarchyExpand, parentEntity.name, parentType);
                else
                    hierarchy += String.Format(hierarchyExpand, parentEntity.english, parentType);
            }
            return String.Format(expanded, typeValue, hierarchy);
        }

        /// <summary>
        /// Gets the abbreviated Thai name.
        /// </summary>
        /// <value>The abbreviated Thai name.</value>
        /// <remarks>Abbreviated name consists of the abbreviation of the administrative type followed by the name.</remarks>
        public String AbbreviatedName
        {
            get
            {
                return ThaiTranslations.EntityAbbreviations[type] + "." + name;
            }
        }

        /// <summary>
        /// Gets the full Thai name.
        /// </summary>
        /// <value>The full Thai name.</value>
        /// <remarks>Full name consists of the administrative type followed by the name.</remarks>
        public String FullName
        {
            get
            {
                return ThaiTranslations.EntityNamesThai[type] + name;
            }
        }
    }
}