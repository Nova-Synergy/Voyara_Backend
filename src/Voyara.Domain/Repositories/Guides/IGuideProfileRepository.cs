namespace Voyara.Domain.Repositories.Guides
{
    /// <summary>
    /// Defines repository operations for retrieving and managing guide profiles.
    /// </summary>
    public interface IGuideProfileRepository
    {
        /// <summary>
        /// Retrieves all guide profiles.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection containing all guide profiles.</returns>
        Task<IReadOnlyList<Voyara.Domain.Entities.Guides.GuideProfile>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated collection of guide profiles.
        /// </summary>
        /// <param name="skip">The number of profiles to skip.</param>
        /// <param name="take">The maximum number of profiles to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of guide profiles matching the specified range.</returns>
        Task<IReadOnlyList<Voyara.Domain.Entities.Guides.GuideProfile>> GetAllAsync(int skip, int take, CancellationToken cancellationToken = default);

        /// <summary>
        /// Counts the total number of guide profiles.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The total number of guide profiles.</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a guide profile by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the guide profile.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The matching guide profile, or null if it does not exist.</returns>
        Task<Voyara.Domain.Entities.Guides.GuideProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a guide profile by its unique identifier, including its related details.
        /// </summary>
        /// <param name="id">The unique identifier of the guide profile.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The guide profile with its related details, or null if it does not exist.</returns>
        Task<Voyara.Domain.Entities.Guides.GuideProfile?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether a guide profile exists with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the guide profile.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the guide profile exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether a user already has a guide profile.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the user already has a guide profile; otherwise, false.</returns>
        Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether a city exists with the specified identifier.
        /// </summary>
        /// <param name="cityId">The unique identifier of the city.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the city exists; otherwise, false.</returns>
        Task<bool> CityExistsAsync(int cityId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new guide profile to the repository.
        /// </summary>
        /// <param name="profile">The guide profile to add.</param>
        void Add(Voyara.Domain.Entities.Guides.GuideProfile profile);

        /// <summary>
        /// Removes an existing guide profile from the repository.
        /// </summary>
        /// <param name="profile">The guide profile to remove.</param>
        void Remove(Voyara.Domain.Entities.Guides.GuideProfile profile);
    }
}