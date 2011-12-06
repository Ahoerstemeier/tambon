using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.ComponentModel;
using De.AHoerstemeier.Geo;
using De.AHoerstemeier.Tambon;

namespace De.AHoerstemeier.GeoTool.Model
{

    // To change to ObservableObject with MVVM light 4

    public class GeoDataModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public const String LocationPropertyName = "Location";
        private GeoPoint _location = null;
        public GeoPoint Location
        {
            get
            {
                return _location;
            }
            set
            {
                if ( value != null )
                {
                    SetGeoLocation(value.Latitude, value.Longitude);
                }
            }
        }

        public const String DatumPropertyName = "Datum";
        private GeoDatum _currentGeoDatum;
        public GeoDatum Datum
        {
            get { return _currentGeoDatum; }
            set
            {
                if ( _currentGeoDatum == value )
                {
                    return;
                }
                var oldValue = _currentGeoDatum;
                _currentGeoDatum = value;

                if ( _UtmPoint != null )
                {
                    var newGeoLocation = new GeoPoint(_UtmPoint, _currentGeoDatum);
                    SetLocationValue(newGeoLocation, _UtmPoint);
                }
                // Update bindings, no broadcast
                RaisePropertyChanged(DatumPropertyName);
            }
        }

        private UtmPoint _UtmPoint = null;
        public UtmPoint UtmLocation
        {
            get { return GetUtmPoint(); }
        }

        private void RaisePropertyChanged(String propertyName)
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public GeoDataModel()
        {
            _location = GeoDataGlobals.Instance.DefaultLocation;
            _currentGeoDatum = GeoDatum.DatumWGS84();
        }


        internal void SetGeoHash(String value)
        {
            GeoPoint geoPoint = null;
            try
            {
                geoPoint = new GeoPoint();
                geoPoint.GeoHash = value;

                SetLocationValue(geoPoint, null);
            }
            catch ( ArgumentException )
            {
            }
        }
        internal Boolean GeoHashValid(String value)
        {
            Boolean valid = false;
            try
            {
                var geoPoint = new GeoPoint();
                geoPoint.GeoHash = value;
                valid = true;
            }
            catch ( ArgumentException )
            {
            }
            return valid;
        }
        internal void SetGeoLocation(String value)
        {
            GeoPoint geoPoint = null;
            try
            {
                geoPoint = new GeoPoint(value);
                SetLocationValue(geoPoint, null);
            }
            catch ( ArgumentException )
            {
            }
        }
        internal void SetUtmLocation(String value)
        {
            String myValue = TambonHelper.ReplaceThaiNumerals(value.ToUpper()).Trim();
            GeoPoint geoPoint = null;
            UtmPoint utmPoint = null;
            try
            {
                utmPoint = UtmPoint.ParseUtmString(myValue);
                geoPoint = new GeoPoint(utmPoint, _currentGeoDatum);
                geoPoint.Datum = GeoDatum.DatumWGS84();
                SetLocationValue(geoPoint, utmPoint);
            }
            catch ( ArgumentException )
            {
            }
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
        internal void SetMgrsLocation(String value)
        {
            String myValue = TambonHelper.ReplaceThaiNumerals(value.ToUpper()).Trim();
            GeoPoint geoPoint = null;
            UtmPoint utmPoint = null;
            try
            {
                if ( !TambonHelper.IsNumeric(value.Substring(0, 2)) )
                {
                    value = ZoneForThailandMgrs(value) + value;
                }
                utmPoint = UtmPoint.ParseMgrsString(value);
                geoPoint = new GeoPoint(utmPoint, _currentGeoDatum);
                geoPoint.Datum = GeoDatum.DatumWGS84();
                SetLocationValue(geoPoint, utmPoint);
            }
            catch ( ArgumentException )
            {
            }
        }

        private UtmPoint GetUtmPoint()
        {
            if ( _UtmPoint == null )
            {
                GeoPoint geoPointWithOtherDatum = new GeoPoint(Location);
                geoPointWithOtherDatum.Datum = Datum;
                _UtmPoint = geoPointWithOtherDatum.CalcUTM();
            }
            return _UtmPoint;
        }

        private void SetLocationValue(GeoPoint geoPoint, UtmPoint utmPoint)
        {
            _location = geoPoint;
            _UtmPoint = utmPoint;

            RaisePropertyChanged(LocationPropertyName);
        }

        internal void SetGeoLocation(Double latitude, Double longitude)
        {
            GeoPoint geoPoint = null;
            try
            {
                geoPoint = new GeoPoint(latitude, longitude);
                SetLocationValue(geoPoint, null);
            }
            catch ( ArgumentException )
            {
            }
        }
    }
}
