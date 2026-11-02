using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Repositories.Catalog
{

    /// <summary>
    /// Persistence contract for the Destination aggregate (images, hours, exceptions, prices, translations).
    /// </summary>
    public interface IDestinationRepository
    {
        Task<Destination?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default);
    }
}