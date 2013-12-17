using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Wikibase;

namespace De.AHoerstemeier.Tambon.UI
{
    public partial class WikiData : Form
    {
        private const String PropertyIdEntityType = "P132";
        private const String PropertyIdCountry = "P17";
        private const String PropertyIdIsInAdministrativeUnit = "P131";
        private const String PropertyIdCoordinate = "P625";
        private const String PropertyIdContainsAdministrativeDivisions = "P150";
        private const String PropertyIdWebsite = "P856";
        private const String PropertyIdSharesBorderWith = "P47";
        private const String PropertyIdHeadOfGovernment = "P6";
        private const String PropertyIdPostalCode = "P281";
        private const String PropertyIdTwinCity = "P190";
        private const String PropertyIdOpenStreetMap = "P402";
        private const String PropertyIdLocationMap = "P242";
        // const String PropertyIdGeocode = "P...";

        public WikiData()
        {
            InitializeComponent();
        }

        private class EntityTypeGrouping<TKey, TElement> : List<TElement>, IGrouping<TKey, TElement>
        {
            public TKey Key
            {
                get;
                set;
            }
        }

        private void btnStatistics_Click(object sender, EventArgs e)
        {
            var entities = GlobalData.CompleteGeocodeList();
            var allEntities = entities.FlatList();
            var entitiesWithWikiData = allEntities.Where(x => x.wiki != null && !String.IsNullOrEmpty(x.wiki.wikidata));
            var wikiDataLinks = new List<String>();
            wikiDataLinks.AddRange(entitiesWithWikiData.Select(x => x.wiki.wikidata));

            var allOffices = allEntities.SelectMany(x => x.office);
            var officesWithWikiData = allOffices.Where(y => y.wiki != null && !String.IsNullOrEmpty(y.wiki.wikidata));
            wikiDataLinks.AddRange(officesWithWikiData.Select(x => x.wiki.wikidata));

            // write to CSV file?

            var fittingEntitiesByType = entitiesWithWikiData.GroupBy(y => y.type).OrderBy(z => z.Count()).ToList();
            var allEntitiesByType = allEntities.Where(x => !x.IsObsolete).GroupBy(y => y.type);
            foreach ( var expectedType in WikiBase.WikiDataItems )
            {
                if ( expectedType.Key != EntityType.Country )
                {
                    if ( allEntitiesByType.Any(x => x.Key == expectedType.Key) )
                    {
                        if ( !fittingEntitiesByType.Any(x => x.Key == expectedType.Key) )
                        {
                            var emptyEntry = new EntityTypeGrouping<EntityType, Entity>();
                            emptyEntry.Key = expectedType.Key;
                            fittingEntitiesByType.Add(emptyEntry);
                        }
                    }
                }
            }
            StringBuilder builder = new StringBuilder();
            foreach ( var type in fittingEntitiesByType )
            {
                var fittingAllEntities = allEntitiesByType.First(x => x.Key == type.Key);
                var expectedCount = fittingAllEntities.Count();
                var actualCount = type.Count();
                builder.AppendFormat("{0}: {1} of {2}", type.Key, type.Count(), expectedCount);
                if ( actualCount != expectedCount && expectedCount - actualCount < 5 )
                {
                    builder.Append(" (");
                    foreach ( var entry in fittingAllEntities )
                    {
                        if ( !entitiesWithWikiData.Contains(entry) )
                        {
                            builder.AppendFormat("{0},", entry.geocode);
                        }
                    }
                    builder.Append(")");
                }
                builder.AppendLine();
            }

            builder.AppendLine();

            var officesWithWikiDataByType = officesWithWikiData.GroupBy(x => x.type).OrderBy(y => y.Count());
            foreach ( var type in officesWithWikiDataByType )
            {
                builder.AppendFormat("{0}: {1}", type.Key, type.Count());
                builder.AppendLine();
            }
            builder.AppendLine();

            var announcementsWithWikiData = GlobalData.AllGazetteAnnouncements.entry.Where(x => x.wiki != null && !String.IsNullOrEmpty(x.wiki.wikidata));
            if ( announcementsWithWikiData.Any() )
            {
                builder.AppendFormat("Announcements: {0}", announcementsWithWikiData.Count());
                builder.AppendLine();
                builder.AppendLine();
            }
            wikiDataLinks.AddRange(announcementsWithWikiData.Select(x => x.wiki.wikidata));

            var duplicateWikiDataLinks = wikiDataLinks.GroupBy(x => x).Where(y => y.Count() > 1);
            if ( duplicateWikiDataLinks.Any() )
            {
                builder.AppendLine("Duplicate links:");
                foreach ( var wikiDataLink in duplicateWikiDataLinks )
                {
                    builder.AppendLine(wikiDataLink.Key);
                }
            }

            var result = builder.ToString();

            var formWikiDataEntries = new StringDisplayForm(
                String.Format("Wikidata coverage ({0})", officesWithWikiData.Count() + entitiesWithWikiData.Count()),
                result);
            formWikiDataEntries.Show();
        }

        private const String AmphoeVibhavadi = "Q476980";
        private const String SamuiCity = "Q13025347";
        private const String SuratThaniProvince = "Q240463";

        private const String germanWikipediaSiteLink = "dewiki";
        private const String englishWikipediaSiteLink = "enwiki";
        private const String thaiWikipediaSiteLink = "thwiki";

        private void btnCountInterwiki_Click(object sender, EventArgs e)
        {
            WikibaseApi api = OpenConnection();

            // Create a new EntityProvider instance and pass the api created above.
            EntityProvider entityProvider = new EntityProvider(api);

            var entities = GlobalData.CompleteGeocodeList();
            var allEntities = entities.FlatList();
            var entitiesWithWikiData = allEntities.Where(x => x.wiki != null && !String.IsNullOrEmpty(x.wiki.wikidata)).Where(x => x.type == EntityType.Tambon);

            var siteLinkCount = new Dictionary<String, Int32>();
            foreach ( var tambon in entitiesWithWikiData )
            {
                // Get an entity by searching for the id
                var entityById = entityProvider.getEntityFromId(EntityId.newFromPrefixedId(tambon.wiki.wikidata));
                var links = (entityById as Item).getSitelinks();
                foreach ( var key in links.Keys )
                {
                    if ( !siteLinkCount.ContainsKey(key) )
                        siteLinkCount[key] = 0;
                    siteLinkCount[key]++;
                }
            }
            // ... and finally logout
            api.logout();

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} Tambon on Wikidata", entitiesWithWikiData.Count());
            builder.AppendLine();
            foreach ( var value in siteLinkCount )
            {
                builder.AppendFormat("  {0}: {1}", value.Key, value.Value);
                builder.AppendLine();
            }
            var result = builder.ToString();

            var formWikiDataEntries = new StringDisplayForm(
                "Wikidata language coverage",
                result);
            formWikiDataEntries.Show();
        }

        private void btnTestGet_Click(object sender, EventArgs e)
        {
            WikibaseApi api = OpenConnection();

            // Create a new EntityProvider instance and pass the api created above.
            EntityProvider entityProvider = new EntityProvider(api);

            var entityById = entityProvider.getEntityFromId(EntityId.newFromPrefixedId(AmphoeVibhavadi));
            var claimTypeOfAdministration = entityById.Claims.Where(x => x.mainSnak.propertyId.ToString() == PropertyIdEntityType.ToLowerInvariant());
            var firstTypeOfAdministration = claimTypeOfAdministration.FirstOrDefault();

            // ... and finally logout
            api.logout();
        }

        private static WikibaseApi OpenConnection()
        {
            WikibaseApi api = new WikibaseApi("https://www.wikidata.org", "TambonBot");
            // Login with username and password
            var username = ConfigurationManager.AppSettings["WikiDataUsername"];
            var password = ConfigurationManager.AppSettings["WikiDataPassword"];

            api.login(username, password);
            return api;
        }

        private void btnTestSet_Click(object sender, EventArgs e)
        {
            var entities = GlobalData.CompleteGeocodeList();
            var allEntities = entities.FlatList();
            var testEntity = allEntities.First(x => x.geocode == 841902);

            WikibaseApi api = OpenConnection();
            // Create a new EntityProvider instance and pass the api created above.
            EntityProvider entityProvider = new EntityProvider(api);

            var entityById = entityProvider.getEntityFromId(EntityId.newFromPrefixedId(testEntity.wiki.wikidata));

            entityById.setDescription("en", testEntity.GetDescription(Language.English));
            // entityById.setDescription("de", testEntity.GetDescription(Language.German));
            entityById.setDescription("th", testEntity.GetDescription(Language.Thai));

            if ( MessageBox.Show("Really do it?", "Confirm send operation", MessageBoxButtons.YesNo) == DialogResult.Yes )
            {
                entityById.save("Added descriptions");
            }
            api.logout();
        }
    }
}