using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
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
        ValidatePageArguments(skip, take);

        var query = ApplyCriteria(_context.Destinations.AsNoTracking(), criteria);
        if (criteria.SortBy == DestinationSortBy.Name)
        {
            var matching = await query
                .Include(destination => destination.Translations)
                .ToListAsync(cancellationToken);

            var pageIds = DestinationSearchOrdering.ApplyInMemory(matching, criteria)
                .Skip(skip)
                .Take(take)
                .Select(destination => destination.Id)
                .ToList();

            return await LoadPageByIdsAsync(pageIds, cancellationToken);
        }

        return await DestinationSearchOrdering.Apply(query, criteria)
            .Include(destination => destination.Translations)
            .Include(destination => destination.Images)
            .Skip(skip)
            .Take(take)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<Destination> Items, int TotalCount)> SearchPageAsync(
        DestinationSearchCriteria criteria,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        ValidatePageArguments(skip, take);

        // Count and page retrieval must observe one consistent database state. Serializable isolation prevents
        // concurrent inserts, updates, or deletes from changing the matching set between the two queries. This is
        // deliberately scoped to this read operation; provider-specific performance/locking should be reviewed during
        // deployment against the configured database provider.
        await using var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);

        var query = ApplyCriteria(_context.Destinations.AsNoTracking(), criteria);
        int totalCount;
        IReadOnlyList<Destination> items;

        if (criteria.SortBy == DestinationSortBy.Name)
        {
            // Database string ordering depends on provider collation. Materialize the filtered set once and sort it
            // with the same ordinal rule used by DestinationSummaryMapping before applying paging.
            var matching = await query
                .Include(destination => destination.Translations)
                .ToListAsync(cancellationToken);

            totalCount = matching.Count;
            var pageIds = DestinationSearchOrdering.ApplyInMemory(matching, criteria)
                .Skip(skip)
                .Take(take)
                .Select(destination => destination.Id)
                .ToList();

            items = await LoadPageByIdsAsync(pageIds, cancellationToken);
        }
        else
        {
            totalCount = await query.CountAsync(cancellationToken);
            items = await DestinationSearchOrdering.Apply(query, criteria)
                .Include(destination => destination.Translations)
                .Include(destination => destination.Images)
                .Skip(skip)
                .Take(take)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
        return (items, totalCount);
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

    private async Task<IReadOnlyList<Destination>> LoadPageByIdsAsync(
        IReadOnlyCollection<Guid> pageIds,
        CancellationToken cancellationToken)
    {
        if (pageIds.Count == 0)
        {
            return Array.Empty<Destination>();
        }

        var pageDestinations = await _context.Destinations
            .AsNoTracking()
            .Where(destination => pageIds.Contains(destination.Id))
            .Include(destination => destination.Translations)
            .Include(destination => destination.Images)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        var destinationsById = pageDestinations.ToDictionary(destination => destination.Id);
        return pageIds
            .Where(destinationsById.ContainsKey)
            .Select(id => destinationsById[id])
            .ToList();
    }

    private static void ValidatePageArguments(int skip, int take)
    {
        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip));
        }

        if (take < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(take));
        }
    }

    private static IQueryable<Destination> ApplyCriteria(
        IQueryable<Destination> query,
        DestinationSearchCriteria criteria)
    {
        // Public visibility rule: only published destinations in active cities/countries and active categories are searchable.
        query = query.Where(destination =>
            destination.Status == DestinationStatus.Published
            && destination.City.IsActive
            && destination.City.Country.IsActive
            && destination.Category.IsActive);

        // A destination without any translation has no displayable name, so it is never listed
        // (publishing requires a translation, this only guards against inconsistent data).
        query = query.Where(destination => destination.Translations.Any());

        var text = criteria.Text?.Trim();
        if (!string.IsNullOrEmpty(text))
        {
            // IndexOf is a literal substring operation, not a LIKE pattern. This keeps %, _ and backslashes
            // as ordinary search characters on relational providers that translate IndexOf to CHARINDEX/strpos.
            // Provider-specific Unicode/collation behavior still needs integration coverage against the target DB.
            var term = text.ToLowerInvariant();

            query = query.Where(destination => destination.Translations.Any(translation =>
                translation.Name.ToLower().IndexOf(term) >= 0
                || (translation.Description != null && translation.Description.ToLower().IndexOf(term) >= 0)));
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
