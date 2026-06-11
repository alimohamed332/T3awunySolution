using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using T3awuny.Application.Contracts;

namespace T3awunyWebService.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        //private readonly IChatService _chatService;

        //public ChatHub(IChatService chatService)
        //{
        //    _chatService = chatService;
        //}

        //public override async Task OnConnectedAsync()
        //{
        //    // each user joins their own personal group
        //    // so we can send them messages directly
        //    var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
        //    await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        //    await base.OnConnectedAsync();
        //}

        //public async Task SendMessage(int conversationId, string content)
        //{
        //    var senderId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);

        //    // 1. Save message to DB
        //    var result = await _chatService.SaveMessageAsync(
        //        senderId!, conversationId, new SendMessageDto { Content = content });

        //    if (!result.Success) return;

        //    // 2. Get the other user in the conversation
        //    var conversation = await _chatService
        //                           .GetOrCreateConversationAsync(senderId!, senderId!);

        //    // 3. Send to sender (confirmation)
        //    await Clients.Caller.SendAsync("MessageSent", result.Data);

        //    // 4. Send to receiver's personal group
        //    var receiverId = result.Data!.ReceiverId;
        //    await Clients.Group($"user_{receiverId}")
        //                 .SendAsync("MessageReceived", result.Data);
        //}

        //public async Task MarkAsRead(int conversationId)
        //{
        //    var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
        //    await _chatService.MarkAsReadAsync(userId!, conversationId);
        //}
    }
}
