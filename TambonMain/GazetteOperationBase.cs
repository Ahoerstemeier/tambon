using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.AHoerstemeier.Tambon
{
    public partial class GazetteOperationBase : IGeocode
    {
        #region IGeocode Members

        /// <summary>
        /// Checks if this instance is about the entity identified by the <paramref name="geocode"/>.
        /// If <paramref name="includeSubEntities"/> is <c>true</c>,
        /// </summary>
        /// <param name="geocode">Geocode to check.</param>
        /// <param name="includeSubEntities">Toggles whether codes under <paramref name="geocode"/> are considered fitting as well.</param>
        /// <returns><c>true</c> if instance is about the code, <c>false</c> otherwise.</returns>
        public Boolean IsAboutGeocode(UInt32 geocode, Boolean includeSubEntities)
        {
            Boolean result = false;
            if ( geocodeSpecified )
            {
                result = result | GeocodeHelper.IsSameGeocode(geocode, this.geocode, includeSubEntities);
            }
            if ( ownerFieldSpecified )
            {
                result = result | GeocodeHelper.IsSameGeocode(geocode, this.owner, includeSubEntities);
            }
            if ( tambonFieldSpecified )
            {
                result = result | GeocodeHelper.IsSameGeocode(geocode, this.tambon, includeSubEntities);
            }

            foreach ( var entry in Items )
            {
                var toTest = entry as IGeocode;
                if ( toTest != null )
                {
                    result = result | toTest.IsAboutGeocode(geocode, includeSubEntities);
                }
            }
            return result;
        }

        #endregion IGeocode Members

        /// <summary>
        /// Converts the gazette operation into a entity history entry.
        /// </summary>
        /// <returns>Corresponding history entry.</returns>
        public virtual HistoryEntryBase ConvertToHistory()
        {
            return null;
        }

        /// <summary>
        /// Gets a flat list of all the operation entries nested under the current one, including the current one.
        /// </summary>
        public IEnumerable<GazetteOperationBase> GazetteOperations()
        {
            var result = new List<GazetteOperationBase>();
            result.Add(this);
            foreach ( var item in Items )
            {
                var OperationItem = item as GazetteOperationBase;
                if ( OperationItem != null )
                {
                    result.AddRange(OperationItem.GazetteOperations());
                }
            }
            return result;
        }
    }
}