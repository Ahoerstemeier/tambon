using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using De.AHoerstemeier.Tambon;

namespace De.AHoerstemeier.Tambon.UI
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Directory to find the DOPA age table source files.
        /// </summary>
        private String _ageDataDirectory;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GlobalData.LoadBasicGeocodeList();

            FillChangwatDropDown();
            edtYear.Maximum = GlobalData.PopulationStatisticMaxYear;
            edtYear.Minimum = GlobalData.PopulationStatisticMinYear;
            edtYear.Value = edtYear.Maximum;

            cbxCensusYears.SelectedItem = cbxCensusYears.Items[0];

            // change to real settings
            //PopulationDataDownloader.CacheDirectory=Path.GetDirectoryName(Application.ExecutablePath) + "\\cache\\";
            //PopulationDataDownloader.OutputDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\output\\";

            var xmlOutputDirectory = ConfigurationManager.AppSettings["XmlOutputDirectory"];
            PopulationDataDownloader.OutputDirectory = xmlOutputDirectory;
            _ageDataDirectory = ConfigurationManager.AppSettings["AgeDataDirectory"];
            PopulationDataDownloader.OutputDirectory = xmlOutputDirectory;
            var pdfDirectory = ConfigurationManager.AppSettings["PdfDirectory"];
            if ( !String.IsNullOrEmpty(pdfDirectory) )
            {
                GlobalData.PdfDirectory = pdfDirectory;
            }

            Boolean allowTestFeatures = false;
            try
            {
                var allowTestFeaturesConfig = ConfigurationManager.AppSettings["AllowTestFeatures"];
                allowTestFeatures = Convert.ToBoolean(allowTestFeaturesConfig);
            }
            catch ( FormatException )
            {
            }

            grpTesting.Enabled = allowTestFeatures;
        }

        private void FillChangwatDropDown()
        {
            cbxChangwat.Items.Clear();
            foreach ( var entry in GlobalData.Provinces )
            {
                cbxChangwat.Items.Add(entry);
                if ( entry.geocode == GlobalData.PreferredProvinceGeocode )
                {
                    cbxChangwat.SelectedItem = entry;
                }
            }
        }

        private void btnCheckNames_Click(object sender, EventArgs e)
        {
            var romanizationMissing = new List<RomanizationEntry>();

            var country = new Entity();
            foreach ( var province in GlobalData.Provinces )
            {
                var provinceData = GlobalData.GetGeocodeList(province.geocode);
                country.entity.Add(provinceData);
            }
            var allEntities = country.FlatList();
            Int32 numberOfEntities = allEntities.Count();

            var romanizator = new Romanization();
            romanizator.Initialize(allEntities);
            var romanizations = romanizator.Romanizations;
            var romanizationMistakes = romanizator.RomanizationMistakes;
            var romanizationSuggestions = romanizator.FindRomanizationSuggestions(out romanizationMissing, allEntities);

            UInt32 provinceFilter = 0;
            //if ( cbxCheckNamesFiltered.Checked )
            //{
            //    provinceFilter = GetCurrentChangwat().Geocode;
            //}

            StringBuilder romanizationMistakesBuilder = new StringBuilder();
            Int32 romanizationMistakeCount = 0;
            foreach ( var entry in romanizationMistakes )
            {
                if ( GeocodeHelper.IsBaseGeocode(provinceFilter, entry.Key.Geocode) )
                {
                    romanizationMistakesBuilder.AppendLine(String.Format(CultureInfo.CurrentUICulture, "{0} {1}: {2} vs. {3}", entry.Key.Geocode, entry.Key.Name, entry.Key.English, entry.Value));
                    romanizationMistakeCount++;
                }
            }

            if ( romanizationMistakeCount > 0 )
            {
                var displayForm = new StringDisplayForm(String.Format(CultureInfo.CurrentUICulture, "Romanization problems ({0})", romanizationMistakeCount),
                    romanizationMistakesBuilder.ToString());
                displayForm.Show();
            }

            StringBuilder romanizationSuggestionBuilder = new StringBuilder();
            Int32 romanizationSuggestionCount = 0;
            foreach ( var entry in romanizationSuggestions )
            {
                if ( GeocodeHelper.IsBaseGeocode(provinceFilter, entry.Geocode) )
                {
                    var entity = allEntities.FirstOrDefault(x => x.geocode == entry.Geocode);
                    var suggestedName = entry.English;
                    romanizationSuggestionBuilder.AppendLine(String.Format(CultureInfo.CurrentUICulture, "<entity type=\"{0}\" geocode=\"{1}\" name=\"{2}\" english=\"{3}\" />", entity.type, entity.geocode, entity.name, suggestedName));
                    romanizationSuggestionCount++;
                }
            }
            if ( romanizationSuggestionCount > 0 )
            {
                var form = new StringDisplayForm(String.Format(CultureInfo.CurrentUICulture, "Romanization suggestions ({0}", romanizationSuggestionCount), romanizationSuggestionBuilder.ToString());
                form.Show();

                List<Tuple<String, String, Int32>> counter = new List<Tuple<String, String, Int32>>();
                foreach ( var entry in romanizationSuggestions )
                {
                    var found = counter.FirstOrDefault(x => x.Item1 == entry.Name);
                    if ( found == null )
                    {
                        counter.Add(Tuple.Create(entry.Name, entry.English, 1));
                    }
                    else
                    {
                        counter.Remove(found);
                        counter.Add(Tuple.Create(entry.Name, entry.English, found.Item3 + 1));
                    }
                }
                counter.RemoveAll(x => x.Item3 < 2);
                if ( counter.Any() )
                {
                    counter.Sort(delegate(Tuple<String, String, Int32> p1, Tuple<String, String, Int32> p2)
                    {
                        return (p2.Item3.CompareTo(p1.Item3));
                    });

                    Int32 suggestionCounter = 0;
                    StringBuilder sortedBuilder = new StringBuilder();
                    foreach ( var entry in counter )
                    {
                        sortedBuilder.AppendLine(String.Format(CultureInfo.CurrentUICulture, "{0}: {1} ({2})", entry.Item1, entry.Item2, entry.Item3));
                        suggestionCounter += entry.Item3;
                    }
                    var displayForm = new StringDisplayForm(String.Format(CultureInfo.CurrentUICulture, "Romanization suggestions ({0} of {1})", suggestionCounter, romanizationSuggestionCount), sortedBuilder.ToString());
                    displayForm.Show();
                }
            }

            // show missing romanizations
            if ( romanizationMissing.Any() )
            {
                List<Tuple<String, Int32>> counter = new List<Tuple<String, Int32>>();
                foreach ( var entry in romanizationMissing )
                {
                    var found = counter.FirstOrDefault(x => x.Item1 == entry.Name);
                    if ( found == null )
                    {
                        counter.Add(Tuple.Create(entry.Name, 1));
                    }
                    else
                    {
                        counter.Remove(found);
                        counter.Add(Tuple.Create(entry.Name, found.Item2 + 1));
                    }
                }
                // counter.RemoveAll(x => x.Item2 < 2);
                if ( counter.Any() )
                {
                    counter.Sort(delegate(Tuple<String, Int32> p1, Tuple<String, Int32> p2)
                    {
                        var result = p2.Item2.CompareTo(p1.Item2);
                        if ( result == 0 )
                        {
                            result = p2.Item1.CompareTo(p1.Item1);
                        }
                        return result;
                    });

                    Int32 missingCounter = 0;
                    StringBuilder sortedBuilder = new StringBuilder();
                    foreach ( var entry in counter )
                    {
                        sortedBuilder.AppendLine(String.Format(CultureInfo.CurrentUICulture, "{0}: {1}", entry.Item1, entry.Item2));
                        missingCounter += entry.Item2;
                    }
                    var displayForm = new StringDisplayForm(String.Format(CultureInfo.CurrentUICulture, "Romanization missing ({0} of {1})", romanizationMissing.Count, numberOfEntities), sortedBuilder.ToString());
                    displayForm.Show();
                }
            }

            var allMuban = allEntities.Where(x => x.type == EntityType.Muban);
            StringBuilder mubanBuilder = new StringBuilder();
            Int32 mubanIssues = 0;
            var mubanNotStartingWithBan = allMuban.Where(x => !String.IsNullOrEmpty(x.english) && !x.english.StartsWith("Ban "));
            var mubanNotStartingWithBanInThai = allMuban.Where(x => !String.IsNullOrEmpty(x.name) && !x.name.StartsWith("บ้าน"));
            var mubanWithWrongCapitalization = allMuban.Where(x => !String.IsNullOrEmpty(x.english) && x.english.Split(' ').Any(y => y.Length > 0 && (Char.IsLower(y[0]) && !y.StartsWith("km"))));
            if ( mubanNotStartingWithBan.Any() )
            {
                mubanBuilder.AppendLine("Not starting with Ban:");
                foreach ( var entry in mubanNotStartingWithBan )
                {
                    mubanBuilder.AppendLine(String.Format(CultureInfo.CurrentUICulture, "{0}: {1} ({2})", entry.geocode, entry.english, entry.name));
                    mubanIssues++;
                }
                mubanBuilder.AppendLine();
            }
            if ( mubanNotStartingWithBanInThai.Any() )
            {
                mubanBuilder.AppendLine("Not starting with บ้าน:");
                foreach ( var entry in mubanNotStartingWithBanInThai )
                {
                    mubanBuilder.AppendLine(String.Format(CultureInfo.CurrentUICulture, "{0}: {1}", entry.geocode, entry.name));
                    mubanIssues++;
                }
                mubanBuilder.AppendLine();
            }
            if ( mubanWithWrongCapitalization.Any() )
            {
                mubanBuilder.AppendLine("Invalid lower-case letters in romanized name:");
                foreach ( var entry in mubanWithWrongCapitalization )
                {
                    mubanBuilder.AppendLine(String.Format(CultureInfo.CurrentUICulture, "{0}: {1}", entry.geocode, entry.english));
                    mubanIssues++;
                }
                mubanBuilder.AppendLine();
            }
            if ( mubanIssues > 0 )
            {
                var displayForm = new StringDisplayForm(String.Format(CultureInfo.CurrentUICulture, "Muban name issues ({0} of {1})", mubanIssues, allMuban.Count()), mubanBuilder.ToString());
                displayForm.Show();
            }
        }

        private void btnCheckTerms_Click(object sender, EventArgs e)
        {
            var itemsWithInvalidCouncilTerms = new List<Entity>();
            var itemsWithInvalidOfficialTerms = new List<Entity>();
            var itemsWithoutAnyCouncilTerms = new List<Entity>();
            var itemsWithUnexplainedCouncilSizeChanges = new List<Entity>();
            foreach ( var changwat in GlobalData.Provinces )
            {
                var itemsWithInvalidCouncilTermsInChangwat = EntitiesWithInvalidCouncilTerms(changwat.geocode);
                var itemsWithoutAnyCouncilTermsInChangwat = EntitiesWithoutAnyCouncilTerms(changwat.geocode);
                var itemsWithInvalidOfficialTermsInChangwat = EntitiesWithInvalidElectedOfficialsTerms(changwat.geocode);
                itemsWithInvalidCouncilTerms.AddRange(itemsWithInvalidCouncilTermsInChangwat);
                itemsWithInvalidOfficialTerms.AddRange(itemsWithInvalidOfficialTermsInChangwat);
                itemsWithoutAnyCouncilTerms.AddRange(itemsWithoutAnyCouncilTermsInChangwat);
                itemsWithUnexplainedCouncilSizeChanges.AddRange(GlobalData.GetGeocodeList(changwat.geocode).FlatList().Where(entity => entity.office.Any(office => office.council.CouncilTerms.Any(term => term.sizechangereason == CouncilSizeChangeReason.Unknown))));
            }
            var builder = new StringBuilder();
            Int32 count = 0;
            if ( itemsWithInvalidCouncilTerms.Any() )
            {
                builder.AppendLine("Council term problems:");
                foreach ( var item in itemsWithInvalidCouncilTerms )
                {
                    builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1})", item.english, item.geocode);
                    builder.AppendLine();
                    count++;
                }
                builder.AppendLine();
            }
            if ( itemsWithInvalidOfficialTerms.Any() )
            {
                builder.AppendLine("Official term problems:");
                foreach ( var item in itemsWithInvalidOfficialTerms )
                {
                    builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1})", item.english, item.geocode);
                    builder.AppendLine();
                    count++;
                }
                builder.AppendLine();
            }
            if ( itemsWithoutAnyCouncilTerms.Any() )
            {
                builder.AppendLine("No term at all:");
                foreach ( var item in itemsWithoutAnyCouncilTerms )
                {
                    builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1})", item.english, item.geocode);
                    builder.AppendLine();
                    count++;
                }
                builder.AppendLine();
            }
            if ( itemsWithUnexplainedCouncilSizeChanges.Any() )
            {
                builder.AppendLine("Unexplained council size changes:");
                foreach ( var item in itemsWithUnexplainedCouncilSizeChanges )
                {
                    builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1})", item.english, item.geocode);
                    builder.AppendLine();
                    count++;
                }
                builder.AppendLine();
            }

            if ( count > 0 )
            {
                var displayForm = new StringDisplayForm(String.Format(CultureInfo.CurrentUICulture, "Invalid terms ({0})", count), builder.ToString());
                displayForm.Show();
            }
        }

        private static IEnumerable<EntityTermEnd> EntitiesWithCouncilTermEndInYear(UInt32 changwatGeocode, Int32 year)
        {
            return EntitiesWithCouncilTermEndInTimeSpan(changwatGeocode, new DateTime(year, 1, 1), new DateTime(year, 12, 31));
        }

        private static IEnumerable<EntityTermEnd> EntitiesWithCouncilTermEndInTimeSpan(UInt32 changwatGeocode, DateTime begin, DateTime end)
        {
            Entity entity;
            if ( changwatGeocode == 0 )
            {
                entity = GlobalData.CompleteGeocodeList();
            }
            else
            {
                entity = GlobalData.GetGeocodeList(changwatGeocode);
            }
            return entity.EntitiesWithCouncilTermEndInTimeSpan(begin, end);
        }

        private static IEnumerable<Entity> EntitiesWithOfficialTermEndInYear(UInt32 changwatGeocode, Int32 year)
        {
            return EntitiesWithOfficialTermEndInTimeSpan(changwatGeocode, new DateTime(year, 1, 1), new DateTime(year, 12, 31));
        }

        private static IEnumerable<Entity> EntitiesWithOfficialTermEndInTimeSpan(UInt32 changwatGeocode, DateTime begin, DateTime end)
        {
            var result = new List<Entity>();
            Entity entity;
            if ( changwatGeocode == 0 )
            {
                entity = GlobalData.CompleteGeocodeList();
            }
            else
            {
                entity = GlobalData.GetGeocodeList(changwatGeocode);
            }
            foreach ( var item in entity.FlatList() )
            {
                foreach ( var office in item.office )
                {
                    office.officials.SortByDate();
                    var latestOfficial = office.officials.OfficialTerms.LastOrDefault();
                    if ( latestOfficial != null )
                    {
                        DateTime termEnd;
                        if ( latestOfficial.endSpecified )
                        {
                            termEnd = latestOfficial.end;
                        }
                        else
                        {
                            termEnd = latestOfficial.begin.AddYears(4).AddDays(-1);
                        }
                        if ( (termEnd.CompareTo(begin) >= 0) & (termEnd.CompareTo(end) <= 0) )
                        {
                            var addOfficial = true;
                            if ( latestOfficial.EndYear > 0 )
                            {
                                if ( (begin.Year > latestOfficial.EndYear) | (end.Year < latestOfficial.EndYear) )
                                {
                                    addOfficial = false;
                                }
                            }
                            if ( addOfficial )
                                result.Add(item);
                        }
                    }
                }
            }

            return result;
        }

        private static IEnumerable<Entity> EntitiesWithoutAnyCouncilTerms(UInt32 changwatGeocode)
        {
            var result = new List<Entity>();
            Entity entity;
            if ( changwatGeocode == 0 )
            {
                entity = GlobalData.CompleteGeocodeList();
            }
            else
            {
                entity = GlobalData.GetGeocodeList(changwatGeocode);
            }
            foreach ( var item in entity.FlatList() )
            {
                foreach ( var office in item.office )
                {
                    if ( office.type == OfficeType.MunicipalityOffice | office.type == OfficeType.TAOOffice | office.type == OfficeType.PAOOffice )
                    {
                        if ( !office.obsolete & !office.council.CouncilTerms.Any() )
                        {
                            result.Add(item);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets all entities within one province which have anything wrong with their council terms.
        /// </summary>
        /// <param name="changwatGeocode">Geocode of the province to check.</param>
        /// <returns>List of entities with invalid council terms.</returns>
        private static IEnumerable<Entity> EntitiesWithInvalidCouncilTerms(UInt32 changwatGeocode)
        {
            var result = new List<Entity>();
            Entity entity;
            if ( changwatGeocode == 0 )
            {
                entity = GlobalData.CompleteGeocodeList();
            }
            else
            {
                entity = GlobalData.GetGeocodeList(changwatGeocode);
            }
            foreach ( var item in entity.FlatList() )
            {
                Boolean hasInvalidTermData = false;
                foreach ( var office in item.office )
                {
                    office.council.SortByDate();

                    CouncilTerm lastTerm = null;
                    foreach ( var term in office.council.CouncilTerms )
                    {
                        hasInvalidTermData = hasInvalidTermData | !term.CouncilSizeValid | !term.TermLengthValid(term.type.TermLength()) | !term.TermDatesValid;
                        if ( lastTerm != null )
                        {
                            if ( lastTerm.endSpecified )
                            {
                                hasInvalidTermData = hasInvalidTermData | lastTerm.end.CompareTo(term.begin) > 0;
                            }
                            if ( (term.size != term.FinalSize) & (term.sizechangereason == CouncilSizeChangeReason.NoChange) )
                            {
                                hasInvalidTermData = true;
                            }
                            if ( (term.type == EntityType.PAO) & (lastTerm.size > 0) & (term.size > 0) )
                            {
                                if ( (term.size != term.FinalSize) & (term.sizechangereason == CouncilSizeChangeReason.NoChange) )
                                {
                                    hasInvalidTermData = hasInvalidTermData | lastTerm.FinalSize != term.size;
                                }
                            }

                            if ( (lastTerm.type == EntityType.TAO) & (term.type == EntityType.TAO) & (lastTerm.size > 0) & (term.size > 0) )
                            {
                                if ( (term.sizechangereason == CouncilSizeChangeReason.NoChange) )
                                {
                                    hasInvalidTermData = hasInvalidTermData | lastTerm.FinalSize != term.size;
                                }
                            }
                        }
                        lastTerm = term;
                    }
                }
                if ( hasInvalidTermData )
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets all entities within one province which have anything wrong with their elected officials.
        /// </summary>
        /// <param name="changwatGeocode">Geocode of the province to check.</param>
        /// <returns>List of entities with invalid council terms.</returns>
        private static IEnumerable<Entity> EntitiesWithInvalidElectedOfficialsTerms(UInt32 changwatGeocode)
        {
            var result = new List<Entity>();
            Entity entity;
            if ( changwatGeocode == 0 )
            {
                entity = GlobalData.CompleteGeocodeList();
            }
            else
            {
                entity = GlobalData.GetGeocodeList(changwatGeocode);
            }
            foreach ( var item in entity.FlatList() )
            {
                Boolean hasInvalidTermData = false;
                foreach ( var office in item.office )
                {
                    var electedOfficials = office.officials.OfficialTerms.Where(x => (x.title == OfficialType.TAOMayor | x.title == OfficialType.Mayor | x.title == OfficialType.PAOChairman)).ToList();
                    electedOfficials.RemoveAll(x => !x.beginSpecified);
                    electedOfficials.Sort((x, y) => x.begin.CompareTo(y.begin));

                    OfficialEntryBase lastOfficialTerm = null;
                    foreach ( var official in electedOfficials )
                    {
                        hasInvalidTermData = hasInvalidTermData | !official.TermLengthValid(4) | !official.TermDatesValid;
                        if ( lastOfficialTerm != null )
                        {
                            if ( lastOfficialTerm.endSpecified & official.beginSpecified )
                            {
                                hasInvalidTermData = hasInvalidTermData | lastOfficialTerm.end.CompareTo(official.begin) > 0;
                            }
                        }
                        lastOfficialTerm = official;
                    }
                }
                if ( hasInvalidTermData )
                {
                    result.Add(item);
                }
            }
            return result;
        }

        private Entity ActiveProvince()
        {
            var selectedItem = cbxChangwat.SelectedItem as Entity;
            return GlobalData.GetGeocodeList(selectedItem.geocode);
        }

        private void btnCreationStatistics_Click(object sender, EventArgs e)
        {
            var form = new CreationStatisticForm();
            form.Show();
        }

        private void btnGazetteLoadAll_Click(object sender, EventArgs e)
        {
            GlobalData.AllGazetteAnnouncements.entry.Clear();

            String searchPath = Path.Combine(Application.StartupPath, "gazette");
            if ( Directory.Exists(searchPath) )
            {
                foreach ( string fileName in Directory.GetFiles(searchPath, "Gazette*.XML") )
                {
                    using ( var filestream = new FileStream(fileName, FileMode.Open, FileAccess.Read) )
                    {
                        var currentGazetteList =
                            XmlManager.XmlToEntity<GazetteListFull>(filestream, new XmlSerializer(typeof(GazetteListFull)));
                        GlobalData.AllGazetteAnnouncements.entry.AddRange(currentGazetteList.AllGazetteEntries);
                    }
                }
                Boolean hasEntries = (GlobalData.AllGazetteAnnouncements.entry.Any());
                btn_GazetteShow.Enabled = hasEntries;
                btn_GazetteShowAll.Enabled = hasEntries;

                var duplicateMentionedGeocodes = new List<UInt32>();
                foreach ( var gazetteEntry in GlobalData.AllGazetteAnnouncements.AllGazetteEntries.Where(x => x.Items.Any(y => y is GazetteAreaDefinition)) )
                {
                    var geocodesInAnnouncement = new List<UInt32>();
                    foreach ( GazetteAreaDefinition areadefinition in gazetteEntry.Items.Where(y => y is GazetteAreaDefinition) )
                    {
                        if ( geocodesInAnnouncement.Contains(areadefinition.geocode) )
                        {
                            duplicateMentionedGeocodes.Add(areadefinition.geocode);
                        }
                        geocodesInAnnouncement.Add(areadefinition.geocode);
                        if ( areadefinition.subdivisionsSpecified && areadefinition.subdivisiontypeSpecified && (areadefinition.subdivisiontype != EntityType.Unknown) )
                        {
                            var geocode = areadefinition.geocode;
                            var entity = GlobalData.LookupGeocode(geocode);
                            while ( (entity != null) && (entity.IsObsolete) )
                            {
                                geocode = entity.newgeocode.FirstOrDefault();
                                if ( geocode == 0 )
                                {
                                    entity = null;
                                }
                                else
                                {
                                    entity = GlobalData.LookupGeocode(geocode);
                                }
                            }
                            if ( entity != null )
                            {
                                var entityCount = new EntityCount();
                                entityCount.year = gazetteEntry.publication.Year.ToString(CultureInfo.InvariantCulture);
                                entityCount.reference.Add(new GazetteRelated(gazetteEntry));
                                entityCount.entry.Add(new EntityCountType()
                                {
                                    count = Convert.ToUInt32(areadefinition.subdivisions),
                                    type = areadefinition.subdivisiontype,
                                }
                                );
                                entity.entitycount.Add(entityCount);
                            }
                        }
                    }
                }
                if ( duplicateMentionedGeocodes.Any() )
                {
                    new StringDisplayForm("Duplicate geocodes", String.Join<UInt32>(Environment.NewLine, duplicateMentionedGeocodes)).Show();
                }
            }
            else
            {
                MessageBox.Show(this, String.Format(CultureInfo.CurrentUICulture, "Fatal error: Directory {0} does not exist." + Environment.NewLine + "Application will be terminated.", searchPath), "Directory error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void btnTambonStatistics_Click(object sender, EventArgs e)
        {
            EntityCounter counter = new EntityCounter(new List<EntityType>() { EntityType.Tambon });
            // var lChangwat = (PopulationDataEntry)cbx_changwat.SelectedItem;
            // lCounter.BaseGeocode = lChangwat.Geocode;
            counter.Calculate();

            var form = new StringDisplayForm("Tambon", counter.CommonNames(20));
            form.Show();
        }

        private void btnMubanStatistics_Click(object sender, EventArgs e)
        {
            EntityCounter counter = new EntityCounter(new List<EntityType>() { EntityType.Muban });
            counter.Calculate();
            var formNames = new StringDisplayForm("Muban", counter.CommonNames(20));
            formNames.Show();
        }

        private void btnChumchonStatistics_Click(object sender, EventArgs e)
        {
            EntityCounter counter = new EntityCounter(new List<EntityType>() { EntityType.Chumchon });
            counter.Calculate();
            var formNames = new StringDisplayForm("Chumchon", counter.CommonNames(20));
            formNames.Show();
        }

        private void btnTermEnds_Click(object sender, EventArgs e)
        {
            var itemsWithTermEnds = new List<EntityTermEnd>();
            var geocode = (cbxChangwat.SelectedItem as Entity).geocode;
            if ( chkAllProvince.Checked )
            {
                geocode = 0;
            }
            var itemWithCouncilTermEndsInChangwat = EntitiesWithCouncilTermEndInYear(geocode, DateTime.Now.Year);
            // var itemWithOfficialTermEndsInChangwat = EntitiesWithCouncilTermEndInYear(geocode, DateTime.Now.Year);
            itemsWithTermEnds.AddRange(itemWithCouncilTermEndsInChangwat);
            itemsWithTermEnds.Sort((x, y) => x.CouncilTerm.begin.CompareTo(y.CouncilTerm.begin));

            var builder = new StringBuilder();
            Int32 count = 0;
            foreach ( var item in itemsWithTermEnds )
            {
                builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2:d}", item.Entity.english, item.Entity.geocode, item.CouncilTerm.begin.AddYears(4).AddDays(-1));
                builder.AppendLine();
                count++;
            }
            if ( count > 0 )
            {
                var displayForm = new StringDisplayForm(String.Format(CultureInfo.CurrentUICulture, "Term ends ({0})", count), builder.ToString());
                displayForm.Show();
            }
        }

        private void btnTimeBetweenElection_Click(object sender, EventArgs e)
        {
            var counter = new FrequencyCounter();
            foreach ( var changwat in GlobalData.Provinces )
            {
                CalculateTimeBetweenLocalElection(changwat.geocode, counter);
            }
            var builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Number of interregnums: {0}", counter.NumberOfValues);
            builder.AppendLine();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Minimum days between elections: {0}", counter.MinValue);
            builder.AppendLine();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Maximum days between elections: {0}", counter.MaxValue);
            builder.AppendLine();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Mean number of days between elections: {0:#0.0}", counter.MeanValue);
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("Offices with longest interregnum:");
            foreach ( var geocode in counter.Data[counter.MaxValue] )
            {
                var entity = GlobalData.LookupGeocode(geocode);
                builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2} days", entity.english, geocode, counter.MaxValue);
                builder.AppendLine();
            }
            builder.AppendLine();
            builder.AppendLine("Offices with shortest interregnum:");
            foreach ( var geocode in counter.Data[counter.MinValue] )
            {
                var entity = GlobalData.LookupGeocode(geocode);
                builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2} days", entity.english, geocode, counter.MinValue);
                builder.AppendLine();
            }

            var result = builder.ToString();
            var formInterregnum = new StringDisplayForm("Time between elections", result);
            formInterregnum.Show();
        }

        private static void CalculateTimeBetweenLocalElection(UInt32 geocode, FrequencyCounter counter)
        {
            var fullChangwat = GlobalData.GetGeocodeList(geocode);
            foreach ( var item in fullChangwat.FlatList() )
            {
                foreach ( var office in item.office )
                {
                    office.council.SortByDate();

                    CouncilTerm lastTerm = null;
                    foreach ( var term in office.council.CouncilTerms )
                    {
                        if ( lastTerm != null )
                        {
                            if ( lastTerm.endSpecified )
                            {
                                var interregnum = term.begin - lastTerm.end;
                                counter.IncrementForCount(interregnum.Days, item.geocode);
                            }
                        }
                        lastTerm = term;
                    }
                }
            }
        }

        private static void CountCouncilElectionWeekday(UInt32 geocode, FrequencyCounter counter)
        {
            var fullChangwat = GlobalData.GetGeocodeList(geocode);
            foreach ( var item in fullChangwat.FlatList() )
            {
                foreach ( var office in item.office )
                {
                    foreach ( var term in office.council.CouncilTerms )
                    {
                        counter.IncrementForCount((Int32)term.begin.DayOfWeek, item.geocode);
                    }
                }
            }
        }

        private static void CountCouncilElectionDate(UInt32 geocode, FrequencyCounter counter)
        {
            var zeroDate = new DateTime(2000, 1, 1);
            var fullChangwat = GlobalData.GetGeocodeList(geocode);
            foreach ( var item in fullChangwat.FlatList() )
            {
                foreach ( var office in item.office )
                {
                    foreach ( var term in office.council.CouncilTerms )
                    {
                        var span = term.begin - zeroDate;
                        counter.IncrementForCount(span.Days, item.geocode);
                    }
                }
            }
        }

        private static void CountNayokElectionDate(UInt32 geocode, FrequencyCounter counter)
        {
            var zeroDate = new DateTime(2000, 1, 1);
            var fullChangwat = GlobalData.GetGeocodeList(geocode);
            foreach ( var item in fullChangwat.FlatList() )
            {
                foreach ( var office in item.office )
                {
                    if ( office.officials != null )
                    {
                        foreach ( var officialTerm in office.officials.OfficialTerms )
                        {
                            if ( officialTerm.beginreason == OfficialBeginType.ElectedDirectly )
                            {
                                var span = officialTerm.begin - zeroDate;
                                counter.IncrementForCount(span.Days, item.geocode);
                            }
                        }
                    }
                }
            }
        }

        private void btnElectionWeekday_Click(object sender, EventArgs e)
        {
            var counter = new FrequencyCounter();
            foreach ( var changwat in GlobalData.Provinces )
            {
                CountCouncilElectionWeekday(changwat.geocode, counter);
            }
            var builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Number of elections: {0}", counter.NumberOfValues);
            builder.AppendLine();
            DayOfWeek leastFrequentDay = DayOfWeek.Sunday;
            Int32 leastFrequentDayCount = Int32.MaxValue;
            foreach ( var dataEntry in counter.Data )
            {
                var count = dataEntry.Value.Count();
                var day = (DayOfWeek)(dataEntry.Key);
                builder.AppendFormat(CultureInfo.CurrentUICulture, "{0}: {1} ({2:#0.0%})", day, count, (Double)count / counter.NumberOfValues);
                if ( count < leastFrequentDayCount )
                {
                    leastFrequentDayCount = count;
                    leastFrequentDay = day;
                }
                builder.AppendLine();
            }

            builder.AppendFormat(CultureInfo.CurrentUICulture, "Elections on {0} at ", leastFrequentDay);
            foreach ( var value in counter.Data[(Int32)leastFrequentDay] )
            {
                builder.AppendFormat(CultureInfo.CurrentUICulture, "{0},", value);
            }
            builder.AppendLine();

            var result = builder.ToString();

            var formElectionDayOfWeek = new StringDisplayForm("Election days", result);
            formElectionDayOfWeek.Show();
        }

        private void btnMubanHelper_Click(object sender, EventArgs e)
        {
            var country = new Entity();
            foreach ( var province in GlobalData.Provinces )
            {
                var provinceData = GlobalData.GetGeocodeList(province.geocode);
                country.entity.Add(provinceData);
            }
            var allEntities = country.FlatList();

            var romanizator = new Romanization();
            romanizator.Initialize(allEntities);
            var form = new MubanHelperForm();
            form.Romanizator = romanizator;
            form.Show();
        }

        private void btnPendingElections_Click(object sender, EventArgs e)
        {
            var itemsWithCouncilElectionsPending = new List<EntityTermEnd>();
            var itemsWithOfficialElectionsPending = new List<EntityTermEnd>();
            var itemsWithOfficialElectionResultUnknown = new List<EntityTermEnd>();

            Entity entity;
            if ( chkAllProvince.Checked )
            {
                entity = GlobalData.CompleteGeocodeList();
            }
            else
            {
                var changwatGeocode = (cbxChangwat.SelectedItem as Entity).geocode;
                entity = GlobalData.GetGeocodeList(changwatGeocode);
            }
            var itemsWithCouncilElectionPendingInChangwat = entity.EntitiesWithCouncilElectionPending();
            itemsWithCouncilElectionsPending.AddRange(itemsWithCouncilElectionPendingInChangwat);
            itemsWithCouncilElectionsPending.Sort((x, y) => x.CouncilTerm.begin.CompareTo(y.CouncilTerm.begin));

            var itemsWithOfficialElectionPendingInChangwat = entity.EntitiesWithOfficialElectionPending();
            itemsWithOfficialElectionsPending.AddRange(itemsWithOfficialElectionPendingInChangwat);
            itemsWithOfficialElectionsPending.Sort((x, y) => x.OfficialTerm.begin.CompareTo(y.OfficialTerm.begin));

            var itemsWithOfficialElectionResultUnknownInChangwat = entity.EntitiesWithLatestOfficialElectionResultUnknown();
            itemsWithOfficialElectionResultUnknown.AddRange(itemsWithOfficialElectionResultUnknownInChangwat);
            itemsWithOfficialElectionResultUnknown.Sort((x, y) => x.OfficialTerm.begin.CompareTo(y.OfficialTerm.begin));

            var councilBuilder = new StringBuilder();
            Int32 councilCount = 0;
            foreach ( var item in itemsWithCouncilElectionsPending )
            {
                DateTime end;
                if ( item.CouncilTerm.endSpecified )
                {
                    end = item.CouncilTerm.end;
                }
                else
                {
                    end = item.CouncilTerm.begin.AddYears(4).AddDays(-1);
                }
                councilBuilder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2:d}", item.Entity.english, item.Entity.geocode, end);
                councilBuilder.AppendLine();
                councilCount++;
            }
            if ( councilCount > 0 )
            {
                var displayForm = new StringDisplayForm(String.Format(CultureInfo.CurrentUICulture, "{0} LAO council elections pending", councilCount), councilBuilder.ToString());
                displayForm.Show();
            }

            var officialBuilder = new StringBuilder();
            Int32 officialCount = 0;
            foreach ( var item in itemsWithOfficialElectionsPending )
            {
                String officialTermEnd = "unknown";
                if ( (item.OfficialTerm.begin != null) && (item.OfficialTerm.begin.Year > 1900) )
                {
                    DateTime end;
                    if ( item.OfficialTerm.endSpecified )
                    {
                        end = item.OfficialTerm.end;
                    }
                    else
                    {
                        end = item.OfficialTerm.begin.AddYears(4).AddDays(-1);
                    }
                    officialTermEnd = String.Format(CultureInfo.CurrentUICulture, "{0:d}", end);
                }
                officialBuilder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2}", item.Entity.english, item.Entity.geocode, officialTermEnd);
                officialBuilder.AppendLine();
                officialCount++;
            }
            if ( officialCount > 0 )
            {
                var displayForm = new StringDisplayForm(String.Format(CultureInfo.CurrentUICulture, "{0} LAO official elections pending", officialCount), officialBuilder.ToString());
                displayForm.Show();
            }

            var officialUnknownBuilder = new StringBuilder();
            Int32 officialUnknownCount = 0;
            foreach ( var item in itemsWithOfficialElectionResultUnknown )
            {
                if ( (item.OfficialTerm.begin != null) && (item.OfficialTerm.begin.Year > 1900) )  // must be always true
                {
                    officialUnknownBuilder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2:d}", item.Entity.english, item.Entity.geocode, item.OfficialTerm.begin);
                    officialUnknownBuilder.AppendLine();
                    officialUnknownCount++;
                }
            }
            if ( officialUnknownCount > 0 )
            {
                var displayForm = new StringDisplayForm(String.Format(CultureInfo.CurrentUICulture, "{0} LAO official elections result missing", officialUnknownCount), officialUnknownBuilder.ToString());
                displayForm.Show();
            }
        }

        private void btnCreateKml_Click(object sender, EventArgs e)
        {
            // TODO!
        }

        private void btnWikiData_Click(object sender, EventArgs e)
        {
            new WikiData().Show();
        }

        private void btnElectionDates_Click(object sender, EventArgs e)
        {
            var counter = new FrequencyCounter();
            foreach ( var changwat in GlobalData.Provinces )
            {
                CountCouncilElectionDate(changwat.geocode, counter);
            }
            var builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Number of elections: {0}", counter.NumberOfValues);
            builder.AppendLine();

            var zeroDate = new DateTime(2000, 1, 1);
            var ordered = counter.Data.OrderBy(x => x.Key);
            foreach ( var entry in ordered )
            {
                var count = entry.Value.Count();
                if ( count > 0 )
                {
                    builder.AppendFormat(CultureInfo.CurrentUICulture, "{0:yyyy-MM-dd}: {1}", zeroDate.AddDays(entry.Key), entry.Value.Count());
                    builder.AppendLine();
                }
            }
            builder.AppendLine();

            var result = builder.ToString();

            var formElectionDayOfWeek = new StringDisplayForm("Election dates", result);
            formElectionDayOfWeek.Show();
        }

        private void btn_Population_Click(Object sender, EventArgs e)
        {
            var downloader = new PopulationDataDownloader(Convert.ToInt32(edtYear.Value), 0);
            // var downloader = new PopulationDataDownloader(Convert.ToInt32(edtYear.Value), 13);
            downloader.Process();
            var output = XmlManager.EntityToXml<Entity>(downloader.Data);
            File.WriteAllText(Path.Combine(PopulationDataDownloader.OutputDirectory, edtYear.Value.ToString() + ".xml"), output);
        }

        private void btnNayokResign_Click(Object sender, EventArgs e)
        {
            UInt32 changwatGeocode = 0;
            if ( !chkAllProvince.Checked )
            {
                changwatGeocode = (cbxChangwat.SelectedItem as Entity).geocode;
            }

            var allTermEnd = EntitiesWithCouncilTermEndInTimeSpan(changwatGeocode, new DateTime(2013, 9, 1), new DateTime(2013, 9, 30));
            var allNextElectionNormal = allTermEnd.Where(x => x.Entity.office.First().council.CouncilTerms.Any(y => y.begin > x.CouncilTerm.end && (y.begin - x.CouncilTerm.end).Days < 60)).ToList();
            List<EntityTermEnd> processedTermEnds = new List<EntityTermEnd>();
            foreach ( var entry in allTermEnd )
            {
                entry.Entity.office.First().officials.SortByDate();
                processedTermEnds.Add(new EntityTermEnd(
                    entry.Entity,
                    entry.CouncilTerm,
                    entry.Entity.office.First().officials.OfficialTerms.FirstOrDefault(y => y.begin == entry.CouncilTerm.begin)));
            }

            var nayokTermStartedSameDate = processedTermEnds.Where(x => x.OfficialTerm != null).ToList();
            var nextElectionNormal = nayokTermStartedSameDate.Where(x => x.Entity.office.First().council.CouncilTerms.Any(y => y.begin > x.CouncilTerm.end && (y.begin - x.CouncilTerm.end).Days < 60)).ToList();
            var nayokTermEndNormal = nextElectionNormal.Where(x => x.OfficialTerm.end == x.CouncilTerm.end).ToList();
            var nayokTermEndUnknown = nextElectionNormal.Where(x => x.OfficialTerm.end != x.CouncilTerm.end).ToList();
            List<EntityTermEnd> nextNayokElectionEarly = new List<EntityTermEnd>();
            List<Entity> nayokElectedAgainEarly = new List<Entity>();
            List<Entity> nayokElectionFailEarly = new List<Entity>();
            List<Entity> nayokElectedAgainNormal = new List<Entity>();
            List<Entity> nayokElectionFailNormal = new List<Entity>();
            foreach ( var entry in nayokTermEndUnknown )
            {
                var nextCouncilTerm = entry.Entity.office.First().council.CouncilTerms.First(y => y.begin > entry.CouncilTerm.end && (y.begin - entry.CouncilTerm.end).Days < 60);
                var nextNayokTerm = entry.Entity.office.First().officials.OfficialTerms.LastOrDefault(y => (y.begin < nextCouncilTerm.begin));
                if ( (nextNayokTerm != null) && (nextNayokTerm.begin.Year == entry.CouncilTerm.end.Year) )
                {
                    nextNayokElectionEarly.Add(new EntityTermEnd(entry.Entity, nextCouncilTerm, nextNayokTerm));
                    var nextOfficial = nextNayokTerm as OfficialEntry;
                    var previousOfficial = entry.OfficialTerm as OfficialEntry;
                    if ( (nextOfficial != null) && (previousOfficial != null) )
                    {
                        if ( nextOfficial.name == previousOfficial.name )
                        {
                            nayokElectedAgainEarly.Add(entry.Entity);
                        }
                        else
                        {
                            nayokElectionFailEarly.Add(entry.Entity);
                        }
                    }
                }
            }
            foreach ( var entry in nayokTermEndNormal )
            {
                var nextCouncilTerm = entry.Entity.office.First().council.CouncilTerms.First(y => y.begin > entry.CouncilTerm.end && (y.begin - entry.CouncilTerm.end).Days < 60);
                var nextOfficial = entry.Entity.office.First().officials.OfficialTerms.LastOrDefault(y => (y.begin == nextCouncilTerm.begin)) as OfficialEntry;
                var previousOfficial = entry.OfficialTerm as OfficialEntry;
                if ( (nextOfficial != null) && (previousOfficial != null) )
                {
                    if ( nextOfficial.name == previousOfficial.name )
                    {
                        nayokElectedAgainNormal.Add(entry.Entity);
                    }
                    else
                    {
                        nayokElectionFailNormal.Add(entry.Entity);
                    }
                }
            }

            var builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Number of council term ends: {0}", allTermEnd.Count());
            builder.AppendLine();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Number of council term elections: {0}", allNextElectionNormal.Count());
            builder.AppendLine();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Number of term started together: {0}", nayokTermStartedSameDate.Count);
            builder.AppendLine();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Number of term into consideration: {0}", nextElectionNormal.Count);
            builder.AppendLine();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Number of Nayok till end of term: {0}", nayokTermEndNormal.Count);
            builder.AppendLine();
            var success = Convert.ToDouble(nayokElectedAgainNormal.Count());
            var fail = Convert.ToDouble(nayokElectionFailNormal.Count());
            builder.AppendFormat(CultureInfo.CurrentUICulture, "   {0} reelected, {1} changed; {2:P} success rate", success, fail, success / (success + fail));
            builder.AppendLine();
            builder.AppendFormat(CultureInfo.CurrentUICulture, "Number of Nayok early election: {0}", nextNayokElectionEarly.Count);
            builder.AppendLine();
            success = Convert.ToDouble(nayokElectedAgainEarly.Count());
            fail = Convert.ToDouble(nayokElectionFailEarly.Count());
            builder.AppendFormat(CultureInfo.CurrentUICulture, "   {0} reelected, {1} changed; {2:P} success rate", success, fail, success / (success + fail));
            builder.AppendLine();
            builder.AppendLine();
            if ( chkAllProvince.Checked )
            {
                foreach ( var changwat in GlobalData.Provinces )
                {
                    var nayokEndTermInChangwat = nayokTermEndNormal.Where(x => GeocodeHelper.ProvinceCode(x.Entity.geocode) == changwat.geocode).Count();
                    var nayokEarlyInChangwat = nextNayokElectionEarly.Where(x => GeocodeHelper.ProvinceCode(x.Entity.geocode) == changwat.geocode).Count();
                    var total = nayokEarlyInChangwat + nayokEndTermInChangwat;
                    if ( total > 0 )
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, "{0}: {1} of {2} normal, {3:P} early", changwat.english, nayokEndTermInChangwat, total, Convert.ToDouble(nayokEarlyInChangwat) / total);
                        builder.AppendLine();
                    }
                }
            }

            var result = builder.ToString();

            var formElectionDayOfWeek = new StringDisplayForm("Nayok info", result);
            formElectionDayOfWeek.Show();
        }

        private void btnShowEntityData_Click(Object sender, EventArgs e)
        {
            var formEntityBrowser = new EntityBrowserForm();
            formEntityBrowser.StartChangwatGeocode = (cbxChangwat.SelectedItem as Entity).geocode;
            formEntityBrowser.PopulationReferenceYear = Convert.ToInt16(edtYear.Value);
            formEntityBrowser.PopulationDataSource = PopulationDataSourceType.DOPA;
            Boolean checkWikiData = false;
            try
            {
                var checkWikidataConfig = ConfigurationManager.AppSettings["EntityBrowserCheckWikiData"];
                checkWikiData = Convert.ToBoolean(checkWikidataConfig);
            }
            catch ( FormatException )
            {
            }
            formEntityBrowser.CheckWikiData = checkWikiData;

            formEntityBrowser.Show();
        }

        private void btnCheckCensus_Click(Object sender, EventArgs e)
        {
            Int16 year = Convert.ToInt16(cbxCensusYears.SelectedItem as String);
            var builder = new StringBuilder();
            var baseEntity = GlobalData.CompleteGeocodeList();
            var type = PopulationDataSourceType.Census;
            // var type = PopulationDataSourceType.DOPA;
            // year = 2015;
            var populationData = GlobalData.LoadPopulationDataUnprocessed(type, year);
            var allEntities = populationData.FlatList().Where(x => x.population.Any(y => y.source == type && y.Year == year)).ToList();
            foreach ( var entity in allEntities )
            {
                var population = entity.population.First(y => y.source == type && y.Year == year);
                Int32 diff = 0;

                var notDistinctPopulationDataTypes =
                    from list in population.data
                    group list by list.type into grouped
                    where grouped.Count() > 1
                    select grouped;

                foreach ( var notDistinctType in notDistinctPopulationDataTypes )
                {
                    var codes = notDistinctType.Select(x => String.Join(",", x.geocode));
                    if ( codes.Distinct().Count() != codes.Count() )
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2} present {3} times)", entity.english, entity.geocode, notDistinctType.Key, notDistinctType.Count());
                        builder.AppendLine();
                    }
                }

                foreach ( var data in population.data )
                {
                    if ( data.male != 0 && data.female != 0 )
                    {
                        diff = Math.Abs(data.total - data.male - data.female);
                        if ( diff > 1 )
                        {
                            builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2} differs male/female to total by {3}", entity.english, entity.geocode, data.type, diff);
                            builder.AppendLine();
                        }
                    }
                    foreach ( var subData in data.data )
                    {
                        if ( data.male != 0 && data.female != 0 )
                        {
                            diff = Math.Abs(data.total - data.male - data.female);
                            if ( diff > 1 )
                            {
                                builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2} of {3} differs male/female to total by {4}", entity.english, entity.geocode, subData.type, data.type, diff);
                                builder.AppendLine();
                            }
                        }
                    }
                    if ( data.geocode.Any() )
                    {
                        switch ( data.type )
                        {
                            case PopulationDataType.ruralsanitary:
                                if ( (data.total > 4999 * data.geocode.Count) )
                                {
                                    builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): Rural sanitary population {2} with just {3} sanitary district(s).", entity.english, entity.geocode, data.total, data.geocode.Count);
                                    builder.AppendLine();
                                }
                                break;
                            case PopulationDataType.urbansanitary:
                                if ( (data.total < 5000 * data.geocode.Count) )
                                {
                                    builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): Urban sanitary population {2} not possible with {3} sanitary district(s).", entity.english, entity.geocode, data.total, data.geocode.Count);
                                    builder.AppendLine();
                                }
                                break;
                        }
                    }
                    if ( !data.AgeTableValid() )
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): {2} age table does not sum up", entity.english, entity.geocode, data.type);
                        builder.AppendLine();
                    }
                }
                diff = population.SumError();
                if ( diff > 1 )
                {
                    builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): Sum of parts differs by {2}", entity.english, entity.geocode, diff);
                    builder.AppendLine();
                }
                var sum = new PopulationData();
                foreach ( var subEntity in entity.entity.Where(x => !x.IsObsolete && x.population.Any()) )
                {
                    var populationToAdd = subEntity.population.FirstOrDefault(y => y.source == type && y.Year == year);
                    if ( populationToAdd != null )
                    {
                        foreach ( var dataPoint in populationToAdd.data )
                        {
                            sum.AddDataPoint(dataPoint);
                        }
                    }
                }
                if ( sum.data.Any() )
                {
                    diff = sum.TotalPopulation.MaxDeviation(entity.population.First(y => y.source == type && y.Year == year).TotalPopulation);
                    if ( diff > 1 )
                    {
                        builder.AppendFormat(CultureInfo.CurrentUICulture, "{0} ({1}): Sum of sub-entities differs by {2}", entity.english, entity.geocode, diff);
                        builder.AppendLine();
                    }
                }
            }
            var result = builder.ToString();

            var formCensusProblems = new StringDisplayForm("Census data problems", result);
            formCensusProblems.Show();
        }

        private void btnAmphoeList_Click(Object sender, EventArgs e)
        {
            Int16 year = Convert.ToInt16(cbxCensusYears.SelectedItem as String);
            GlobalData.LoadPopulationData(PopulationDataSourceType.Census, year);
            var baseEntity = GlobalData.CompleteGeocodeList();
            var allAmphoe = baseEntity.FlatList().Where(x => x.type.IsCompatibleEntityType(EntityType.Amphoe) && !x.IsObsolete).ToList();
            var builder = new StringBuilder();
            foreach ( var entity in allAmphoe )
            {
                var location = String.Empty;
                var point = entity.office.First().Point;
                if ( point != null )
                {
                    location = String.Format(CultureInfo.InvariantCulture, "{0} N {1} E", point.lat, point.@long);
                }
                Int32 population = 0;
                var populationData = entity.population.FirstOrDefault(x => x.Year == year && x.source == PopulationDataSourceType.Census);
                if ( populationData != null )
                {
                    population = populationData.TotalPopulation.total;
                }
                builder.AppendFormat(CultureInfo.CurrentUICulture, "{0},{1},{2},{3},{4}", entity.english, entity.name, entity.geocode, population, location);
                builder.AppendLine();
            }
            Clipboard.Clear();
            Clipboard.SetText(builder.ToString());
        }

        private void btnGovernor_Click(Object sender, EventArgs e)
        {
            var baseEntity = GlobalData.CompleteGeocodeList();
            var allChangwat = baseEntity.FlatList().Where(x => x.type == EntityType.Changwat && !x.IsObsolete).ToList();
            List<Entity> vacantChangwat = new List<Entity>();
            var frequency = new FrequencyCounter();
            foreach ( var changwat in allChangwat )
            {
                var officials = changwat.office.First(y => y.type == OfficeType.ProvinceHall).officials.OfficialTerms;
                var official = officials.First();
                if ( !official.InOffice() )
                {
                    vacantChangwat.Add(changwat);
                }
                else
                {
                    var yearsInOffice = DateTime.Now.Year - officials.First().begin.Year;
                    frequency.IncrementForCount(yearsInOffice, changwat.geocode);
                }
            }
            var builder = new StringBuilder();

            builder.AppendLine(String.Format(CultureInfo.CurrentUICulture, "Most common number of years in office: {0}", frequency.MostCommonValue));
            builder.AppendLine(String.Format(CultureInfo.CurrentUICulture, "Mean number of years in office: {0:F2}", frequency.MeanValue));
            var longestTermChangwat = frequency.Data[frequency.MaxValue].Select(x => allChangwat.First(y => y.geocode == x)).Select(x => x.english);
            builder.AppendLine(String.Format(CultureInfo.CurrentUICulture, "Longest time in office: {0} years (in {1})", frequency.MaxValue, String.Join(", ", longestTermChangwat)));
            builder.AppendLine();
            if ( vacantChangwat.Any() )
            {
                builder.Append(String.Join(Environment.NewLine, vacantChangwat.Select(x => x.english)));
            }
            var formGovernors = new StringDisplayForm("Governors", builder.ToString());
            formGovernors.Show();
        }

        private void btnAgeTable_Click(Object sender, EventArgs e)
        {
            var baseEntity = ActiveProvince();
            var entity = new Entity();
            entity.CopyBasicDataFrom(baseEntity);
            var code = entity.geocode;
            Int16 year = Convert.ToInt16(edtYear.Value);
            var path = Path.Combine(_ageDataDirectory, String.Format(CultureInfo.InvariantCulture, "{0:00}12cc{1:00}.txt", year - 2000 + 43, code));
            if ( File.Exists(path) )
            {
                String input = File.ReadAllText(path);
                using ( StringReader reader = new StringReader(input) )
                {
                    String line = reader.ReadLine();
                    var fields = line.Split('|');
                    var populationData = new PopulationData();
                    populationData.year = year.ToString(CultureInfo.InvariantCulture);
                    populationData.source = PopulationDataSourceType.DOPA;
                    populationData.referencedate = new DateTime(year, 12, 31);
                    var dataEntry = new HouseholdDataPoint();
                    dataEntry.type = PopulationDataType.total;
                    var ageTable = new AgeTable();
                    dataEntry.agetable = ageTable;
                    populationData.data.Add(dataEntry);
                    entity.population.Add(populationData);

                    for ( UInt32 age = 0 ; age <= 101 ; age++ )
                    {
                        var ageEntry = new AgeTableEntry();
                        ageEntry.begin = age;
                        if ( age > 100 )
                        {
                            ageEntry.end = 120;
                        }
                        else
                        {
                            ageEntry.end = age;
                        }
                        ageEntry.male = Convert.ToInt32(fields[age * 2 + 1]);
                        ageEntry.female = Convert.ToInt32(fields[age * 2 + 2]);
                        ageEntry.total = ageEntry.male + ageEntry.female;
                        ageTable.age.Add(ageEntry);
                    }
                    ageTable.unknown = new PopulationDataPoint();
                    ageTable.unknown.male = Convert.ToInt32(fields[205]);
                    ageTable.unknown.female = Convert.ToInt32(fields[206]);
                    ageTable.unknown.total = Convert.ToInt32(fields[207]);
                    dataEntry.houseregister = new PopulationDataPoint();
                    dataEntry.houseregister.male = Convert.ToInt32(fields[208]);
                    dataEntry.houseregister.female = Convert.ToInt32(fields[209]);
                    dataEntry.houseregister.total = Convert.ToInt32(fields[210]);
                    dataEntry.foreigner = new PopulationDataPoint();
                    dataEntry.foreigner.male = Convert.ToInt32(fields[211]);
                    dataEntry.foreigner.female = Convert.ToInt32(fields[212]);
                    dataEntry.foreigner.total = Convert.ToInt32(fields[213]);
                    dataEntry.moving = new PopulationDataPoint();
                    dataEntry.moving.male = Convert.ToInt32(fields[214]);
                    dataEntry.moving.female = Convert.ToInt32(fields[215]);
                    dataEntry.moving.total = Convert.ToInt32(fields[216]);
                    dataEntry.male = Convert.ToInt32(fields[217]);
                    dataEntry.female = Convert.ToInt32(fields[218]);
                    dataEntry.total = Convert.ToInt32(fields[219]);

                    var output = XmlManager.EntityToXml<Entity>(entity);
                    File.WriteAllText(Path.Combine(PopulationDataDownloader.OutputDirectory, String.Format(CultureInfo.InvariantCulture, "age{0} {1}.xml", year, entity.english)), output);
                }
            }
        }

        private void btnNumbers_Click(Object sender, EventArgs e)
        {
            var formEntityNumbers = new EntityNumbersForm();
            formEntityNumbers.Show();
        }

        private void btnMubanList_Click(Object sender, EventArgs e)
        {
            var baseEntity = GlobalData.CompleteGeocodeList();
            baseEntity.PropagateObsoleteToSubEntities();
            var allMuban = baseEntity.FlatList().Where(x => x.type.IsCompatibleEntityType(EntityType.Muban) && !x.IsObsolete).ToList();
            var builder = new StringBuilder();
            foreach ( var entity in allMuban )
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0},{1},{2}", entity.geocode, entity.english, entity.name);
                builder.AppendLine();
            }
            Clipboard.Clear();
            Clipboard.SetText(builder.ToString());
        }

        private void btnTambonList_Click(Object sender, EventArgs e)
        {
            Int16 year = Convert.ToInt16(edtYear.Value);
            GlobalData.LoadPopulationData(PopulationDataSourceType.DOPA, year);
            var baseEntity = GlobalData.CompleteGeocodeList();
            baseEntity.PropagateObsoleteToSubEntities();
            var allTambon = baseEntity.FlatList().Where(x => x.type.IsCompatibleEntityType(EntityType.Tambon) && !x.IsObsolete).ToList();
            var builder = new StringBuilder();
            foreach ( var entity in allTambon )
            {
                Int32 population = 0;
                Int32 households = 0;
                var populationData = entity.population.FirstOrDefault(x => x.Year == year && x.source == PopulationDataSourceType.DOPA);
                if ( populationData != null )
                {
                    population = populationData.TotalPopulation.total;
                    var householdData = populationData.TotalPopulation as HouseholdDataPoint;
                    if ( householdData != null )
                    {
                        households = householdData.households;
                    }
                }
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4}", entity.geocode, entity.english, entity.name, population, households);
                builder.AppendLine();
            }
            Clipboard.Clear();
            Clipboard.SetText(builder.ToString());
        }

        private void btnDisambiguation_Click(Object sender, EventArgs e)
        {
            new DisambiguationForm().Show();
        }

        private void btnPopulationTable_Click(Object sender, EventArgs e)
        {
            Int16 year = Convert.ToInt16(edtYear.Value);
            GlobalData.LoadPopulationData(PopulationDataSourceType.DOPA, year);
            var baseEntity = GlobalData.CompleteGeocodeList();
            var data = baseEntity.entity.Select(x => new Tuple<UInt32, PopulationData>(x.geocode, x.population.FirstOrDefault(y => y.Year == year && y.source == PopulationDataSourceType.DOPA))).OrderBy(x => x.Item1);
            var builder = new StringBuilder();
            foreach ( var item in data )
            {
                if ( item.Item2 != null )
                {
                    builder.AppendFormat(CultureInfo.InvariantCulture, "{1}", item.Item1, item.Item2.TotalPopulation.total);
                }
                builder.AppendLine();
            }
            Clipboard.Clear();
            Clipboard.SetText(builder.ToString());
        }

        private void btnShowPopulation_Click(Object sender, EventArgs e)
        {
            var formPopulationByEntityTypeViewer = new PopulationByEntityTypeViewer();
            formPopulationByEntityTypeViewer.PopulationReferenceYear = Convert.ToInt16(edtYear.Value);
            formPopulationByEntityTypeViewer.PopulationDataSource = PopulationDataSourceType.DOPA;
            formPopulationByEntityTypeViewer.Show();
        }
    }
}