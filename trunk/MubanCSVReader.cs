using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace De.AHoerstemeier.Tambon
{
    class MubanCSVReader
    {
        public void Parse(Int32 iGeocode)
        {
            String lFilename = Path.Combine(AHGlobalSettings.HTMLCacheDir, "Muban");
            lFilename = Path.Combine(lFilename, iGeocode.ToString()+".txt");
            StreamReader lReader = new StreamReader(lFilename);
            Parse(lReader);
        }
        public void Parse(StreamReader iReader)
        {
            String lCurrentLine = String.Empty;
            PopulationDataEntry lCurrentChangwat = new PopulationDataEntry();
            PopulationDataEntry lCurrentAmphoe = new PopulationDataEntry();
            PopulationDataEntry lCurrentTambon = new PopulationDataEntry();

            while ((lCurrentLine = iReader.ReadLine()) != null)
            {
                var lSubStrings = lCurrentLine.Split(new Char[] { '\t' });
                if ((lSubStrings.Length>0) & (!String.IsNullOrEmpty(lSubStrings[0])) & (Helper.IsNumeric(lSubStrings[0]))) 
                {
                    PopulationDataEntry lCurrentMuban = new PopulationDataEntry();
                    String lAmphoe = lSubStrings[1].Replace('"', ' ').Trim();
                    String lTambon = lSubStrings[2].Replace('"', ' ').Trim();
                    lCurrentMuban.Geocode = Convert.ToInt32(lSubStrings[3].Replace('"', ' ').Trim());
                    lCurrentMuban.Name = lSubStrings[4].Replace('"', ' ').Trim();
                    Int32 lMuban = Convert.ToInt32(lSubStrings[5].Replace('"', ' ').Trim());
                    if (lMuban != (lCurrentMuban.Geocode % 100))
                    { 
                        String lComment = "Code is "+lCurrentMuban.Geocode.ToString()+',';
                        lComment = lComment + " Muban number is " + lMuban.ToString(); 
                        lCurrentMuban.Comment = lComment;
                        lCurrentMuban.Geocode = lCurrentMuban.Geocode - (lCurrentMuban.Geocode % 100) + lMuban;
                    }
                    if ((lCurrentMuban.Geocode / 10000) != lCurrentAmphoe.Geocode)
                    {
                        lCurrentAmphoe = new PopulationDataEntry();
                        lCurrentAmphoe.Name = lTambon;
                        lCurrentAmphoe.Geocode = (lCurrentMuban.Geocode / 10000);
                        lCurrentChangwat.SubEntities.Add(lCurrentAmphoe);
                    }
                    if ((lCurrentMuban.Geocode / 100) != lCurrentTambon.Geocode)
                    {
                        lCurrentTambon = new PopulationDataEntry();
                        lCurrentTambon.Name = lTambon;
                        lCurrentTambon.Geocode = (lCurrentMuban.Geocode / 100);
                        lCurrentAmphoe.SubEntities.Add(lCurrentTambon);
                    }
                    lCurrentTambon.SubEntities.Add(lCurrentMuban);
                }
            }
            // ToDo: Save To XML
            // Compare with existing Geocode XML
            // Muban per Tambon statistics
        }
    
    }
}

// Column 1 : is number, then use
// Column 2 : Amphoe name
// Column 3 : Tambon name
// Column 4 : Code
// Column 5 : Name
// Column 6 : Muban number

// source file: cache-dir/muban/##.txt