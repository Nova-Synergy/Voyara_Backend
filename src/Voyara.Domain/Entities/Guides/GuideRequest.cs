using Voyara.Domain.Enums; 
using Voyara.SharedKernel.Entities;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Entities.Catalog;

namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// Represents a tourist's request to book a tour with a guide.
    /// </summary>
    public sealed class GuideRequest : AggregateRoot
    {
        /// <summary>
        /// Gets the requested tour date.
        /// </summary>
        public DateOnly Date { get; private set; }

        /// <summary>
        /// Gets the requested tour start time.
        /// </summary>
        public TimeOnly StartTime { get; private set; }

        /// <summary>
        /// Gets the requested tour duration in hours.
        /// </summary>
        public int DurationHours { get; private set; }

        /// <summary>
        /// Gets the number of tourists included in the request.
        /// </summary>
        public int PartySize { get; private set; }

        /// <summary>
        /// Gets the tourist's preferred language for the tour.
        /// </summary>
        public string PreferredLanguage { get; private set; } = string.Empty;

        /// <summary>
        /// Gets any additional requirements provided by the tourist.
        /// </summary>
        public string? SpecialRequirements { get; private set; }

        /// <summary>
        /// Gets the current status of the guide request.
        /// </summary>
        public RequestStatus Status { get; private set; } = RequestStatus.Pending;

        /// <summary>
        /// Gets the reason provided when the request is rejected, if applicable.
        /// </summary>
        public string? RejectionReason { get; private set; }

        /// <summary>
        /// Gets the expiration date and time of the request.
        /// </summary>
        public DateTimeOffset ExpiresAt { get; private set; }

        /// <summary>
        /// Gets the identifier of the tourist who created the request.
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Gets the identifier of the requested guide profile.
        /// </summary>
        public Guid GuideProfileId { get; private set; }

        /// <summary>
        /// Gets the guide profile associated with this request.
        /// </summary>
        public GuideProfile? GuideProfile { get; private set; }

        /// <summary>
        /// Gets the optional session associated with this request.
        /// </summary>
        public GuideSession? GuideSession { get; private set; }

        /// <summary>
        /// Backing field for <see cref="ChatThreads"/>. Written to only by this aggregate (and by EF Core when loading).
        /// </summary>
        private readonly List<ChatThread> _chatThreads = new();

        /// <summary>
        /// Gets the chat threads associated with this request, as a read-only view.
        /// Threads can only be added through <see cref="OpenChatThread"/>, which requires the request to be accepted.
        /// </summary>
        public IReadOnlyCollection<ChatThread> ChatThreads => _chatThreads.AsReadOnly();

        /// <summary>
        /// Gets the optional review associated with this request.
        /// </summary>
        public GuideReview? GuideReview { get; private set; }

        /// <summary>
        /// Gets the tourist user associated with this request.
        /// </summary>
        public User? User { get; private set; }

        /// <summary>
        /// Gets the destinations associated with this request.
        /// </summary>
        public ICollection<Destination> Destinations { get; private set; } = new List<Destination>();

        /// <summary>
        /// Initializes a new instance of the GuideRequest class. 
        /// </summary>
        /// <remarks>This constructor is private and is intended for internal use only. Instances of
        /// GuideRequest cannot be created directly by consumers.</remarks>
        private GuideRequest()
        {

        }


        /// <summary>
        /// Creates a new guide request with the specified details and validation.  
        /// </summary>
        /// <param name="userId">The unique identifier of the user making the request. Must not be <see cref="Guid.Empty"/>.</param>
        /// <param name="guideProfileId">The unique identifier of the guide profile to which the request is directed. Must not be <see
        /// cref="Guid.Empty"/>.</param>
        /// <param name="date">The date on which the requested guide service is to take place.</param>
        /// <param name="startTime">The local start time for the requested guide service.</param>
        /// <param name="durationHours">The duration of the requested service, in hours. Must be a positive integer.</param>
        /// <param name="partySize">The number of people in the party. Must be a positive integer.</param>
        /// <param name="preferredLanguage">The preferred language for the guide service. Must not be null, empty, or longer than 10 characters.</param>
        /// <param name="specialRequirements">Any special requirements or notes for the guide. May be null or empty, but if provided, must not exceed 1000
        /// characters.</param>
        /// <param name="now">The current date and time, used as the reference point for calculating the expiration time of the request.</param>
        /// <param name="responseWindow">The time window within which a response is expected, relative to <paramref name="now"/>. Must be a positive
        /// duration.</param>
        /// <returns>A new <see cref="GuideRequest"/> instance initialized with the specified parameters and a pending status.</returns>
        /// <exception cref="ArgumentException">Thrown if any parameter is invalid: <paramref name="userId"/> or <paramref name="guideProfileId"/> is <see
        /// cref="Guid.Empty"/>; <paramref name="durationHours"/> or <paramref name="partySize"/> is not positive;
        /// <paramref name="preferredLanguage"/> is null, empty, or longer than 10 characters; <paramref
        /// name="specialRequirements"/> exceeds 1000 characters; or <paramref name="responseWindow"/> is not positive.</exception>
        public static GuideRequest Create(Guid userId, Guid guideProfileId, DateOnly date, TimeOnly startTime,
            int durationHours, int partySize, string preferredLanguage, string? specialRequirements,
            DateTimeOffset now, TimeSpan responseWindow)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("A user is required.", nameof(userId));
            
            if (guideProfileId == Guid.Empty)
                throw new ArgumentException("A guide profile is required.", nameof(guideProfileId));
            
            if (durationHours <= 0) 
                throw new ArgumentException("Duration must be positive.", nameof(durationHours));
            
            if (partySize <= 0) 
                throw new ArgumentException("Party size must be positive.", nameof(partySize));
            
            if (string.IsNullOrWhiteSpace(preferredLanguage) || preferredLanguage.Trim().Length > 10)
                throw new ArgumentException("Preferred language is required (max 10 characters).", nameof(preferredLanguage));
            
            if (specialRequirements is { Length: > 1000 })
                throw new ArgumentException("Special requirements cannot exceed 1000 characters.", nameof(specialRequirements));
            
            if (responseWindow <= TimeSpan.Zero)
                throw new ArgumentException("Response window must be positive.", nameof(responseWindow));

            return new GuideRequest
            {
                UserId = userId,
                GuideProfileId = guideProfileId,
                Date = date,
                StartTime = startTime,
                DurationHours = durationHours,
                PartySize = partySize,
                PreferredLanguage = preferredLanguage.Trim(),
                SpecialRequirements = specialRequirements?.Trim(),
                Status = RequestStatus.Pending,
                ExpiresAt = now + responseWindow
            };
        }

        /// <summary>Accepts a pending request and creates its session within the same aggregate.</summary>
        public GuideSession Accept(string meetingLocation, decimal agreedPrice, string currency, DateTimeOffset now)
        {
            EnsureStatus(RequestStatus.Pending, "accepted");

            if (now >= ExpiresAt)
                throw new InvalidOperationException("The request has expired and can no longer be accepted.");

            var session = GuideSession.Create(Id, meetingLocation, agreedPrice, currency);

            Status = RequestStatus.Accepted;
            GuideSession = session;
            return session;
        }

        /// <summary>
        /// Opens a chat thread for an accepted request.
        /// </summary>
        public ChatThread OpenChatThread()
        {
            if (Status != RequestStatus.Accepted)
                throw new InvalidOperationException("Chat is available only for accepted requests.");

            var thread = ChatThread.Create(Id);
            _chatThreads.Add(thread);
            return thread;
        }

        /// <summary>
        /// Rejects a pending request, recording the reason.
        /// </summary>
        public void Reject(string reason)
        {
            EnsureStatus(RequestStatus.Pending, "rejected");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("A rejection reason is required.", nameof(reason));
            
            if (reason.Trim().Length > 500)
                throw new ArgumentException("Reason cannot exceed 500 characters.", nameof(reason));

            Status = RequestStatus.Rejected;
            RejectionReason = reason.Trim();
        }

        /// <summary>
        /// Cancels a request that is still pending or already accepted.
        /// </summary>
        public void Cancel()
        {
            if (Status is not (RequestStatus.Pending or RequestStatus.Accepted))
                throw new InvalidOperationException($"A request in status '{Status}' cannot be cancelled.");

            Status = RequestStatus.Cancelled;
            CloseChatThreads();

        }

        /// <summary>
        /// Expires a pending request that received no response in time.
        /// </summary>
        public void Expire(DateTimeOffset now)
        {
            EnsureStatus(RequestStatus.Pending, "expired");

            if (now < ExpiresAt)
                throw new InvalidOperationException("The request has not reached its expiry time yet.");

            Status = RequestStatus.Expired;
        }

        /// <summary>
        /// Marks an accepted request as completed after the session took place.
        /// </summary>
        public void Complete()
        {
            EnsureStatus(RequestStatus.Accepted, "completed");
            Status = RequestStatus.Completed;
            CloseChatThreads();
        }

        /// <summary>
        /// Adds the tourist's review once the request is completed. One review per request.
        /// </summary>
        public GuideReview AddReview(int rating, string text)
        {
            EnsureStatus(RequestStatus.Completed, "reviewed");

            if (GuideReview is not null)
                throw new InvalidOperationException("This request has already been reviewed.");

            var review = GuideReview.Create(Id, GuideProfileId, rating, text);
            GuideReview = review;
            return review;
        }

        /// <summary>
        /// Closes the loaded chat threads. The caller must include <see cref="ChatThreads"/> when loading the request.
        /// </summary>
        private void CloseChatThreads()
        {
            foreach (var thread in _chatThreads)
                thread.Close();
        }

        /// <summary>
        /// Ensures that the request is in the required status before performing an action.
        /// </summary>
        /// <param name="required">The required status for the action to be valid.</param>
        /// <param name="action">The action being performed.</param>
        /// <exception cref="InvalidOperationException">Thrown if the request is not in the required status.</exception>
        private void EnsureStatus(RequestStatus required, string action)
        {
            if (Status != required)
                throw new InvalidOperationException($"Only requests in status '{required}' can be {action}. Current status: '{Status}'.");
        }
    }
}

