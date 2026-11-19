using Voyara.Domain.Entities.Geography;

namespace Voyara.Domain.Repositories.Geography;

public interface ICityRepository
{
    Task<IReadOnlyList<City>> GetAllAsync(
        int? countryId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<City?> GetByIdWithCountryAsync(
        int id,
        CancellationToken cancellationToken = default);
}