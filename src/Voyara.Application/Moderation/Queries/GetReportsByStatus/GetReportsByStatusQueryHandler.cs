
using MediatR;
using Voyara.Application.Moderation.DTOs;
using Voyara.Domain.Repositories.Moderation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Moderation.Queries.GetReportsByStatus;

/// <summary>
/// Handles retrieving moderation reports filtered by their current status.
/// </summary>
public sealed class GetReportsByStatusQueryHandler
    : IRequestHandler<
        GetReportsByStatusQuery,
        Result<IReadOnlyList<ReportResponse>>>
{
    private readonly IReportRepository _reportRepository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GetReportsByStatusQueryHandler"/> class.
    /// </summary>
    /// <param name="reportRepository">
    /// The repository used to retrieve moderation reports.
    /// </param>
    public GetReportsByStatusQueryHandler(
        IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    /// <summary>
    /// Retrieves reports matching the requested status and converts them into response DTOs.
    /// </summary>
    /// <param name="request">The query containing the requested report status.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A read-only list of matching report responses.</returns>
    public async Task<Result<IReadOnlyList<ReportResponse>>> Handle(
        GetReportsByStatusQuery request,
        CancellationToken cancellationToken)
    {
        var reports = await _reportRepository.GetByStatusAsync(
            request.Status,
            cancellationToken);

        var responses = reports
            .Select(ReportResponse.FromEntity)
            .ToList();

        return responses;
    }
}
