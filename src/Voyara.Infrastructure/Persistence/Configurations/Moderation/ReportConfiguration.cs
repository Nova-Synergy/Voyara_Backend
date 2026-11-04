using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Moderation;

namespace Voyara.Infrastructure.Persistence.Configurations.Moderation;

/// <summary>
/// Configures the Report aggregate root for Entity Framework Core persistence.
/// </summary>
/// <remarks>
/// <para>
/// This configuration establishes the database schema and constraints for the Report entity,
/// which represents a user's complaint about content or a person on the platform. The Report
/// aggregate manages the full lifecycle of a moderation workflow, transitioning through states
/// (Reported → Investigation → ActionTaken → Resolved) as defined in state machine ST-07.
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// Table mapping to "moderation_reports" with required properties for ReporterId, Reason, Status,
/// and the polymorphic ReportTarget value object (Type and Id columns).
/// </description>
/// </item>
/// <item>
/// <description>
/// Enum-to-string conversion for Status (ReportStatus) and ActionTaken (ModerationAction) with
/// appropriate max length constraints for storage.
/// </description>
/// </item>
/// <item>
/// <description>
/// Owned value object ReportTarget flattened into the same table with custom column names
/// (target_type and target_id), allowing polymorphic reference to any reportable entity.
/// </description>
/// </item>
/// <item>
/// <description>
/// Indexes on ReporterId, Status, and ResolvedBy for efficient filtering by reporter, status,
/// and resolution admin. A composite index on (Status, CreatedAt) optimizes moderation queue queries.
/// </description>
/// </item>
/// <item>
/// <description>
/// Global query filter to exclude soft-deleted reports from ordinary queries by default.
/// </description>
/// </item>
/// <item>
/// <description>
/// Required navigation to ReportTarget, ensuring every report has a valid target reference.
/// </description>
/// </item>
/// </list>
/// </remarks>
public sealed class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    /// <summary>
    /// Configures the Report entity type for the database context.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is called during DbContext model creation to establish the complete mapping
    /// for the Report aggregate and its owned ReportTarget value object. The configuration enforces
    /// domain invariants including required properties, enum conversions, indexes for query performance,
    /// and soft-delete filtering to hide deleted reports from normal queries.
    /// </para>
    /// <para>
    /// The ReportTarget is an owned value object that captures the polymorphic reference to the
    /// reported entity (Review, User, Guide, Destination, or other Content). Its properties are
    /// stored as columns in the same "moderation_reports" table to maintain aggregate consistency.
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// Primary key on the Id property.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Required constraint on ReporterId, establishing the identity of the user filing the report
    /// (reference to the Identity module).
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Required constraint on Reason with a maximum length constrained by Report.MaxReasonLength (100 chars).
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Optional Comment property with a maximum length constrained by Report.MaxCommentLength (2000 chars),
    /// providing additional context from the reporter.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Enum-to-string conversion for Status, storing ReportStatus as a string with max length of 30 characters.
    /// Valid values: Reported, Investigation, ActionTaken, Resolved.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Enum-to-string conversion for ActionTaken, storing ModerationAction as an optional string with
    /// max length of 30 characters. Valid values: Hide, Delete, Warn, Suspend. Null until the report is upheld.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Optional ResolvedBy property storing the identifier of the admin who closed the report (null until resolved).
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Owned value object ReportTarget configured with custom column names (target_type, target_id) and
    /// an index on the (Type, Id) pair for efficient queries filtering by target.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Index on ReporterId for efficient retrieval of all reports filed by a specific user.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Index on Status for filtering reports by their lifecycle state (Reported, Investigation, etc.).
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Index on ResolvedBy for efficient queries of reports closed by a specific admin.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Composite index on (Status, CreatedAt) to optimize moderation queue queries that typically
    /// sort pending reports by creation date.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Global query filter to automatically exclude soft-deleted reports from queries, supporting
    /// the aggregate's soft-delete flag for audit and recovery purposes.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="builder">The entity type builder used to configure the Report entity.</param>
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("moderation_reports");

        builder.HasKey(report => report.Id);

        builder.Property(report => report.ReporterId)
            .IsRequired();

        builder.Property(report => report.Reason)
            .HasMaxLength(Report.MaxReasonLength)
            .IsRequired();

        builder.Property(report => report.Comment)
            .HasMaxLength(Report.MaxCommentLength);

        builder.Property(report => report.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(report => report.ActionTaken)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(report => report.ResolvedBy);

        // ReportTarget is a value object owned by the Report aggregate.
        // Its properties are stored as columns in the same table.
        builder.OwnsOne(report => report.Target, target =>
        {
            target.Property(value => value.Type)
                .HasColumnName("target_type")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            target.Property(value => value.Id)
                .HasColumnName("target_id")
                .IsRequired();

            target.HasIndex(value => new
            {
                value.Type,
                value.Id
            });
        });

        builder.Navigation(report => report.Target)
            .IsRequired();

        builder.HasIndex(report => report.ReporterId);
        builder.HasIndex(report => report.Status);
        builder.HasIndex(report => report.ResolvedBy);

        // Supports moderation queue queries.
        builder.HasIndex(report => new
        {
            report.Status,
            report.CreatedAt
        });

        // Aggregate root: hide soft-deleted reports from ordinary queries.
        builder.HasQueryFilter(report => !report.IsDeleted);
    }
}
