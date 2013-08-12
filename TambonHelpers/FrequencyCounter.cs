using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public class FrequencyCounter
    {
        private Boolean _dirty = true;

        #region properties

        public Dictionary<Int32, List<UInt32>> Data
        {
            get;
            private set;
        }

        private double _meanValue = 0;

        public double MeanValue
        {
            get
            {
                if ( _dirty )
                {
                    CalculateStatistics();
                };
                return _meanValue;
            }
        }

        private Int32 _maxValue = 0;

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

        private Int32 _minValue = 0;

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

        private Int32 _mostCommonValue = 0;

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

        private Int32 _mostCommonValueCount = 0;

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

        private Int32 _count = 0;

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

        private Int32 _sum = 0;

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

        private Double _standardDeviation = 0;

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

        #endregion properties

        #region constructor

        public FrequencyCounter()
        {
            Data = new Dictionary<Int32, List<UInt32>>();
        }

        #endregion constructor

        #region methods

        public void IncrementForCount(Int32 value, UInt32 geocode)
        {
            if ( !Data.ContainsKey(value) )
            {
                Data.Add(value, new List<UInt32>());
            }
            Data[value].Add(geocode);
            _dirty = true;
        }

        private void CalculateStatistics()
        {
            NormalizeData();
            _meanValue = 0;
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
                _meanValue = (_sum * 1.0 / _count);
                double deviation = 0;
                foreach ( var keyValue in Data )
                {
                    if ( keyValue.Value != null )
                    {
                        Int32 currentCount = keyValue.Value.Count;
                        if ( currentCount > 0 )
                        {
                            deviation = deviation + Math.Pow(keyValue.Key - _meanValue, 2);
                        }
                    }
                }
                _standardDeviation = Math.Sqrt(deviation / _count);
            }
        }

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