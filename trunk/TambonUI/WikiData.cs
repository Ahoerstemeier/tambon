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
        private WikiDataHelper _helper;

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

            var noUpgradeHistoryEntry = new List<Entity>();
            foreach ( var entity in allEntities.Where(x => x.type.IsCompatibleEntityType(EntityType.Thesaban) && x.tambonSpecified && !x.IsObsolete) )
            {
                if ( !entity.history.Items.Any(x => x is HistoryStatus) )
                {
                    noUpgradeHistoryEntry.Add(entity);
                }
            }
            noUpgradeHistoryEntry.Sort((x, y) => x.geocode.CompareTo(y.geocode));
            if ( noUpgradeHistoryEntry.Any() )
            {
                builder.AppendFormat("No history ({0}):", noUpgradeHistoryEntry.Count);
                builder.AppendLine();
                foreach ( var entity in noUpgradeHistoryEntry )
                {
                    builder.AppendFormat("{0}: {1}", entity.geocode, entity.english);
                    builder.AppendLine();
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
            allEntities.AddRange(entities.FlatList().Where(x => !x.IsObsolete));
            entities.PropagatePostcodeRecursive();

            var allThesaban = allEntities.Where(x => x.type.IsCompatibleEntityType(EntityType.Thesaban));
            foreach ( var entity in allThesaban )
            {
                if ( (entity.wiki == null) || String.IsNullOrEmpty(entity.wiki.wikidata) )
                {
                    var office = entity.office.FirstOrDefault();
                    if ( office == null )
                    {
                        edtCollisions.Text += "No office: " + entity.geocode.ToString() + Environment.NewLine;
                    }
                    else if ( (office.wiki != null) && !String.IsNullOrEmpty(office.wiki.wikidata) )
                    {
                        edtCollisions.Text += "Wikidata at office: " + entity.geocode.ToString() + Environment.NewLine;
                    }
                }
            }

            var allTambon = allEntities.Where(x => x.type == EntityType.Tambon).ToList();
            foreach ( var tambon in allTambon )
            {
                var localGovernmentEntity = tambon.CreateLocalGovernmentDummyEntity();
                if ( localGovernmentEntity != null )
                {
                    allEntities.Add(localGovernmentEntity);
                }
            }

            cbxChangwat.Items.AddRange(allEntities.Where(x=> x.type.IsCompatibleEntityType(EntityType.Changwat)).ToArray());
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            var entityTypes = CurrentActiveEntityTypes();
            var entitiesWithWikiData = allEntities.Where(x => x.wiki != null && !String.IsNullOrEmpty(x.wiki.wikidata));
            var workItems = entitiesWithWikiData.Where(x => entityTypes.Contains(x.type));
            Int32 startingValue = 0;
            if ( !String.IsNullOrWhiteSpace(edtStartingItemId.Text) )
            {
                startingValue = Convert.ToInt32(edtStartingItemId.Text);
            }
            if ( startingValue > 0 )
            {
                workItems = workItems.Where(x => x.wiki.NumericalWikiData > startingValue);
            }
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
            _helper = new WikiDataHelper(api);
            _bot = new WikiDataBot(_helper);

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
            btnCreate.Enabled = false;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            //WikibaseApi api = new WikibaseApi("https://test.wikidata.org", "TambonBot");
            //// Login with username and password
            //var username = ConfigurationManager.AppSettings["WikiDataUsername"];
            //var password = ConfigurationManager.AppSettings["WikiDataPassword"];
            //api.login(username, password);
            //var provider = new EntityProvider(api);
            //var entity = provider.getEntityFromId(new EntityId("q", 281));
            //var statement = entity.Claims.First() as Statement;
            //var qualifier = new Qualifier(statement, SnakType.Value, new EntityId("P11"), new StringValue("abc"));
            //qualifier.Save("Qualifier save");

            var tis1099thesaban = new List<UInt32>() {
            1195,
1196,
1197,
1198,
1199,
1295,
1296,
1297,
1298,
1299,
1396,
1397,
1398,
1399,
1496,
1497,
1498,
1499,
1598,
1599,
1697,
1698,
1699,
1799,
1898,
1899,
1996,
1997,
1998,
1999,
2093,
2094,
2095,
2096,
2097,
2098,
2099,
2197,
2198,
2199,
2297,
2298,
2299,
2399,
2498,
2499,
2598,
2599,
2699,
2798,
2799,
3096,
3097,
3098,
3099,
3198,
3199,
3299,
3399,
3497,
3498,
3499,
3599,
3699,
3799,
3999,
4096,
4097,
4098,
4099,
4199,
4299,
4399,
4499,
4599,
4699,
4799,
4899,
4999,
5099,
5199,
5299,
5398,
5399,
5499,
5599,
5699,
5799,
5899,
6097,
6098,
6099,
6199,
6299,
6398,
6399,
6498,
6499,
6599,
6697,
6698,
6699,
6798,
6799,
7097,
7098,
7099,
7198,
7199,
7298,
7299,
7399,
7497,
7498,
7499,
7598,
7599,
7698,
7699,
7798,
7799,
8097,
8098,
8099,
8199,
8298,
8299,
8398,
8399,
8497,
8498,
8499,
8599,
8698,
8699,
9096,
9097,
9098,
9099,
9199,
9297,
9298,
9299,
9399,
9498,
9499,
9598,
9599,
9698,
9699,
};

            var entites = allEntities.Where(x => tis1099thesaban.Contains(x.geocode));
            foreach (var entity in entites)
            {
                var item = _helper.GetWikiDataItemForEntity(entity);
                if (_helper.GeocodeCorrect(item, entity) == WikiDataState.NotSet)
                {
                    var statement = _helper.SetGeocode(item, entity, false);
                    if (statement != null)
                    {
                        statement.save(_helper.GetClaimSaveEditSummary(statement));
                        var snak = new Snak(SnakType.Value, new EntityId(WikiBase.PropertyIdStatedIn), new EntityIdValue(new EntityId(WikiBase.ItemSourceTIS1099BE2548)));
                        var reference = statement.CreateReferenceForSnak(snak);
                        reference.Save(_helper.GetReferenceSaveEditSummary(reference));
                    }
                }
            }
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            var entityTypes = new List<EntityType>()
            {
            EntityType.Amphoe,
            EntityType.Thesaban,
            EntityType.TAO,
            EntityType.Tambon,
            EntityType.Muban,
            };
            foreach ( var entity in allEntities.Where(x => x.type == EntityType.Changwat) )
            {
                foreach ( var entityType in entityTypes )
                {
                    var wikiDataItem = _helper.FindThaiCategory(entity, entityType);
                    if ( wikiDataItem != null )
                    {
                        _helper.AddCategoryCombinesTopic(wikiDataItem, entity, entityType);
                        _helper.AddCategoryListOf(wikiDataItem, entity, entityType);
                    }
                }
            }
        }

        private void btnAllItems_Click(object sender, EventArgs e)
        {
            var entitiesWithWikiData = allEntities.Where(x => x.wiki != null && !String.IsNullOrEmpty(x.wiki.wikidata));
            var wikiDataLinks = new List<String>();
            wikiDataLinks.AddRange(entitiesWithWikiData.Select(x => x.wiki.wikidata));
            edtCollisions.Text = String.Join(Environment.NewLine, wikiDataLinks);
        }

        private void cbxChangwat_SelectedValueChanged(object sender, EventArgs e)
        {
            var changwat = cbxChangwat.SelectedItem as Entity;
            cbxAmphoe.Items.Clear();
            cbxAmphoe.SelectedItem = null;
            if (changwat != null)
            {
                cbxAmphoe.Items.AddRange(changwat.entity.Where(x => !x.IsObsolete && x.type.IsCompatibleEntityType(EntityType.Amphoe)).ToArray());
            }
        }

        private void cbxAmphoe_SelectedValueChanged(object sender, EventArgs e)
        {
            var amphoe = cbxAmphoe.SelectedItem as Entity;
            lblTambonInfo.Text = String.Empty;
            if (amphoe != null)
            {
                var allTambon = amphoe.entity.Where(x => x.type.IsCompatibleEntityType(EntityType.Tambon) && !x.IsObsolete).ToList();
                var wikidataCount = allTambon.Count(x => x.wiki != null && !String.IsNullOrWhiteSpace(x.wiki.wikidata));
                lblTambonInfo.Text = String.Format("{0} of {1} done",
                    wikidataCount, allTambon.Count);
                btnCreate.Enabled = (wikidataCount < allTambon.Count) && (_bot!=null);
            }
            else
            {
                btnCreate.Enabled = false;
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            var amphoe = cbxAmphoe.SelectedItem as Entity;
            if (amphoe != null)
            {
                var allTambon = amphoe.entity.Where(x => x.type.IsCompatibleEntityType(EntityType.Tambon) && !x.IsObsolete);
                var missingTambon =allTambon.Where(x => x.wiki == null || String.IsNullOrWhiteSpace(x.wiki.wikidata)).ToList();
                foreach (var tambon in missingTambon)
                {
                    _bot.CreateItem(tambon);
                    edtCollisions.Text += String.Format("{0}: {1} ({2})\n",
                        tambon.geocode,
                        tambon.wiki.wikidata,
                        tambon.english);
                }
                var dummy = new StringBuilder();
                _bot.SetContainsSubdivisionTask.Task(new List<Entity>(){amphoe},dummy,false);
            }

        }
    }
}