using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Moderation;
using Voyara.Domain.Repositories.Moderation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Moderation.Commands.SubmitReport;

/// <summary>
/// Handles submitting a moderation report on behalf of the currently authenticated user,
/// preventing duplicate reports against the same target and persisting the new report.
/// </summary>
public sealed class SubmitReportCommandHandler
    : IRequestHandler<SubmitReportCommand, Result<Guid>>
{
    private readonly IReportRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubmitReportCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository used to check existing reports and persist new report aggregates.</param>
    /// <param name="unitOfWork">The unit of work used to commit changes to the database.</param>
    /// <param name="currentUser">The service providing information about the currently authenticated user.</param>
    public SubmitReportCommandHandler(
        IReportRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Processes the <see cref="SubmitReportCommand"/> to verify authentication,
    /// prevent duplicate reports, instantiate a <see cref="Report"/> aggregate, and persist it.
    /// </summary>
    /// <param name="request">The command containing the report target details, reason, and optional comment.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the identifier of the newly created <see cref="Report"/> on success;
    /// otherwise, a failure result with an unauthorized, conflict, or domain validation error.
    /// </returns>
    public async Task<Result<Guid>> Handle(
        SubmitReportCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not Guid reporterId)
        {
            return Result.Failure<Guid>(
                Error.Unauthorized(
                    "Report.Unauthenticated",
                    "Authentication is required to submit a report."));
        }

        var alreadyReported =
            await _repository.ExistsByReporterAndTargetAsync(
                reporterId,
                request.TargetType,
                request.TargetId,
                cancellationToken);

        if (alreadyReported)
        {
            return Result.Failure<Guid>(ReportErrors.AlreadyReported);
        }

        var reportResult = Report.Create(
            reporterId,
            request.TargetType,
            request.TargetId,
            request.Reason,
            request.Comment);

        if (reportResult.IsFailure)
        {
            return Result.Failure<Guid>(reportResult.Error);
        }

        var report = reportResult.Value;

        _repository.Add(report);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(report.Id);
    }
}