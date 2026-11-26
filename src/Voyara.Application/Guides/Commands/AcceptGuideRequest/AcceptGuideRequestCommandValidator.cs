using FluentValidation;

namespace Voyara.Application.Guides.Commands.AcceptGuideRequest;

public sealed class AcceptGuideRequestCommandValidator : AbstractValidator<AcceptGuideRequestCommand>
{
    public AcceptGuideRequestCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty();
        RuleFor(x => x.MeetingLocation).NotEmpty().MaximumLength(500);
        RuleFor(x => x.AgreedPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}
