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
        public PopulationDataEntry Parse(Int32 geocode)
        {
            String filename = Path.Combine(GlobalSettings.HTMLCacheDir, "Muban");
            filename = Path.Combine(filename, "Muban" + geocode.ToString() + ".txt");

            if ( !File.Exists(filename) )
            {
                return null;
            }

            StreamReader reader = new StreamReader(filename);
            PopulationDataEntry result = Parse(reader);
            reader.Dispose();
            result.Geocode = geocode;
            return result;
        }
        public PopulationDataEntry Parse(StreamReader reader)
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

            String currentLine = String.Empty;
            PopulationDataEntry currentChangwat = new PopulationDataEntry();
            PopulationDataEntry currentAmphoe = new PopulationDataEntry();
            PopulationDataEntry currentTambon = new PopulationDataEntry();

            while ( (currentLine = reader.ReadLine()) != null )
            {
                var subStrings = currentLine.Split(new Char[] { '\t' });
                if ( (subStrings.Length > 0) & (!String.IsNullOrEmpty(subStrings[0])) & TambonHelper.IsNumeric(subStrings[0]) )
                {
                    PopulationDataEntry currentMuban = new PopulationDataEntry();
                    String amphoe = subStrings[1].Replace('"', ' ').Trim();
                    String tambon = subStrings[2].Replace('"', ' ').Trim();
                    String geocode = subStrings[3].Replace('"', ' ').Replace(" ", "").Trim();
                    currentMuban.Geocode = Convert.ToInt32(geocode);
                    currentMuban.Name = subStrings[4].Replace('"', ' ').Trim();
                    currentMuban.Type = EntityType.Muban;
                    String comment = subStrings[6].Replace('"', ' ').Trim();
                    String easting = subStrings[7].Replace('"', ' ').Replace('E', ' ').Trim();
                    String northing = subStrings[8].Replace('"', ' ').Replace('N', ' ').Trim();
                    if ( TambonHelper.IsNumeric(easting) && TambonHelper.IsNumeric(northing) )
                    {
                        EntityOffice office = new EntityOffice();
                        office.Type = OfficeType.VillageHeadmanOffice;
                        UtmPoint utmLocation = new UtmPoint(Convert.ToInt32(easting), Convert.ToInt32(northing), 47, true);
                        office.Location = new GeoPoint(utmLocation, GeoDatum.DatumIndian1975());
                        office.Location.Datum = GeoDatum.DatumWGS84();
                        currentMuban.Offices.Add(office);
                    }
                    String mubanString = subStrings[5].Replace('"', ' ').Trim();
                    if ( TambonHelper.IsNumeric(mubanString) )
                    {
                        Int32 muban = Convert.ToInt32(mubanString);
                        if ( muban != (currentMuban.Geocode % 100) )
                        {
                            comment = comment + Environment.NewLine + "Code is " + currentMuban.Geocode.ToString() + ',';
                            comment = comment + " Muban number is " + muban.ToString();
                            currentMuban.Geocode = currentMuban.Geocode - (currentMuban.Geocode % 100) + muban;
                        }
                    }
                    if ( (currentMuban.Geocode / 10000) != currentAmphoe.Geocode )
                    {
                        currentAmphoe = new PopulationDataEntry();
                        currentAmphoe.Name = tambon;
                        currentAmphoe.Type = EntityType.Amphoe;
                        currentAmphoe.Geocode = (currentMuban.Geocode / 10000);
                        currentChangwat.SubEntities.Add(currentAmphoe);
                    }
                    if ( (currentMuban.Geocode / 100) != currentTambon.Geocode )
                    {
                        currentTambon = new PopulationDataEntry();
                        currentTambon.Name = tambon;
                        currentTambon.Type = EntityType.Tambon;
                        currentTambon.Geocode = (currentMuban.Geocode / 100);
                        currentAmphoe.SubEntities.Add(currentTambon);
                    }
                    currentMuban.Comment = comment;
                    currentTambon.SubEntities.Add(currentMuban);
                }
            }
            currentChangwat.Type = EntityType.Changwat;
            return currentChangwat;

        }
        static public void Statistics(PopulationDataEntry changwat, FrequencyCounter counter)
        {
            foreach ( PopulationDataEntry amphoe in changwat.SubEntities )
            {
                foreach ( PopulationDataEntry tambon in amphoe.SubEntities )
                {
                    Int32 lNumberOfMuban = tambon.SubEntities.Count;
                    counter.IncrementForCount(lNumberOfMuban, tambon.Geocode);
                }
            }
        }
        static public FrequencyCounter Statistics(PopulationDataEntry changwat)
        {
            FrequencyCounter counter = new FrequencyCounter();
            Statistics(changwat, counter);
            return counter;
        }
        static public String StatisticsText(FrequencyCounter counter)
        {
            StringBuilder builder = new StringBuilder();

            Int32 lCount = counter.NumberOfValues;
            builder.AppendLine(lCount.ToString() + " Tambon");
            builder.AppendLine(Math.Round(counter.MeanValue * lCount).ToString() + " Muban");
            builder.AppendLine();
            builder.AppendLine(counter.MeanValue.ToString("F2", CultureInfo.InvariantCulture) + " Muban per Tambon");
            builder.AppendLine(counter.MaxValue.ToString() + " Muban per Tambon max.");
            String tambonCodes = String.Empty;
            if ( counter.MaxValue != 0 )
            {
                foreach ( var lEntry in counter.Data[counter.MaxValue] )
                {
                    tambonCodes = tambonCodes + lEntry.ToString() + ", ";
                }
            }
            if ( tambonCodes.Length > 0 )
            {
                builder.AppendLine(tambonCodes.Substring(0, tambonCodes.Length - 2));
            }
            String result = builder.ToString();
            return result;
        }
        static public String StatisticsText(PopulationDataEntry changwat)
        {
            FrequencyCounter statistics = Statistics(changwat);
            String result = StatisticsText(statistics);
            return result;
        }
        public Dictionary<PopulationDataEntry, PopulationDataEntry> DifferentMubanNames(PopulationDataEntry changwat)
        {
            var RetVal = new Dictionary<PopulationDataEntry, PopulationDataEntry>();

            PopulationData geocodes = TambonHelper.GetGeocodeList(changwat.Geocode);
            PopulationDataEntry currentChangwat = geocodes.Data;
            foreach ( PopulationDataEntry currentAmphoe in currentChangwat.SubEntities )
            {
                foreach ( PopulationDataEntry currentTambon in currentAmphoe.SubEntities )
                {
                    foreach ( PopulationDataEntry currentMuban in currentTambon.SubEntities )
                    {
                        if ( currentMuban.Type == EntityType.Muban )
                        {
                            PopulationDataEntry mubanDopa = changwat.FindByCode(currentMuban.Geocode);
                            if ( mubanDopa != null )
                            {
                                if ( !TambonHelper.IsSameMubanName(mubanDopa.Name, currentMuban.Name) )
                                {
                                    RetVal.Add(currentMuban, mubanDopa);
                                }
                            }
                        }
                    }

                }
            }
            return RetVal;
        }

        public string Information(PopulationDataEntry changwat)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(StatisticsText(changwat));
            builder.AppendLine();

            foreach ( KeyValuePair<PopulationDataEntry, PopulationDataEntry> keyValuePair in DifferentMubanNames(changwat) )
            {
                builder.Append(keyValuePair.Key.Geocode.ToString());
                builder.Append(' ');
                builder.Append(TambonHelper.StripBanOrChumchon(keyValuePair.Key.Name));
                builder.Append(" instead of ");
                builder.AppendLine(TambonHelper.StripBanOrChumchon(keyValuePair.Value.Name));
            }

            String result = builder.ToString();
            return result;
        }
        #endregion
    }
}

// ToDo: Save To XML