using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.ChatModule;
using T3awuny.Core.Repository.Contracts;
using T3awuny.Infrastructure.Data;

namespace T3awuny.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly T3awunyDbContext _dbContext;

        public ChatRepository(T3awunyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Conversation>?> GetUserConverstionsAsync(string userId)
        {

            var  conv1 = await _dbContext.Conversations.Where(c => c.User1Id == userId).Include(c => c.User2).Include(c => c.Messages.OrderByDescending(m=>m.SentAt).Take(1)).AsNoTracking().ToListAsync();
            var  conv2 = await _dbContext.Conversations.Where(c => c.User2Id == userId).Include(c => c.User1).Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1)).AsNoTracking().ToListAsync();
            return  conv1.Union(conv2);
            
        }

        public async Task<Conversation?> GetUserConverstionWithAnotherAsync(string user1Id, string user2Id)
        {

            var conv = await _dbContext.Conversations.Include(c => c.User1).Include(c => c.User2)
                .Include(c => c.Messages.OrderBy(m => m.SentAt))
                .FirstOrDefaultAsync(c => c.User1Id == user1Id && c.User2Id == user2Id);
            return conv;
        }

        public async Task<IReadOnlyList<Message>> GetConversationMessagesAsync(int conversationId, int page, int pageSize)
        {
            return await _dbContext.Messages.OrderBy(m => m.SentAt).Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
        }

        public async Task MarkAsReadAsync(string userId, int conversationId)
        {
            var conversation = await _dbContext.Conversations.Include(c => c.Messages.Where(m => !m.IsRead)).FirstOrDefaultAsync(c => c.Id == conversationId);
            if (conversation is null)
                return;
            foreach (var mes in conversation.Messages)
            {
                if (mes.SenderId != userId)
                    mes.IsRead = true;
            }
                

            _dbContext.SaveChanges();
        }
    }
}
