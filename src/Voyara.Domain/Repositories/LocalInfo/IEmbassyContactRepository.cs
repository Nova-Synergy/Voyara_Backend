using Voyara.Domain.Entities.LocalInfo;

namespace Voyara.Domain.Repositories.LocalInfo;

/// <summary>
/// Persistence contract for EmbassyContact.
/// </summary>
public interface IEmbassyContactRepository
{
    Task<EmbassyContact?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<EmbassyContact?> GetByCountryCodeAsync(
        string countryCode,
        CancellationToken cancellationToken = default);

    void Add(EmbassyContact contact);
}