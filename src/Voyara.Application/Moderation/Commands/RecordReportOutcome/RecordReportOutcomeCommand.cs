
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Moderation.Commands.RecordReportOutcome;

/// <summary>
/// Records the outcome of a moderation action and closes the report.
/// </summary>
/// <param name="ReportId">The identifier of the report whose outcome is being recorded.</param>
[AuthorizeRequest(Roles.Admin)]
public sealed record RecordReportOutcomeCommand(Guid ReportId) : ICommand;
