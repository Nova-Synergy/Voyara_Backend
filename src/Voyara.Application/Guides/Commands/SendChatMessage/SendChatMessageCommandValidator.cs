using FluentValidation;

namespace Voyara.Application.Guides.Commands.SendChatMessage;

public sealed class SendChatMessageCommandValidator : AbstractValidator<SendChatMessageCommand>
{
    public SendChatMessageCommandValidator()
    {
        RuleFor(x => x.ThreadId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(4000);
    }
}
