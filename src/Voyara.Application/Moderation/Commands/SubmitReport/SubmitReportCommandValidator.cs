using FluentValidation;
using Voyara.Domain.Enums;
using Voyara.Domain.Entities.Moderation;

namespace Voyara.Application.Moderation.Commands.SubmitReport;

/// <summary>
/// Validates <see cref="SubmitReportCommand"/> requests before execution.
/// </summary>
public sealed class SubmitReportCommandValidator
    : AbstractValidator<SubmitReportCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubmitReportCommandValidator"/> class
    /// and configures validation rules for submitting a moderation report.
    /// </summary>
    public SubmitReportCommandValidator()
    {
        RuleFor(x => x.TargetType)
            .IsInEnum()
            .WithMessage("The report target type is invalid.");

        RuleFor(x => x.TargetId)
            .NotEmpty()
            .WithMessage("The reported target is required.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("A reason for the report is required.")
            .MaximumLength(Report.MaxReasonLength)
            .WithMessage(
                $"The report reason cannot exceed {Report.MaxReasonLength} characters.");

        RuleFor(x => x.Comment)
            .MaximumLength(Report.MaxCommentLength)
            .WithMessage(
                $"The report comment cannot exceed {Report.MaxCommentLength} characters.")
            .When(x => x.Comment is not null);
    }
}