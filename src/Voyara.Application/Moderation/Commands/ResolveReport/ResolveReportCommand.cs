
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Domain.Enums;

namespace Voyara.Application.Moderation.Commands.ResolveReport;

/// <summary>
/// Records the moderation action taken against the target of a report.
/// </summary>
/// <param name="ReportId">The identifier of the report being handled.</param>
/// <param name="Action">The moderation action taken against the reported target.</param>
[AuthorizeRequest(Roles.Admin)]
public sealed record ResolveReportCommand(
    Guid ReportId,
    ModerationAction Action) : ICommand;
