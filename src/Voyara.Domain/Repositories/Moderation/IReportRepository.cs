
using Voyara.Domain.Entities.Moderation;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Repositories.Moderation;

/// <summary>
/// Defines persistence operations for managing <see cref="Report"/> aggregates.
/// </summary>
public interface IReportRepository
{
    /// <summary>
    /// Retrieves a report by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the report.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The report if found; otherwise, <c>null</c>.</returns>
    Task<Report?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a read-only list of reports matching the specified status asynchronously.
    /// </summary>
    /// <param name="status">The moderation status to filter by.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A read-only list of reports matching the given status.</returns>
    Task<IReadOnlyList<Report>> GetByStatusAsync(
        ReportStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a report already exists for the specified reporter and target entity asynchronously.
    /// </summary>
    /// <param name="reporterId">The unique identifier of the user who submitted the report.</param>
    /// <param name="targetType">The type of entity being reported.</param>
    /// <param name="targetId">The unique identifier of the entity being reported.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns><c>true</c> if a matching report exists; otherwise, <c>false</c>.</returns>
    Task<bool> ExistsByReporterAndTargetAsync(
        Guid reporterId,
        ReportTargetType targetType,
        Guid targetId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new report aggregate to the repository tracking context.
    /// </summary>
    /// <param name="report">The report entity to add.</param>
    void Add(Report report);
}
