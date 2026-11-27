using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.AiAssistant;

namespace Voyara.Infrastructure.Persistence.Configurations.AiAssistant;

/// <summary>EF Core mapping of the Conversation aggregate.</summary>
public sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("ai_conversations");

        builder.HasKey(conversation => conversation.Id);

        // Ids are generated in the domain (Guid.NewGuid), never by the database.
        builder.Property(conversation => conversation.Id)
            .ValueGeneratedNever();

        builder.Property(conversation => conversation.UserId)
            .IsRequired();

        builder.Property(conversation => conversation.Language)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(conversation => conversation.State)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        // UTC timestamp (datetime2 on SQL Server).
        builder.Property(conversation => conversation.StartedAt)
            .IsRequired();

        // The journey aggregate does not exist yet, so the link is a plain identifier without a foreign key.
        builder.Property(conversation => conversation.JourneyId);

        builder.HasOne(conversation => conversation.User)
            .WithMany()
            .HasForeignKey(conversation => conversation.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // The collected trip requirements are stored in the conversation row (all columns nullable).
        builder.OwnsOne(conversation => conversation.Requirements, requirements =>
        {
            requirements.Property(r => r.Country)
                .HasColumnName("requirements_country")
                .HasMaxLength(200);

            requirements.Property(r => r.DurationDays)
                .HasColumnName("requirements_duration_days");

            requirements.Property(r => r.BudgetAmount)
                .HasColumnName("requirements_budget_amount")
                .HasPrecision(12, 2);

            requirements.Property(r => r.BudgetCurrency)
                .HasColumnName("requirements_budget_currency")
                .HasMaxLength(3);

            requirements.Property(r => r.TravelStyle)
                .HasColumnName("requirements_travel_style")
                .HasMaxLength(100);

            requirements.Property(r => r.TransportPreference)
                .HasColumnName("requirements_transport_preference")
                .HasMaxLength(100);

            // The list of interests is stored as a JSON array in one nvarchar(max) column (provider-neutral).
            requirements.Property(r => r.Interests)
                .HasColumnName("requirements_interests")
                .HasConversion(
                    interests => JsonSerializer.Serialize(interests, (JsonSerializerOptions?)null),
                    json => (IReadOnlyList<string>)(JsonSerializer.Deserialize<List<string>>(json, (JsonSerializerOptions?)null)
                                                    ?? new List<string>()).AsReadOnly(),
                    new ValueComparer<IReadOnlyList<string>>(
                        (left, right) => left!.SequenceEqual(right!),
                        interests => interests.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
                        interests => interests.ToList().AsReadOnly()));
        });

        builder.HasMany(conversation => conversation.Messages)
            .WithOne()
            .HasForeignKey(message => message.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(conversation => conversation.Messages)
            .HasField("_messages")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(conversation => new { conversation.UserId, conversation.State });

        builder.HasQueryFilter(conversation => !conversation.IsDeleted);
    }
}
