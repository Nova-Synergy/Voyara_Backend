using Voyara.Domain.Entities.Identity;

namespace Voyara.Domain.Repositories.Identity
{

    /// <summary>
    /// Persistence contract for the <see cref="TouristProfile"/> aggregate (interests and visit history).
    /// </summary>
    public interface ITouristProfileRepository
    {
        /// <summary>
        /// Gets a profile with interests and visit history, or <see langword="null"/>.
        /// </summary>
        Task<TouristProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the profile that belongs to the user, or <see langword="null"/>.
        /// </summary>
        Task<TouristProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether the user already has a profile.
        /// </summary>
        Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers a new profile for insertion.
        /// </summary>
        void Add(TouristProfile profile);
    }
}
