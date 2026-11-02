using FluentValidation;

namespace Voyara.Application.Community.Commands.CreateReview;

/// <summary>
/// Validator for the CreateReviewCommand, ensuring required fields are present and within valid ranges.
/// </summary>
/// <remarks>
/// <para>
/// Validates that the DestinationId is not empty, the Rating is between 1 and 5 inclusive,
/// the Text is not empty and does not exceed 4000 characters, and the VisitDate is on or
/// before the current UTC date.
/// </para>
/// </remarks>
public sealed class CreateReviewCommandValidator
    : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.DestinationId)
            .NotEmpty();

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5);

        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.VisitDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}