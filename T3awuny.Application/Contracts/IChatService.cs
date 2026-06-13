using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.DTOs.Chat;
using T3awuny.Application.Helpers;

namespace T3awuny.Application.Contracts
{
    public interface IChatService
    {
        Task<ApiResponse<ConversationDto>> GetOrCreateConversationAsync(string user1Id, string user2Id);
        Task<ApiResponse<IReadOnlyList<ConversationResponseDto>>> GetMyConversationsAsync(string userId);
        Task<ApiResponse<Pagination<MessageResponseDto>>> GetMessagesAsync(int conversationId, int page, int pageSize);
        Task<ApiResponse<MessageResponseDto>> SaveMessageAsync(string senderId, int conversationId, SendMessageDto dto);
        Task MarkAsReadAsync(string userId, int conversationId);
    }
}
