using Voyara.Domain.Entities.Guides;

namespace Voyara.Domain.Repositories.Guides
{
    /// <summary>A time slot already taken on a guide's calendar by an accepted or completed request.</summary>
    public sealed record GuideRequestSlot(TimeOnly StartTime, int DurationHours);

    /// <summary>
    /// Defines repository operations for the <see cref="GuideRequest"/> aggregate (which owns its session,
    /// chat threads, messages and review).
    /// </summary>
    public interface IGuideRequestRepository
    {
        /// <summary>Adds a new guide request.</summary>
        void Add(GuideRequest request);

        /// <summary>Gets a tracked request together with its chat threads (needed by cancel/complete, which close them).</summary>
        Task<GuideRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>Gets a tracked request together with its review (if any).</summary>
        Task<GuideRequest?> GetByIdWithReviewAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the tracked request that owns the given chat thread, with its guide profile and only that thread loaded.
        /// When <paramref name="messageId"/> is supplied, that single message is loaded into the thread as well.
        /// </summary>
        Task<GuideRequest?> GetByChatThreadIdAsync(Guid chatThreadId, Guid? messageId = null, CancellationToken cancellationToken = default);

        /// <summary>Gets the guide profile identifier of a request without tracking anything.</summary>
        Task<Guid?> GetGuideProfileIdAsync(Guid requestId, CancellationToken cancellationToken = default);

        /// <summary>Returns whether the tourist already has a pending or accepted request for the same guide and slot.</summary>
        Task<bool> ExistsActiveAsync(Guid userId, Guid guideProfileId, DateOnly date, TimeOnly startTime, CancellationToken cancellationToken = default);

        /// <summary>Lists the slots already taken (accepted or completed requests) on a guide's calendar for a date.</summary>
        Task<IReadOnlyList<GuideRequestSlot>> ListTakenSlotsAsync(Guid guideProfileId, DateOnly date, CancellationToken cancellationToken = default);

        /// <summary>Lists tracked pending requests whose expiry time has passed, oldest first.</summary>
        Task<IReadOnlyList<GuideRequest>> ListExpiredPendingAsync(DateTimeOffset now, int batchSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Serialises concurrent schedule-changing operations (accept, review statistics) for one guide until the
        /// surrounding transaction ends. Must be called inside a transaction.
        /// </summary>
        Task AcquireGuideLockAsync(Guid guideProfileId, CancellationToken cancellationToken = default);
    }
}
