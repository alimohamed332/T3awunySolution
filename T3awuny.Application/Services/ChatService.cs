using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Chat;
using T3awuny.Application.Helpers;
using T3awuny.Core;
using T3awuny.Core.Entities.ChatModule;
using T3awuny.Core.Entities.UserModule;
using T3awuny.Core.Repository.Contracts;

namespace T3awuny.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;
        private readonly string _baseUrl;
        private readonly UserManager<ApplicationUser> _userManger;

        public ChatService(IUnitOfWork unitOfWork, IChatRepository chatRepository, IMapper mapper, IConfiguration configuration, UserManager<ApplicationUser> userManger)
        {
            _unitOfWork = unitOfWork;
            _chatRepository = chatRepository;
            _mapper = mapper;
            _baseUrl = configuration["App:ApplicationUrl"] ?? "";
            _userManger = userManger;
        }



        public async Task<ApiResponse<ConversationDto>> GetOrCreateConversationAsync(string user1Id, string user2Id)
        {
            if (user1Id == user2Id)
                return ApiResponse<ConversationDto>.Fail("مش متاح انك تعمل محادثة مع نفسك");

            // normalize order so (A,B) and (B,A) find the same conversation
            var firstId = string.Compare(user1Id, user2Id) < 0 ? user1Id : user2Id;
            var secondId = string.Compare(user1Id, user2Id) < 0 ? user2Id : user1Id;

            var existing = await _chatRepository.GetUserConverstionWithAnotherAsync(firstId, secondId);

            var conversionDto = new ConversationDto()
            {
                CurrentLoginedUserId = user1Id,
                OtherUserId = user2Id
            };

            if (existing is not null)
            {
                conversionDto.Id = existing.Id;
                conversionDto.Messages = existing.Messages;
                foreach(var msg in conversionDto.Messages)
                {
                    msg.SentAt = msg.SentAt.AddHours(-3);//////////////////////////////////////////////
                }
                if ( user1Id == existing.User1Id)
                {
                    conversionDto.OtherUserName = existing.User2.UserName!;
                    conversionDto.OtherUserImageUrl = $"{_baseUrl}{existing.User2.ProfileImageUrl}";
                }
                else
                {
                    conversionDto.OtherUserName = existing.User1.UserName!;
                    conversionDto.OtherUserImageUrl = $"{_baseUrl}{existing.User1.ProfileImageUrl}";
                }
                
                return ApiResponse<ConversationDto>.Ok(conversionDto,"تم العثور علي محادثة بالفعل");
            }
                

            var conversation = new Conversation
            {
                User1Id = firstId,
                User2Id = secondId,
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };
            var user = await _userManger.FindByIdAsync(user2Id);
            await _unitOfWork.Repository<Conversation>().AddAsync(conversation);
            if (await _unitOfWork.CompleteAsync() <= 0 || user is null)
                return ApiResponse<ConversationDto>.Fail("المستخدم المستهدف غير موجود او جدثت مشكلة أثناء الحفظ");

            conversionDto.Id = conversation.Id;
            conversionDto.OtherUserName =user.Name!;
            conversionDto.OtherUserImageUrl = $"{_baseUrl}{user.ProfileImageUrl}";
            return ApiResponse<ConversationDto>.Ok(conversionDto,"تم إنشاء محادثة بنجاح");
        }

        public async Task<ApiResponse<IReadOnlyList<ConversationResponseDto>>> GetMyConversationsAsync(string userId)
        {
            var conversations = await _chatRepository.GetUserConverstionsAsync(userId);
            if (conversations == null || !conversations.Any())
                return ApiResponse<IReadOnlyList<ConversationResponseDto>>.Ok(new List<ConversationResponseDto>(),"لا يوجد محادثات خاصة بك");

            var conversationDtos = new List<ConversationResponseDto>();
            foreach(var con in conversations)
            {
                var conDto = new ConversationResponseDto();
                conDto.Id = con.Id;
                conDto.LastMessage = con.Messages?.FirstOrDefault()?.Content;
                conDto.UnreadCount = con.Messages?.Count(m => !m.IsRead)??0;
                conDto.LastMessageAt = con.LastMessageAt.AddHours(-3);////////////////////////////////////////////////
                if(con.User1Id == userId)
                {
                    conDto.OtherUserId = con.User2Id;
                    conDto.OtherUserName = con.User2.Name;
                    conDto.OtherUserImageUrl = $"{_baseUrl}{con.User2.ProfileImageUrl}";
                }
                else
                {
                    conDto.OtherUserId = con.User1Id;
                    conDto.OtherUserName = con.User1.Name;
                    conDto.OtherUserImageUrl = $"{_baseUrl}{con.User1.ProfileImageUrl}";
                }

                conversationDtos.Add(conDto);
            }

            return ApiResponse<IReadOnlyList<ConversationResponseDto>>.Ok(conversationDtos, "تم الحصول علي المحادثات الخاصة بك");
        }

        public async Task<ApiResponse<Pagination<MessageResponseDto>>> GetMessagesAsync( int conversationId, int page, int pageSize)
        {
            var messages = await _chatRepository.GetConversationMessagesAsync(conversationId, page, pageSize);
            if (!messages.Any())
                return ApiResponse<Pagination<MessageResponseDto>>.Ok(new Pagination<MessageResponseDto>(page, pageSize, 0, new List<MessageResponseDto>()), "لا يوجد رسائل لعرضها");

            var messageDtos = messages.Select(m => _mapper.Map<MessageResponseDto>(m)).ToList();

            var paginatedResult = new Pagination<MessageResponseDto>(page, pageSize, 0, messageDtos);

            return ApiResponse<Pagination<MessageResponseDto>>.Ok(paginatedResult, "تم الحصول علي الرسائل والعدد مش مظبوط لو محتاجه قولي هظبطهولك");
        }

        public async Task<ApiResponse<MessageResponseDto>> SaveMessageAsync(string senderId, int conversationId, SendMessageDto dto)
        {
            var message = new Message()
            {
                SenderId = senderId,
                ConversationId = conversationId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _unitOfWork.Repository<Message>().AddAsync(message);
            if(await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<MessageResponseDto>.Fail("فشل في حفظ الرسالة");

            var messageDto = _mapper.Map<MessageResponseDto>(message);
            var conv = await _unitOfWork.Repository<Conversation>().GetByIdAsync(conversationId);
            messageDto.ReceiverId = conv?.User1Id == senderId ? conv.User2Id : conv?.User1Id ?? "";
            return ApiResponse<MessageResponseDto>.Ok(messageDto,"تم حفظ الرسالة بنجاح");
        }

        public async Task MarkAsReadAsync(string userId, int conversationId)
        {
            await _chatRepository.MarkAsReadAsync(userId, conversationId);
        }
    }
}
