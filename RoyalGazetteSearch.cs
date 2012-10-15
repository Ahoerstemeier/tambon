using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace De.AHoerstemeier.Tambon
{
    public partial class RoyalGazetteSearch : Form
    {
        private class SearchData
        {
            internal String SearchString { get; set; }
            internal Int32 StartYear { get; set; }
            internal Int32 EndYear { get; set; }
            internal List<EntityType> EntityTypes { get; set; }
            internal List<EntityModification> EntityModifications { get; set; }

            internal SearchData()
            {
                StartYear = 0;
                EndYear = 0;
                EntityTypes = new List<EntityType>();
                EntityModifications = new List<EntityModification>();
            }
        }

        public RoyalGazetteSearch()
        {
            InitializeComponent();
        }
        internal event RoyalGazetteProcessingFinishedHandler SearchFinished;
        private void button1_Click(object sender, EventArgs e)
        {
            SearchData data = new SearchData();

            if ( !cbx_AllYears.Checked )
            {
                data.StartYear = Convert.ToInt32(edtYearStart.Value);
                data.EndYear = Convert.ToInt32(edtYearEnd.Value);
            }
            data.SearchString = cbxSearchKey.Text;

            if ( chkChangwat.Checked )
            {
                data.EntityTypes.Add(EntityType.Changwat);
            }
            if ( chkAmphoe.Checked )
            {
                data.EntityTypes.Add(EntityType.Amphoe);
                data.EntityTypes.Add(EntityType.KingAmphoe);
                data.EntityTypes.Add(EntityType.Khet);
            }
            if ( chkTambon.Checked )
            {
                data.EntityTypes.Add(EntityType.Tambon);
                data.EntityTypes.Add(EntityType.Khwaeng);
            }
            if ( chkThesaban.Checked )
            {
                data.EntityTypes.Add(EntityType.Thesaban);
                data.EntityTypes.Add(EntityType.ThesabanTambon);
                data.EntityTypes.Add(EntityType.ThesabanMueang);
                data.EntityTypes.Add(EntityType.ThesabanNakhon);
            }
            if ( chkSukhaphiban.Checked )
            {
                data.EntityTypes.Add(EntityType.Sukhaphiban);
            }
            if ( chkMuban.Checked )
            {
                data.EntityTypes.Add(EntityType.Muban);
            }
            if ( chkTAO.Checked )
            {
                data.EntityTypes.Add(EntityType.TAO);
            }
            if ( chkPAO.Checked )
            {
                data.EntityTypes.Add(EntityType.PAO);
            }
            if ( chkTambonCouncil.Checked )
            {
                data.EntityTypes.Add(EntityType.SaphaTambon);
            }

            if ( chkCreation.Checked )
            {
                data.EntityModifications.Add(EntityModification.Creation);
            }
            if ( chkAbolishment.Checked )
            {
                data.EntityModifications.Add(EntityModification.Abolishment);
            }
            if ( chkArea.Checked )
            {
                data.EntityModifications.Add(EntityModification.AreaChange);
            }
            if ( chkRename.Checked )
            {
                data.EntityModifications.Add(EntityModification.Rename);
            }
            if ( chkStatus.Checked )
            {
                data.EntityModifications.Add(EntityModification.StatusChange);
            }
            if ( chkConstituency.Checked )
            {
                data.EntityModifications.Add(EntityModification.Constituency);
            }

            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = false;
            b.WorkerSupportsCancellation = false;
            b.DoWork += BackgroundWorker_DoWork;
            b.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            b.RunWorkerAsync(data);
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            RoyalGazetteList list = new RoyalGazetteList();
            SearchData data = e.Argument as SearchData;
            if ( data != null )
            {
                var searcher = new RoyalGazetteOnlineSearch();
                DateTime dateStart;
                DateTime dateEnd;
                dateStart = new DateTime(Math.Max(1800, data.StartYear), 1, 1);
                dateEnd = new DateTime(Math.Max(1800, data.EndYear), 1, 1);

                if ( !String.IsNullOrEmpty(data.SearchString) )
                {
                    list.AddRange(searcher.SearchString(dateStart, dateEnd, data.SearchString));
                }

                if ( data.EntityTypes.Any() && data.EntityModifications.Any() )
                {
                    list.AddRange(searcher.SearchNewsRangeAdministrative(dateStart, dateEnd, data.EntityTypes, data.EntityModifications));
                }
            }
            e.Result = list;
        }

        private void BackgroundWorker_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            RoyalGazetteList list = e.Result as RoyalGazetteList;
            if ( list != null )
            {
                SearchFinished(this,new RoyalGazetteEventArgs(list));
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
