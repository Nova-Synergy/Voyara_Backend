using FluentValidation;

namespace Voyara.Application.Guides.Commands.RejectGuideRequest;

public sealed class RejectGuideRequestCommandValidator : AbstractValidator<RejectGuideRequestCommand>
{
    public RejectGuideRequestCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
