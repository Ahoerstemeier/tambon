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
    public partial class MubanHelperForm : Form
    {
        public Romanization Romanizator
        {
            get;
            set;
        }

        public MubanHelperForm()
        {
            InitializeComponent();
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            var entities = new List<Entity>();
            UInt32 count = 0;
            UInt32 baseGeocode = Convert.ToUInt32(edtGeocode.Value);
            foreach ( var line in edtText.Lines )
            {
                var name = line.Replace('\t', ' ').Trim();
                if ( !String.IsNullOrEmpty(name) )
                {
                    count++;
                    var entityType = EntityType.Muban;
                    if ( chkStripBefore.Checked )
                    {
                        var startPosition = name.IndexOf(ThaiLanguageHelper.Ban);
                        if ( startPosition >= 0 )
                        {
                            name = name.Substring(startPosition);
                        }
                        if ( chkStripAfter.Checked )
                        {
                            name = name.Split(' ').First();
                        }
                    }
                    var entity = new Entity();
                    entity.name = name;
                    entity.type = entityType;
                    entity.geocode = baseGeocode * 100 + count;
                    entities.Add(entity);
                }
            }
            if ( Romanizator != null )
            {
                List<RomanizationEntry> dummy;
                var romanizations = Romanizator.FindRomanizationSuggestions(out dummy, entities);
                foreach ( var entry in romanizations )
                {
                    var entity = entities.First(x => x.geocode == entry.Geocode);
                    entity.english = entry.English;
                }
            }

            StringBuilder mubanListBuilder = new StringBuilder();
            foreach ( var entity in entities )
            {
                if ( !String.IsNullOrEmpty(entity.english) )
                {
                    mubanListBuilder.AppendLine(String.Format("<entity type=\"{0}\" geocode=\"{1}\" name=\"{2}\" english=\"{3}\" />",
                        entity.type, entity.geocode, entity.name, entity.english));
                }
                else
                {
                    mubanListBuilder.AppendLine(String.Format("<entity type=\"{0}\" geocode=\"{1}\" name=\"{2}\" />",
                        entity.type, entity.geocode, entity.name));
                }
            }
            var form = new StringDisplayForm("Muban", mubanListBuilder.ToString());
            form.Show();
        }

        private void btnAddBan_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            foreach ( var line in edtText.Lines )
            {
                var value = line.Trim();
                if ( !String.IsNullOrEmpty(value) )
                {
                    if ( !value.StartsWith(ThaiLanguageHelper.Ban) )
                    {
                        value = ThaiLanguageHelper.Ban + value;
                    }
                    builder.AppendLine(value);
                }
            }
            edtText.Text = builder.ToString();
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
        }

        private void MubanHelperForm_Load(object sender, EventArgs e)
        {
            cbxChangwat.Items.AddRange(GlobalData.CompleteGeocodeList().FlatList().Where(x => x.type.IsCompatibleEntityType(EntityType.Changwat)).ToArray());
        }

        private void cbxAmphoe_SelectedValueChanged(object sender, EventArgs e)
        {
            var amphoe = cbxAmphoe.SelectedItem as Entity;
            cbxTambon.Items.Clear();
            if ( amphoe != null )
            {
                cbxTambon.Items.AddRange(amphoe.entity.Where(x => !x.IsObsolete && x.type.IsCompatibleEntityType(EntityType.Tambon)).ToArray());
            }
            cbxTambon.SelectedItem = null;
        }

        private void cbxTambon_SelectedValueChanged(object sender, EventArgs e)
        {
            if ( cbxTambon.SelectedItem != null )
            {
                edtGeocode.Value = ((Entity)cbxTambon.SelectedItem).geocode;
            }
        }
    }
}