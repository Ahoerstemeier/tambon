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
            if (lNorthingCount > 17)
            {
                lZoneChar = 'Q';
            }
            else if (lNorthingCount>8)
            {
                lZoneChar = 'P';
            }
            else
            {
                lZoneChar = 'N';
            }
            String lResult = lZone.ToString() + lZoneChar;
            return lResult;
        }

        private void GeoCoordinateForm_Load(object sender, EventArgs e)
        {
            cbx_datum.Items.Add(GeoDatum.DatumWGS84());
            cbx_datum.SelectedIndex = 0;
            cbx_datum.Items.Add(GeoDatum.DatumIndian1975());
            cbx_datum.Items.Add(GeoDatum.DatumIndian1954());
        }

        private void SetValues(GeoPoint iGeoPoint, UTMPoint iUTM, object sender)
        {
            if (sender != edt_LatLong)
            {
                if (iGeoPoint == null)
                {
                    edt_LatLong.Text = String.Empty;
                }
                else
                {
                    edt_LatLong.Text = iGeoPoint.ToString();
                }
            }
            if (sender != edt_geohash)
            {
                if (iGeoPoint == null)
                {
                    edt_geohash.Text = String.Empty;
                }
                else
                {
                    edt_geohash.Text = iGeoPoint.GeoHash;
                }
            }
            if (sender != edt_UTM)
            {
                if (iUTM == null)
                {
                    edt_UTM.Text = String.Empty;
                }
                else
                {
                    edt_UTM.Text = iUTM.ToString();
                }
            }
            if (sender != edt_MGRS)
            {
                if (iUTM == null)
                {
                    edt_MGRS.Text = String.Empty;
                }
                else
                {
                    edt_MGRS.Text = iUTM.ToMGRSString(6);
                }
            }
        }

        private void edit_MGRS_TextChanged(object sender, EventArgs e)
        {
            if (!mChanging)
            {
                String lValue = TambonHelper.ReplaceThaiNumerals(edt_MGRS.Text.ToUpper()).Trim();
                GeoPoint lGeo = null;
                UTMPoint lUTM = null;
                try
                {
                    mChanging = true;
                    if (!TambonHelper.IsNumeric(lValue.Substring(0,2)))
                    {
                      lValue = ZoneForThailandMGRS(lValue) + lValue;
                    }
                    lUTM = UTMPoint.ParseMGRSString(lValue);
                    lGeo = new GeoPoint(lUTM, (GeoDatum)cbx_datum.SelectedItem);
                    lGeo.Datum = GeoDatum.DatumWGS84();
                }
                catch
                {
                    // invalid string
                    lUTM = null;
                    lGeo = null;
                }
                SetValues(lGeo, lUTM, sender);
                mChanging = false;
            }
        }

        private void edit_UTM_TextChanged(object sender, EventArgs e)
        {
            if (!mChanging)
            {
                String lValue = TambonHelper.ReplaceThaiNumerals(edt_UTM.Text.ToUpper()).Trim();
                GeoPoint lGeo = null;
                UTMPoint lUTM = null;
                try
                {
                    mChanging = true;
                    lUTM = UTMPoint.ParseUTMString(lValue);
                    lGeo = new GeoPoint(lUTM, (GeoDatum)cbx_datum.SelectedItem);
                    lGeo.Datum = GeoDatum.DatumWGS84();
                }
                catch
                {
                    // invalid string
                    lUTM = null;
                    lGeo = null;
                }
                SetValues(lGeo, lUTM, sender);
                mChanging = false;
            }
        }

        private void edt_geohash_TextChanged(object sender, EventArgs e)
        {
            if (!mChanging)
            {
                GeoPoint lGeo = null;
                UTMPoint lUTM = null;
                try
                {
                    mChanging = true;
                    lGeo = new GeoPoint();
                    lGeo.GeoHash = edt_geohash.Text;
                    GeoPoint lGeoOtherDatum = new GeoPoint(lGeo);
                    lGeoOtherDatum.Datum = (GeoDatum)cbx_datum.SelectedItem;
                    lUTM = lGeoOtherDatum.CalcUTM();
                }
                catch
                {
                    // invalid string
                    lUTM = null;
                    lGeo = null;
                }
                SetValues(lGeo, lUTM, sender);
                mChanging = false;
            }

        }
    }
}
