using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public class CreationStatisticsMuban : CreationStatisticsCentralGovernment
    {
        #region properties

        private Int32 _creationsWithoutParentName;
        private FrequencyCounter _highestMubanNumber = new FrequencyCounter();
        private Dictionary<String, Int32> _newNameSuffix = new Dictionary<string, Int32>();
        private Dictionary<String, Int32> _newNamePrefix = new Dictionary<string, Int32>();

        #endregion properties

        #region constructor

        public CreationStatisticsMuban()
        {
            StartYear = 1883;
            EndYear = DateTime.Now.Year;
        }

        public CreationStatisticsMuban(Int32 iStartYear, Int32 iEndYear)
        {
            StartYear = iStartYear;
            EndYear = iEndYear;
        }

        #endregion constructor

        #region methods

        protected override String DisplayEntityName()
        {
            return "Muban";
        }

        protected override void Clear()
        {
            base.Clear();
            _newNameSuffix = new Dictionary<string, Int32>();
            _newNamePrefix = new Dictionary<string, Int32>();
            _highestMubanNumber = new FrequencyCounter();
            _creationsWithoutParentName = 0;
        }

        protected override Boolean EntityFitting(EntityType iEntityType)
        {
            Boolean result = (iEntityType == EntityType.Muban);
            return result;
        }

        protected void ProcessContentForName(GazetteCreate create)
        {
            UInt32 tambonGeocode = create.geocode / 100;
            String name = create.name.StripBanOrChumchon();
            if ( !String.IsNullOrEmpty(name) )
            {
                String parentName = String.Empty;
                foreach ( var subEntry in create.Items )
                {
                    var areaChange = subEntry as GazetteAreaChange;
                    if ( areaChange != null )
                    {
                        if ( !String.IsNullOrEmpty(areaChange.name) )
                        {
                            parentName = areaChange.name;
                        }
                        // this really happened once, so cannot use this check for sanity of input data
                        // Debug.Assert(tambonGeocode == (areaChange.geocode / 100), String.Format("Parent muban for {0} has a different geocode {1}", create.geocode, areaChange.geocode));
                    }
                }
                parentName = parentName.StripBanOrChumchon();
                if ( !String.IsNullOrEmpty(parentName) )
                {
                    if ( name.StartsWith(parentName) )
                    {
                        String suffix = name.Remove(0, parentName.Length).Trim();
                        if ( _newNameSuffix.ContainsKey(suffix) )
                        {
                            _newNameSuffix[suffix]++;
                        }
                        else
                        {
                            _newNameSuffix.Add(suffix, 1);
                        }
                    }
                    if ( name.EndsWith(parentName) )
                    {
                        String prefix = name.Replace(parentName, "").Trim();

                        if ( _newNamePrefix.ContainsKey(prefix) )
                        {
                            _newNamePrefix[prefix]++;
                        }
                        else
                        {
                            _newNamePrefix.Add(prefix, 1);
                        }
                    }
                }
                else
                {
                    _creationsWithoutParentName++;
                }
            }
        }

        protected override void ProcessContent(GazetteCreate content)
        {
            base.ProcessContent(content);

            UInt32 mubanNumber = content.geocode % 100;
            if ( mubanNumber != content.geocode )
            {
                _highestMubanNumber.IncrementForCount(Convert.ToInt32(mubanNumber), content.geocode);
            }
            ProcessContentForName(content);
        }

        protected Int32 SuffixFrequency(String suffix)
        {
            Int32 result = 0;
            if ( _newNameSuffix.ContainsKey(suffix) )
            {
                result = _newNameSuffix[suffix];
            }
            return result;
        }

        protected Int32 PrefixFrequency(String iSuffix)
        {
            Int32 retval = 0;
            if ( _newNamePrefix.ContainsKey(iSuffix) )
            {
                retval = _newNamePrefix[iSuffix];
            }
            return retval;
        }

        protected Int32 SuffixFrequencyNumbers()
        {
            Int32 result = 0;
            foreach ( var keyValue in _newNameSuffix )
            {
                String name = ThaiNumeralHelper.ReplaceThaiNumerals(keyValue.Key);
                if ( (!String.IsNullOrEmpty(name)) && (name.IsNumeric()) )
                {
                    result = result + keyValue.Value;
                }
            }
            return result;
        }

        public override String Information()
        {
            StringBuilder lBuilder = new StringBuilder();
            AppendBasicInfo(lBuilder);
            AppendProblems(lBuilder);
            AppendChangwatInfo(lBuilder);
            AppendMubanNumberInfo(lBuilder);
            AppendParentFrequencyInfo(lBuilder, "Tambon");
            AppendChangwatInfo(lBuilder);
            AppendDayOfYearInfo(lBuilder);
            AppendNameInfo(lBuilder);

            String retval = lBuilder.ToString();
            return retval;
        }

        private void AppendProblems(StringBuilder iBuilder)
        {
            if ( _creationsWithoutParentName > 0 )
            {
                iBuilder.AppendLine(_creationsWithoutParentName.ToString() + " have no parent name");
            }
        }

        private void AppendMubanNumberInfo(StringBuilder iBuilder)
        {
            iBuilder.AppendLine("Highest number of muban: " + _highestMubanNumber.MaxValue.ToString());
            if ( _highestMubanNumber.MaxValue > 0 )
            {
                foreach ( Int32 lGeocode in _highestMubanNumber.Data[_highestMubanNumber.MaxValue] )
                {
                    iBuilder.Append(lGeocode.ToString() + ' ');
                }
            }
            iBuilder.AppendLine();
        }

        private void AppendNameInfo(StringBuilder builder)
        {
            builder.AppendLine(String.Format("Name equal: {0} times", SuffixFrequency(String.Empty)));
            List<String> standardSuffices = new List<String>() { "เหนือ", "ใต้", "พัฒนา", "ใหม่", "ทอง", "น้อย", "ใน" };
            foreach ( String suffix in standardSuffices )
            {
                builder.AppendLine(String.Format("Suffix {0}: {1} times", suffix, SuffixFrequency(suffix)));
            }
            builder.AppendLine("Suffix with number:" + SuffixFrequencyNumbers().ToString() + " times");

            List<String> standardPrefixes = new List<String>() { "ใหม่" };
            foreach ( String prefix in standardPrefixes )
            {
                builder.AppendLine(String.Format("Prefix {0}: {1} times", prefix, PrefixFrequency(prefix)));
            }

            builder.AppendLine();

            builder.Append("Other suffices: ");
            var sortedSuffices = new List<KeyValuePair<String, Int32>>();
            foreach ( var keyValuePair in _newNameSuffix )
            {
                String name = ThaiNumeralHelper.ReplaceThaiNumerals(keyValuePair.Key);
                if ( standardSuffices.Contains(name) )
                {
                }
                else if ( String.IsNullOrEmpty(keyValuePair.Key) )
                {
                }
                else if ( name.IsNumeric() )
                {
                }
                else
                {
                    sortedSuffices.Add(keyValuePair);
                }
            }
            sortedSuffices.Sort(delegate(KeyValuePair<String, Int32> x, KeyValuePair<String, Int32> y)
            {
                return y.Value.CompareTo(x.Value);
            });
            foreach ( var keyValuePair in sortedSuffices )
            {
                builder.Append(keyValuePair.Key + " (" + keyValuePair.Value.ToString() + ") ");
            }
            builder.AppendLine();
        }

        #endregion methods
    }
}