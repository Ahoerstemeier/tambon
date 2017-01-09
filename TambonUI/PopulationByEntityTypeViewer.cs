using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace De.AHoerstemeier.Tambon
{
    public partial class PopulationByEntityTypeViewer : Form
    {
        // ToDo: Sort by Column
        // Export as CSV

        #region fields

        /// <summary>
        /// Country base entity.
        /// </summary>
        private Entity _country;

        /// <summary>
        /// Current base entity.
        /// </summary>
        private Entity _baseEntity;

        /// <summary>
        /// All local government entities.
        /// </summary>
        private List<Entity> _localGovernments;

        /// <summary>
        /// All active entities.
        /// </summary>
        private List<Entity> _allEntities;

        #endregion fields

        #region properties

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

        #endregion properties

        #region constructor

        /// <summary>
        /// Creates a new instance of <see cref="PopulationByEntityTypeViewer"/>.
        /// </summary>
        public PopulationByEntityTypeViewer()
        {
            InitializeComponent();
            UpdateEntityTypeCheckboxes();
        }

        #endregion constructor

        #region UI event handler

        /// <summary>
        /// Handles the change of the selected entity types to include.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void chkEntity_CheckStateChanged(Object sender, EventArgs e)
        {
            UpdateList();
        }

        /// <summary>
        /// Handles the change of the selected entity type group.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void rbxEntity_CheckedChanged(Object sender, EventArgs e)
        {
            UpdateEntityTypeCheckboxes();
            UpdateList();
        }

        /// <summary>
        /// Initializes the control.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PopulationByEntityTypeViewer_Load(Object sender, EventArgs e)
        {
            InitializeData();
            FillProvincesCombobox();
            _baseEntity = _country;

            edtCompareYear.Maximum = GlobalData.PopulationStatisticMaxYear;
            edtCompareYear.Minimum = GlobalData.PopulationStatisticMinYear;
            edtCompareYear.Value = edtCompareYear.Maximum;
            edtCompareYear.Enabled = chkCompare.Checked;

            UpdateList();
        }

        /// <summary>
        /// Handles whether a comparison with another year shall be done.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void chkCompare_CheckedChanged(Object sender, EventArgs e)
        {
            edtCompareYear.Enabled = chkCompare.Checked;
            UpdateList();
        }

        /// <summary>
        /// Handles the change of the year with which the data shall be compared.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void edtCompareYear_ValueChanged(Object sender, EventArgs e)
        {
            Int16 newYear = Convert.ToInt16(edtCompareYear.Value);
            if ( chkCompare.Checked )
            {
                if ( !_country.population.Any(x => x.source == PopulationDataSource && x.Year == newYear) )
                {
                    GlobalData.LoadPopulationData(PopulationDataSource, newYear);
                    // GlobalData.CompleteGeocodeList creates a clone, thus need to use the new instances to get the new population data

                    _country = GlobalData.CompleteGeocodeList();
                    _country.PropagateObsoleteToSubEntities();
                    _allEntities = _country.FlatList().Where(x => !x.IsObsolete).Where(x => x.type != EntityType.Muban && x.type != EntityType.Chumchon).ToList();

                    // re-calculate the local government populations
                    var allTambon = _allEntities.Where(x => x.type == EntityType.Tambon).ToList();
                    Entity.CalculateLocalGovernmentPopulation(_localGovernments, allTambon, PopulationDataSource, Convert.ToInt16(edtCompareYear.Value));

                    UpdateBaseEntity();  // need to get _baseEntity
                }
                UpdateList();
            }
        }

        /// <summary>
        /// Handles the change of the selected province.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void cbxChangwat_SelectedValueChanged(Object sender, EventArgs e)
        {
            UpdateBaseEntity();
        }

        #endregion UI event handler

        /// <summary>
        /// Initializes the data for the view.
        /// </summary>
        private void InitializeData()
        {
            _country = GlobalData.CompleteGeocodeList();
            _country.PropagateObsoleteToSubEntities();
            _allEntities = _country.FlatList().Where(x => !x.IsObsolete).Where(x => x.type != EntityType.Muban && x.type != EntityType.Chumchon).ToList();
            _localGovernments = new List<Entity>();
            var allLocalGovernmentParents = _allEntities.Where(x => x.type == EntityType.Tambon || x.type == EntityType.Changwat).ToList();
            _localGovernments.AddRange(_allEntities.Where(x => x.type.IsLocalGovernment()));

            foreach ( var tambon in allLocalGovernmentParents )
            {
                var localGovernmentEntity = tambon.CreateLocalGovernmentDummyEntity();
                if ( localGovernmentEntity != null && !localGovernmentEntity.IsObsolete )
                {
                    _localGovernments.Add(localGovernmentEntity);
                    _allEntities.Add(localGovernmentEntity);
                }
            }

            var allTambon = _allEntities.Where(x => x.type == EntityType.Tambon).ToList();
            GlobalData.LoadPopulationData(PopulationDataSource, PopulationReferenceYear);
            Entity.CalculateLocalGovernmentPopulation(_localGovernments, allTambon, PopulationDataSource, PopulationReferenceYear);
        }

        /// <summary>
        /// Updates the <see cref="Control.Enabled"/> for the radiobuttons of the subdivision types.
        /// </summary>
        private void UpdateEntityTypeCheckboxes()
        {
            chkAmphoe.Enabled = rbxAmphoeKhet.Checked;
            chkKhet.Enabled = rbxAmphoeKhet.Checked;
            chkTambon.Enabled = rbxTambonKhwaeng.Checked;
            chkKhwaeng.Enabled = rbxTambonKhwaeng.Checked;
            chkThesabanNakhon.Enabled = rbxThesaban.Checked;
            chkThesabanMueang.Enabled = rbxThesaban.Checked;
            chkThesabanTambon.Enabled = rbxThesaban.Checked;
            chkTambonAdministrativeOrganization.Enabled = rbxThesaban.Checked;
        }

        /// <summary>
        /// Fills <see cref="cbxChangwat"/> with the province entities.
        /// </summary>
        private void FillProvincesCombobox()
        {
            cbxChangwat.Items.Clear();
            cbxChangwat.Items.Add(_country);
            foreach ( var changwat in _country.entity )
            {
                cbxChangwat.Items.Add(changwat);
            }
            cbxChangwat.SelectedItem = _country;
        }

        /// <summary>
        /// Refreshes <see cref="_baseEntity"/> from the selected item in <see cref="cbxChangwat"/>.
        /// </summary>
        private void UpdateBaseEntity()
        {
            var newBaseEntity = cbxChangwat.SelectedItem as Entity;
            if ( newBaseEntity.type == EntityType.Country )
            {
                _baseEntity = _country;
            }
            else
            {
                _baseEntity = _country.entity.FirstOrDefault(x => x.geocode == newBaseEntity.geocode);
            }
        }

        /// <summary>
        /// Updates the calculated data.
        /// </summary>
        private void UpdateList()
        {
            IEnumerable<Entity> list = CalculateList();
            IEnumerable<Tuple<UInt32, Int32, Double>> populationChanges = null;
            if ( chkCompare.Checked )
            {
                populationChanges = CalcPopulationChanges(list, Convert.ToInt16(edtCompareYear.Value));
            }
            FillListView(list, populationChanges);

            FrequencyCounter counter = new FrequencyCounter();
            foreach ( var entity in list )
            {
                var populationData = entity.GetPopulationDataPoint(PopulationDataSource, PopulationReferenceYear);
                counter.IncrementForCount(populationData.total, entity.geocode);
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Total population: {0:##,###,##0}", counter.SumValue);
            builder.AppendLine();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Number of entities: {0}", counter.NumberOfValues);
            builder.AppendLine();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Mean population: {0:##,###,##0.0}", counter.MeanValue);
            builder.AppendLine();
            if ( list.Any() )
            {
                var maxEntity = list.Last();
                builder.AppendFormat(CultureInfo.CurrentUICulture, "Maximum population: {0:##,###,##0} ({1} - {2})", counter.MaxValue, maxEntity.geocode, maxEntity.english);
                builder.AppendLine();
                var minEntity = list.First();
                builder.AppendFormat(CultureInfo.CurrentUICulture, "Minimum population: {0:##,###,##0} ({1} - {2})", counter.MinValue, minEntity.geocode, minEntity.english);
                builder.AppendLine();
            }
            if ( (populationChanges != null) && (populationChanges.Any(x => x.Item2 != 0)) )
            {
                builder.AppendLine();
                var ordered = populationChanges.OrderBy(x => x.Item2);
                var winner = ordered.Last();
                var winnerEntry = list.First(x => x.geocode == winner.Item1);
                var looser = ordered.First();
                var looserEntry = list.First(x => x.geocode == looser.Item1);
                builder.AppendFormat(CultureInfo.CurrentUICulture, "Biggest winner: {0:##,###,##0} by {1} ({2})", winner.Item2, winnerEntry.english, winner.Item1);
                builder.AppendLine();
                builder.AppendFormat(CultureInfo.CurrentUICulture, "Biggest looser: {0:##,###,##0} by {1} ({2})", looser.Item2, looserEntry.english, looser.Item1);
                builder.AppendLine();
            }
            if ( (populationChanges != null) && (populationChanges.Any(x => x.Item2 != 0)) )
            {
                builder.AppendLine();
                var ordered = populationChanges.OrderBy(x => x.Item3);
                var winner = ordered.Last();
                var winnerEntry = list.First(x => x.geocode == winner.Item1);
                var looser = ordered.First();
                var looserEntry = list.First(x => x.geocode == looser.Item1);
                builder.AppendFormat(CultureInfo.CurrentUICulture, "Biggest winner: {0:##0.00}% by {1} ({2})", winner.Item3, winnerEntry.english, winner.Item1);
                builder.AppendLine();
                builder.AppendFormat(CultureInfo.CurrentUICulture, "Biggest looser: {0:##0.00}% by {1} ({2})", looser.Item3, looserEntry.english, looser.Item1);
                builder.AppendLine();
            }

            txtStatistics.Text = builder.ToString();
        }

        /// <summary>
        /// Calculates the population changes.
        /// </summary>
        /// <param name="entityList">List of entities.</param>
        /// <param name="year">Year with which the data shall be compared.</param>
        /// <returns>Enumeration of population changes. For the <see cref="Tuple{T1, T2, T3}"/>, the values are the <see cref="Entity.geocode"/>, the difference in <see cref="PopulationDataPoint.total"/>total population</see> and the percentual change of total population.</returns>
        private IEnumerable<Tuple<UInt32, Int32, Double>> CalcPopulationChanges(IEnumerable<Entity> entityList, Int16 year)
        {
            List<Tuple<UInt32, Int32, Double>> result = new List<Tuple<UInt32, Int32, Double>>();
            if ( entityList.Any() )
            {
                foreach ( var entity in entityList )
                {
                    if ( entity.geocode != 0 )
                    {
                        var currentEntry = entity.GetPopulationDataPoint(PopulationDataSource, PopulationReferenceYear);
                        var compareEntry = entity.GetPopulationDataPoint(PopulationDataSource, year);
                        if ( (compareEntry != null) && (compareEntry.total > 0) )
                        {
                            Int32 populationChange = currentEntry.total - compareEntry.total;
                            Double changePercent = 100.0 * populationChange / compareEntry.total;
                            result.Add(Tuple.Create(entity.geocode, populationChange, changePercent));
                        }
                    }
                }
            }
            return result;
        }

        private IEnumerable<Entity> CalculateList()
        {
            List<Entity> list = new List<Entity>();
            List<EntityType> entityTypes = new List<EntityType>();
            var subItems = _baseEntity.FlatList().Where(x => !x.IsObsolete);
            if ( rbx_Changwat.Checked )
            {
                entityTypes.Add(EntityType.Changwat);
                entityTypes.Add(EntityType.Bangkok);
                list.AddRange(subItems.Where(x => entityTypes.Contains(x.type)));
                if ( entityTypes.Contains(_baseEntity.type) )
                {
                    list.Add(_baseEntity);
                }
            }
            else if ( rbxAmphoeKhet.Checked )
            {
                if ( chkAmphoe.Checked )
                {
                    entityTypes.Add(EntityType.Amphoe);
                    entityTypes.Add(EntityType.KingAmphoe);
                }
                if ( chkKhet.Checked )
                {
                    entityTypes.Add(EntityType.Khet);
                }
                list.AddRange(subItems.Where(x => entityTypes.Contains(x.type)));
            }
            else if ( rbxTambonKhwaeng.Checked )
            {
                if ( chkTambon.Checked )
                {
                    entityTypes.Add(EntityType.Tambon);
                }
                if ( chkKhwaeng.Checked )
                {
                    entityTypes.Add(EntityType.Khwaeng);
                }
                list.AddRange(subItems.Where(x => entityTypes.Contains(x.type)));
            }
            else if ( rbxThesaban.Checked )
            {
                if ( chkThesabanTambon.Checked )
                {
                    entityTypes.Add(EntityType.ThesabanTambon);
                }
                if ( chkThesabanMueang.Checked )
                {
                    entityTypes.Add(EntityType.ThesabanMueang);
                }
                if ( chkThesabanNakhon.Checked )
                {
                    entityTypes.Add(EntityType.ThesabanNakhon);
                }
                if ( chkTambonAdministrativeOrganization.Checked )
                {
                    entityTypes.Add(EntityType.TAO);
                }

                list.AddRange(_baseEntity.LocalGovernmentEntitiesOf(_localGovernments).Where(x => entityTypes.Contains(x.type)));
            }

            return list.OrderBy(x => x.GetPopulationDataPoint(PopulationDataSource, PopulationReferenceYear).total);
        }

        private void FillListView(IEnumerable<Entity> entityList, IEnumerable<Tuple<UInt32, Int32, Double>> compare)
        {
            lvData.BeginUpdate();
            lvData.Items.Clear();
            if ( entityList.Any() )
            {
                foreach ( var entity in entityList )
                {
                    ListViewItem listViewItem = lvData.Items.Add(entity.english);
                    listViewItem.SubItems.Add(entity.name);
                    listViewItem.SubItems.Add(entity.geocode.ToString(CultureInfo.CurrentUICulture));
                    PopulationDataPoint populationDataPoint = entity.GetPopulationDataPoint(PopulationDataSource, PopulationReferenceYear);
                    listViewItem.SubItems.Add(populationDataPoint.total.ToString(CultureInfo.CurrentUICulture));
                    if ( compare != null )
                    {
                        var compareEntry = compare.FirstOrDefault(x => x.Item1 == entity.geocode);
                        if ( compareEntry != null )
                        {
                            listViewItem.SubItems.Add(compareEntry.Item2.ToString(CultureInfo.CurrentUICulture));
                            listViewItem.SubItems.Add(compareEntry.Item3.ToString("##0.00", CultureInfo.CurrentUICulture));
                        }
                    }
                }
            }
            lvData.EndUpdate();
        }
    }
}