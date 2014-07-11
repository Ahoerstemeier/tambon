using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace De.AHoerstemeier.Tambon.UI
{
    public partial class EntityBrowserForm : Form
    {
        private List<Entity> localGovernments = new List<Entity>();
        private Entity baseEntity;

        public EntityBrowserForm()
        {
            InitializeComponent();
            PopulationDataSource = PopulationDataSourceType.DOPA;
            PopulationReferenceYear = 2013;
        }

        private void EntityBrowserForm_Load(object sender, EventArgs e)
        {
            baseEntity = GlobalData.CompleteGeocodeList();
            baseEntity.PropagatePostcodeRecursive();
            var allEntities = baseEntity.FlatList().Where(x => !x.IsObsolete).ToList();
            var allTambon = allEntities.Where(x => x.type == EntityType.Tambon).ToList();
            foreach ( var tambon in allTambon )
            {
                var localGovernmentEntity = tambon.CreateLocalGovernmentDummyEntity();
                if ( localGovernmentEntity != null )
                {
                    localGovernments.Add(localGovernmentEntity);
                }
            }
            foreach ( var item in allEntities.Where(x => x.type.IsLocalGovernment()) )
            {
                localGovernments.Add(item);
            }

            PopulationDataToTreeView();

            GlobalData.LoadPopulationData(PopulationDataSource, PopulationReferenceYear);
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
            foreach ( var entity in baseEntity.entity.Where(x => !x.IsObsolete) )
            {
                TreeNode baseNode = EntityToTreeNode(entity);
                if ( baseNode != null )
                {
                    treeviewSelection.Nodes.Add(baseNode);
                    if ( entity.geocode == StartChangwatGeocode )
                    {
                        treeviewSelection.SelectedNode = baseNode;
                    }
                }
            }
            treeviewSelection.EndUpdate();
        }

        private void treeviewSelection_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var selectedNode = treeviewSelection.SelectedNode;
            var entity = (Entity)(selectedNode.Tag);
            EntityToCentralAdministrativeListView(entity);
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
    }
}