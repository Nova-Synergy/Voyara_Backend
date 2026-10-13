namespace Voyara.SharedKernel.Entities;

/// <summary>
/// Entity with audit and soft-delete metadata, filled generically by infrastructure interceptors.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    /// <summary>UTC timestamp when the entity was created.</summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Identifier of the user or process that created the entity.</summary>
    public string? CreatedBy { get; set; }

    /// <summary>UTC timestamp of the last update.</summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Identifier of the user or process that performed the last update.</summary>
    public string? UpdatedBy { get; set; }

    /// <summary>Soft-delete flag; set instead of physically removing the row.</summary>
    public bool IsDeleted { get; set; }
}
