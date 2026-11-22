using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Voyara.Application.Guides.Commands.SendChatMessage;
using Voyara.Application.Guides.Queries.GetChatThread;

namespace Voyara.Api.Hubs;

/// <summary>
/// Real-time delivery for guide request chat. All rules (participants only, closed threads, message validation,
/// persistence) live in the Application use cases that the REST endpoints use as well; the hub only transports.
/// </summary>
[Authorize]
public sealed class ChatHub : Hub
{
    /// <summary>Client event raised for every persisted message.</summary>
    public const string ReceiveMessageEvent = "ReceiveMessage";

    /// <summary>Client event raised when a message is marked as read.</summary>
    public const string MessageReadEvent = "MessageRead";

    private readonly ISender _sender;

    public ChatHub(ISender sender) => _sender = sender;

    /// <summary>The SignalR group that carries one thread's events.</summary>
    public static string GroupName(Guid threadId) => $"chat-thread:{threadId:N}";

    /// <summary>Joins a thread's group; only its two participants may.</summary>
    public async Task JoinThread(Guid threadId)
    {
        var result = await _sender.Send(new GetChatThreadQuery(threadId), Context.ConnectionAborted);
        if (result.IsFailure)
            throw new HubException(result.Error.Description);

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(threadId), Context.ConnectionAborted);
    }

    /// <summary>Leaves a thread's group.</summary>
    public Task LeaveThread(Guid threadId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(threadId), Context.ConnectionAborted);

    /// <summary>Persists a message through the shared command, then broadcasts it to the thread's group.</summary>
    public async Task SendMessage(Guid threadId, string text)
    {
        var result = await _sender.Send(new SendChatMessageCommand(threadId, text), Context.ConnectionAborted);
        if (result.IsFailure)
            throw new HubException(result.Error.Description);

        await Clients.Group(GroupName(threadId)).SendAsync(ReceiveMessageEvent, result.Value, Context.ConnectionAborted);
    }
}
