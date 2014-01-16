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
using Wikibase.DataValues;

namespace De.AHoerstemeier.Tambon.UI
{
    public partial class WikiData : Form
    {
        private WikiDataBot _bot;

        private List<Entity> allEntities = null;

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
            var entitiesWithWikiData = allEntities.Where(x => x.wiki != null && !String.IsNullOrEmpty(x.wiki.wikidata));
            var wikiDataLinks = new List<String>();
            wikiDataLinks.AddRange(entitiesWithWikiData.Select(x => x.wiki.wikidata));

            var allOffices = allEntities.SelectMany(x => x.office);
            //var officesWithWikiData = allOffices.Where(y => y.wiki != null && !String.IsNullOrEmpty(y.wiki.wikidata));
            //wikiDataLinks.AddRange(officesWithWikiData.Select(x => x.wiki.wikidata));

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

            //var officesWithWikiDataByType = officesWithWikiData.GroupBy(x => x.type).OrderBy(y => y.Count());
            //foreach ( var type in officesWithWikiDataByType )
            //{
            //    builder.AppendFormat("{0}: {1}", type.Key, type.Count());
            //    builder.AppendLine();
            //}
            //builder.AppendLine();

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
                String.Format("Wikidata coverage ({0})", entitiesWithWikiData.Count()),
                result);
            formWikiDataEntries.Show();
        }

        private void btnCountInterwiki_Click(object sender, EventArgs e)
        {
            var entityTypes = CurrentActiveEntityTypes();
            var entitiesWithWikiData = allEntities.Where(x => x.wiki != null && !String.IsNullOrEmpty(x.wiki.wikidata));
            var workItems = entitiesWithWikiData.Where(x => entityTypes.Contains(x.type));

            StringBuilder collisions = new StringBuilder();
            var siteLinkCount = _bot.CountSiteLinks(workItems, collisions);

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} entities on Wikidata", workItems.Count());
            builder.AppendLine();
            foreach ( var value in siteLinkCount )
            {
                builder.AppendFormat("  {0}: {1}", value.Key, value.Value);
                builder.AppendLine();
            }
            var result = builder.ToString();
            edtCollisions.Text = collisions.ToString();

            var formWikiDataEntries = new StringDisplayForm(
                "Wikidata language coverage",
                result);
            formWikiDataEntries.Show();
        }

        private static WikibaseApi OpenConnection()
        {
            WikibaseApi api = new WikibaseApi("https://www.wikidata.org", "TambonBot");
            // Login with username and password
            var username = ConfigurationManager.AppSettings["WikiDataUsername"];
            var password = ConfigurationManager.AppSettings["WikiDataPassword"];

            api.login(username, password);
            api.botEdits = true;
            api.editlimit = true;
            api.editLaps = 1000;  // one edit per second
            return api;
        }

        private void WikiData_Load(object sender, EventArgs e)
        {
            chkTypes.Items.Add(EntityType.Changwat);
            chkTypes.Items.Add(EntityType.Amphoe);
            chkTypes.Items.Add(EntityType.Tambon);
            chkTypes.Items.Add(EntityType.Muban);
            chkTypes.Items.Add(EntityType.Thesaban);
            chkTypes.Items.Add(EntityType.TAO);
            chkTypes.Items.Add(EntityType.Khet);
            chkTypes.Items.Add(EntityType.Khwaeng);
            chkTypes.Items.Add(EntityType.Chumchon);
            chkTypes.SetItemCheckState(0, CheckState.Checked);

            allEntities = new List<Entity>();
            var entities = GlobalData.CompleteGeocodeList();
            allEntities.AddRange(entities.FlatList());
            var allTambon = allEntities.Where(x => x.type == EntityType.Tambon).ToList();
            foreach ( var tambon in allTambon )
            {
                var localGovernmentEntity = tambon.CreateLocalGovernmentDummyEntity();
                if ( localGovernmentEntity != null )
                {
                    allEntities.Add(localGovernmentEntity);
                }
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            var entityTypes = CurrentActiveEntityTypes();
            var entitiesWithWikiData = allEntities.Where(x => x.wiki != null && !String.IsNullOrEmpty(x.wiki.wikidata));
            var workItems = entitiesWithWikiData.Where(x => entityTypes.Contains(x.type));

            StringBuilder warnings = new StringBuilder();

            var activity = cbxActivity.SelectedItem as WikiDataTaskInfo;
            if ( activity != null )
            {
                activity.Task(workItems, warnings, chkOverride.Checked);
            }

            StringBuilder info = new StringBuilder();
            info.AppendFormat("{0} items", workItems.Count());
            info.AppendLine();
            foreach ( var keyvaluepair in _bot.RunInfo )
            {
                if ( keyvaluepair.Value > 0 )
                {
                    info.AppendFormat("{0} items had state {1}", keyvaluepair.Value, keyvaluepair.Key);
                    info.AppendLine();
                }
            }
            info.AppendLine();

            edtCollisions.Text = info.ToString() + warnings.ToString();
        }

        private IEnumerable<EntityType> CurrentActiveEntityTypes()
        {
            var entityTypes = new List<EntityType>();
            foreach ( var item in chkTypes.CheckedItems )
            {
                entityTypes.Add((EntityType)item);
            }
            if ( entityTypes.Contains(EntityType.Thesaban) )
            {
                entityTypes.Add(EntityType.ThesabanTambon);
                entityTypes.Add(EntityType.ThesabanMueang);
                entityTypes.Add(EntityType.ThesabanNakhon);
            }
            return entityTypes;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var api = OpenConnection();
            var helper = new WikiDataHelper(api);
            _bot = new WikiDataBot(helper);

            foreach ( var activity in _bot.AvailableTasks )
            {
                cbxActivity.Items.Add(activity);
            }
            btnRun.Enabled = true;
            btnLogout.Enabled = true;
            btnLogin.Enabled = false;
            btnCountInterwiki.Enabled = true;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            _bot.LogOut();
            _bot = null;

            btnRun.Enabled = false;
            btnLogout.Enabled = false;
            btnLogin.Enabled = true;
            btnCountInterwiki.Enabled = false;
        }
    }
}