using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.ChatModule;

namespace T3awuny.Application.DTOs.Chat
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public string CurrentLoginedUserId { get; set; } = string.Empty;
        public string OtherUserId { get; set; } = string.Empty;//
        public string OtherUserName { get; set; } = string.Empty;//
        public string? OtherUserImageUrl { get; set; }//
        public ICollection<Message> Messages { get; set; } = new List<Message>();

    }
}
