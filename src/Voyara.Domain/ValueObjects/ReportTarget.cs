using Voyara.Domain.Enums;
using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.ValueObjects;

/// <summary>
/// The polymorphic reference of a report: the kind of object being reported plus its identifier. The pair
/// is one value object, so a target can never be half-defined (a type without an id, or an unknown type).
/// The reported object may live in any module, so there is no foreign key and no navigation to it.
/// The target also knows which moderation actions make sense for it: content can be hidden or deleted,
/// people can be warned or suspended.
/// </summary>
public sealed class ReportTarget : ValueObject
{
    /// <summary>The kind of object being reported.</summary>
    public ReportTargetType Type { get; private set; }

    /// <summary>The identifier of the reported object. Its meaning depends on <see cref="Type"/>.</summary>
    public Guid Id { get; private set; }

    private ReportTarget()
    {
    }

    private ReportTarget(ReportTargetType type, Guid id)
    {
        Type = type;
        Id = id;
    }

    /// <summary>Creates a target, or fails when the type is undefined or the id is empty.</summary>
    public static Result<ReportTarget> Create(ReportTargetType type, Guid id)
    {
        if (!Enum.IsDefined(type))
        {
            return Result.Failure<ReportTarget>(Error.Validation(
                "ReportTarget.TypeInvalid", "The report target type is invalid."));
        }

        if (id == Guid.Empty)
        {
            return Result.Failure<ReportTarget>(Error.Validation(
                "ReportTarget.IdRequired", "The reported target is required."));
        }

        return new ReportTarget(type, id);
    }

    /// <summary>Whether this target is the user account with the given id.</summary>
    public bool IsUser(Guid userId) => Type == ReportTargetType.User && Id == userId;

    /// <summary>
    /// Whether the moderation action can be applied to this kind of target. Hide and Delete apply to content
    /// (reviews, destinations, other content); Warn and Suspend apply to people (users, guides).
    /// </summary>
    public bool Allows(ModerationAction action) => action switch
    {
        ModerationAction.Hide or ModerationAction.Delete =>
            Type is ReportTargetType.Review or ReportTargetType.Destination or ReportTargetType.Content,
        ModerationAction.Warn or ModerationAction.Suspend =>
            Type is ReportTargetType.User or ReportTargetType.Guide,
        _ => false
    };

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Type;
        yield return Id;
    }
}