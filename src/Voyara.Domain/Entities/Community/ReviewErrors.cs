using Voyara.Domain.Enums;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Community;

/// <summary>Expected business errors of the <see cref="Review"/> aggregate.</summary>
public static class ReviewErrors
{
    /// <summary>The author id is missing.</summary>
    public static readonly Error UserRequired =
        Error.Validation("Review.UserRequired", "The review author is required.");

    /// <summary>The destination id is missing.</summary>
    public static readonly Error DestinationRequired =
        Error.Validation("Review.DestinationRequired", "The reviewed destination is required.");

    /// <summary>The rating is outside the allowed range.</summary>
    public static readonly Error InvalidRating =
        Error.Validation("Review.InvalidRating", "Rating must be between 1 and 5.");

    /// <summary>The review text is empty.</summary>
    public static readonly Error TextRequired =
        Error.Validation("Review.TextRequired", "Review text is required.");

    /// <summary>The visit date is in the future.</summary>
    public static readonly Error VisitDateInFuture =
        Error.Validation("Review.VisitDateInFuture", "Visit date cannot be in the future.");

    /// <summary>More photos than allowed were supplied.</summary>
    public static readonly Error TooManyPhotos =
        Error.Validation("Review.TooManyPhotos", $"A review can have at most {Review.MaxPhotos} photos.");

    /// <summary>A photo URL is empty.</summary>
    public static readonly Error InvalidPhotoUrl =
        Error.Validation("Review.InvalidPhotoUrl", "Photo URLs cannot be empty.");

    /// <summary>A rejection was attempted without giving a reason.</summary>
    public static readonly Error RejectionReasonRequired =
        Error.Validation("Review.RejectionReasonRequired", "A rejection reason is required.");

    /// <summary>The requested status change is not allowed from the current status.</summary>
    public static Error InvalidTransition(string operation, ReviewStatus current) =>
        Error.Conflict("Review.InvalidTransition", $"Cannot {operation} a review that is {current}.");
}