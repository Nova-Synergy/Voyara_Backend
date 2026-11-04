using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Guides;

namespace Voyara.Infrastructure.Persistence.Repositories.Guides;

/// <summary>
/// Provides EF Core persistence operations for the <see cref="GuideRequest"/> aggregate.
/// </summary>
public sealed class GuideRequestRepository : IGuideRequestRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuideRequestRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used to access guide request data.</param>
    public GuideRequestRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all guide requests submitted by a specific tourist, optionally filtered by status.
    /// </summary>
    /// <param name="userId">The unique identifier of the tourist.</param>
    /// <param name="status">The optional request status used to filter the results.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list containing the tourist's guide requests.</returns>
    public Task<IReadOnlyList<GuideRequest>> GetForTouristAsync(Guid userId, RequestStatus? status, CancellationToken cancellationToken = default)
        => GetForTouristAsync(userId, status, 0, int.MaxValue, cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of guide requests submitted by a specific tourist,
    /// optionally filtered by status.
    /// </summary>
    /// <param name="userId">The unique identifier of the tourist.</param>
    /// <param name="status">The optional request status used to filter the results.</param>
    /// <param name="skip">The number of records to skip before retrieving results.</param>
    /// <param name="take">The maximum number of records to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list containing the matching guide requests.</returns>
    public async Task<IReadOnlyList<GuideRequest>> GetForTouristAsync(Guid userId, RequestStatus? status, int skip, int take, CancellationToken cancellationToken = default)
        => await QueryWithDetails()
            .Where(request => request.UserId == userId)
            .Where(request => status == null || request.Status == status)
            .OrderByDescending(request => request.Date)
            .ThenByDescending(request => request.StartTime)
            .ThenBy(request => request.Id)
            .Skip(Math.Max(0, skip))
            .Take(Math.Max(1, take))
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Counts the guide requests submitted by a specific tourist,
    /// optionally filtered by status.
    /// </summary>
    /// <param name="userId">The unique identifier of the tourist.</param>
    /// <param name="status">The optional request status used to filter the count.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The number of matching guide requests.</returns>
    public Task<int> CountForTouristAsync(Guid userId, RequestStatus? status, CancellationToken cancellationToken = default)
        => _context.Set<GuideRequest>()
            .Where(request => request.UserId == userId)
            .Where(request => status == null || request.Status == status)
            .CountAsync(cancellationToken);

    /// <summary>
    /// Retrieves all guide requests assigned to a specific guide, optionally filtered by status.
    /// </summary>
    /// <param name="guideProfileId">The unique identifier of the guide profile.</param>
    /// <param name="status">The optional request status used to filter the results.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list containing the guide's requests.</returns>
    public Task<IReadOnlyList<GuideRequest>> GetForGuideAsync(Guid guideProfileId, RequestStatus? status, CancellationToken cancellationToken = default)
        => GetForGuideAsync(guideProfileId, status, 0, int.MaxValue, cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of guide requests assigned to a specific guide,
    /// optionally filtered by status.
    /// </summary>
    /// <param name="guideProfileId">The unique identifier of the guide profile.</param>
    /// <param name="status">The optional request status used to filter the results.</param>
    /// <param name="skip">The number of records to skip before retrieving results.</param>
    /// <param name="take">The maximum number of records to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list containing the matching guide requests.</returns>
    public async Task<IReadOnlyList<GuideRequest>> GetForGuideAsync(Guid guideProfileId, RequestStatus? status, int skip, int take, CancellationToken cancellationToken = default)
        => await QueryWithDetails()
            .Where(request => request.GuideProfileId == guideProfileId)
            .Where(request => status == null || request.Status == status)
            .OrderByDescending(request => request.Date)
            .ThenByDescending(request => request.StartTime)
            .ThenBy(request => request.Id)
            .Skip(Math.Max(0, skip))
            .Take(Math.Max(1, take))
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Counts the guide requests assigned to a specific guide,
    /// optionally filtered by status.
    /// </summary>
    /// <param name="guideProfileId">The unique identifier of the guide profile.</param>
    /// <param name="status">The optional request status used to filter the count.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The number of matching guide requests.</returns>
    public Task<int> CountForGuideAsync(Guid guideProfileId, RequestStatus? status, CancellationToken cancellationToken = default)
        => _context.Set<GuideRequest>()
            .Where(request => request.GuideProfileId == guideProfileId)
            .Where(request => status == null || request.Status == status)
            .CountAsync(cancellationToken);

    /// <summary>
    /// Retrieves a guide request by its identifier, including its related details.
    /// </summary>
    /// <param name="id">The unique identifier of the guide request.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// The matching guide request with its related details,
    /// or <see langword="null"/> if no request is found.
    /// </returns>
    public Task<GuideRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => QueryWithDetails()
            .Include(request => request.GuideProfile)
            .FirstOrDefaultAsync(request => request.Id == id, cancellationToken);

    /// <summary>
    /// Retrieves a guide request with its associated guide profile and review,
    /// for review-related operations.
    /// </summary>
    /// <param name="id">The unique identifier of the guide request.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// The matching guide request with its guide profile and review,
    /// or <see langword="null"/> if no request is found.
    /// </returns>
    public Task<GuideRequest?> GetForReviewAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Set<GuideRequest>()
            .Include(request => request.GuideProfile)
            .Include(request => request.GuideReview)
            .FirstOrDefaultAsync(request => request.Id == id, cancellationToken);

    /// <summary>
    /// Retrieves the guide request associated with a specific chat thread.
    /// Optionally includes the messages belonging to the chat thread.
    /// </summary>
    /// <param name="threadId">The unique identifier of the chat thread.</param>
    /// <param name="includeMessages">
    /// Indicates whether the chat thread's messages should be included.
    /// </param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// The associated guide request, or <see langword="null"/> if no matching request is found.
    /// </returns>
    public Task<GuideRequest?> GetByChatThreadIdAsync(Guid threadId, bool includeMessages, CancellationToken cancellationToken = default)
    {
        IQueryable<GuideRequest> query = _context.Set<GuideRequest>()
            .Include(request => request.GuideProfile)
            .Include(request => request.ChatThreads)
            .Where(request => request.ChatThreads.Any(thread => thread.Id == threadId));

        if (includeMessages)
        {
            query = query.Include(request => request.ChatThreads)
                .ThenInclude(thread => thread.Messages);
        }

        return query.AsSplitQuery().FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all reviews submitted for a specific guide.
    /// </summary>
    /// <param name="guideProfileId">The unique identifier of the guide profile.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list containing the guide's reviews.</returns>
    public Task<IReadOnlyList<GuideReview>> GetReviewsForGuideAsync(Guid guideProfileId, CancellationToken cancellationToken = default)
        => GetReviewsForGuideAsync(guideProfileId, 0, int.MaxValue, cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of reviews submitted for a specific guide.
    /// </summary>
    /// <param name="guideProfileId">The unique identifier of the guide profile.</param>
    /// <param name="skip">The number of records to skip before retrieving results.</param>
    /// <param name="take">The maximum number of records to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list containing the matching reviews.</returns>
    public async Task<IReadOnlyList<GuideReview>> GetReviewsForGuideAsync(Guid guideProfileId, int skip, int take, CancellationToken cancellationToken = default)
        => await _context.Set<GuideReview>()
            .AsNoTracking()
            .Where(review => review.GuideProfileId == guideProfileId)
            .OrderByDescending(review => review.CreatedAt)
            .ThenBy(review => review.Id)
            .Skip(Math.Max(0, skip))
            .Take(Math.Max(1, take))
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Counts the reviews submitted for a specific guide.
    /// </summary>
    /// <param name="guideProfileId">The unique identifier of the guide profile.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total number of reviews for the specified guide.</returns>
    public Task<int> CountReviewsForGuideAsync(Guid guideProfileId, CancellationToken cancellationToken = default)
        => _context.Set<GuideReview>()
            .Where(review => review.GuideProfileId == guideProfileId)
            .CountAsync(cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of messages belonging to a specific chat thread.
    /// </summary>
    /// <param name="threadId">The unique identifier of the chat thread.</param>
    /// <param name="skip">The number of messages to skip before retrieving results.</param>
    /// <param name="take">The maximum number of messages to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list containing the matching chat messages.</returns>
    public async Task<IReadOnlyList<ChatMessage>> GetMessagesForThreadAsync(Guid threadId, int skip, int take, CancellationToken cancellationToken = default)
        => await _context.Set<ChatMessage>()
            .AsNoTracking()
            .Where(message => message.ChatThreadId == threadId)
            .OrderBy(message => message.SentAt)
            .ThenBy(message => message.Id)
            .Skip(Math.Max(0, skip))
            .Take(Math.Max(1, take))
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Counts the messages belonging to a specific chat thread.
    /// </summary>
    /// <param name="threadId">The unique identifier of the chat thread.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total number of messages in the specified chat thread.</returns>
    public Task<int> CountMessagesForThreadAsync(Guid threadId, CancellationToken cancellationToken = default)
        => _context.Set<ChatMessage>()
            .Where(message => message.ChatThreadId == threadId)
            .CountAsync(cancellationToken);

    /// <summary>
    /// Adds a new guide request to the current DbContext for persistence.
    /// </summary>
    /// <param name="request">The guide request to add.</param>
    public void Add(GuideRequest request) => _context.Set<GuideRequest>().Add(request);

    /// <summary>
    /// Creates a query for guide requests with their commonly required related details.
    /// </summary>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of guide requests including their guide session,
    /// chat threads, and guide review.
    /// </returns>
    private IQueryable<GuideRequest> QueryWithDetails()
        => _context.Set<GuideRequest>()
            .AsNoTracking()
            .Include(request => request.GuideSession)
            .Include(request => request.ChatThreads)
            .Include(request => request.GuideReview)
            .AsSplitQuery();
}