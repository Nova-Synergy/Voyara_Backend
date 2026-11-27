using Voyara.Domain.Entities.Guides;

namespace Voyara.Domain.Repositories.Guides
{
    /// <summary>
    /// Defines repository operations for guide profiles.
    /// </summary>
    public interface IGuideProfileRepository
    {
        /// <summary>Adds a new guide profile aggregate.</summary>
        void Add(GuideProfile profile);

        /// <summary>Returns whether the user already owns a guide profile.</summary>
        Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>Gets a tracked profile (without children) by identifier.</summary>
        Task<GuideProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>Gets a tracked profile (without children) by owner.</summary>
        Task<GuideProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>Gets a tracked profile with languages, expertise, availability and pricing loaded.</summary>
        Task<GuideProfile?> GetByUserIdWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
