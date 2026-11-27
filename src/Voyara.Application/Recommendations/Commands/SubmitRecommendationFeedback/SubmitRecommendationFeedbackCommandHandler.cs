using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Entities.AiAssistant;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Recommendations.Commands.SubmitRecommendationFeedback;

/// <summary>
/// Handles recording recommendation feedback for AI model optimization and analytics.
/// </summary>
public sealed class SubmitRecommendationFeedbackCommandHandler
    : IRequestHandler<SubmitRecommendationFeedbackCommand, Result<RecommendationFeedbackResponse>>
{
    private readonly IRecommendationFeedbackRepository _feedbackRepository;
    private readonly IDestinationRepository _destinationRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="SubmitRecommendationFeedbackCommandHandler"/>.
    /// </summary>
    public SubmitRecommendationFeedbackCommandHandler(
        IRecommendationFeedbackRepository feedbackRepository,
        IDestinationRepository destinationRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _feedbackRepository = feedbackRepository;
        _destinationRepository = destinationRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<RecommendationFeedbackResponse>> Handle(
        SubmitRecommendationFeedbackCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        if (userId == Guid.Empty)
        {
            return Result.Failure<RecommendationFeedbackResponse>(
                Error.Unauthorized("User.Unauthorized", "User must be authenticated to submit recommendation feedback."));
        }

        if (request.DestinationId == Guid.Empty)
        {
            return Result.Failure<RecommendationFeedbackResponse>(
                Error.Validation("Feedback.InvalidDestination", "A valid DestinationId is required."));
        }

        if (request.Score < 0.0 || request.Score > 5.0)
        {
            return Result.Failure<RecommendationFeedbackResponse>(
                Error.Validation("Feedback.InvalidScore", "Feedback score must be between 0.0 and 5.0."));
        }

        var destination = await _destinationRepository.GetByIdAsync(request.DestinationId, cancellationToken);
        if (destination is null)
        {
            return Result.Failure<RecommendationFeedbackResponse>(
                Error.NotFound("Destination.NotFound", $"Destination '{request.DestinationId}' was not found."));
        }

        var reason = string.IsNullOrWhiteSpace(request.FeedbackReason)
            ? "User rating feedback"
            : request.FeedbackReason.Trim();

        var log = RecommendationLog.Create(
            userId,
            request.DestinationId,
            request.Score,
            reason);

        _feedbackRepository.Add(log);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RecommendationFeedbackResponse(
            log.Id,
            log.DestinationId,
            log.UserId,
            log.Score,
            log.Reason,
            log.CreatedAt);
    }
}
