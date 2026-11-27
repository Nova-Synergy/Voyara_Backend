namespace Voyara.Application.DTOs;

/// <summary>One page of a paginated collection.</summary>
public sealed record PagedResult<T>
{
    private readonly List<T> _items;

    /// <summary>Items on the current page.</summary>
    public IReadOnlyList<T> Items => _items;

    /// <summary>Total item count across all pages.</summary>
    public int TotalCount { get; }

    /// <summary>One-based page number.</summary>
    public int Page { get; }

    /// <summary>Page size.</summary>
    public int PageSize { get; }

    /// <summary>Total number of pages for <see cref="TotalCount"/> items at <see cref="PageSize"/>.</summary>
    public int TotalPages => (int)(((long)TotalCount + PageSize - 1) / PageSize);

    /// <summary>Indicates whether a page exists before the current one.</summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>Indicates whether a page exists after the current one.</summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>Creates a page.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The count is negative or the page or page size is below 1.</exception>
    public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentOutOfRangeException.ThrowIfNegative(totalCount);
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        _items = [.. items];
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}
