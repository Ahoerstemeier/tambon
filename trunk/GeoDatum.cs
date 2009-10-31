using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    class GeoDatum : ICloneable
    {
        #region properties
        public String Name { get; set; }
        public GeoEllipsoid Ellipsoid { get; set; }
        public double deltaX { get; set; }
        public double deltaY { get; set; }
        public double deltaZ { get; set; }
        #endregion
        #region constructor
        public GeoDatum(String iName, GeoEllipsoid iEllipsoid, double ideltaX, double ideltaY, double ideltaZ)
        {
            Name = iName;
            Ellipsoid = iEllipsoid;
            deltaX = ideltaX;
            deltaY = ideltaY;
            deltaZ = ideltaZ;
        }
        public GeoDatum(GeoDatum iValue)
        {
            Name = iValue.Name;
            deltaX = iValue.deltaX;
            deltaY = iValue.deltaY;
            deltaZ = iValue.deltaZ;
            Ellipsoid = (GeoEllipsoid)iValue.Ellipsoid.Clone();
        }
        #endregion
        public static GeoDatum DatumWGS84()
        {
            return new GeoDatum("WGS84", GeoEllipsoid.EllipsoidWGS84(), 0, 0, 0);
        }
        public static GeoDatum DatumIndian1975()
        {
            return new GeoDatum("Indian 1975", GeoEllipsoid.EllipsoidEverest(), 210, 814, 289);
        }
        #region ICloneable Members

        public object Clone()
        {
            return new GeoDatum(this);
        }

        #endregion
    }
}
