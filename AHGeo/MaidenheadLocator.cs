/*
 * Original author: G. Monz (DK7IO), 2011-03-31
 * This file is distributed without any warranty.
 * http://www.mydarc.de/DK7IO/programmierung/GM.Geodesy/MaidenheadLocator.cs
 * */
using System;
using System.Collections.Generic;

namespace De.AHoerstemeier.Geo
{
    /// <summary>
    /// Converts geographical coordinates to a 'Maidenhead Locator' and vice versa.
    /// </summary>
    internal static class MaidenheadLocator
    {
        #region Constants
        #region Number of zones
        /// <summary>
        /// Number of zones for 'Field' (Precision step 1).
        /// </summary>
        public const int ZonesOddStep1 = 18;

        /// <summary>
        /// Number of zones for 'Subsquare', 'Subsubsubsquare', etc. (Precision steps 3, 5, etc.).
        /// </summary>
        public const int ZonesOddStepsExcept1 = 24;

        /// <summary>
        /// Number of zones for 'Square', 'Subsubsquare', etc. (Precision steps 2, 4, etc.).
        /// </summary>
        public const int ZonesEvenSteps = 10;
        #endregion

        #region First characters for locator text
        /// <summary>
        /// The first character for 'Field' (Precision step 1).
        /// </summary>
        private const char mFirstOddStep1Character = 'A';

        /// <summary>
        /// The first character for 'Subsquare', 'Subsubsubsquare', etc. (Precision steps 3, 5, etc.).
        /// </summary>
        private const char mFirstOddStepsExcept1Character = 'a';

        /// <summary>
        /// The first character for 'Square', 'Subsubsquare', etc. (Precision steps 2, 4, etc.).
        /// </summary>
        private const char mFirstEvenStepsCharacter = '0';
        #endregion

        #region Implementation constraints
        /// <summary>
        /// The lowest allowed precision.
        /// </summary>
        public const int MinPrecision = 1;

        /// <summary>
        /// The highest allowed precision.
        /// </summary>
        public const int MaxPrecision = 12;
        #endregion

        private const int mLowerLatitudeLimit = -90;
        private const int mUpperLatitudeLimit = +90;
        private const int mLowerLongitudeLimit = -180;
        private const int mUpperLongitudeLimit = +180;
        #endregion

        /// <summary>
        /// Converts geographical coordinates (latitude and longitude, in degrees)
        /// to a 'Maidenhead Locator' until a specific precision.
        /// The maximum precision is 12 due to numerical limits of floating point operations.
        /// </summary>
        /// <param name="iLatitude">
        /// The latitude to convert ([-90...+90]).
        /// +90 is handled like +89.999...
        /// </param>
        /// <param name="iLongitude">
        /// The longitude to convert ([-180...+180]).
        /// +180 is handled like +179.999...
        /// </param>
        /// <param name="iSmallLettersForSubsquares">If true: generate small (if false: big) letters for 'Subsquares', 'Subsubsquare', etc.</param>
        /// <param name="iPrecision">
        /// The precision for conversion, must be &gt;=1 and &lt;=12.
        /// <para></para>
        /// <list type="bullet">
        ///		<listheader>
        ///			<description>Examples for precision use:</description>
        ///		</listheader>
        ///		<item><term>precision1</term><description>HF: 'Field' only is needed -&gt; precision=1 -&gt; JN</description></item>
        ///		<item><term>precision2</term><description>6m: 'Field' and 'Square' is needed -&gt; precision=2 -&gt; JN39</description></item>
        ///		<item><term>precision3</term><description>VHF/UHF: 'Field' until 'Subsquare' is needed -&gt; precision=3 -&gt; JN39ml</description></item>
        ///		<item><term>precision4</term><description>SHF/EHF: 'Field' until 'Subsubsquare' is needed -&gt; precision=4 -&gt; JN39ml36</description></item>
        /// </list>
        /// </param>
        /// <returns>The 'Maidenhead Locator'.</returns>
        /// <exception cref="ArgumentException">If the latitude or longitude exceeds its allowed interval.</exception>
        public static String GetMaidenheadLocator(Double iLatitude, Double iLongitude, Boolean iSmallLettersForSubsquares, Int32 iPrecision)
        {
            Int32 lPrecision = Math.Min(MaxPrecision, Math.Max(MinPrecision, iPrecision));

            String lResult = String.Empty;
            {
                List<char> locatorCharacters = new List<char>();

                Double lLatitudeWork = iLatitude + (-mLowerLatitudeLimit);
                Double lLongitudeWork = iLongitude + (-mLowerLongitudeLimit);

                //Zone size for step "0"
                Double lHeight;
                Double lWidth;
                InitializeZoneSize(out lHeight, out lWidth);

                for ( Int32 lStep = MinPrecision ; lStep <= lPrecision ; lStep++ )
                {
                    Int32 lZones;
                    char lFirstCharacter;
                    RetrieveStepValues(lStep, iSmallLettersForSubsquares, out lZones, out lFirstCharacter);

                    //Zone size of current step
                    lHeight /= lZones;
                    lWidth /= lZones;

                    //Retrieve zones and locator characters
                    Int32 lLatitudeZone;
                    Int32 lLongitudeZone;
                    {
                        lLongitudeZone = Math.Min(lZones - 1, (int)(lLongitudeWork / lWidth));
                        {
                            char lLocatorCharacter = (char)(lFirstCharacter + lLongitudeZone);
                            locatorCharacters.Add(lLocatorCharacter);
                        }

                        lLatitudeZone = Math.Min(lZones - 1, (int)(lLatitudeWork / lHeight));
                        {
                            char lLocatorCharacter = (char)(lFirstCharacter + lLatitudeZone);
                            locatorCharacters.Add(lLocatorCharacter);
                        }
                    }

                    //Prepare the next step
                    {
                        lLatitudeWork -= lLatitudeZone * lHeight;
                        lLongitudeWork -= lLongitudeZone * lWidth;
                    }
                }

                //Build the result (Locator text)
                lResult = new string(locatorCharacters.ToArray());
            }

            return lResult;
        }

        /// <summary>
        /// Converts a 'Maidenhead Locator' to geographical coordinates (latitude and longitude, in degrees).
        /// </summary>
        /// <param name="iMaidenheadLocator">The 'Maidenhead Locator'.</param>
        /// <param name="lPositionInRectangle">The position of the geographical coordinates in the locator.</param>
        /// <param name="oLatitude">The geographical latitude.</param>
        /// <param name="oLongitude">The geographical longitude.</param>
        /// <exception cref="ArgumentException">
        /// If the length of the locator text is null or not an even number.
        /// If the locator text contains invalid characters.
        /// </exception>
        public static void GeographicalCoordinatesByMaidenheadLocator(String iMaidenheadLocator, PositionInRectangle lPositionInRectangle, out Double oLatitude, out Double oLongitude)
        {
            //Check arguments
            if ( String.IsNullOrEmpty(iMaidenheadLocator) || iMaidenheadLocator.Length % 2 != 0 )
            {
                throw new ArgumentException("Length of locator text is null or not an even number.", "maidenheadLocator");
            }

            //Corrections
            iMaidenheadLocator = iMaidenheadLocator.ToUpper();

            //Work
            {
                Int32 lPrecision = iMaidenheadLocator.Length / 2;

                oLatitude = mLowerLatitudeLimit;
                oLongitude = mLowerLongitudeLimit;

                //Zone size for step "0"
                Double lHeight;
                Double lWidth;
                InitializeZoneSize(out lHeight, out lWidth);

                for ( int lStep = 1 ; lStep <= lPrecision ; lStep++ )
                {
                    Int32 lZones;
                    char lFirstCharacter;
                    RetrieveStepValues(lStep, false, out lZones, out lFirstCharacter);

                    //Zone size of current step
                    lHeight /= lZones;
                    lWidth /= lZones;

                    //Retrieve precision specific geographical coordinates
                    Double lLongitudeStep = 0;
                    Double lLatitudeStep = 0;
                    {
                        Boolean lError = false;
                        Int32 lPosition = -1;

                        if ( !lError )
                        {
                            //Longitude

                            lPosition = lStep * 2 - 2;
                            char lLocatorCharacter = iMaidenheadLocator[lPosition];
                            Int32 lZone = (Int32)(lLocatorCharacter - lFirstCharacter);

                            if ( lZone >= 0 && lZone < lZones )
                            {
                                lLongitudeStep = lZone * lWidth;
                            }
                            else
                            {
                                lError = true;
                            }
                        }

                        if ( !lError )
                        {
                            //Latitude

                            lPosition = lStep * 2 - 1;
                            char lLocatorCharacter = iMaidenheadLocator[lPosition];
                            Int32 lZone = (Int32)(lLocatorCharacter - lFirstCharacter);

                            if ( lZone >= 0 && lZone < lZones )
                            {
                                lLatitudeStep = lZone * lHeight;
                            }
                            else
                            {
                                lError = true;
                            }
                        }

                        if ( lError )
                        {
                            throw new ArgumentException("Locator text contains an invalid character at position " + (lPosition + 1) + " (Current precision step is " + lStep + ").", "maidenheadLocator");
                        }
                    }
                    oLongitude += lLongitudeStep;
                    oLatitude += lLatitudeStep;
                }

                //Corrections according argument positionInRectangle
                GeoPoint.ShiftPositionInRectangle(ref oLatitude, ref oLongitude, lPositionInRectangle, lHeight, lWidth);
            }
        }

        private static void InitializeZoneSize(out Double oHeight, out Double oWidth)
        {
            oHeight = mUpperLatitudeLimit - mLowerLatitudeLimit;
            oWidth = mUpperLongitudeLimit - mLowerLongitudeLimit;
        }

        private static void RetrieveStepValues(Int32 iStep, Boolean iSmallLettersForSubsquares, out Int32 oZones, out char oFirstCharacter)
        {
            if ( iStep % 2 == 0 )
            {
                //Step is even

                oZones = ZonesEvenSteps;
                oFirstCharacter = mFirstEvenStepsCharacter;
            }
            else
            {
                //Step is odd

                oZones = (iStep == 1 ? ZonesOddStep1 : ZonesOddStepsExcept1);
                oFirstCharacter = ((iStep >= 3 && iSmallLettersForSubsquares) ? mFirstOddStepsExcept1Character : mFirstOddStep1Character);
            }
        }
    }
}

