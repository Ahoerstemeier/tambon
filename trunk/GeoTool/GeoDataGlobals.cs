using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using De.AHoerstemeier.Geo;
using System.Configuration;

namespace De.AHoerstemeier.GeoTool
{
    public class GeoDataGlobals
    {
        public GeoPoint DefaultLocation
        {
            get;
            private set;
        }

        public String BingMapsKey
        {
            get;
            private set;
        }

        private GeoDataGlobals()
        {
            DefaultLocation = new GeoPoint();
            // DefaultLocation.GeoHash = "9f2w1p8zf";  // Bangkok City Hall
        }

        /// <summary>
        /// Instance of this singleton.
        /// </summary>
        private static GeoDataGlobals _instance = null;
        /// <summary>
        /// Lock to protect the instance during creation.
        /// </summary>
        private static Object _syncRoot = new Object();

        /// <summary>
        /// Gets the instance of this singleton.
        /// </summary>
        /// <value>Instance of the singleton.</value>
        public static GeoDataGlobals Instance
        {
            get
            {
                if ( _instance == null )
                {
                    lock ( _syncRoot )
                    {
                        if ( _instance == null )
                            _instance = GeoDataGlobals.Load();
                    }
                }

                return _instance;
            }
        }

        private static GeoDataGlobals Load()
        {
            GeoDataGlobals retVal = null;  // Don't initialize here, it will be set up with a default in case all other methods fail!
            AppSettingsReader reader = new AppSettingsReader();

            retVal = new GeoDataGlobals();
            try
            {
                retVal.BingMapsKey = (String)reader.GetValue("BingMapsKey", typeof(String));
            }
            catch ( InvalidOperationException )
            {
            }

            try
            {
                retVal.DefaultLocation = new GeoPoint((String)reader.GetValue("DefaultLocation", typeof(String)));
            }
            catch ( InvalidOperationException )
            {
            }
            catch ( ArgumentException )
            {
            }

            return retVal;
        }
    }

}
