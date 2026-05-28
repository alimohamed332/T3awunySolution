using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Core;
using T3awuny.Core.Entities.ChatModule;

namespace T3awuny.Application.Services
{
    public class ChatService : IChatService
    {
        //public async Task<ApiResponse<ConversationResponseDto>> GetOrCreateConversationAsync(string user1Id, string user2Id)
        //{
        //    if (user1Id == user2Id)
        //        return ApiResponse<ConversationResponseDto>.Fail(
        //            "Cannot start conversation with yourself");

        //    // normalize order so (A,B) and (B,A) find the same conversation
        //    var firstId = string.Compare(user1Id, user2Id) < 0 ? user1Id : user2Id;
        //    var secondId = string.Compare(user1Id, user2Id) < 0 ? user2Id : user1Id;

        //    var existing = await _unitOfWork.Conversations
        //                                    .GetByUsersAsync(firstId, secondId);
        //    if (existing is not null)
        //        return ApiResponse<ConversationResponseDto>.Ok(MapToDto(existing, user1Id));

        //    var conversation = new Conversation
        //    {
        //        User1Id = firstId,
        //        User2Id = secondId,
        //        CreatedAt = DateTime.UtcNow,
        //        LastMessageAt = DateTime.UtcNow
        //    };

        //    await _unitOfWork.Conversations.AddAsync(conversation);
        //    await _unitOfWork.SaveChangesAsync();

        //    return ApiResponse<ConversationResponseDto>.Ok(
        //        MapToDto(conversation, user1Id));
        //}
    }
}
