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
                        if (EntityTypeHelper.Thesaban.Contains(lEntry.Type))
                        {
                            Boolean lSuccess =false;
                            // ToDo: How about the former TAO only qualified by Tambon geocode?
                            foreach (RoyalGazette lGazette in TambonHelper.GlobalGazetteList.AllAboutEntity(lEntry.Geocode,false))
                            {
                                foreach (RoyalGazetteContent lContent in lGazette.Content)
                                {
                                    if (lContent.IsAboutGeocode(lEntry.Geocode,false) && (lContent.GetType()==typeof(RoyalGazetteContentConstituency)))
                                    {
                                        lSuccess = lSuccess || (((RoyalGazetteContentConstituency)lContent).Type==lEntry.Type);
                                    }
                                }
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
