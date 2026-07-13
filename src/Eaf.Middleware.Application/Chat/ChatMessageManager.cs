using Abp;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.RealTime;
using Abp.Timing;
using Abp.UI;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Cache;
using Eaf.Middleware.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Chat
{
    /// <summary>
    /// Gerenciador responsável pela lógica de ChatMessage.
    /// </summary>
    [AbpAuthorize]
    public class ChatMessageManager : DomainService, IChatMessageManager
    {
        private readonly IChatCommunicator _chatCommunicator;
        private readonly IChatFeatureChecker _chatFeatureChecker;
        private readonly IRepository<ChatMessage, long> _chatMessageRepository;
        private readonly IFriendshipManager _friendshipManager;
        private readonly IOnlineClientManager<ChatChannel> _onlineClientManager;
        private readonly ITenantCache _tenantCache;
        private readonly IUserEmailer _userEmailer;
        private readonly IUserFriendsCache _userFriendsCache;
        private readonly UserManager _userManager;

        /// <summary>
        /// ChatMessageManager.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public ChatMessageManager(
            IFriendshipManager friendshipManager,
            IChatCommunicator chatCommunicator,
            IOnlineClientManager<ChatChannel> onlineClientManager,
            UserManager userManager,
            ITenantCache tenantCache,
            IUserFriendsCache userFriendsCache,
            IUserEmailer userEmailer,
            IRepository<ChatMessage, long> chatMessageRepository,
            IChatFeatureChecker chatFeatureChecker,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _friendshipManager = friendshipManager;
            _chatCommunicator = chatCommunicator;
            _onlineClientManager = onlineClientManager;
            _userManager = userManager;
            _tenantCache = tenantCache;
            _userFriendsCache = userFriendsCache;
            _userEmailer = userEmailer;
            _chatMessageRepository = chatMessageRepository;
            _chatFeatureChecker = chatFeatureChecker;
            UnitOfWorkManager = unitOfWorkManager;

            LocalizationSourceName = MiddlewareAppConsts.LocalizationSourceName;
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources.
        /// </summary>
        protected override string L(string name)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources com formatação.
        /// </summary>
        protected override string L(string name, params object[] args)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, args);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources para uma cultura específica.
        /// </summary>
        protected override string L(string name, CultureInfo culture)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, culture);
        }

        [UnitOfWork]
        public async Task<ChatMessage> FindMessageAsync(int id, long userId)
        {
            return await _chatMessageRepository.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        }

        [UnitOfWork]
        public virtual int GetUnreadMessageCount(UserIdentifier sender, UserIdentifier receiver)
        {
            using (CurrentUnitOfWork.SetTenantId(receiver.TenantId))
            {
                return _chatMessageRepository.Count(cm => cm.UserId == receiver.UserId &&
                                                          cm.TargetUserId == sender.UserId &&
                                                          cm.TargetTenantId == sender.TenantId &&
                                                          cm.ReadState == ChatMessageReadState.Unread);
            }
        }

        [UnitOfWork]
        public virtual long Save(ChatMessage message)
        {
            using (CurrentUnitOfWork.SetTenantId(message.TenantId))
            {
                return _chatMessageRepository.InsertAndGetId(message);
            }
        }

        [UnitOfWork]
        public void Delete(Guid sharedMessageId)
        {
            var dateLimit = Clock.Now.AddDays(-3);
            var allMessageToDelete = _chatMessageRepository.GetAll().Where(x => x.SharedMessageId == sharedMessageId);

            if (allMessageToDelete.Any() && allMessageToDelete.Any(x => x.CreationTime < dateLimit))
                throw new UserFriendlyException($"Could not delete chat message {sharedMessageId} before date {dateLimit:dd/MM HH:mm}");

            long[] ids = allMessageToDelete.Select(x => x.Id).ToArray();
            _chatMessageRepository.Delete(x => ids.Contains(x.Id));

            UnitOfWorkManager.Current.SaveChanges();
        }

        [UnitOfWork]
        public async Task SendMessageToGroupAsync(UserIdentifier sender, UserIdentifier receiverGroup, string message)
        {
            _chatFeatureChecker.CheckChatFeatures(sender.TenantId, receiverGroup.TenantId);

            // Grupo e usuários deste Grupo ainda não implementados.
            var receiversUser = _userManager.Users.Where(x => x.IsActive && !x.IsDeleted).ToList();
            var receivers = receiversUser.Select(x => x.ToUserIdentifier()).ToList();

            //Send for All Useres in Group
            await HandleReceiversToSenderAsync(sender, receivers, message);
        }

        [UnitOfWork]
        public async Task SendMessageAsync(UserIdentifier sender, UserIdentifier receiver, string message, string senderTenancyName, string senderUserName, Guid? senderProfilePictureId)
        {
            //Send for a User
            CheckReceiverExists(receiver);

            var friendshipState = (await _friendshipManager.GetFriendshipOrNullAsync(sender, receiver))?.State;
            if (friendshipState == FriendshipState.Blocked)
            {
                throw new UserFriendlyException(L("UserIsBlocked"));
            }

            var sharedMessageId = Guid.NewGuid();

            await HandleSenderToReceiverAsync(sender, receiver, message, sharedMessageId);
            await HandleReceiverToSenderAsync(sender, receiver, message, sharedMessageId);
            await HandleSenderUserInfoChangeAsync(sender, receiver, senderTenancyName, senderUserName, senderProfilePictureId);
        }

        private void CheckReceiverExists(UserIdentifier receiver)
        {
            var receiverUser = _userManager.GetUserOrNull(receiver);
            if (receiverUser == null)
            {
                throw new UserFriendlyException(L("TargetUserNotFoundProbablyDeleted"));
            }
        }

        private async Task HandleReceiversToSenderAsync(UserIdentifier senderIdentifier, List<UserIdentifier> receiversIdentifier, string message)
        {
            var sharedMessageId = Guid.NewGuid();

            var sentMessage = new ChatMessage(
                    senderIdentifier,
                    new UserIdentifier(senderIdentifier.TenantId, 0),
                    ChatSide.Sender,
                    message,
                    ChatMessageReadState.Read,
                    sharedMessageId,
                    ChatMessageReadState.Read
                );

            Save(sentMessage);

            await _chatCommunicator.SendMessageToClient(await
               _onlineClientManager.GetAllByUserIdAsync(senderIdentifier),
               sentMessage
               );

            foreach (var receiverIdentifier in receiversIdentifier)
            {
                if (senderIdentifier == receiverIdentifier)
                    continue;

                sentMessage = new ChatMessage(
                    new UserIdentifier(senderIdentifier.TenantId, 0),
                    senderIdentifier,
                    ChatSide.Receiver,
                    message,
                    ChatMessageReadState.Read,
                    sharedMessageId,
                    ChatMessageReadState.Unread
                );

                Save(sentMessage);

                var clients = await _onlineClientManager.GetAllByUserIdAsync(receiverIdentifier);
                if (clients.Any())
                {
                    await _chatCommunicator.SendMessageToClient(clients, sentMessage);
                }
            }
        }

        private async Task HandleReceiverToSenderAsync(UserIdentifier senderIdentifier, UserIdentifier receiverIdentifier, string message, Guid sharedMessageId)
        {
            var friendshipState = (await _friendshipManager.GetFriendshipOrNullAsync(receiverIdentifier, senderIdentifier))?.State;

            if (friendshipState == null)
            {
                var senderTenancyName = senderIdentifier.TenantId.HasValue ?
                    (await _tenantCache.GetAsync(senderIdentifier.TenantId.Value)).TenancyName :
                    null;

                var senderUser = await _userManager.GetUserAsync(senderIdentifier);
                await _friendshipManager.CreateFriendshipAsync(
                    new Friendship(
                        receiverIdentifier,
                        senderIdentifier,
                        senderTenancyName,
                        senderUser.UserName,
                        senderUser.ProfilePictureId,
                        FriendshipState.Accepted
                    )
                );
            }

            if (friendshipState == FriendshipState.Blocked)
            {
                //Do not send message if receiver banned the sender
                return;
            }

            var sentMessage = new ChatMessage(
                    receiverIdentifier,
                    senderIdentifier,
                    ChatSide.Receiver,
                    message,
                    ChatMessageReadState.Unread,
                    sharedMessageId,
                    ChatMessageReadState.Read
                );

            Save(sentMessage);

            var clients = await _onlineClientManager.GetAllByUserIdAsync(receiverIdentifier);
            if (clients.Any())
            {
                await _chatCommunicator.SendMessageToClient(clients, sentMessage);
            }
            else if (GetUnreadMessageCount(senderIdentifier, receiverIdentifier) == 1)
            {
                var senderTenancyName = senderIdentifier.TenantId.HasValue ?
                    (await _tenantCache.GetAsync(senderIdentifier.TenantId.Value)).TenancyName :
                    null;

                await _userEmailer.TryToSendChatMessageMail(
                      await _userManager.GetUserAsync(receiverIdentifier),
                      (await _userManager.GetUserAsync(senderIdentifier)).UserName,
                      senderTenancyName,
                      sentMessage
                  );
            }
        }

        private async Task HandleSenderToReceiverAsync(UserIdentifier senderIdentifier, UserIdentifier receiverIdentifier, string message, Guid sharedMessageId)
        {
            var friendshipState = (await _friendshipManager.GetFriendshipOrNullAsync(senderIdentifier, receiverIdentifier))?.State;
            if (friendshipState == null)
            {
                friendshipState = FriendshipState.Accepted;

                var receiverTenancyName = receiverIdentifier.TenantId.HasValue
                    ? (await _tenantCache.GetAsync(receiverIdentifier.TenantId.Value)).TenancyName
                    : null;

                var receiverUser = await _userManager.GetUserAsync(receiverIdentifier);
                await _friendshipManager.CreateFriendshipAsync(
                    new Friendship(
                        senderIdentifier,
                        receiverIdentifier,
                        receiverTenancyName,
                        receiverUser.UserName,
                        receiverUser.ProfilePictureId,
                        friendshipState.Value)
                );
            }

            if (friendshipState.Value == FriendshipState.Blocked)
            {
                //Do not send message if receiver banned the sender
                return;
            }

            var sentMessage = new ChatMessage(
                senderIdentifier,
                receiverIdentifier,
                ChatSide.Sender,
                message,
                ChatMessageReadState.Read,
                sharedMessageId,
                ChatMessageReadState.Unread
            );

            Save(sentMessage);

            await _chatCommunicator.SendMessageToClient(
                await _onlineClientManager.GetAllByUserIdAsync(senderIdentifier),
                sentMessage
                );
        }

        private async Task HandleSenderUserInfoChangeAsync(UserIdentifier sender, UserIdentifier receiver, string senderTenancyName, string senderUserName, Guid? senderProfilePictureId)
        {
            var receiverCacheItem = _userFriendsCache.GetCacheItemOrNull(receiver);

            var senderAsFriend = receiverCacheItem?.Friends.FirstOrDefault(f => f.FriendTenantId == sender.TenantId && f.FriendUserId == sender.UserId);
            if (senderAsFriend == null)
            {
                return;
            }

            if (senderAsFriend.FriendTenancyName == senderTenancyName &&
                senderAsFriend.FriendUserName == senderUserName &&
                senderAsFriend.FriendProfilePictureId == senderProfilePictureId)
            {
                return;
            }

            var friendship = (await _friendshipManager.GetFriendshipOrNullAsync(receiver, sender));
            if (friendship == null)
            {
                return;
            }

            friendship.FriendTenancyName = senderTenancyName;
            friendship.FriendUserName = senderUserName;
            friendship.FriendProfilePictureId = senderProfilePictureId;

            await _friendshipManager.UpdateFriendshipAsync(friendship);
        }
    }
}