using System;
using System.Collections.Generic;
using System.Text;

namespace De.AHoerstemeier.Geo
{
    public class GeoEllipsoid : ICloneable, IEquatable<GeoEllipsoid>
    {
        #region properties
        public String Name { get; set; }
        public double SemiMajorAxis { get; set; }
        public double Flattening { get; set; }
        public double DenominatorOfFlattening { get { return 1 / Flattening; } }
        public double ExcentricitySquared { get { return 2 * Flattening - Flattening * Flattening; } }
        #endregion
        #region constructor
        public GeoEllipsoid(String iName, double iSemiMajorAxis, double iFlattening)
        {
            Name = iName;
            SemiMajorAxis = iSemiMajorAxis;
            Flattening = iFlattening;
        }
        public GeoEllipsoid(GeoEllipsoid iValue)
        {
            Name = iValue.Name;
            SemiMajorAxis = iValue.SemiMajorAxis;
            Flattening = iValue.Flattening;
        }
        #endregion
        #region ICloneable Members

        public object Clone()
        {
            return new GeoEllipsoid(this);
        }

        #endregion
        #region IEquatable Members
        public bool Equals(GeoEllipsoid iObj)
        {
            bool lResult = Math.Abs(iObj.SemiMajorAxis - this.SemiMajorAxis) < 0.0001;
            lResult = lResult & (Math.Abs(iObj.DenominatorOfFlattening - this.DenominatorOfFlattening) < 0.0000000001);
            return lResult;
        }
        #endregion

        // http://www.colorado.edu/geography/gcraft/notes/datum/edlist.html
        public static GeoEllipsoid EllipsoidWGS84()
        {
            return new GeoEllipsoid("WGS84", 6378137, 1 / 298.257223563);
        }
        public static GeoEllipsoid EllipsoidEverest()
        {
            return new GeoEllipsoid("Everest", 6377276.345, 1 / 300.8017);
        }
    }
}