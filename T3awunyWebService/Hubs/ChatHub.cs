using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Chat;

namespace T3awunyWebService.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }
        public override async Task OnConnectedAsync() 
        {
            // each user joins their own personal group so we can send them messages directly
            //var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = Context.User!.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            await base.OnConnectedAsync();
        }

        [HubMethodName("sendmessage")]
        public async Task SendMessage(int conversationId, string content) // when click on send message
        {
            var senderId = Context.User!.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;

            // 1. Save message to DB
            var result = await _chatService.SaveMessageAsync(senderId!, conversationId, new SendMessageDto { Content = content });

            if (!result.IsSuccess) return;

            // 2. Get the other user in the conversation
            //var conversation = await _chatService.GetOrCreateConversationAsync(senderId!, senderId!);

            // 3. Send to sender (confirmation)
            //await Clients.Caller.SendAsync("MessageSent", result.Data);

            // 4. Send to receiver's personal group
            var receiverId = result.Data?.ReceiverId;
            await Clients.Group($"user_{receiverId}").SendAsync("messagereceived", result.Data);
            //chceck to send  chat bot
            //take ressult and return it again to the sender
        }
        [HubMethodName("markasread")]
        public async Task MarkAsRead(int conversationId) // when open some conversation
        {
            var userId = Context.User!.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            await _chatService.MarkAsReadAsync(userId!, conversationId);
        }
    }
}
