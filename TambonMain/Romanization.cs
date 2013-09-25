using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public class RomanizationEntry
    {
        public UInt32 Geocode
        {
            get;
            private set;
        }

        public String Name
        {
            get;
            private set;
        }

        public String English
        {
            get;
            private set;
        }

        public RomanizationEntry(UInt32 geocode, String name)
        {
            Geocode = geocode;
            Name = name;
            English = String.Empty;
        }

        public RomanizationEntry(UInt32 geocode, String name, String english)
        {
            Geocode = geocode;
            Name = name;
            English = english;
        }
    }

    public class Romanization
    {
        #region properties

        public IDictionary<String, String> Romanizations
        {
            get;
            private set;
        }

        public IDictionary<RomanizationEntry, String> RomanizationMistakes
        {
            get;
            private set;
        }

        #endregion properties

        private RomanizationEntry CreateRomanizationEntry(Entity entity, String romanization)
        {
            var suggestedName = romanization;
            switch ( entity.type )
            {
                case EntityType.Muban:
                    romanization = "Ban " + romanization.StripBanOrChumchon();
                    break;
                case EntityType.Chumchon:
                    romanization = "Chumchon " + romanization.StripBanOrChumchon();
                    break;
                default:
                    break;
            }
            return new RomanizationEntry(entity.geocode, entity.name, romanization);
        }

        public List<RomanizationEntry> FindRomanizationSuggestions(out List<RomanizationEntry> romanizationMissing, IEnumerable<Entity> entitiesToCheck)
        {
            var result = new List<RomanizationEntry>();
            romanizationMissing = new List<RomanizationEntry>();
            foreach ( var entityToCheck in entitiesToCheck )
            {
                if ( String.IsNullOrEmpty(entityToCheck.name) )
                {
                    continue;
                }
                if ( String.IsNullOrEmpty(entityToCheck.english) )
                {
                    String foundEnglishName = String.Empty;
                    if ( Romanizations.Keys.Contains(entityToCheck.name) )
                    {
                        foundEnglishName = entityToCheck.name;
                    }
                    else
                    {
                        var searchName = entityToCheck.name.StripBanOrChumchon();

                        if ( Romanizations.Keys.Contains(searchName) )
                        {
                            foundEnglishName = searchName;
                        }
                        else
                        {
                            // Chumchon may have the name "Chumchon Ban ..."
                            searchName = searchName.StripBanOrChumchon();
                            if ( Romanizations.Keys.Contains(searchName) )
                            {
                                foundEnglishName = searchName;
                            }
                        }
                    }

                    if ( !String.IsNullOrEmpty(foundEnglishName) )
                    {
                        result.Add(CreateRomanizationEntry(entityToCheck, Romanizations[foundEnglishName]));
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
                                    result.Add(CreateRomanizationEntry(entityToCheck, keyValuePair.Value));
                                    found = true;
                                }
                                else if ( Romanizations.Keys.Contains(searchString) )
                                {
                                    result.Add(CreateRomanizationEntry(entityToCheck, Romanizations[searchString] + " " + keyValuePair.Value));

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
                                        result.Add(CreateRomanizationEntry(entityToCheck, keyValuePair.Value));
                                        found = true;
                                    }
                                    else if ( Romanizations.Keys.Contains(searchString) )
                                    {
                                        result.Add(CreateRomanizationEntry(entityToCheck, keyValuePair.Value + " " + Romanizations[searchString]));
                                        found = true;
                                    }
                                }
                            }
                        }
                        if ( !found )
                        {
                            romanizationMissing.Add(new RomanizationEntry(entityToCheck.geocode, entityToCheck.name));
                        }
                    }
                }
            }
            return result;
        }

        public void Initialize(IEnumerable<Entity> allEntities)
        {
            Romanizations.Clear();
            RomanizationMistakes.Clear();

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
                    if ( Romanizations.Keys.Contains(name) )
                    {
                        if ( Romanizations[name] != english )
                        {
                            RomanizationMistakes[new RomanizationEntry(entityToCheck.geocode, name, english)] = Romanizations[name];
                        }
                    }
                    else
                    {
                        Romanizations[name] = english;
                    }
                }
            }
        }

        public Romanization()
        {
            Romanizations = new Dictionary<String, String>();
            RomanizationMistakes = new Dictionary<RomanizationEntry, String>();
        }
    }
}