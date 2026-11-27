using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Recommendations.Commands.SubmitRecommendationFeedback;

/// <summary>
/// Command to submit tourist feedback or ratings on a recommended destination.
/// </summary>
/// <param name="DestinationId">The recommended destination identifier.</param>
/// <param name="Score">Affinity rating or satisfaction score (0.0 to 5.0).</param>
/// <param name="FeedbackReason">Optional feedback rationale or comments.</param>
public sealed record SubmitRecommendationFeedbackCommand(
    Guid DestinationId,
    double Score,
    string? FeedbackReason = null) : ICommand<RecommendationFeedbackResponse>;
