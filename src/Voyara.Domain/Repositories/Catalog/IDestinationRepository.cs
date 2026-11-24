using Voyara.Domain.Entities.Catalog;

namespace Voyara.Domain.Repositories.Catalog
{
    /// <summary>
    /// Persistence contract for the Destination aggregate (images, hours, exceptions, prices, translations).
    /// </summary>
    public interface IDestinationRepository
    {
        /// <summary>
        /// Gets a destination (any status) by identifier, tracked and with its translations loaded,
        /// or null when it does not exist.
        /// </summary>
        Task<Destination?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches the public catalog: only published destinations located in an active city of an
        /// active country are returned. Results are read-only and include translations and images.
        /// </summary>
        /// <param name="criteria">The search specification.</param>
        /// <param name="skip">Number of results to skip.</param>
        /// <param name="take">Maximum number of results to return.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<Destination>> SearchAsync(
            DestinationSearchCriteria criteria,
            int skip,
            int take,
            CancellationToken cancellationToken = default);

        /// <summary>Counts the destinations matching the criteria, ignoring paging.</summary>
        Task<int> CountAsync(DestinationSearchCriteria criteria, CancellationToken cancellationToken = default);

        /// <summary>Adds a new destination to the persistence context.</summary>
        void Add(Destination destination);
    }
}
