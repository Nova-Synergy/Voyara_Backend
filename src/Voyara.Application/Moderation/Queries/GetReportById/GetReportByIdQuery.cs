
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.Moderation.DTOs;

namespace Voyara.Application.Moderation.Queries.GetReportById;

/// <summary>
/// Requests a moderation report by its unique identifier.
/// </summary>
/// <param name="Id">The identifier of the report to retrieve.</param>
[AuthorizeRequest(Roles.Admin)]
public sealed record GetReportByIdQuery(Guid Id)
    : IQuery<ReportResponse>;
