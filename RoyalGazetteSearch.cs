using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace De.AHoerstemeier.Tambon
{
    public partial class RoyalGazetteSearch : Form
    {
        public RoyalGazetteSearch()
        {
            InitializeComponent();
        }
        internal event RoyalGazetteList.ProcessingFinished OnSearchFinished;
        private void button1_Click(object sender, EventArgs e)
        {
            var lList = new RoyalGazetteList();
            var lSearcher = new RoyalGazetteOnlineSearch();
            DateTime lDateStart ;
            DateTime lDateEnd ;
            if (cbx_AllYears.Checked)
            {
                lDateEnd = new DateTime(1800, 1, 1);
                lDateStart = new DateTime(1800, 1, 1);
            }
            else
            {
                lDateStart = new DateTime(Convert.ToInt32(edtYearStart.Value), 1, 1);
                lDateEnd = new DateTime(Convert.ToInt32(edtYearEnd.Value), 1, 1);
            }
            String lSearchString = cbxSearchKey.Text;

            if (!String.IsNullOrEmpty(lSearchString))
            {
                lList.AddRange(lSearcher.SearchString(lDateStart,lDateEnd,lSearchString));
            }

            var lEntityTypes = new List<EntityType>();
            var lEntityModifications = new List<EntityModification>();

            if (chkChangwat.Checked)
            {
                lEntityTypes.Add(EntityType.Changwat);
            }
            if (chkAmphoe.Checked)
            {
                lEntityTypes.Add(EntityType.Amphoe);
                lEntityTypes.Add(EntityType.KingAmphoe);
                lEntityTypes.Add(EntityType.Khet);
            }
            if (chkTambon.Checked)
            {
                lEntityTypes.Add(EntityType.Tambon);
                lEntityTypes.Add(EntityType.Khwaeng);
            }
            if (chkThesaban.Checked)
            {
                lEntityTypes.Add(EntityType.Thesaban);
            }
            if (chkSukhaphiban.Checked)
            {
                lEntityTypes.Add(EntityType.Sukhaphiban);
            }
            if (chkMuban.Checked)
            {
                lEntityTypes.Add(EntityType.Muban);
            }
            if (chkTAO.Checked)
            {
                lEntityTypes.Add(EntityType.TAO);
            }
            if (chkPAO.Checked)
            {
                lEntityTypes.Add(EntityType.PAO);
            }
            if (chkTambonCouncil.Checked)
            {
                lEntityTypes.Add(EntityType.SaphaTambon);
            }
            if (chkCreation.Checked)
            {
                lEntityModifications.Add(EntityModification.Creation);
            }
            if (chkAbolishment.Checked)
            {
                lEntityModifications.Add(EntityModification.Abolishment);
            }
            if (chkArea.Checked)
            {
                lEntityModifications.Add(EntityModification.AreaChange);
            }
            if (chkRename.Checked)
            {
                lEntityModifications.Add(EntityModification.Rename);
            }
            if (chkStatus.Checked)
            {
                lEntityModifications.Add(EntityModification.StatusChange);
            }
            if (chkConstituency.Checked)
            {
                lEntityModifications.Add(EntityModification.Constituency);
            }
            lList.AddRange(lSearcher.SearchNewsRangeAdministrative(lDateStart, lDateEnd, lEntityTypes, lEntityModifications));

            if (OnSearchFinished != null)
            {
                OnSearchFinished(lList);
            }

        }

        private void cbx_AllYears_CheckedChanged(object sender, EventArgs e)
        {
            edtYearEnd.Enabled = !cbx_AllYears.Checked;
            edtYearStart.Enabled = !cbx_AllYears.Checked;
        }

        private void RoyalGazetteSearch_Load(object sender, EventArgs e)
        {
            edtYearEnd.Maximum = DateTime.Now.Year;
            edtYearEnd.Value = DateTime.Now.Year;
            edtYearStart.Maximum = DateTime.Now.Year;
        }

    }
}
