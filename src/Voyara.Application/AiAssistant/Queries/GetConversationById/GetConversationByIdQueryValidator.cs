using FluentValidation;

namespace Voyara.Application.AiAssistant.Queries.GetConversationById;

/// <summary>Validates conversation detail requests.</summary>
public sealed class GetConversationByIdQueryValidator : AbstractValidator<GetConversationByIdQuery>
{
    /// <summary>Creates the validator.</summary>
    public GetConversationByIdQueryValidator()
    {
        RuleFor(query => query.ConversationId)
            .NotEmpty();
    }
}
