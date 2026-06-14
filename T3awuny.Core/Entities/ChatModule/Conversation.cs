using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Core.Entities.ChatModule
{
    public class Conversation : BaseEntity
    {
        public int Id { get; set; }
        public string User1Id { get; set; } = string.Empty;
        public ApplicationUser User1 { get; set; } = default!;
        public string User2Id { get; set; } = string.Empty;
        public ApplicationUser User2 { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime LastMessageAt { get; set; }  // for sorting inbox
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
