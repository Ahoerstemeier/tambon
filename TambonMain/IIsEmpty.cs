using System;


namespace De.AHoerstemeier.Tambon
{
/// <summary>
/// Interface to check whether a structure has any data.
/// </summary>
    interface IIsEmpty
    {
        /// <summary>
        /// Checks whether the structure has any data set other than default values.
        /// </summary>
        /// <returns><c>true</c> if structure has any data set, <c>false</c> otherwise.</returns>
        Boolean IsEmpty();
    }
}
