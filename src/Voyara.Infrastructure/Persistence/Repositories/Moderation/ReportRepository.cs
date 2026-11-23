using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Moderation;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Moderation;

namespace Voyara.Infrastructure.Persistence.Repositories.Moderation;

/// <summary>
/// Provides the Entity Framework Core implementation of the <see cref="IReportRepository"/> interface.
/// </summary>
public sealed class ReportRepository : IReportRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for persistence operations.</param>
    public ReportRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<Report?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Reports
            .FirstOrDefaultAsync(
                report => report.Id == id,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Report>> GetByStatusAsync(
        ReportStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .AsNoTracking()
            .Where(report => report.Status == status)
            .OrderBy(report => report.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ExistsByReporterAndTargetAsync(
        Guid reporterId,
        ReportTargetType targetType,
        Guid targetId,
        CancellationToken cancellationToken = default)
    {
        return _context.Reports
            .AnyAsync(
                report =>
                    report.ReporterId == reporterId &&
                    report.Target.Type == targetType &&
                    report.Target.Id == targetId,
                cancellationToken);
    }

    /// <inheritdoc />
    public void Add(Report report)
    {
        _context.Reports.Add(report);
    }
}