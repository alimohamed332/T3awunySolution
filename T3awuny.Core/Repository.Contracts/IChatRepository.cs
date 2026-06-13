using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.ChatModule;

namespace T3awuny.Core.Repository.Contracts
{
    public interface IChatRepository
    {
        Task<IEnumerable<Conversation>?> GetUserConverstionsAsync(string userId);
        Task<IReadOnlyList<Message>> GetConversationMessagesAsync(int conversationId, int page, int pageSize);
        Task MarkAsReadAsync(string userId, int conversationId);
    }
}
