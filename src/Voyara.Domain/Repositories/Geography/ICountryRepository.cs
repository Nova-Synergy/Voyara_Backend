using Voyara.Domain.Entities.Geography;

namespace Voyara.Domain.Repositories.Geography;

public interface ICountryRepository
{
    Task<IReadOnlyList<Country>> GetAllAsync(
        bool? isActive = null,
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<Country?> GetByIdWithCitiesAsync(
        int id,
        CancellationToken cancellationToken = default);
    Task<Country?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIsoCodeAsync(string isoCode, CancellationToken cancellationToken = default);
    void Add(Country country);
    void Update(Country country);
}