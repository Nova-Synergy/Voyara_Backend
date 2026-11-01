using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Guides;

namespace Voyara.Api.DTOs.Guides;

public class SendChatMessageDto
{
    [Required]
    [StringLength(4000)]
    public string Text { get; set; } = string.Empty;
}

public class ChatMessageReadDto
{
    public Guid Id { get; set; }
    public Guid ChatThreadId { get; set; }
    public Guid UserId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }

    public static ChatMessageReadDto FromEntity(ChatMessage message) => new()
    {
        Id = message.Id,
        ChatThreadId = message.ChatThreadId,
        UserId = message.UserId,
        Text = message.Text,
        SentAt = message.SentAt,
        ReadAt = message.ReadAt
    };
}
