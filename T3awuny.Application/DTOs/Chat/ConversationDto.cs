using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Chat
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        //public string OtherUserName { get; set; } = string.Empty;
        //public string? OtherUserImageUrl { get; set; }
        public DateTime LastMessageAt { get; set; }
    }
}
