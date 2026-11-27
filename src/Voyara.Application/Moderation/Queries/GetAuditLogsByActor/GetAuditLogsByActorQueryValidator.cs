using FluentValidation;

namespace Voyara.Application.Moderation.Queries.GetAuditLogsByActor;

/// <summary>
/// Validates <see cref="GetAuditLogsByActorQuery"/> requests before execution.
/// </summary>
public sealed class GetAuditLogsByActorQueryValidator
    : AbstractValidator<GetAuditLogsByActorQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAuditLogsByActorQueryValidator"/> class
    /// and configures validation rules for retrieving audit logs by actor identifier.
    /// </summary>
    public GetAuditLogsByActorQueryValidator()
    {
        RuleFor(query => query.ActorId)
            .NotEmpty()
            .WithMessage("Actor ID is required.");
    }
}