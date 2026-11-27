using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Voyara.Api.Contracts.Guides;
using Voyara.Api.Hubs;

namespace Voyara.Api.Controllers.Guides
{
    /// <summary>Chat between the tourist and the guide of an accepted request.</summary>
    [ApiController]
    [Authorize]
    [Route("api/v1/chat-threads")]
    public sealed class ChatThreadsController : BaseApiController
    {
        private readonly IHubContext<ChatHub> _hub;

        public ChatThreadsController(IHubContext<ChatHub> hub) => _hub = hub;

        /// <summary>Sends a message. It is persisted first, then pushed to the thread's SignalR group.</summary>
        [HttpPost("{id:guid}/messages")]
        public async Task<IActionResult> SendMessage(
            Guid id, [FromBody] SendChatMessageRequest request, CancellationToken cancellationToken)
        {
            var result = await Sender.Send(new SendChatMessageCommand(id, request.Text), cancellationToken);

            if (result.IsSuccess)
            {
                await _hub.Clients.Group(ChatHub.GroupName(id))
                    .SendAsync(ChatHub.ReceiveMessageEvent, result.Value, cancellationToken);
            }

            return HandleResult(result);
        }

        /// <summary>Lists a thread's messages, oldest first.</summary>
        [HttpGet("{id:guid}/messages")]
        public async Task<IActionResult> GetMessages(
            Guid id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
            => HandleResult(await Sender.Send(new GetChatMessagesQuery(id, page, pageSize), cancellationToken));
    }

}
