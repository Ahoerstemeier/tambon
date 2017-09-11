﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace De.AHoerstemeier.Tambon.UI
{
    public partial class DisambiguationForm : Form
    {
        private class EntityList
        {
            public List<Entity> Entities = new List<Entity>();

            public override string ToString()
            {
                return String.Format("{0} ({1})", Entities.First().english, Entities.Count());
            }

            public EntityList(IEnumerable<Entity> entities)
            {
                Entities.AddRange(entities);
            }
        }

        private List<Entity> allEntities = null;

        public DisambiguationForm()
        {
            InitializeComponent();
        }

        private void DisambiguationForm_Load(Object sender, EventArgs e)
        {
            cbxEntityType.Items.Add(EntityType.Tambon);
            cbxEntityType.Items.Add(EntityType.TAO);
            cbxEntityType.Items.Add(EntityType.ThesabanTambon);
            cbxEntityType.Items.Add(EntityType.ThesabanMueang);

            allEntities = new List<Entity>();
            var entities = GlobalData.CompleteGeocodeList();
            allEntities.AddRange(entities.FlatList().Where(x => !x.IsObsolete));
            cbxProvinces.Items.AddRange(allEntities.Where(x => x.type == EntityType.Changwat).ToArray());

            var allTambon = allEntities.Where(x => x.type == EntityType.Tambon).ToList();
            foreach ( var tambon in allTambon )
            {
                var localGovernmentEntity = tambon.CreateLocalGovernmentDummyEntity();
                if ( localGovernmentEntity != null && !localGovernmentEntity.IsObsolete )
                {
                    allEntities.Add(localGovernmentEntity);
                }
            }
        }

        private void cbxEntityType_SelectedValueChanged(Object sender, EventArgs e)
        {
            UpdateDisambiguationList();
        }

        private void UpdateDisambiguationList()
        {
            lbxNames.Items.Clear();
            UInt32 geocode = 0;
            if ( cbxProvinces.SelectedItem != null )
            {
                geocode = (cbxProvinces.SelectedItem as Entity).geocode;
            }
            if ( cbxEntityType.SelectedItem != null )
            {
                var selectedType = (EntityType)cbxEntityType.SelectedItem;
                var currentEntities = allEntities.Where(x => x.type == selectedType).ToList();
                var names = currentEntities.GroupBy(x => x.name).Where(y => y.Count() > 1).OrderByDescending(z => z.Count()).ThenBy(z => z.First().english);
                var currentNames = names.Select(x => new EntityList(x.OrderBy(y => y.geocode)));
                if ( geocode != 0 )
                {
                    currentNames = currentNames.Where(x => x.Entities.Any(y => GeocodeHelper.IsBaseGeocode(geocode, y.geocode)));
                }
                foreach ( var x in currentNames )
                {
                    lbxNames.Items.Add(x);
                }
            }
        }

        private void btnThaiWikipedia_Click(Object sender, EventArgs e)
        {
            var item = lbxNames.SelectedItem as EntityList;
            if ( item != null )
            {
                var provinces = allEntities.Where(x => x.type.IsCompatibleEntityType(EntityType.Changwat));
                var allAmphoe = allEntities.Where(x => x.type.IsCompatibleEntityType(EntityType.Amphoe));
                var provincesUsed = item.Entities.SelectMany(x => provinces.Where(y => GeocodeHelper.IsBaseGeocode(y.geocode, x.geocode))).ToList();

                var builder = new StringBuilder();
                builder.AppendFormat("'''{0}''' สามารถหมายถึง", item.Entities.First().FullName);
                builder.AppendLine();
                foreach ( var subItem in item.Entities )
                {
                    var province = provinces.FirstOrDefault(x => GeocodeHelper.IsBaseGeocode(x.geocode, subItem.geocode));
                    var amphoe = allAmphoe.FirstOrDefault(x => GeocodeHelper.IsBaseGeocode(x.geocode, subItem.geocode));
                    if ( amphoe == null && subItem.type.IsLocalGovernment() )
                    {
                        var firstTambonCode = subItem.LocalGovernmentAreaCoverage.First().geocode;
                        amphoe = allAmphoe.FirstOrDefault(x => GeocodeHelper.IsBaseGeocode(x.geocode, firstTambonCode));
                    }
                    var parentInfo = String.Format("{0} {1}", amphoe.FullName, province.FullName);
                    String disambiguatedName = String.Format("{0} ({1})", subItem.FullName, province.FullName);
                    if ( provincesUsed.Count(x => x == province) > 1 )
                    {
                        disambiguatedName = String.Format("{0} ({1})", subItem.FullName, amphoe.FullName);
                    }
                    builder.AppendFormat("* [[{0}|{1}]] {2}", disambiguatedName, subItem.FullName, parentInfo);
                    builder.AppendLine();
                }
                builder.AppendLine();
                builder.AppendLine("{{แก้กำกวม}}");

                Clipboard.Clear();
                Clipboard.SetText(builder.ToString());
            }
        }

        private void cbxProvinces_SelectedValueChanged(Object sender, EventArgs e)
        {
            UpdateDisambiguationList();
        }
    }
}