using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EARTHLib;
using De.AHoerstemeier.Geo;
using System.Threading;
using System.Xml;
using System.IO;

namespace De.AHoerstemeier.Tambon
{
    public partial class GeoCoordinateForm : Form
    {
        private Boolean _Changing = false;
        private GeoPoint _Point = null;

        public GeoCoordinateForm()
        {
            InitializeComponent();
        }

        private String ZoneForThailandMgrs(String value)
        {
            Int32 zone = 0;
            Char eastingChar = value[0];
            if ( (eastingChar >= 'A') && (eastingChar <= 'H') )
            {
                zone = 49;
            }
            else if ( (eastingChar >= 'J') && (eastingChar <= 'R') )
            {
                zone = 47;
            }
            else if ( (eastingChar >= 'S') && (eastingChar <= 'Z') )
            {
                zone = 48;
            }
            Char northingChar = value[1];
            String northingCharacters = UtmPoint.MgrsNorthingChars(zone);
            Int32 northingCount = northingCharacters.IndexOf(northingChar);

            Char zoneChar;
            if ( (northingCount > 17) | (northingCount == 0) )
            {
                zoneChar = 'Q';
            }
            else if ( northingCount > 8 )
            {
                zoneChar = 'P';
            }
            else
            {
                zoneChar = 'N';
            }
            String result = zone.ToString() + zoneChar;
            return result;
        }

        private void GeoCoordinateForm_Load(object sender, EventArgs e)
        {
            cbx_datum.Items.Add(GeoDatum.DatumWGS84());
            cbx_datum.SelectedIndex = 0;
            cbx_datum.Items.Add(GeoDatum.DatumIndian1975());
            cbx_datum.Items.Add(GeoDatum.DatumIndian1954());
        }

        private void SetValues(GeoPoint geoPoint, UtmPoint utmPoint, object sender)
        {
            if (sender != edt_LatLong)
            {
                if (geoPoint == null)
                {
                    edt_LatLong.Text = String.Empty;
                }
                else
                {
                    edt_LatLong.Text = geoPoint.ToString();
                }
            }
            if (sender != edt_geohash)
            {
                if (geoPoint == null)
                {
                    edt_geohash.Text = String.Empty;
                }
                else
                {
                    edt_geohash.Text = geoPoint.GeoHash;
                }
            }
            if (sender != edt_UTM)
            {
                if (utmPoint == null)
                {
                    edt_UTM.Text = String.Empty;
                }
                else
                {
                    edt_UTM.Text = utmPoint.ToString();
                }
            }
            if (sender != edt_MGRS)
            {
                if (utmPoint == null)
                {
                    edt_MGRS.Text = String.Empty;
                }
                else
                {
                    edt_MGRS.Text = utmPoint.ToMgrsString(6);
                }
            }
            _Point = geoPoint;
            btnFlyTo.Enabled = (_Point != null);

            if (geoPoint != null)
            {
                label1.Text = RtsdMapIndex.IndexL7018(geoPoint);
            }
        }

        private void edit_MGRS_TextChanged(object sender, EventArgs e)
        {
            if ( !_Changing )
            {
                String value = TambonHelper.ReplaceThaiNumerals(edt_MGRS.Text.ToUpper()).Trim();
                GeoPoint geoPoint = null;
                UtmPoint utmPoint = null;
                try
                {
                    _Changing = true;
                    if ( !TambonHelper.IsNumeric(value.Substring(0, 2)) )
                    {
                        value = ZoneForThailandMgrs(value) + value;
                    }
                    utmPoint = UtmPoint.ParseMgrsString(value);
                    geoPoint = new GeoPoint(utmPoint, (GeoDatum)cbx_datum.SelectedItem);
                    geoPoint.Datum = GeoDatum.DatumWGS84();
                }
                catch
                {
                    // invalid string
                    utmPoint = null;
                    geoPoint = null;
                }
                SetValues(geoPoint, utmPoint, sender);
                _Changing = false;
            }
        }

        private void edit_UTM_TextChanged(object sender, EventArgs e)
        {
            if ( !_Changing )
            {
                String value = TambonHelper.ReplaceThaiNumerals(edt_UTM.Text.ToUpper()).Trim();
                GeoPoint geoPoint = null;
                UtmPoint utmPoint = null;
                try
                {
                    _Changing = true;
                    utmPoint = UtmPoint.ParseUtmString(value);
                    geoPoint = new GeoPoint(utmPoint, (GeoDatum)cbx_datum.SelectedItem);
                    geoPoint.Datum = GeoDatum.DatumWGS84();
                }
                catch
                {
                    // invalid string
                    utmPoint = null;
                    geoPoint = null;
                }
                SetValues(geoPoint, utmPoint, sender);
                _Changing = false;
            }
        }

        private void edt_geohash_TextChanged(object sender, EventArgs e)
        {
            if ( !_Changing )
            {
                GeoPoint geoPoint = null;
                UtmPoint utmPoint = null;
                try
                {
                    _Changing = true;
                    geoPoint = new GeoPoint();
                    geoPoint.GeoHash = edt_geohash.Text;
                    GeoPoint lGeoOtherDatum = new GeoPoint(geoPoint);
                    lGeoOtherDatum.Datum = (GeoDatum)cbx_datum.SelectedItem;
                    utmPoint = lGeoOtherDatum.CalcUTM();
                }
                catch
                {
                    // invalid string
                    utmPoint = null;
                    geoPoint = null;
                }
                SetValues(geoPoint, utmPoint, sender);
                _Changing = false;
            }

        }

        private void edt_LatLong_TextChanged(object sender, EventArgs e)
        {
            if ( !_Changing )
            {
                GeoPoint geoPoint = null;
                UtmPoint utmPoint = null;
                try
                {
                    _Changing = true;
                    geoPoint = new GeoPoint(edt_LatLong.Text);
                    geoPoint.Datum = (GeoDatum)cbx_datum.SelectedItem;
                    GeoPoint lGeoOtherDatum = new GeoPoint(geoPoint);
                    lGeoOtherDatum.Datum = (GeoDatum)cbx_datum.SelectedItem;
                    utmPoint = lGeoOtherDatum.CalcUTM();
                }
                catch
                {
                    // invalid string
                    geoPoint = null;
                    utmPoint = null;
                }
                SetValues(geoPoint, utmPoint, sender);
                _Changing = false;
            }
        }

        private void btnFlyTo_Click(object sender, EventArgs e)
        {
            try
            {
                var googleEarth = new ApplicationGEClass();

                String tempKmlFile = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".kml";
                KmlHelper kmlWriter = new KmlHelper();
                kmlWriter.AddPoint(_Point.Latitude, _Point.Longitude, "Temporary location", "", "", "");
                kmlWriter.SaveToFile(tempKmlFile);
                while ( googleEarth.IsInitialized() == 0 )
                {
                    Thread.Sleep(500);
                }
                googleEarth.OpenKmlFile(tempKmlFile, 0);
            }
            catch ( Exception ex )
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
