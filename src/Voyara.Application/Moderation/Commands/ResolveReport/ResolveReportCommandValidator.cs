
using FluentValidation;
using Voyara.Domain.Enums;

namespace Voyara.Application.Moderation.Commands.ResolveReport;

/// <summary>
/// Validates requests to record a moderation action.
/// </summary>
public sealed class ResolveReportCommandValidator
    : AbstractValidator<ResolveReportCommand>
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ResolveReportCommandValidator"/> class.
    /// </summary>
    public ResolveReportCommandValidator()
    {
        RuleFor(x => x.ReportId)
            .NotEmpty()
            .WithMessage("The report identifier is required.");

        RuleFor(x => x.Action)
            .IsInEnum()
            .WithMessage("The moderation action is invalid.");
    }
}
