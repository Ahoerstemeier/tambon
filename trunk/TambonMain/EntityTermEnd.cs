using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Class to encapsulate an entity and highlighting the term ends.
    /// </summary>
    public class EntityTermEnd
    {
        /// <summary>
        /// Gets the entity which has a term end.
        /// </summary>
        /// <value>The entity</value>
        public Entity Entity
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the council term in question.
        /// </summary>
        /// <value>The council term.</value>
        /// <remarks>Can be <c>null</c> if no council term ends.</remarks>
        public CouncilTerm CouncilTerm
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the official term in question.
        /// </summary>
        /// <value>The official term.</value>
        /// <remarks>Can be <c>null</c> if no official term ends.</remarks>
        public OfficialEntryBase OfficialTerm
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of <see cref="EntityTermEnd"/>.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="councilTerm">Council term.</param>
        /// <param name="officialTerm">Official term.</param>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="councilerm"/> and <paramref name="officialTerm"/> are <c>null</c> at same time.</exception>
        /// <remarks><paramref name="councilerm"/> and <paramref name="officialTerm"/> can be <c>null</c>, but not both.</remarks>
        public EntityTermEnd(Entity entity, CouncilTerm councilTerm, OfficialEntryBase officialTerm)
        {
            if ( entity == null )
            {
                throw new ArgumentNullException("entity");
            }
            if ( (councilTerm == null) & (officialTerm == null) )
            {
                throw new ArgumentException();
            }

            Entity = entity;
            CouncilTerm = councilTerm;
            OfficialTerm = officialTerm;
        }
    }
}