using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.AiAssistant;

namespace Voyara.Infrastructure.Persistence.Configurations.AiAssistant;

/// <summary>EF Core mapping of a conversation message (identity bigint key, ordered by insertion).</summary>
public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("ai_messages");

        builder.HasKey(message => message.Id);

        builder.Property(message => message.Id)
            .ValueGeneratedOnAdd();

        builder.Property(message => message.ConversationId)
            .IsRequired();

        builder.Property(message => message.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // No maximum length: nvarchar(max) on SQL Server.
        builder.Property(message => message.Content)
            .IsRequired();

        builder.Property(message => message.TokensUsed)
            .IsRequired();

        builder.Property(message => message.CreatedAt)
            .IsRequired();

        builder.HasIndex(message => new { message.ConversationId, message.Id });
    }
}
