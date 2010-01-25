using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Geo
{
    public class UTMPoint : ICloneable, IEquatable<UTMPoint>
    {
        #region properties
        public Int32 Northing { get; set; }
        public Int32 Easting { get; set; }
        public Int32 ZoneNumber { get; set; }
        // public char ZoneBand { get; set; }
        public Boolean IsNorthernHemisphere { get; set; }
        #endregion

        #region constructor
        public UTMPoint()
        {
        }
        public UTMPoint(string iValue)
        {
            ParseUTMString(iValue);
        }
        public UTMPoint(UTMPoint iValue)
        {
            Northing = iValue.Northing;
            Easting = iValue.Easting;
            ZoneNumber = iValue.ZoneNumber;
            IsNorthernHemisphere = iValue.IsNorthernHemisphere;
        }
        public UTMPoint(Int32 iNorthing, Int32 iEasting, Int32 iZoneNumber, Boolean iIsNorthernHemisphere)
        {
            Northing = iNorthing;
            Easting = iEasting;
            ZoneNumber = iZoneNumber;
            IsNorthernHemisphere = iIsNorthernHemisphere;
        }
        #endregion

        #region methods
        public char ZoneBand()
        {
            throw new NotImplementedException();
        }
        public String ToString(Int32 iDigits)
        {
            throw new NotImplementedException();
        }
        public String ToMGRSString(Int32 iDigits)
        {
            throw new NotImplementedException();
        }
        public void ParseUTMString(String iValue)
        { 
            throw new NotImplementedException(); 
        }
        public void ParseMGRSString(String iValue)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new UTMPoint(this);
        }

        #endregion
        #region IEquatable Members
        public bool Equals(UTMPoint iObj)
        {
            bool lResult = (iObj.Northing == this.Northing);
            lResult = lResult & (iObj.Easting == this.Easting);
            lResult = lResult & (iObj.ZoneNumber == this.ZoneNumber);
            lResult = lResult & (iObj.IsNorthernHemisphere == this.IsNorthernHemisphere);
            return lResult;
        }
        #endregion

    }
}
