namespace Voyara.Application.Abstractions;

/// <summary>
/// Transaction and persistence boundary controlled by the Application layer;
/// implemented by infrastructure without exposing ORM types.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Persists tracked changes within the current transaction.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>Begins an explicit transaction.</summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Commits the current transaction.</summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Rolls back the current transaction.</summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
