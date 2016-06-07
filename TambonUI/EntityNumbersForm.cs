using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace De.AHoerstemeier.Tambon.UI
{
    public partial class EntityNumbersForm : Form
    {
        private const Int32 _startYear = 1950;
        private Int32 _endYear;
        private List<Entity> _localGovernments = new List<Entity>();
        private Entity _baseEntity;
        private List<Entity> _allEntities;
        private Dictionary<EntityType, IEnumerable<Tuple<Int32, Int32>>> _numberyByYear = new Dictionary<EntityType, IEnumerable<Tuple<int, int>>>();

        public EntityNumbersForm()
        {
            InitializeComponent();
        }

        private void EntityNumbers_Load(object sender, EventArgs e)
        {
            _baseEntity = GlobalData.CompleteGeocodeList();
            _baseEntity.CalcOldGeocodesRecursive();
            _baseEntity.PropagateObsoleteToSubEntities();
            _allEntities = _baseEntity.FlatList()./*Where(x => !x.IsObsolete).*/ToList();
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

            var entityTypes = new List<EntityType>()
            {
                EntityType.Changwat,
                EntityType.Amphoe,
                EntityType.KingAmphoe,
                EntityType.Khet,
                EntityType.Tambon,
                EntityType.Khwaeng,
                // EntityType.Muban,
                EntityType.Sukhaphiban,
                EntityType.ThesabanNakhon,
                EntityType.ThesabanMueang,
                EntityType.ThesabanTambon,
                // EntityType.TAO,
            };
            _endYear = DateTime.Now.Year;

            foreach ( var entityType in entityTypes )
            {
                _numberyByYear[entityType] = CalcEntityTypeByYear(entityType, _allEntities);

                var series = new Series();
                series.LegendText = entityType.Translate(Language.English);
                series.Tag = entityType;
                foreach ( var dataPoint in _numberyByYear[entityType] )
                {
                    series.Points.Add(new DataPoint(dataPoint.Item1, dataPoint.Item2));
                }
                series.ChartType = SeriesChartType.Line;
                chart.Series.Add(series);
            }

            edtYear.Maximum = _endYear;
            edtYear.Minimum = _startYear;
            edtYear.Value = _endYear;
        }

        private IEnumerable<Tuple<Int32, Int32>> CalcEntityTypeByYear(EntityType entityType, List<Entity> _allEntities)
        {
            var result = new List<Tuple<Int32, Int32>>();
            var entities = _allEntities.Where(x => x.type.IsCompatibleEntityType(entityType)).Where(x => !x.IsObsolete || x.history.Items.Any(y => y is HistoryAbolish)).ToList();
            for ( Int32 year = _startYear ; year <= _endYear ; year++ )
            {
                result.Add(new Tuple<Int32, Int32>(year, entities.Count(x => x.history.CheckTypeAtDate(entityType, x.type, new DateTime(year, 1, 1)))));
                //var items = entities.Where(x => x.history.CheckTypeAtDate(entityType, x.type, new DateTime(year, 1, 1))).ToList();
                //var codes = items.Select(x => x.geocode).ToList();
            }
            return result;
        }

        private void edtYear_ValueChanged(Object sender, EventArgs e)
        {
            txtData.Text = String.Empty;
            foreach ( var entityType in _numberyByYear.Keys )
            {
                var number = _numberyByYear[entityType].FirstOrDefault(x => x.Item1 == edtYear.Value);
                if ( number != null )
                {
                    txtData.Text += String.Format("{0}: {1}", entityType.Translate(Language.English), number.Item2) + Environment.NewLine;
                }
            }
        }
    }
}