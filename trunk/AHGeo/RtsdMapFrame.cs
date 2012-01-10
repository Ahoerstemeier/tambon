using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Geo
{
    public class RtsdMapFrame : GeoFrameBase
    {
        #region fields
        private GeoPoint _northWestCorner;
        #endregion

        #region properties
        public Double LatitudeExtendDegree { get; private set; }
        public Double LongitudeExtendDegree { get; private set; }
        public Tuple<Int32, Int32> Index { get; private set; }
        #endregion

        #region constructor
        public RtsdMapFrame(GeoPoint northWestCorner, Double latitudeExtend, Double longitudeExtend, Tuple<Int32, Int32> index)
        {
            _northWestCorner = northWestCorner;
            LatitudeExtendDegree = latitudeExtend;
            LongitudeExtendDegree = longitudeExtend;
            Index = index;
            String subIndexName = String.Empty;
            switch (index.Item2)
            {
                case 1:
                    subIndexName = "I";
                    break;
                case 2:
                    subIndexName = "II";
                    break;
                case 3:
                    subIndexName = "III";
                    break;
                case 4:
                    subIndexName = "IV";
                    break;
            }
            Name = String.Format("{0:####} {1}", index.Item1, subIndexName);
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

        public override string ToString()
        {
            return Name;
        }

    }
}
