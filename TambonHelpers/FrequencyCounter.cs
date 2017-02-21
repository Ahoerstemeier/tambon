using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Number frequency counter specific for values concerning entities with a geocode.
    /// </summary>
    public class FrequencyCounter
    {
        #region fields

        /// <summary>
        /// Dirty indicator.
        /// </summary>
        private Boolean _dirty = true;

        #endregion fields

        #region properties

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        /// <remarks>Key is the value, Value is the list of geocodes for the given value.</remarks>
        public Dictionary<Int32, List<UInt32>> Data
        {
            get;
            private set;
        }

        #region MediaValue

        /// <summary>
        /// Backing field for <see cref=" MedianValue"/>.
        /// </summary>
        private double _medianValue = 0;

        /// <summary>
        /// Gets the median value.
        /// </summary>
        /// <value>The median value.</value>
        public Double MedianValue
        {
            get
            {
                if ( _dirty )
                {
                    CalculateStatistics();
                };
                return _medianValue;
            }
        }

        #endregion MediaValue

        #region MaxValue

        /// <summary>
        /// Backing field for <see cref=" MaxValue"/>.
        /// </summary>
        private Int32 _maxValue = 0;

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        /// <value>The maximum value.</value>
        public Int32 MaxValue
        {
            get
            {
                if ( _dirty )
                {
                    CalculateStatistics();
                };
                return _maxValue;
            }
        }

        #endregion MaxValue

        #region MinValue

        /// <summary>
        /// Backing field for <see cref=" MinValue"/>.
        /// </summary>
        private Int32 _minValue = 0;

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        /// <value>The minimum value.</value>
        public Int32 MinValue
        {
            get
            {
                if ( _dirty )
                {
                    CalculateStatistics();
                };
                return _minValue;
            }
        }

        #endregion MinValue

        #region MostCommonValue

        /// <summary>
        /// Backing field for <see cref="MostCommonValue"/>.
        /// </summary>
        private Int32 _mostCommonValue = 0;

        /// <summary>
        /// Gets the most common value.
        /// </summary>
        /// <value>The most common value.</value>
        public Int32 MostCommonValue
        {
            get
            {
                if ( _dirty )
                {
                    CalculateStatistics();
                };
                return _mostCommonValue;
            }
        }

        #endregion MostCommonValue

        #region MostCommonValueValueCount

        /// <summary>
        /// Backing field for <see cref="MostCommonValueCount"/>.
        /// </summary>
        private Int32 _mostCommonValueCount = 0;

        /// <summary>
        /// Gets the number of entities which has the most common value.
        /// </summary>
        /// <value>The number of entities with the most common value.</value>
        public Int32 MostCommonValueCount
        {
            get
            {
                if ( _dirty )
                {
                    CalculateStatistics();
                };
                return _mostCommonValueCount;
            }
        }

        #endregion MostCommonValueValueCount

        #region NumberOfValues

        /// <summary>
        /// Backing field for <see cref="NumberOfValues"/>.
        /// </summary>
        private Int32 _count = 0;

        /// <summary>
        /// Gets the number of values.
        /// </summary>
        /// <value>The number of values.</value>
        public Int32 NumberOfValues
        {
            get
            {
                if ( _dirty )
                {
                    CalculateStatistics();
                };
                return _count;
            }
        }

        #endregion NumberOfValues

        #region SumValue

        /// <summary>
        /// Backing field for <see cref="SumValue"/>.
        /// </summary>
        private Int32 _sum = 0;

        /// <summary>
        /// Gets the sum of values.
        /// </summary>
        /// <value>The sum of values.</value>
        public Int32 SumValue
        {
            get
            {
                if ( _dirty )
                {
                    CalculateStatistics();
                };
                return _sum;
            }
        }

        #endregion SumValue

        #region StandardDeviation

        /// <summary>
        /// Backing field for <see cref="StandardDeviation"/>.
        /// </summary>
        private Double _standardDeviation = 0;

        /// <summary>
        /// Gets the standard deviation.
        /// </summary>
        /// <value>The standard deviation.</value>
        public Double StandardDeviation
        {
            get
            {
                if ( _dirty )
                {
                    CalculateStatistics();
                };
                return _standardDeviation;
            }
        }

        #endregion StandardDeviation

        #endregion properties

        #region constructor

        public FrequencyCounter()
        {
            Data = new Dictionary<Int32, List<UInt32>>();
        }

        #endregion constructor

        #region methods

        /// <summary>
        /// Adds a datapoint.
        /// </summary>
        /// <param name="value">Data value.</param>
        /// <param name="geocode">Geocode of the entity.</param>
        public void IncrementForCount(Int32 value, UInt32 geocode)
        {
            if ( !Data.ContainsKey(value) )
            {
                Data.Add(value, new List<UInt32>());
            }
            Data[value].Add(geocode);
            _dirty = true;
        }

        /// <summary>
        /// Recalculates the statistical values.
        /// </summary>
        private void CalculateStatistics()
        {
            NormalizeData();
            _medianValue = 0;
            _mostCommonValue = 0;
            _mostCommonValueCount = 0;
            _standardDeviation = 0;
            _sum = 0;
            _count = 0;
            var keys = Data.Keys.ToList();
            keys.Sort();
            _minValue = keys.FirstOrDefault();
            _maxValue = keys.LastOrDefault();

            foreach ( var keyValue in Data )
            {
                Int32 currentCount = keyValue.Value.Count;
                _count += currentCount;
                _sum += keyValue.Key * currentCount;
                if ( currentCount > _mostCommonValueCount )
                {
                    _mostCommonValueCount = currentCount;
                    _mostCommonValue = keyValue.Key;
                }
            }
            if ( _count > 0 )
            {
                _medianValue = (_sum * 1.0 / _count);
                double deviation = 0;
                foreach ( var keyValue in Data )
                {
                    if ( keyValue.Value != null )
                    {
                        Int32 currentCount = keyValue.Value.Count;
                        if ( currentCount > 0 )
                        {
                            deviation = deviation + Math.Pow(keyValue.Key - _medianValue, 2) * currentCount;
                        }
                    }
                }
                _standardDeviation = Math.Sqrt(deviation / _count);
            }
        }

        /// <summary>
        /// Cleaning up the <see cref="Data"/>.
        /// </summary>
        private void NormalizeData()
        {
            foreach ( var value in Data.Where(x => x.Value == null).ToList() )
            {
                Data.Remove(value.Key);
            }
            foreach ( var value in Data.Where(x => !x.Value.Any()).ToList() )
            {
                Data.Remove(value.Key);
            }
        }

        #endregion methods
    }
}