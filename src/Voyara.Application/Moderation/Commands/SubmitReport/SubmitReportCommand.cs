
using Voyara.Application.Abstractions.Messaging;
using Voyara.Domain.Enums;

namespace Voyara.Application.Moderation.Commands.SubmitReport;

/// <summary>
/// Submits a report against content, a destination, a guide, or a user account.
/// </summary>
/// <param name="TargetType">The type of object being reported.</param>
/// <param name="TargetId">The identifier of the object being reported.</param>
/// <param name="Reason">The reason for submitting the report.</param>
/// <param name="Comment">Optional additional information about the report.</param>
public sealed record SubmitReportCommand(
    ReportTargetType TargetType,
    Guid TargetId,
    string Reason,
    string? Comment = null) : ICommand<Guid>;
