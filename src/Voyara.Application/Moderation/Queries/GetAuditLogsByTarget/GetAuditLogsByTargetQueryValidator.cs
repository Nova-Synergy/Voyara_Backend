using FluentValidation;
using Voyara.Domain.Entities.Moderation;

namespace Voyara.Application.Moderation.Queries.GetAuditLogsByTarget;

/// <summary>
/// Validates <see cref="GetAuditLogsByTargetQuery"/> requests before execution.
/// </summary>
public sealed class GetAuditLogsByTargetQueryValidator
    : AbstractValidator<GetAuditLogsByTargetQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAuditLogsByTargetQueryValidator"/> class
    /// and configures validation rules for retrieving audit logs by target type and identifier.
    /// </summary>
    public GetAuditLogsByTargetQueryValidator()
    {
        RuleFor(query => query.TargetType)
            .NotEmpty()
            .WithMessage("Target type is required.")
            .MaximumLength(AuditLog.MaxTargetTypeLength)
            .WithMessage($"Target type cannot exceed {AuditLog.MaxTargetTypeLength} characters.");

        RuleFor(query => query.TargetId)
            .NotEmpty()
            .WithMessage("Target ID is required.")
            .MaximumLength(AuditLog.MaxTargetIdLength)
            .WithMessage($"Target ID cannot exceed {AuditLog.MaxTargetIdLength} characters.");
    }
}