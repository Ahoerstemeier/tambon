using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Xml;

namespace De.AHoerstemeier.Tambon
{
    public class DopaGeocodeList
    {
        private const String SourceURL = "http://www.dopa.go.th/dload/ccaa.txt";
        private PopulationDataEntry mGeocodes = new PopulationDataEntry();

        protected void ParseList(StreamReader iReader)
        {
            String lCurrentLine = String.Empty;
            PopulationDataEntry lCurrentEntry = null;
            while ((lCurrentLine = iReader.ReadLine()) != null)
            {
                if (lCurrentLine.Length > 8)
                {
                    Int32 lPos = 8;
                    if (lCurrentLine.Contains('|'))
                    {
                        lPos = lCurrentLine.IndexOf('|');
                        lCurrentLine = lCurrentLine.Replace("|", "");
                    }
                    String lGeocodeString = lCurrentLine.Substring(0, lPos);
                    Int32 lGeocode = Convert.ToInt32(lGeocodeString);
                    while (lGeocode % 100 == 0)
                    {
                        lGeocode = lGeocode / 100;
                    }
                    String lName = lCurrentLine.Substring(8, lCurrentLine.Length - 8);
                    lName = lName.Replace("|", "").Trim();
                    lCurrentEntry = new PopulationDataEntry();
                    lCurrentEntry.Geocode = lGeocode;
                    lCurrentEntry.ParseName(lName);
                    if (lGeocode < 100)
                    {
                        lCurrentEntry.Type = EntityType.Changwat;
                    }
                    mGeocodes.SubEntities.Add(lCurrentEntry);
                }
            }
        }

        #region constructor
        public DopaGeocodeList()
        {
        }
        public DopaGeocodeList(String iFileName)
        {
            //Stream lStream = new System.IO.FileStream(iFileName, System.IO.FileMode.Open);
            StreamReader lReader = new StreamReader(iFileName);
            ParseList(lReader);
            //lStream.Dispose();
        }
        #endregion
        static public DopaGeocodeList CreateFromOnline()
        {
            System.Net.WebClient lWebClient = new System.Net.WebClient();
            System.IO.Stream WebStream = lWebClient.OpenRead(SourceURL);
            DopaGeocodeList retval = new DopaGeocodeList();
            StreamReader lReader = new StreamReader(WebStream, TambonHelper.ThaiEncoding);
            retval.ParseList(lReader);
            return retval;
        }
        private void RemoveKnownGeocodes(PopulationDataEntry iEntry)
        {
            if (iEntry.Geocode != 0)
            {
                PopulationDataEntry lFoundEntry = mGeocodes.FindByCode(iEntry.Geocode);
                if (lFoundEntry != null)
                {
                    // TODO: Check for spelling changes
                    mGeocodes.SubEntities.Remove(lFoundEntry);
                }
            }
            if ((!EntityTypeHelper.Thesaban.Contains(iEntry.Type)) &&
                (iEntry.Type != EntityType.Tambon) &&
                (iEntry.Type != EntityType.Khwaeng))
            {
                foreach (PopulationDataEntry lEntry in iEntry.SubEntities)
                {
                    RemoveKnownGeocodes(lEntry);
                }
            }
        }
        public void RemoveAllKnownGeocodes()
        {
            foreach (var lEntry in TambonHelper.ProvinceGeocodes)
            {
                PopulationData lCurrentList = TambonHelper.GetGeocodeList(lEntry.Geocode);
                RemoveKnownGeocodes(lCurrentList.Data);
            }
        }

        public void ExportToXML(String iFilename)
        {
            XmlDocument lXmlDocument = new XmlDocument();
            mGeocodes.ExportToXML(lXmlDocument);
            lXmlDocument.Save(iFilename);
        }
        public override String ToString()
        {
            StringBuilder lBuilder = new StringBuilder();
            foreach (PopulationDataEntry lGeocode in mGeocodes.SubEntities)
            {
                lBuilder.Append(lGeocode.Geocode.ToString());
                lBuilder.Append(' ');
                lBuilder.AppendLine(lGeocode.Name);
            }
            return lBuilder.ToString();
        }
    }
}
