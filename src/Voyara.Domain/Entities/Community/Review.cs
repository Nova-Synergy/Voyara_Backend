using Voyara.Domain.Enums;
using Voyara.Domain.Events.Community;
using Voyara.SharedKernel.Entities;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Community;

/// <summary>
/// Aggregate root representing a tourist's review of a destination.
/// Controls review creation, moderation, and attached photos.
/// </summary>
public sealed class Review : AggregateRoot
{
    /// <summary>Maximum number of photos allowed on a review.</summary>
    public const int MaxPhotos = 5;

    private readonly List<ReviewPhoto> _photos = [];

    /// <summary>Identifier of the review author.</summary>
    public Guid UserId { get; private set; }

    /// <summary>Identifier of the reviewed destination.</summary>
    public Guid DestinationId { get; private set; }

    /// <summary>Rating from one to five stars.</summary>
    public int Rating { get; private set; }

    /// <summary>Written review text.</summary>
    public string Text { get; private set; } = string.Empty;

    /// <summary>Date the tourist visited the destination.</summary>
    public DateOnly VisitDate { get; private set; }

    /// <summary>Current moderation status.</summary>
    public ReviewStatus Status { get; private set; } = ReviewStatus.Pending;

    /// <summary>Time of the most recent moderation decision.</summary>
    public DateTimeOffset? ModeratedAt { get; private set; }

    /// <summary>Reason the review was rejected, if applicable.</summary>
    public string? RejectionReason { get; private set; }

    /// <summary>Photos managed by this review aggregate.</summary>
    public IReadOnlyCollection<ReviewPhoto> Photos => _photos.AsReadOnly();

    private Review() { }

    private Review(
        Guid userId,
        Guid destinationId,
        int rating,
        string text,
        DateOnly visitDate)
    {
        UserId = userId;
        DestinationId = destinationId;
        Rating = rating;
        Text = text;
        VisitDate = visitDate;
        Status = ReviewStatus.Pending;
    }

    /// <summary>Creates a review after validating its required fields.</summary>
    public static Result<Review> Create(
        Guid userId,
        Guid destinationId,
        int rating,
        string text,
        DateOnly visitDate)
    {
        if (userId == Guid.Empty)
            return Result.Failure<Review>(ReviewErrors.UserRequired);

        if (destinationId == Guid.Empty)
            return Result.Failure<Review>(ReviewErrors.DestinationRequired);

        if (rating is < 1 or > 5)
            return Result.Failure<Review>(ReviewErrors.InvalidRating);

        if (string.IsNullOrWhiteSpace(text))
            return Result.Failure<Review>(ReviewErrors.TextRequired);

        if (visitDate > DateOnly.FromDateTime(DateTime.UtcNow))
            return Result.Failure<Review>(ReviewErrors.VisitDateInFuture);

        var review = new Review(
            userId, destinationId, rating, text.Trim(), visitDate);

        review.RaiseDomainEvent(new ReviewSubmittedDomainEvent(
            Guid.NewGuid(), DateTimeOffset.UtcNow, review.Id,
            review.UserId, review.DestinationId));

        return review;
    }

    /// <summary>Adds a photo to the review, subject to the photo limit.</summary>
    public Result AddPhoto(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Result.Failure(ReviewErrors.InvalidPhotoUrl);

        if (_photos.Count >= MaxPhotos)
            return Result.Failure(ReviewErrors.TooManyPhotos);

        _photos.Add(ReviewPhoto.Create(Id, url));
        return Result.Success();
    }

    /// <summary>Removes a photo owned by this review.</summary>
    public Result RemovePhoto(Guid photoId)
    {
        var photo = _photos.FirstOrDefault(p => p.Id == photoId);

        if (photo is null)
            return Result.Failure(ReviewErrors.PhotoNotFound);

        _photos.Remove(photo);
        return Result.Success();
    }

    /// <summary>Approves a pending review or clears a report on an approved review.</summary>
    public Result Approve(Guid adminId)
    {
        if (adminId == Guid.Empty)
            return Result.Failure(ReviewErrors.AdminRequired);

        if (Status is not (ReviewStatus.Pending or ReviewStatus.Reported))
            return Result.Failure(ReviewErrors.InvalidTransition("approve", Status));

        Status = ReviewStatus.Approved;
        RejectionReason = null;
        ModeratedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new ReviewApprovedDomainEvent(
            Guid.NewGuid(), ModeratedAt.Value, Id, UserId, DestinationId, adminId));

        return Result.Success();
    }

    /// <summary>Rejects a pending review or a reported review, recording the reason.</summary>
    public Result Reject(Guid adminId, string reason)
    {
        if (adminId == Guid.Empty)
            return Result.Failure(ReviewErrors.AdminRequired);

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(ReviewErrors.RejectionReasonRequired);

        if (Status is not (ReviewStatus.Pending or ReviewStatus.Reported))
            return Result.Failure(ReviewErrors.InvalidTransition("reject", Status));

        Status = ReviewStatus.Rejected;
        RejectionReason = reason.Trim();
        ModeratedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new ReviewRejectedDomainEvent(
            Guid.NewGuid(), ModeratedAt.Value, Id, UserId, adminId, RejectionReason));

        return Result.Success();
    }

    /// <summary>Marks an approved review as reported for moderation.</summary>
    public Result MarkAsReported(Guid reporterId)
    {
        if (reporterId == Guid.Empty)
            return Result.Failure(ReviewErrors.ReporterRequired);

        if (reporterId == UserId)
            return Result.Failure(ReviewErrors.AuthorCannotReportOwnReview);

        if (Status != ReviewStatus.Approved)
            return Result.Failure(ReviewErrors.InvalidTransition("report", Status));

        Status = ReviewStatus.Reported;

        RaiseDomainEvent(new ReviewReportedDomainEvent(
            Guid.NewGuid(), DateTimeOffset.UtcNow, Id, reporterId));

        return Result.Success();
    }

    /// <summary>Hides an approved or reported review from public view.</summary>
    public Result Hide(Guid adminId)
    {
        if (adminId == Guid.Empty)
            return Result.Failure(ReviewErrors.AdminRequired);

        if (Status is not (ReviewStatus.Approved or ReviewStatus.Reported))
            return Result.Failure(ReviewErrors.InvalidTransition("hide", Status));

        Status = ReviewStatus.Hidden;
        ModeratedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new ReviewHiddenDomainEvent(
            Guid.NewGuid(), ModeratedAt.Value, Id, UserId, adminId));

        return Result.Success();
    }
}