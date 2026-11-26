using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Voyara.Api.Hubs;

/// <summary>
/// SignalR hub for guide chat. Integration point only — chat logic is out of scope in this scaffold.
/// </summary>
public sealed class ChatHub : Hub
{
    /// <summary>Client event raised for every persisted message.</summary>
    public const string ReceiveMessageEvent = "ReceiveMessage";

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
