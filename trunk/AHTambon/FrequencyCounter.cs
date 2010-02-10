using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public class FrequencyCounter
    {
        private Boolean mDirty = true;
        #region properties
        private Dictionary<Int32, List<Int32>> mData = new Dictionary<Int32, List<Int32>>();
        public Dictionary<Int32, List<Int32>> Data { get { return mData; } }
        private double mMeanValue = 0;
        public double MeanValue { get { if (mDirty) { CalculateStatistics();}; return mMeanValue;}}
        private Int32 mMaxValue = 0;
        public Int32 MaxValue { get { if (mDirty) { CalculateStatistics();}; return mMaxValue;}}
        private Int32 mMinValue = 0;
        public Int32 MinValue { get { if (mDirty) { CalculateStatistics(); }; return mMinValue; } }
        private Int32 mMostCommonValue = 0;
        public Int32 MostCommonValue { get { if (mDirty) { CalculateStatistics(); }; return mMostCommonValue; } }
        private Int32 mMostCommonValueCount = 0;
        public Int32 MostCommonValueCount { get { if (mDirty) { CalculateStatistics(); }; return mMostCommonValueCount; } }
        private Int32 mCount = 0;
        public Int32 NumberOfValues { get { if (mDirty) { CalculateStatistics(); }; return mCount; } }
        private double mStandardDeviation = 0;
        public double StandardDeviation { get { if (mDirty) { CalculateStatistics(); }; return mStandardDeviation; } }
        #endregion
        #region constructor
        public FrequencyCounter()
        {
        }
        #endregion
        #region methods
        public void IncrementForCount(Int32 iValue, Int32 iGeocode)
        {
            if (!mData.ContainsKey(iValue))
            {
                mData.Add(iValue,new List<Int32>());
            }
            mData[iValue].Add(iGeocode);
            mDirty = true;
        }
        protected void CalculateStatistics()
        {
            mMeanValue = 0;
            mMaxValue = 0;
            mMinValue = 0;
            mMostCommonValue = 0;
            mMostCommonValueCount = 0;
            mStandardDeviation = 0;
            Int32 lSum = 0;
            mCount = 0;
            foreach (KeyValuePair<Int32, List<Int32>> lKeyValue in mData)
            {
                if ((lKeyValue.Value != null)&&(lKeyValue.Key!=0))
                {
                    Int32 lCurrentCount = lKeyValue.Value.Count;
                    if (lCurrentCount > 0)
                    {
                        mCount=mCount+lCurrentCount;
                        lSum = lSum + lKeyValue.Key*lCurrentCount;
                        if (mMinValue == 0)
                        {
                            mMinValue = lKeyValue.Key;
                        }
                        mMinValue = Math.Min(mMinValue, lKeyValue.Key);
                        mMaxValue = Math.Max(mMaxValue, lKeyValue.Key);
                        if (lCurrentCount > mMostCommonValueCount)
                        {
                            mMostCommonValueCount = lCurrentCount;
                            mMostCommonValue = lKeyValue.Key;
                        }
                    }
                }
            }
            if (mCount > 0)
            {
                mMeanValue = (lSum * 1.0 / mCount);
                double lDeviation = 0;
                foreach (KeyValuePair<Int32, List<Int32>> lKeyValue in mData)
                {
                    if (lKeyValue.Value != null)
                    {
                        Int32 lCurrentCount = lKeyValue.Value.Count;
                        if (lCurrentCount > 0)
                        {
                            lDeviation = lDeviation + Math.Pow(lKeyValue.Key - mMeanValue, 2);
                        }
                    }
                }
                mStandardDeviation = Math.Sqrt(lDeviation / mCount);
            }
        }
        #endregion
    }
}
