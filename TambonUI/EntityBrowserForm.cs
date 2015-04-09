using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Wikibase;

namespace De.AHoerstemeier.Tambon.UI
{
    public partial class EntityBrowserForm : Form
    {
        #region fields

        private List<Entity> _localGovernments = new List<Entity>();
        private Entity _baseEntity;
        private List<Entity> _allEntities;

        #endregion fields

        #region properties

        public Boolean ShowDolaErrors
        {
            get;
            set;
        }

        public UInt32 StartChangwatGeocode
        {
            get;
            set;
        }

        public PopulationDataSourceType PopulationDataSource
        {
            get;
            set;
        }

        public Int16 PopulationReferenceYear
        {
            get;
            set;
        }

        public Boolean CheckWikiData
        {
            get;
            set;
        }

        #endregion properties

        #region constructor

        public EntityBrowserForm()
        {
            InitializeComponent();
            PopulationDataSource = PopulationDataSourceType.DOPA;
            PopulationReferenceYear = 2013;
        }

        #endregion constructor

        #region private methods

        private void EntityBrowserForm_Load(object sender, EventArgs e)
        {
            _baseEntity = GlobalData.CompleteGeocodeList();
            _baseEntity.CalcOldGeocodesRecursive();
            _baseEntity.PropagatePostcodeRecursive();
            _baseEntity.PropagateObsoleteToSubEntities();
            _allEntities = _baseEntity.FlatList().Where(x => !x.IsObsolete).ToList();
            var allLocalGovernmentParents = _allEntities.Where(x => x.type == EntityType.Tambon || x.type == EntityType.Changwat).ToList();
            _localGovernments.AddRange(_allEntities.Where(x => x.type.IsLocalGovernment()));
            foreach ( var tambon in allLocalGovernmentParents )
            {
                var localGovernmentEntity = tambon.CreateLocalGovernmentDummyEntity();
                if ( localGovernmentEntity != null )
                {
                    _localGovernments.Add(localGovernmentEntity);
                    _allEntities.Add(localGovernmentEntity);
                }
            }
            var allTambon = _allEntities.Where(x => x.type == EntityType.Tambon).ToList();
            foreach ( var lao in _localGovernments )
            {
                lao.CalculatePostcodeForLocalAdministration(allTambon);
            }

            GlobalData.LoadPopulationData(PopulationDataSource, PopulationReferenceYear);
            Entity.CalculateLocalGovernmentPopulation(_localGovernments, allTambon, PopulationDataSource, PopulationReferenceYear);
            PopulationDataToTreeView();
        }

        private TreeNode EntityToTreeNode(Entity data)
        {
            TreeNode retval = null;
            if ( data != null )
            {
                retval = new TreeNode(data.english);
                retval.Tag = data;
                if ( !data.type.IsThirdLevelAdministrativeUnit() )  // No Muban in Treeview
                {
                    foreach ( Entity entity in data.entity )
                    {
                        if ( !entity.IsObsolete && !entity.type.IsLocalGovernment() )
                        {
                            retval.Nodes.Add(EntityToTreeNode(entity));
                        }
                    }
                }
            }

            return retval;
        }

        private void PopulationDataToTreeView()
        {
            treeviewSelection.BeginUpdate();
            treeviewSelection.Nodes.Clear();

            TreeNode baseNode = EntityToTreeNode(_baseEntity);
            treeviewSelection.Nodes.Add(baseNode);
            baseNode.Expand();
            foreach ( TreeNode node in baseNode.Nodes )
            {
                if ( ((Entity)(node.Tag)).geocode == StartChangwatGeocode )
                {
                    treeviewSelection.SelectedNode = node;
                    node.Expand();
                }
            }
            treeviewSelection.EndUpdate();
        }

        private void treeviewSelection_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var selectedNode = treeviewSelection.SelectedNode;
            var entity = (Entity)(selectedNode.Tag);
            EntityToCentralAdministrativeListView(entity);
            EntityToLocalAdministrativeListView(entity);
            SetInfo(entity);
            CheckForErrors(entity);
            CalcElectionData(entity);
            CalcMubanData(entity);
            CalcLocalGovernmentData(entity);

            mnuMubanDefinitions.Enabled = AreaDefinitionAnnouncements(entity).Any();

            mnuWikipediaGerman.Enabled = entity.type.IsCompatibleEntityType(EntityType.Amphoe);
            mnuWikipediaEnglish.Enabled = mnuWikipediaGerman.Enabled;

            mnuWikipediaTambonEnglish.Enabled = entity.type.IsCompatibleEntityType(EntityType.Tambon);
        }

        private void CalcElectionData(Entity entity)
        {
            var localGovernmentsInEntity = entity.LocalGovernmentEntitiesOf(_localGovernments);
            var dummyEntity = new Entity();
            dummyEntity.entity.AddRange(localGovernmentsInEntity);

            var itemsWithCouncilElectionsPending = new List<EntityTermEnd>();
            var itemsWithOfficialElectionsPending = new List<EntityTermEnd>();
            var itemsWithOfficialElectionResultUnknown = new List<EntityTermEnd>();

            var itemsWithCouncilElectionPendingInParent = dummyEntity.EntitiesWithCouncilElectionPending();
            itemsWithCouncilElectionsPending.AddRange(itemsWithCouncilElectionPendingInParent);
            itemsWithCouncilElectionsPending.Sort((x, y) => x.CouncilTerm.begin.CompareTo(y.CouncilTerm.begin));

            var itemsWithOfficialElectionPendingInParent = dummyEntity.EntitiesWithOfficialElectionPending();
            itemsWithOfficialElectionsPending.AddRange(itemsWithOfficialElectionPendingInParent);
            itemsWithOfficialElectionsPending.Sort((x, y) => x.OfficialTerm.begin.CompareTo(y.OfficialTerm.begin));

            var itemsWithOfficialElectionResultUnknownInParent = dummyEntity.EntitiesWithLatestOfficialElectionResultUnknown();
            itemsWithOfficialElectionResultUnknown.AddRange(itemsWithOfficialElectionResultUnknownInParent);
            itemsWithOfficialElectionResultUnknown.Sort((x, y) => x.OfficialTerm.begin.CompareTo(y.OfficialTerm.begin));

            var result = String.Empty;
            var councilBuilder = new StringBuilder();
            Int32 councilCount = 0;
            foreach ( var item in itemsWithCouncilElectionsPending )
            {
                DateTime end;
                if ( item.CouncilTerm.endSpecified )
                {
                    end = item.CouncilTerm.end;
                }
                else
                {
                    end = item.CouncilTerm.begin.AddYears(4).AddDays(-1);
                }
                councilBuilder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2:d}", item.Entity.english, item.Entity.geocode, end);
                councilBuilder.AppendLine();
                councilCount++;
            }
            if ( councilCount > 0 )
            {
                result +=
                    String.Format("{0} LAO council elections pending", councilCount) + Environment.NewLine +
                    councilBuilder.ToString() + Environment.NewLine;
            }

            var officialBuilder = new StringBuilder();
            Int32 officialCount = 0;
            foreach ( var item in itemsWithOfficialElectionsPending )
            {
                String officialTermEnd = "unknown";
                if ( (item.OfficialTerm.begin != null) && (item.OfficialTerm.begin.Year > 1900) )
                {
                    DateTime end;
                    if ( item.OfficialTerm.endSpecified )
                    {
                        end = item.OfficialTerm.end;
                    }
                    else
                    {
                        end = item.OfficialTerm.begin.AddYears(4).AddDays(-1);
                    }
                    officialTermEnd = String.Format(CultureInfo.CurrentUICulture, "{0:d}", end);
                }
                officialBuilder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2}", item.Entity.english, item.Entity.geocode, officialTermEnd);
                officialBuilder.AppendLine();
                officialCount++;
            }
            if ( officialCount > 0 )
            {
                result +=
                    String.Format("{0} LAO official elections pending", officialCount) + Environment.NewLine +
                    officialBuilder.ToString() + Environment.NewLine;
            }

            var officialUnknownBuilder = new StringBuilder();
            Int32 officialUnknownCount = 0;
            foreach ( var item in itemsWithOfficialElectionResultUnknown )
            {
                if ( (item.OfficialTerm.begin != null) && (item.OfficialTerm.begin.Year > 1900) )  // must be always true
                {
                    officialUnknownBuilder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2:d}", item.Entity.english, item.Entity.geocode, item.OfficialTerm.begin);
                    officialUnknownBuilder.AppendLine();
                    officialUnknownCount++;
                }
            }
            if ( officialUnknownCount > 0 )
            {
                result +=
                    String.Format("{0} LAO official elections result missing", officialUnknownCount) + Environment.NewLine +
                    officialUnknownBuilder.ToString() + Environment.NewLine;
            }
            txtElections.Text = result;
        }

        private void CheckForErrors(Entity entity)
        {
            var text = String.Empty;
            var wrongGeocodes = entity.WrongGeocodes();
            if ( wrongGeocodes.Any() )
            {
                text += "Wrong geocodes:" + Environment.NewLine;
                foreach ( var code in wrongGeocodes )
                {
                    text += String.Format(" {0}", code) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            var localGovernmentsInEntity = entity.LocalGovernmentEntitiesOf(_localGovernments).ToList();
            // var localGovernmentsInProvince = LocalGovernmentEntitiesOf(this.baseEntity.entity.First(x => x.geocode == GeocodeHelper.ProvinceCode(entity.geocode))).ToList();
            var localEntitiesWithOffice = localGovernmentsInEntity.Where(x => x.Dola != null && !x.IsObsolete).ToList();  // Dola != null when there is a local government office
            // var localEntitiesInProvinceWithOffice = localGovernmentsInProvince.Where(x => x.Dola != null && !x.IsObsolete).ToList();  // Dola != null when there is a local government office
            if ( ShowDolaErrors )
            {
                var entitiesWithDolaCode = localEntitiesWithOffice.Where(x => x.Dola.codeSpecified).ToList();
                var allDolaCodes = entitiesWithDolaCode.Select(x => x.Dola.code).ToList();
                var duplicateDolaCodes = allDolaCodes.GroupBy(s => s).SelectMany(grp => grp.Skip(1)).ToList();
                if ( duplicateDolaCodes.Any() )
                {
                    text += "Duplicate DOLA codes:" + Environment.NewLine;
                    foreach ( var code in duplicateDolaCodes )
                    {
                        text += String.Format(" {0}", code) + Environment.NewLine;
                    }
                    text += Environment.NewLine;
                }
                var invalidDolaCodeEntities = entitiesWithDolaCode.Where(x => !x.DolaCodeValid()).ToList();
                if ( invalidDolaCodeEntities.Any() )
                {
                    text += "Invalid DOLA codes:" + Environment.NewLine;
                    foreach ( var dolaEntity in invalidDolaCodeEntities )
                    {
                        text += String.Format(" {0} {1} ({2})", dolaEntity.Dola.code, dolaEntity.english, dolaEntity.type) + Environment.NewLine;
                    }
                    text += Environment.NewLine;
                }
            }

            var localEntitiesWithoutParent = localEntitiesWithOffice.Where(x => !x.parent.Any());
            if ( localEntitiesWithoutParent.Any() )
            {
                text += "Local governments without parent:" + Environment.NewLine;
                foreach ( var subEntity in localEntitiesWithoutParent )
                {
                    text += String.Format(" {0} {1}", subEntity.geocode, subEntity.english) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }

            var allTambon = entity.FlatList().Where(x => x.type == EntityType.Tambon && !x.IsObsolete).ToList();
            var localGovernmentCoverages = new List<LocalGovernmentCoverageEntity>();
            foreach ( var item in localEntitiesWithOffice )
            {
                localGovernmentCoverages.AddRange(item.LocalGovernmentAreaCoverage);
            }
            var localGovernmentCoveragesByTambon = localGovernmentCoverages.GroupBy(s => s.geocode);
            var tambonWithMoreThanOneCoverage = localGovernmentCoveragesByTambon.Where(x => x.Count() > 1);
            var duplicateCompletelyCoveredTambon = tambonWithMoreThanOneCoverage.Where(x => x.Any(y => y.coverage == CoverageType.completely)).Select(x => x.Key);
            var invalidLocalGovernmentCoverages = localGovernmentCoveragesByTambon.Where(x => !allTambon.Any(y => y.geocode == x.Key));
            // var tambonWithMoreThanOneCoverage = localGovernmentCoveragesByTambon.SelectMany(grp => grp.Skip(1)).ToList();
            // var duplicateCompletelyCoveredTambon = tambonWithMoreThanOneCoverage.Where(x => x.coverage == CoverageType.completely);
            if ( invalidLocalGovernmentCoverages.Any() )
            {
                text += "Invalid Tambon references by areacoverage:" + Environment.NewLine;
                foreach ( var code in invalidLocalGovernmentCoverages )
                {
                    text += String.Format(" {0}", code.Key) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            if ( duplicateCompletelyCoveredTambon.Any() )
            {
                text += "Tambon covered completely more than once:" + Environment.NewLine;
                foreach ( var code in duplicateCompletelyCoveredTambon )
                {
                    text += String.Format(" {0}", code) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            var partialLocalGovernmentCoverages = localGovernmentCoverages.Where(x => x.coverage == CoverageType.partially);
            var partiallyCoveredTambon = partialLocalGovernmentCoverages.GroupBy(s => s.geocode);
            var onlyOnePartialCoverage = partiallyCoveredTambon.Select(group => new
            {
                code = group.Key,
                count = group.Count()
            }).Where(x => x.count == 1).Select(y => y.code);
            if ( onlyOnePartialCoverage.Any() )
            {
                text += "Tambon covered partially only once:" + Environment.NewLine;
                foreach ( var code in onlyOnePartialCoverage )
                {
                    text += String.Format(" {0}", code) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            var tambonWithoutCoverage = allTambon.Where(x => !localGovernmentCoveragesByTambon.Any(y => y.Key == x.geocode));
            if ( tambonWithoutCoverage.Any() )
            {
                text += String.Format("Tambon without coverage ({0}):", tambonWithoutCoverage.Count()) + Environment.NewLine;
                foreach ( var tambon in tambonWithoutCoverage )
                {
                    text += String.Format(" {0}", tambon.geocode) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            var localGovernmentWithoutCoverage = localEntitiesWithOffice.Where(x => x.type != EntityType.PAO && !x.LocalGovernmentAreaCoverage.Any());
            if ( localGovernmentWithoutCoverage.Any() )
            {
                text += String.Format("LAO without coverage ({0}):", localGovernmentWithoutCoverage.Count()) + Environment.NewLine;
                foreach ( var tambon in localGovernmentWithoutCoverage )
                {
                    text += String.Format(" {0}", tambon.geocode) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }

            var tambonWithoutPostalCode = allTambon.Where(x => !x.codes.post.value.Any());
            if ( tambonWithoutPostalCode.Any() )
            {
                text += String.Format("Tambon without postal code ({0}):", tambonWithoutPostalCode.Count()) + Environment.NewLine;
                foreach ( var tambon in tambonWithoutPostalCode )
                {
                    text += String.Format(" {0}", tambon.geocode) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }

            if ( GlobalData.AllGazetteAnnouncements.AllGazetteEntries.Any() )
            {
                var tambonWithoutAreaDefinition = allTambon.Where(x => !x.entitycount.Any());
                if ( tambonWithoutAreaDefinition.Any() )
                {
                    text += String.Format("Tambon without Royal Gazette area definition ({0}):", tambonWithoutAreaDefinition.Count()) + Environment.NewLine;
                    foreach ( var tambon in tambonWithoutAreaDefinition )
                    {
                        text += String.Format(" {0}", tambon.geocode) + Environment.NewLine;
                    }
                    text += Environment.NewLine;
                }

                var tambonError = String.Empty;
                var startingDate = new DateTime(1950, 1, 1);
                var gazetteNewerThan1950 = GlobalData.AllGazetteAnnouncements.AllGazetteEntries.Where(x => x.publication > startingDate);
                var gazetteTambonCreation = gazetteNewerThan1950.Where(x => x.Items.Any(y => y is GazetteCreate && ((GazetteCreate)y).type == EntityType.Tambon)).ToList();
                var tambonCreation = gazetteTambonCreation.SelectMany(x => x.Items.Where(y => y is GazetteCreate), (Gazette, History) => new
                {
                    Gazette,
                    History
                }).ToList();
                var tambonCreationInCurrentEntity = tambonCreation.Where(x => GeocodeHelper.IsBaseGeocode(entity.geocode, ((GazetteCreate)x.History).geocode)).ToList();

                foreach ( var creation in tambonCreationInCurrentEntity )
                {
                    var tambon = allTambon.FirstOrDefault(x => x.geocode == ((GazetteCreate)creation.History).geocode);
                    if ( tambon == null )
                    {
                        tambonError += String.Format("Tambon {0} created {1:d} not found in entitylist", ((GazetteCreate)creation.History).geocode, creation.Gazette.publication) + Environment.NewLine;
                    }
                    else if ( !tambon.history.Items.Any(x => x is HistoryCreate) )
                    {
                        tambonError += String.Format("Tambon {0} ({1}) created {2:d} has no history", tambon.english, ((GazetteCreate)creation.History).geocode, creation.Gazette.publication) + Environment.NewLine;
                    }
                }
                text += tambonError;
            }

            var unknownNeighbors = new List<UInt32>();
            var onewayNeighbors = new List<UInt32>();
            foreach ( var entityWithNeighbors in entity.FlatList().Where(x => x.area.bounding.Any()) )
            {
                foreach ( var neighbor in entityWithNeighbors.area.bounding.Select(x => x.geocode) )
                {
                    var targetEntity = _allEntities.FirstOrDefault(x => x.geocode == neighbor);
                    if ( targetEntity == null )
                    {
                        unknownNeighbors.Add(neighbor);
                    }
                    else if ( targetEntity.area.bounding.Any() && !targetEntity.area.bounding.Any(x => x.geocode == entityWithNeighbors.geocode) )
                    {
                        if ( !onewayNeighbors.Contains(entityWithNeighbors.geocode) )
                        {
                            onewayNeighbors.Add(entityWithNeighbors.geocode);
                        }
                    }
                }
            }
            if ( unknownNeighbors.Any() )
            {
                text += String.Format("Invalid neighboring entities ({0}):", unknownNeighbors.Count()) + Environment.NewLine;
                foreach ( var code in unknownNeighbors )
                {
                    text += String.Format(" {0}", code) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            if ( onewayNeighbors.Any() )
            {
                text += String.Format("Neighboring entities not found in both direction ({0}):", onewayNeighbors.Count()) + Environment.NewLine;
                foreach ( var code in onewayNeighbors )
                {
                    text += String.Format(" {0}", code) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }

            text += CheckCode(entity, new List<EntityType>() { EntityType.Changwat }, "FIPS10", (Entity x) => x.codes.fips10.value, "TH\\d\\d");
            text += CheckCode(entity, new List<EntityType>() { EntityType.Changwat }, "ISO3166", (Entity x) => x.codes.iso3166.value, "TH-(\\d\\d|S)");
            text += CheckCode(entity, new List<EntityType>() { EntityType.Changwat, EntityType.Amphoe }, "HASC", (Entity x) => x.codes.hasc.value, "TH(\\.[A-Z]{2}){1,2}");
            text += CheckCode(entity, new List<EntityType>() { EntityType.Changwat, EntityType.Amphoe }, "SALB", (Entity x) => x.codes.salb.value, "THA[\\d{3}]{1,2}");

            var entityWithoutSlogan = entity.FlatList().Where(x => !x.IsObsolete && (x.type.IsCompatibleEntityType(EntityType.Changwat) || x.type.IsCompatibleEntityType(EntityType.Amphoe)) && !x.symbols.slogan.Any());
            if ( entityWithoutSlogan.Any() )
            {
                text += String.Format("Province/District without slogan ({0}):", entityWithoutSlogan.Count()) + Environment.NewLine;
                foreach ( var item in entityWithoutSlogan )
                {
                    text += String.Format(" {0}: {1}", item.geocode, item.english) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }

            // check areacoverages
            txtErrors.Text = text;
        }

        private String CheckCode(Entity entity, IEnumerable<EntityType> entityTypes, String codeName, Func<Entity, String> selector, String format)
        {
            String text = String.Empty;
            var allEntites = entity.FlatList().Where(x => !x.IsObsolete);
            var allEntityOfFittingType = allEntites.Where(x => x.type.IsCompatibleEntityType(entityTypes));
            var entitiesWithoutCode = allEntityOfFittingType.Where(x => String.IsNullOrEmpty(selector(x)));
            if ( entitiesWithoutCode.Any() )
            {
                text += String.Format("Entity without {0} code ({1}):", codeName, entitiesWithoutCode.Count()) + Environment.NewLine;
                foreach ( var subEntity in entitiesWithoutCode )
                {
                    text += String.Format(" {0}", subEntity.geocode) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            var allCodes = allEntites.Where(x => !String.IsNullOrEmpty(selector(x))).Select(y => selector(y)).ToList();
            var duplicateCodes = allCodes.GroupBy(s => s).SelectMany(grp => grp.Skip(1)).ToList();
            if ( duplicateCodes.Any() )
            {
                text += String.Format("Duplicate {0} codes:", codeName) + Environment.NewLine;
                foreach ( var code in duplicateCodes )
                {
                    text += String.Format(" {0}", code) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            var regex = new Regex(format);
            var invalidCodes = allCodes.Where(x => !regex.IsMatch(x));
            if ( invalidCodes.Any() )
            {
                text += String.Format("Invalid {0} codes:", codeName) + Environment.NewLine;
                foreach ( var code in invalidCodes )
                {
                    text += String.Format(" {0}", code) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }

            return text;
        }

        private void SetInfo(Entity entity)
        {
            var value = String.Empty;
            var populationData = entity.population.FirstOrDefault(x => x.Year == PopulationReferenceYear && x.source == PopulationDataSource);
            if ( populationData != null )
            {
                value = String.Format("Population: {0} ({1} male,  {2} female)",
                    populationData.TotalPopulation.total,
                    populationData.TotalPopulation.male,
                    populationData.TotalPopulation.female) + Environment.NewLine + Environment.NewLine;
            }
            value += EntitySubDivisionCount(entity) + Environment.NewLine;

            txtSubDivisions.Text = value;
        }

        private void CalcMubanData(Entity entity)
        {
            String result = String.Empty;
            var allTambon = entity.FlatList().Where(x => !x.IsObsolete && x.type == EntityType.Tambon);
            var allMuban = entity.FlatList().Where(x => !x.IsObsolete && x.type == EntityType.Muban);
            var mubanNumbers = allTambon.GroupBy(x => x.entity.Count(y => !y.IsObsolete && y.type == EntityType.Muban))
                .Select(g => g.Key).ToList();
            mubanNumbers.Sort();
            if ( allMuban.Count() == 0 )
            {
                result = "No Muban" + Environment.NewLine;
            }
            else
            {
                result = String.Format("{0} Muban; Tambon have between {1} and {2} Muban" + Environment.NewLine,
                    allMuban.Count(),
                    mubanNumbers.First(),
                    mubanNumbers.Last());
                var counter = new FrequencyCounter();
                foreach ( var tambon in allTambon )
                {
                    counter.IncrementForCount(tambon.entity.Count(x => x.type == EntityType.Muban && !x.IsObsolete), tambon.geocode);
                }
                result += String.Format("Most common Muban number: {0}", counter.MostCommonValue) + Environment.NewLine;
                result += String.Format("Median Muban number: {0:0.0}", counter.MeanValue) + Environment.NewLine;
                List<UInt32> tambonWithNoMuban = null;
                if ( counter.Data.TryGetValue(0, out tambonWithNoMuban) )
                {
                    result += String.Format("Tambon without Muban: {0}", tambonWithNoMuban.Count) + Environment.NewLine;
                }
            }

            // could add: Muban creations in last years
            var tambonWithInvalidMubanNumber = TambonWithInvalidMubanNumber(allTambon);
            if ( tambonWithInvalidMubanNumber.Any() )
            {
                result += Environment.NewLine + String.Format("Muban inconsistent for {0} Muban:", tambonWithInvalidMubanNumber.Count()) + Environment.NewLine;
                foreach ( var tambon in tambonWithInvalidMubanNumber )
                {
                    result += String.Format("{0}: {1}", tambon.geocode, tambon.english) + Environment.NewLine;
                }
            }
            txtMuban.Text = result;
        }

        private void CalcLocalGovernmentData(Entity entity)
        {
            String result = String.Empty;

            var localGovernmentsInEntity = entity.LocalGovernmentEntitiesOf(_localGovernments).ToList();
            // var localGovernmentsInProvince = LocalGovernmentEntitiesOf(this.baseEntity.entity.First(x => x.geocode == GeocodeHelper.ProvinceCode(entity.geocode))).ToList();
            var localEntitiesWithOffice = localGovernmentsInEntity.Where(x => x.Dola != null && !x.IsObsolete).ToList();  // Dola != null when there is a local government office
            // var localEntitiesInProvinceWithOffice = localGovernmentsInProvince.Where(x => x.Dola != null && !x.IsObsolete).ToList();  // Dola != null when there is a local government office
            var localEntitiesWithCoverage = localEntitiesWithOffice.Where(x => x.LocalGovernmentAreaCoverage.Any());
            var localEntitiesWithoutCoverage = localEntitiesWithOffice.Where(x => !x.LocalGovernmentAreaCoverage.Any() && x.type != EntityType.PAO);

            var localGovernmentCoveringMoreThanOneTambon = localEntitiesWithCoverage.Where(x => x.LocalGovernmentAreaCoverage.Count() > 1);
            var localGovernmentCoveringExactlyOneTambon = localEntitiesWithCoverage.Where(x => x.LocalGovernmentAreaCoverage.Count() == 1 && x.LocalGovernmentAreaCoverage.First().coverage == CoverageType.completely);
            var localGovernmentCoveringOneTambonPartially = localEntitiesWithCoverage.Where(x => x.LocalGovernmentAreaCoverage.Count() == 1 && x.LocalGovernmentAreaCoverage.First().coverage == CoverageType.partially);
            var localGovernmentCoveringMoreThanOneTambonAndAllCompletely = localGovernmentCoveringMoreThanOneTambon.Where(x => x.LocalGovernmentAreaCoverage.All(y => y.coverage == CoverageType.completely));

            result += String.Format("LAO: {0}", localEntitiesWithOffice.Count()) + Environment.NewLine;
            result += String.Format("LAO with coverage: {0}", localEntitiesWithCoverage.Count()) + Environment.NewLine;
            if ( localEntitiesWithoutCoverage.Any() )
            {
                result += String.Format("LAO missing coverage: {0}", localEntitiesWithoutCoverage.Count()) + Environment.NewLine;
            }
            result += String.Format("LAO covering exactly one Tambon: {0}", localGovernmentCoveringExactlyOneTambon.Count()) + Environment.NewLine;
            result += String.Format("LAO covering one Tambon partially: {0}", localGovernmentCoveringOneTambonPartially.Count()) + Environment.NewLine;
            result += String.Format("LAO covering more than one Tambon: {0} ({1} TAO)",
                localGovernmentCoveringMoreThanOneTambon.Count(),
                localGovernmentCoveringMoreThanOneTambon.Where(x => x.type == EntityType.TAO).Count()) + Environment.NewLine;
            result += String.Format("LAO covering more than one Tambon and all completely: {0} ({1} TAO)",
                localGovernmentCoveringMoreThanOneTambonAndAllCompletely.Count(),
                localGovernmentCoveringMoreThanOneTambonAndAllCompletely.Where(x => x.type == EntityType.TAO).Count()) + Environment.NewLine;

            var localGovernmentExpectingHistory = localGovernmentsInEntity.Where(x => x.Dola != null && x.type != EntityType.PAO);
            var localGovernmentWithoutLatestHistory = localGovernmentExpectingHistory.Where(x =>
                !x.history.Items.Any(y => y is HistoryCreate) &&
                (!x.history.Items.Any(y => y is HistoryCreate && (y as HistoryCreate).type == x.type) ||
                 !x.history.Items.Any(y => y is HistoryStatus && (y as HistoryStatus).@new == x.type))
                ).ToList();
            localGovernmentWithoutLatestHistory.AddRange(localGovernmentExpectingHistory.Where(x =>
                x.IsObsolete &&
                !x.history.Items.Any(y => y is HistoryAbolish && (y as HistoryAbolish).type == x.type))
                );
            result += Environment.NewLine + String.Format("LAO without latest history: {0} ({1} TAO)",
                localGovernmentWithoutLatestHistory.Count(),
                localGovernmentWithoutLatestHistory.Where(x => x.type == EntityType.TAO).Count()) + Environment.NewLine;
            txtLocalGovernment.Text = result;
        }

        private IEnumerable<Entity> TambonWithInvalidMubanNumber(IEnumerable<Entity> allTambon)
        {
            var result = new List<Entity>();
            foreach ( var tambon in allTambon.Where(x => x.type == EntityType.Tambon) )
            {
                if ( !tambon.MubanNumberConsistent() )
                {
                    result.Add(tambon);
                }
            }
            return result;
        }

        private String EntitySubDivisionCount(Entity entity)
        {
            var counted = entity.CountAllSubdivisions(_localGovernments);
            var noLocation = entity.CountSubdivisionsWithoutLocation(_localGovernments);

            var result = String.Empty;
            foreach ( var keyvaluepair in counted )
            {
                Int32 noLocationCount = 0;
                if ( noLocation.TryGetValue(keyvaluepair.Key, out noLocationCount) )
                {
                    result += String.Format("{0}: {1} ({2} without location)", keyvaluepair.Key, keyvaluepair.Value, noLocationCount) + Environment.NewLine;
                }
                else
                {
                    result += String.Format("{0}: {1}", keyvaluepair.Key, keyvaluepair.Value) + Environment.NewLine;
                }
            }
            return result;
        }

        private void EntityToCentralAdministrativeListView(Entity entity)
        {
            listviewCentralAdministration.BeginUpdate();
            listviewCentralAdministration.Items.Clear();
            foreach ( Entity subEntity in entity.entity.Where(x => !x.IsObsolete && !x.type.IsLocalGovernment()) )
            {
                ListViewItem item = listviewCentralAdministration.Items.Add(subEntity.english);
                item.SubItems.Add(subEntity.name);
                item.SubItems.Add(subEntity.geocode.ToString());
                var populationData = subEntity.population.FirstOrDefault(x => x.Year == PopulationReferenceYear && x.source == PopulationDataSource);
                if ( populationData != null )
                {
                    item.SubItems.Add(populationData.TotalPopulation.total.ToString());
                }
            }
            listviewCentralAdministration.EndUpdate();
        }

        private void EntityToLocalAdministrativeListView(Entity entity)
        {
            listviewLocalAdministration.BeginUpdate();
            listviewLocalAdministration.Items.Clear();
            var localGovernmentsInEntity = entity.LocalGovernmentEntitiesOf(_localGovernments).ToList();
            foreach ( Entity subEntity in localGovernmentsInEntity )
            {
                ListViewItem item = listviewLocalAdministration.Items.Add(subEntity.english);
                item.SubItems.Add(subEntity.name);
                item.SubItems.Add(subEntity.type.ToString());
                if ( subEntity.geocode > 9999 )
                {
                    // generated geocode
                    item.SubItems.Add(String.Empty);
                }
                else
                {
                    item.SubItems.Add(subEntity.geocode.ToString());
                }
                String dolaCode = String.Empty;
                var office = subEntity.office.FirstOrDefault(x => x.type == OfficeType.TAOOffice || x.type == OfficeType.PAOOffice || x.type == OfficeType.MunicipalityOffice);
                if ( office != null )
                {
                    if ( (office.dola != null) && (office.dola.codeSpecified) )
                    {
                        dolaCode = office.dola.code.ToString();
                    }
                }
                item.SubItems.Add(dolaCode);
                var populationData = subEntity.population.FirstOrDefault(x => x.Year == PopulationReferenceYear && x.source == PopulationDataSource);
                if ( populationData != null )
                {
                    item.SubItems.Add(populationData.TotalPopulation.total.ToString());
                }
            }
            listviewLocalAdministration.EndUpdate();
        }

        private void treeviewSelection_MouseUp(Object sender, MouseEventArgs e)
        {
            if ( e.Button == MouseButtons.Right )
            {
                // Select the clicked node
                treeviewSelection.SelectedNode = treeviewSelection.GetNodeAt(e.X, e.Y);

                if ( treeviewSelection.SelectedNode != null )
                {
                    contextMenuStrip1.Show(treeviewSelection, e.Location);
                }
            }
        }

        private IEnumerable<GazetteEntry> AreaDefinitionAnnouncements(Entity entity)
        {
            var result = new List<GazetteEntry>();
            if ( entity.type != EntityType.Country )
            {
                var allAboutGeocode = GlobalData.AllGazetteAnnouncements.AllGazetteEntries.Where(x =>
                    x.IsAboutGeocode(entity.geocode, true) || entity.OldGeocodes.Any(y => x.IsAboutGeocode(y, true)));
                var allAreaDefinitionAnnouncements = allAboutGeocode.Where(x => x.Items.Any(y => y is GazetteAreaDefinition));
                foreach ( var announcement in allAreaDefinitionAnnouncements )
                {
                    var areaDefinitions = announcement.Items.Where(x => x is GazetteAreaDefinition);
                    if ( areaDefinitions.Any(x => (x as GazetteAreaDefinition).IsAboutGeocode(entity.geocode, true) ||
                        entity.OldGeocodes.Any(y => (x as GazetteAreaDefinition).IsAboutGeocode(y, true))) )
                    {
                        result.Add(announcement);
                    }
                }
            }
            return result;
        }

        private void mnuMubanDefinitions_Click(Object sender, EventArgs e)
        {
            var selectedNode = treeviewSelection.SelectedNode;
            var entity = (Entity)(selectedNode.Tag);
            foreach ( var entry in AreaDefinitionAnnouncements(entity) )
            {
                ShowPdf(entry);
            }
        }

        private void ShowPdf(GazetteEntry entry)
        {
            try
            {
                entry.MirrorToCache();
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                // TODO
                String pdfFilename = entry.LocalPdfFileName;

                if ( File.Exists(pdfFilename) )
                {
                    p.StartInfo.FileName = pdfFilename;
                    p.Start();
                }
            }
            catch
            {
                // throw;
            }
        }

        private void mnuWikipediaGerman_Click(Object sender, EventArgs e)
        {
            AmphoeToWikipedia(Language.German);
        }

        private void mnuWikipediaEnglish_Click(Object sender, EventArgs e)
        {
            AmphoeToWikipedia(Language.English);
        }

        private void AmphoeToWikipedia(Language language)
        {
            var selectedNode = treeviewSelection.SelectedNode;
            var entity = (Entity)(selectedNode.Tag);
            if ( entity.type.IsCompatibleEntityType(EntityType.Amphoe) )
            {
                var exporter = new WikipediaExporter(_baseEntity, _localGovernments);
                exporter.CheckWikiData = CheckWikiData;
                exporter.PopulationReferenceYear = PopulationReferenceYear;
                exporter.PopulationDataSource = PopulationDataSource;
                var text = exporter.AmphoeToWikipedia(entity, language);

                CopyToClipboard(text);
            }
        }

        private void mnuWikipediaTambonEnglish_Click(Object sender, EventArgs e)
        {
            var selectedNode = treeviewSelection.SelectedNode;
            var entity = (Entity)(selectedNode.Tag);
            if ( entity.type.IsCompatibleEntityType(EntityType.Tambon) )
            {
                var exporter = new WikipediaExporter(_baseEntity, _localGovernments);
                exporter.CheckWikiData = CheckWikiData;
                exporter.PopulationReferenceYear = PopulationReferenceYear;
                exporter.PopulationDataSource = PopulationDataSource;
                var text = exporter.TambonArticle(entity, Language.English);

                CopyToClipboard(text);
            }
        }

        private void CopyToClipboard(string text)
        {
            Boolean success = false;
            while ( !success )
            {
                try
                {
                    Clipboard.Clear();
                    Clipboard.SetText(text);
                    success = true;
                }
                catch
                {
                    if ( MessageBox.Show(this, "Copying text to clipboard failed.", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) != DialogResult.Retry )
                    {
                        break;
                    }
                }
            }
        }

        #endregion private methods
    }
}