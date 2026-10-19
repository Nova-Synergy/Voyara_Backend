using Voyara.Domain.Entities.Transportation;

namespace Voyara.Domain.Repositories.Transportation;

/// <summary>Persistence contract for the <see cref="CityTransport"/> aggregate.</summary>
public interface ICityTransportRepository
{
    /// <summary>Gets a city transport with its routes, stations and linked destinations, or <see langword="null"/>.</summary>
    Task<CityTransport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Checks whether a city transport already exists for the city and transport type.</summary>
    Task<bool> ExistsAsync(int cityId, int transportTypeId, CancellationToken cancellationToken = default);

    /// <summary>Adds a new city transport.</summary>
    Task AddAsync(CityTransport cityTransport, CancellationToken cancellationToken = default);
}
