namespace Voyara.Application.DTOs;

/// <summary>One page of a paginated collection.</summary>
public sealed record PagedResult<T>
{
    private readonly List<T> _items;

    /// <summary>Items on the current page.</summary>
    public IReadOnlyList<T> Items => _items;

    /// <summary>Total item count across all pages.</summary>
    public int TotalCount { get; }

    /// <summary>Zero-based page index.</summary>
    public int Page { get; }

    /// <summary>Page size.</summary>
    public int PageSize { get; }

    /// <summary>Creates a page.</summary>
    public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        _items = [.. items];
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}
