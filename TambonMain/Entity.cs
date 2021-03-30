﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.AHoerstemeier.Tambon
{
    public partial class Entity
    {
        #region fixup serialization

        /// <summary>
        /// Checks whether <see cref="history"/> needs to be serialized.
        /// </summary>
        /// <returns><c>true</c> if history needs to be serialized, <c>false</c> otherwise.</returns>
        public Boolean ShouldSerializehistory()
        {
            return history.Items.Any();
        }

        /// <summary>
        /// Checks whether <see cref="area"/> needs to be serialized.
        /// </summary>
        /// <returns><c>true</c> if area needs to be serialized, <c>false</c> otherwise.</returns>
        public Boolean ShouldSerializearea()
        {
            return area.area.Any() || area.bounding.Any();
        }

        /// <summary>
        /// Checks whether <see cref="newgeocode"/> needs to be serialized.
        /// </summary>
        /// <returns><c>true</c> if new geocode list needs to be serialized, <c>false</c> otherwise.</returns>
        public Boolean ShouldSerializenewgeocode()
        {
            return newgeocode.Any();
        }

        /// <summary>
        /// Checks whether <see cref="parent"/> needs to be serialized.
        /// </summary>
        /// <returns><c>true</c> if parent list needs to be serialized, <c>false</c> otherwise.</returns>
        public Boolean ShouldSerializeparent()
        {
            return parent.Any();
        }

        /// <summary>
        /// Checks whether <see cref="entitycount"/> needs to be serialized.
        /// </summary>
        /// <returns><c>true</c> if entity count needs to be serialized, <c>false</c> otherwise.</returns>
        public Boolean ShouldSerializeentitycount()
        {
            return entitycount.Any();
        }

        /// <summary>
        /// Checks whether <see cref="codes"/> needs to be serialized.
        /// </summary>
        /// <returns><c>true</c> if codes needs to be serialized, <c>false</c> otherwise.</returns>
        public Boolean ShouldSerializecodes()
        {
            return !codes.IsEmpty();
        }

        /// <summary>
        /// Checks whether <see cref="symbols"/> needs to be serialized.
        /// </summary>
        /// <returns><c>true</c> if symbols needs to be serialized, <c>false</c> otherwise.</returns>
        public Boolean ShouldSerializesymbols()
        {
            return !symbols.IsEmpty();
        }

        /// <summary>
        /// Checks whether <see cref="muban"/> needs to be serialized.
        /// </summary>
        /// <returns><c>true</c> if muban needs to be serialized, <c>false</c> otherwise.</returns>
        public Boolean ShouldSerializemuban()
        {
            return (muban != null) && muban.Any();
        }

        #endregion fixup serialization

        /// <summary>
        /// Creates a new instance copying all the contents.
        /// </summary>
        /// <returns>New copied instance.</returns>
        public Entity Clone()
        {
            // Don't I need a deep value copy?
            var newEntity = (Entity)(this.MemberwiseClone());
            newEntity.entity = new List<Entity>();
            foreach (var subEntity in this.entity)
            {
                newEntity.entity.Add(subEntity.Clone());
            }
            return newEntity;
        }

        /// <summary>
        /// List of municipalities within the entity. Only used for <see cref="EntityType.Changwat"/>.
        /// </summary>
        private readonly ICollection<Entity> _thesaban = new List<Entity>();

        /// <summary>
        /// List of previous geocodes. <c>null</c> if not yet calculated.
        /// </summary>
        private List<UInt32> _oldGeocode = null;

        /// <summary>
        /// Gets whether the entity at the given geocode is active/valid.
        /// </summary>
        /// <value><c>true</c> if entity is obsolete, <c>false</c> if it is valid.</value>
        public Boolean IsObsolete
        {
            get
            {
                return obsolete | newgeocode.Any();
            }
        }

        /// <summary>
        /// Gets the municipalities.
        /// </summary>
        /// <value>The municipalities.</value>
        public IEnumerable<Entity> Thesaban
        {
            get
            {
                return _thesaban;
            }
        }

        public void ReorderThesaban()
        {
            foreach (var subEntity in entity)
            {
                if (subEntity != null)
                {
                    if (subEntity.type.IsLocalGovernment() | subEntity.type.IsSakha())
                    {
                        _thesaban.Add(subEntity);
                    }
                }
            }
            foreach (var thesaban in _thesaban)
            {
                entity.Remove(thesaban);
            }

            // set the population data type of the non-municipal items
            PopulationDataType nonThesabanType = PopulationDataType.total;
            if (_thesaban.Any())
            {
                nonThesabanType = PopulationDataType.nonmunicipal;
            }
            foreach (var amphoe in entity)
            {
                foreach (var entry in amphoe.FlatList())
                {
                    if (entry.population.Any())
                    {
                        var data = entry.population.First().data.FirstOrDefault();
                        if (data != null)
                        {
                            data.type = nonThesabanType;
                        }
                    }
                }
            }

            foreach (var thesaban in _thesaban)
            {
                if (thesaban.entity.Any())
                {
                    foreach (var tambon in thesaban.entity)
                    {
                        var data = tambon.population.First().data.First();
                        data.type = PopulationDataType.municipal;
                        data.geocode.Add(thesaban.geocode);
                        AddTambonInThesabanToAmphoe(tambon, thesaban);
                    }
                }
            }
            foreach (var subEntity in entity)
            {
                if (subEntity != null)
                {
                    subEntity.entity.Sort((x, y) => x.geocode.CompareTo(y.geocode));
                }
            }
        }

        internal void AddTambonInThesabanToAmphoe(Entity tambon, Entity thesaban)
        {
            var allSubEntities = entity.SelectMany(x => x.entity).ToList();
            var mainTambon = allSubEntities.SingleOrDefault(x => (GeocodeHelper.IsSameGeocode(x.geocode, tambon.geocode, false)) & (x.type == tambon.type));
            var mainAmphoe = entity.FirstOrDefault(x => (x.geocode == tambon.geocode / 100));
            if (mainTambon == null)
            {
                if (mainAmphoe != null)
                {
                    mainTambon = XmlManager.MakeClone<Entity>(tambon);
                    mainAmphoe.entity.Add(mainTambon);
                }
            }
            else
            {
                if (mainTambon.population.Any())
                {
                    mainTambon.population.First().data.AddRange(tambon.population.First().data);
                }
                else
                {
                    mainTambon.population.Add(tambon.population.First());
                }
            }
            if (mainAmphoe != null)
            {
                var population = tambon.population.First();
                foreach (var dataPoint in population.data)
                {
                    var amphoePopulation = mainAmphoe.population.FirstOrDefault();
                    if (amphoePopulation == null)
                    {
                        amphoePopulation = new PopulationData
                        {
                            referencedate = population.referencedate,
                            referencedateSpecified = population.referencedateSpecified,
                            source = population.source,
                            year = population.year
                        };
                        mainAmphoe.population.Add(amphoePopulation);
                    }
                    amphoePopulation.AddDataPoint(dataPoint);
                }
            }
        }

        /// <summary>
        /// Calculates the population data by summing up the data of the sub-entities.
        /// </summary>
        /// <param name="year">Year of data.</param>
        /// <param name="dataSource">Data source.</param>
        public void CalculatePopulationFromSubEntities(Int32 year, PopulationDataSourceType dataSource)
        {
            foreach (var subEntity in entity)
            {
                foreach (var dataPoint in subEntity.population.First(x => x.Year == year && x.source == dataSource).data)
                {
                    this.population.First(x => x.Year == year && x.source == dataSource).AddDataPoint(dataPoint);
                }
            }
        }

        internal void ParseName(String value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                foreach (var abbreviation in ThaiTranslations.EntityAbbreviations)
                {
                    value = value.Replace(abbreviation.Value + ".", ThaiTranslations.EntityNamesThai[abbreviation.Key]);  // especially the ThesabanTambon occurs sometimes
                }
                EntityType entityType = EntityType.Unknown;
                foreach (var entityTypeName in ThaiTranslations.EntityNamesThai)
                {
                    if (value.StartsWith(entityTypeName.Value))
                    {
                        entityType = entityTypeName.Key;
                    }
                }
                if (value.StartsWith("หมู่ที่"))
                {
                    entityType = EntityType.Muban;
                    name = value.Split(' ').ElementAtOrDefault(2);
                }
                else if ((entityType == EntityType.Unknown) | (entityType == EntityType.Bangkok))
                {
                    name = value;
                }
                else
                {
                    name = value.Replace(ThaiTranslations.EntityNamesThai[entityType], "");
                }
                if (entityType.IsSakha())
                {
                    // Some pages have the syntax "Name AmphoeName" with the word อำเภอ, others without
                    //Int32 pos = Name.IndexOf(Helper.EntityNames[EntityType.Amphoe]);
                    //if (pos > 0)
                    //{
                    //    mName = mName.Remove(pos - 1);
                    //}
                    Int32 pos = name.IndexOf(" ");
                    if (pos > 0)
                    {
                        name = name.Remove(pos);
                    }
                }
                obsolete = name.Contains("*");
                name = name.Replace("*", "");
                if (name.StartsWith("."))
                {
                    // Mistake in DOPA population statistic for Buriram 2005, a leading "."
                    name = name.Substring(1, name.Length - 1);
                }
                name = name.Trim();
                type = entityType;
            }
        }

        /// <summary>
        /// Gets the sub-entities where the geocode does not fit.
        /// </summary>
        /// <returns>Sub-entities where the geocode does not fit.</returns>
        public IEnumerable<Entity> InvalidGeocodeEntries()
        {
            var result = new List<Entity>();

            foreach (var subEntity in entity)
            {
                if (!GeocodeHelper.IsBaseGeocode(this.geocode, subEntity.geocode))
                {
                    result.Add(subEntity);
                }

                Int32 entitiesWithSameCode = 0;
                foreach (var subEntityForCount in entity)
                {
                    if (subEntityForCount.geocode == subEntity.geocode)
                    {
                        entitiesWithSameCode++;
                    }
                }
                if (entitiesWithSameCode > 1)
                {
                    result.Add(subEntity);
                }

                result.AddRange(subEntity.InvalidGeocodeEntries());
            }
            return result;
        }

        /// <summary>
        /// Copies the basic data from <paramref name="source"/> to self.
        /// </summary>
        /// <param name="source">Source entity.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
        /// <remarks>Copies <see cref="geocode"/>, <see cref="english"/>, <see cref="name"/>, <see cref="type"/> and <see cref="parent"/>.</remarks>
        public void CopyBasicDataFrom(Entity source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            geocode = source.geocode;
            english = source.english;
            name = source.name;
            type = source.type;
            parent.AddRange(source.parent);
        }

        /// <summary>
        /// Gets a list of previous names.
        /// </summary>
        /// <value>List of previous names.</value>
        internal IEnumerable<String> OldNames
        {
            get
            {
                var result = new List<String>();
                foreach (var item in history.Items)
                {
                    if (item is HistoryRename itemRename)
                    {
                        result.Add(itemRename.oldname);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the population data of the given type and year.
        /// </summary>
        /// <param name="source">Data source type.</param>
        /// <param name="year">Reference year.</param>
        /// <returns>Population data, or <c>null</c> if none found.</returns>
        public PopulationDataPoint GetPopulationDataPoint(PopulationDataSourceType source, Int16 year)
        {
            var data = this.population.FirstOrDefault(x => (x.Year == year) && (x.source == source));
            if (data != null)
            {
                return data.TotalPopulation;
            }
            else
            {
                return null;
            }
        }

        private Boolean SameNameAndType(String findName, EntityType findType)
        {
            return (name == findName) & (type.IsCompatibleEntityType(findType));
        }

        private Entity FindByNameAndType(String findName, EntityType findType, Boolean allowOldNames)
        {
            var result = FindByNameAndType(findName, findType, allowOldNames, false, 0);
            if (result == null)
            {
                result = FindByNameAndType(findName, findType, allowOldNames, true, 0);
            }
            return result;
        }

        private Entity FindByNameAndType(String findName, EntityType findType, Boolean allowOldNames, Boolean allowObsolete, Int32 startPosition)
        {
            Entity retval = null;
            this.entity.Sort((x, y) => x.geocode.CompareTo(y.geocode));

            foreach (var subEntity in entity)
            {
                if (subEntity.SameNameAndType(findName, findType))
                {
                    if ((!subEntity.obsolete) | allowObsolete)
                    {
                        startPosition--;
                        if (startPosition < 0)
                        {
                            retval = subEntity;
                            break;
                        }
                    }
                }
                if (allowOldNames & (subEntity.OldNames.Contains(findName)) & (subEntity.type.IsCompatibleEntityType(findType)))
                {
                    if ((!subEntity.obsolete) | allowObsolete)
                    {
                        startPosition--;
                        if (startPosition < 0)
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

            if (geocodeSource != null)
            {
                var sourceFlat = geocodeSource.FlatList();
                foreach (var entity in this.FlatList())
                {
                    var source = sourceFlat.FirstOrDefault(x => GeocodeHelper.IsSameGeocode(x.geocode, entity.geocode, false));
                    if (source == null)
                    {
                        missedEntities.Add(entity);
                    }
                    else
                    {
                        entity.CopyBasicDataFrom(source);
                    }
                }
            }
        }

        internal void ConsolidatePopulationData()
        {
            foreach (var data in population)
            {
                data.MergeIdenticalEntries();
            }
        }

        /// <summary>
        /// Propagate the post code to the entities within the entity.
        /// </summary>
        public void PropagatePostcode()
        {
            // only propagate if exactly one postcode
            if ((codes != null) && (codes.post != null) && (codes.post.value.Count == 1))
            {
                var postCode = codes.post.value.Single();
                if (postCode > 100)  // don't propagate the province post codes!
                {
                    foreach (var subentity in entity)
                    {
                        subentity.codes.post.value.Add(postCode);
                    }
                }
            }
        }

        /// <summary>
        /// Propagate the post code to the entities within the entity, and doing the same for every sub entity.
        /// </summary>
        public void PropagatePostcodeRecursive()
        {
            PropagatePostcode();
            foreach (var subentity in entity)
            {
                subentity.PropagatePostcodeRecursive();
            }
        }

        /// <summary>
        /// Sets <see cref="obsolete"/> to all sub-entities if the parent entity is obsolete.
        /// </summary>
        public void PropagateObsoleteToSubEntities()
        {
            foreach (var item in entity.Where(x => x.IsObsolete && x.entity.Any()))
            {
                foreach (var subItem in item.FlatList())
                {
                    subItem.obsolete = true;
                }
            }
            foreach (var item in entity.Where(x => !x.IsObsolete))
            {
                item.PropagateObsoleteToSubEntities();
            }
        }

        /// <summary>
        /// Calculates the postal codes from the tambon in the area coverage list of the LAO office.
        /// </summary>
        /// <param name="tambon">List of tambon in the parent of the LAO.</param>
        public void CalculatePostcodeForLocalAdministration(IEnumerable<Entity> tambon)
        {
            var laoOffice = office.FirstOrDefault(x => x.areacoverage.Any());
            // var fullCoveredPostCodes = new List<UInt32>();
            if (!this.codes.post.value.Any() && laoOffice != null)
            {
                // fullCoveredPostCodes.Clear();
                foreach (var coverage in laoOffice.areacoverage)
                {
                    var foundTambon = tambon.FirstOrDefault(x => x.geocode == coverage.geocode);
                    if (foundTambon != null)
                    {
                        this.codes.post.value.AddRange(foundTambon.codes.post.value);
                        if (coverage.coverage == CoverageType.completely || foundTambon.codes.post.value.Count == 1)
                        {
                            this.codes.post.comment = "Postcodes might contain too many values.";
                            // fullCoveredPostCodes.AddRange(foundTambon.codes.post.value);
                        }
                    }
                }
                this.codes.post.value = this.codes.post.value.Distinct().ToList();
                // if ( this.codes.post.value.Where(x => !fullCoveredPostCodes.Contains(x)).Any() )
                // {
                //     this.codes.post.comment = "Postcodes might contain too many values.";
                // }
            }
        }

        /// <summary>
        /// Gets the display name of the entity.
        /// </summary>
        /// <returns>The display name of the entity.</returns>
        /// <remarks>Always returns <see cref="Entity.english"/>.</remarks>
        public override String ToString()
        {
            return english;
        }

        /// <summary>
        /// Places all sub-entities into one flat list.
        /// </summary>
        /// <returns>All sub-entities.</returns>
        public IEnumerable<Entity> FlatList()
        {
            var result = new List<Entity>
            {
                this
            };
            foreach (var subEntity in entity)
            {
                result.AddRange(subEntity.FlatList());
            }
            return result;
        }

        private readonly IEnumerable<OfficeType> _officesWithElectedOfficials = new List<OfficeType>() { OfficeType.MunicipalityOffice, OfficeType.PAOOffice, OfficeType.TAOOffice };

        private IEnumerable<EntityTermEnd> OfficialElectionsPending(Boolean includeVacant)
        {
            var result = new List<EntityTermEnd>();
            foreach (var officeEntry in office)
            {
                if ((!officeEntry.obsolete) && _officesWithElectedOfficials.Contains(officeEntry.type))
                {
                    var term = officeEntry.officials.OfficialTermsOrVacancies.FirstOrDefault();
                    if (term != null)
                    {
                        DateTime termEnd;
                        if (term is OfficialVacancy vacancy)
                        {
                            if (includeVacant)
                            {
                                if (vacancy.endSpecified && vacancy.end.CompareTo(DateTime.Now) <= 0)
                                {
                                    result.Add(new EntityTermEnd(this, null, new OfficialEntryUnnamed() { end = vacancy.end, endSpecified = true }));
                                }
                                else
                                {
                                    result.Add(new EntityTermEnd(this, null, new OfficialEntryUnnamed()));
                                }
                            }
                        }
                        else
                        {
                            var officialTerm = term as OfficialEntryBase;
                            if (term.endSpecified)
                            {
                                termEnd = term.end;
                            }
                            else if (officialTerm.beginreason == OfficialBeginType.TermExtended)
                            {
                                termEnd = DateTime.Now.AddYears(50);
                            }
                            else
                            {
                                termEnd = term.begin.AddYears(4).AddDays(-1);
                            }
                            if ((termEnd.CompareTo(DateTime.Now) <= 0))
                            {
                                result.Add(new EntityTermEnd(this, null, officialTerm));
                            }
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

        private IEnumerable<EntityTermEnd> OfficialVacant()
        {
            var result = new List<EntityTermEnd>();
            foreach (var officeEntry in office)
            {
                if ((!officeEntry.obsolete) && _officesWithElectedOfficials.Contains(officeEntry.type))
                {
                    var vacancy = officeEntry.officials.OfficialTermsOrVacancies.FirstOrDefault();
                    if ( vacancy is OfficialVacancy)
                    {
                        if (vacancy.endSpecified && vacancy.end.CompareTo(DateTime.Now) <= 0)
                        {
                            result.Add(new EntityTermEnd(this, null, new OfficialEntryUnnamed() { end = vacancy.end, endSpecified = true }));
                        }
                        else
                        {
                            result.Add(new EntityTermEnd(this, null, new OfficialEntryUnnamed()));
                        }
                    }
                }
            }

            return result;
        }

        private IEnumerable<EntityTermEnd> LatestOfficialElectionResultUnknown()
        {
            var result = new List<EntityTermEnd>();
            foreach (var officeEntry in office)
            {
                if ((!officeEntry.obsolete) && _officesWithElectedOfficials.Contains(officeEntry.type))
                {
                    officeEntry.officials.SortByDate();
                    var term = officeEntry.officials.OfficialTerms.FirstOrDefault();
                    if (term != null)
                    {
                        var name = String.Empty;
                        if (term is OfficialEntry officialTerm)
                        {
                            name = officialTerm.name;
                        }
                        if (String.IsNullOrWhiteSpace(name))
                        {
                            result.Add(new EntityTermEnd(this, null, term));
                        }
                    }
                }
            }
            return result;
        }

        private IEnumerable<EntityTermEnd> CouncilElectionsPending()
        {
            var result = new List<EntityTermEnd>();
            foreach (var officeEntry in office)
            {
                if (!officeEntry.obsolete)
                {
                    officeEntry.council.SortByDate();
                    var term = officeEntry.council.CouncilTerms.FirstOrDefault();
                    if (term != null)
                    // foreach ( var term in office.council )
                    {
                        DateTime termEnd;
                        if (term.endSpecified)
                        {
                            termEnd = term.end;
                        }
                        else if (term.beginreason == TermBeginType.TermExtended)
                        {
                            if (term.type == EntityType.TAO)
                            {
                                termEnd = new DateTime(2050, 1, 1);  // just a dummy future date
                            }
                            else
                            {
                                termEnd = new DateTime(2021, 3, 27);
                            }
                        }
                        else
                        {
                            termEnd = term.begin.AddYears(4).AddDays(-1);
                        }
                        if ((termEnd.CompareTo(DateTime.Now) <= 0))
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
            foreach (var officeEntry in office)
            {
                officeEntry.officials.SortByDate();
                var term = officeEntry.officials.OfficialTerms.FirstOrDefault();
                if (term != null)
                // foreach ( var term in office.council )
                {
                    DateTime termEnd;
                    if (term.endSpecified)
                    {
                        termEnd = term.end;
                    }
                    else if (term.beginreason == OfficialBeginType.TermExtended)
                    {
                        if (term.title == OfficialType.TAOMayor)
                        {
                            termEnd = new DateTime(2050, 1, 1);  // just a dummy future date
                        }
                        else
                        {
                            termEnd = new DateTime(2021, 3, 27);
                        }
                    }
                    else
                    {
                        termEnd = term.begin.AddYears(4).AddDays(-1);
                    }
                    if ((termEnd.CompareTo(begin) >= 0) & (termEnd.CompareTo(end) <= 0))
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
            foreach (var officeEntry in office)
            {
                officeEntry.council.SortByDate();
                foreach (var term in officeEntry.council.CouncilTerms)
                {
                    DateTime termEnd;
                    if (term.endSpecified)
                    {
                        termEnd = term.end;
                    }
                    else
                    {
                        termEnd = term.begin.AddYears(4).AddDays(-1);
                    }
                    if ((termEnd.CompareTo(begin) >= 0) & (termEnd.CompareTo(end) <= 0))
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
            foreach (var item in FlatList())
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
            foreach (var item in FlatList())
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
            foreach (var item in FlatList())
            {
                result.AddRange(item.CouncilElectionsPending());
            }

            return result;
        }

        /// <summary>
        /// Calculates list of entities which have no elected official currently.
        /// </summary>
        /// <param name="includeVacancy"><c>true</c> to include explicitly vacant posts as well, <c>false</c> otherwise.</param>
        /// <returns>List of entities without elected official.</returns>
        public IEnumerable<EntityTermEnd> EntitiesWithOfficialElectionPending(Boolean includeVacancy)
        {
            var result = new List<EntityTermEnd>();
            foreach (var item in FlatList())
            {
                result.AddRange(item.OfficialElectionsPending(includeVacancy));
            }

            return result;
        }

        /// <summary>
        /// Calculates list of entities which have no elected official currently.
        /// </summary>
        /// <returns>List of entities without elected official.</returns>
        public IEnumerable<EntityTermEnd> EntitiesWithOfficialVacant()
        {
            var result = new List<EntityTermEnd>();
            foreach (var item in FlatList())
            {
                result.AddRange(item.OfficialVacant());
            }

            return result;
        }

        /// <summary>
        /// Calculates list of entities without a result for the latest official election.
        /// </summary>
        /// <returns>List of entities without a result for the latest official election.</returns>
        public IEnumerable<EntityTermEnd> EntitiesWithLatestOfficialElectionResultUnknown()
        {
            var result = new List<EntityTermEnd>();
            foreach (var item in FlatList())
            {
                result.AddRange(item.LatestOfficialElectionResultUnknown());
            }

            return result;
        }

        private String GetGermanWikiDataDescription()
        {
            String expandText = String.Empty;
            switch (this.type)
            {
                case EntityType.Changwat:
                    return "Provinz in Thailand";

                case EntityType.Khet:
                    return "Bezirk von Bangkok, Thailand";

                case EntityType.Khwaeng:
                    expandText = "Unterbezirk von {0}, Bangkok, Thailand";
                    break;

                case EntityType.Amphoe:
                case EntityType.KingAmphoe:
                    expandText = "Landkreis in der Provinz {0}, Thailand";
                    break;

                case EntityType.Tambon:
                    expandText = "Kommune im Landkreis {0}, Provinz {1}, Thailand";
                    break;

                case EntityType.Muban:
                    expandText = "Dorf in Kommune {0}, Landkreis {1}, Provinz {2}, Thailand";
                    break;

                case EntityType.TAO:
                case EntityType.Thesaban:
                case EntityType.ThesabanTambon:
                case EntityType.ThesabanMueang:
                case EntityType.ThesabanNakhon:
                    expandText = this.type.Translate(Language.German) + " im Landkreis {0}, Provinz {1}, Thailand";
                    break;
            }
            var allEntities = GlobalData.CompleteGeocodeList().FlatList();
            var parents = new String[3];
            var currentGeocode = geocode;
            if (this.parent.Any())
            {
                currentGeocode = this.parent.First() * 100;
            }
            var index = 0;
            while (currentGeocode / 100 != 0)
            {
                currentGeocode = currentGeocode / 100;
                var parentEntity = allEntities.First(x => x.geocode == currentGeocode);
                parents[index] = parentEntity.english;
                index++;
            }
            for (Int32 i = index; i < 3; i++)
            {
                parents[i] = String.Empty;
            }
            return String.Format(expandText, parents);
        }

        /// <summary>
        /// Gets the description ready to be set to WikiData.
        /// </summary>
        /// <param name="language">Language.</param>
        /// <returns>Description of the entity.</returns>
        public String GetWikiDataDescription(Language language)
        {
            if (type == EntityType.PAO)
            {
                switch (language)
                {
                    case Language.English:
                        return "local government unit in Thailand";

                    case Language.German:
                        return "kommunale Verwaltungseinheit von Thailand";

                    case Language.Thai:
                        return "องค์กรปกครองส่วนท้องถิ่นของประเทศไทย";

                    default:
                        return String.Empty;
                }
            }
            else if (language == Language.German)
            {
                // the hierarchical expansion does not sound good in German
                return GetGermanWikiDataDescription();
            }
            else
            {
                var allEntities = GlobalData.CompleteGeocodeList().FlatList();
                var typeValue = this.type.Translate(language);
                var expanded = String.Empty;  // 0 = type, 1 = hierarchy
                var expandedTopLevel = String.Empty;  // 0 = type
                var hierarchy = String.Empty;
                var hierarchyExpand = String.Empty;  // 0 = name, 1 = type
                switch (language)
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
                if (this.parent.Any())
                {
                    currentGeocode = this.parent.First() * 100;
                }
                while (currentGeocode / 100 != 0)
                {
                    currentGeocode = currentGeocode / 100;
                    var parentEntity = allEntities.First(x => x.geocode == currentGeocode);
                    var parentType = parentEntity.type.Translate(language);
                    if (language == Language.Thai)
                        hierarchy += String.Format(hierarchyExpand, parentEntity.name, parentType);
                    else if (parentEntity.type == EntityType.Bangkok)
                        hierarchy += String.Format(hierarchyExpand, String.Empty, "Bangkok").TrimStart();
                    else
                        hierarchy += String.Format(hierarchyExpand, parentEntity.english, parentType);
                }
                return String.Format(expanded, typeValue, hierarchy);
            }
        }

        /// <summary>
        /// Gets the abbreviated Thai name.
        /// </summary>
        /// <value>The abbreviated Thai name, or String.Empty if no abbreviation is available.</value>
        /// <remarks>Abbreviated name consists of the abbreviation of the administrative type followed by the name.</remarks>
        public String AbbreviatedName
        {
            get
            {
                var abbreviation = ThaiTranslations.EntityAbbreviations[type];
                if (String.IsNullOrEmpty(abbreviation))
                {
                    return String.Empty;
                }
                else
                {
                    return String.Format("{0}.{1}", abbreviation, name);
                }
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
                var prefix = ThaiTranslations.EntityNamesThai[type];
                if (name.StartsWith(prefix))
                {
                    return name;
                }
                else
                {
                    return ThaiTranslations.EntityNamesThai[type] + name;
                }
            }
        }

        /// <summary>
        /// Gets the full English name.
        /// </summary>
        /// <value>The full English name.</value>
        /// <remarks>Full name consists of the romanized name followed by the administrative type.</remarks>
        public String EnglishFullName
        {
            get
            {
                return english + " " + ThaiTranslations.EntityNamesEnglish[type];
            }
        }

        /// <summary>
        /// Gets a list of obsolete geocodes of the entity.
        /// </summary>
        /// <value>List of obsolete geocodes of the entity.</value>
        public IEnumerable<UInt32> OldGeocodes
        {
            get
            {
                if (_oldGeocode == null)
                {
                    CalcOldGeocodes(GlobalData.CompleteGeocodeList().FlatList());
                }
                return _oldGeocode;
            }
        }

        private void CalcOldGeocodes(IEnumerable<Entity> allEntities)
        {
            if (_oldGeocode == null)
            {
                var entities = allEntities.Where(x => x.newgeocode.Contains(this.geocode));
                _oldGeocode = entities.Select(x => x.geocode).ToList();
            }
        }

        private void CalcOldGeocodesRecursive(IEnumerable<Entity> allEntities)
        {
            CalcOldGeocodes(allEntities);
            foreach (var subEntity in entity)
            {
                subEntity.CalcOldGeocodesRecursive(allEntities);
            }
            if ((this.type == EntityType.Tambon) && _oldGeocode != null && _oldGeocode.Any())
            {
                foreach (var muban in this.entity)
                {
                    if (muban._oldGeocode == null)
                    {
                        muban._oldGeocode = new List<UInt32>();
                    }
                    foreach (var oldCode in _oldGeocode)
                    {
                        muban._oldGeocode.Add(oldCode * 100 + muban.geocode % 100);
                    }
                }
            }
        }

        /// <summary>
        /// Fills <see cref="OldGeocodes"/> for this entity and all its sub-entities.
        /// </summary>
        /// <remarks>Usually to be used on country or province entities.</remarks>
        public void CalcOldGeocodesRecursive()
        {
            CalcOldGeocodesRecursive(GlobalData.CompleteGeocodeList().FlatList().Where(x => x.newgeocode.Any()).ToList());
            var allTambon = this.FlatList().Where(x => x.type == EntityType.Tambon);
            foreach (var thesabanWithTambon in this.FlatList().Where(x => x.tambonSpecified))
            {
                if (thesabanWithTambon._oldGeocode == null)
                {
                    thesabanWithTambon._oldGeocode = new List<UInt32>();
                }
                if (!thesabanWithTambon._oldGeocode.Contains(thesabanWithTambon.tambon + 50))
                {
                    thesabanWithTambon._oldGeocode.Add(thesabanWithTambon.tambon + 50);
                }
                var tambon = allTambon.FirstOrDefault(x => x.geocode == thesabanWithTambon.tambon);
                if ((tambon != null) && (tambon._oldGeocode != null))
                {
                    foreach (var oldCode in tambon._oldGeocode)
                    {
                        thesabanWithTambon._oldGeocode.Add(oldCode + 50);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a special entity for the local governments which have no geocode by themselves, but are still linked with the Tambon.
        /// </summary>
        /// <returns>Newly created entity, or <c>null</c> is instance has no local government to split off.</returns>
        public Entity CreateLocalGovernmentDummyEntity()
        {
            Entity result = null;
            if (this.type == EntityType.Tambon || this.type == EntityType.Changwat)
            {
                var office = this.office.SingleOrDefault(x => x.type == OfficeType.TAOOffice || x.type == OfficeType.MunicipalityOffice || x.type == OfficeType.PAOOffice);
                if (office != null)
                {
                    result = new Entity
                    {
                        name = this.name,
                        english = this.english
                    };
                    if (this.type == EntityType.Tambon)
                    {
                        result.geocode = this.geocode + 50;  // see http://tambon.blogspot.com/2009/07/geocodes-for-municipalities-my-proposal.html
                    }
                    else
                    {
                        result.geocode = this.geocode * 100 * 100 + 50;
                    }
                    result.obsolete = office.obsolete;
                    if (office.type == OfficeType.TAOOffice)
                    {
                        result.type = EntityType.TAO;
                    }
                    else if (office.type == OfficeType.PAOOffice)
                    {
                        result.type = EntityType.PAO;
                    }
                    else
                    {
                        result.type = EntityType.Thesaban;
                    }
                    if (result.type == EntityType.PAO)
                    {
                        result.parent.Add(this.geocode);        // Province
                    }
                    else
                    {
                        result.tambon = this.geocode;
                        result.tambonSpecified = true;
                        result.parent.Add(this.geocode / 100);  // Amphoe
                    }

                    result.wiki = office.wiki;
                    result.area = office.area;
                    result.office.Add(office);
                    // history has latest change at beginning
                    foreach (var history in office.history.Items.Where(x => x.status == ChangeStatus.Done || x.status == ChangeStatus.Gazette).Reverse())
                    {
                        if (history is HistoryRename rename)
                        {
                            result.name = rename.name;
                            result.english = rename.english;
                        }
                        if (history is HistoryStatus status)
                        {
                            result.type = status.@new;
                        }
                        if (history is HistoryCreate create)
                        {
                            result.type = create.type;
                        }
                        result.history.Items.Add(history);
                    }
                    if (result.type == EntityType.ThesabanTambon || result.type == EntityType.ThesabanMueang || result.type == EntityType.ThesabanNakhon)
                    {
                        if (office.type == OfficeType.TAOOffice)
                        {
                            office.type = OfficeType.MunicipalityOffice;
                        }
                    }
                    if (this._oldGeocode != null)
                    {
                        if (result._oldGeocode == null)
                        {
                            result._oldGeocode = new List<UInt32>();
                        }
                        foreach (var code in this._oldGeocode)
                        {
                            result._oldGeocode.Add(code + 50);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Sorts the whole tree by its <see cref="Entity.geocode"/>.
        /// </summary>
        public void SortByGeocodeRecursively()
        {
            entity.Sort((x, y) => x.geocode.CompareTo(y.geocode));
            foreach (var subEntity in entity)
            {
                subEntity.SortByGeocodeRecursively();
            }
        }

        /// <summary>
        /// Gets an enumeration of all geocodes which are not set correctly.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UInt32> WrongGeocodes()
        {
            var result = new List<UInt32>();
            var subGeocodes = entity.Select(x => x.geocode);
            result.AddRange(subGeocodes.Where(x => !GeocodeHelper.IsBaseGeocode(this.geocode, x)));
            var duplicates = subGeocodes.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
            result.AddRange(duplicates);
            foreach (var subentity in entity)
            {
                result.AddRange(subentity.WrongGeocodes());
            }
            return result;
        }

        /// <summary>
        /// Gets the DOLA data entries.
        /// </summary>
        /// <value>DOLA data entries.</value>
        public IEnumerable<LocalAdministrationData> Dola
        {
            get
            {
                var localOffice = office.FirstOrDefault(y => y.type == OfficeType.MunicipalityOffice || y.type == OfficeType.TAOOffice || y.type == OfficeType.PAOOffice);
                if (localOffice != null)
                {
                    return localOffice.dola;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the area coverage entries of the local government.
        /// </summary>
        /// <value>The area coverage entries of the local government.</value>
        public IEnumerable<LocalGovernmentCoverageEntity> LocalGovernmentAreaCoverage
        {
            get
            {
                var localOffice = office.FirstOrDefault(y => !y.obsolete && (y.type == OfficeType.MunicipalityOffice || y.type == OfficeType.TAOOffice || y.type == OfficeType.PAOOffice));
                if (localOffice != null)
                {
                    return localOffice.areacoverage;
                }
                else
                {
                    return new List<LocalGovernmentCoverageEntity>();
                }
            }
        }

        /// <summary>
        /// Gets the latest code as assigned by DOLA.
        /// </summary>
        /// <returns>Latest codes as assigned by DOLA. 0 if no code assigned.</returns>
        public UInt32 LastedDolaCode()
        {
            var myDolaList = Dola;
            if (myDolaList == null)
            {
                return 0; // nothing specified
            }
            var myDola = myDolaList.Where(x => x.codeSpecified).OrderBy(y => y.year).LastOrDefault();
            if (myDola == null)
            {
                return 0; // no entry with code
            }
            else
            {
                return myDola.code;
            }
        }

        /// <summary>
        /// Checks whether the <see cref="LatestDolaCode"/> is well-formed.
        /// </summary>
        /// <returns><c>true</c> if latest DOLA code is well-formed, <c>false</c> otherwise.</returns>
        /// <remarks>If no code found, <c>true</c> is returned.</remarks>
        public Boolean DolaCodeValid()
        {
            var code = LastedDolaCode();
            if (code == 0)
            {
                return true;  // nothing specified -> valid
            }
            else
            {
                var result = true;
                var dolaCodeType = code / 1000000;
                switch (type)
                {
                    case EntityType.PAO:
                        result &= (dolaCodeType == 2);
                        break;

                    case EntityType.ThesabanNakhon:
                        result &= (dolaCodeType == 3);
                        break;

                    case EntityType.ThesabanMueang:
                        result &= (dolaCodeType == 4);
                        break;

                    case EntityType.ThesabanTambon:
                        result &= (dolaCodeType == 5);
                        break;

                    case EntityType.TAO:
                        result &= (dolaCodeType == 6);
                        break;
                }
                UInt32 dolaAmphoe = (code % 1000000) / 100;
                if (type == EntityType.PAO)
                {
                    result &= dolaAmphoe == (geocode / 10000) * 100 + 1;  // Amphoe Mueang of province
                }
                else
                {
                    result &= parent.Contains(dolaAmphoe);
                }
                return result;
            }
        }

        /// <summary>
        /// Checks whether the <see cref="EntityType.Muban"/> is consistent.
        /// </summary>
        /// <returns><c>true</c> if number is consistent, <c>false</c> otherwise.</returns>
        /// <remarks>Checks for numbers being consecutively, and the council size <see cref="EntityType.TAO"/> fitting with the number.</remarks>
        public Boolean MubanNumberConsistent()
        {
            var nrOfMuban = entity.Count(x => x.type == EntityType.Muban);
            var nrOfActiveMuban = entity.Count(x => x.type == EntityType.Muban && !x.IsObsolete);
            if (nrOfMuban != 0)
            {
                if (entity.Last(x => x.type == EntityType.Muban).geocode % 100 != nrOfMuban)
                {
                    return false;
                }
            }
            var taoOffice = office.FirstOrDefault(x => x.type == OfficeType.TAOOffice && !x.obsolete);
            if (taoOffice != null)
            {
                if (taoOffice.areacoverage.Count < 2)  // if more than one Tambon, this simple check will fail
                {
                    var latestTerm = taoOffice.council.CouncilTerms.First();
                    if (latestTerm.FinalSize > Math.Max(6, nrOfActiveMuban * 2))  // max(6,x) due to minimum size of council
                    {
                        return false;
                    }
                }
            }
            foreach (var counter in entitycount.SelectMany(x => x.entry.Where(y => y.type == EntityType.Muban)))
            {
                if (counter.count > nrOfMuban)
                {
                    return false;  // had more Muban in past than now
                }
                if ((counter.count == 0) && (nrOfActiveMuban > 0))
                {
                    return false;  // had no Muban in past, but now has some
                }
            }
            return true;
        }

        /// <summary>
        /// Calculates the population for each of the local governments.
        /// </summary>
        /// <param name="localGovernments">Local governments to calculate.</param>
        /// <param name="allEntities">All entities covered by the local governments.</param>
        /// <param name="populationDataSource">Data source of the population data.</param>
        /// <param name="populationYear">Reference year of the population data.</param>
        public static void FillExplicitLocalGovernmentPopulation(IEnumerable<Entity> localGovernments, IEnumerable<Entity> allEntities, PopulationDataSourceType populationDataSource, Int16 populationYear)
        {
            var allPopulationData = allEntities.SelectMany(x => x.population.Where(y => y.Year == populationYear && y.source == populationDataSource));
            // ToList() as the add of population data below will change the enumeration
            var allPopulationDataWithGeocode = allPopulationData.Where(p => p.data.Any(d => d.geocode.Any())).ToList();
            foreach (var sourcePopulationData in allPopulationDataWithGeocode)
            {
                // only use those population data which are exactly on one entity
                foreach (var populationDataPoint in allPopulationDataWithGeocode.SelectMany(p => p.data.Where(d => d.geocode.Count == 1)))
                {
                    var localGovernment = localGovernments.FirstOrDefault(x => populationDataPoint.geocode.Contains(x.geocode));
                    if (localGovernment != null && !localGovernment.population.Any(x => x.year == sourcePopulationData.year && x.source == sourcePopulationData.source))
                    {
                        var newDataPoint = new HouseholdDataPoint(populationDataPoint)
                        {
                            type = PopulationDataType.total
                        };
                        var populationData = new PopulationData
                        {
                            year = sourcePopulationData.year,
                            referencedate = sourcePopulationData.referencedate,
                            referencedateSpecified = sourcePopulationData.referencedateSpecified,
                            source = sourcePopulationData.source
                        };
                        populationData.data.Add(newDataPoint);
                        populationData.reference.AddRange(sourcePopulationData.reference);
                        localGovernment.population.Add(populationData);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the population for each of the local governments.
        /// </summary>
        /// <param name="localGovernments">Local governments to calculate.</param>
        /// <param name="allTambon">All tambon covered by the local governments.</param>
        /// <param name="populationDataSource">Data source of the population data.</param>
        /// <param name="populationYear">Reference year of the population data.</param>
        public static void CalculateLocalGovernmentPopulation(IEnumerable<Entity> localGovernments, IEnumerable<Entity> allTambon, PopulationDataSourceType populationDataSource, Int16 populationYear)
        {
            foreach (var localEntityWithoutPopulation in localGovernments.Where(x =>
               x.LocalGovernmentAreaCoverage.Any() && !x.population.Any(
               y => y.Year == populationYear && y.source == populationDataSource)))
            {
                var populationData = new PopulationData();
                localEntityWithoutPopulation.population.Add(populationData);
                foreach (var coverage in localEntityWithoutPopulation.LocalGovernmentAreaCoverage)
                {
                    var tambon = allTambon.Single(x => x.geocode == coverage.geocode);
                    var sourcePopulationData = tambon.population.FirstOrDefault(y => y.Year == populationYear && y.source == populationDataSource);
                    if (sourcePopulationData != null)
                    {
                        populationData.year = sourcePopulationData.year;
                        populationData.referencedate = sourcePopulationData.referencedate;
                        populationData.referencedateSpecified = sourcePopulationData.referencedateSpecified;
                        populationData.source = sourcePopulationData.source;

                        List<HouseholdDataPoint> dataPointToClone = new List<HouseholdDataPoint>();
                        dataPointToClone.AddRange(sourcePopulationData.data.Where(x => x.geocode.Contains(localEntityWithoutPopulation.geocode)));
                        if (!dataPointToClone.Any())
                        {
                            if (coverage.coverage == CoverageType.completely)
                            {
                                dataPointToClone.AddRange(sourcePopulationData.data);
                            }
                            else
                            {
                                dataPointToClone.AddRange(sourcePopulationData.data.Where(x => x.type == PopulationDataType.nonmunicipal));
                            }
                        }
                        foreach (var dataPoint in dataPointToClone)
                        {
                            var newDataPoint = new HouseholdDataPoint(dataPoint);
                            newDataPoint.geocode.Add(coverage.geocode);
                            populationData.data.Add(newDataPoint);
                        }
                    }
                }
                if (populationData.data.Count == 1)
                {
                    populationData.data.First().type = PopulationDataType.total;
                }
                populationData.CalculateTotal();
            }
        }

        /// <summary>
        /// Counts the types of entities in the given enumeration.
        /// </summary>
        /// <param name="entities">Entities to count.</param>
        /// <returns>Number of entities by entity type.</returns>
        public static Dictionary<EntityType, Int32> CountSubdivisions(IEnumerable<Entity> entities)
        {
            var counted = entities.GroupBy(x => x.type).Select(group => new
            {
                type = group.Key,
                count = group.Count()
            });
            var result = new Dictionary<EntityType, Int32>();
            foreach (var keyvaluepair in counted)
            {
                result[keyvaluepair.type] = keyvaluepair.count;
            }
            return result;
        }

        /// <summary>
        /// Calculates the subdivisions within this instance which have no location set.
        /// </summary>
        /// <param name="localGovernments">Local governments, to include if below this instance.</param>
        /// <returns>Number of entities by entity type.</returns>
        public Dictionary<EntityType, Int32> CountSubdivisionsWithoutLocation(IEnumerable<Entity> localGovernments)
        {
            var toCount = new List<Entity>();
            if (localGovernments != null)
            {
                toCount.AddRange(LocalGovernmentEntitiesOf(localGovernments));
            }
            toCount.AddRange(this.FlatList().Where(x => !x.type.IsLocalGovernment()));
            toCount.RemoveAll(x => x.type == EntityType.Unknown || x.IsObsolete);
            toCount.RemoveAll(x => x.office.Any(y => y.Point != null));
            return Entity.CountSubdivisions(toCount);
        }

        /// <summary>
        /// Calculates the subdivisions within this instance.
        /// </summary>
        /// <param name="localGovernments">Local governments, to include if below this instance.</param>
        /// <returns>Number of entities by entity type.</returns>
        public Dictionary<EntityType, Int32> CountAllSubdivisions(IEnumerable<Entity> localGovernments)
        {
            var toCount = new List<Entity>();
            if (localGovernments != null)
            {
                toCount.AddRange(LocalGovernmentEntitiesOf(localGovernments).SelectMany(x => x.FlatList()));
            }
            // Chumchon and local governments are already in list, so filter them out while adding the central government units
            toCount.AddRange(this.FlatList().Where(x => !x.type.IsLocalGovernment() && x.type != EntityType.Chumchon));
            toCount.RemoveAll(x => x.type == EntityType.Unknown || x.IsObsolete);
            return Entity.CountSubdivisions(toCount);
        }

        /// <summary>
        /// Filters the local governments which are below this instance.
        /// </summary>
        /// <param name="localGovernments">Local governments to filter.</param>
        /// <returns>Filtered local governments.</returns>
        public IEnumerable<Entity> LocalGovernmentEntitiesOf(IEnumerable<Entity> localGovernments)
        {
            return localGovernments.Where(x => x.parent.Contains(this.geocode) || GeocodeHelper.IsBaseGeocode(this.geocode, x.geocode));
        }

        /// <summary>
        /// Gets the current entity in case <see cref="IsObsolete"/> is <c>true</c>.
        /// </summary>
        /// <param name="baseEntity">Base entity to lookup for the current entity.</param>
        /// <returns>Self if <see cref="IsObsolete"/> is <c>false</c>, the active entity referenced by <see cref="newgeocode"/> otherwise.</returns>
        public Entity CurrentEntity(Entity baseEntity)
        {
            var result = this;
            if (newgeocode.Any())
            {
                var flatList = baseEntity.FlatList();
                var newEntities = newgeocode.SelectMany(x => flatList.Where(y => y.geocode == x));
                result = newEntities.FirstOrDefault(x => !x.IsObsolete);
                if (result == null)
                {
                    newEntities = newEntities.SelectMany(x => flatList.Where(y => y.geocode == x.geocode));
                    result = newEntities.FirstOrDefault(x => !x.IsObsolete);
                }
            }
            return result;
        }

        /// <summary>
        /// Calculates the sub-entity after which the entity was named.
        /// </summary>
        /// <returns>Sub-entity after which the entity was named. <c>null</c> if not named after any sub-entity.</returns>
        /// <remarks>In case either entity or sub-entity had name changes, <c>null</c> is returned instead of checking the name at creation.</remarks>
        public Entity NamedAfterEntity()
        {
            Entity result = null;
            var creation = history.Items.OfType<HistoryCreate>().FirstOrDefault(x => x.status == ChangeStatus.Done || x.status == ChangeStatus.Gazette);
            if (creation != null && !creation.Items.OfType<HistoryRename>().Any())
            {
                if (this.type.IsLocalGovernment())
                {
                    if (creation.type == EntityType.TAO || creation.type == EntityType.SaphaTambon)
                    {
                        var allTambon = GlobalData.CompleteGeocodeList().FlatList().Where(x => x.type == EntityType.Tambon);
                        var sameNamedTambon = allTambon.Where(x => x.name == this.name).ToList();
                        result = sameNamedTambon.FirstOrDefault(x => this.LocalGovernmentAreaCoverage.Any(y => y.geocode == x.geocode));
                    }
                }
                else
                {
                    var subEntity = entity.FirstOrDefault(x => x.name == this.name || x.name.StripBanOrChumchon() == this.name);
                    if (subEntity != null)
                    {
                        var subEntityReassign = subEntity.history.Items.OfType<HistoryReassign>().FirstOrDefault(x => x.effective == creation.effective);
                        if (subEntityReassign != null && !subEntityReassign.Items.OfType<HistoryRename>().Any())
                        {
                            result = subEntity;
                        }
                    }
                }
            }
            return result;
        }
    }
}