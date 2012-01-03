using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Geo
{
    public class RtdsMapFrame : GeoFrameBase
    {
        #region fields
        private GeoPoint _northWestCorner;
        #endregion

        #region properties
        public Double LatitudeExtendDegree { get; private set; }
        public Double LongitudeExtendDegree { get; private set; }
        #endregion

        #region constructor
        public RtdsMapFrame(GeoPoint northWestCorner, Double latitudeExtend, Double longitudeExtend, String name)
        {
            _northWestCorner = northWestCorner;
            LatitudeExtendDegree = latitudeExtend;
            LongitudeExtendDegree = longitudeExtend;
            Name = name;
        }
        #endregion

        #region private methods
        protected override GeoPoint GetNorthWestCorner()
        {
            return _northWestCorner;
        }
        protected override GeoPoint GetNorthEastCorner()
        {
            GeoPoint northEastCorner = new GeoPoint(NorthWestCorner);
            northEastCorner.Longitude += LongitudeExtendDegree;
            return northEastCorner;
        }
        protected override GeoPoint GetSouthWestCorner()
        {
            GeoPoint southWestCorner = new GeoPoint(NorthWestCorner);
            southWestCorner.Latitude -= LatitudeExtendDegree;
            return southWestCorner;
        }
        protected override GeoPoint GetSouthEastCorner()
        {
            GeoPoint southEastCorner = new GeoPoint(NorthWestCorner);
            southEastCorner.Latitude -= LatitudeExtendDegree;
            southEastCorner.Longitude += LongitudeExtendDegree;
            return southEastCorner;
        }
        #endregion

    }
}
