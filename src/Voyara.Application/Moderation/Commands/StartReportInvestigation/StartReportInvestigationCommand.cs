
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Moderation.Commands.StartReportInvestigation;

/// <summary>
/// Starts an investigation for a submitted moderation report.
/// </summary>
/// <param name="ReportId">The identifier of the report to investigate.</param>
[AuthorizeRequest(Roles.Admin)]
public sealed record StartReportInvestigationCommand(Guid ReportId) : ICommand;
