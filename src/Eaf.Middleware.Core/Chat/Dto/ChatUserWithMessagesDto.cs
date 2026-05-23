using System.Collections.Generic;

namespace Eaf.Middleware.Chat.Dto
{
    /// <summary>
    /// Representa a classe ChatUserWithMessagesDto.
    /// </summary>
    public class ChatUserWithMessagesDto : ChatUserDto
    {
        public List<ChatMessageDto> Messages { get; set; }
    }
}