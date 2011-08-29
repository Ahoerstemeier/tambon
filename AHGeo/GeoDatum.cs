using System;
using System.Collections.Generic;
using System.Text;

namespace De.AHoerstemeier.Geo
{
    /// <summary>
    /// Encapsulates a geo datum containing an ellipsoid and coordinate offsets.
    /// </summary>
    public class GeoDatum : ICloneable, IEquatable<GeoDatum>
    {
        #region properties
        /// <summary>
        /// Gets the name of the datum.
        /// </summary>
        /// <value>The name of the datum.</value>
        public String Name { get; private set; }

        /// <summary>
        /// Gets the ellipsoid of the datum.
        /// </summary>
        /// <value>The ellipsoid of the the datum.</value>
        public GeoEllipsoid Ellipsoid
        {
            get { return _Ellipsoid; }
            private set { _Ellipsoid = value; }
        }
        private GeoEllipsoid _Ellipsoid = GeoEllipsoid.EllipsoidWGS84();

        /// <summary>
        /// Gets the offset in longitude from the reference ellipsoid.
        /// </summary>
        /// <value>The offset in longitude from the reference ellipsoid.</value>
        public double DeltaX { get; private set; }
        /// <summary>
        /// Gets the offset in latitude from the reference ellipsoid.
        /// </summary>
        /// <value>The offset in latitude from the reference ellipsoid.</value>
        public double DeltaY { get; private set; }
        /// <summary>
        /// Gets the offset in altitude from the reference ellipsoid.
        /// </summary>
        /// <value>The offset in altitude from the reference ellipsoid.</value>
        public double DeltaZ { get; private set; }
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the GeoDatum class.
        /// </summary>
        /// <param name="name">Name of the geodatum.</param>
        /// <param name="ellipsoid">Ellipsoid used.</param>
        /// <param name="deltaXValue">Longitude offset.</param>
        /// <param name="deltaYValue">Latitude offset.</param>
        /// <param name="deltaZValue">Altitude offset.</param>
        public GeoDatum(String name, GeoEllipsoid ellipsoid, Double deltaXValue, Double deltaYValue, Double deltaZValue)
        {
            Name = name;
            Ellipsoid = ellipsoid;
            DeltaX = deltaXValue;
            DeltaY = deltaYValue;
            DeltaZ = deltaZValue;
        }
        /// <summary>
        /// Initializes a new instance of the GeoDatum class.
        /// </summary>
        /// <param name="value">Geodatum to copy data from.</param>
        public GeoDatum(GeoDatum value)
        {
            Name = value.Name;
            DeltaX = value.DeltaX;
            DeltaY = value.DeltaY;
            DeltaZ = value.DeltaZ;
            Ellipsoid = (GeoEllipsoid)value.Ellipsoid.Clone();
        }
        #endregion

        #region methods
        /// <summary>
        /// Geodatum for the WGS84 (GPS) ellipsoid.
        /// </summary>
        /// <returns>Geodatum for the WGS84 (GPS) ellipsoid.</returns>
        public static GeoDatum DatumWGS84()
        {
            return new GeoDatum("WGS84", GeoEllipsoid.EllipsoidWGS84(), 0, 0, 0);
        }
        /// <summary>
        /// Geodatum for the Indian 1975 datum.
        /// </summary>
        /// <returns>Geodatum for the Indian 1975 datum.</returns>
        public static GeoDatum DatumIndian1975()
        {
            return new GeoDatum("Indian 1975 - Thailand", GeoEllipsoid.EllipsoidEverest(), 209, 818, 290);
        }
        /// <summary>
        /// Geodatum for the Indian 1954 datum.
        /// </summary>
        /// <returns>Geodatum for the Indian 1954 datum.</returns>
        public static GeoDatum DatumIndian1954()
        {
            return new GeoDatum("Indian 1954 - Thailand", GeoEllipsoid.EllipsoidEverest(), 218, 816, 297);
        }
        /// <summary>
        /// Geodatum for the North American datum of 1927 (NAD27).
        /// </summary>
        /// <returns>Geodatum for the North American datum of 1927 (NAD27).</returns>
        public static GeoDatum DatumNorthAmerican27MeanConus()
        {
            return new GeoDatum("North American Datum 1927 (NAD27, mean for conus)", GeoEllipsoid.EllipsoidClarke1866(), -8, 160, 176);
        }
        /// <summary>
        /// Converts the datum into a string representation.
        /// </summary>
        /// <returns>The name of the datum.</returns>
        public override string ToString()
        {
            return this.Name;
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new GeoDatum(this);
        }

        #endregion

        #region IEquatable Members
        public bool Equals(GeoDatum value)
        {
            bool result = value.Ellipsoid.Equals(this.Ellipsoid);
            result = result & (Math.Abs(value.DeltaX - this.DeltaX) < 0.1);
            result = result & (Math.Abs(value.DeltaY - this.DeltaY) < 0.1);
            result = result & (Math.Abs(value.DeltaZ - this.DeltaZ) < 0.1);
            return result;
        }
        #endregion
    }
}
