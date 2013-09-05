using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public static class ConstituencyChecker
    {
        private static Boolean HasConstituencyAnnouncement(UInt32 geocode, EntityType requestType)
        {
            Boolean success = false;
            foreach ( var gazetteEntry in GlobalData.AllGazetteAnnouncements.AllAboutGeocode(geocode, false) )
            {
                foreach ( var content in gazetteEntry.Items )
                {
                    var constituencyAnnounce = content as GazetteConstituency;
                    if ( constituencyAnnounce != null )
                    {
                        if ( constituencyAnnounce.IsAboutGeocode(geocode, false) )
                        {
                            success = success || (constituencyAnnounce.type == requestType);
                        }
                    }
                }
            }
            return success;
        }

        public static IEnumerable<Entity> ThesabanWithoutConstituencies(UInt32 geocode)
        {
            var result = new List<Entity>();

            var geocodes = GlobalData.GetGeocodeList(geocode);
            geocodes.ReorderThesaban();
            foreach ( var entry in geocodes.Thesaban )
            {
                if ( entry.type.IsThesaban() & (!entry.obsolete) )
                {
                    Boolean success = HasConstituencyAnnouncement(entry.geocode, entry.type);

                    if ( (!success) & (entry.tambon != 0) )
                    {
                        success = HasConstituencyAnnouncement(entry.tambon, entry.type);
                    }
                    if ( !success )
                    {
                        result.Add(entry);
                    }
                }
            }
            return result;
        }
    }
}