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
        public void TestCopyConstructor()
        {
            GeoPoint lBasePoint = new GeoPoint(mLatitudeBangkok, mLongitudeBangkok);
            lBasePoint.Altitude = mAltitudeBangkok;
            lBasePoint.Datum = GeoDatum.DatumWGS84();
            GeoPoint lClonePoint = new GeoPoint(lBasePoint);
            Assert.IsTrue(lBasePoint.Equals(lClonePoint));
            // Assert.AreEqual<GeoPoint>(lClonePoint, lBasePoint); // does not use the IEquatable
        }
    }
}
