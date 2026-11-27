using FluentValidation;

namespace Voyara.Application.AiAssistant.Queries.GetMyConversations;

/// <summary>Validates the paging and state of the conversation list.</summary>
public sealed class GetMyConversationsQueryValidator : AbstractValidator<GetMyConversationsQuery>
{
    /// <summary>Largest allowed page size.</summary>
    public const int MaxPageSize = 100;

    /// <summary>Largest allowed page number; keeps the row offset far from integer overflow.</summary>
    public const int MaxPage = 10_000;

    /// <summary>Creates the validator.</summary>
    public GetMyConversationsQueryValidator()
    {
        RuleFor(query => query.Page)
            .InclusiveBetween(1, MaxPage);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, MaxPageSize);

        RuleFor(query => query.State)
            .IsInEnum()
            .When(query => query.State.HasValue);
    }
}
