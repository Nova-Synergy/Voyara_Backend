using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Catalog.Queries.SearchDestinations;

/// <summary>Searches destinations with paging; cached under a stable key and invalidated by tag.</summary>
public sealed record SearchDestinationsQuery(string? SearchTerm, int Page, int PageSize)
    : ICachedQuery<PagedResult<DestinationResponse>>
{
    /// <inheritdoc />
    public string CacheKey => $"destinations:search:{SearchTerm?.Trim().ToLowerInvariant()}:{Page}:{PageSize}";

    /// <inheritdoc />
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);

    /// <inheritdoc />
    public IReadOnlyCollection<string> Tags => new[] { "destinations" };
}
