using Voyara.Domain.Entities.AiAssistant;

namespace Voyara.Application.Abstractions;

/// <summary>
/// Persistence contract for recording recommendation feedback and audit logs.
/// </summary>
public interface IRecommendationFeedbackRepository
{
    /// <summary>
    /// Registers a recommendation feedback log entry for insertion.
    /// </summary>
    /// <param name="log">The recommendation log to record.</param>
    void Add(RecommendationLog log);

    /// <summary>
    /// Retrieves recorded feedback logs for a given destination.
    /// </summary>
    /// <param name="destinationId">Destination identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of recommendation log entries.</returns>
    Task<IReadOnlyList<RecommendationLog>> GetByDestinationIdAsync(Guid destinationId, CancellationToken cancellationToken = default);
}
