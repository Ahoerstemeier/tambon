using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace De.AHoerstemeier.Tambon
{
    public class ConstituencyChecker
    {
        private Int32 mGeocode;

        public ConstituencyChecker(Int32 iGeocode)
        {
            mGeocode = iGeocode;
        }

        private static Boolean HasConstituencyAnnouncement(Int32 iGeocode, EntityType iType)
        {
            Boolean lSuccess = false;
            foreach (RoyalGazette lGazette in TambonHelper.GlobalGazetteList.AllAboutEntity(iGeocode, false))
            {
                foreach (RoyalGazetteContent lContent in lGazette.Content)
                {
                    if (lContent.IsAboutGeocode(iGeocode, false) && (lContent.GetType() == typeof(RoyalGazetteContentConstituency)))
                    {
                        lSuccess = lSuccess || (((RoyalGazetteContentConstituency)lContent).Type == iType);
                    }
                }
            }
            return lSuccess;
        }

        public List<PopulationDataEntry> ThesabanWithoutConstituencies()
        {
            List<PopulationDataEntry> lResult = new List<PopulationDataEntry>();

                String lFilename = TambonHelper.GeocodeSourceFile(mGeocode);
                if (File.Exists(lFilename))
                {
                    PopulationData lGeocodes = PopulationData.Load(lFilename);
                    lGeocodes.ReOrderThesaban();
                    foreach (PopulationDataEntry lEntry in lGeocodes.Thesaban)
                    { 
                        if (EntityTypeHelper.Thesaban.Contains(lEntry.Type) & (!lEntry.Obsolete) )
                        {
                            Boolean lSuccess = HasConstituencyAnnouncement(lEntry.Geocode, lEntry.Type);

                            if ((!lSuccess) & (lEntry.GeocodeOfCorrespondingTambon != 0))
                            {
                                lSuccess = HasConstituencyAnnouncement(lEntry.GeocodeOfCorrespondingTambon, lEntry.Type);
                            }
                            if (!lSuccess)
                            {
                                lResult.Add(lEntry);
                            }
                        }
                    }
                }

            return lResult;
        }
    }
}
