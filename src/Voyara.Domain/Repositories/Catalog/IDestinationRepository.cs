using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Repositories.Catalog
{

    /// <summary>
    /// Persistence contract for the Destination aggregate (images, hours, exceptions, prices, translations).
    /// </summary>
    public interface IDestinationRepository
    {
        /// <summary>
        /// Gets the full destination aggregate by identifier, or <see langword="null"/>.
        /// </summary>
        Task<Destination?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns destinations matching an optional search term (against translated name/description),
        /// status, city and category, ordered by popularity then identifier.
        /// </summary>
        Task<IReadOnlyList<Destination>> SearchAsync(
            string? searchTerm,
            DestinationStatus? status,
            int skip,
            int take,
            CancellationToken cancellationToken = default,
            Guid? cityId = null,
            Guid? categoryId = null);

        /// <summary>
        /// Counts destinations matching the same filters as <see cref="SearchAsync"/>.
        /// </summary>
        Task<int> CountAsync(
            string? searchTerm,
            DestinationStatus? status,
            CancellationToken cancellationToken = default,
            Guid? cityId = null,
            Guid? categoryId = null);

        /// <summary>
        /// Registers a new destination for insertion.
        /// </summary>
        void Add(Destination destination);
    }
}