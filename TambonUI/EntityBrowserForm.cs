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
            _baseEntity.PropagatePostcodeRecursive();
            _allEntities = _baseEntity.FlatList().Where(x => !x.IsObsolete).ToList();
            var allLocalGovernmentParents = _allEntities.Where(x => x.type == EntityType.Tambon || x.type == EntityType.Changwat).ToList();
            foreach (var tambon in allLocalGovernmentParents)
            {
                var localGovernmentEntity = tambon.CreateLocalGovernmentDummyEntity();
                if (localGovernmentEntity != null)
                {
                    _localGovernments.Add(localGovernmentEntity);
                }
            }
            foreach (var item in _allEntities.Where(x => x.type.IsLocalGovernment()))
            {
                _localGovernments.Add(item);
            }

            GlobalData.LoadPopulationData(PopulationDataSource, PopulationReferenceYear);
            CalculateLocalGovernmentPopulation();
            PopulationDataToTreeView();
        }

        private void CalculateLocalGovernmentPopulation()
        {
            var allTambon = _allEntities.Where(x => x.type == EntityType.Tambon).ToList();
            foreach (var localEntityWithoutPopulation in _localGovernments.Where(x =>
                x.LocalGovernmentAreaCoverage.Any() && !x.population.Any(
                y => y.Year == PopulationReferenceYear && y.source == PopulationDataSource)))
            {
                var populationData = new PopulationData();
                localEntityWithoutPopulation.population.Add(populationData);
                foreach (var coverage in localEntityWithoutPopulation.LocalGovernmentAreaCoverage)
                {
                    var tambon = allTambon.Single(x => x.geocode == coverage.geocode);
                    var sourcePopulationData = tambon.population.First(y => y.Year == PopulationReferenceYear && y.source == PopulationDataSource);
                    populationData.year = sourcePopulationData.year;
                    populationData.referencedate = sourcePopulationData.referencedate;
                    populationData.referencedateSpecified = sourcePopulationData.referencedateSpecified;
                    populationData.source = sourcePopulationData.source;

                    List<HouseholdDataPoint> dataPointToClone = new List<HouseholdDataPoint>();
                    dataPointToClone.AddRange(sourcePopulationData.data.Where(x => x.geocode == localEntityWithoutPopulation.geocode));
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
                        var newDataPoint = new HouseholdDataPoint();
                        newDataPoint.male = dataPoint.male;
                        newDataPoint.female = dataPoint.female;
                        newDataPoint.households = dataPoint.households;
                        newDataPoint.total = dataPoint.total;
                        newDataPoint.geocode = coverage.geocode;
                        newDataPoint.type = dataPoint.type;
                        populationData.data.Add(newDataPoint);
                    }
                }
                if (populationData.data.Count == 1)
                {
                    populationData.data.First().type = PopulationDataType.total;
                }
                populationData.CalculateTotal();
            }
        }

        private TreeNode EntityToTreeNode(Entity data)
        {
            TreeNode retval = null;
            if (data != null)
            {
                retval = new TreeNode(data.english);
                retval.Tag = data;
                if (!data.type.IsThirdLevelAdministrativeUnit())  // No Muban in Treeview
                {
                    foreach (Entity entity in data.entity)
                    {
                        if (!entity.IsObsolete && !entity.type.IsLocalGovernment())
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
            foreach (TreeNode node in baseNode.Nodes)
            {
                if (((Entity)(node.Tag)).geocode == StartChangwatGeocode)
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
        }

        private void CalcElectionData(Entity entity)
        {
            var localGovernmentsInEntity = LocalGovernmentEntitiesOf(entity);
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
            foreach (var item in itemsWithCouncilElectionsPending)
            {
                DateTime end;
                if (item.CouncilTerm.endSpecified)
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
            if (councilCount > 0)
            {
                result +=
                    String.Format("{0} LAO council elections pending", councilCount) + Environment.NewLine +
                    councilBuilder.ToString() + Environment.NewLine;
            }

            var officialBuilder = new StringBuilder();
            Int32 officialCount = 0;
            foreach (var item in itemsWithOfficialElectionsPending)
            {
                String officialTermEnd = "unknown";
                if ((item.OfficialTerm.begin != null) && (item.OfficialTerm.begin.Year > 1900))
                {
                    DateTime end;
                    if (item.OfficialTerm.endSpecified)
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
            if (officialCount > 0)
            {
                result +=
                    String.Format("{0} LAO official elections pending", officialCount) + Environment.NewLine +
                    officialBuilder.ToString() + Environment.NewLine;
            }

            var officialUnknownBuilder = new StringBuilder();
            Int32 officialUnknownCount = 0;
            foreach (var item in itemsWithOfficialElectionResultUnknown)
            {
                if ((item.OfficialTerm.begin != null) && (item.OfficialTerm.begin.Year > 1900))  // must be always true
                {
                    officialUnknownBuilder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2:d}", item.Entity.english, item.Entity.geocode, item.OfficialTerm.begin);
                    officialUnknownBuilder.AppendLine();
                    officialUnknownCount++;
                }
            }
            if (officialUnknownCount > 0)
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
            if (wrongGeocodes.Any())
            {
                text += "Wrong geocodes:" + Environment.NewLine;
                foreach (var code in wrongGeocodes)
                {
                    text += String.Format(" {0}", code) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            var localGovernmentsInEntity = LocalGovernmentEntitiesOf(entity).ToList();
            // var localGovernmentsInProvince = LocalGovernmentEntitiesOf(this.baseEntity.entity.First(x => x.geocode == GeocodeHelper.ProvinceCode(entity.geocode))).ToList();
            var localEntitiesWithOffice = localGovernmentsInEntity.Where(x => x.Dola != null && !x.IsObsolete).ToList();  // Dola != null when there is a local government office
            // var localEntitiesInProvinceWithOffice = localGovernmentsInProvince.Where(x => x.Dola != null && !x.IsObsolete).ToList();  // Dola != null when there is a local government office
            if (ShowDolaErrors)
            {
                var entitiesWithDolaCode = localEntitiesWithOffice.Where(x => x.Dola.codeSpecified).ToList();
                var allDolaCodes = entitiesWithDolaCode.Select(x => x.Dola.code).ToList();
                var duplicateDolaCodes = allDolaCodes.GroupBy(s => s).SelectMany(grp => grp.Skip(1)).ToList();
                if (duplicateDolaCodes.Any())
                {
                    text += "Duplicate DOLA codes:" + Environment.NewLine;
                    foreach (var code in duplicateDolaCodes)
                    {
                        text += String.Format(" {0}", code) + Environment.NewLine;
                    }
                    text += Environment.NewLine;
                }
                var invalidDolaCodeEntities = entitiesWithDolaCode.Where(x => !x.DolaCodeValid()).ToList();
                if (invalidDolaCodeEntities.Any())
                {
                    text += "Invalid DOLA codes:" + Environment.NewLine;
                    foreach (var dolaEntity in invalidDolaCodeEntities)
                    {
                        text += String.Format(" {0} {1} ({2})", dolaEntity.Dola.code, dolaEntity.english, dolaEntity.type) + Environment.NewLine;
                    }
                    text += Environment.NewLine;
                }
            }

            var localEntitiesWithoutParent = localEntitiesWithOffice.Where(x => !x.parent.Any());
            if (localEntitiesWithoutParent.Any())
            {
                text += "Local governments without parent:" + Environment.NewLine;
                foreach (var subEntity in localEntitiesWithoutParent)
                {
                    text += String.Format(" {0} {1}", subEntity.geocode, subEntity.english) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }

            var allTambon = entity.FlatList().Where(x => x.type == EntityType.Tambon && !x.IsObsolete).ToList();
            var localGovernmentCoverages = new List<LocalGovernmentCoverageEntity>();
            foreach (var item in localEntitiesWithOffice)
            {
                localGovernmentCoverages.AddRange(item.LocalGovernmentAreaCoverage);
            }
            var localGovernmentCoveragesByTambon = localGovernmentCoverages.GroupBy(s => s.geocode);
            var tambonWithMoreThanOneCoverage = localGovernmentCoveragesByTambon.Where(x => x.Count() > 1);
            var duplicateCompletelyCoveredTambon = tambonWithMoreThanOneCoverage.Where(x => x.Any(y => y.coverage == CoverageType.completely)).Select(x => x.Key);
            var invalidLocalGovernmentCoverages = localGovernmentCoveragesByTambon.Where(x => !allTambon.Any(y => y.geocode == x.Key));
            // var tambonWithMoreThanOneCoverage = localGovernmentCoveragesByTambon.SelectMany(grp => grp.Skip(1)).ToList();
            // var duplicateCompletelyCoveredTambon = tambonWithMoreThanOneCoverage.Where(x => x.coverage == CoverageType.completely);
            if (invalidLocalGovernmentCoverages.Any())
            {
                text += "Invalid Tambon references by areacoverage:" + Environment.NewLine;
                foreach (var code in invalidLocalGovernmentCoverages)
                {
                    text += String.Format(" {0}", code.Key) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            if (duplicateCompletelyCoveredTambon.Any())
            {
                text += "Tambon covered completely more than once:" + Environment.NewLine;
                foreach (var code in duplicateCompletelyCoveredTambon)
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
            if (onlyOnePartialCoverage.Any())
            {
                text += "Tambon covered partially only once:" + Environment.NewLine;
                foreach (var code in onlyOnePartialCoverage)
                {
                    text += String.Format(" {0}", code) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            var tambonWithoutCoverage = allTambon.Where(x => !localGovernmentCoveragesByTambon.Any(y => y.Key == x.geocode));
            if (tambonWithoutCoverage.Any())
            {
                text += String.Format("Tambon without coverage ({0}):", tambonWithoutCoverage.Count()) + Environment.NewLine;
                foreach (var tambon in tambonWithoutCoverage)
                {
                    text += String.Format(" {0}", tambon.geocode) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            var localGovernmentWithoutCoverage = localEntitiesWithOffice.Where(x => x.type != EntityType.PAO && !x.LocalGovernmentAreaCoverage.Any());
            if (localGovernmentWithoutCoverage.Any())
            {
                text += String.Format("LAO without coverage ({0}):", localGovernmentWithoutCoverage.Count()) + Environment.NewLine;
                foreach (var tambon in localGovernmentWithoutCoverage)
                {
                    text += String.Format(" {0}", tambon.geocode) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }

            var tambonWithoutPostalCode = allTambon.Where(x => !x.codes.post.value.Any());
            if (tambonWithoutPostalCode.Any())
            {
                text += String.Format("Tambon without postal code ({0}):", tambonWithoutPostalCode.Count()) + Environment.NewLine;
                foreach (var tambon in tambonWithoutPostalCode)
                {
                    text += String.Format(" {0}", tambon.geocode) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }

            if (GlobalData.AllGazetteAnnouncements.AllGazetteEntries.Any())
            {
                var tambonWithoutAreaDefinition = allTambon.Where(x => !x.entitycount.Any());
                if (tambonWithoutAreaDefinition.Any())
                {
                    text += String.Format("Tambon without Royal Gazette area definition ({0}):", tambonWithoutAreaDefinition.Count()) + Environment.NewLine;
                    foreach (var tambon in tambonWithoutAreaDefinition)
                    {
                        text += String.Format(" {0}", tambon.geocode) + Environment.NewLine;
                    }
                    text += Environment.NewLine;
                }
            }

            var unknownNeighbors = new List<UInt32>();
            var onewayNeighbors = new List<UInt32>();
            foreach (var entityWithNeighbors in entity.FlatList().Where(x => x.area.bounding.Any()))
            {
                foreach (var neighbor in entityWithNeighbors.area.bounding.Select(x => x.geocode))
                {
                    var targetEntity = _allEntities.FirstOrDefault(x => x.geocode == neighbor);
                    if (targetEntity == null)
                    {
                        unknownNeighbors.Add(neighbor);
                    }
                    else if (targetEntity.area.bounding.Any() && !targetEntity.area.bounding.Any(x => x.geocode == entityWithNeighbors.geocode))
                    {
                        if (!onewayNeighbors.Contains(entityWithNeighbors.geocode))
                        {
                            onewayNeighbors.Add(entityWithNeighbors.geocode);
                        }
                    }
                }
            }
            if (unknownNeighbors.Any())
            {
                text += String.Format("Invalid neighboring entities ({0}):", unknownNeighbors.Count()) + Environment.NewLine;
                foreach (var code in unknownNeighbors)
                {
                    text += String.Format(" {0}", code) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            if (onewayNeighbors.Any())
            {
                text += String.Format("Neighboring entities not found in both direction ({0}):", onewayNeighbors.Count()) + Environment.NewLine;
                foreach (var code in onewayNeighbors)
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
            if (entityWithoutSlogan.Any())
            {
                text += String.Format("Province/District without slogan ({0}):", entityWithoutSlogan.Count()) + Environment.NewLine;
                foreach (var item in entityWithoutSlogan)
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
            if (entitiesWithoutCode.Any())
            {
                text += String.Format("Entity without {0} code ({1}):", codeName, entitiesWithoutCode.Count()) + Environment.NewLine;
                foreach (var subEntity in entitiesWithoutCode)
                {
                    text += String.Format(" {0}", subEntity.geocode) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            var allCodes = allEntites.Where(x => !String.IsNullOrEmpty(selector(x))).Select(y => selector(y)).ToList();
            var duplicateCodes = allCodes.GroupBy(s => s).SelectMany(grp => grp.Skip(1)).ToList();
            if (duplicateCodes.Any())
            {
                text += String.Format("Duplicate {0} codes:", codeName) + Environment.NewLine;
                foreach (var code in duplicateCodes)
                {
                    text += String.Format(" {0}", code) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            var regex = new Regex(format);
            var invalidCodes = allCodes.Where(x => !regex.IsMatch(x));
            if (invalidCodes.Any())
            {
                text += String.Format("Invalid {0} codes:", codeName) + Environment.NewLine;
                foreach (var code in invalidCodes)
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
            if (populationData != null)
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
            if (allMuban.Count() == 0)
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
                foreach (var tambon in allTambon)
                {
                    counter.IncrementForCount(tambon.entity.Count(x => x.type == EntityType.Muban && !x.IsObsolete), tambon.geocode);
                }
                result += String.Format("Most common Muban number: {0}", counter.MostCommonValue) + Environment.NewLine;
                result += String.Format("Median Muban number: {0:0.0}", counter.MeanValue) + Environment.NewLine;
                List<UInt32> tambonWithNoMuban = null;
                if (counter.Data.TryGetValue(0, out tambonWithNoMuban))
                {
                    result += String.Format("Tambon without Muban: {0}", tambonWithNoMuban.Count) + Environment.NewLine;
                }
            }

            // could add: Muban creations in last years
            var tambonWithInvalidMubanNumber = TambonWithInvalidMubanNumber(allTambon);
            if (tambonWithInvalidMubanNumber.Any())
            {
                result += Environment.NewLine + String.Format("Muban inconsistent for {0} Muban:", tambonWithInvalidMubanNumber.Count()) + Environment.NewLine;
                foreach (var tambon in tambonWithInvalidMubanNumber)
                {
                    result += String.Format("{0}: {1}", tambon.geocode, tambon.english) + Environment.NewLine;
                }
            }
            txtMuban.Text = result;
        }

        private void CalcLocalGovernmentData(Entity entity)
        {
            String result = String.Empty;

            var localGovernmentsInEntity = LocalGovernmentEntitiesOf(entity).ToList();
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
            if (localEntitiesWithoutCoverage.Any())
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

            txtLocalGovernment.Text = result;
        }

        private IEnumerable<Entity> TambonWithInvalidMubanNumber(IEnumerable<Entity> allTambon)
        {
            var result = new List<Entity>();
            foreach (var tambon in allTambon.Where(x => x.type == EntityType.Tambon))
            {
                if (!tambon.MubanNumberConsistent())
                {
                    result.Add(tambon);
                }
            }
            return result;
        }

        private Dictionary<EntityType, Int32> CountSubdivisions(IEnumerable<Entity> entities)
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

        private Dictionary<EntityType, Int32> CountSubdivisions(Entity entity)
        {
            var toCount = _localGovernments.Where(x => x.parent.Contains(entity.geocode) || GeocodeHelper.IsBaseGeocode(entity.geocode, x.geocode)).SelectMany(x => x.FlatList()).ToList();
            // Chumchon and local governments are already in list, so filter them out while adding the central government units
            toCount.AddRange(entity.FlatList().Where(x => !x.type.IsLocalGovernment() && x.type != EntityType.Chumchon));
            toCount.RemoveAll(x => x.type == EntityType.Unknown || x.IsObsolete);
            return CountSubdivisions(toCount);
        }

        private Dictionary<EntityType, Int32> CountSubdivisionsWithoutLocation(Entity entity)
        {
            var toCount = _localGovernments.Where(x => x.parent.Contains(entity.geocode) || GeocodeHelper.IsBaseGeocode(entity.geocode, x.geocode)).ToList();
            toCount.AddRange(entity.FlatList().Where(x => !x.type.IsLocalGovernment()));
            toCount.RemoveAll(x => x.type == EntityType.Unknown || x.IsObsolete);
            toCount.RemoveAll(x => x.office.Any(y => y.Point != null));
            return CountSubdivisions(toCount);
        }

        private String EntitySubDivisionCount(Entity entity)
        {
            var counted = CountSubdivisions(entity);
            var noLocation = CountSubdivisionsWithoutLocation(entity);

            var result = String.Empty;
            foreach (var keyvaluepair in counted)
            {
                Int32 noLocationCount = 0;
                if (noLocation.TryGetValue(keyvaluepair.Key, out noLocationCount))
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
            foreach (Entity subEntity in entity.entity.Where(x => !x.IsObsolete && !x.type.IsLocalGovernment()))
            {
                ListViewItem item = listviewCentralAdministration.Items.Add(subEntity.english);
                item.SubItems.Add(subEntity.name);
                item.SubItems.Add(subEntity.geocode.ToString());
                var populationData = subEntity.population.FirstOrDefault(x => x.Year == PopulationReferenceYear && x.source == PopulationDataSource);
                if (populationData != null)
                {
                    item.SubItems.Add(populationData.TotalPopulation.total.ToString());
                }
            }
            listviewCentralAdministration.EndUpdate();
        }

        private IEnumerable<Entity> LocalGovernmentEntitiesOf(Entity entity)
        {
            return _localGovernments.Where(x => x.parent.Contains(entity.geocode) || GeocodeHelper.IsBaseGeocode(entity.geocode, x.geocode) && !x.IsObsolete);
        }

        private void EntityToLocalAdministrativeListView(Entity entity)
        {
            listviewLocalAdministration.BeginUpdate();
            listviewLocalAdministration.Items.Clear();
            var localGovernmentsInEntity = LocalGovernmentEntitiesOf(entity).ToList();
            foreach (Entity subEntity in localGovernmentsInEntity)
            {
                ListViewItem item = listviewLocalAdministration.Items.Add(subEntity.english);
                item.SubItems.Add(subEntity.name);
                item.SubItems.Add(subEntity.type.ToString());
                if (subEntity.geocode > 9999)
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
                if (office != null)
                {
                    if ((office.dola != null) && (office.dola.codeSpecified))
                    {
                        dolaCode = office.dola.code.ToString();
                    }
                }
                item.SubItems.Add(dolaCode);
                var populationData = subEntity.population.FirstOrDefault(x => x.Year == PopulationReferenceYear && x.source == PopulationDataSource);
                if (populationData != null)
                {
                    item.SubItems.Add(populationData.TotalPopulation.total.ToString());
                }
            }
            listviewLocalAdministration.EndUpdate();
        }

        private void treeviewSelection_MouseUp(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Select the clicked node
                treeviewSelection.SelectedNode = treeviewSelection.GetNodeAt(e.X, e.Y);

                if (treeviewSelection.SelectedNode != null)
                {
                    contextMenuStrip1.Show(treeviewSelection, e.Location);
                }
            }
        }

        private delegate String CountAsString(Int32 count);

        private AmphoeDataForWikipediaExport CalculateAmphoeData(Entity entity, Language language)
        {
            if (entity.type.IsCompatibleEntityType(EntityType.Amphoe))
            {
                var result = new AmphoeDataForWikipediaExport();
                result.Province = _baseEntity.entity.FirstOrDefault(x => x.geocode == GeocodeHelper.ProvinceCode(entity.geocode));
                result.AllTambon.AddRange(entity.entity.Where(x => x.type.IsCompatibleEntityType(EntityType.Tambon) && !x.IsObsolete));
                result.LocalAdministrations.AddRange(LocalGovernmentEntitiesOf(entity).Where(x => !x.IsObsolete));

                var allEntities = result.AllTambon.ToList();
                allEntities.AddRange(result.LocalAdministrations);
                if (CheckWikiData)
                {
                    foreach (var keyValuePair in RetrieveWikpediaLinks(allEntities, language))
                    {
                        result.WikipediaLinks[keyValuePair.Key] = keyValuePair.Value;
                    }
                }
                var counted = CountSubdivisions(entity);
                if (!counted.ContainsKey(EntityType.Muban))
                {
                    counted[EntityType.Muban] = 0;
                }
                foreach (var keyValuePair in counted)
                {
                    result.CentralAdministrationCountByEntity[keyValuePair.Key] = keyValuePair.Value;
                }

                result.MaxPopulation = 0;
                foreach (var tambon in result.AllTambon)
                {
                    var populationData = tambon.population.FirstOrDefault(x => x.Year == PopulationReferenceYear && x.source == PopulationDataSource);
                    if (populationData != null)
                    {
                        result.MaxPopulation = Math.Max(result.MaxPopulation, populationData.TotalPopulation.total);
                    }
                }

                foreach (var keyValuePair in CountSubdivisions(result.LocalAdministrations))
                {
                    result.LocalAdministrationCountByEntity[keyValuePair.Key] = keyValuePair.Value;
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        private void mnuWikipediaGerman_Click(Object sender, EventArgs e)
        {
            var numberStrings = new Dictionary<Int32, String>() {
                { 1, "eine" },
                { 2, "zwei" },
                { 3, "drei" },
                { 4, "vier" },
                { 5, "fünf" },
                { 6, "sechs" },
                { 7, "sieben" },
                { 8, "acht" },
                { 9, "neun" },
                { 10, "zehn" },
                { 11, "elf" },
                { 12, "zwölf" },
            };
            var wikipediaLink = new Dictionary<EntityType, String>()
            {
                {EntityType.ThesabanNakhon, "[[Thesaban#Großstadt|Thesaban Nakhon]]"},
                {EntityType.ThesabanMueang, "[[Thesaban#Stadt|Thesaban Mueang]]"},
                {EntityType.ThesabanTambon, "[[Thesaban#Kleinstadt|Thesaban Tambon]]"},
                {EntityType.TAO, "[[Verwaltungsgliederung Thailands#Tambon-Verwaltungsorganisationen|Tambon-Verwaltungsorganisationen]]"},
            };

            var selectedNode = treeviewSelection.SelectedNode;
            var entity = (Entity)(selectedNode.Tag);

            if (entity.type.IsCompatibleEntityType(EntityType.Amphoe))
            {
                var amphoeData = CalculateAmphoeData(entity, Language.German);
                var germanCulture = new CultureInfo("de-DE");

                String headerBangkok = "== Verwaltung ==" + Environment.NewLine;
                String textBangkok = "Der Bezirk {0} ist in {1} ''[[Khwaeng]]'' („Unterbezirke“) eingeteilt." + Environment.NewLine + Environment.NewLine;
                String headerAmphoe = "== Verwaltung ==" + Environment.NewLine + "=== Provinzverwaltung ===" + Environment.NewLine;
                String textAmphoe = "Der Landkreis {0} ist in {1} ''[[Tambon]]'' („Unterbezirke“ oder „Gemeinden“) eingeteilt, die sich weiter in {2} ''[[Muban]]'' („Dörfer“) unterteilen." + Environment.NewLine + Environment.NewLine;
                String textAmphoeSingle = "Der Landkreis {0} ist in genau einen ''[[Tambon]]'' („Unterbezirk“ oder „Gemeinde“) eingeteilt, der sich weiter in {2} ''[[Muban]]'' („Dörfer“) unterteilt." + Environment.NewLine + Environment.NewLine;
                String tableHeaderAmphoe =
                    "{{| class=\"wikitable\"" + Environment.NewLine +
                    "! Nr." + Environment.NewLine +
                    "! Name" + Environment.NewLine +
                    "! Thai" + Environment.NewLine +
                    "! Muban" + Environment.NewLine +
                    "! Einw.{0}" + Environment.NewLine;
                String tableHeaderBangkok =
                    "{{| class=\"wikitable\"" + Environment.NewLine +
                    "! Nr." + Environment.NewLine +
                    "! Name" + Environment.NewLine +
                    "! Thai" + Environment.NewLine +
                    "! Einw.{0}" + Environment.NewLine;
                String tableEntryAmphoe = "|-" + Environment.NewLine +
                    "||{0}.||{1}||{{{{lang|th|{2}}}}}||{3}||{4}" + Environment.NewLine;
                String tableEntryBangkok = "|-" + Environment.NewLine +
                    "||{0}.||{1}||{{{{lang|th|{2}}}}}||{4}" + Environment.NewLine;
                String tableFooter = "|}" + Environment.NewLine;

                String headerLocal = "=== Lokalverwaltung ===" + Environment.NewLine;
                String textLocalSingular = "Es gibt eine Kommune mit „{0}“-Status ''({1})'' im Landkreis:" + Environment.NewLine;
                String textLocalPlural = "Es gibt {0} Kommunen mit „{1}“-Status ''({2})'' im Landkreis:" + Environment.NewLine;
                String taoWithThesaban = "Außerdem gibt es {0} „[[Verwaltungsgliederung Thailands#Tambon-Verwaltungsorganisationen|Tambon-Verwaltungsorganisationen]]“ ({{{{lang|th|องค์การบริหารส่วนตำบล}}}} – Tambon Administrative Organizations, TAO)" + Environment.NewLine;
                String taoWithoutThesaban = "Im Landkreis gibt es {0} „[[Verwaltungsgliederung Thailands#Tambon-Verwaltungsorganisationen|Tambon-Verwaltungsorganisationen]]“ ({{{{lang|th|องค์การบริหารส่วนตำบล}}}} – Tambon Administrative Organizations, TAO)" + Environment.NewLine;
                String entryLocal = "* {0} (Thai: {{{{lang|th|{1}}}}})";
                String entryLocalCoverage = " bestehend aus {0}.";
                String entryLocalCoverageTwo = " bestehend aus {0} und {1}.";
                String tambonCompleteSingular = "dem kompletten Tambon {0}";
                String tambonPartiallySingular = "Teilen des Tambon {0}";
                String tambonCompletePlural = "den kompletten Tambon {0}";
                String tambonPartiallyPlural = "den Teilen der Tambon {0}";

                CountAsString countAsString = delegate(Int32 count)
                {
                    String countAsStringResult;
                    if (!numberStrings.TryGetValue(count, out countAsStringResult))
                    {
                        countAsStringResult = count.ToString(germanCulture);
                    }
                    return countAsStringResult;
                };

                var result = String.Empty;
                if (entity.type == EntityType.Khet)
                {
                    result = headerBangkok +
                        String.Format(germanCulture, textBangkok, entity.english, countAsString(amphoeData.CentralAdministrationCountByEntity[EntityType.Khwaeng])) +
                        String.Format(germanCulture, tableHeaderBangkok, PopulationDataDownloader.WikipediaReference(GeocodeHelper.ProvinceCode(entity.geocode), PopulationReferenceYear, Language.German));
                }
                else if (amphoeData.CentralAdministrationCountByEntity[EntityType.Tambon] == 1)
                {
                    result = headerAmphoe +
                        String.Format(germanCulture, textAmphoeSingle, entity.english, countAsString(amphoeData.CentralAdministrationCountByEntity[EntityType.Tambon]), countAsString(amphoeData.CentralAdministrationCountByEntity[EntityType.Muban])) +
                        String.Format(germanCulture, tableHeaderAmphoe, PopulationDataDownloader.WikipediaReference(GeocodeHelper.ProvinceCode(entity.geocode), PopulationReferenceYear, Language.German));
                }
                else
                {
                    result = headerAmphoe +
                        String.Format(germanCulture, textAmphoe, entity.english, countAsString(amphoeData.CentralAdministrationCountByEntity[EntityType.Tambon]), countAsString(amphoeData.CentralAdministrationCountByEntity[EntityType.Muban])) +
                        String.Format(germanCulture, tableHeaderAmphoe, PopulationDataDownloader.WikipediaReference(GeocodeHelper.ProvinceCode(entity.geocode), PopulationReferenceYear, Language.German));
                }

                foreach (var tambon in amphoeData.AllTambon)
                {
                    if (entity.type == EntityType.Khet)
                    {
                        result += WikipediaTambonTableEntry(tambon, amphoeData, tableEntryBangkok, germanCulture);
                    }
                    else
                    {
                        result += WikipediaTambonTableEntry(tambon, amphoeData, tableEntryAmphoe, germanCulture);
                    }
                }
                result += tableFooter + Environment.NewLine;

                if (amphoeData.LocalAdministrationCountByEntity.Any())
                {
                    result += headerLocal;
                    var check = new List<EntityType>()
                {
                    EntityType.ThesabanNakhon,
                    EntityType.ThesabanMueang,
                    EntityType.ThesabanTambon,
                    EntityType.TAO,
                };
                    foreach (var entityType in check)
                    {
                        Int32 count = 0;
                        if (amphoeData.LocalAdministrationCountByEntity.TryGetValue(entityType, out count))
                        {
                            if (entityType == EntityType.TAO)
                            {
                                if (amphoeData.LocalAdministrationCountByEntity.Keys.Count == 1)
                                {
                                    result += String.Format(germanCulture, taoWithoutThesaban, countAsString(count));
                                }
                                else
                                {
                                    result += String.Format(germanCulture, taoWithThesaban, countAsString(count));
                                }
                            }
                            else
                            {
                                if (count == 1)
                                {
                                    result += String.Format(germanCulture, textLocalSingular, entityType.Translate(Language.German), wikipediaLink[entityType]);
                                }
                                else
                                {
                                    result += String.Format(germanCulture, textLocalPlural, countAsString(count), entityType.Translate(Language.German), wikipediaLink[entityType]);
                                }
                            }
                            foreach (var localEntity in amphoeData.LocalAdministrations.Where(x => x.type == entityType))
                            {
                                result += WikipediaLocalAdministrationTableEntry(
                                    localEntity,
                                    amphoeData,
                                    entryLocal,
                                    tambonCompleteSingular,
                                    tambonCompletePlural,
                                    tambonPartiallySingular,
                                    tambonPartiallyPlural,
                                    entryLocalCoverage,
                                    entryLocalCoverageTwo,
                                    germanCulture);
                            }
                            result += Environment.NewLine;
                        }
                    }
                }

                Boolean success = false;
                while (!success)
                {
                    try
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(result);
                        success = true;
                    }
                    catch
                    {
                        if (MessageBox.Show(this, "Copying text to clipboard failed.", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) != DialogResult.Retry)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private String WikiLink(String link, String title)
        {
            if (link == title)
            {
                return "[[" + title + "]]";
            }
            else
            {
                return "[[" + link + "|" + title + "]]";
            }
        }

        private String WikipediaTambonTableEntry(Entity tambon, AmphoeDataForWikipediaExport amphoeData, String format, CultureInfo culture)
        {
            var subCounted = CountSubdivisions(tambon);
            var muban = 0;
            if (!subCounted.TryGetValue(EntityType.Muban, out muban))
            {
                muban = 0;
            }
            var citizen = 0;
            var populationData = tambon.population.FirstOrDefault(x => x.Year == PopulationReferenceYear && x.source == PopulationDataSource);
            if (populationData != null)
            {
                citizen = populationData.TotalPopulation.total;
            }
            var geocodeString = (tambon.geocode % 100).ToString(culture);
            if (tambon.geocode % 100 < 10)
            {
                geocodeString = "{{0}}" + geocodeString;
            }
            String mubanString;
            if (muban == 0)
            {
                mubanString = "-";
            }
            else if (muban < 10)
            {
                mubanString = "{{0}}" + muban.ToString(culture);
            }
            else
            {
                mubanString = muban.ToString();
            }
            var citizenString = citizen.ToString("###,##0", culture);
            for (int i = citizenString.Length; i < amphoeData.MaxPopulation.ToString("###,##0", culture).Length; i++)
            {
                citizenString = "{{0}}" + citizenString;
            }
            var romanizedName = tambon.english;
            var link = String.Empty;
            if (amphoeData.WikipediaLinks.TryGetValue(tambon, out link))
            {
                romanizedName = WikiLink(link, romanizedName);
            }

            return String.Format(culture, format, geocodeString, romanizedName, tambon.name, mubanString, citizenString);
        }

        private String WikipediaLocalAdministrationTableEntry(
            Entity localEntity,
            AmphoeDataForWikipediaExport amphoeData,
            String entryLocal,
            String tambonCompleteSingular,
            String tambonCompletePlural,
            String tambonPartiallySingular,
            String tambonPartiallyPlural,
            String entryLocalCoverageOne,
            String entryLocalCoverageTwo,
            CultureInfo culture)
        {
            var result = String.Empty;
            var english = localEntity.english;
            var link = String.Empty;
            if (amphoeData.WikipediaLinks.TryGetValue(localEntity, out link))
            {
                english = WikiLink(link, english);
            }
            result += String.Format(culture, entryLocal, english, localEntity.FullName);
            if (localEntity.LocalGovernmentAreaCoverage.Any())
            {
                var coverage = localEntity.LocalGovernmentAreaCoverage.GroupBy(x => x.coverage).Select(group => new
                {
                    Coverage = group.Key,
                    TambonCount = group.Count()
                });
                var textComplete = String.Empty;
                var textPartially = String.Empty;

                if (coverage.Any(x => x.Coverage == CoverageType.completely))
                {
                    var completeTambon = localEntity.LocalGovernmentAreaCoverage.
                        Where(x => x.coverage == CoverageType.completely).
                        Select(x => amphoeData.Province.FlatList().FirstOrDefault(y => y.geocode == x.geocode));
                    var tambonString = String.Join(", ", completeTambon.Select(x => x.english));
                    if (coverage.First(x => x.Coverage == CoverageType.completely).TambonCount == 1)
                    {
                        textComplete = String.Format(culture, tambonCompleteSingular, tambonString);
                    }
                    else
                    {
                        textComplete = String.Format(culture, tambonCompletePlural, tambonString);
                    }
                }
                if (coverage.Any(x => x.Coverage == CoverageType.partially))
                {
                    var completeTambon = localEntity.LocalGovernmentAreaCoverage.
                        Where(x => x.coverage == CoverageType.partially).
                        Select(x => amphoeData.Province.FlatList().FirstOrDefault(y => y.geocode == x.geocode));
                    var tambonString = String.Join(", ", completeTambon.Select(x => x.english));
                    if (coverage.First(x => x.Coverage == CoverageType.partially).TambonCount == 1)
                    {
                        textPartially = String.Format(culture, tambonPartiallySingular, tambonString);
                    }
                    else
                    {
                        textPartially = String.Format(culture, tambonPartiallyPlural, tambonString);
                    }
                }
                if (!String.IsNullOrEmpty(textPartially) && !String.IsNullOrEmpty(textComplete))
                {
                    result += String.Format(culture, entryLocalCoverageTwo, textComplete, textPartially);
                }
                else
                {
                    result += String.Format(culture, entryLocalCoverageOne, textComplete + textPartially);
                }
            }
            result += Environment.NewLine;
            return result;
        }

        private Dictionary<Entity, String> RetrieveWikpediaLinks(IEnumerable<Entity> entities, Language language)
        {
            var result = new Dictionary<Entity, String>();
            var api = new WikibaseApi("https://www.wikidata.org", "TambonBot");
            var helper = new WikiDataHelper(api);
            foreach (var entity in entities.Where(x => x.wiki != null && !String.IsNullOrEmpty(x.wiki.wikidata)))
            {
                var item = helper.GetWikiDataItemForEntity(entity);
                if (item != null)
                {
                    var links = item.getSitelinks();
                    String languageLink;
                    String wikiIdentifier = String.Empty;
                    switch (language)
                    {
                        case Language.German:
                            wikiIdentifier = "dewiki";
                            break;

                        case Language.English:
                            wikiIdentifier = "enwiki";
                            break;

                        case Language.Thai:
                            wikiIdentifier = "thwiki";
                            break;
                    }
                    if (item.getSitelinks().TryGetValue(wikiIdentifier, out languageLink))
                    {
                        result[entity] = languageLink;
                    }
                }
            }
            return result;
        }

        #endregion private methods

        private IEnumerable<GazetteEntry> AreaDefinitionAnnouncements(Entity entity)
        {
            var result = new List<GazetteEntry>();
            if (entity.type != EntityType.Country)
            {
                var allAboutGeocode = GlobalData.AllGazetteAnnouncements.AllGazetteEntries.Where(x =>
                    x.IsAboutGeocode(entity.geocode, true) || entity.OldGeocodes.Any(y => x.IsAboutGeocode(y, true)));
                var allAreaDefinitionAnnouncements = allAboutGeocode.Where(x => x.Items.Any(y => y is GazetteAreaDefinition));
                foreach (var announcement in allAreaDefinitionAnnouncements)
                {
                    var areaDefinitions = announcement.Items.Where(x => x is GazetteAreaDefinition);
                    if (areaDefinitions.Any(x => (x as GazetteAreaDefinition).IsAboutGeocode(entity.geocode, true) ||
                        entity.OldGeocodes.Any(y => (x as GazetteAreaDefinition).IsAboutGeocode(y, true))))
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
            foreach (var entry in AreaDefinitionAnnouncements(entity))
            {
                ShowPgf(entry);
            }
        }

        private void ShowPgf(GazetteEntry entry)
        {
            try
            {
                entry.MirrorToCache();
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                // TODO
                String pgfFilename = entry.LocalPdfFileName;

                if (File.Exists(pgfFilename))
                {
                    p.StartInfo.FileName = pgfFilename;
                    p.Start();
                }
            }
            catch
            {
                // throw;
            }
        }

        private void mnuWikipediaEnglish_Click(object sender, EventArgs e)
        {
            var selectedNode = treeviewSelection.SelectedNode;
            var entity = (Entity)(selectedNode.Tag);
            if (entity.type.IsCompatibleEntityType(EntityType.Amphoe))
            {
                var englishCulture = new CultureInfo("en-US");
                var amphoeData = CalculateAmphoeData(entity, Language.English);

                String headerBangkok = "== Administration ==" + Environment.NewLine;
                String textBangkok = "The district {0} is subdivided into {1} subdistricts (''[[Khwaeng]]'')." + Environment.NewLine + Environment.NewLine;
                String headerAmphoe = "== Administration ==" + Environment.NewLine + "=== Central administration ===" + Environment.NewLine;
                String textAmphoe = "The district {0} is subdivided into {1} subdistricts (''[[Tambon]]''), which are further subdivided into {2} administrative villages (''[[Muban]]'')." + Environment.NewLine + Environment.NewLine;
                String textAmphoeSingleTambon = "The district {0} is subdivided into {1} subdistrict (''[[Tambon]]''), which is further subdivided into {2} administrative villages (''[[Muban]]'')." + Environment.NewLine + Environment.NewLine;
                String tableHeaderAmphoe =
                    "{{| class=\"wikitable sortable\"" + Environment.NewLine +
                    "! No." + Environment.NewLine +
                    "! Name" + Environment.NewLine +
                    "! Thai" + Environment.NewLine +
                    "! Villages" + Environment.NewLine +
                    "! [[Population|Pop.]]{0}" + Environment.NewLine;
                String tableHeaderBangkok =
                    "{{| class=\"wikitable sortable\"" + Environment.NewLine +
                    "! No." + Environment.NewLine +
                    "! Name" + Environment.NewLine +
                    "! Thai" + Environment.NewLine +
                    "! [[Population|Pop.]]{0}" + Environment.NewLine;
                String tableEntryAmphoe = "|-" + Environment.NewLine +
                    "||{0}.||{1}||{{{{lang|th|{2}}}}}||{3}||{4}" + Environment.NewLine;
                String tableEntryBangkok = "|-" + Environment.NewLine +
                    "||{0}.||{1}||{{{{lang|th|{2}}}}}||{3}" + Environment.NewLine;
                String tableFooter = "|}" + Environment.NewLine;

                String headerLocal = "=== Local administration ===" + Environment.NewLine;
                String textLocalSingular = "There is one {0} in the district:" + Environment.NewLine;
                String textLocalPlural = "There are {0} {1} in the district:" + Environment.NewLine;
                String entryLocal = "* {0} (Thai: {{{{lang|th|{1}}}}})";
                String entryLocalCoverage = " consisting of {0}.";
                String entryLocalCoverageTwo = " consisting of {0} and {1}.";
                String tambonCompleteSingular = "the complete subdistrict {0}";
                String tambonPartiallySingular = "parts of the subdistrict {0}";
                String tambonCompletePlural = "the complete subdistrict {0}";
                String tambonPartiallyPlural = "parts of the subdistricts {0}";

                var enWikipediaLink = new Dictionary<EntityType, String>()
                {
                    {EntityType.ThesabanNakhon, "city (''[[Thesaban#City municipality|Thesaban Nakhon]]'')"},
                    {EntityType.ThesabanMueang, "town (''[[Thesaban#Town municipality|Thesaban Mueang]]'')"},
                    {EntityType.ThesabanTambon, "subdistrict municipality (''[[Thesaban#Subdistrict municipality|Thesaban Tambon]]'')"},
                    {EntityType.TAO, "[[Subdistrict administrative organization|subdistrict administrative organization (SAO)]]"},
                };
                var enWikipediaLinkPlural = new Dictionary<EntityType, String>()
                {
                    {EntityType.ThesabanNakhon, "cities (''[[Thesaban#City municipality|Thesaban Nakhon]]'')"},
                    {EntityType.ThesabanMueang, "towns (''[[Thesaban#Town municipality|Thesaban Mueang]]'')"},
                    {EntityType.ThesabanTambon, "subdistrict municipalities (''[[Thesaban#Subdistrict municipality|Thesaban Tambon]]'')"},
                    {EntityType.TAO, "[[Subdistrict administrative organization|subdistrict administrative organizations (SAO)]]"},
                };

                var result = String.Empty;
                if (entity.type == EntityType.Khet)
                {
                    result = headerBangkok +
                        String.Format(englishCulture, textBangkok, entity.english, amphoeData.CentralAdministrationCountByEntity[EntityType.Khwaeng]) +
                        String.Format(englishCulture, tableHeaderBangkok, PopulationDataDownloader.WikipediaReference(GeocodeHelper.ProvinceCode(entity.geocode), PopulationReferenceYear, Language.English));
                }
                else if (amphoeData.CentralAdministrationCountByEntity[EntityType.Tambon] == 1)
                {
                    result = headerAmphoe +
                        String.Format(englishCulture, textAmphoeSingleTambon, entity.english, amphoeData.CentralAdministrationCountByEntity[EntityType.Tambon], amphoeData.CentralAdministrationCountByEntity[EntityType.Muban]) +
                        String.Format(englishCulture, tableHeaderAmphoe, PopulationDataDownloader.WikipediaReference(GeocodeHelper.ProvinceCode(entity.geocode), PopulationReferenceYear, Language.English));
                }
                else
                {
                    result = headerAmphoe +
                        String.Format(englishCulture, textAmphoe, entity.english, amphoeData.CentralAdministrationCountByEntity[EntityType.Tambon], amphoeData.CentralAdministrationCountByEntity[EntityType.Muban]) +
                        String.Format(englishCulture, tableHeaderAmphoe, PopulationDataDownloader.WikipediaReference(GeocodeHelper.ProvinceCode(entity.geocode), PopulationReferenceYear, Language.English));
                }
                foreach (var tambon in amphoeData.AllTambon)
                {
                    if (entity.type == EntityType.Khet)
                    {
                        result += WikipediaTambonTableEntry(tambon, amphoeData, tableEntryBangkok, englishCulture);
                    }
                    else
                    {
                        result += WikipediaTambonTableEntry(tambon, amphoeData, tableEntryAmphoe, englishCulture);
                    }
                }
                result += tableFooter + Environment.NewLine;

                if (amphoeData.LocalAdministrationCountByEntity.Any())
                {
                    result += headerLocal;
                    var check = new List<EntityType>()
                {
                    EntityType.ThesabanNakhon,
                    EntityType.ThesabanMueang,
                    EntityType.ThesabanTambon,
                    EntityType.TAO,
                };
                    foreach (var entityType in check)
                    {
                        Int32 count = 0;
                        if (amphoeData.LocalAdministrationCountByEntity.TryGetValue(entityType, out count))
                        {
                            if (count == 1)
                            {
                                result += String.Format(englishCulture, textLocalSingular, enWikipediaLink[entityType]);
                            }
                            else
                            {
                                result += String.Format(englishCulture, textLocalPlural, count, enWikipediaLinkPlural[entityType]);
                            }
                            foreach (var localEntity in amphoeData.LocalAdministrations.Where(x => x.type == entityType))
                            {
                                result += WikipediaLocalAdministrationTableEntry(
                                    localEntity,
                                    amphoeData,
                                    entryLocal,
                                    tambonCompleteSingular,
                                    tambonCompletePlural,
                                    tambonPartiallySingular,
                                    tambonPartiallyPlural,
                                    entryLocalCoverage,
                                    entryLocalCoverageTwo,
                                    englishCulture);
                            }
                            result += Environment.NewLine;
                        }
                    }
                }

                Boolean success = false;
                while (!success)
                {
                    try
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(result);
                        success = true;
                    }
                    catch
                    {
                        if (MessageBox.Show(this, "Copying text to clipboard failed.", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) != DialogResult.Retry)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    internal class AmphoeDataForWikipediaExport
    {
        public Dictionary<Entity, String> WikipediaLinks
        {
            get;
            private set;
        }

        public Dictionary<EntityType, Int32> CentralAdministrationCountByEntity
        {
            get;
            private set;
        }

        public Dictionary<EntityType, Int32> LocalAdministrationCountByEntity
        {
            get;
            private set;
        }

        public Int32 MaxPopulation
        {
            get;
            set;
        }

        public List<Entity> AllTambon
        {
            get;
            private set;
        }

        public List<Entity> LocalAdministrations
        {
            get;
            private set;
        }

        public Entity Province
        {
            get;
            set;
        }

        public AmphoeDataForWikipediaExport()
        {
            WikipediaLinks = new Dictionary<Entity, String>();
            CentralAdministrationCountByEntity = new Dictionary<EntityType, Int32>();
            LocalAdministrationCountByEntity = new Dictionary<EntityType, Int32>();
            AllTambon = new List<Entity>();
            LocalAdministrations = new List<Entity>();
        }
    }
}