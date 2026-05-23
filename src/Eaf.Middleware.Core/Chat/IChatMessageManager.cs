using Abp;
using Abp.Domain.Services;
using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.Chat
{
    /// <summary>
    /// Representa a interface IChatMessageManager.
    /// </summary>
    public interface IChatMessageManager : IDomainService
    {
        Task<ChatMessage> FindMessageAsync(int id, long userId);

        int GetUnreadMessageCount(UserIdentifier sender, UserIdentifier receiver);

        long Save(ChatMessage message);

        void Delete(Guid sharedMessageId);

        Task SendMessageToGroupAsync(UserIdentifier sender, UserIdentifier receiverGroup, string message);

        Task SendMessageAsync(UserIdentifier sender, UserIdentifier receiver, string message, string senderTenancyName, string senderUserName, Guid? senderProfilePictureId);
    }
}