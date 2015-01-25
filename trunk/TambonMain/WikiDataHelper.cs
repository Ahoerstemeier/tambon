using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
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
        #region fields

        private IEnumerable<Entity> _allEntities;
        private EntityProvider _entityProvider;

        #endregion fields

        #region properties

        /// <summary>
        /// Gets the WikiData writing API encapsulation.
        /// </summary>
        /// <value>The WikiData writing API encapsulation.</value>
        public WikibaseApi Api
        {
            get;
            private set;
        }

        #endregion properties

        #region constructor

        /// <summary>
        /// Creates a new instance of <see cref="WikiDataHelper"/>.
        /// </summary>
        /// <param name="api">The WikiData API encapsulation.</param>
        public WikiDataHelper(WikibaseApi api)
        {
            if ( api == null )
                throw new ArgumentNullException("api");
            Api = api;
            _entityProvider = new EntityProvider(Api);

            var entities = GlobalData.CompleteGeocodeList();
            _allEntities = entities.FlatList();
        }

        #endregion constructor

        #region private methods

        private WikiDataState CheckStringValue(Item item, String propertyId, String expected, Boolean createStatement, Boolean overrideWrongData, out Statement statement)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            WikiDataState result = WikiDataState.Unknown;

            // Statement claim = item.Claims.FirstOrDefault(x => x.IsAboutProperty(WikiBase.PropertyIdCountry)) as Statement;
            var property = new EntityId(propertyId);
            Statement claim = item.Claims.FirstOrDefault(x => property.Equals(x.mainSnak.PropertyId)) as Statement;

            var dataValue = new StringValue(expected);
            var snak = new Snak(SnakType.Value, new EntityId(propertyId), dataValue);
            if ( claim == null )
            {
                if ( String.IsNullOrEmpty(expected) )
                {
                    result = WikiDataState.Valid;
                }
                else
                {
                    result = WikiDataState.NotSet;
                    if ( createStatement )
                    {
                        claim = item.createStatementForSnak(snak);
                    }
                }
            }
            else
            {
                Snak oldSnak = claim.mainSnak;
                var oldDataValue = oldSnak.DataValue as StringValue;
                if ( oldDataValue.Value == dataValue.Value )
                {
                    result = WikiDataState.Valid;
                }
                else
                {
                    if ( !String.IsNullOrEmpty(expected) )
                    {
                        result = WikiDataState.WrongValue;
                        if ( overrideWrongData )
                        {
                            claim.mainSnak = snak;
                        }
                    }
                    else
                    {
                        result = WikiDataState.DataMissing;
                    }
                }
            }

            statement = claim as Statement;
            return result;
        }

        private WikiDataState CheckPropertyValue(Item item, String propertyId, String expectedItemId, Boolean createStatement, Boolean overrideWrongData, out Statement statement)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            WikiDataState result = WikiDataState.Unknown;

            // Statement claim = item.Claims.FirstOrDefault(x => x.IsAboutProperty(WikiBase.PropertyIdCountry)) as Statement;
            var property = new EntityId(propertyId);
            Statement claim = item.Claims.FirstOrDefault(x => property.Equals(x.mainSnak.PropertyId)) as Statement;

            var entity = new EntityId(expectedItemId);
            var dataValue = new EntityIdValue(entity);

            var snak = new Snak(SnakType.Value, new EntityId(propertyId), dataValue);
            if ( claim == null )
            {
                if ( String.IsNullOrEmpty(expectedItemId) )
                {
                    result = WikiDataState.Valid;
                }
                else
                {
                    result = WikiDataState.NotSet;
                    if ( createStatement )
                    {
                        claim = item.createStatementForSnak(snak);
                    }
                }
            }
            else
            {
                Snak oldSnak = claim.mainSnak;
                var oldDataValue = oldSnak.DataValue as EntityIdValue;
                if ( oldDataValue.NumericId == dataValue.NumericId )
                {
                    result = WikiDataState.Valid;
                }
                else
                {
                    if ( !String.IsNullOrEmpty(expectedItemId) )
                    {
                        result = WikiDataState.WrongValue;
                        if ( overrideWrongData )
                        {
                            claim.mainSnak = snak;
                        }
                    }
                    else
                    {
                        result = WikiDataState.DataMissing;
                    }
                }
            }

            statement = claim as Statement;
            return result;
        }

        private WikiDataState CheckPropertyMultiValue(Item item, String propertyId, String expectedItemId, Boolean createStatement, out Statement statement)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            WikiDataState result = WikiDataState.Unknown;

            if ( String.IsNullOrEmpty(expectedItemId) )
            {
                statement = null;
                return result;  // TODO better handling!
            }
            var entity = new EntityId(expectedItemId);
            var dataValue = new EntityIdValue(entity);
            var snak = new Snak(SnakType.Value, new EntityId(propertyId), dataValue);

            var property = new EntityId(propertyId);
            Statement foundStatement = null;
            foreach ( var claim in item.Claims.Where(x => property.Equals(x.mainSnak.PropertyId)) )
            {
                Snak oldSnak = claim.mainSnak;
                var oldDataValue = oldSnak.DataValue as EntityIdValue;
                if ( oldDataValue.NumericId == dataValue.NumericId )
                {
                    foundStatement = claim as Statement;
                    result = WikiDataState.Valid;
                }
            }

            if ( foundStatement == null )
            {
                if ( String.IsNullOrEmpty(expectedItemId) )
                {
                    result = WikiDataState.Valid;
                }
                else
                {
                    result = WikiDataState.Incomplete;
                    if ( createStatement )
                    {
                        foundStatement = item.createStatementForSnak(snak);
                    }
                }
            }

            statement = foundStatement;
            return result;
        }

        private WikiDataState CheckStringMultiValue(Item item, String propertyId, String expectedValue, Boolean createStatement, out Statement statement)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            WikiDataState result = WikiDataState.Unknown;

            if ( String.IsNullOrEmpty(expectedValue) )
            {
                statement = null;
                return result;  // TODO better handling!
            }
            var dataValue = new StringValue(expectedValue);
            var snak = new Snak(SnakType.Value, new EntityId(propertyId), dataValue);

            var property = new EntityId(propertyId);
            Statement foundStatement = null;
            foreach ( var claim in item.Claims.Where(x => property.Equals(x.mainSnak.PropertyId)) )
            {
                Snak oldSnak = claim.mainSnak;
                var oldDataValue = oldSnak.DataValue as StringValue;
                if ( oldDataValue.Value == dataValue.Value )
                {
                    foundStatement = claim as Statement;
                    result = WikiDataState.Valid;
                }
            }

            if ( foundStatement == null )
            {
                if ( String.IsNullOrEmpty(expectedValue) )
                {
                    result = WikiDataState.Valid;
                }
                else
                {
                    result = WikiDataState.Incomplete;
                    if ( createStatement )
                    {
                        foundStatement = item.createStatementForSnak(snak);
                    }
                }
            }

            statement = foundStatement;
            return result;
        }

        private WikiDataState CheckMonoLanguageValue(Item item, String propertyId, Language language, String expectedValue, Boolean createStatement, out Statement statement)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            WikiDataState result = WikiDataState.Unknown;

            if ( String.IsNullOrEmpty(expectedValue) )
            {
                statement = null;
                return result;  // TODO better handling!
            }
            var languageCode = language.ToCode();
            var dataValue = new MonolingualTextValue(expectedValue, languageCode);
            var snak = new Snak(SnakType.Value, new EntityId(propertyId), dataValue);

            var property = new EntityId(propertyId);
            Statement foundStatement = null;
            foreach ( var claim in item.Claims.Where(x => property.Equals(x.mainSnak.PropertyId)) )
            {
                Snak oldSnak = claim.mainSnak;
                var oldDataValue = oldSnak.DataValue as MonolingualTextValue;
                if ( (oldDataValue.Text == dataValue.Text) && (oldDataValue.Language == dataValue.Language) )
                {
                    foundStatement = claim as Statement;
                    result = WikiDataState.Valid;
                }
            }

            if ( foundStatement == null )
            {
                if ( String.IsNullOrEmpty(expectedValue) )
                {
                    result = WikiDataState.Valid;
                }
                else
                {
                    result = WikiDataState.Incomplete;
                    if ( createStatement )
                    {
                        foundStatement = item.createStatementForSnak(snak);
                    }
                }
            }

            statement = foundStatement;
            return result;
        }

        private WikiDataState CheckCoordinateValue(Item item, String propertyId, Point expected, Boolean createStatement, Boolean overrideWrongData, out Statement statement)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            WikiDataState result = WikiDataState.Unknown;

            // Statement claim = item.Claims.FirstOrDefault(x => x.IsAboutProperty(WikiBase.PropertyIdCountry)) as Statement;
            var property = new EntityId(propertyId);
            Statement claim = item.Claims.FirstOrDefault(x => property.Equals(x.mainSnak.PropertyId)) as Statement;

            var dataValue = new GlobeCoordinateValue(Convert.ToDouble(expected.lat), Convert.ToDouble(expected.@long), 0.0001, Globe.Earth);
            var snak = new Snak(SnakType.Value, new EntityId(propertyId), dataValue);
            if ( claim == null )
            {
                result = WikiDataState.NotSet;
                if ( createStatement )
                {
                    claim = item.createStatementForSnak(snak);
                }
            }
            else
            {
                Snak oldSnak = claim.mainSnak;
                var oldDataValue = oldSnak.DataValue as GlobeCoordinateValue;
                if ( (Math.Abs(oldDataValue.Latitude - dataValue.Latitude) < dataValue.Precision) &&
                    (Math.Abs(oldDataValue.Longitude - dataValue.Longitude) < dataValue.Precision) )
                {
                    result = WikiDataState.Valid;
                }
                else
                {
                    result = WikiDataState.WrongValue;
                    if ( overrideWrongData )
                    {
                        claim.mainSnak = snak;
                    }
                }
            }

            statement = claim as Statement;
            return result;
        }

        /// <summary>
        /// Gets the location as a geopoint.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <param name="propertyId">Id of the property.</param>
        /// <returns>Location as geopoint, or <c>null</c> if none is set.</returns>
        public GeoCoordinate GetCoordinateValue(Item item, String propertyId)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            // Statement claim = item.Claims.FirstOrDefault(x => x.IsAboutProperty(WikiBase.PropertyIdCountry)) as Statement;
            var property = new EntityId(propertyId);
            Statement claim = item.Claims.FirstOrDefault(x => property.Equals(x.mainSnak.PropertyId)) as Statement;
            if ( claim != null )
            {
                Snak oldSnak = claim.mainSnak;
                if ( oldSnak != null )
                {
                    var oldDataValue = oldSnak.DataValue as GlobeCoordinateValue;
                    if ( oldDataValue != null )
                    {
                        return new GeoCoordinate(oldDataValue.Latitude, oldDataValue.Longitude);
                    }
                }
            }
            return null;
        }

        #endregion private methods

        #region public methods

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

            Item entityById = _entityProvider.getEntityFromId(new EntityId(entity.wiki.wikidata)) as Item;
            return entityById;
        }

        #region IsInAdministrative

        private WikiDataState IsInAdministrativeUnit(Item item, Entity entity, Boolean createStatement, Boolean overrideWrongData, out Statement statement)
        {
            Entity parentEntity = null;
            if ( entity.type == EntityType.Chumchon )
            {
                // Between Chumchon and Thesaban there is one level without numbers, at least so far
                parentEntity = _allEntities.FirstOrDefault(x => x.geocode == entity.geocode / 100);
                if ( parentEntity == null )
                {
                    parentEntity = _allEntities.FirstOrDefault(x => x.geocode == entity.geocode / 10000);
                }
            }
            else if ( entity.type.IsLocalGovernment() )
            {
                var parentGeocode = entity.parent.FirstOrDefault();
                if ( parentGeocode == 0 )
                {
                    parentGeocode = entity.geocode / 100;
                }
                parentEntity = _allEntities.FirstOrDefault(x => x.geocode == parentGeocode);
            }
            else
            {
                parentEntity = _allEntities.FirstOrDefault(x => x.geocode == entity.geocode / 100);
            }
            if ( (parentEntity != null) && (parentEntity.wiki != null) && (!String.IsNullOrEmpty(parentEntity.wiki.wikidata)) )
            {
                var parent = parentEntity.wiki.wikidata;
                return CheckPropertyValue(item, WikiBase.PropertyIdIsInAdministrativeUnit, parent, createStatement, overrideWrongData, out statement);
            }
            else
            {
                statement = null;
                return WikiDataState.Unknown;
            }
        }

        /// <summary>
        /// Gets the statement containing the parent administrative unit.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <returns>Statement containing the parent administrative unit.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> or <paramref name="entity"/> is <c>null</c>.</exception>
        public Statement SetIsInAdministrativeUnit(Item item, Entity entity, Boolean overrideWrongData)
        {
            if ( item == null )
                throw new ArgumentNullException("item");
            if ( entity == null )
                throw new ArgumentNullException("entity");

            Statement result;
            IsInAdministrativeUnit(item, entity, true, overrideWrongData, out result);

            return result;
        }

        /// <summary>
        /// Checks if the statement containing the parent administrative unit is set correctly.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <returns>Statement containing the parent administrative unit.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> or <paramref name="entity"/> is <c>null</c>.</exception>
        public WikiDataState IsInAdministrativeUnitCorrect(Item item, Entity entity)
        {
            if ( item == null )
                throw new ArgumentNullException("item");
            if ( entity == null )
                throw new ArgumentNullException("entity");

            Statement dummy;
            return IsInAdministrativeUnit(item, entity, false, false, out dummy);
        }

        #endregion IsInAdministrative

        #region TypeOfAdministrativeUnit

        private WikiDataState TypeOfAdministrativeUnit(Item item, Entity entity, Boolean createStatement, Boolean overrideWrongData, out Statement statement)
        {
            var entityType = WikiBase.WikiDataItems[entity.type];
            return CheckPropertyValue(item, WikiBase.PropertyIdInstanceOf, entityType, createStatement, overrideWrongData, out statement);
        }

        /// <summary>
        /// Gets the statement containing the type of administrative unit.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <returns>Statement containing the type of administrative unit.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> or <paramref name="entity"/> is <c>null</c>.</exception>
        public Statement SetTypeOfAdministrativeUnit(Item item, Entity entity, Boolean overrideWrongData)
        {
            if ( item == null )
                throw new ArgumentNullException("item");
            if ( entity == null )
                throw new ArgumentNullException("entity");

            Statement result;
            TypeOfAdministrativeUnit(item, entity, true, overrideWrongData, out result);

            return result;
        }

        /// <summary>
        /// Checks if the statement containing the type of administrative unit is correct.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <returns>Statement containing the type of administrative unit.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> or <paramref name="entity"/> is <c>null</c>.</exception>
        public WikiDataState TypeOfAdministrativeUnitCorrect(Item item, Entity entity)
        {
            if ( item == null )
                throw new ArgumentNullException("item");
            if ( entity == null )
                throw new ArgumentNullException("entity");

            Statement dummy;
            return TypeOfAdministrativeUnit(item, entity, false, false, out dummy);
        }

        #endregion TypeOfAdministrativeUnit

        #region IsInCountry

        private WikiDataState IsInCountry(Item item, Boolean createStatement, Boolean overrideWrongData, out Statement statement)
        {
            var parent = WikiBase.WikiDataItems[EntityType.Country];
            return CheckPropertyValue(item, WikiBase.PropertyIdCountry, parent, createStatement, overrideWrongData, out statement);
        }

        /// <summary>
        /// Gets the statement containing the country.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="overrideWrongData"><c>true</c> is a wrong claim should be overwritten, <c>false</c> otherwise.</param>
        /// <returns>Statement containing the country.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public Statement SetIsInCountry(Item item, Boolean overrideWrongData)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            Statement result;
            IsInCountry(item, true, overrideWrongData, out result);
            return result;
        }

        /// <summary>
        /// Gets whether the statement containing the country is set correctly.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public WikiDataState IsInCountryCorrect(Item item)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            Statement dummy;
            return IsInCountry(item, false, false, out dummy);
        }

        #endregion IsInCountry

        #region OpenStreetMapId

        private WikiDataState OpenStreetMap(Item item, Entity entity, Boolean createStatement, Boolean overrideWrongData, out Statement statement)
        {
            var stringValue = String.Empty;
            if ( entity.wiki.openstreetmapSpecified )
            {
                stringValue = entity.wiki.openstreetmap.ToString(CultureInfo.InvariantCulture);
            }
            return CheckStringValue(item, WikiBase.PropertyIdOpenStreetMap, stringValue, createStatement, overrideWrongData, out statement);
        }

        /// <summary>
        /// Gets the statement containing the open street map id.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <param name="overrideWrongData"><c>true</c> is a wrong claim should be overwritten, <c>false</c> otherwise.</param>
        /// <returns>Statement containing the country.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public Statement SetOpenStreetMap(Item item, Entity entity, Boolean overrideWrongData)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            Statement result;
            OpenStreetMap(item, entity, true, overrideWrongData, out result);
            return result;
        }

        /// <summary>
        /// Gets whether the statement containing the open street map id is set correctly.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public WikiDataState OpenStreetMapCorrect(Item item, Entity entity)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            Statement dummy;
            return OpenStreetMap(item, entity, false, false, out dummy);
        }

        #endregion OpenStreetMapId

        #region Geocode

        private WikiDataState Geocode(Item item, Entity entity, Boolean createStatement, Boolean overrideWrongData, out Statement statement)
        {
            var stringValue = String.Empty;
            stringValue = entity.geocode.ToString(CultureInfo.InvariantCulture);
            return CheckStringValue(item, WikiBase.PropertyIdThaiGeocode, stringValue, createStatement, overrideWrongData, out statement);
        }

        /// <summary>
        /// Gets the statement containing the geocode.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <param name="overrideWrongData"><c>true</c> is a wrong claim should be overwritten, <c>false</c> otherwise.</param>
        /// <returns>Statement containing the country.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public Statement SetGeocode(Item item, Entity entity, Boolean overrideWrongData)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            Statement result;
            Geocode(item, entity, true, overrideWrongData, out result);
            return result;
        }

        /// <summary>
        /// Gets whether the statement containing the geocode is set correctly.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public WikiDataState GeocodeCorrect(Item item, Entity entity)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            Statement dummy;
            return Geocode(item, entity, false, false, out dummy);
        }

        #endregion Geocode

        #region GND

        private WikiDataState Gnd(Item item, Entity entity, Boolean createStatement, Boolean overrideWrongData, out Statement statement)
        {
            var stringValue = String.Empty;
            stringValue = entity.codes.gnd.value;
            return CheckStringValue(item, WikiBase.PropertyIdGND, stringValue, createStatement, overrideWrongData, out statement);
        }

        /// <summary>
        /// Gets the statement containing the GND reference.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <param name="overrideWrongData"><c>true</c> is a wrong claim should be overwritten, <c>false</c> otherwise.</param>
        /// <returns>Statement containing the country.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public Statement SetGnd(Item item, Entity entity, Boolean overrideWrongData)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            Statement result;
            Gnd(item, entity, true, overrideWrongData, out result);
            return result;
        }

        /// <summary>
        /// Gets whether the statement containing the GND reference is set correctly.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public WikiDataState GndCorrect(Item item, Entity entity)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            Statement dummy;
            return Gnd(item, entity, false, false, out dummy);
        }

        #endregion GND

        #region Location

        private WikiDataState Location(Item item, Entity entity, Boolean createStatement, Boolean overrideWrongData, out Statement statement)
        {
            Point value = null;
            var office = entity.office.FirstOrDefault();
            if ( office != null )
            {
                value = office.Point;
            }
            if ( value != null )
            {
                return CheckCoordinateValue(item, WikiBase.PropertyIdCoordinate, value, createStatement, overrideWrongData, out statement);
            }
            else
            {
                statement = null;
                return WikiDataState.Unknown;
            }
        }

        /// <summary>
        /// Creates the statement containing the location.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <param name="overrideWrongData"><c>true</c> is a wrong claim should be overwritten, <c>false</c> otherwise.</param>
        /// <returns>Statement containing the location.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public Statement SetLocation(Item item, Entity entity, Boolean overrideWrongData)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            Statement result;
            Location(item, entity, true, overrideWrongData, out result);
            return result;
        }

        /// <summary>
        /// Gets whether the statement containing the location is set correctly.
        /// </summary>
        /// <param name="item">The WikiData item.</param>
        /// <param name="entity">The administrative unit.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public WikiDataState LocationCorrect(Item item, Entity entity)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            Statement dummy;
            return Location(item, entity, false, false, out dummy);
        }

        #endregion Location

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
            var entityIdValue = snak.DataValue as EntityIdValue;
            if ( entityIdValue != null )
            {
                result = String.Format("[[Property:P{0}]]: [[Q{1}]]", snak.PropertyId.NumericId, entityIdValue.NumericId);
            }
            var stringValue = snak.DataValue as StringValue;
            if ( stringValue != null )
            {
                result = String.Format("[[Property:P{0}]]: {1}", snak.PropertyId.NumericId, stringValue.Value);
            }
            var coordinateValue = snak.DataValue as GlobeCoordinateValue;
            if ( coordinateValue != null )
            {
                result = String.Format(CultureInfo.InvariantCulture, "[[Property:P{0}]]: {1:#.####}, {2:#.####}", snak.PropertyId.NumericId, coordinateValue.Latitude, coordinateValue.Longitude);
            }
            var quantityValue = snak.DataValue as QuantityValue;
            if ( quantityValue != null )
            {
                if ( (quantityValue.LowerBound == quantityValue.UpperBound) && (quantityValue.Amount == quantityValue.UpperBound) )
                {
                    result = String.Format("[[Property:P{0}]]: {1}", snak.PropertyId.NumericId, quantityValue.Amount);
                }
                // TODO: ± if upper and lower not same
            }
            return result;
        }

        /// <summary>
        /// Get the default edit summary for a reference save.
        /// </summary>
        /// <param name="value">Reference to be parsed.</param>
        /// <returns>Edit summary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        public String GetReferenceSaveEditSummary(Reference value)
        {
            if ( value == null )
                throw new ArgumentNullException("value");

            var result = String.Empty;
            Snak snak = value.Statement.mainSnak;
            if ( String.IsNullOrEmpty(value.Hash) )
            {
                result = String.Format("Added reference to claim: [[Property:P{0}]]", snak.PropertyId.NumericId);
            }
            else
            {
                result = String.Format("Modified reference to claim: [[Property:P{0}]]", snak.PropertyId.NumericId);
            }
            return result;
        }

        /// <summary>
        /// Get the default edit summary for a qualifier save.
        /// </summary>
        /// <param name="value">Qualifier to be parsed.</param>
        /// <returns>Edit summary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        public String GetQualifierSaveEditSummary(Qualifier value)
        {
            if ( value == null )
                throw new ArgumentNullException("value");

            var result = String.Empty;
            // include qualifier info?
            if ( String.IsNullOrEmpty(value.Hash) )
            {
                result = String.Format("‎Added one qualifier of claim: [[Property:P{0}]]", value.Statement.mainSnak.PropertyId.NumericId);
            }
            else
            {
                result = String.Format("‎Changed one qualifier of claim: [[Property:P{0}]]", value.Statement.mainSnak.PropertyId.NumericId);
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

        #endregion public methods

        #region ContainsSubdivisions

        public WikiDataState ContainsSubdivisionsCorrect(Item item, Entity entity, Entity subEntity)
        {
            var expected = String.Empty;
            if ( (subEntity.wiki != null) && (!String.IsNullOrEmpty(subEntity.wiki.wikidata)) )
            {
                expected = subEntity.wiki.wikidata;
            }
            Statement dummy;
            return CheckPropertyMultiValue(item, WikiBase.PropertyIdContainsAdministrativeDivisions, expected, false, out dummy);
        }

        public Statement SetContainsSubdivisions(Item item, Entity entity, Entity subEntity)
        {
            var expected = String.Empty;
            if ( (subEntity.wiki != null) && (!String.IsNullOrEmpty(subEntity.wiki.wikidata)) )
            {
                expected = subEntity.wiki.wikidata;
            }
            Statement result;
            CheckPropertyMultiValue(item, WikiBase.PropertyIdContainsAdministrativeDivisions, expected, true, out result);
            return result;
        }

        #endregion ContainsSubdivisions

        #region PopulationData

        private WikiDataState PopulationData(Item item, PopulationData data, Boolean createStatement, Boolean overrideWrongData, out Statement statement)
        {
            var total = data.data.FirstOrDefault(y => y.type == PopulationDataType.total);
            var propertyName = WikiBase.PropertyIdPopulation;

            WikiDataState result = WikiDataState.Unknown;

            // Statement claim = item.Claims.FirstOrDefault(x => x.IsAboutProperty(WikiBase.PropertyIdCountry)) as Statement;
            var property = new EntityId(propertyName);
            var propertyPointInTime = new EntityId(WikiBase.PropertyIdPointInTime);
            var claimsForProperty = item.Claims.Where(x => property.Equals(x.mainSnak.PropertyId));
            Statement claim = claimsForProperty.FirstOrDefault(
                x => x.Qualifiers.Any(
                    y => y.PropertyId.Equals(propertyPointInTime) &&
                         y.DataValue is TimeValue &&
                         (y.DataValue as TimeValue).DateTime.Year == data.Year)) as Statement;

            var dataValue = new QuantityValue(total.total);
            var snak = new Snak(SnakType.Value, new EntityId(propertyName), dataValue);
            if ( claim == null )
            {
                result = WikiDataState.NotSet;
                if ( createStatement )
                {
                    claim = item.createStatementForSnak(snak);
                }
            }
            else
            {
                Snak oldSnak = claim.mainSnak;
                var oldDataValue = snak.DataValue as QuantityValue;
                if ( oldDataValue.Equals(dataValue) )
                {
                    result = WikiDataState.Valid;
                }
                else
                {
                    result = WikiDataState.WrongValue;
                    if ( overrideWrongData )
                    {
                        claim.mainSnak = snak;
                    }
                }
            }

            statement = claim as Statement;
            return result;
        }

        public Statement SetPopulationData(Item item, PopulationData data, Boolean overrideWrongData)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            Statement result;
            PopulationData(item, data, true, overrideWrongData, out result);
            return result;
        }

        public WikiDataState PopulationDataCorrect(Item item, PopulationData data)
        {
            if ( item == null )
                throw new ArgumentNullException("item");

            Statement dummy;
            return PopulationData(item, data, false, false, out dummy);
        }

        public void AddPopulationDataReferences(Statement statement, PopulationData data, Entity entity)
        {
            Reference reference = null;
            Snak snak;
            switch ( data.source )
            {
                case PopulationDataSourceType.Census:
                    var statedInItem = String.Empty;
                    if ( WikiBase.ItemCensus.Keys.Contains(data.Year) )
                    {
                        statedInItem = WikiBase.ItemCensus[data.Year];
                    }
                    snak = new Snak(SnakType.Value, new EntityId(WikiBase.PropertyIdStatedIn), new EntityIdValue(new EntityId(statedInItem)));
                    reference = statement.CreateReferenceForSnak(snak);
                    foreach ( var refItem in data.reference )
                    {
                        var urlReference = refItem as MyUri;
                        if ( urlReference != null )
                        {
                            reference.AddSnak(new Snak(SnakType.Value, new EntityId(WikiBase.PropertyIdReferenceUrl), new StringValue(urlReference.Value)));
                        }
                    }
                    break;

                case PopulationDataSourceType.DOPA:
                    Uri source = PopulationDataDownloader.GetDisplayUrl(data.Year, entity.geocode);
                    snak = new Snak(SnakType.Value, new EntityId(WikiBase.PropertyIdReferenceUrl), new StringValue(source.AbsoluteUri));
                    reference = statement.CreateReferenceForSnak(snak);
                    reference.AddSnak(new Snak(SnakType.Value, new EntityId(WikiBase.PropertyIdPublisher), new EntityIdValue(new EntityId(WikiBase.ItemDopa))));
                    break;
            }

            if ( reference != null )
            {
                statement.AddReference(reference);
            }
        }

        public void AddPopulationDataQualifiers(Statement statement, PopulationData data)
        {
            var pointInTimeQualifier = new Qualifier(statement, SnakType.Value, new EntityId(WikiBase.PropertyIdPointInTime), TimeValue.DateValue(data.referencedate));
            statement.Qualifiers.Add(pointInTimeQualifier);
        }

        #endregion PopulationData

        #region category

        public Item FindThaiCategory(Entity entity, EntityType entityType)
        {
            var siteLink = "หมวดหมู่:" + entityType.Translate(Language.Thai) + "ในจังหวัด" + entity.name;
            var result = _entityProvider.getEntityFromSitelink("thwiki", siteLink);
            return result as Item;
        }

        public void AddCategoryCombinesTopic(Item item, Entity entity, EntityType entityType)
        {
            if ( !item.Claims.Any(x => x.IsAboutProperty(WikiBase.PropertyIdCategoryCombinesTopic)) )
            {
                var dataValue1 = new EntityIdValue(new EntityId(WikiBase.WikiDataItems[entityType]));
                var snak1 = new Snak(SnakType.Value, new EntityId(WikiBase.PropertyIdCategoryCombinesTopic), dataValue1);
                var claim1 = item.createStatementForSnak(snak1);
                claim1.save(GetClaimSaveEditSummary(claim1));

                var dataValue2 = new EntityIdValue(new EntityId(entity.wiki.wikidata));
                var snak2 = new Snak(SnakType.Value, new EntityId(WikiBase.PropertyIdCategoryCombinesTopic), dataValue2);
                var claim2 = item.createStatementForSnak(snak2);
                claim2.save(GetClaimSaveEditSummary(claim2));
            }
        }

        public void AddCategoryListOf(Item item, Entity entity, EntityType entityType)
        {
            if ( !item.Claims.Any(x => x.IsAboutProperty(WikiBase.PropertyIdIsListOf)) )
            {
                var dataValue = new EntityIdValue(new EntityId(WikiBase.WikiDataItems[entityType]));
                var snak = new Snak(SnakType.Value, new EntityId(WikiBase.PropertyIdIsListOf), dataValue);
                var claim = item.createStatementForSnak(snak);
                claim.save(GetClaimSaveEditSummary(claim));

                var dataValue2 = new EntityIdValue(new EntityId(entity.wiki.wikidata));
                var qualifier = new Qualifier(claim, SnakType.Value, new EntityId(WikiBase.PropertyIdIsInAdministrativeUnit), dataValue2);
                qualifier.Save(GetQualifierSaveEditSummary(qualifier));
            }
        }

        #endregion category

        #region PostalCode

        public WikiDataState PostalCodeCorrect(Item item, Entity entity, UInt32 code)
        {
            var expected = code.ToString(CultureInfo.InvariantCulture);
            Statement dummy;
            return CheckStringMultiValue(item, WikiBase.PropertyIdPostalCode, expected, false, out dummy);
        }

        public Statement SetPostalCode(Item item, Entity entity, UInt32 code)
        {
            var expected = code.ToString(CultureInfo.InvariantCulture);
            Statement result;
            CheckStringMultiValue(item, WikiBase.PropertyIdPostalCode, expected, true, out result);
            return result;
        }

        #endregion PostalCode

        #region Slogan

        public WikiDataState SloganCorrect(Item item, Entity entity)
        {
            var firstSlogan = entity.symbols.slogan.FirstOrDefault();
            if ( firstSlogan != null )
            {
                var expected = firstSlogan.Value;
                Statement dummy;

                return CheckMonoLanguageValue(item, WikiBase.PropertyIdMotto, Language.Thai, expected, false, out dummy);
            }
            else
            {
                return WikiDataState.NoData;
            }
        }

        public Statement SetSlogan(Item item, Entity entity)
        {
            var firstSlogan = entity.symbols.slogan.FirstOrDefault();
            if ( firstSlogan != null )
            {
                var expected = firstSlogan.Value;
                Statement result;

                CheckMonoLanguageValue(item, WikiBase.PropertyIdMotto, Language.Thai, expected, true, out result);
                return result;
            }
            return null;
        }

        #endregion Slogan

        /// <summary>
        /// Get the default edit summary for the creation of a item.
        /// </summary>
        /// <param name="value">New item.</param>
        /// <returns>Edit summary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public String GetItemCreateSaveSummary(Item item)
        {
            if ( item == null )
                throw new ArgumentNullException("item");
            return item.getLabel("en");
        }
    }

    /// <summary>
    /// Possible states of data in WikiData vs. Tambon XML.
    /// </summary>
    public enum WikiDataState
    {
        /// <summary>
        /// Unknown state.
        /// </summary>
        Unknown,

        /// <summary>
        /// Vaue in Wikidata matches data in Tambon XML.
        /// </summary>
        Valid,

        /// <summary>
        /// Value not set in Wikidata.
        /// </summary>
        NotSet,

        /// <summary>
        /// Value in WikiData does not match value in Tambon XML.
        /// </summary>
        WrongValue,

        /// <summary>
        /// List statement in WikiData does not have all values required by XML.
        /// </summary>
        Incomplete,

        /// <summary>
        /// Item ID does not refer to a valid WikiData item.
        /// </summary>
        ItemNotFound,

        /// <summary>
        /// No data in Tambon XML.
        /// </summary>
        NoData,

        /// <summary>
        /// No data in Tambon XML, but value present in WikiData.
        /// </summary>
        DataMissing
    }
}