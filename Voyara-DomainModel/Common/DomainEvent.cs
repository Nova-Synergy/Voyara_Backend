using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara_DomainModel.Common
{
    /// <summary>
    /// Marker contract for anything that "happened" inside the domain
    /// (e.g. a review was approved). Other modules can react to these events
    /// without the module that raised them knowing who is listening.
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>The UTC date and time at which the event occurred.</summary>
        DateTime OccurredOn { get; }
    }

    /// <summary>
    /// Base record for all domain events. It stamps each event with the UTC time
    /// at which it was created, so concrete events only declare their own data.
    /// </summary>
    public abstract record DomainEvent : IDomainEvent
    {
        /// <inheritdoc />
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
