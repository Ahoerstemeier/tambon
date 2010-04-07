using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using De.AHoerstemeier.Geo;

namespace De.AHoerstemeier.Tambon
{
    public partial class GeoCoordinateForm : Form
    {
        private Boolean mChanging = false;

        public GeoCoordinateForm()
        {
            InitializeComponent();
        }

        private String ZoneForThailandMGRS(String iValue)
        {
            Int32 lZone = 0;
            char lEastingChar = iValue[0];
            if ((lEastingChar >= 'A') && (lEastingChar <= 'H'))
            {
                lZone = 49;
            }
            else if ((lEastingChar >= 'J') && (lEastingChar <= 'R'))
            {
                lZone = 47;
            }
            else if ((lEastingChar >= 'S') && (lEastingChar <= 'Z'))
            {
                lZone = 48;
            }
            char lNorthingChar = iValue[1];
            String lNorthingCharacters = UTMPoint.MGRSNorthingChars(lZone);
            Int32 lNorthingCount = lNorthingCharacters.IndexOf(lNorthingChar);

            char lZoneChar;
            if (lNorthingCount >= 17)
            {
                lZoneChar = 'Q';
            }
            else
            {
                lZoneChar = 'P';
            }
            String lResult = lZone.ToString() + lZoneChar;
            return lResult;
        }

        private void edit_MGRS_TextChanged(object sender, EventArgs e)
        {
            if (!mChanging)
            {
                String lValue = TambonHelper.ReplaceThaiNumerals(edit_MGRS.Text.ToUpper()).Trim();
                try
                {
                    mChanging = true;
                    if (!TambonHelper.IsNumeric(lValue.Substring(0,2)))
                    {
                      lValue = ZoneForThailandMGRS(lValue) + lValue;
                    }
                    UTMPoint lUTM = UTMPoint.ParseMGRSString(lValue);
                    edit_UTM.Text = lUTM.ToString();
                    GeoPoint lGeo = new GeoPoint(lUTM, (GeoDatum)cbx_datum.SelectedItem);
                    lGeo.Datum = GeoDatum.DatumWGS84();
                    edit_LatLong.Text = lGeo.ToString();
                }
                catch
                {
                    // invalid string
                    edit_UTM.Text = String.Empty;
                    edit_LatLong.Text = String.Empty;
                }
                mChanging = false;
            }
        }
        private void GeoCoordinateForm_Load(object sender, EventArgs e)
        {
            cbx_datum.Items.Add(GeoDatum.DatumWGS84());
            cbx_datum.SelectedIndex = 0;
            cbx_datum.Items.Add(GeoDatum.DatumIndian1975());
            cbx_datum.Items.Add(GeoDatum.DatumIndian1954());
        }

        private void edit_UTM_TextChanged(object sender, EventArgs e)
        {
            if (!mChanging)
            {
                String lValue = TambonHelper.ReplaceThaiNumerals(edit_UTM.Text.ToUpper()).Trim();
                try
                {
                    mChanging = true;
                    UTMPoint lUTM = UTMPoint.ParseUTMString(lValue);
                    edit_MGRS.Text = lUTM.ToMGRSString(6);
                    GeoPoint lGeo = new GeoPoint(lUTM, (GeoDatum)cbx_datum.SelectedItem);
                    lGeo.Datum = GeoDatum.DatumWGS84();
                    edit_LatLong.Text = lGeo.ToString();
                }
                catch
                {
                    // invalid string
                    edit_MGRS.Text = String.Empty;
                    edit_LatLong.Text = String.Empty;
                }
                mChanging = false;
            }
        }
    }
}
