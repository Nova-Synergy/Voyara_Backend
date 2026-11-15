using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Catalog;

namespace Voyara.Infrastructure.Persistence.Repositories.Catalog;

/// <summary>EF Core implementation of the destination repository.</summary>
public sealed class DestinationRepository : IDestinationRepository
{
    private const string LikeEscapeCharacter = "\\";

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
        return await ApplyCriteria(_context.Destinations.AsNoTracking(), criteria)
            .Include(destination => destination.Translations)
            .Include(destination => destination.Images)
            .OrderByDescending(destination => destination.PopularityScore)
            .ThenBy(destination => destination.Id)
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

        var text = criteria.Text?.Trim();
        if (!string.IsNullOrEmpty(text))
        {
            var pattern = $"%{EscapeLikePattern(text)}%";

            // Matches any translation, so a visitor can search in a language other than the display one.
            query = query.Where(destination => destination.Translations.Any(translation =>
                EF.Functions.ILike(translation.Name, pattern, LikeEscapeCharacter)
                || (translation.Description != null
                    && EF.Functions.ILike(translation.Description, pattern, LikeEscapeCharacter))));
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

    private static string EscapeLikePattern(string value)
        => value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal);
}
