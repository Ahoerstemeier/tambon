﻿using System;
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
        const double _LatitudeBangkok = 13.7535;
        const double _LongitudeBangkok = 100.5018;
        const double _AltitudeBangkok = 2.0;
        const String _BangkokGeoHash = "w4rqnzxpv";
        const String _BangkokMaidenhead = "OK03gs";

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
            GeoPoint basePoint = new GeoPoint(_LatitudeBangkok, _LongitudeBangkok, _AltitudeBangkok, GeoDatum.DatumWGS84());
            GeoPoint clonePoint = new GeoPoint(basePoint);
            Assert.IsTrue(basePoint.Equals(clonePoint));
            // Assert.AreEqual<GeoPoint>(lClonePoint, lBasePoint); // does not use the IEquatable
        }
        [TestMethod]
        public void TestCalcUTM()
        {
            // Dresden according to Wikipedia : 13° 44' 29"E 51° 02' 55"N
            GeoPoint basePoint = new GeoPoint(51.0 + 02.0 / 60.0 + 55.0 / 3600.0, 13.0 + 44.0 / 60.0 + 29.0 / 3600.0);
            UtmPoint utmPoint = basePoint.CalcUTM();

            // Expected result: Zone 33 North, Northing 5655984 Easting 411777
            UtmPoint expected = new UtmPoint(411777, 5655984, 33, true);
            Assert.IsTrue(expected.Equals(utmPoint));
        }
        [TestMethod]
        public void TestUTMToGeo()
        {
            // Dresden according to Wikipedia : 13° 44' 29"E 51° 02' 55"N = UTM 33U 0411777 5655984
            UtmPoint utmPoint = new UtmPoint("33U 0411777 5655984");
            GeoPoint geoPoint = new GeoPoint(utmPoint, GeoDatum.DatumWGS84());
            GeoPoint expected = new GeoPoint(51.0 + 02.0 / 60.0 + 55.0 / 3600.0, 13.0 + 44.0 / 60.0 + 29.0 / 3600.0);
            Assert.IsTrue(expected.Equals(geoPoint));
        }
        [TestMethod]
        public void TestUTMToMGRS()
        {
            UtmPoint utmPoint = new UtmPoint("33U 0411777 5655984");
            String mgrs = utmPoint.ToMgrsString(7).Replace(" ", "");
            Assert.AreEqual("33UVS1177755984", mgrs);
        }
        [TestMethod]
        public void TestParseMGRS()
        {
            UtmPoint utmPoint = UtmPoint.ParseMgrsString("33UVS1177755984");
            UtmPoint utmPointExpected = new UtmPoint("33U 0411777 5655984");
            Assert.IsTrue(utmPointExpected.Equals(utmPoint));
        }
        [TestMethod]
        public void TestDatumConversion()
        {
            // example as of http://www.colorado.edu/geography/gcraft/notes/datum/gif/molodens.gif
            GeoPoint point = new GeoPoint(30, -100, 232, GeoDatum.DatumNorthAmerican27MeanConus());
            point.Datum = GeoDatum.DatumWGS84();
            GeoPoint expected = new GeoPoint(30.0002239, -100.0003696, 194.816, GeoDatum.DatumWGS84());
            Assert.IsTrue(expected.Equals(point));
        }
        [TestMethod]
        public void TestGeoHashToGeo()
        {
            GeoPoint geoPoint = new GeoPoint();
            geoPoint.GeoHash = _BangkokGeoHash;
            GeoPoint expected = new GeoPoint(_LatitudeBangkok, _LongitudeBangkok);
            Assert.IsTrue(expected.Equals(geoPoint));
        }
        [TestMethod]
        public void TestGeoToGeoHash()
        {
            GeoPoint geoPoint = new GeoPoint(_LatitudeBangkok, _LongitudeBangkok);
            String geoHash = geoPoint.GeoHash;
            Assert.Equals(geoHash, _BangkokGeoHash);
        }
        [TestMethod]
        public void TestParseCoordinateDegMinSec()
        {
            String coordinateString = " 13°45'46.08\" N 100°28'41.16\" E";
            GeoPoint geoPoint = new GeoPoint(coordinateString);
            Assert.IsTrue(new GeoPoint(13.7628, 100.478100).Equals(geoPoint));
        }
        [TestMethod]
        public void TestParseCoordinateDecimalDegree()
        {
            String coordinateString = " 13.7628° N 100.478100° E";
            GeoPoint geoPoint = new GeoPoint(coordinateString);
            Assert.IsTrue(new GeoPoint(13.7628, 100.478100).Equals(geoPoint));
        }

        [TestMethod]
        public void TestMaidenheadToGeo()
        {
            GeoPoint geoPoint = new GeoPoint();
            geoPoint.Maidenhead = _BangkokMaidenhead;
            GeoPoint expected = new GeoPoint(_LatitudeBangkok, _LongitudeBangkok);
            Assert.IsTrue(expected.Equals(geoPoint));
        }
        [TestMethod]
        public void TestGeoToMaidenhead()
        {
            GeoPoint geoPoint = new GeoPoint(_LatitudeBangkok, _LongitudeBangkok);
            String maidenhead = geoPoint.Maidenhead;
            maidenhead = maidenhead.Substring(0, _BangkokMaidenhead.Length);
            Assert.AreEqual(_BangkokMaidenhead,maidenhead);
        }
    }
}
