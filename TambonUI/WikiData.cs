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
            builder.AppendFormat("{0} entities on WikiData", workItems.Count());
            builder.AppendLine();
            foreach ( var value in siteLinkCount )
            {
                builder.AppendFormat("  {0}: {1}", value.Key, value.Value);
                builder.AppendLine();
            }
            var result = builder.ToString();
            edtCollisions.Text = collisions.ToString();

            var formWikiDataEntries = new StringDisplayForm(
                "WikiData language coverage",
                result);
            formWikiDataEntries.Show();
        }

        private static WikibaseApi OpenConnection()
        {
            WikibaseApi api = new WikibaseApi("https://www.wikidata.org", "TambonBot");
            // Login with user name and password
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
            chkTypes.Items.Add(EntityType.PAO);
            chkTypes.SetItemCheckState(0, CheckState.Checked);

            allEntities = new List<Entity>();
            var entities = GlobalData.CompleteGeocodeList();
            entities.PropagateObsoleteToSubEntities();
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
                        edtCollisions.Text += "WikiData at office: " + entity.geocode.ToString() + Environment.NewLine;
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
            var allChangwat = allEntities.Where(x => x.type == EntityType.Changwat).ToList();
            foreach ( var changwat in allChangwat )
            {
                var pao = changwat.CreateLocalGovernmentDummyEntity();
                if ( pao != null )
                {
                    allEntities.Add(pao);
                }
            }
            var localGovernments = allEntities.Where(x => x.type.IsLocalGovernment());
            foreach ( var lao in localGovernments )
            {
                lao.CalculatePostcodeForLocalAdministration(allTambon);
            }

            GlobalData.LoadPopulationData(PopulationDataSourceType.DOPA, GlobalData.PopulationStatisticMaxYear);
            Entity.CalculateLocalGovernmentPopulation(localGovernments, allTambon, PopulationDataSourceType.DOPA, GlobalData.PopulationStatisticMaxYear);

            cbxChangwat.Items.AddRange(allEntities.Where(x => x.type.IsCompatibleEntityType(EntityType.Changwat)).ToArray());
            lblTambonInfo.Text = String.Empty;

            var thesabanMueangWithWikiData = localGovernments.Where(x => x.type == EntityType.ThesabanMueang && (x.wiki == null || String.IsNullOrEmpty(x.wiki.wikidata))).Select(x => String.Format("{0} ({1})", x.english, x.geocode)).ToArray();
            edtCollisions.Text = String.Join(Environment.NewLine, thesabanMueangWithWikiData);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            var entityTypes = CurrentActiveEntityTypes();
            var entitiesWithWikiData = allEntities.Where(x => x.wiki != null && !String.IsNullOrEmpty(x.wiki.wikidata));
            IEnumerable<Entity> workItems = null;
            if ( !String.IsNullOrWhiteSpace(edtSpecificItemId.Text) )
            {
                Int32 specificValue = Convert.ToInt32(edtSpecificItemId.Text);
                workItems = entitiesWithWikiData.Where(x => x.wiki.NumericalWikiData == specificValue);
            }
            else
            {
                workItems = entitiesWithWikiData.Where(x => entityTypes.Contains(x.type));
                Int32 startingValue = 0;
                if ( !String.IsNullOrWhiteSpace(edtStartingItemId.Text) )
                {
                    startingValue = Convert.ToInt32(edtStartingItemId.Text);
                }
                if ( startingValue > 0 )
                {
                    workItems = workItems.Where(x => x.wiki.NumericalWikiData > startingValue);
                }
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
            btnCheckCommonsCategory.Enabled = true;
            RefreshAmphoeSelection();
            CalculateCreateLocalGovernmentEnabled();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            _bot.LogOut();
            _bot = null;

            btnRun.Enabled = false;
            btnLogout.Enabled = false;
            btnLogin.Enabled = true;
            btnCountInterwiki.Enabled = false;
            btnCreateTambon.Enabled = false;
            btnAmphoeCategory.Enabled = false;
            btnMap.Enabled = false;
            btnCheckCommonsCategory.Enabled = false;
        }

        private void btnCheckCommonsCategory_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            var entityTypes = CurrentActiveEntityTypes();
            var entitiesWithWikiData = allEntities.Where(x => x.wiki != null && !String.IsNullOrEmpty(x.wiki.wikidata));
            var workItems = entitiesWithWikiData.Where(x => entityTypes.Contains(x.type));
            foreach ( var changwat in workItems )
            {
                if ( !_bot.CheckCommonsCategory(changwat) )
                {
                    String categoryName = _bot.GetCommonsCategory(changwat);
                    builder.AppendFormat("{0}: {1}", changwat.english, categoryName);
                    builder.AppendLine();
                }
            }
            edtCollisions.Text = builder.ToString();
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
            if ( changwat != null )
            {
                cbxAmphoe.Items.AddRange(changwat.entity.Where(x => !x.IsObsolete && x.type.IsCompatibleEntityType(EntityType.Amphoe)).ToArray());
            }
            cbxAmphoe.SelectedItem = null;
            RefreshAmphoeSelection();
            btnMap.Enabled = changwat != null;
        }

        private void cbxAmphoe_SelectedValueChanged(object sender, EventArgs e)
        {
            RefreshAmphoeSelection();
            RefreshLocalGovernmentSelection();
        }

        private void RefreshLocalGovernmentSelection()
        {
            var amphoe = cbxAmphoe.SelectedItem as Entity;
            cbxLocalGovernments.Items.Clear();
            if ( amphoe != null )
            {
                var allLocalGovernment = allEntities.Where(x => x.type.IsLocalGovernment() && x.parent.Contains(amphoe.geocode)).ToList();
                allLocalGovernment.Sort((x, y) => x.geocode.CompareTo(y.geocode));
                cbxLocalGovernments.Items.AddRange(allLocalGovernment.ToArray());
                cbxLocalGovernments.SelectedItem = null;
                btnCreateLocalGovernment.Enabled = false;
            }
        }

        private void RefreshAmphoeSelection()
        {
            var amphoe = cbxAmphoe.SelectedItem as Entity;
            lblTambonInfo.Text = String.Empty;
            if ( amphoe != null )
            {
                var allTambon = amphoe.entity.Where(x => x.type.IsCompatibleEntityType(EntityType.Tambon) && !x.IsObsolete).ToList();
                var wikidataCount = allTambon.Count(x => x.wiki != null && !String.IsNullOrWhiteSpace(x.wiki.wikidata));
                lblTambonInfo.Text = String.Format("{0} of {1} done",
                    wikidataCount, allTambon.Count);
                btnCreateTambon.Enabled = (wikidataCount < allTambon.Count) && (_bot != null);
                btnAmphoeCategory.Enabled = true;
            }
            else
            {
                btnCreateTambon.Enabled = false;
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            var amphoe = cbxAmphoe.SelectedItem as Entity;
            if ( amphoe != null )
            {
                edtCollisions.Text = String.Empty;
                var allTambon = amphoe.entity.Where(x => x.type.IsCompatibleEntityType(EntityType.Tambon) && !x.IsObsolete);
                var missingTambon = allTambon.Where(x => x.wiki == null || String.IsNullOrWhiteSpace(x.wiki.wikidata)).ToList();
                foreach ( var tambon in missingTambon )
                {
                    _bot.CreateItem(tambon);
                    edtCollisions.Text += String.Format("{0} ({2}): <wiki wikidata=\"{1}\" />",
                        tambon.geocode,
                        tambon.wiki.wikidata,
                        tambon.english) + Environment.NewLine;
                }
                var dummy = new StringBuilder();
                _bot.SetContainsSubdivisionTask.Task(new List<Entity>() { amphoe }, dummy, false);
                RefreshAmphoeSelection();
            }
        }

        private void cbxLocalGovernments_SelectedValueChanged(object sender, EventArgs e)
        {
            CalculateCreateLocalGovernmentEnabled();
        }

        private void CalculateCreateLocalGovernmentEnabled()
        {
            var localGovernment = cbxLocalGovernments.SelectedItem as Entity;
            btnCreateLocalGovernment.Enabled = (_bot != null) && (localGovernment != null) && (localGovernment.wiki == null || String.IsNullOrWhiteSpace(localGovernment.wiki.wikidata));
        }

        private void btnCreateLocalGovernment_Click(object sender, EventArgs e)
        {
            var localGovernment = cbxLocalGovernments.SelectedItem as Entity;
            if ( localGovernment != null )
            {
                _bot.CreateItem(localGovernment);
                edtCollisions.Text = String.Format("{0} ({2}): <wiki wikidata=\"{1}\" />",
                    localGovernment.geocode,
                    localGovernment.wiki.wikidata,
                    localGovernment.english) + Environment.NewLine;
                btnCreateLocalGovernment.Enabled = false;
            }
        }

        private void btnTambonList_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder();
            var codes = new List<String>();
            foreach ( var province in allEntities.Where(x => x.type.IsCompatibleEntityType(EntityType.Changwat)) )
            {
                result.AppendFormat("* {{{{Q|{0}}}}}", province.wiki.wikidata.Remove(0, 1));
                result.AppendLine();
                foreach ( var amphoe in province.entity.Where(x => !x.IsObsolete && x.type.IsCompatibleEntityType(EntityType.Amphoe)) )
                {
                    result.AppendFormat("** {{{{Q|{0}}}}}: ", amphoe.wiki.wikidata.Remove(0, 1));
                    codes.Clear();
                    foreach ( var tambon in amphoe.entity.Where(x => !x.IsObsolete && x.type.IsCompatibleEntityType(EntityType.Tambon)) )
                    {
                        if ( tambon.wiki != null && !String.IsNullOrEmpty(tambon.wiki.wikidata) )
                        {
                            // codes.Add(String.Format("{{{{Q|{0}}}}}", tambon.wiki.wikidata.Remove(0, 1)));
                            codes.Add(String.Format("[[{0}]]", tambon.wiki.wikidata));
                        }
                        else
                        {
                            codes.Add(tambon.english);
                        }
                    }
                    result.AppendLine(String.Join(" - ", codes));
                }
            }
            edtCollisions.Text = result.ToString();
        }

        private void btnLaoList_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder();
            var codes = new List<String>();
            foreach ( var province in allEntities.Where(x => x.type == EntityType.Changwat) )
            {
                result.AppendFormat("* {{{{Q|{0}}}}}: ", province.wiki.wikidata.Remove(0, 1));
                codes.Clear();
                var localGovernment = allEntities.Where(x => !x.IsObsolete && x.type.IsLocalGovernment() && GeocodeHelper.IsBaseGeocode(province.geocode, x.geocode));
                foreach ( var lao in localGovernment )
                {
                    if ( lao.wiki != null && !String.IsNullOrEmpty(lao.wiki.wikidata) )
                    {
                        // codes.Add(String.Format("{{{{Q|{0}}}}}", tambon.wiki.wikidata.Remove(0, 1)));
                        codes.Add(String.Format("[[{0}]]", lao.wiki.wikidata));
                    }
                    else
                    {
                        codes.Add(lao.english);
                    }
                }
                result.AppendLine(String.Join(" - ", codes));
            }
            edtCollisions.Text = result.ToString();
        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            var changwat = cbxChangwat.SelectedItem as Entity;
            if ( changwat != null )
            {
                var amphoe = changwat.entity.Where(x => !x.IsObsolete && x.type.IsCompatibleEntityType(EntityType.Amphoe));
                var dummy = new StringBuilder();
                _bot.SetLocatorMapTask.Task(amphoe, dummy, false);
            }
        }

        private void btnAmphoeCategory_Click(object sender, EventArgs e)
        {
            var amphoe = cbxAmphoe.SelectedItem as Entity;
            if ( amphoe != null )
            {
                if ( _helper.GetCategoryOfItem(amphoe) == null )
                {
                    _bot.CreateCategory(amphoe);
                }
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            WikibaseApi api = new WikibaseApi("https://test.wikidata.org", "TambonBot");

            var entityProvider = new EntityProvider(api);
            var item = entityProvider.getEntityFromId(new EntityId("Q42")) as Item;
        }
    }
}