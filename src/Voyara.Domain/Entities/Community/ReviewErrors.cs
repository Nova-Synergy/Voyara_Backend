using Voyara.Domain.Enums;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Community;

/// <summary>Expected business errors produced by the Review aggregate.</summary>
public static class ReviewErrors
{
    public static readonly Error UserRequired =
        Error.Validation("Review.UserRequired", "The review author is required.");

    public static readonly Error DestinationRequired =
        Error.Validation("Review.DestinationRequired", "The destination is required.");

    public static readonly Error InvalidRating =
        Error.Validation("Review.InvalidRating", "Rating must be between 1 and 5.");

    public static readonly Error TextRequired =
        Error.Validation("Review.TextRequired", "Review text is required.");

    public static readonly Error VisitDateInFuture =
        Error.Validation("Review.VisitDateInFuture", "Visit date cannot be in the future.");

    public static readonly Error TooManyPhotos =
        Error.Validation("Review.TooManyPhotos", $"A review can have at most {Review.MaxPhotos} photos.");

    public static readonly Error InvalidPhotoUrl =
        Error.Validation("Review.InvalidPhotoUrl", "Photo URL cannot be empty.");

    public static readonly Error PhotoNotFound =
        Error.NotFound("Review.PhotoNotFound", "The photo does not belong to this review.");

    public static readonly Error AdminRequired =
        Error.Validation("Review.AdminRequired", "An administrator ID is required.");

    public static readonly Error ReporterRequired =
        Error.Validation("Review.ReporterRequired", "A reporter ID is required.");

    public static readonly Error AuthorCannotReportOwnReview =
        Error.Validation("Review.AuthorCannotReportOwnReview", "The review author cannot report their own review.");

    public static readonly Error RejectionReasonRequired =
        Error.Validation("Review.RejectionReasonRequired", "A rejection reason is required.");

    public static Error InvalidTransition(string operation, ReviewStatus current) =>
        Error.Conflict(
            "Review.InvalidTransition",
            $"Cannot {operation} a review that is {current}.");
}