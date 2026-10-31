using Voyara.Domain.Entities.Transportation;

namespace Voyara.Domain.Repositories.Transportation;

/// <summary>Persistence contract for the CityTransport aggregate.</summary>
public interface ICityTransportRepository
{
    /// <summary>Gets a city transport with its routes, stations and linked destinations, or <see langword="null"/>.</summary>
    Task<CityTransport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Lists the city transports of a city, for reading. The results are not tracked for changes.</summary>
    Task<IReadOnlyList<CityTransport>> ListByCityAsync(int cityId, CancellationToken cancellationToken = default);

    /// <summary>Checks whether a city transport already exists for the city and transport type.</summary>
    Task<bool> ExistsAsync(int cityId, int transportTypeId, CancellationToken cancellationToken = default);

    /// <summary>Registers a new city transport for insertion.</summary>
    void Add(CityTransport cityTransport);
}
