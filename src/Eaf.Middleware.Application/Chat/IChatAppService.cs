using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.Middleware.Chat.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Chat
{
    /// <summary>
    /// Representa a interface IChatAppService.
    /// </summary>
    public interface IChatAppService : IApplicationService
    {
        Task<GetUserChatFriendsWithSettingsOutput> GetUserChatFriendsWithSettingsAsync();

        Task<ListResultDto<ChatMessageDto>> GetUserChatMessages(GetUserChatMessagesInput input);

        /// <summary>
        /// Gets contextual chat history filtered by conversation, game, match or context type.
        /// </summary>
        Task<ListResultDto<ChatMessageDto>> GetHistoryAsync(GetChatHistoryInput input);

        Task MarkAllUnreadMessagesOfUserAsRead(MarkAllUnreadMessagesOfUserAsReadInput input);

        /// <summary>
        /// Marks contextual chat messages as read.
        /// </summary>
        Task MarkReadAsync(MarkChatReadInput input);
    }
}
