using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wikibase;
using Wikibase.DataValues;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Main translation class between <see cref="Entity"/> and the WikiData structures.
    /// </summary>
    public class WikiDataHelper
    {
        private IEnumerable<Entity> _allEntities;

        /// <summary>
        /// Gets the WikiData writing API encapsulation.
        /// </summary>
        /// <value>The WikiData writing API encapsulation.</value>
        public WikibaseApi Api
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of <see cref="WikiDataHelper"/>.
        /// </summary>
        /// <param name="api">The WikiData API encapsulation.</param>
        public WikiDataHelper(WikibaseApi api)
        {
            if ( api == null )
                throw new ArgumentNullException("api");
            Api = api;

            var entities = GlobalData.CompleteGeocodeList();
            _allEntities = entities.FlatList();
        }

        /// <summary>
        /// Gets the wikidata item for the given <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">Entity to search.</param>
        /// <returns>Wikidata entity.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="entity"/> has no wikidata link.</exception>
        public Item GetWikiDataItemForEntity(Entity entity)
        {
            if ( entity == null )
                throw new ArgumentNullException("entity");
            if ( (entity.wiki == null) || (String.IsNullOrEmpty(entity.wiki.wikidata)) )
                throw new ArgumentException("no wikidata entity yet");
            EntityProvider entityProvider = new EntityProvider(Api);

            Item entityById = entityProvider.getEntityFromId(EntityId.newFromPrefixedId(entity.wiki.wikidata)) as Item;
            return entityById;
        }

        /// <summary>
        /// Gets the statement containing the parent administrative unit.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <returns>Statement containing the parent administrative unit.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> or <paramref name="entity"/> is <c>null</c>.</exception>
        public Statement IsInAdministrativeUnit(Item item, Entity entity)
        {
            if ( item == null )
                throw new ArgumentNullException("item");
            if ( entity == null )
                throw new ArgumentNullException("entity");

            Statement claim = item.Claims.FirstOrDefault(x => x.id.ToUpperInvariant() == WikiBase.PropertyIdIsInAdministrativeUnit) as Statement;
            var parentEntity = _allEntities.First(x => x.geocode == entity.geocode / 100);
            var parent = EntityId.newFromPrefixedId(parentEntity.wiki.wikidata);
            var dataValueParent = new EntityIdValue("item", parent.numericId);
            var parentSnak = new Snak("value", EntityId.newFromPrefixedId(WikiBase.PropertyIdIsInAdministrativeUnit), dataValueParent);
            if ( claim == null )
            {
                claim = item.createStatementForSnak(parentSnak);
            }
            else
            {
                Snak snak = claim.mainSnak;
                var dataValue = snak.dataValue as EntityIdValue;
                if ( dataValue.numericId != parent.numericId )
                {
                    claim.mainSnak = parentSnak;
                }
            }
            return claim as Statement;
        }

        /// <summary>
        /// Gets the statement containing the country.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <returns>Statement containing the country.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public Statement IsInCountry(Item item)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            Statement claim = item.Claims.FirstOrDefault(x => x.id.ToUpperInvariant() == WikiBase.PropertyIdCountry) as Statement;
            var country = EntityId.newFromPrefixedId(WikiBase.WikiDataItems[EntityType.Country]);
            var dataValueCountry = new EntityIdValue("item", country.numericId);
            var countrySnak = new Snak("value", EntityId.newFromPrefixedId(WikiBase.PropertyIdCountry), dataValueCountry);
            if ( claim == null )
            {
                claim = item.createStatementForSnak(countrySnak);
            }
            else
            {
                Snak snak = claim.mainSnak;
                var dataValue = snak.dataValue as EntityIdValue;
                if ( dataValue.numericId != country.numericId )
                {
                    claim.mainSnak = countrySnak;
                }
            }
            return claim as Statement;
        }

        /// <summary>
        /// Get the default edit summary for a claim save.
        /// </summary>
        /// <param name="value">Claim to ber parsed.</param>
        /// <returns>Edit summary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        public String GetClaimSaveEditSummary(Claim value)
        {
            if ( value == null )
                throw new ArgumentNullException("value");

            var result = String.Empty;
            Snak snak = value.mainSnak;
            var entityIdValue = snak.dataValue as EntityIdValue;
            if ( entityIdValue != null )
            {
                result = String.Format("[[Property:P{0}]] [[Q{1}]]", snak.propertyId.numericId, entityIdValue.numericId);
            }
            var stringValue = snak.dataValue as StringValue;
            if ( stringValue != null )
            {
                result = String.Format("[[Property:P{0}]] {1}", snak.propertyId.numericId, stringValue.str);
            }
            return result;
        }

        /// <summary>
        /// Sets the descriptions, labels and aliases to a WikiData item.
        /// </summary>
        /// <param name="item">The WikiData item to be modified.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> or <paramref name="entity"/> is <c>null</c>.</exception>
        /// <remarks>Set the following things:
        /// <list type="bullet">
        /// <item>English label to <see cref="Entity.english"/></item>
        /// <item>Thai label to <see cref="Entity.FullName"/></item>
        /// <item>Thai alias to <see cref="Entity.name"/></item>
        /// <item>Thai alias to <see cref="Entity.AbbreviatedName"/></item>
        /// <item>Thai and English descriptions to <see cref="Entity.GetWikiDataDescription"/></item>
        /// </list>
        /// </remarks>
        public void SetDescriptionsAndLabels(Item item, Entity entity)
        {
            if ( item == null )
                throw new ArgumentNullException("item");
            if ( entity == null )
                throw new ArgumentNullException("entity");

            item.setDescription("en", entity.GetWikiDataDescription(Language.English));
            // entityById.setDescription("de", testEntity.GetWikiDataDescription(Language.German));
            item.setDescription("th", entity.GetWikiDataDescription(Language.Thai));
            item.setLabel("en", entity.english);
            item.setLabel("th", entity.FullName);
            item.addAlias("th", entity.AbbreviatedName);
            item.addAlias("th", entity.name);
        }

        /// <summary>
        /// Gets all statements containing subdivision administrative units which are not present before in the item.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <returns>Enumeration of all missing subdivision statements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> or <paramref name="entity"/> is <c>null</c>.</exception>
        public IEnumerable<Statement> MissingContainsAdministrativeDivisionsStatements(Item item, Entity entity)
        {
            if ( item == null )
                throw new ArgumentNullException("item");
            if ( entity == null )
                throw new ArgumentNullException("entity");
            var propertyContainsAdministrativeDivisions = EntityId.newFromPrefixedId(WikiBase.PropertyIdContainsAdministrativeDivisions);
            var result = new List<Statement>();

            var claims = item.Claims.Where(x => x.mainSnak.propertyId.numericId == propertyContainsAdministrativeDivisions.numericId).ToList();
            foreach ( var subEntity in entity.entity )
            {
                if ( (subEntity.wiki != null) && (!String.IsNullOrEmpty(subEntity.wiki.wikidata)) && (!subEntity.IsObsolete) && (!subEntity.type.IsThesaban()) )
                {
                    var subEntityItemId = EntityId.newFromPrefixedId(subEntity.wiki.wikidata);
                    Boolean claimFound = claims.Any(x => (x.mainSnak.dataValue as EntityIdValue).numericId == subEntityItemId.numericId);
                    if ( !claimFound )
                    {
                        var subEntityDataValue = new EntityIdValue("item", subEntityItemId.numericId);
                        var subEntitySnak = new Snak("value", propertyContainsAdministrativeDivisions, subEntityDataValue);
                        var claim = item.createStatementForSnak(subEntitySnak);
                        result.Add(claim);
                    }
                    else
                    {
                        claims.RemoveAll(x => (x.mainSnak.dataValue as EntityIdValue).numericId == subEntityItemId.numericId);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets all statements with the geocodes, including the obsolete codes.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <returns>Enumeration of all missing geocode statements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> or <paramref name="entity"/> is <c>null</c>.</exception>
        public IEnumerable<Statement> MissingGeocodeStatements(Item item, Entity entity)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (entity == null)
                throw new ArgumentNullException("entity");
            var propertyGeocode = EntityId.newFromPrefixedId(WikiBase.PropertyIdThaiGeocode);
            var propertyStatedIn = EntityId.newFromPrefixedId(WikiBase.PropertyIdStatedIn);
            var result = new List<Statement>();

            var claims = item.Claims.Where(x => x.mainSnak.propertyId.numericId == propertyGeocode.numericId).ToList();
                var geocodeValue =entity.geocode.ToString();
                    Boolean claimFound = claims.Any(x => (x.mainSnak.dataValue as StringValue).str == geocodeValue);
                    if (!claimFound)
                    {
                        var geocodeDataValue = new StringValue(geocodeValue);
                        var geocodeSnak = new Snak("value", propertyGeocode, geocodeDataValue);
                        var claim = item.createStatementForSnak(geocodeSnak);
                        if (GeocodeHelper.ProvinceCode(entity.geocode)!=38)
                        {
                            if (entity.type.IsCompatibleEntityType(EntityType.Changwat))
                            {
                        var referenceTIS1099BE2535SnakValue = new EntityIdValue("item",EntityId.newFromPrefixedId(WikiBase.ItemSourceTIS1099BE2535).numericId);
                        var referenceTIS1099BE2535Snak = new Snak("value", propertyStatedIn, referenceTIS1099BE2535SnakValue);
                                claim.createReferenceForSnak(referenceTIS1099BE2535Snak);
                            }
                        var referenceTIS1099BE2548SnakValue = new EntityIdValue("item",EntityId.newFromPrefixedId(WikiBase.ItemSourceTIS1099BE2548).numericId);
                        var referenceTIS1099BE2548Snak = new Snak("value", propertyStatedIn, referenceTIS1099BE2548SnakValue);
                                claim.createReferenceForSnak(referenceTIS1099BE2548Snak);
                        }
                        var referenceCCAATTSnakValue = new EntityIdValue("item",EntityId.newFromPrefixedId(WikiBase.ItemSourceCCAATT).numericId);
                        var referenceCCAATTSnak = new Snak("value", propertyStatedIn, referenceCCAATTSnakValue);
                                claim.createReferenceForSnak(referenceCCAATTSnak);
                        // claim.rank="high";
                        result.Add(claim);
                    }
                    else
                    {
                        claims.RemoveAll(x => (x.mainSnak.dataValue as StringValue).str == geocodeValue);
                    }


            foreach (var oldGeocode in entity.OldGeocodes)
            {
                var oldGeocodeValue =oldGeocode.ToString();
                    claimFound = claims.Any(x => (x.mainSnak.dataValue as StringValue).str == oldGeocodeValue);
                    if (!claimFound)
                    {
                        var subEntityDataValue = new StringValue( oldGeocodeValue);
                        var subEntitySnak = new Snak("value", propertyGeocode, subEntityDataValue);
                        var claim = item.createStatementForSnak(subEntitySnak);
                        result.Add(claim);
                    }
                    else
                    {
                        claims.RemoveAll(x => (x.mainSnak.dataValue as StringValue).str == oldGeocodeValue);
                    }
                }
            
            return result;
        }
    }
}