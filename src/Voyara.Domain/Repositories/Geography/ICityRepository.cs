using Voyara.Domain.Entities.Geography;

namespace Voyara.Domain.Repositories.Geography;

/// <summary>Read-side lookups for <see cref="City"/>. Cities are created and changed through their owning country.</summary>
public interface ICityRepository
{
    /// <summary>Gets a city by its GUID identifier.</summary>
    /// <param name="id">The identifier of the city to retrieve.</param>
    /// <param name="cancellationToken">Token used to cancel the database operation.</param>
    /// <returns>The city when it exists and is not soft-deleted; otherwise, <see langword="null"/>.</returns>
    Task<City?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
