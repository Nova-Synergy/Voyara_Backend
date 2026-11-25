
using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Moderation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Moderation.Commands.StartReportInvestigation;

/// <summary>
/// Handles starting an investigation for a moderation report.
/// </summary>
public sealed class StartReportInvestigationCommandHandler
    : IRequestHandler<StartReportInvestigationCommand, Result>
{
    private readonly IReportRepository _reportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="StartReportInvestigationCommandHandler"/> class.
    /// </summary>
    /// <param name="reportRepository">The repository used to retrieve reports.</param>
    /// <param name="unitOfWork">The unit of work used to save changes.</param>
    /// <param name="currentUser">The service providing the current user's identity.</param>
    public StartReportInvestigationCommandHandler(
        IReportRepository reportRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _reportRepository = reportRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Starts an investigation after verifying the current admin and retrieving the report.
    /// </summary>
    /// <param name="request">The command containing the report identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A success result or a failure describing why the operation could not be completed.</returns>
    public async Task<Result> Handle(
        StartReportInvestigationCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not Guid adminId)
        {
            return Result.Failure(
                Error.Unauthorized(
                    "Report.Unauthenticated",
                    "Authentication is required to investigate a report."));
        }

        var report = await _reportRepository.GetByIdAsync(
            request.ReportId,
            cancellationToken);

        if (report is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Report.NotFound",
                    $"Report '{request.ReportId}' was not found."));
        }

        var result = report.StartInvestigation(adminId);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
