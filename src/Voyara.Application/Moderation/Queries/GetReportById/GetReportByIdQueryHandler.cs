
using MediatR;
using Voyara.Application.Moderation.DTOs;
using Voyara.Domain.Repositories.Moderation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Moderation.Queries.GetReportById;

/// <summary>
/// Handles retrieving a moderation report by its unique identifier.
/// </summary>
public sealed class GetReportByIdQueryHandler
    : IRequestHandler<GetReportByIdQuery, Result<ReportResponse>>
{
    private readonly IReportRepository _reportRepository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GetReportByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="reportRepository">
    /// The repository used to retrieve moderation reports.
    /// </param>
    public GetReportByIdQueryHandler(
        IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    /// <summary>
    /// Retrieves a report and converts it into a response DTO.
    /// </summary>
    /// <param name="request">The query containing the report identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// The report response if found; otherwise, a not-found failure result.
    /// </returns>
    public async Task<Result<ReportResponse>> Handle(
        GetReportByIdQuery request,
        CancellationToken cancellationToken)
    {
        var report = await _reportRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (report is null)
        {
            return Result.Failure<ReportResponse>(
                Error.NotFound(
                    "Report.NotFound",
                    $"Report '{request.Id}' was not found."));
        }

        return ReportResponse.FromEntity(report);
    }
}
