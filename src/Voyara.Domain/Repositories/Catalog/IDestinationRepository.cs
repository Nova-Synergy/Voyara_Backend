using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Repositories.Catalog;

/// <summary>Persistence contract for the Destination aggregate.</summary>
public interface IDestinationRepository
{
    /// <summary>Gets a destination by identifier.</summary>
    Task<Destination?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Returns destinations matching an optional search term and status.</summary>
    Task<IReadOnlyList<Destination>> SearchAsync(
        string? searchTerm,
        DestinationStatus? status,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    /// <summary>Registers a new destination for insertion.</summary>
    void Add(Destination destination);

    /// <summary>Counts destinations matching an optional search term and status.</summary>
    Task<int> CountAsync(string? searchTerm, DestinationStatus? status, CancellationToken cancellationToken = default);
}
