using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Voyara.Application.Abstractions;
using Voyara.Application.Catalog;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Entities.Geography;
using Voyara.Domain.Entities.Localization;

namespace Voyara.Infrastructure.Persistence;

/// <summary>
/// Unit of work backed by <see cref="VoyaraDbContext"/> with explicit transaction control.
/// It invalidates destination-search cache entries after committed changes to search-visible data.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly VoyaraDbContext _context;
    private readonly ICacheService _cache;
    private IDbContextTransaction? _transaction;
    private bool _invalidateDestinationSearchAfterCommit;

    /// <summary>Creates the unit of work.</summary>
    /// <param name="context">The scoped EF Core database context.</param>
    /// <param name="cache">The cache service used to invalidate destination search results.</param>
    public UnitOfWork(VoyaraDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _context.ChangeTracker.DetectChanges();
        var hasSearchVisibleChanges = _context.ChangeTracker.Entries()
            .Any(RequiresDestinationSearchInvalidation);

        var affectedRows = await _context.SaveChangesAsync(cancellationToken);
        if (!hasSearchVisibleChanges)
        {
            return affectedRows;
        }

        if (_transaction is not null)
        {
            // Invalidate only after the enclosing transaction commits; otherwise another request could repopulate
            // the cache from the old committed state while this transaction is still open.
            _invalidateDestinationSearchAfterCommit = true;
        }
        else
        {
            await _cache.RemoveByTagAsync(CatalogCacheTags.Destinations, CancellationToken.None);
        }

        return affectedRows;
    }

    /// <inheritdoc />
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _invalidateDestinationSearchAfterCommit = false;
    }

    /// <inheritdoc />
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            return;
        }

        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;

        if (_invalidateDestinationSearchAfterCommit)
        {
            _invalidateDestinationSearchAfterCommit = false;
            await _cache.RemoveByTagAsync(CatalogCacheTags.Destinations, CancellationToken.None);
        }
    }

    /// <inheritdoc />
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            return;
        }

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
        _invalidateDestinationSearchAfterCommit = false;
    }

    private static bool RequiresDestinationSearchInvalidation(EntityEntry entry)
    {
        if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            return false;
        }

        // Destination changes include status, location, category, popularity and rating updates.
        // Images and ticket prices affect the search response or its filters.
        if (entry.Entity is Destination
            or DestinationImage
            or TicketPrice
            or DestinationTranslation)
        {
            return true;
        }

        // Only relationship/visibility properties on already-persisted geography and category rows affect public
        // search. Adding an unused city/category does not change any existing result and should not evict the cache.
        if (entry.Entity is City && entry.State == EntityState.Modified)
        {
            return entry.Property(nameof(City.IsActive)).IsModified
                || entry.Property(nameof(City.IsDeleted)).IsModified
                || entry.Property(nameof(City.CountryId)).IsModified;
        }

        if (entry.Entity is Country && entry.State == EntityState.Modified)
        {
            return entry.Property(nameof(Country.IsActive)).IsModified
                || entry.Property(nameof(Country.IsDeleted)).IsModified;
        }

        if (entry.Entity is Category && entry.State == EntityState.Modified)
        {
            return entry.Property(nameof(Category.IsActive)).IsModified;
        }

        return false;
    }
}
