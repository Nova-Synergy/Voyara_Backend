using FluentValidation;

namespace Voyara.Application.Guides.Queries.GetChatMessages;

public sealed class GetChatMessagesQueryValidator : AbstractValidator<GetChatMessagesQuery>
{
    public GetChatMessagesQueryValidator()
    {
        RuleFor(x => x.ThreadId).NotEmpty();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, GuideRequestPolicy.MaxPageSize);
    }
}
