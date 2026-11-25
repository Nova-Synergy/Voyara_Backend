
using Voyara.Domain.Entities.Moderation;
using Voyara.Domain.Enums;

namespace Voyara.Application.Moderation.DTOs;

/// <summary>
/// Represents the information returned when retrieving a moderation report.
/// </summary>
/// <param name="Id">The unique identifier of the report.</param>
/// <param name="ReporterId">The identifier of the user who submitted the report.</param>
/// <param name="TargetType">The type of object being reported.</param>
/// <param name="TargetId">The identifier of the reported object.</param>
/// <param name="Reason">The reason provided for the report.</param>
/// <param name="Comment">Optional additional information provided by the reporter.</param>
/// <param name="Status">The current moderation status of the report.</param>
/// <param name="ActionTaken">The moderation action taken, if any.</param>
/// <param name="ResolvedBy">The identifier of the admin who resolved the report, if resolved.</param>
/// <param name="IsResolved">Whether the report has reached its final state.</param>
public sealed record ReportResponse(
    Guid Id,
    Guid ReporterId,
    ReportTargetType TargetType,
    Guid TargetId,
    string Reason,
    string? Comment,
    ReportStatus Status,
    ModerationAction? ActionTaken,
    Guid? ResolvedBy,
    bool IsResolved)
{
    /// <summary>
    /// Creates a response DTO from a moderation report aggregate.
    /// </summary>
    /// <param name="report">The report to convert.</param>
    /// <returns>A response containing the report's current data.</returns>
    public static ReportResponse FromEntity(Report report)
    {
        return new ReportResponse(
            report.Id,
            report.ReporterId,
            report.TargetType,
            report.TargetId,
            report.Reason,
            report.Comment,
            report.Status,
            report.ActionTaken,
            report.ResolvedBy,
            report.IsResolved);
    }
}
