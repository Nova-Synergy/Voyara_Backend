using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Entities.Identity;

namespace Voyara.Domain.Entities.AiAssistant;

/// <summary>
/// Represents an audit log entry of a personalized destination recommendation produced by the AI engine.
/// </summary>
/// <remarks>
/// Tracks algorithmic scoring and explainability rationale presented to a tourist, supporting model evaluation and recommendation feedback loops.
/// </remarks>
public sealed class RecommendationLog
{
    /// <summary>
    /// Gets the unique 64-bit integer identifier of the recommendation record.
    /// </summary>
    /// <value>The database-generated primary key.</value>
    public long Id { get; private set; }

    /// <summary>
    /// Gets the relevance or compatibility score computed for the recommendation.
    /// </summary>
    /// <value>A numerical score representing match affinity.</value>
    public double Score { get; private set; }

    /// <summary>
    /// Gets the natural-language rationale or justification produced by the AI model.
    /// </summary>
    /// <value>A human-readable explanation of why this destination was recommended.</value>
    public string Reason { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the identifier of the tourist user for whom the recommendation was generated.
    /// </summary>
    /// <value>The foreign key pointing to the target <see cref="User"/>.</value>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the navigation reference to the user.
    /// </summary>
    /// <value>The owning <see cref="User"/> entity.</value>
    public User? User { get; private set; }

    /// <summary>
    /// Gets the identifier of the destination that was recommended.
    /// </summary>
    /// <value>The foreign key pointing to the recommended <see cref="Destination"/>.</value>
    public Guid DestinationId { get; private set; }

    /// <summary>
    /// Gets the navigation reference to the recommended destination.
    /// </summary>
    /// <value>The recommended <see cref="Destination"/> entity.</value>
    public Destination? Destination { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the recommendation was generated and logged.
    /// </summary>
    /// <value>The date and time of the recommendation.</value>
    public DateTime CreatedAt { get; private set; }

    private RecommendationLog()
    {
    }

    private RecommendationLog(Guid userId, Guid destinationId, double score, string reason)
    {
        UserId = userId;
        DestinationId = destinationId;
        Score = score;
        Reason = reason;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new <see cref="RecommendationLog"/> entry.
    /// </summary>
    /// <param name="userId">The identifier of the user who received the recommendation.</param>
    /// <param name="destinationId">The identifier of the recommended destination.</param>
    /// <param name="score">The computed relevance or affinity score.</param>
    /// <param name="reason">The explanation justifying the recommendation.</param>
    /// <returns>A new <see cref="RecommendationLog"/> instance.</returns>
    public static RecommendationLog Create(Guid userId, Guid destinationId, double score, string reason)
    {
        return new RecommendationLog(
            userId,
            destinationId,
            score,
            reason ?? string.Empty);
    }
}

