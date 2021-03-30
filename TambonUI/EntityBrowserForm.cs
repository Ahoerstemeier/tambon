using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace De.AHoerstemeier.Tambon.UI
{
    public partial class EntityBrowserForm : Form
    {
        #region fields

        private readonly List<Entity> _localGovernments = new List<Entity>();
        private Entity _baseEntity;
        private List<Entity> _allEntities;
        private Dictionary<UInt32, HistoryList> _creationHistories;

        #endregion fields

        #region properties

        /// <summary>
        /// Gets or sets whether erroneous DOLA codes shall be included in the invalid data list.
        /// </summary>
        /// <value><c>true</c> to include wrong DOLA codes in the invalid data list, <c>false</c> otherwise.</value>
        public Boolean ShowDolaErrors
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the geocode of the province which should be selected upon opening the view.
        /// </summary>
        /// <value>The geocode of the province which should be selected upon opening the view.</value>
        public UInt32 StartChangwatGeocode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data source to be used for the population data.
        /// </summary>
        /// <value>The data source to be used for the population data.</value>
        public PopulationDataSourceType PopulationDataSource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the reference year to be used for the population data.
        /// </summary>
        /// <value>The reference year to be used for the population data.</value>
        public Int16 PopulationReferenceYear
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether wikidata should be checked to include Wikipedia links.
        /// </summary>
        /// <value><c>true</c> to check Wikidata for links, <c>false</c> otherwise.</value>
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
            PopulationReferenceYear = GlobalData.PopulationStatisticMaxYear;
        }

        #endregion constructor

        #region private methods

        private TreeNode EntityToTreeNode(Entity data)
        {
            TreeNode retval = null;
            if (data != null)
            {
                retval = new TreeNode(data.english)
                {
                    Tag = data
                };
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

        private void CalcElectionData(Entity entity)
        {
            var localGovernmentsInEntity = entity.LocalGovernmentEntitiesOf(_localGovernments);
            var dummyEntity = new Entity();
            dummyEntity.entity.AddRange(localGovernmentsInEntity);

            var itemsWithCouncilElectionsPending = new List<EntityTermEnd>();
            var itemsWithOfficialElectionsPending = new List<EntityTermEnd>();
            var itemsWithOfficialElectionResultUnknown = new List<EntityTermEnd>();
            var itemsWithOfficialVacant = new List<EntityTermEnd>();

            var itemsWithCouncilElectionPendingInParent = dummyEntity.EntitiesWithCouncilElectionPending();
            itemsWithCouncilElectionsPending.AddRange(itemsWithCouncilElectionPendingInParent);
            itemsWithCouncilElectionsPending.Sort((x, y) => x.CouncilTerm.begin.CompareTo(y.CouncilTerm.begin));

            var itemsWithOfficialElectionPendingInParent = dummyEntity.EntitiesWithOfficialElectionPending(false);
            itemsWithOfficialElectionsPending.AddRange(itemsWithOfficialElectionPendingInParent);
            itemsWithOfficialElectionsPending.Sort((x, y) => x.OfficialTerm.begin.CompareTo(y.OfficialTerm.begin));

            var itemsWithOfficialElectionResultUnknownInParent = dummyEntity.EntitiesWithLatestOfficialElectionResultUnknown();
            itemsWithOfficialElectionResultUnknown.AddRange(itemsWithOfficialElectionResultUnknownInParent);
            itemsWithOfficialElectionResultUnknown.Sort((x, y) => x.OfficialTerm.begin.CompareTo(y.OfficialTerm.begin));

            var itemsWithOfficialVacantInParent = dummyEntity.EntitiesWithOfficialVacant();
            itemsWithOfficialVacant.AddRange(itemsWithOfficialVacantInParent);

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
                    String.Format(CultureInfo.CurrentUICulture, "{0} LAO council elections pending", councilCount) + Environment.NewLine +
                    councilBuilder.ToString() + Environment.NewLine;
            }

            var vacantBuilder = new StringBuilder();
            Int32 vacantCount = 0;
            foreach (var item in itemsWithOfficialVacant)
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
                vacantBuilder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2}", item.Entity.english, item.Entity.geocode, officialTermEnd);
                vacantBuilder.AppendLine();
                vacantCount++;
            }
            if (vacantCount > 0)
            {
                result +=
                    String.Format(CultureInfo.CurrentUICulture, "{0} LAO official vacant", vacantCount) + Environment.NewLine +
                    vacantBuilder.ToString() + Environment.NewLine;
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
                    String.Format(CultureInfo.CurrentUICulture, "{0} LAO official elections pending", officialCount) + Environment.NewLine +
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
                    String.Format(CultureInfo.CurrentUICulture, "{0} LAO official elections result missing", officialUnknownCount) + Environment.NewLine +
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
            var localGovernmentsInEntity = entity.LocalGovernmentEntitiesOf(_localGovernments).ToList();
            // var localGovernmentsInProvince = LocalGovernmentEntitiesOf(this.baseEntity.entity.First(x => x.geocode == GeocodeHelper.ProvinceCode(entity.geocode))).ToList();
            var localEntitiesWithOffice = localGovernmentsInEntity.Where(x => x.Dola != null && !x.IsObsolete).ToList();  // Dola != null when there is a local government office
            // var localEntitiesInProvinceWithOffice = localGovernmentsInProvince.Where(x => x.Dola != null && !x.IsObsolete).ToList();  // Dola != null when there is a local government office
            if (ShowDolaErrors)
            {
                var entitiesWithDolaCode = localEntitiesWithOffice.Where(x => x.LastedDolaCode() != 0).ToList();
                var allDolaCodes = entitiesWithDolaCode.Select(x => x.LastedDolaCode()).ToList();
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
                        text += String.Format(" {0} {1} ({2})", dolaEntity.LastedDolaCode(), dolaEntity.english, dolaEntity.type) + Environment.NewLine;
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
            var selfNeighbors = new List<UInt32>();
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
                if (entityWithNeighbors.area.bounding.Any(x => x.geocode == entityWithNeighbors.geocode))
                {
                    selfNeighbors.Add(entityWithNeighbors.geocode);
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
            if (selfNeighbors.Any())
            {
                text += String.Format("Neighboring entities includes self ({0}):", selfNeighbors.Count()) + Environment.NewLine;
                foreach (var code in selfNeighbors)
                {
                    text += String.Format(" {0}", code) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }

            text += CheckCode(entity, new List<EntityType>() { EntityType.Changwat }, "FIPS10", (Entity x) => x.codes.fips10.value, "^TH\\d\\d$", true);
            text += CheckCode(entity, new List<EntityType>() { EntityType.Changwat }, "ISO3166", (Entity x) => x.codes.iso3166.value, "^TH-(\\d\\d|S)$", true);
            text += CheckCode(entity, new List<EntityType>() { EntityType.Changwat, EntityType.Amphoe }, "HASC", (Entity x) => x.codes.hasc.value, "^TH(\\.[A-Z]{2}){1,2}$", true);
            text += CheckCode(entity, new List<EntityType>() { EntityType.Changwat, EntityType.Amphoe }, "SALB", (Entity x) => x.codes.salb.value, "^THA(\\d{3}){1,2}$", false);
            text += CheckCode(entity, new List<EntityType>() { EntityType.Changwat, EntityType.Amphoe }, "GNS-UFI", (Entity x) => x.codes.gnsufi.value, "^[-]{0,1}\\d+$", true);
            text += CheckCode(entity, new List<EntityType>() { EntityType.Changwat, EntityType.Amphoe }, "WOEID", (Entity x) => x.codes.woeid.value, "^\\d+$", true);
            text += CheckCode(entity, new List<EntityType>() { EntityType.Changwat, EntityType.Amphoe }, "geonames", (Entity x) => x.codes.geonames.value, "^\\d+$", true);
            text += CheckCode(entity, new List<EntityType>() { EntityType.Changwat, EntityType.Amphoe, EntityType.Tambon }, "GADM", (Entity x) => x.codes.gadm.value, "^THA(\\.\\d{1,2}){1,3}_\\d$", false);

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

            var entitiesToCheckForHistory = entity.FlatList().Where(x => x.type.IsCompatibleEntityType(EntityType.Amphoe) || (x.type == EntityType.Tambon) || x.type.IsCompatibleEntityType(EntityType.Changwat));
            var entitiesToCheckWithCreationHistory = entitiesToCheckForHistory.Where(x => x.history.Items.Any(y => y is HistoryCreate));
            var entitiesCreationWithoutSubdivisions = entitiesToCheckWithCreationHistory.Where(x => (x.history.Items.FirstOrDefault(y => y is HistoryCreate) as HistoryCreate).subdivisions == 0);
            if (entitiesCreationWithoutSubdivisions.Any())
            {
                text += String.Format("Entities with creation but no subdivision number ({0}):", entitiesCreationWithoutSubdivisions.Count()) + Environment.NewLine;
                foreach (var item in entitiesCreationWithoutSubdivisions)
                {
                    text += String.Format(" {0}: {1}", item.geocode, item.english) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }
            var entitiesWithoutCreation = entity.FlatList().Where(x => !x.IsObsolete && !x.history.Items.Any(y => y is HistoryCreate));
            var entitiesWithGazetteCreation = entitiesWithoutCreation.Where(x => GazetteCreationHistory(x) != null);
            // remove those strange 1947 Tambon creation
            var entitiesWithGazetteCreationFiltered = entitiesWithGazetteCreation.Where(x => x.type != EntityType.Tambon || GazetteCreationHistory(x).effective.Year >= 1950);
            if (entitiesWithGazetteCreationFiltered.Any())
            {
                text += String.Format("Entities with creation missing ({0}):", entitiesWithGazetteCreationFiltered.Count()) + Environment.NewLine;
                foreach (var item in entitiesWithGazetteCreationFiltered)
                {
                    text += String.Format(" {0}: {1}", item.geocode, item.english) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }

            var entitiesWithArea2003 = entity.FlatList().Where(x => !x.IsObsolete && x.area.area.Any(y => y.year == "2003" && !y.invalid));
            foreach (var entityWithArea in entitiesWithArea2003)
            {
                var subEntitiesWithArea = entityWithArea.entity.Where(x => !x.IsObsolete && x.area.area.Any(y => y.year == "2003" && !y.invalid));
                if (subEntitiesWithArea.Any())
                {
                    Decimal area = 0;
                    foreach (var subEntity in subEntitiesWithArea)
                    {
                        area += subEntity.area.area.First(x => x.year == "2003" && !x.invalid).value;
                    }
                    var expected = entityWithArea.area.area.First(x => x.year == "2003").value;
                    if (area != expected)
                    {
                        text += String.Format("Area sum in 2003 not correct for {0} (expected {1}, actual {2}):", entityWithArea.english, expected, area) + Environment.NewLine;
                    }
                }
            }

            var laoWithoutWebId = localGovernmentsInEntity.Where(x => !x.IsObsolete && !x.office.First().webidSpecified).ToList();
            if (laoWithoutWebId.Any())
            {
                text += String.Format("Entities without DOLA web id ({0}):", laoWithoutWebId.Count()) + Environment.NewLine;
                foreach (var item in laoWithoutWebId)
                {
                    text += String.Format(" {0}: {1}", item.geocode, item.english) + Environment.NewLine;
                }
                text += Environment.NewLine;
            }

            // check area coverages
            txtErrors.Text = text;
        }

        private static Dictionary<UInt32, HistoryList> ExtractHistoriesFromGazette(Entity entity, IEnumerable<Entity> allEntities)
        {
            // converting to dictionary much faster than FirstOrDefault(x => x.geocode) for each item later
            // var allEntityDictionary = allEntities.ToDictionary(x => x.geocode);

            // TODO - area change as well
            // Use IEnumerable<>.OfType/Cast to speed it up?
            var result = new Dictionary<UInt32, HistoryList>();
            //var startingDate = new DateTime(1950, 1, 1);
            //var gazetteNewerThan1950 = GlobalData.AllGazetteAnnouncements.AllGazetteEntries.Where(x => x.publication > startingDate);
            // var gazetteWithCreationOrStatus = gazetteNewerThan1950.Where(x => x.Items.Any(y => y is GazetteCreate || y is GazetteStatusChange)).ToList();
            var gazetteContent = GlobalData.AllGazetteAnnouncements.AllGazetteEntries.SelectMany(x => x.GazetteOperations().OfType<GazetteOperationBase>().Where(y => y is GazetteCreate || y is GazetteStatusChange || y is GazetteRename || y is GazetteAreaChange || y is GazetteReassign || y is GazetteParentChange), (Gazette, History) => new
            {
                Gazette,
                History
            }).ToList();
            // var gazetteContentInCurrentEntity = gazetteContent.Where(x => GeocodeHelper.IsBaseGeocode(entity.geocode, ((GazetteOperationBase)x.History).geocode)).ToList();

            foreach (var gazetteHistoryTuple in gazetteContent)
            {
                var gazetteOperation = gazetteHistoryTuple.History;
                if (gazetteOperation != null)
                {
                    UInt32 geocode = 0;
                    if (gazetteOperation.tambonSpecified)
                    {
                        geocode = gazetteOperation.tambon + 50;
                    }
                    if (gazetteOperation.geocodeSpecified)
                    {
                        geocode = gazetteOperation.geocode;
                    }
                    if (geocode != 0)
                    {
                        HistoryEntryBase history = gazetteOperation.ConvertToHistory();

                        if (history != null)
                        {
                            history.AddGazetteReference(gazetteHistoryTuple.Gazette);
                            var doAdd = true;
                            if ((GeocodeHelper.GeocodeLevel(geocode) == 4) && (history is HistoryReassign))
                            {
                                // skip the reassign for Muban as the Muban geocodes are not stable!
                                doAdd = false;
                            }
                            if (doAdd)
                            {
                                if (!result.Keys.Contains(geocode))
                                {
                                    result[geocode] = new HistoryList();
                                }
                                result[geocode].Items.Add(history);
                                result[geocode].Items.Sort((x, y) => y.effective.CompareTo(x.effective));
                            }
                        }
                    }
                }
            }
            return result;
        }

        private String CheckCode(Entity entity, IEnumerable<EntityType> entityTypes, String codeName, Func<Entity, String> selector, String format, Boolean checkMissing)
        {
            String text = String.Empty;
            var allEntites = entity.FlatList().Where(x => !x.IsObsolete);
            var allEntityOfFittingType = allEntites.Where(x => x.type.IsCompatibleEntityType(entityTypes));
            var entitiesWithoutCode = allEntityOfFittingType.Where(x => String.IsNullOrEmpty(selector(x)));
            if (checkMissing && entitiesWithoutCode.Any())
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
                result += String.Format("Median Muban number: {0:0.0}", counter.MedianValue) + Environment.NewLine;
                if (counter.Data.TryGetValue(0, out List<UInt32> tambonWithNoMuban))
                {
                    result += String.Format("Tambon without Muban: {0}", tambonWithNoMuban.Count) + Environment.NewLine;
                }
            }
            var mubanCreatedRecently = allMuban.Where(x => x.history.Items.Any(y => y is HistoryCreate)).ToList();
            if (mubanCreatedRecently.Any())
            {
                result += String.Format("Muban created recently: {0}", mubanCreatedRecently.Count) + Environment.NewLine;
                var mubanByYear = mubanCreatedRecently.GroupBy(x => ((HistoryCreate)(x.history.Items.First(y => y is HistoryCreate))).effective.Year).OrderBy(x => x.Key);
                foreach (var item in mubanByYear)
                {
                    result += String.Format("  {0}: {1}", item.Key, item.Count()) + Environment.NewLine;
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

            var localGovernmentsInEntity = entity.LocalGovernmentEntitiesOf(_localGovernments).ToList();
            // var localGovernmentsInProvince = LocalGovernmentEntitiesOf(this.baseEntity.entity.First(x => x.geocode == GeocodeHelper.ProvinceCode(entity.geocode))).ToList();
            var localGovernmentsObsolete = localGovernmentsInEntity.Where(x => x.IsObsolete);
            var localEntitiesWithOffice = localGovernmentsInEntity.Where(x => x.Dola != null && !x.IsObsolete).ToList();  // Dola != null when there is a local government office
            // var localEntitiesInProvinceWithOffice = localGovernmentsInProvince.Where(x => x.Dola != null && !x.IsObsolete).ToList();  // Dola != null when there is a local government office
            var localEntitiesWithCoverage = localEntitiesWithOffice.Where(x => x.LocalGovernmentAreaCoverage.Any());
            var localEntitiesWithoutCoverage = localEntitiesWithOffice.Where(x => !x.LocalGovernmentAreaCoverage.Any() && x.type != EntityType.PAO);

            var localGovernmentCoveringMoreThanOneTambon = localEntitiesWithCoverage.Where(x => x.LocalGovernmentAreaCoverage.Count() > 1);
            var localGovernmentCoveringExactlyOneTambon = localEntitiesWithCoverage.Where(x => x.LocalGovernmentAreaCoverage.Count() == 1 && x.LocalGovernmentAreaCoverage.First().coverage == CoverageType.completely);
            var localGovernmentCoveringOneTambonPartially = localEntitiesWithCoverage.Where(x => x.LocalGovernmentAreaCoverage.Count() == 1 && x.LocalGovernmentAreaCoverage.First().coverage == CoverageType.partially);
            var localGovernmentCoveringMoreThanOneTambonAndAllCompletely = localGovernmentCoveringMoreThanOneTambon.Where(x => x.LocalGovernmentAreaCoverage.All(y => y.coverage == CoverageType.completely));

            result += String.Format(CultureInfo.CurrentUICulture, "LAO: {0}", localEntitiesWithOffice.Count()) + Environment.NewLine;
            if (localGovernmentsObsolete.Any())
            {
                result += String.Format(CultureInfo.CurrentUICulture, "Abolished LAO: {0}", localGovernmentsObsolete.Count()) + Environment.NewLine;
            }
            result += String.Format(CultureInfo.CurrentUICulture, "LAO with coverage: {0}", localEntitiesWithCoverage.Count()) + Environment.NewLine;
            if (localEntitiesWithoutCoverage.Any())
            {
                result += String.Format(CultureInfo.CurrentUICulture, "LAO missing coverage: {0}", localEntitiesWithoutCoverage.Count()) + Environment.NewLine;
            }
            result += String.Format(CultureInfo.CurrentUICulture, "LAO covering exactly one Tambon: {0}", localGovernmentCoveringExactlyOneTambon.Count()) + Environment.NewLine;
            result += String.Format(CultureInfo.CurrentUICulture, "LAO covering one Tambon partially: {0}", localGovernmentCoveringOneTambonPartially.Count()) + Environment.NewLine;
            result += String.Format(CultureInfo.CurrentUICulture, "LAO covering more than one Tambon: {0} ({1} TAO)",
                localGovernmentCoveringMoreThanOneTambon.Count(),
                localGovernmentCoveringMoreThanOneTambon.Where(x => x.type == EntityType.TAO).Count()) + Environment.NewLine;
            result += String.Format(CultureInfo.CurrentUICulture, "LAO covering more than one Tambon and all completely: {0} ({1} TAO)",
                localGovernmentCoveringMoreThanOneTambonAndAllCompletely.Count(),
                localGovernmentCoveringMoreThanOneTambonAndAllCompletely.Where(x => x.type == EntityType.TAO).Count()) + Environment.NewLine + Environment.NewLine;

            var localEntitiesWithVacantMayor = localEntitiesWithOffice.Where(x => x.office.First().officials.OfficialTermsOrVacancies.FirstOrDefault() as OfficialVacancy != null);
            if (localEntitiesWithVacantMayor.Any())
            {
                result += String.Format(CultureInfo.CurrentUICulture, "LAO with vacant mayor: {0} ({1} TAO)",
                localEntitiesWithVacantMayor.Count(),
                localEntitiesWithVacantMayor.Where(x => x.type == EntityType.TAO).Count()) + Environment.NewLine;
            }
            var localEntitiesWithVacantCouncil = localEntitiesWithOffice.Where(x => x.office.First().council.CouncilTermsOrVacancies.FirstOrDefault() as CouncilVacancy != null);
            if (localEntitiesWithVacantCouncil.Any())
            {
                result += String.Format(CultureInfo.CurrentUICulture, "LAO with vacant council: {0} ({1} TAO)",
                   localEntitiesWithVacantCouncil.Count(),
                   localEntitiesWithVacantCouncil.Where(x => x.type == EntityType.TAO).Count()) + Environment.NewLine;
            }
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
            if (localGovernmentWithoutLatestHistory.Count() > 0)
            {
                result += String.Format(CultureInfo.CurrentUICulture, "LAO without latest history: {0} ({1} TAO)",
                   localGovernmentWithoutLatestHistory.Count(),
                   localGovernmentWithoutLatestHistory.Where(x => x.type == EntityType.TAO).Count()) + Environment.NewLine;
            }
            var localGovernmentWithoutCreation = localGovernmentExpectingHistory.Where(x =>
                !x.history.Items.Any(y => y is HistoryCreate)).ToList();
            if (localGovernmentWithoutCreation.Count() > 0)
            {
                result += String.Format(CultureInfo.CurrentUICulture, "LAO without creation history: {0} ({1} TAO)",
                   localGovernmentWithoutCreation.Count(),
                   localGovernmentWithoutCreation.Where(x => x.type == EntityType.TAO).Count()) + Environment.NewLine;
            }

            // initialize empty SML data dictionary
            var sml = new Dictionary<EntityType, Dictionary<SmallMediumLarge, Int32>>();
            var smlTypes = new List<SmallMediumLarge>() { SmallMediumLarge.L, SmallMediumLarge.M, SmallMediumLarge.S, SmallMediumLarge.Undefined };
            var laoTypes = new List<EntityType>() { EntityType.ThesabanNakhon, EntityType.ThesabanMueang, EntityType.ThesabanTambon, EntityType.TAO, EntityType.Mueang, EntityType.SpecialAdministrativeUnit, EntityType.PAO };
            foreach (var laoType in laoTypes)
            {
                var smlData = new Dictionary<SmallMediumLarge, Int32>();
                foreach (var smlType in smlTypes)
                {
                    smlData[smlType] = 0;
                }
                sml[laoType] = smlData;
            }
            // fill SML data
            foreach (var lao in localGovernmentsInEntity)
            {
                if (lao.Dola != null)
                {
                    var dola = lao.Dola.FirstOrDefault();
                    if (dola != null)
                    {
                        {
                            sml[lao.type][dola.SML]++;
                        }
                    }
                }
            }
            // Convert to display
            foreach (var laoType in laoTypes)
            {
                var smlData = sml[laoType];
                var count = 0;
                foreach (var smlType in smlTypes)
                {
                    count += smlData[smlType];
                }
                if (count > 0)
                {
                    var data = String.Empty;
                    foreach (var smlType in smlTypes)
                    {
                        if (smlData[smlType] > 0)
                        {
                            data += String.Format(CultureInfo.CurrentUICulture, "{0}: {1}, ", smlType, smlData[smlType]);
                        }
                    }
                    data = data.Remove(data.Length - 2, 2);
                    if (laoType != EntityType.PAO)
                    {
                        result += String.Format(CultureInfo.CurrentUICulture, "{0}: {1}", laoType, data) + Environment.NewLine;
                    }
                }
            }

            txtLocalGovernment.Text = result;
        }

        private void CalcLocalGovernmentConstituencies(Entity entity)
        {
            String result = String.Empty;

            var localGovernmentsInEntity = entity.LocalGovernmentEntitiesOf(_localGovernments).Where(x => !x.IsObsolete).ToList();
            var gazetteConstituency = GlobalData.AllGazetteAnnouncements.AllGazetteEntries.Where(x => x.GazetteOperations().Any(y => y is GazetteConstituency)).ToList();

            var latestConstituencyGazettes = new List<(Entity Entity, GazetteEntry Gazette)>();
            var laoWithoutConstituency = new List<Entity>();

            foreach (var lao in localGovernmentsInEntity)
            {
                var gazette = gazetteConstituency.Where(x => x.GazetteOperations().Any(y => y is GazetteConstituency && (y.IsAboutGeocode(lao.geocode, false) || (lao.tambonSpecified && y.IsAboutGeocode(lao.tambon, false))))).OrderBy(x => x.publication);
                if (gazette.Any())
                {
                    latestConstituencyGazettes.Add((lao, gazette.Last()));
                }
                else if (lao.type != EntityType.TAO)
                {
                    laoWithoutConstituency.Add(lao);
                }
            }
            var latestConstituencyGazettesSorted = latestConstituencyGazettes.OrderBy(x => x.Gazette.publication);
            foreach (var item in latestConstituencyGazettesSorted)
            {
                result += String.Format(CultureInfo.CurrentUICulture, "{0} ({1}): {2:yyyy}", item.Entity.english, item.Entity.geocode, item.Gazette.publication) + Environment.NewLine;
            }
            if (laoWithoutConstituency.Any())
            {
                result += Environment.NewLine + "No constituency:" + Environment.NewLine;
                foreach (var item in laoWithoutConstituency)
                {
                    result += String.Format(CultureInfo.CurrentUICulture, "{0} ({1})", item.english, item.geocode) + Environment.NewLine;
                }
            }

            txtConstituency.Text = result;
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

        private String EntitySubDivisionCount(Entity entity)
        {
            var counted = entity.CountAllSubdivisions(_localGovernments);
            var noLocation = entity.CountSubdivisionsWithoutLocation(_localGovernments);

            var result = String.Empty;
            foreach (var keyvaluepair in counted)
            {
                if (noLocation.TryGetValue(keyvaluepair.Key, out Int32 noLocationCount))
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
                item.Tag = subEntity;
                item.SubItems.Add(subEntity.name);
                item.SubItems.Add(subEntity.geocode.ToString(CultureInfo.CurrentUICulture));
                AddPopulationToItems(subEntity, item);
                AddCreationDateToItems(entity, subEntity, item);
            }
            listviewCentralAdministration.EndUpdate();
        }

        private void AddPopulationToItems(Entity subEntity, ListViewItem item)
        {
            var populationData = subEntity.population.FirstOrDefault(x => x.Year == PopulationReferenceYear && x.source == PopulationDataSource);
            if (populationData != null && populationData.TotalPopulation != null)
            {
                item.SubItems.Add(populationData.TotalPopulation.total.ToString(CultureInfo.CurrentUICulture));
            }
            else
            {
                item.SubItems.Add(String.Empty);
            }
        }

        private void AddCreationDateToItems(Entity entity, Entity subEntity, ListViewItem item)
        {
            if (subEntity.history.Items.FirstOrDefault(x => x is HistoryCreate) is HistoryCreate creationHistory)
            {
                item.SubItems.Add(creationHistory.effective.ToString("yyyy-MM-dd", CultureInfo.CurrentUICulture));
            }
            else
            {
                creationHistory = GazetteCreationHistory(subEntity);
                if (creationHistory != null)
                {
                    item.SubItems.Add("(" + creationHistory.effective.ToString("yyyy-MM-dd", CultureInfo.CurrentUICulture) + ")");
                }
                else
                {
                    item.SubItems.Add(String.Empty);
                }
            }
        }

        private HistoryCreate GazetteCreationHistory(Entity subEntity)
        {
            HistoryCreate creationHistory;
            var histories = new HistoryList();
            if (_creationHistories.Keys.Contains(subEntity.geocode))
            {
                histories.Items.AddRange(_creationHistories[subEntity.geocode].Items);
            }
            foreach (var oldGeocode in subEntity.OldGeocodes)
            {
                if (_creationHistories.Keys.Contains(oldGeocode))
                {
                    histories.Items.AddRange(_creationHistories[oldGeocode].Items);
                }
            }
            creationHistory = histories.Items.FirstOrDefault(x => x is HistoryCreate) as HistoryCreate;
            return creationHistory;
        }

        private void EntityToLocalAdministrativeListView(Entity entity)
        {
            listviewLocalAdministration.BeginUpdate();
            listviewLocalAdministration.Items.Clear();
            var localGovernmentsInEntity = entity.LocalGovernmentEntitiesOf(_localGovernments).ToList();
            foreach (Entity subEntity in localGovernmentsInEntity)
            {
                ListViewItem item = listviewLocalAdministration.Items.Add(subEntity.english);
                item.Tag = subEntity;
                item.SubItems.Add(subEntity.name);
                item.SubItems.Add(subEntity.type.ToString());
                if (subEntity.geocode > 9999)
                {
                    // generated geocode
                    item.SubItems.Add(String.Empty);
                }
                else
                {
                    item.SubItems.Add(subEntity.geocode.ToString(CultureInfo.CurrentUICulture));
                }
                String dolaCode = String.Empty;
                var office = subEntity.office.FirstOrDefault(x => x.type == OfficeType.TAOOffice || x.type == OfficeType.PAOOffice || x.type == OfficeType.MunicipalityOffice);
                if (office != null)
                {
                    var dolaEntry = office.dola.Where(x => x.codeSpecified).OrderBy(y => y.year).LastOrDefault();
                    if (dolaEntry != null)
                    {
                        dolaCode = dolaEntry.code.ToString(CultureInfo.CurrentUICulture);
                    }
                }
                AddPopulationToItems(subEntity, item);
                AddCreationDateToItems(entity, subEntity, item);
                var currentOfficialTerm = office.officials.OfficialTerms.OrderByDescending(x => x.begin).FirstOrDefault();
                item.SubItems.Add(currentOfficialTerm?.begin.ToString("yyyy-MM-dd") ?? String.Empty);
                var currentCouncilTerm = office.council.CouncilTerms.OrderByDescending(x => x.begin).FirstOrDefault();
                item.SubItems.Add(currentCouncilTerm?.begin.ToString("yyyy-MM-dd") ?? String.Empty);
                item.SubItems.Add(dolaCode);
            }
            listviewLocalAdministration.EndUpdate();
        }

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

        private void ShowPdf(GazetteEntry entry)
        {
            try
            {
                entry.MirrorToCache();
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                // TODO
                String pdfFilename = entry.LocalPdfFileName;

                if (File.Exists(pdfFilename))
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

        private void AmphoeToWikipedia(Language language)
        {
            var selectedNode = treeviewSelection.SelectedNode;
            var entity = (Entity)(selectedNode.Tag);
            if (entity.type.IsCompatibleEntityType(EntityType.Amphoe))
            {
                var exporter = new WikipediaExporter(_baseEntity, _localGovernments)
                {
                    CheckWikiData = CheckWikiData,
                    PopulationReferenceYear = PopulationReferenceYear,
                    PopulationDataSource = PopulationDataSource
                };
                var text = exporter.AmphoeToWikipedia(entity, language);

                CopyToClipboard(text);
            }
        }

        private void CopyToClipboard(String text)
        {
            Boolean success = false;
            while (!success)
            {
                try
                {
                    Clipboard.Clear();
                    Clipboard.SetText(text);
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

        private void ExportEntityHistory(Entity entity)
        {
            var histories = new HistoryList();
            if (_creationHistories.Keys.Contains(entity.geocode))
            {
                histories.Items.AddRange(_creationHistories[entity.geocode].Items);
            }
            foreach (var oldGeocode in entity.OldGeocodes)
            {
                if (_creationHistories.Keys.Contains(oldGeocode))
                {
                    histories.Items.AddRange(_creationHistories[oldGeocode].Items);
                }
            }
            if (histories.Items.Any())
            {
                histories.Items.Sort((x, y) => y.effective.CompareTo(x.effective));
                var historyXml = XmlManager.EntityToXml<HistoryList>(histories);
                // EntityToXml adds a header with BOM, exports with a different name than used in the entity XMLs
                historyXml = historyXml.Replace("HistoryList", "history");
                var startPos = historyXml.IndexOf("<history");
                historyXml = historyXml.Substring(startPos);
                // remove namespace from history tag
                startPos = historyXml.IndexOf(">") + 1;
                historyXml = "<history>" + historyXml.Substring(startPos);

                CopyToClipboard(historyXml);
            }
        }

        private void CheckHistoryAvailable(ListView listview, ToolStripMenuItem menuItem)
        {
            var history = false;
            if (listview.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in listview.SelectedItems)
                {
                    if (item.Tag is Entity entity)
                    {
                        history = _creationHistories.Keys.Contains(entity.geocode);
                        foreach (var oldGeocode in entity.OldGeocodes)
                        {
                            history |= _creationHistories.Keys.Contains(oldGeocode);
                        }
                    }
                }
                menuItem.Enabled = history;
            }
        }

        /// <summary>
        /// Gets the DLA web id of the local government unit corresponding to the selected item in the listview.
        /// </summary>
        /// <param name="listView">Listview to check.</param>
        /// <returns>Web id, or 0 if no id was found.</returns>
        private Int32 GetWebIdOfSelectedItem(ListView listView)
        {
            var webId = 0;
            if (listView.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in listView.SelectedItems)
                {
                    if (item.Tag is Entity entity)
                    {
                        var office = entity.office.FirstOrDefault(x => x.webidSpecified);
                        if (office != null)
                        {
                            webId = office.webid;
                        }
                    }
                }
            }
            return webId;
        }

        #endregion private methods

        #region event handler

        private void EntityBrowserForm_Load(object sender, EventArgs e)
        {
            _baseEntity = GlobalData.CompleteGeocodeList();
            _baseEntity.CalcOldGeocodesRecursive();
            _baseEntity.PropagatePostcodeRecursive();
            _baseEntity.PropagateObsoleteToSubEntities();
            _allEntities = _baseEntity.FlatList().Where(x => !x.IsObsolete).ToList();
            var allLocalGovernmentParents = _allEntities.Where(x => x.type == EntityType.Tambon || x.type == EntityType.Changwat).ToList();
            _localGovernments.AddRange(_allEntities.Where(x => x.type.IsLocalGovernment()));

            foreach (var tambon in allLocalGovernmentParents)
            {
                var localGovernmentEntity = tambon.CreateLocalGovernmentDummyEntity();
                if (localGovernmentEntity != null)
                {
                    _localGovernments.Add(localGovernmentEntity);
                    _allEntities.Add(localGovernmentEntity);
                }
            }
            using (var fileStream = new FileStream(GlobalData.BaseXMLDirectory + "\\DOLA web id.xml", FileMode.Open, FileAccess.Read))
            {
                var dolaWebIds = XmlManager.XmlToEntity<WebIdList>(fileStream, new XmlSerializer(typeof(WebIdList)));
                var errors = string.Empty;
                foreach (var entry in dolaWebIds.item)
                {
                    var entity = _localGovernments.FirstOrDefault(x => x.geocode == entry.geocode || x.OldGeocodes.Contains(entry.geocode));
                    if (entity != null)
                    {
                        var office = entity.office.FirstOrDefault(x => x.type.IsLocalGovernmentOffice());
                        if (!office.webidSpecified)
                        {
                            office.webid = entry.id;
                            office.webidSpecified = true;
                        }
                        else
                        {
                            errors += String.Format("Duplicate webId {0}", entry.id) + Environment.NewLine;
                        }
                    }
                    else if (!entry.obsolete)  
                    {
                        errors += String.Format("WebId {0} refers to invalid LAO {1}", entry.id, entry.geocode) + Environment.NewLine;
                    }
                }
                if (!String.IsNullOrEmpty(errors))
                {
                    MessageBox.Show(errors, "WebId errors");
                }
            }
            foreach (var file in Directory.EnumerateFiles(GlobalData.BaseXMLDirectory + "\\DOLA\\"))
            {
                using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    var dolaData = XmlManager.XmlToEntity<Entity>(fileStream, new XmlSerializer(typeof(Entity)));
                    foreach (var sourceEntity in dolaData.FlatList())
                    {
                        var targetEntity = _localGovernments.FirstOrDefault(x => x.geocode == sourceEntity.geocode);
                        if (targetEntity != null)
                        {
                            var sourceOffice = sourceEntity.office.FirstOrDefault(x => x.type.IsLocalGovernmentOffice());
                            var targetOffice = targetEntity.office.FirstOrDefault(x => x.type.IsLocalGovernmentOffice());
                            if (sourceOffice != null && targetOffice != null)
                            {
                                targetOffice.dola.AddRange(sourceOffice.dola);
                                targetOffice.dola.Sort((x, y) => y.year.CompareTo(x.year));
                            }

                            targetEntity.area.area.AddRange(sourceEntity.area.area);
                            // targetEntity.area.area.Sort((x, y) => String.Compare(y.date,x.date));
                            targetEntity.entitycount.AddRange(sourceEntity.entitycount);
                            targetEntity.entitycount.Sort((x, y) => y.year.CompareTo(x.year));
                        }
                    }
                }
            }

            var allTambon = _allEntities.Where(x => x.type == EntityType.Tambon).ToList();
            foreach (var lao in _localGovernments)
            {
                lao.CalculatePostcodeForLocalAdministration(allTambon);
            }

            GlobalData.LoadPopulationData(PopulationDataSource, PopulationReferenceYear);
            Entity.CalculateLocalGovernmentPopulation(_localGovernments, allTambon, PopulationDataSource, PopulationReferenceYear);

            var allEntities = new List<Entity>();
            allEntities.AddRange(_localGovernments);
            allEntities.AddRange(_baseEntity.FlatList());
            _creationHistories = ExtractHistoriesFromGazette(_baseEntity, allEntities.Distinct());

            PopulationDataToTreeView();
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
            CalcLocalGovernmentConstituencies(entity);

            mnuMubanDefinitions.Enabled = AreaDefinitionAnnouncements(entity).Any();

            mnuWikipediaGerman.Enabled = entity.type.IsCompatibleEntityType(EntityType.Amphoe);
            mnuWikipediaEnglish.Enabled = mnuWikipediaGerman.Enabled;
            mnuHistory.Enabled = _creationHistories.Keys.Contains(entity.geocode) || entity.OldGeocodes.Any(x => _creationHistories.Keys.Contains(x));

            mnuWikipediaTambonEnglish.Enabled = entity.type.IsCompatibleEntityType(EntityType.Tambon);
        }

        private void mnuMubanDefinitions_Click(Object sender, EventArgs e)
        {
            var selectedNode = treeviewSelection.SelectedNode;
            var entity = (Entity)(selectedNode.Tag);
            foreach (var entry in AreaDefinitionAnnouncements(entity))
            {
                ShowPdf(entry);
            }
        }


        private void mnuWikipediaTambonEnglish_Click(Object sender, EventArgs e)
        {
            var selectedNode = treeviewSelection.SelectedNode;
            var entity = (Entity)(selectedNode.Tag);
            if (entity.type.IsCompatibleEntityType(EntityType.Tambon))
            {
                var exporter = new WikipediaExporter(_baseEntity, _localGovernments)
                {
                    CheckWikiData = CheckWikiData,
                    PopulationReferenceYear = PopulationReferenceYear,
                    PopulationDataSource = PopulationDataSource
                };
                var text = exporter.TambonArticle(entity, Language.English);

                CopyToClipboard(text);
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

        private void treeviewSelection_MouseUp(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Select the clicked node
                treeviewSelection.SelectedNode = treeviewSelection.GetNodeAt(e.X, e.Y);

                if (treeviewSelection.SelectedNode != null)
                {
                    popupTree.Show(treeviewSelection, e.Location);
                }
            }
        }

        private void popupListviewLocal_Opening(Object sender, CancelEventArgs e)
        {
            CheckHistoryAvailable(listviewLocalAdministration, mnuHistoryLocal);
            var hasWebId = false;
            var hasWikidata = false;
            var hasWebsite = false;
            var hasLocation = false;
            if (listviewLocalAdministration.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in listviewLocalAdministration.SelectedItems)
                {
                    if (item.Tag is Entity entity)
                    {
                        hasWebId = entity.office.Any(x => x.webidSpecified);
                        hasWikidata = !String.IsNullOrEmpty(entity?.wiki?.wikidata);
                        hasWebsite = entity.office.First().url.Any();
                        hasLocation = entity.office.First().Point != null;
                    }
                }
            }
            mnuGeneralInfoPage.Enabled = hasWebId;
            mnuAdminInfoPage.Enabled = hasWebId;
            mnuWikidataLocal.Enabled = hasWikidata;
            mnuWebsite.Enabled = hasWebsite;
            mnuLocation.Enabled = hasLocation;
        }

        private void popupListviewCentral_Opening(Object sender, CancelEventArgs e)
        {
            CheckHistoryAvailable(listviewCentralAdministration, mnuHistoryCentral);
            var hasWikidata = false;
            if (listviewCentralAdministration.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in listviewCentralAdministration.SelectedItems)
                {
                    if (item.Tag is Entity entity)
                    {
                        hasWikidata = !String.IsNullOrEmpty(entity?.wiki?.wikidata);
                    }
                }
            }
            mnuWikidataCentral.Enabled = hasWikidata;
        }

        private void mnuAdminInfoPage_Click(Object sender, EventArgs e)
        {
            var webId = GetWebIdOfSelectedItem(listviewLocalAdministration);
            if (webId > 0)
            {
                var url = String.Format(CultureInfo.CurrentUICulture, "http://www.dla.go.th/info/info_councilor.jsp?orgId={0}", webId);
                Process.Start(url);
            }
        }

        private void mnuGeneralInfoPage_Click(Object sender, EventArgs e)
        {
            var webId = GetWebIdOfSelectedItem(listviewLocalAdministration);
            if (webId > 0)
            {
                var url = String.Format(CultureInfo.CurrentUICulture, "http://infov1.dla.go.th/public/surveyInfo.do?cmd=surveyForm&orgInfoId={0}", webId);
                Process.Start(url);
            }
        }

        private void mnuGoogleSearchLocal_Click(Object sender, EventArgs e)
        {
            if (listviewLocalAdministration.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in listviewLocalAdministration.SelectedItems)
                {
                    var entity = item.Tag as Entity;

                    var url = String.Format(CultureInfo.CurrentUICulture, "https://www.google.de/search?source=hp&q={0}", entity.FullName);
                    Process.Start(url);
                }
            }
        }

        private void mnuWikidataLocal_Click(Object sender, EventArgs e)
        {
            if (listviewLocalAdministration.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in listviewLocalAdministration.SelectedItems)
                {
                    var entity = item.Tag as Entity;
                    if (!String.IsNullOrEmpty(entity?.wiki?.wikidata))
                    {
                        var url = String.Format(CultureInfo.CurrentUICulture, "https://www.wikidata.org/wiki/{0}", entity.wiki.wikidata);
                        Process.Start(url);
                    }
                }
            }
        }

        private void mnuHistory_Click(Object sender, EventArgs e)
        {
            var selectedNode = treeviewSelection.SelectedNode;
            var entity = (Entity)(selectedNode.Tag);
            ExportEntityHistory(entity);
        }

        private void mnuHistoryLocal_Click(Object sender, EventArgs e)
        {
            if (listviewLocalAdministration.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in listviewLocalAdministration.SelectedItems)
                {
                    var entity = item.Tag as Entity;
                    ExportEntityHistory(entity);
                }
            }
        }

        private void mnuHistoryCentral_Click(Object sender, EventArgs e)
        {
            if (listviewCentralAdministration.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in listviewCentralAdministration.SelectedItems)
                {
                    var entity = item.Tag as Entity;
                    ExportEntityHistory(entity);
                }
            }
        }

        private void mnuConstituency_Click(object sender, EventArgs e)
        {
            var selectedNode = treeviewSelection.SelectedNode;
            var entity = (Entity)(selectedNode.Tag);

            Int16 numberOfConstituencies = 350;

            var result = ConstituencyCalculator.Calculate(entity.geocode, PopulationReferenceYear, numberOfConstituencies);

            String displayResult = String.Empty;
            foreach (KeyValuePair<Entity, Int32> entry in result)
            {
                Int32 votersPerSeat = 0;
                if (entry.Value != 0)
                {
                    votersPerSeat = entry.Key.GetPopulationDataPoint(PopulationDataSourceType.DOPA, PopulationReferenceYear).total / entry.Value;
                }
                displayResult = displayResult +
                    String.Format("{0} {1} ({2} per seat)", entry.Key.english, entry.Value, votersPerSeat) + Environment.NewLine;
            }

            new StringDisplayForm(String.Format("Constituencies {0}", PopulationReferenceYear), displayResult).Show();
        }

        private void mnuWebsite_Click(object sender, EventArgs e)
        {
            if (listviewLocalAdministration.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in listviewLocalAdministration.SelectedItems)
                {
                    var entity = item.Tag as Entity;
                    var urls = entity.office.First().url;
                    if (urls.Any())
                    {
                        Process.Start(urls.OrderByDescending(x => x.lastchecked).First().Value);
                    }
                }
            }
        }

        private void mnuLocation_Click(object sender, EventArgs e)
        {
            if (listviewLocalAdministration.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in listviewLocalAdministration.SelectedItems)
                {
                    var entity = item.Tag as Entity;
                    var location = entity.office.FirstOrDefault()?.Point;
                    if (location != null)
                    {
                        var url = String.Format(CultureInfo.CurrentUICulture, "https://maps.google.com/maps?ll={0},{1}&q={0},{1}&hl=en&t=m&z=15", location.lat, location.@long);
                        Process.Start(url);
                    }
                }
            }
        }

        private void mnuWikidataCentral_Click(Object sender, EventArgs e)
        {
            if (listviewCentralAdministration.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in listviewCentralAdministration.SelectedItems)
                {
                    var entity = item.Tag as Entity;
                    if (!String.IsNullOrEmpty(entity?.wiki?.wikidata))
                    {
                        var url = String.Format(CultureInfo.CurrentUICulture, "https://www.wikidata.org/wiki/{0}", entity.wiki.wikidata);
                        Process.Start(url);
                    }
                }
            }

        }
        #endregion

        private void listviewLocalAdministration_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listviewLocalAdministration.SelectedItems.Count == 1)
            {
                var entity = listviewLocalAdministration.SelectedItems[0]?.Tag as Entity;
                var office = entity?.office.FirstOrDefault();
                txtLocalGovernment.Text = String.Empty;
                if (office != null && !office.obsolete)
                {
                    var term = office.council.CouncilTerms.FirstOrDefault();
                    if (term != null && term.beginreason != TermBeginType.TermExtended)
                    {
                        term.end = term.begin.AddYears(4).AddDays(-1);
                        term.endreason = TermEndType.EndOfTerm;
                        var newTerm = new CouncilTerm()
                        {
                            begin = term.begin.AddYears(4),
                            beginreason = TermBeginType.TermExtended,
                            size = term.size,
                            type = term.type,
                        };
                        String txt;
                        if (entity.type == EntityType.TAO)
                        {
                            txt =
                                String.Format("<term begin=\"{0:yyyy-MM-dd}\" type=\"{1}\" size=\"{2}\" beginreason=\"TermExtended\" />", newTerm.begin, newTerm.type, newTerm.size) + Environment.NewLine +
                                String.Format("<term begin=\"{0:yyyy-MM-dd}\" end=\"{1:yyyy-MM-dd}\" type=\"{2}\" size=\"{3}\" />", term.begin, term.end, term.type, term.size) + Environment.NewLine;
                        }
                        else
                        {
                            txt =
                                String.Format("<term begin=\"2021-03-28\" type=\"{0}\" size=\"{1}\" />",newTerm.type, newTerm.size) + Environment.NewLine +
                                String.Format("<term begin=\"{0:yyyy-MM-dd}\" end=\"2021-03-27\" type=\"{1}\" size=\"{2}\" beginreason=\"TermExtended\" />", newTerm.begin, newTerm.type, newTerm.size) + Environment.NewLine +
                                String.Format("<term begin=\"{0:yyyy-MM-dd}\" end=\"{1:yyyy-MM-dd}\" type=\"{2}\" size=\"{3}\" />", term.begin, term.end, term.type, term.size) + Environment.NewLine;
                        }
                        txtLocalGovernment.Text += txt;
                    }
                    var official = office.officials.OfficialTerms.FirstOrDefault() as OfficialEntry;
                    if (official != null && official.beginreason != OfficialBeginType.TermExtended)
                    {
                        official.end = official.begin.AddYears(4).AddDays(-1);
                        official.endreason = OfficialEndType.EndOfTerm;
                        var newOfficial = new OfficialEntry()
                        {
                            begin = official.begin.AddYears(4),
                            beginreason = OfficialBeginType.TermExtended,
                            name = official.name,
                            title = official.title,
                        };
                        String txt;
                        if (entity.type == EntityType.TAO)
                        {
                            txt =
                                String.Format("<official title=\"{0}\" name=\"{1}\" begin=\"{2:yyyy-MM-dd}\" beginreason=\"TermExtended\" />", newOfficial.title, newOfficial.name, newOfficial.begin) + Environment.NewLine +
                                String.Format("<official title=\"{0}\" name=\"{1}\" begin=\"{2:yyyy-MM-dd}\" end=\"{3:yyyy-MM-dd}\" beginreason=\"ElectedDirectly\" endreason=\"EndOfTerm\" />", official.title, official.name, official.begin, official.end) + Environment.NewLine;
                        }
                        else 
                        {
                            txt =
                                "<officialterm title=\"Mayor\" begin=\"2013-03-28\" beginreason=\"ElectedDirectly\" />" + Environment.NewLine +
                                String.Format("<official title=\"{0}\" name=\"{1}\" begin=\"{2:yyyy-MM-dd}\" end=\"2021-03-27\" beginreason=\"TermExtended\" endreason=\"EndOfTerm\" />", newOfficial.title, newOfficial.name, newOfficial.begin) + Environment.NewLine +
                                String.Format("<official title=\"{0}\" name=\"{1}\" begin=\"{2:yyyy-MM-dd}\" end=\"{3:yyyy-MM-dd}\" beginreason=\"ElectedDirectly\" endreason=\"EndOfTerm\" />", official.title, official.name, official.begin, official.end) + Environment.NewLine;
                        }

                        txtLocalGovernment.Text += txt;


                    }
                    if (!String.IsNullOrEmpty(txtLocalGovernment.Text))
                    {
                        CopyToClipboard(txtLocalGovernment.Text);
                    }
                }
            }
        }
    }
}