using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadBasicGeocodeList();

            FillChangwatDropDown();
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

        private static void LoadBasicGeocodeList()
        {
            var fileName = GlobalData.BaseXMLDirectory + "\\geocode.xml";
            using ( var filestream = new FileStream(fileName, FileMode.Open, FileAccess.Read) )
            {
                Entity geocodes = XmlManager.XmlToEntity<Entity>(filestream, new XmlSerializer(typeof(Entity)));
                var provinces = new List<Entity>();
                foreach ( var entity in geocodes.entity.Where(x => x.type.IsFirstLevelAdministrativeUnit() && !x.IsObsolete) )
                {
                    provinces.Add(entity);
                }
                provinces.Sort((x, y) => x.english.CompareTo(y.english));
                GlobalData.Provinces = provinces;
            }
        }

        private void btnCheckNames_Click(object sender, EventArgs e)
        {
            List<Tuple<UInt32, String, String, String>> romanizationMistakes = null;
            var romanizationMissing = new List<Tuple<UInt32, String>>();

            var country = new Entity();
            foreach ( var province in GlobalData.Provinces )
            {
                var provinceData = GlobalData.GetGeocodeList(province.geocode);
                country.entity.Add(provinceData);
            }
            var allEntities = country.FlatList();
            Int32 numberOfEntities = allEntities.Count();

            var romanizations = BuildRomanizationDictionary(out romanizationMistakes, allEntities);
            var romanizationSuggestions = FindRomanizationSuggestions(out romanizationMissing, allEntities, romanizations);

            UInt32 provinceFilter = 0;
            //if ( cbxCheckNamesFiltered.Checked )
            //{
            //    provinceFilter = GetCurrentChangwat().Geocode;
            //}

            StringBuilder romanizationMistakesBuilder = new StringBuilder();
            Int32 romanizationMistakeCount = 0;
            foreach ( var entry in romanizationMistakes )
            {
                if ( GeocodeHelper.IsBaseGeocode(provinceFilter, entry.Item1) )
                {
                    romanizationMistakesBuilder.AppendLine(String.Format("{0} {1}: {2} vs. {3}", entry.Item1, entry.Item2, entry.Item3, entry.Item4));
                    romanizationMistakeCount++;
                }
            }

            if ( romanizationMistakeCount > 0 )
            {
                var lForm = new StringDisplayForm(
                    String.Format("Romanization problems ({0})", romanizationMistakeCount),
                    romanizationMistakesBuilder.ToString());
                lForm.Show();
            }

            StringBuilder romanizationSuggestionBuilder = new StringBuilder();
            Int32 romanizationSuggestionCount = 0;
            foreach ( var entry in romanizationSuggestions )
            {
                if ( GeocodeHelper.IsBaseGeocode(provinceFilter, entry.Item1) )
                {
                    var entity = allEntities.FirstOrDefault(x => x.geocode == entry.Item1);
                    var suggestedName = entry.Item3;
                    switch ( entity.type )
                    {
                        case EntityType.Muban:
                            suggestedName = "Ban " + suggestedName.StripBanOrChumchon();
                            break;
                        case EntityType.Chumchon:
                            suggestedName = "Chumchon " + suggestedName.StripBanOrChumchon();
                            break;
                        default:
                            break;
                    }
                    romanizationSuggestionBuilder.AppendLine(String.Format("<entity type=\"{0}\" geocode=\"{1}\" name=\"{2}\" english=\"{3}\" />",
                        entity.type, entity.geocode, entity.name, suggestedName));
                    romanizationSuggestionCount++;
                }
            }
            if ( romanizationSuggestionCount > 0 )
            {
                var lForm = new StringDisplayForm(
                    String.Format("Romanization suggestions ({0}", romanizationSuggestionCount),
                    romanizationSuggestionBuilder.ToString());
                lForm.Show();

                List<Tuple<String, String, Int32>> counter = new List<Tuple<String, String, Int32>>();
                foreach ( var entry in romanizationSuggestions )
                {
                    var found = counter.FirstOrDefault(x => x.Item1 == entry.Item2);
                    if ( found == null )
                    {
                        counter.Add(Tuple.Create(entry.Item2, entry.Item3, 1));
                    }
                    else
                    {
                        counter.Remove(found);
                        counter.Add(Tuple.Create(entry.Item2, entry.Item3, found.Item3 + 1));
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
                        sortedBuilder.AppendLine(String.Format("{0}: {1} ({2})", entry.Item1, entry.Item2, entry.Item3));
                        suggestionCounter += entry.Item3;
                    }
                    var lForm2 = new StringDisplayForm(
                        String.Format("Romanization suggestions ({0} of {1})", suggestionCounter, romanizationSuggestionCount),
                        sortedBuilder.ToString());
                    lForm2.Show();
                }
            }

            // show missing romanizations
            if ( romanizationMissing.Any() )
            {
                List<Tuple<String, Int32>> counter = new List<Tuple<String, Int32>>();
                foreach ( var entry in romanizationMissing )
                {
                    var found = counter.FirstOrDefault(x => x.Item1 == entry.Item2);
                    if ( found == null )
                    {
                        counter.Add(Tuple.Create(entry.Item2, 1));
                    }
                    else
                    {
                        counter.Remove(found);
                        counter.Add(Tuple.Create(entry.Item2, found.Item2 + 1));
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
                        sortedBuilder.AppendLine(String.Format("{0}: {1}", entry.Item1, entry.Item2));
                        missingCounter += entry.Item2;
                    }
                    var lForm2 = new StringDisplayForm(
                        String.Format("Romanization missing ({0} of {1})", romanizationMissing.Count, numberOfEntities),
                        sortedBuilder.ToString());
                    lForm2.Show();
                }
            }
        }

        private static List<Tuple<UInt32, String, String>> FindRomanizationSuggestions(out List<Tuple<UInt32, String>> romanizationMissing, IEnumerable<Entity> allEntities, IDictionary<String, String> romanizations)
        {
            var result = new List<Tuple<UInt32, String, String>>();
            romanizationMissing = new List<Tuple<UInt32, String>>();
            foreach ( var entityToCheck in allEntities )
            {
                if ( String.IsNullOrEmpty(entityToCheck.name) )
                {
                    continue;
                }
                if ( String.IsNullOrEmpty(entityToCheck.english) )
                {
                    String foundEnglishName = String.Empty;
                    if ( romanizations.Keys.Contains(entityToCheck.name) )
                    {
                        foundEnglishName = entityToCheck.name;
                    }
                    else
                    {
                        var searchName = entityToCheck.name.StripBanOrChumchon();

                        if ( romanizations.Keys.Contains(searchName) )
                        {
                            foundEnglishName = searchName;
                        }
                        else
                        {
                            // Chumchon may have the name "Chumchon Ban ..."
                            searchName = searchName.StripBanOrChumchon();
                            if ( romanizations.Keys.Contains(searchName) )
                            {
                                foundEnglishName = searchName;
                            }
                        }
                    }

                    if ( !String.IsNullOrEmpty(foundEnglishName) )
                    {
                        result.Add(Tuple.Create(entityToCheck.geocode, entityToCheck.name, romanizations[foundEnglishName]));
                    }
                    else
                    {
                        Boolean found = false;
                        String name = entityToCheck.name.StripBanOrChumchon();
                        name = ThaiNumeralHelper.ReplaceThaiNumerals(name);
                        List<Char> numerals = new List<Char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                        foreach ( Char c in numerals )
                        {
                            name = name.Replace(c, ' ');
                        }
                        name = name.Trim();
                        foreach ( var keyValuePair in ThaiLanguageHelper.NameSuffixRomanizations )
                        {
                            if ( entityToCheck.name.EndsWith(keyValuePair.Key) )
                            {
                                String searchString = name.Substring(0, name.Length - keyValuePair.Key.Length).StripBanOrChumchon();
                                if ( String.IsNullOrEmpty(searchString) )
                                {
                                    result.Add(Tuple.Create(entityToCheck.geocode, entityToCheck.name, keyValuePair.Value));
                                    found = true;
                                }
                                else if ( romanizations.Keys.Contains(searchString) )
                                {
                                    result.Add(Tuple.Create(entityToCheck.geocode, entityToCheck.name, romanizations[searchString] + " " + keyValuePair.Value));
                                    found = true;
                                }
                            }
                        }
                        if ( !found )
                        {
                            var prefixes = ThaiLanguageHelper.NamePrefixRomanizations.Union(ThaiLanguageHelper.NameSuffixRomanizations);
                            foreach ( var keyValuePair in prefixes )
                            {
                                if ( name.StartsWith(keyValuePair.Key) )
                                {
                                    String searchString = name.Substring(keyValuePair.Key.Length);
                                    if ( String.IsNullOrEmpty(searchString) )
                                    {
                                        result.Add(Tuple.Create(entityToCheck.geocode, entityToCheck.name, keyValuePair.Value));
                                        found = true;
                                    }
                                    else if ( romanizations.Keys.Contains(searchString) )
                                    {
                                        result.Add(Tuple.Create(entityToCheck.geocode, entityToCheck.name, keyValuePair.Value + " " + romanizations[searchString]));
                                        found = true;
                                    }
                                }
                            }
                        }
                        if ( !found )
                        {
                            romanizationMissing.Add(Tuple.Create(entityToCheck.geocode, entityToCheck.name));
                        }
                    }
                }
            }
            return result;
        }

        private static Dictionary<String, String> BuildRomanizationDictionary(out List<Tuple<UInt32, String, String, String>> romanizationMistakes, IEnumerable<Entity> allEntities)
        {
            var result = new Dictionary<String, String>();
            romanizationMistakes = new List<Tuple<UInt32, String, String, String>>();
            foreach ( var entityToCheck in allEntities )
            {
                if ( !String.IsNullOrEmpty(entityToCheck.english) )
                {
                    String name = entityToCheck.name;
                    String english = entityToCheck.english;
                    if ( (entityToCheck.type == EntityType.Muban) | (entityToCheck.type == EntityType.Chumchon) )
                    {
                        name = name.StripBanOrChumchon();

                        if ( english.StartsWith("Ban ") )
                        {
                            english = english.Remove(0, "Ban ".Length).Trim();
                        }
                        if ( english.StartsWith("Chumchon ") )
                        {
                            english = english.Remove(0, "Chumchon ".Length).Trim();
                        }

                        if ( entityToCheck.type == EntityType.Chumchon )
                        {
                            // Chumchon may have the name "Chumchon Ban ..."
                            name = name.StripBanOrChumchon();

                            // or even Chumchon Mu Ban
                            const String ThaiStringMuban = "หมู่บ้าน";
                            if ( name.StartsWith(ThaiStringMuban, StringComparison.Ordinal) )
                            {
                                name = name.Remove(0, ThaiStringMuban.Length).Trim();
                            }

                            if ( english.StartsWith("Ban ") )
                            {
                                english = english.Remove(0, "Ban ".Length).Trim();
                            }
                            if ( english.StartsWith("Mu Ban ") )
                            {
                                english = english.Remove(0, "Mu Ban ".Length).Trim();
                            }
                        }
                    }
                    if ( result.Keys.Contains(name) )
                    {
                        if ( result[name] != english )
                        {
                            romanizationMistakes.Add(Tuple.Create(entityToCheck.geocode, name, english, result[name]));
                        }
                    }
                    else
                    {
                        result[name] = english;
                    }
                }
            }
            return result;
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
                itemsWithUnexplainedCouncilSizeChanges.AddRange(GlobalData.GetGeocodeList(changwat.geocode).FlatList().Where(entity => entity.office.Any(office => office.council.Any(term => term.sizechangereason == CouncilSizeChangeReason.Unknown))));
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
                var displayForm = new StringDisplayForm(
                    String.Format("Invalid terms ({0})", count),
                    builder.ToString());
                displayForm.Show();
            }
        }

        private static IEnumerable<EntityTermEnd> EntitiesWithCouncilTermEndInYear(UInt32 changwatGeocode, Int32 year)
        {
            return EntitiesWithCouncilTermEndInTimeSpan(changwatGeocode, new DateTime(year, 1, 1), new DateTime(year, 12, 31));
        }

        private static IEnumerable<EntityTermEnd> EntitiesWithCouncilTermEndInTimeSpan(UInt32 changwatGeocode, DateTime begin, DateTime end)
        {
            var fullChangwat = GlobalData.GetGeocodeList(changwatGeocode);
            return fullChangwat.EntitiesWithCouncilTermEndInTimeSpan(begin, end);
        }

        private static IEnumerable<Entity> EntitiesWithOfficialTermEndInYear(UInt32 changwatGeocode, Int32 year)
        {
            return EntitiesWithOfficialTermEndInTimeSpan(changwatGeocode, new DateTime(year, 1, 1), new DateTime(year, 12, 31));
        }

        private static IEnumerable<Entity> EntitiesWithOfficialTermEndInTimeSpan(UInt32 changwatGeocode, DateTime begin, DateTime end)
        {
            var result = new List<Entity>();
            var fullChangwat = GlobalData.GetGeocodeList(changwatGeocode);
            foreach ( var item in fullChangwat.FlatList() )
            {
                foreach ( var office in item.office )
                {
                    office.officials.Items.Sort((x, y) => x.begin.CompareTo(y.begin));
                    var latestOfficial = office.officials.Items.LastOrDefault();
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
            var fullChangwat = GlobalData.GetGeocodeList(changwatGeocode);
            foreach ( var item in fullChangwat.FlatList() )
            {
                foreach ( var office in item.office )
                {
                    if ( office.type == OfficeType.MunicipalityOffice | office.type == OfficeType.TAOOffice | office.type == OfficeType.PAOOffice )
                    {
                        if ( !office.obsolete & !office.council.Any() )
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
            var fullChangwat = GlobalData.GetGeocodeList(changwatGeocode);
            foreach ( var item in fullChangwat.FlatList() )
            {
                Boolean hasInvalidTermData = false;
                foreach ( var office in item.office )
                {
                    office.council.Sort((x, y) => x.begin.CompareTo(y.begin));

                    CouncilTerm lastTerm = null;
                    foreach ( var term in office.council )
                    {
                        hasInvalidTermData = hasInvalidTermData | !term.CouncilSizeValid | !term.TermLengthValid(term.type.TermLength()) | !term.TermDatesValid;
                        if ( lastTerm != null )
                        {
                            if ( lastTerm.endSpecified )
                            {
                                hasInvalidTermData = hasInvalidTermData | lastTerm.end.CompareTo(term.begin) > 0;
                            }
                            if ( (term.Size != term.FinalSize) & (term.sizechangereason == CouncilSizeChangeReason.NoChange) )
                            {
                                hasInvalidTermData = true;
                            }
                            if ( (term.type == EntityType.PAO) & (lastTerm.Size > 0) & (term.Size > 0) )
                            {
                                if ( (term.Size != term.FinalSize) & (term.sizechangereason == CouncilSizeChangeReason.NoChange) )
                                {
                                    hasInvalidTermData = hasInvalidTermData | lastTerm.FinalSize != term.Size;
                                }
                            }

                            if ( (lastTerm.type == EntityType.TAO) & (term.type == EntityType.TAO) & (lastTerm.Size > 0) & (term.Size > 0) )
                            {
                                if ( (term.sizechangereason == CouncilSizeChangeReason.NoChange) )
                                {
                                    hasInvalidTermData = hasInvalidTermData | lastTerm.FinalSize != term.Size;
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
            var fullChangwat = GlobalData.GetGeocodeList(changwatGeocode);
            foreach ( var item in fullChangwat.FlatList() )
            {
                Boolean hasInvalidTermData = false;
                foreach ( var office in item.office )
                {
                    var electedOfficials = office.officials.Items.Where(x => (x.title == OfficialType.TAOMayor | x.title == OfficialType.Mayor | x.title == OfficialType.PAOChairman)).ToList();
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
            }
            else
            {
                MessageBox.Show(this, String.Format("Fatal error: Directory {0} does not exist." + Environment.NewLine + "Application will be terminated.", searchPath), "Directory error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            var itemWithCouncilTermEndsInChangwat = EntitiesWithCouncilTermEndInYear((cbxChangwat.SelectedItem as Entity).geocode, DateTime.Now.Year);
            // var itemWithOfficialTermEndsInChangwat = EntitiesWithCouncilTermEndInYear((cbxChangwat.SelectedItem as Entity).geocode, DateTime.Now.Year);
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
                var lForm = new StringDisplayForm(
                    String.Format("Term ends ({0})", count),
                    builder.ToString());
                lForm.Show();
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
                    office.council.Sort((x, y) => x.begin.CompareTo(y.begin));

                    CouncilTerm lastTerm = null;
                    foreach ( var term in office.council )
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

        private static void CountElectionWeekday(UInt32 geocode, FrequencyCounter counter)
        {
            var fullChangwat = GlobalData.GetGeocodeList(geocode);
            foreach ( var item in fullChangwat.FlatList() )
            {
                foreach ( var office in item.office )
                {
                    foreach ( var term in office.council )
                    {
                        counter.IncrementForCount((Int32)term.begin.DayOfWeek, item.geocode);
                    }
                }
            }
        }

        private void btnElectionWeekday_Click(object sender, EventArgs e)
        {
            var counter = new FrequencyCounter();
            foreach ( var changwat in GlobalData.Provinces )
            {
                CountElectionWeekday(changwat.geocode, counter);
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
    }
}