
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.Moderation.DTOs;
using Voyara.Domain.Enums;

namespace Voyara.Application.Moderation.Queries.GetReportsByStatus;

/// <summary>
/// Requests all moderation reports matching a specified status.
/// </summary>
/// <param name="Status">The moderation status used to filter reports.</param>
[AuthorizeRequest(Roles.Admin)]
public sealed record GetReportsByStatusQuery(ReportStatus Status)
    : IQuery<IReadOnlyList<ReportResponse>>;
