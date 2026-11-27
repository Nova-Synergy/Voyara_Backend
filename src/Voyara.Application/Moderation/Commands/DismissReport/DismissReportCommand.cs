
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Moderation.Commands.DismissReport;

/// <summary>
/// Dismisses a moderation report that was found to be unfounded.
/// </summary>
/// <param name="ReportId">The identifier of the report to dismiss.</param>
[AuthorizeRequest(Roles.Admin)]
public sealed record DismissReportCommand(Guid ReportId) : ICommand;
