using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara_DomainModel.Common
{
    /// <summary>
    /// Base class for any domain object that has an identity.
    /// Two entities are considered different if their <see cref="Id"/> differs,
    /// even when all their other values are identical.
    /// </summary>
    /// <typeparam name="TId">The type of the identifier (usually <see cref="Guid"/>).</typeparam>
    public abstract class Entity<TId>
    {
        /// <summary>
        /// Unique identifier of the entity. Assigned once when the entity is created
        /// and never changed afterwards.
        /// </summary>
        public TId Id { get; protected set; } = default!;
    }
}
