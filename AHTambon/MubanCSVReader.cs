using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using De.AHoerstemeier.Geo;

namespace De.AHoerstemeier.Tambon
{
    public class MubanCSVReader
    {
        #region constructor
        public MubanCSVReader()
        {
        }
        #endregion
        #region methods
        public PopulationDataEntry Parse(Int32 iGeocode)
        {
            String lFilename = Path.Combine(GlobalSettings.HTMLCacheDir, "Muban");
            lFilename = Path.Combine(lFilename, "Muban" + iGeocode.ToString() + ".txt");
            StreamReader lReader = new StreamReader(lFilename);
            PopulationDataEntry RetVal = Parse(lReader);
            lReader.Dispose();
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
            // Column 7 : Location source/placemark
            // Column 8 : Location UTM Easting (47N, Indian 1974)
            // Column 9 : Location UTM Northing (47N, Indian 1974)

            String lCurrentLine = String.Empty;
            PopulationDataEntry lCurrentChangwat = new PopulationDataEntry();
            PopulationDataEntry lCurrentAmphoe = new PopulationDataEntry();
            PopulationDataEntry lCurrentTambon = new PopulationDataEntry();

            while ((lCurrentLine = iReader.ReadLine()) != null)
            {
                var lSubStrings = lCurrentLine.Split(new Char[] { '\t' });
                if ((lSubStrings.Length > 0) & (!String.IsNullOrEmpty(lSubStrings[0])) & TambonHelper.IsNumeric(lSubStrings[0]))
                {
                    PopulationDataEntry lCurrentMuban = new PopulationDataEntry();
                    String lAmphoe = lSubStrings[1].Replace('"', ' ').Trim();
                    String lTambon = lSubStrings[2].Replace('"', ' ').Trim();
                    String lGeocode = lSubStrings[3].Replace('"', ' ').Replace(" ", "").Trim();
                    lCurrentMuban.Geocode = Convert.ToInt32(lGeocode);
                    lCurrentMuban.Name = lSubStrings[4].Replace('"', ' ').Trim();
                    lCurrentMuban.Type = EntityType.Muban;
                    String lComment = lSubStrings[6].Replace('"', ' ').Trim();
                    String lEasting = lSubStrings[7].Replace('"', ' ').Replace('E', ' ').Trim();
                    String lNorthing = lSubStrings[8].Replace('"', ' ').Replace('N', ' ').Trim();
                    if (TambonHelper.IsNumeric(lEasting) && TambonHelper.IsNumeric(lNorthing))
                    {
                        EntityOffice lOffice = new EntityOffice();
                        lOffice.Type = OfficeType.VillageHeadmanOffice;
                        UtmPoint lUTMLocation = new UtmPoint(Convert.ToInt32(lEasting), Convert.ToInt32(lNorthing), 47, true);
                        lOffice.Location = new GeoPoint(lUTMLocation, GeoDatum.DatumIndian1975());
                        lOffice.Location.Datum = GeoDatum.DatumWGS84();
                        lCurrentMuban.Offices.Add(lOffice);
                    }
                    String lMubanString = lSubStrings[5].Replace('"', ' ').Trim();
                    if (TambonHelper.IsNumeric(lMubanString))
                    {
                        Int32 lMuban = Convert.ToInt32(lMubanString);
                        if (lMuban != (lCurrentMuban.Geocode % 100))
                        {
                            lComment = lComment + Environment.NewLine + "Code is " + lCurrentMuban.Geocode.ToString() + ',';
                            lComment = lComment + " Muban number is " + lMuban.ToString();
                            lCurrentMuban.Geocode = lCurrentMuban.Geocode - (lCurrentMuban.Geocode % 100) + lMuban;
                        }
                    }
                    if ((lCurrentMuban.Geocode / 10000) != lCurrentAmphoe.Geocode)
                    {
                        lCurrentAmphoe = new PopulationDataEntry();
                        lCurrentAmphoe.Name = lTambon;
                        lCurrentAmphoe.Type = EntityType.Amphoe;
                        lCurrentAmphoe.Geocode = (lCurrentMuban.Geocode / 10000);
                        lCurrentChangwat.SubEntities.Add(lCurrentAmphoe);
                    }
                    if ((lCurrentMuban.Geocode / 100) != lCurrentTambon.Geocode)
                    {
                        lCurrentTambon = new PopulationDataEntry();
                        lCurrentTambon.Name = lTambon;
                        lCurrentTambon.Type = EntityType.Tambon;
                        lCurrentTambon.Geocode = (lCurrentMuban.Geocode / 100);
                        lCurrentAmphoe.SubEntities.Add(lCurrentTambon);
                    }
                    lCurrentMuban.Comment = lComment;
                    lCurrentTambon.SubEntities.Add(lCurrentMuban);
                }
            }
            lCurrentChangwat.Type = EntityType.Changwat;
            return lCurrentChangwat;

        }
        static public void Statistics(PopulationDataEntry iChangwat, FrequencyCounter ioCounter)
        {
            foreach (PopulationDataEntry lAmphoe in iChangwat.SubEntities)
            {
                foreach (PopulationDataEntry lTambon in lAmphoe.SubEntities)
                {
                    Int32 lNumberOfMuban = lTambon.SubEntities.Count;
                    ioCounter.IncrementForCount(lNumberOfMuban, lTambon.Geocode);
                }
            }
        }
        static public FrequencyCounter Statistics(PopulationDataEntry iChangwat)
        {
            FrequencyCounter lCounter = new FrequencyCounter();
            Statistics(iChangwat, lCounter);
            return lCounter;
        }
        static public String StatisticsText(FrequencyCounter iCounter)
        {
            StringBuilder lBuilder = new StringBuilder();

            Int32 lCount = iCounter.NumberOfValues;
            lBuilder.AppendLine(lCount.ToString() + " Tambon");
            lBuilder.AppendLine(Math.Round(iCounter.MeanValue * lCount).ToString() + " Muban");
            lBuilder.AppendLine();
            lBuilder.AppendLine(iCounter.MeanValue.ToString("F2", CultureInfo.InvariantCulture) + " Muban per Tambon");
            lBuilder.AppendLine(iCounter.MaxValue.ToString() + " Muban per Tambon max.");
            String lTambonCodes = String.Empty;
            if (iCounter.MaxValue != 0)
            {
                foreach (var lEntry in iCounter.Data[iCounter.MaxValue])
                {
                    lTambonCodes = lTambonCodes + lEntry.ToString() + ", ";
                }
            }
            if (lTambonCodes.Length > 0)
            {
                lBuilder.AppendLine(lTambonCodes.Substring(0, lTambonCodes.Length - 2));
            }
            String RetVal = lBuilder.ToString();
            return RetVal;
        }
        static public String StatisticsText(PopulationDataEntry iChangwat)
        {
            FrequencyCounter lStatistics = Statistics(iChangwat);
            String RetVal = StatisticsText(lStatistics);
            return RetVal;
        }
        public Dictionary<PopulationDataEntry, PopulationDataEntry> DifferentMubanNames(PopulationDataEntry iChangwat)
        {
            var RetVal = new Dictionary<PopulationDataEntry, PopulationDataEntry>();

            PopulationData lGeocodes = TambonHelper.GetGeocodeList(iChangwat.Geocode);
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
                                if (!TambonHelper.IsSameMubanName(lMubanDopa.Name, lMuban.Name))
                                {
                                    RetVal.Add(lMuban, lMubanDopa);
                                }
                            }
                        }
                    }

                }
            }
            return RetVal;
        }

        public string Information(PopulationDataEntry iChangwat)
        {
            StringBuilder lBuilder = new StringBuilder();

            lBuilder.AppendLine(StatisticsText(iChangwat));
            lBuilder.AppendLine();

            foreach (KeyValuePair<PopulationDataEntry, PopulationDataEntry> lKeyValuePair in DifferentMubanNames(iChangwat))
            {
                lBuilder.Append(lKeyValuePair.Key.Geocode.ToString());
                lBuilder.Append(' ');
                lBuilder.Append(TambonHelper.StripBanOrChumchon(lKeyValuePair.Key.Name));
                lBuilder.Append(" instead of ");
                lBuilder.AppendLine(TambonHelper.StripBanOrChumchon(lKeyValuePair.Value.Name));
            }

            String RetVal = lBuilder.ToString();
            return RetVal;
        }
        #endregion
    }
}

// ToDo: Save To XML