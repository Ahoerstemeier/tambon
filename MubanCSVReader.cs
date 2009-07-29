using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace De.AHoerstemeier.Tambon
{
    class MubanCSVReader
    {
        #region constructor
        public MubanCSVReader()
        {
        }
        #endregion
        #region methods
        public PopulationDataEntry Parse(Int32 iGeocode)
        {
            String lFilename = Path.Combine(AHGlobalSettings.HTMLCacheDir, "Muban");
            lFilename = Path.Combine(lFilename, "Muban" + iGeocode.ToString() + ".txt");
            StreamReader lReader = new StreamReader(lFilename);
            PopulationDataEntry RetVal = Parse(lReader);
            RetVal.Geocode = iGeocode;
            return RetVal;
        }
        public PopulationDataEntry Parse(StreamReader iReader)
        {
            // Column 1 : is number, then use
            // Column 2 : Amphoe name
            // Column 3 : Tambon name
            // Column 4 : Code
            // Column 5 : Name
            // Column 6 : Muban number

            String lCurrentLine = String.Empty;
            PopulationDataEntry lCurrentChangwat = new PopulationDataEntry();
            PopulationDataEntry lCurrentAmphoe = new PopulationDataEntry();
            PopulationDataEntry lCurrentTambon = new PopulationDataEntry();

            while ((lCurrentLine = iReader.ReadLine()) != null)
            {
                var lSubStrings = lCurrentLine.Split(new Char[] { '\t' });
                if ((lSubStrings.Length > 0) & (!String.IsNullOrEmpty(lSubStrings[0])) & (Helper.IsNumeric(lSubStrings[0])))
                {
                    PopulationDataEntry lCurrentMuban = new PopulationDataEntry();
                    String lAmphoe = lSubStrings[1].Replace('"', ' ').Trim();
                    String lTambon = lSubStrings[2].Replace('"', ' ').Trim();
                    lCurrentMuban.Geocode = Convert.ToInt32(lSubStrings[3].Replace('"', ' ').Trim());
                    lCurrentMuban.Name = lSubStrings[4].Replace('"', ' ').Trim();
                    Int32 lMuban = Convert.ToInt32(lSubStrings[5].Replace('"', ' ').Trim());
                    if (lMuban != (lCurrentMuban.Geocode % 100))
                    {
                        String lComment = "Code is " + lCurrentMuban.Geocode.ToString() + ',';
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
            return lCurrentChangwat;

        }
        public FrequencyCounter Statistics(PopulationDataEntry iChangwat)
        {
            FrequencyCounter RetVal = new FrequencyCounter();
            foreach (PopulationDataEntry lAmphoe in iChangwat.SubEntities)
            {
                foreach (PopulationDataEntry lTambon in lAmphoe.SubEntities)
                {
                    Int32 lNumberOfMuban = lTambon.SubEntities.Count;
                    RetVal.IncrementForCount(lNumberOfMuban, lTambon.Geocode);
                }
            }
            return RetVal;
        }
        public String StatisticsText(PopulationDataEntry iChangwat)
        {
            StringBuilder lBuilder = new StringBuilder();
            FrequencyCounter lStatistics = Statistics(iChangwat);

            Int32 lCount = lStatistics.NumberOfValues;
            lBuilder.AppendLine(lCount.ToString() + " Tambon");
            lBuilder.AppendLine(Math.Round(lStatistics.MeanValue * lCount).ToString() + " Muban");
            lBuilder.AppendLine();
            lBuilder.AppendLine(lStatistics.MeanValue.ToString("F2", CultureInfo.InvariantCulture) + " Muban per Tambon");
            lBuilder.AppendLine(lStatistics.MaxValue.ToString() + " Muban per Tambon max.");

            String RetVal = lBuilder.ToString();
            return RetVal;
        }
        public Dictionary<PopulationDataEntry, PopulationDataEntry> DifferentMubanNames(PopulationDataEntry iChangwat)
        {
            var RetVal = new Dictionary<PopulationDataEntry, PopulationDataEntry>();

            String lFilename = Helper.GeocodeSourceFile(iChangwat.Geocode);
            if (File.Exists(lFilename))
            {
                PopulationData lGeocodes = PopulationData.Load(lFilename);
                PopulationDataEntry lChangwat = lGeocodes.Data;
                foreach (PopulationDataEntry lAmphoe in lChangwat.SubEntities)
                {
                    foreach (PopulationDataEntry lTambon in lAmphoe.SubEntities)
                    {
                        foreach (PopulationDataEntry lMuban in lTambon.SubEntities)
                        {
                            if (lMuban.Type == EntityType.Muban)
                            {
                                PopulationDataEntry lMubanDopa = iChangwat.FindByCode(lMuban.Geocode);
                                if (lMubanDopa != null)
                                {
                                    if (!Helper.IsSameMubanName(lMubanDopa.Name, lMuban.Name))
                                    {
                                        RetVal.Add(lMuban, lMubanDopa);
                                    }
                                }
                            }
                        }

                    }
                }
            }
            return RetVal;
        }
        #endregion
    }
}

// ToDo: Save To XML
// ToDo: Dialog to display results