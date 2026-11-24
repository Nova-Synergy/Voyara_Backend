using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Catalog;

namespace Voyara.Infrastructure.Persistence.Repositories.Catalog;

/// <summary>EF Core implementation of the destination repository.</summary>
public sealed class DestinationRepository : IDestinationRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>Creates the repository.</summary>
    public DestinationRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<Destination?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Destinations
            .Include(destination => destination.Translations)
            .FirstOrDefaultAsync(destination => destination.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Destination>> SearchAsync(
        DestinationSearchCriteria criteria,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await DestinationSearchOrdering.Apply(ApplyCriteria(_context.Destinations.AsNoTracking(), criteria), criteria)
            .Include(destination => destination.Translations)
            .Include(destination => destination.Images)
            .Skip(skip)
            .Take(take)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountAsync(DestinationSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        return ApplyCriteria(_context.Destinations.AsNoTracking(), criteria)
            .CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Add(Destination destination)
    {
        _context.Destinations.Add(destination);
    }

    private static IQueryable<Destination> ApplyCriteria(
        IQueryable<Destination> query,
        DestinationSearchCriteria criteria)
    {
        // Public visibility rule: only published destinations of active cities/countries are searchable.
        query = query.Where(destination =>
            destination.Status == DestinationStatus.Published
            && destination.City.IsActive
            && destination.City.Country.IsActive);

        // A destination without any translation has no displayable name, so it is never listed
        // (publishing requires a translation, this only guards against inconsistent data).
        query = query.Where(destination => destination.Translations.Any());

        var text = criteria.Text?.Trim();
        if (!string.IsNullOrEmpty(text))
        {
            // Provider-neutral and literal: EF translates ToLower().Contains(...) for every relational provider
            // (strpos on PostgreSQL), so '%', '_' and '\' in the term are plain characters and no
            // provider-specific function (such as ILIKE) is needed.
            var term = text.ToLowerInvariant();

            query = query.Where(destination => destination.Translations.Any(translation =>
                translation.Name.ToLower().Contains(term)
                || (translation.Description != null && translation.Description.ToLower().Contains(term))));
        }

        // Location and category filters combine with AND (a city outside the requested country yields no results).
        if (criteria.CountryId is { } countryId)
        {
            query = query.Where(destination => destination.City.CountryId == countryId);
        }

        if (criteria.CityId is { } cityId)
        {
            query = query.Where(destination => destination.CityId == cityId);
        }

        if (criteria.CategoryId is { } categoryId)
        {
            query = query.Where(destination => destination.CategoryId == categoryId);
        }

        if (criteria.MinRating is { } minRating)
        {
            query = query.Where(destination => destination.AverageRating >= minRating);
        }

        if (criteria is { MaxPrice: { } maxPrice, Currency: { } currency })
        {
            query = query.Where(destination => destination.TicketPrices.Any(ticket =>
                ticket.Price.Currency == currency && ticket.Price.Amount <= maxPrice));
        }

        return query;
    }
}
