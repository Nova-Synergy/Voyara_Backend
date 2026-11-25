using Voyara.Domain.Entities.Identity;

namespace Voyara.Domain.Repositories.Identity
{

    /// <summary>
    /// Persistence contract for the <see cref="TouristProfile"/> aggregate (interests and visit history).
    /// </summary>
    public interface ITouristProfileRepository
    {
        /// <summary>
        /// Gets the tracked profile of a user with its owning user and interests, for modification; null when none exists.
        /// </summary>
        Task<TouristProfile?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the profile of a user with its owning user and interests without change tracking, for reading; null when none exists.
        /// </summary>
        Task<TouristProfile?> GetReadOnlyByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
