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
                var name = line.Trim();
                if ( !String.IsNullOrEmpty(name) )
                {
                    count++;
                    var entityType = EntityType.Muban;
                    if ( chkStripBefore.Checked )
                    {
                        var startPosition = name.IndexOf("บ้าน");
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
    }
}