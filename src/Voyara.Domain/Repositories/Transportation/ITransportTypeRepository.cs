using Voyara.Domain.Entities.Transportation;

namespace Voyara.Domain.Repositories.Transportation;

/// <summary>Persistence contract for the <see cref="TransportType"/> reference data.</summary>
public interface ITransportTypeRepository
{
    /// <summary>Gets a transport type by identifier, or <see langword="null"/>.</summary>
    Task<TransportType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Checks whether a transport type exists.</summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Lists all transport types.</summary>
    Task<IReadOnlyList<TransportType>> ListAsync(CancellationToken cancellationToken = default);
}
