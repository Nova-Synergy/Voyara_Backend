using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Transportation;
using Voyara.Domain.Repositories.Transportation;

namespace Voyara.Infrastructure.Persistence.Repositories.Transportation;

/// <summary>EF Core implementation of <see cref="ITransportTypeRepository"/>.</summary>
public sealed class TransportTypeRepository : ITransportTypeRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>Creates the repository.</summary>
    public TransportTypeRepository(VoyaraDbContext context) => _context = context;

    /// <inheritdoc />
    public Task<TransportType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.TransportTypes.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        => _context.TransportTypes.AnyAsync(t => t.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<TransportType>> ListAsync(CancellationToken cancellationToken = default)
    {
        var results = await _context.TransportTypes
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        return results;
    }
}
