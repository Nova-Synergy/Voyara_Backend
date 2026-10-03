namespace Voyara.SharedKernel.Entities;

/// <summary>
/// Base type for all entities, providing identity equality semantics.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Unique identifier of the entity.</summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public static bool operator ==(BaseEntity? left, BaseEntity? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(BaseEntity? left, BaseEntity? right) => !(left == right);

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity other || GetType() != other.GetType())
        {
            return false;
        }

        return Id == other.Id;
    }

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode() * 31 + GetType().GetHashCode();
}
