using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using De.AHoerstemeier.Tambon;

namespace Tambon
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AHGlobalSettings.LoadSettings();
        }

        private void btnGazetteMirror_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(Helper.GlobalGazetteList.MirrorAllToCache);
            t.Name = "Worker Thread Gazette Mirror";
            t.Start();
        }
        private void PopulationDataReadyShow(PopulationData iData)
        {
            Invoke(new PopulationData.ProcessingFinished(ShowPopulationDialog), new object[] { iData });
        }
        private void PopulationDataReadyCheck(PopulationData iData)
        {
            Invoke(new PopulationData.ProcessingFinished(CheckPopulationData), new object[] { iData });
        }
        private void ShowPopulationDialog(PopulationData iData)
        {
            var lDataForm = new PopulationDataView();
            lDataForm.OnShowGazette += ShowGazetteDialog;
            lDataForm.Data = iData;
            lDataForm.Show();
        }
        private void CheckPopulationData(PopulationData iData)
        {
            iData.SaveXML();
            var lEntitiesWithoutGeocode = iData.EntitiesWithoutGeocode();
            if (lEntitiesWithoutGeocode.Count > 0)
            {
                String lMessage = "";

                foreach (PopulationDataEntry lEntry in lEntitiesWithoutGeocode)
                {
                    lMessage = lMessage + lEntry.Name + "\n";
                }

                MessageBox.Show(lEntitiesWithoutGeocode.Count.ToString() + " entities without geocode in " + iData.year.ToString() + "\n" + lMessage);
            }
            var lEntitiesWithInvalidGeocode = iData.EntitiesWithInvalidGeocode();
            if (lEntitiesWithInvalidGeocode.Count > 0)
            {
                String lMessage = "";

                foreach (PopulationDataEntry lEntry in lEntitiesWithInvalidGeocode)
                {
                    lMessage = lMessage + lEntry.Geocode.ToString()+ ' ' + lEntry.Name + "\n";
                }

                MessageBox.Show(lEntitiesWithInvalidGeocode.Count.ToString() + " entities with invalid geocode in " + iData.year.ToString() + "\n" + lMessage);
            }

        }
        private void btnPopulation_Click(object sender, EventArgs e)
        {
            PopulationData lDownloader = new PopulationData(Convert.ToInt32(edt_year.Value), GetCurrentChangwat().Geocode);
            Thread lThread = new Thread(lDownloader.Process);

            lDownloader.OnProcessingFinished += PopulationDataReadyCheck;
            lDownloader.OnProcessingFinished += PopulationDataReadyShow;

            lThread.Name = "Worker Thread Population " + GetCurrentChangwat().English + " " + lDownloader.year.ToString();
            lThread.Start();
        }

        private void FillChangwatCombobox(ComboBox cbx_changwat)
        {
            Helper.LoadGeocodeList();

            cbx_changwat.Items.Clear();
            foreach (PopulationDataEntry lEntry in Helper.Geocodes)
            {
                cbx_changwat.Items.Add(lEntry);
                if (lEntry.Geocode == 84)
                {
                    cbx_changwat.SelectedItem = lEntry;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FillChangwatCombobox(cbx_changwat);
        }

        private void btnPopulationDownloadAll_click(object sender, EventArgs e)
        {
            var lChangwat = (PopulationDataEntry)cbx_changwat.SelectedItem;
            Int32 lGeocode = lChangwat.Geocode;
            for (int lYear = 1993; lYear <= 2007; lYear++)
            {

                PopulationData lDownloader = new PopulationData(lYear, lGeocode);
                lDownloader.OnProcessingFinished += PopulationDataReadyCheck;
                Thread t = new Thread(lDownloader.Process);
                t.Name = "Worker Thread Population " + lChangwat.English + " " + lDownloader.year.ToString();
                t.Start();
            }
        }
        private PopulationDataEntry GetCurrentChangwat()
        {
            var lChangwat = (PopulationDataEntry)cbx_changwat.SelectedItem;
            return lChangwat;
        }

        private void btn_GazetteShow_Click(object sender, EventArgs e)
        {
            RoyalGazetteList lGazetteList = Helper.GlobalGazetteList.AllAboutEntity(GetCurrentChangwat().Geocode, true);
            RoyalGazetteViewer.ShowGazetteDialog(lGazetteList);
        }

        private void btn_GazetteLoad_Click(object sender, EventArgs e)
        {
            // DirectoryInfo lDirInfo = new DirectoryInfo("e:\\thailand\\dopa\\tambon\\gazette\\");

            string lDir = Path.Combine(Application.StartupPath, "gazette");
            if (Directory.Exists(lDir))
            {
                foreach (string lFilename in Directory.GetFiles(lDir, "Gazette*.XML"))
                {
                    RoyalGazetteList lCurrentGazetteList = RoyalGazetteList.Load(lFilename);
                    Helper.GlobalGazetteList.AddRange(lCurrentGazetteList);
                }
                Boolean lHasEntries = (Helper.GlobalGazetteList.Count > 0);
                btn_GazetteShow.Enabled = lHasEntries;
                btn_GazetteShowAll.Enabled = lHasEntries;
            }
            else
            {
                MessageBox.Show(this, "Fatal error: Directory " + lDir + " is not existing.\nApplication will be terminated.", "Directory error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void cbx_changwat_SelectedValueChanged(object sender, EventArgs e)
        {
            Int32 lGeocode = GetCurrentChangwat().Geocode;
            Boolean lHasData = (File.Exists(Helper.GeocodeSourceFile(lGeocode)));
            btn_Population.Enabled = lHasData;
            btn_PopulationAll.Enabled = lHasData;
        }

        private void GazetteNewsReady(RoyalGazetteList data)
        {
            Invoke(new RoyalGazetteList.ProcessingFinished(RoyalGazetteViewer.ShowGazetteNewsDialog), new object[] { data });
        }
        private void ShowGazetteDialog(RoyalGazetteList data)
        {
            Invoke(new RoyalGazetteList.ProcessingFinished(RoyalGazetteViewer.ShowGazetteDialog), new object[] { data });
        }
        private void btn_CheckForNews_Click(object sender, EventArgs e)
        {
            RoyalGazetteOnlineSearch lSearcher = new RoyalGazetteOnlineSearch();
            lSearcher.OnProcessingFinished += GazetteNewsReady;
            Thread t = new Thread(lSearcher.SearchNewsNow);
            t.Name = "Worker Thread Gazette News";
            t.Start();
        }

        private void btn_GazetteShowAll_Click(object sender, EventArgs e)
        {
            ShowGazetteDialog(Helper.GlobalGazetteList);
        }

        private void btn_GazetteSearchYear_Click(object sender, EventArgs e)
        {
            RoyalGazetteOnlineSearch lSearcher = new RoyalGazetteOnlineSearch();
            var lGazetteList = lSearcher.SearchNews(new DateTime((Int32)edt_year.Value, 1, 1));
            GazetteNewsReady(lGazetteList);
        }

        private void btn_LoadCcaatt_Click(object sender, EventArgs e)
        {
            // ToDo: Konfigurierbares Verzeichnis für ccaatt files
            openFileDialogCCAATT.InitialDirectory = @"e:\Thailand\geocode\";
            openFileDialogCCAATT.FileName = "ccaatt.txt";
            openFileDialogCCAATT.ShowDialog();
        }

        private void ccaatt_postprocessing(AHDopaGeocodeList iData)
        {
            iData.RemoveAllKnownGeocodes();
            iData.ExportToXML(Path.Combine(AHGlobalSettings.XMLOutputDir, "unknowngeocodes.xml"));
            var lForm = new StringDisplayForm("New Geocodes", iData.ToString());
            lForm.Show();
        }
        private void openFileDialogCCAATT_FileOk(object sender,
                System.ComponentModel.CancelEventArgs e)
        {
            this.Activate();
            String[] lFiles = openFileDialogCCAATT.FileNames;
            foreach (String lFileName in lFiles)
            {
                var lData = new AHDopaGeocodeList(lFileName);
                ccaatt_postprocessing(lData);
            }
        }
        private void openFileDialogXML_FileOk(object sender,
                System.ComponentModel.CancelEventArgs e)
        {
            this.Activate();
            String[] lFiles = openFileDialogXML.FileNames;
            RoyalGazetteList lCurrentGazetteList = new RoyalGazetteList();
            foreach (String lFileName in lFiles)
            {
                RoyalGazetteList lLoadedGazetteList = RoyalGazetteList.Load(lFileName);
                lCurrentGazetteList.AddRange(lLoadedGazetteList);
            }
            ShowGazetteDialog(lCurrentGazetteList);
        }

        private void btn_DownloadCcaatt_Click(object sender, EventArgs e)
        {
            var lData = AHDopaGeocodeList.CreateFromOnline();
            ccaatt_postprocessing(lData);
        }

        private void btn_GazetteSearch_Click(object sender, EventArgs e)
        {
            RoyalGazetteSearch lSearchWindow = new RoyalGazetteSearch();
            lSearchWindow.OnSearchFinished += GazetteNewsReady;
            lSearchWindow.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AHAmphoeComDownloader lDownloader = new AHAmphoeComDownloader();
            Int32 lGeocode = GetCurrentChangwat().Geocode;
            var lData = lDownloader.DoItAll(lGeocode);
            XmlDocument lXmlDocument = new XmlDocument();
            var lElement = (XmlElement)lXmlDocument.CreateNode("element", "mirror", "");
            lElement.SetAttribute("date", DateTime.Now.ToString("yyyy-MM-dd"));
            lXmlDocument.AppendChild(lElement);
            foreach (var lDataEntry in lData)
            {
                lDataEntry.ExportToXML(lElement);
            }
            Directory.CreateDirectory(OutputDir());
            lXmlDocument.Save(Path.Combine(OutputDir(), "amphoe" + lGeocode.ToString() + ".xml"));

        }
        private String OutputDir()
        {
            String retval = Path.Combine(AHGlobalSettings.XMLOutputDir, "amphoe");
            return retval;
        }

        private void ButtonThesaban_Click(object sender, EventArgs e)
        {
            var lDownloader = new KontessabanDownloader();
            var lData = lDownloader.DoIt();

            XmlDocument lXmlDocumentAll = new XmlDocument();
            var lNodeAll = (XmlElement)lXmlDocumentAll.CreateNode("element", "country", "");
            lXmlDocumentAll.AppendChild(lNodeAll);
            XmlDocument lXmlDocumentNew = new XmlDocument();
            var lNodeNew = (XmlElement)lXmlDocumentNew.CreateNode("element", "country", "");
            lXmlDocumentNew.AppendChild(lNodeNew);
            foreach (KontessabanDataEntry entity in lData)
            {
                entity.ExportToXML(lNodeAll);
                if (entity.Geocode == 0)
                {
                    entity.ExportToXML(lNodeNew);
                }
            }
            lXmlDocumentAll.Save(Path.Combine(AHGlobalSettings.XMLOutputDir, "Tessaban.xml"));
            lXmlDocumentNew.Save(Path.Combine(AHGlobalSettings.XMLOutputDir, "NewTessaban.xml"));
        }

        private void btnSetup_Click(object sender, EventArgs e)
        {
            Setup lSetupForm = new Setup();
            lSetupForm.ShowDialog();
        }

        private void btnNumerals_Click(object sender, EventArgs e)
        {
            NumeralsHelper lNumeralsHelperForm = new NumeralsHelper();
            lNumeralsHelperForm.Show();
        }

        private void btnTambonFrequency_Click(object sender, EventArgs e)
        {
            List<EntityType> lTypes = new List<EntityType>()
            {
                EntityType.Tambon
            };
            EntityCounter lCounter = new EntityCounter(lTypes);
            var lChangwat = (PopulationDataEntry)cbx_changwat.SelectedItem;
            // lCounter.BaseGeocode = lChangwat.Geocode;
            lCounter.Calculate();

            var lForm = new StringDisplayForm("Tambon", lCounter.CommonNames(20));
            lForm.Show();
        }

        private void btnTambonCreation_Click(object sender, EventArgs e)
        {
            CreationStatisticForm lForm = new CreationStatisticForm();
            lForm.Show();
        }

        private void btn_LoadGazetteXML_Click(object sender, EventArgs e)
        {
            openFileDialogXML.InitialDirectory = Path.Combine(Application.StartupPath, "gazette");
            openFileDialogXML.ShowDialog();
        }

        private void btnAreaUnits_Click(object sender, EventArgs e)
        {
            UnitConverterForm lForm = new UnitConverterForm();
            lForm.Show();
        }

        private void btnGovernor_Click(object sender, EventArgs e)
        {
            ProvinceGovernorParser lParser = new ProvinceGovernorParser();
            lParser.ParseUrl("http://www.moi.go.th/portal/page?_pageid=33,76197,33_76230&_dad=portal&_schema=PORTAL");
            // lParser.ParseFile("C:\\Users\\Andy\\Dropbox\\My Dropbox\\Misc\\governor list 2008.htm");

            XmlDocument lXmlDocument = new XmlDocument();
            lParser.ExportToXML(lXmlDocument);
            Directory.CreateDirectory(AHGlobalSettings.XMLOutputDir);
            lXmlDocument.Save(Path.Combine(AHGlobalSettings.XMLOutputDir, "governor.xml"));

            var lNewGovernors = lParser.NewGovernorsList();
            lXmlDocument = new XmlDocument();
            lParser.ExportToXML(lXmlDocument);
            lXmlDocument.Save(Path.Combine(AHGlobalSettings.XMLOutputDir, "newgovernor.xml"));

            var lForm = new StringDisplayForm("New governors", lParser.NewGovernorsText());
            lForm.Show();
        }

        private void btnBoard_Click(object sender, EventArgs e)
        {
            String lFilename = Path.GetDirectoryName(Application.ExecutablePath) + "\\misc\\board.xml";
            BoardMeetingList lBoardMeetingList = BoardMeetingList.Load(lFilename);
            if (lBoardMeetingList != null)
            {
                FrequencyCounter lCounter = lBoardMeetingList.EffectiveDateTillPublication();
                lCounter = lBoardMeetingList.MeetingDateTillPublication();
            }
        }

        private void btnMuban_Click(object sender, EventArgs e)
        {
            Int32 lGeocode = GetCurrentChangwat().Geocode;
            var lMuban = new MubanCSVReader();
            var lData = lMuban.Parse(lGeocode);

            var lForm = new StringDisplayForm("Muban", lMuban.Information(lData));

            String lOutFilename = Path.Combine(AHGlobalSettings.XMLOutputDir, "Muban"+lGeocode.ToString()+".kml");
            lData.ExportToKml(lOutFilename);
            lForm.Show();
        }

        private void btnMubanNames_Click(object sender, EventArgs e)
        {
            List<EntityType> lTypes = new List<EntityType>()
            {
                EntityType.Muban
            };
            EntityCounter lNamesCounter = new EntityCounter(lTypes);
            if (chkUseCsv.Checked)
            {
                var lList = new List<PopulationDataEntry>();
                var lCounter = new FrequencyCounter();
                foreach (PopulationDataEntry lEntry in Helper.Geocodes)
                {
                    if (lEntry.Geocode != 10)
                    {
                        var lReader = new MubanCSVReader();
                        var lData = lReader.Parse(lEntry.Geocode);
                        MubanCSVReader.Statistics(lData, lCounter);
                        var lFlatData = lData.FlatList(lTypes);
                        lList.AddRange(lFlatData);
                    }
                }
                var lFormStatistics = new StringDisplayForm("Muban", MubanCSVReader.StatisticsText(lCounter));
                lFormStatistics.Show();
                lNamesCounter.Calculate(lList);
            }
            else
            {
                lNamesCounter.Calculate();
            }
            var lFormNames = new StringDisplayForm("Muban", lNamesCounter.CommonNames(20));
            lFormNames.Show();

        }

        private void btnCreateKml_Click(object sender, EventArgs e)
        {
            var lList = new List<PopulationDataEntry>();
            foreach (PopulationDataEntry lEntry in Helper.Geocodes)
            {
                    String lFilename = Helper.GeocodeSourceFile(lEntry.Geocode);
                    if (File.Exists(lFilename))
                    {
                        PopulationData lEntities = PopulationData.Load(lFilename);
                        lList.Add(lEntities.Data);
                    }
                }
            PopulationDataEntry lMaster = new PopulationDataEntry();
            lMaster.SubEntities.AddRange(lList);
            String lOutFilename = Path.Combine(AHGlobalSettings.XMLOutputDir,"offices.kml");
            lMaster.ExportToKml(lOutFilename);

            var lGeotaggedOffices = new Dictionary<OfficeType, Int32>();
            var lAnyOffices = new Dictionary<OfficeType, Int32>();
            var lFlatList = lMaster.FlatList(EntityTypeHelper.AllEntityTypes);
            foreach (var lEntity in lFlatList)
            {
                foreach (var lOffice in lEntity.Offices)
                {
                    if (lOffice.Location != null)
                    {
                        if (!lGeotaggedOffices.ContainsKey(lOffice.Type))
                        {
                            lGeotaggedOffices.Add(lOffice.Type, 0); 
                        }
                        lGeotaggedOffices[lOffice.Type]++;
                    }
                    if (!lAnyOffices.ContainsKey(lOffice.Type))
                    {
                        lAnyOffices.Add(lOffice.Type, 0);
                    }
                    lAnyOffices[lOffice.Type]++;
                }
            }
            String lOfficeType = String.Empty;
            foreach (OfficeType i in System.Enum.GetValues(typeof(OfficeType)))
            {
                if (lGeotaggedOffices.ContainsKey(i))
                {
                    lOfficeType = lOfficeType + 
                        i.ToString()+ ' ' + 
                        lGeotaggedOffices[i].ToString() + 
                        " (of " + lAnyOffices[i].ToString() + ")\r\n";
                }
            }
            var lForm = new StringDisplayForm("Office types", lOfficeType);
            lForm.Show();
        }

        private void btnGeo_Click(object sender, EventArgs e)
        {
            GeoCoordinateForm lGeoForm = new GeoCoordinateForm();
            lGeoForm.Show();
        }
    }
}