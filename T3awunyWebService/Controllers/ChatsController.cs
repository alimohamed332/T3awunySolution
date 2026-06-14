using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Chat;
using T3awuny.Application.Helpers;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatsController(IChatService chatService)
        {
            _chatService = chatService;
        }

        ///api/chat/conversations
        [Authorize]
        [HttpGet("my-conversations")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<ConversationResponseDto>>>> GetMyConversations()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value??string.Empty;
            var result = await _chatService.GetMyConversationsAsync(userId);
            if(!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        //api/chat/conversations/{id}/messages
        [Authorize, HttpGet("conversations/{conversationId}/messages")]
        public async Task<ActionResult<ApiResponse<Pagination<MessageResponseDto>>>> GetMyConversationMessages(int conversationId,int page = 1, int pageSize = 10)
        {
            var result = await _chatService.GetMessagesAsync(conversationId,page,pageSize);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }

        //api/chat/conversations/{userId}
        [Authorize]
        [HttpPost("conversations/start/{targetUserId}")]
        public async Task<ActionResult<ApiResponse<ConversationDto>>> StartConversation(string targetUserId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? string.Empty;
            var result = await _chatService.GetOrCreateConversationAsync(userId, targetUserId);

            if(!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }

    }
}
