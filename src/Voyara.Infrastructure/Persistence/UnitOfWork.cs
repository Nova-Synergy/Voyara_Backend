using Microsoft.EntityFrameworkCore.Storage;
using Voyara.Application.Abstractions;

namespace Voyara.Infrastructure.Persistence;

/// <summary>Unit of work backed by <see cref="VoyaraDbContext"/> with explicit transaction control.</summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly VoyaraDbContext _context;
    private IDbContextTransaction? _transaction;

    /// <summary>Creates the unit of work.</summary>
    public UnitOfWork(VoyaraDbContext context) => _context = context;

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

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
    }
}
