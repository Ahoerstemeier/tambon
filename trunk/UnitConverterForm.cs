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
    public partial class UnitConverterForm : Form
    {
        const Decimal SquareKilometerToRai = 0.0016M;
        Boolean mChanging = false;
        #region properties
        private Decimal mArea = 0.0M;
        public Decimal Area { get { return mArea; } set { mArea = value; UpdateEdits(); } }
        #endregion
        public UnitConverterForm()
        {
            InitializeComponent();
        }
        protected void UpdateEdits()
        {
            try
            {
                mChanging = true;
                edtSquareKilometer.Value = mArea;
                Decimal lAreaRai = mArea / SquareKilometerToRai;
                edtRaiDecimal.Value = lAreaRai;
                edtRaiInteger.Value = Math.Truncate(lAreaRai);
                Decimal lAreaNgan = (lAreaRai - Math.Truncate(lAreaRai)) * 4;
                edtNganInteger.Value = Math.Truncate(lAreaNgan);
                Decimal lAreaWa = (lAreaNgan - Math.Truncate(lAreaNgan)) * 100;
                edtTarangWaInteger.Value = Math.Truncate(lAreaWa);
            }
            finally
            { 
                mChanging = false; 
            }
        }

        private void edtSquareKilometer_ValueChanged(object sender, EventArgs e)
        {
            if (!mChanging)
            {
                Area = edtSquareKilometer.Value;
            }
        }
        private void edtRaiDecimal_ValueChanged(object sender, EventArgs e)
        {
            if (!mChanging)
            {
                Area = edtRaiDecimal.Value * SquareKilometerToRai;
            }
        }

        private void edtInteger_ValueChanged(object sender, EventArgs e)
        {
            if (!mChanging)
            {
                Decimal lArea = ((edtTarangWaInteger.Value / 100 + edtNganInteger.Value) / 4 + edtRaiInteger.Value) * SquareKilometerToRai;
                Area = lArea;
            }
        }
    }
}
