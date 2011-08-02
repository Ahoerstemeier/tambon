using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using De.AHoerstemeier.Geo;

namespace TestProject1
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class GeoPointTest
    {
        const double mLatitudeBangkok = 13.7535;
        const double mLongitudeBangkok = 100.5018;
        const double mAltitudeBangkok = 2.0;
        const String mBangkokGeoHash = "w4rqnzxpv";
        const String mBangkokMaidenhead = "OK03GS";

        public GeoPointTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestGeoPointCopyConstructor()
        {
            GeoPoint lBasePoint = new GeoPoint(mLatitudeBangkok, mLongitudeBangkok, mAltitudeBangkok, GeoDatum.DatumWGS84());
            GeoPoint lClonePoint = new GeoPoint(lBasePoint);
            Assert.IsTrue(lBasePoint.Equals(lClonePoint));
            // Assert.AreEqual<GeoPoint>(lClonePoint, lBasePoint); // does not use the IEquatable
        }
        [TestMethod]
        public void TestCalcUTM()
        {
            // Dresden according to Wikipedia : 13° 44' 29"E 51° 02' 55"N
            GeoPoint lBasePoint = new GeoPoint(51.0 + 02.0 / 60.0 + 55.0 / 3600.0, 13.0 + 44.0 / 60.0 + 29.0 / 3600.0);
            UTMPoint lUTMPoint = lBasePoint.CalcUTM();

            // Expected result: Zone 33 North, Northing 5655984 Easting 411777
            UTMPoint lExpected = new UTMPoint(411777, 5655984, 33, true);
            Assert.IsTrue(lExpected.Equals(lUTMPoint));
        }
        [TestMethod]
        public void TestUTMToGeo()
        {
            // Dresden according to Wikipedia : 13° 44' 29"E 51° 02' 55"N = UTM 33U 0411777 5655984
            UTMPoint lUTMPoint = new UTMPoint("33U 0411777 5655984");
            GeoPoint lGeoPoint = new GeoPoint(lUTMPoint, GeoDatum.DatumWGS84());
            GeoPoint lExpected = new GeoPoint(51.0 + 02.0 / 60.0 + 55.0 / 3600.0, 13.0 + 44.0 / 60.0 + 29.0 / 3600.0);
            Assert.IsTrue(lExpected.Equals(lGeoPoint));
        }
        [TestMethod]
        public void TestUTMToMGRS()
        {
            UTMPoint lUTMPoint = new UTMPoint("33U 0411777 5655984");
            String lMGRS = lUTMPoint.ToMGRSString(7).Replace(" ", "");
            Assert.AreEqual("33UVS1177755984", lMGRS);
        }
        [TestMethod]
        public void TestParseMGRS()
        {
            UTMPoint lUTMPoint = UTMPoint.ParseMGRSString("33UVS1177755984");
            UTMPoint lUTMPointExpected = new UTMPoint("33U 0411777 5655984");
            Assert.IsTrue(lUTMPointExpected.Equals(lUTMPoint));
        }
        [TestMethod]
        public void TestDatumCoversion()
        {
            // example as of http://www.colorado.edu/geography/gcraft/notes/datum/gif/molodens.gif
            GeoPoint lPoint = new GeoPoint(30, -100, 232, GeoDatum.DatumNorthAmerican27MeanConus());
            lPoint.Datum = GeoDatum.DatumWGS84();
            GeoPoint lExpected = new GeoPoint(30.0002239, -100.0003696, 194.816, GeoDatum.DatumWGS84());
            Assert.IsTrue(lExpected.Equals(lPoint));
        }
        [TestMethod]
        public void TestGeoHashToGeo()
        {
            GeoPoint lGeoPoint = new GeoPoint();
            lGeoPoint.GeoHash = mBangkokGeoHash;
            GeoPoint lExpected = new GeoPoint(mLatitudeBangkok, mLongitudeBangkok);
            Assert.IsTrue(lExpected.Equals(lGeoPoint));
        }
        [TestMethod]
        public void TestGeoToGeoHash()
        {
            GeoPoint lGeoPoint = new GeoPoint(mLatitudeBangkok, mLongitudeBangkok);
            String lGeoHash = lGeoPoint.GeoHash;
            Assert.IsTrue(lGeoHash == mBangkokGeoHash, "Returned " + lGeoHash + " instead of " + mBangkokGeoHash);
        }
        [TestMethod]
        public void TestParseCoordinate()
        {
            // String mCoordinateString = "N 13.7628° E 100.478100°";
            String mCoordinateString = " 13°45'46.08\" N 100°28'41.16\" E";
            GeoPoint lGeoPoint = new GeoPoint(mCoordinateString);
            Assert.IsTrue(Math.Abs(lGeoPoint.Latitude - 13.7628) < 1e-6, "Parsed latitude " + lGeoPoint.Latitude.ToString() + " not fitting to " + mCoordinateString);
            Assert.IsTrue(Math.Abs(lGeoPoint.Longitude - 100.4781) < 1e-6, "Parsed longitude " + lGeoPoint.Longitude.ToString() + " not fitting to " + mCoordinateString);
        }

        [TestMethod]
        public void TestMaidenheadToGeo()
        {
            GeoPoint lGeoPoint = new GeoPoint();
            lGeoPoint.Maidenhead = mBangkokMaidenhead;
            GeoPoint lExpected = new GeoPoint(mLatitudeBangkok, mLongitudeBangkok);
            Assert.IsTrue(lExpected.Equals(lGeoPoint));
        }
        [TestMethod]
        public void TestGeoToMaidenhead()
        {
            GeoPoint lGeoPoint = new GeoPoint(mLatitudeBangkok, mLongitudeBangkok);
            String lMaidenhead = lGeoPoint.Maidenhead;
            Assert.IsTrue(lMaidenhead == mBangkokMaidenhead, "Returned " + lMaidenhead + " instead of " + mBangkokMaidenhead);
        }
    }
}
