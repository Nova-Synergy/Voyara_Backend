using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara_DomainModel.Common;

namespace Voyara_DomainModel.Community.Reviews
{
    internal class Review : AggregateRoot<Guid>
    {

        /// <summary>The maximum number of photos a single review may contain.</summary>
        public const int MaxPhotos = 3;

        private readonly List<ReviewPhoto> _photos = new();

        /// <summary>Id of the tourist who wrote the review.</summary>
        public Guid UserId { get; private set; }

        /// <summary>Id of the reviewed destination.</summary>
        public Guid DestinationId { get; private set; }

        /// <summary>The star rating, from 1 to 5.</summary>
        public int Rating { get; private set; }

        /// <summary>The written review opinion.</summary>
        public string Text { get; private set; }

        /// <summary>The day the tourist visited the destination.</summary>
        public DateOnly VisitDate { get; private set; }

        /// <summary>Where the review currently is in its moderation lifecycle.</summary>
        public ReviewStatus Status { get; private set; }

        /// <summary>When an admin last moderated the review, or <c>null</c> if never.</summary>
        public DateTime? ModeratedAt { get; private set; }

        /// <summary>Reason given when admin rejects user review, otherwise <c>null</c>.</summary>
        public string? RejectionReason { get; private set; }


        /////////////////////////////////////////////
        //public List<ReviewPhoto> Photos { get; private set; } = new();
        // destinationid

        /// <summary>Creates a new review in the <see cref="ReviewStatus.Pending"/> state.</summary>
        /// <param name="userId">The author's id.</param>
        /// <param name="destinationId">The reviewed destination's id.</param>
        /// <param name="rating">Star rating between 1 and 5.</param>
        /// <param name="text">The written opinion (required).</param>
        /// <param name="visitDate">The visit date; it cannot be in the future.</param>
        /// <param name="photoUrls">Optional photo URLs (at most <see cref="MaxPhotos"/>).</param> /////////// VERIFY THIS
        /// <returns>A valid, pending review.</returns>
        public Review(Guid userId, Guid destinationId, int rating, string text, DateOnly visitDate)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException("UserId is required");
            UserId = userId;

            if (destinationId == Guid.Empty)
                throw new ArgumentNullException("DestinationId is required");
            DestinationId = destinationId;

            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");
            Rating = rating;

            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("Review text is required");
            Text = text;

            if (visitDate > DateOnly.FromDateTime(DateTime.UtcNow))
                throw new Exception("Visit date cannot be in the future.");
            VisitDate = visitDate;
        }

        /// <summary>Approves the review. Allowed from Pending or Reported.</summary>
        /// <param name="adminId">The admin making the decision.</param>
        public void Approve(Guid adminId)
        {
            Status = ReviewStatus.Approved;
            ModeratedAt = DateTime.UtcNow;
            RejectionReason = null;
        }

        /// <summary>Rejects a pending review and records why.</summary>
        /// <param name="adminId">The admin making the decision.</param>
        /// <param name="rejectionReason">The explanation shown to the author (required).</param>
        public void Reject(Guid adminId, string rejectionReason)
        {
            Status = ReviewStatus.Rejected;

            if (string.IsNullOrEmpty(rejectionReason))
                throw new ArgumentNullException("Rejection reason is required");
            RejectionReason = rejectionReason.Trim();

            ModeratedAt = DateTime.UtcNow;
        }

        /// <summary>Hides the review from public view. Allowed from Approved or Reported.</summary>
        /// <param name="adminId">The admin hiding the review.</param>
        public void Hide(Guid adminId)
        {
            Status = ReviewStatus.Hidden;
            ModeratedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Flags an approved review as reported. Calling it on an already reported review
        /// does nothing, because many users can report the same review.
        /// </summary>
        public void MarkReported()
        {
            Status = ReviewStatus.Reported;
        }


    }
}
