using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Catalog;

namespace Voyara.Infrastructure.Persistence.Repositories.Catalog;

/// <summary>EF Core implementation of <see cref="IDestinationRepository"/>.</summary>
public sealed class DestinationRepository : IDestinationRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>Creates the repository.</summary>
    public DestinationRepository(VoyaraDbContext context) => _context = context;

    /// <inheritdoc />
    public Task<Destination?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Destinations.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Destination>> SearchAsync(
        string? searchTerm,
        DestinationStatus? status,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Destinations.AsNoTracking();

        if (status is not null)
        {
            query = query.Where(d => d.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLowerInvariant();
            query = query.Where(d => d.Name.ToLower().Contains(term)
                || (d.Description != null && d.Description.ToLower().Contains(term)));
        }

        var results = await query
            .OrderBy(d => d.Name)
            .Skip(Math.Max(0, skip))
            .Take(Math.Max(1, take))
            .ToListAsync(cancellationToken);

        return results;
    }

    /// <inheritdoc />
    public void Add(Destination destination) => _context.Destinations.Add(destination);

    /// <inheritdoc />
    public Task<int> CountAsync(string? searchTerm, DestinationStatus? status, CancellationToken cancellationToken = default)
    {
        var query = _context.Destinations.AsNoTracking();

        if (status is not null)
        {
            query = query.Where(d => d.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLowerInvariant();
            query = query.Where(d => d.Name.ToLower().Contains(term)
                || (d.Description != null && d.Description.ToLower().Contains(term)));
        }

        return query.CountAsync(cancellationToken);
    }
}
