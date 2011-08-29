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
        private const int _ZonesOddStep1 = 18;

        /// <summary>
        /// Number of zones for 'Subsquare', 'Subsubsubsquare', etc. (Precision steps 3, 5, etc.).
        /// </summary>
        private const int _ZonesOddStepsExcept1 = 24;

        /// <summary>
        /// Number of zones for 'Square', 'Subsubsquare', etc. (Precision steps 2, 4, etc.).
        /// </summary>
        private const int _ZonesEvenSteps = 10;
        #endregion

        #region First characters for locator text
        /// <summary>
        /// The first character for 'Field' (Precision step 1).
        /// </summary>
        private const char _FirstOddStep1Character = 'A';

        /// <summary>
        /// The first character for 'Subsquare', 'Subsubsubsquare', etc. (Precision steps 3, 5, etc.).
        /// </summary>
        private const char _FirstOddStepsExcept1Character = 'a';

        /// <summary>
        /// The first character for 'Square', 'Subsubsquare', etc. (Precision steps 2, 4, etc.).
        /// </summary>
        private const char _FirstEvenStepsCharacter = '0';
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

        private const int _LowerLatitudeLimit = -90;
        private const int _UpperLatitudeLimit = +90;
        private const int _LowerLongitudeLimit = -180;
        private const int _UpperLongitudeLimit = +180;
        #endregion

        /// <summary>
        /// Converts geographical coordinates (latitude and longitude, in degrees)
        /// to a 'Maidenhead Locator' until a specific precision.
        /// The maximum precision is 12 due to numerical limits of floating point operations.
        /// </summary>
        /// <param name="latitude">
        /// The latitude to convert ([-90...+90]).
        /// +90 is handled like +89.999...
        /// </param>
        /// <param name="longitude">
        /// The longitude to convert ([-180...+180]).
        /// +180 is handled like +179.999...
        /// </param>
        /// <param name="smallLettersForSubsquares">If true: generate small (if false: big) letters for 'Subsquares', 'Subsubsquare', etc.</param>
        /// <param name="precision">
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
        public static String GetMaidenheadLocator(Double latitude, Double longitude, Boolean smallLettersForSubsquares, Int32 precision)
        {
            Int32 precisionValue = Math.Min(MaxPrecision, Math.Max(MinPrecision, precision));

            String result = String.Empty;
            {
                List<char> locatorCharacters = new List<char>();

                Double latitudeWork = latitude + (-_LowerLatitudeLimit);
                Double longitudeWork = longitude + (-_LowerLongitudeLimit);

                //Zone size for step "0"
                Double height;
                Double width;
                InitializeZoneSize(out height, out width);

                for ( Int32 step = MinPrecision ; step <= precisionValue ; step++ )
                {
                    Int32 zones;
                    char firstCharacter;
                    RetrieveStepValues(step, smallLettersForSubsquares, out zones, out firstCharacter);

                    //Zone size of current step
                    height /= zones;
                    width /= zones;

                    //Retrieve zones and locator characters
                    Int32 latitudeZone;
                    Int32 longitudeZone;
                    {
                        longitudeZone = Math.Min(zones - 1, (int)(longitudeWork / width));
                        {
                            char locatorCharacter = (char)(firstCharacter + longitudeZone);
                            locatorCharacters.Add(locatorCharacter);
                        }

                        latitudeZone = Math.Min(zones - 1, (int)(latitudeWork / height));
                        {
                            char locatorCharacter = (char)(firstCharacter + latitudeZone);
                            locatorCharacters.Add(locatorCharacter);
                        }
                    }

                    //Prepare the next step
                    {
                        latitudeWork -= latitudeZone * height;
                        longitudeWork -= longitudeZone * width;
                    }
                }

                //Build the result (Locator text)
                result = new string(locatorCharacters.ToArray());
            }

            return result;
        }

        /// <summary>
        /// Converts a 'Maidenhead Locator' to geographical coordinates (latitude and longitude, in degrees).
        /// </summary>
        /// <param name="maidenheadLocator">The 'Maidenhead Locator'.</param>
        /// <param name="positionInRectangle">The position of the geographical coordinates in the locator.</param>
        /// <param name="latitude">The geographical latitude.</param>
        /// <param name="longitude">The geographical longitude.</param>
        /// <exception cref="ArgumentException">
        /// If the length of the locator text is null or not an even number.
        /// If the locator text contains invalid characters.
        /// </exception>
        public static void GeographicalCoordinatesByMaidenheadLocator(String maidenheadLocator, PositionInRectangle positionInRectangle, out Double latitude, out Double longitude)
        {
            //Check arguments
            if ( String.IsNullOrEmpty(maidenheadLocator) || maidenheadLocator.Length % 2 != 0 )
            {
                throw new ArgumentException("Length of locator text is null or not an even number.", "maidenheadLocator");
            }

            //Corrections
            maidenheadLocator = maidenheadLocator.ToUpper();

            //Work
            {
                Int32 precisionValue = maidenheadLocator.Length / 2;

                latitude = _LowerLatitudeLimit;
                longitude = _LowerLongitudeLimit;

                //Zone size for step "0"
                Double height;
                Double width;
                InitializeZoneSize(out height, out width);

                for ( int step = 1 ; step <= precisionValue ; step++ )
                {
                    Int32 zones;
                    char firstCharacter;
                    RetrieveStepValues(step, false, out zones, out firstCharacter);

                    //Zone size of current step
                    height /= zones;
                    width /= zones;

                    //Retrieve precision specific geographical coordinates
                    Double longitudeStep = 0;
                    Double latitudeStep = 0;
                    {
                        Boolean error = false;
                        Int32 position = -1;

                        if ( !error )
                        {
                            //Longitude

                            position = step * 2 - 2;
                            char locatorCharacter = maidenheadLocator[position];
                            Int32 zone = (Int32)(locatorCharacter - firstCharacter);

                            if ( zone >= 0 && zone < zones )
                            {
                                longitudeStep = zone * width;
                            }
                            else
                            {
                                error = true;
                            }
                        }

                        if ( !error )
                        {
                            //Latitude

                            position = step * 2 - 1;
                            char locatorCharacter = maidenheadLocator[position];
                            Int32 zone = (Int32)(locatorCharacter - firstCharacter);

                            if ( zone >= 0 && zone < zones )
                            {
                                latitudeStep = zone * height;
                            }
                            else
                            {
                                error = true;
                            }
                        }

                        if ( error )
                        {
                            throw new ArgumentException("Locator text contains an invalid character at position " + (position + 1) + " (Current precision step is " + step + ").", "maidenheadLocator");
                        }
                    }
                    longitude += longitudeStep;
                    latitude += latitudeStep;
                }

                //Corrections according argument positionInRectangle
                GeoPoint.ShiftPositionInRectangle(ref latitude, ref longitude, positionInRectangle, height, width);
            }
        }

        private static void InitializeZoneSize(out Double height, out Double width)
        {
            height = _UpperLatitudeLimit - _LowerLatitudeLimit;
            width = _UpperLongitudeLimit - _LowerLongitudeLimit;
        }

        private static void RetrieveStepValues(Int32 step, Boolean smallLettersForSubsquares, out Int32 zones, out char firstCharacter)
        {
            if ( step % 2 == 0 )
            {
                //Step is even

                zones = _ZonesEvenSteps;
                firstCharacter = _FirstEvenStepsCharacter;
            }
            else
            {
                //Step is odd

                zones = (step == 1 ? _ZonesOddStep1 : _ZonesOddStepsExcept1);
                firstCharacter = ((step >= 3 && smallLettersForSubsquares) ? _FirstOddStepsExcept1Character : _FirstOddStep1Character);
            }
        }
    }
}

