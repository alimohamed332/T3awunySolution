using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Chat
{
    public class ConversationResponseDto
    {
        public int Id { get; set; }
        public string OtherUserId { get; set; } = string.Empty;//
        public string OtherUserName { get; set; } = string.Empty;//
        public string? OtherUserImageUrl { get; set; }//
        public string? LastMessage { get; set; }//
        public DateTime LastMessageAt { get; set; }
        public int UnreadCount { get; set; }//
    }

}
