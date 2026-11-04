using Voyara.Domain.Entities.Identity;

namespace Voyara.Domain.Repositories.Identity
{

    /// <summary>
    /// Persistence contract for the <see cref="TouristProfile"/> aggregate (interests and visit history).
    /// </summary>
    public interface ITouristProfileRepository
    {
        /// <summary>
        /// Gets a profile (with its user and interests) by the owning user id.
        /// </summary>
        Task<TouristProfile?> GetByUserIdAsync(Guid userId, bool asNoTracking = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches tourists by name or e-mail with paging (read-only).
        /// </summary>
        Task<(IReadOnlyList<TouristProfile> Items, int TotalCount)> SearchAsync(
            string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts tracking a new profile.
        /// </summary>
        void Add(TouristProfile profile);
    }
}
