using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Domain.Repositories.Guides
{
    /// <summary>
    /// Defines repository operations for retrieving and managing guide requests,
    /// guide reviews, and chat messages associated with guide requests.
    /// </summary>
    public interface IGuideRequestRepository
    {
        /// <summary>
        /// Retrieves guide requests submitted by a specific tourist,
        /// optionally filtered by request status.
        /// </summary>
        /// <param name="userId">The unique identifier of the tourist.</param>
        /// <param name="status">An optional status used to filter the requests.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of guide requests submitted by the tourist.</returns>
        Task<IReadOnlyList<Voyara.Domain.Entities.Guides.GuideRequest>> GetForTouristAsync(Guid userId, Voyara.Domain.Enums.RequestStatus? status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated collection of guide requests submitted by a specific tourist,
        /// optionally filtered by request status.
        /// </summary>
        /// <param name="userId">The unique identifier of the tourist.</param>
        /// <param name="status">An optional status used to filter the requests.</param>
        /// <param name="skip">The number of requests to skip.</param>
        /// <param name="take">The maximum number of requests to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A paginated collection of the tourist's guide requests.</returns>
        Task<IReadOnlyList<Voyara.Domain.Entities.Guides.GuideRequest>> GetForTouristAsync(Guid userId, Voyara.Domain.Enums.RequestStatus? status, int skip, int take, CancellationToken cancellationToken = default);

        /// <summary>
        /// Counts the guide requests submitted by a specific tourist,
        /// optionally filtered by request status.
        /// </summary>
        /// <param name="userId">The unique identifier of the tourist.</param>
        /// <param name="status">An optional status used to filter the requests.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The total number of matching guide requests.</returns>
        Task<int> CountForTouristAsync(Guid userId, Voyara.Domain.Enums.RequestStatus? status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves guide requests assigned to a specific guide profile,
        /// optionally filtered by request status.
        /// </summary>
        /// <param name="guideProfileId">The unique identifier of the guide profile.</param>
        /// <param name="status">An optional status used to filter the requests.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of requests assigned to the guide.</returns>
        Task<IReadOnlyList<Voyara.Domain.Entities.Guides.GuideRequest>> GetForGuideAsync(Guid guideProfileId, Voyara.Domain.Enums.RequestStatus? status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated collection of guide requests assigned to a specific guide profile,
        /// optionally filtered by request status.
        /// </summary>
        /// <param name="guideProfileId">The unique identifier of the guide profile.</param>
        /// <param name="status">An optional status used to filter the requests.</param>
        /// <param name="skip">The number of requests to skip.</param>
        /// <param name="take">The maximum number of requests to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A paginated collection of requests assigned to the guide.</returns>
        Task<IReadOnlyList<Voyara.Domain.Entities.Guides.GuideRequest>> GetForGuideAsync(Guid guideProfileId, Voyara.Domain.Enums.RequestStatus? status, int skip, int take, CancellationToken cancellationToken = default);

        /// <summary>
        /// Counts the requests assigned to a specific guide profile,
        /// optionally filtered by request status.
        /// </summary>
        /// <param name="guideProfileId">The unique identifier of the guide profile.</param>
        /// <param name="status">An optional status used to filter the requests.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The total number of matching guide requests.</returns>
        Task<int> CountForGuideAsync(Guid guideProfileId, Voyara.Domain.Enums.RequestStatus? status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a guide request by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the guide request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The matching guide request, or null if it does not exist.</returns>
        Task<Voyara.Domain.Entities.Guides.GuideRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a guide request with the information required to process its review.
        /// </summary>
        /// <param name="id">The unique identifier of the guide request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The guide request prepared for review, or null if it does not exist.</returns>
        Task<Voyara.Domain.Entities.Guides.GuideRequest?> GetForReviewAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a guide request associated with a specific chat thread.
        /// </summary>
        /// <param name="threadId">The unique identifier of the chat thread.</param>
        /// <param name="includeMessages">Indicates whether the chat thread's messages should be included.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The associated guide request, or null if no matching request exists.</returns>
        Task<Voyara.Domain.Entities.Guides.GuideRequest?> GetByChatThreadIdAsync(Guid threadId, bool includeMessages, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all reviews associated with a specific guide profile.
        /// </summary>
        /// <param name="guideProfileId">The unique identifier of the guide profile.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of reviews submitted for the guide.</returns>
        Task<IReadOnlyList<Voyara.Domain.Entities.Guides.GuideReview>> GetReviewsForGuideAsync(Guid guideProfileId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated collection of reviews associated with a specific guide profile.
        /// </summary>
        /// <param name="guideProfileId">The unique identifier of the guide profile.</param>
        /// <param name="skip">The number of reviews to skip.</param>
        /// <param name="take">The maximum number of reviews to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A paginated collection of reviews for the guide.</returns>
        Task<IReadOnlyList<Voyara.Domain.Entities.Guides.GuideReview>> GetReviewsForGuideAsync(Guid guideProfileId, int skip, int take, CancellationToken cancellationToken = default);

        /// <summary>
        /// Counts the reviews associated with a specific guide profile.
        /// </summary>
        /// <param name="guideProfileId">The unique identifier of the guide profile.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The total number of reviews for the guide.</returns>
        Task<int> CountReviewsForGuideAsync(Guid guideProfileId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated collection of messages belonging to a specific chat thread.
        /// </summary>
        /// <param name="threadId">The unique identifier of the chat thread.</param>
        /// <param name="skip">The number of messages to skip.</param>
        /// <param name="take">The maximum number of messages to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A paginated collection of chat messages.</returns>
        Task<IReadOnlyList<Voyara.Domain.Entities.Guides.ChatMessage>> GetMessagesForThreadAsync(Guid threadId, int skip, int take, CancellationToken cancellationToken = default);

        /// <summary>
        /// Counts the messages belonging to a specific chat thread.
        /// </summary>
        /// <param name="threadId">The unique identifier of the chat thread.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The total number of messages in the chat thread.</returns>
        Task<int> CountMessagesForThreadAsync(Guid threadId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new guide request to the repository.
        /// </summary>
        /// <param name="request">The guide request to add.</param>
        void Add(Voyara.Domain.Entities.Guides.GuideRequest request);
    }
}