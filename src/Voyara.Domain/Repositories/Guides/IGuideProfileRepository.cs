using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Repositories.Guides;

/// <summary>
/// Provides persistence operations for tour guide profiles.
/// </summary>
public interface IGuideProfileRepository
{
    /// <summary>
    /// Gets a guide profile by its identifier.
    /// </summary>
    /// <param name="id">The guide profile identifier.</param>
    /// <param name="asNoTracking">
    /// <see langword="true"/> to retrieve the profile without tracking changes;
    /// otherwise, tracks it for updates.
    /// </param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The matching guide profile, or <see langword="null"/> if none is found.</returns>
    Task<GuideProfile?> GetByIdAsync(
        Guid id,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a guide profile to the repository.
    /// </summary>
    /// <param name="profile">The guide profile to add.</param>
    void Add(GuideProfile profile);

    /// <summary>
    /// Searches for guide profiles based on the specified criteria.
    /// </summary>
    /// <param name="cityId">The identifier of the city where the guide operates.</param>
    /// <param name="status">The status of the guide.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A tuple containing the matching guide profiles and the total count of profiles matching the criteria.</returns>
    Task<(IReadOnlyList<GuideProfile> Items, int Total)> SearchAsync(
        int? cityId,
        GuideStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}