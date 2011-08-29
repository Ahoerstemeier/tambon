using System;
using System.Collections.Generic;
using System.Text;

namespace De.AHoerstemeier.Geo
{
    /// <summary>
    /// Encapsulates an ellipsoid.
    /// </summary>
    public class GeoEllipsoid : ICloneable, IEquatable<GeoEllipsoid>
    {
        #region properties
        /// <summary>
        /// Gets the Name of the ellipsoid.
        /// </summary>
        /// <value>Name of the ellipsoid.</value>
        public String Name { get; private set; }
        /// <summary>
        /// Gets the size of the semi major axis of the ellipsoid.
        /// </summary>
        /// <value>The size of the semi major axis of the ellipsoid</value>
        public Double SemiMajorAxis { get; private set; }
        /// <summary>
        /// Gets the flattening of the ellipsoid.
        /// </summary>
        /// <value>The flattening of the ellipsoid</value>
        public Double Flattening { get; private set; }
        /// <summary>
        /// Gets the denominator if the flattening.
        /// </summary>
        /// <value>The denominator if the flattening</value>
        public Double DenominatorOfFlattening { get { return 1 / Flattening; } }
        /// <summary>
        /// Gets the square of the excentricity.
        /// </summary>
        /// <value>The square of the excentricity</value>
        public Double ExcentricitySquared { get { return 2 * Flattening - Flattening * Flattening; } }
        #endregion
        #region constructor
        /// <summary>
        /// Initializes an ellipsoid.
        /// </summary>
        /// <param name="name">Name of the ellipsoid.</param>
        /// <param name="semiMajorAxis">Semi major axis of the ellipsoid.</param>
        /// <param name="flattening">Flattening of the ellipsoid.</param>
        public GeoEllipsoid(String name, Double semiMajorAxis, Double flattening)
        {
            Name = name;
            SemiMajorAxis = semiMajorAxis;
            Flattening = flattening;
        }
        /// <summary>
        /// Initializes an ellipsoid.
        /// </summary>
        /// <param name="value">Ellipsoid to be copies from.</param>
        public GeoEllipsoid(GeoEllipsoid value)
        {
            Name = value.Name;
            SemiMajorAxis = value.SemiMajorAxis;
            Flattening = value.Flattening;
        }
        #endregion
        #region ICloneable Members
        /// <summary>
        /// Returns a new instance copied from the current one.
        /// </summary>
        /// <returns>New instance of the ellipsoid.</returns>
        public object Clone()
        {
            return new GeoEllipsoid(this);
        }

        #endregion
        #region IEquatable Members
        /// <summary>
        /// Returns a value indicating whether this instance is equal to the specified.
        /// </summary>
        /// <param name="value">Instance to be compared with.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public bool Equals(GeoEllipsoid value)
        {
            bool result = Math.Abs(value.SemiMajorAxis - this.SemiMajorAxis) < 0.0001;
            result = result & (Math.Abs(value.DenominatorOfFlattening - this.DenominatorOfFlattening) < 0.0000000001);
            return result;
        }
        #endregion

        #region static fixed ellipsoids
        // http://www.colorado.edu/geography/gcraft/notes/datum/edlist.html

        /// <summary>
        /// Returns the WGS84 ellipsoid.
        /// </summary>
        /// <returns>WGS84 ellipsoid.</returns>
        public static GeoEllipsoid EllipsoidWGS84()
        {
            return new GeoEllipsoid("WGS84", 6378137, 1 / 298.257223563);
        }
        /// <summary>
        /// Returns the Everest ellipsoid.
        /// </summary>
        /// <returns>Everest ellipsoid.</returns>
        public static GeoEllipsoid EllipsoidEverest()
        {
            return new GeoEllipsoid("Everest", 6377276.345, 1 / 300.8017);
        }
        /// <summary>
        /// Returns the Clarke 1866 ellipsoid.
        /// </summary>
        /// <returns>Clarke 1866 ellipsoid.</returns>
        public static GeoEllipsoid EllipsoidClarke1866()
        {
            return new GeoEllipsoid("Clarke1866", 6378206.4, 1 / 294.978698);
        }
        #endregion
    }
}