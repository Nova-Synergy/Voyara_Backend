using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Catalog;

namespace Voyara.Infrastructure.Persistence.Repositories.Catalog;

/// <summary>
/// EF Core implementation of <see cref="IDestinationRepository"/>.
/// </summary>
public sealed class DestinationRepository : IDestinationRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Creates the repository.
    /// </summary>
    public DestinationRepository(VoyaraDbContext context) => _context = context;

    /// <inheritdoc />
    public Task<Destination?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Destinations
            .Include(d => d.Images)
            .Include(d => d.OpeningHours)
            .Include(d => d.OpeningExceptions)
            .Include(d => d.TicketPrices)
            .Include(d => d.Translations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Destination>> SearchAsync(
        string? searchTerm,
        DestinationStatus? status,
        int skip,
        int take,
        CancellationToken cancellationToken = default,
        Guid? cityId = null,
        Guid? categoryId = null)
    {
        var query = Filter(_context.Destinations.AsNoTracking(), searchTerm, status, cityId, categoryId);

        return await query
            .Include(d => d.Translations)
            .Include(d => d.Images)
            .AsSplitQuery()
            .OrderByDescending(d => d.PopularityScore)
            .ThenBy(d => d.Id)
            .Skip(Math.Max(0, skip))
            .Take(Math.Max(1, take))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountAsync(
        string? searchTerm,
        DestinationStatus? status,
        CancellationToken cancellationToken = default,
        Guid? cityId = null,
        Guid? categoryId = null)
        => Filter(_context.Destinations.AsNoTracking(), searchTerm, status, cityId, categoryId)
            .CountAsync(cancellationToken);

    /// <inheritdoc />
    public void Add(Destination destination) => _context.Destinations.Add(destination);

    private static IQueryable<Destination> Filter(
        IQueryable<Destination> query,
        string? searchTerm,
        DestinationStatus? status,
        Guid? cityId,
        Guid? categoryId)
    {
        if (status is not null)
        {
            query = query.Where(d => d.Status == status);
        }

        if (cityId is not null)
        {
            query = query.Where(d => d.CityId == cityId);
        }

        if (categoryId is not null)
        {
            query = query.Where(d => d.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            // Name and description live in the translations, so match against any language.
            var term = searchTerm.Trim().ToLowerInvariant();
            query = query.Where(d => d.Translations.Any(t =>
                t.Name.ToLower().Contains(term)
                || (t.Description != null && t.Description.ToLower().Contains(term))));
        }

        return query;
    }
}
