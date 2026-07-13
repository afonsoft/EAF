using Abp;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.RealTime;
using Abp.Runtime.Session;
using Abp.Timing;
using Eaf.Middleware.Chat.Dto;
using Eaf.Middleware.Friendships.Cache;
using Eaf.Middleware.Friendships.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Chat
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de Chat.
    /// </summary>
    [AbpAuthorize]
    public class ChatAppService : MiddlewareAppServiceBase, IChatAppService
    {
        private readonly IChatCommunicator _chatCommunicator;
        private readonly IRepository<ChatMessage, long> _chatMessageRepository;
        private readonly IOnlineClientManager<ChatChannel> _onlineClientManager;
        private readonly IUserFriendsCache _userFriendsCache;

        /// <summary>
        /// ChatAppService.
        /// </summary>
        /// <param name="chatMessageRepository">Parâmetro chatMessageRepository.</param>
        /// <param name="userFriendsCache">Parâmetro userFriendsCache.</param>
        /// <param name="onlineClientManager">Parâmetro onlineClientManager.</param>
        /// <param name="chatCommunicator">Parâmetro chatCommunicator.</param>
        /// <returns>Resultado da operação.</returns>
        public ChatAppService(
            IRepository<ChatMessage, long> chatMessageRepository,
            IUserFriendsCache userFriendsCache,
            IOnlineClientManager<ChatChannel> onlineClientManager,
            IChatCommunicator chatCommunicator)
        {
            _chatMessageRepository = chatMessageRepository;
            _userFriendsCache = userFriendsCache;
            _onlineClientManager = onlineClientManager;
            _chatCommunicator = chatCommunicator;
        }

        [DisableAuditing]
        [Produces("application/json", "application/json-patch+json", "text/json")]
        public async Task<GetUserChatFriendsWithSettingsOutput> GetUserChatFriendsWithSettingsAsync()
        {
            var userIdentifier = AbpSession.ToUserIdentifier();
            if (userIdentifier == null)
            {
                return new GetUserChatFriendsWithSettingsOutput();
            }

            var cacheItem = _userFriendsCache.GetCacheItem(userIdentifier);
            var friends = ObjectMapper.Map<List<FriendshipDto>>(cacheItem.Friends);

            friends ??= new List<FriendshipDto>();

            foreach (var friend in friends)
            {
                friend.IsOnline = await _onlineClientManager.IsOnlineAsync(
                    new UserIdentifier(friend.FriendTenantId, friend.FriendUserId)
                );
            }

            if (userIdentifier.TenantId != null && userIdentifier.TenantId.HasValue &&
                await FeatureChecker.IsEnabledAsync(userIdentifier.TenantId.Value, AppFeatures.GroupChatFeature))
            {
                friends.Insert(0, new FriendshipDto
                {
                    GroupId = 1,
                    FriendUserId = 0,
                    FriendTenancyName = "Default",
                    FriendTenantId = userIdentifier.TenantId,
                    IsOnline = true,
                    State = Friendships.FriendshipState.Accepted,
                    Email = L("GroupEmail"),
                    FriendUserName = L("Group"),
                    Name = L("Group"),
                    Surname = "",
                    UnreadMessageCount = await (await _chatMessageRepository.GetAllAsync()).CountAsync(cm => cm.ReadState == ChatMessageReadState.Unread &&
                                                               cm.UserId == userIdentifier.UserId &&
                                                               cm.TenantId == userIdentifier.TenantId &&
                                                               cm.TargetUserId == 0 &&
                                                               cm.TargetTenantId == userIdentifier.TenantId &&
                                                               cm.Side == ChatSide.Receiver) +
                                         await (await _chatMessageRepository.GetAllAsync()).CountAsync(cm => cm.ReadState == ChatMessageReadState.Unread &&
                                                               cm.UserId == 0 &&
                                                               cm.TenantId == userIdentifier.TenantId &&
                                                               cm.TargetUserId == userIdentifier.UserId &&
                                                               cm.TargetTenantId == userIdentifier.TenantId &&
                                                               cm.Side == ChatSide.Receiver)
                });
            }

            return new GetUserChatFriendsWithSettingsOutput
            {
                Friends = friends,
                ServerTime = Clock.Now
            };
        }

        [DisableAuditing]
        [Produces("application/json", "application/json-patch+json", "text/json")]
        public async Task<ListResultDto<ChatMessageDto>> GetUserChatMessages(GetUserChatMessagesInput input)
        {
            var userId = AbpSession.GetUserId();

            if (input.UserId.HasValue && input.UserId.Value > 0)
            {
                return await GetUserChatMessagesAsync(input, userId);
            }

            if (input.GroupId.HasValue && input.GroupId.Value > 0)
            {
                return await GetGroupChatMessagesAsync(input, userId);
            }

            return new ListResultDto<ChatMessageDto>(new List<ChatMessageDto>());
        }

        private async Task<ListResultDto<ChatMessageDto>> GetUserChatMessagesAsync(GetUserChatMessagesInput input, long userId)
        {
            var messages = await (await _chatMessageRepository.GetAllAsync())
                .WhereIf(input.MinMessageId.HasValue, m => m.Id < input.MinMessageId.Value)
                .Where(m => m.UserId == userId && m.TargetTenantId == input.TenantId && m.TargetUserId == input.UserId)
                .OrderByDescending(m => m.CreationTime)
                .Take(100)
                .ToListAsync();

            messages.Reverse();
            var listMessages = ObjectMapper.Map<List<ChatMessageDto>>(messages);

            await SetTargetUserNamesAsync(listMessages);

            return new ListResultDto<ChatMessageDto>(listMessages);
        }

        private async Task<ListResultDto<ChatMessageDto>> GetGroupChatMessagesAsync(GetUserChatMessagesInput input, long userId)
        {
            var messages = await (await _chatMessageRepository.GetAllAsync())
                .WhereIf(input.MinMessageId.HasValue, m => m.Id < input.MinMessageId.Value)
                .Where(m => m.TargetTenantId == input.TenantId && m.TargetUserId == 0)
                .OrderByDescending(m => m.CreationTime)
                .Take(100)
                .ToListAsync();

            messages.Reverse();
            var listMessages = ObjectMapper.Map<List<ChatMessageDto>>(messages);

            listMessages = listMessages.Select(x => new ChatMessageDto
            {
                CreationTime = x.CreationTime,
                Id = x.Id,
                Message = x.Message,
                ReadState = x.ReadState,
                ReceiverReadState = x.ReceiverReadState,
                SharedMessageId = x.SharedMessageId,
                Side = x.UserId == userId ? ChatSide.Sender : ChatSide.Receiver,
                TargetTenantId = x.TargetTenantId,
                TargetUserId = x.UserId,
                UserId = userId,
                TenantId = x.TenantId
            }).ToList();

            await SetTargetUserNamesAsync(listMessages);

            return new ListResultDto<ChatMessageDto>(listMessages);
        }

        private async Task SetTargetUserNamesAsync(List<ChatMessageDto> messages)
        {
            foreach (var message in messages)
            {
                try
                {
                    message.TargetUserName = (await UserManager.GetUserByIdAsync(message.TargetUserId)).Name;
                }
                catch
                {
                    message.TargetUserName = "";
                }
            }
        }

        [Produces("application/json", "application/json-patch+json", "text/json")]
        public async Task MarkAllUnreadMessagesOfUserAsRead(MarkAllUnreadMessagesOfUserAsReadInput input)
        {
            var userId = AbpSession.GetUserId();
            var tenantId = AbpSession.TenantId;

            if (input.UserId.HasValue && input.UserId.Value > 0)
            {
                await MarkUserMessagesAsReadAsync(input, userId, tenantId);
            }
            else if (input.GroupId.HasValue && input.GroupId.Value > 0)
            {
                await MarkGroupMessagesAsReadAsync(input, userId, tenantId);
            }
        }

        private async Task MarkUserMessagesAsReadAsync(MarkAllUnreadMessagesOfUserAsReadInput input, long userId, int? tenantId)
        {
            var messages = await (await _chatMessageRepository.GetAllAsync())
                .Where(m =>
                    m.UserId == userId &&
                    m.TargetTenantId == input.TenantId &&
                    m.TargetUserId == input.UserId &&
                    m.ReadState == ChatMessageReadState.Unread)
                .ToListAsync();

            if (!messages.Any())
            {
                return;
            }

            foreach (var message in messages)
            {
                message.ChangeReadState(ChatMessageReadState.Read);
            }

            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                var reverseMessages = await (await _chatMessageRepository.GetAllAsync())
                    .Where(m => m.UserId == input.UserId && m.TargetTenantId == tenantId && m.TargetUserId == userId)
                    .ToListAsync();

                if (!reverseMessages.Any())
                {
                    return;
                }

                foreach (var message in reverseMessages)
                {
                    message.ChangeReceiverReadState(ChatMessageReadState.Read);
                }
            }

            var userIdentifier = AbpSession.ToUserIdentifier();
            var friendIdentifier = input.ToUserIdentifier();

            _userFriendsCache.ResetUnreadMessageCount(userIdentifier, friendIdentifier);

            var onlineUserClients = await _onlineClientManager.GetAllByUserIdAsync(userIdentifier);
            if (onlineUserClients.Any())
            {
                await _chatCommunicator.SendAllUnreadMessagesOfUserReadToClients(onlineUserClients, friendIdentifier);
            }

            var onlineFriendClients = await _onlineClientManager.GetAllByUserIdAsync(friendIdentifier);
            if (onlineFriendClients.Any())
            {
                await _chatCommunicator.SendReadStateChangeToClients(onlineFriendClients, userIdentifier);
            }
        }

        private async Task MarkGroupMessagesAsReadAsync(MarkAllUnreadMessagesOfUserAsReadInput input, long userId, int? tenantId)
        {
            var messages = await (await _chatMessageRepository.GetAllAsync())
                .Where(m =>
                    m.UserId == 0 &&
                    m.TargetTenantId == input.TenantId &&
                    m.TargetUserId == userId &&
                    m.ReadState == ChatMessageReadState.Unread)
                .ToListAsync();

            if (!messages.Any())
            {
                return;
            }

            foreach (var message in messages)
            {
                message.ChangeReadState(ChatMessageReadState.Read);
            }

            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                var reverseMessages = await (await _chatMessageRepository.GetAllAsync())
                    .Where(m => m.UserId == 0
                        && m.TargetTenantId == tenantId
                        && m.TargetUserId == userId
                        && m.ReadState == ChatMessageReadState.Unread)
                    .ToListAsync();

                if (!reverseMessages.Any())
                {
                    return;
                }

                foreach (var message in reverseMessages)
                {
                    message.ChangeReceiverReadState(ChatMessageReadState.Read);
                }
            }

            var userIdentifier = AbpSession.ToUserIdentifier();

            var onlineUserClients = await _onlineClientManager.GetAllByUserIdAsync(userIdentifier);
            if (onlineUserClients.Any())
            {
                await _chatCommunicator.SendAllUnreadMessagesOfUserReadToClients(onlineUserClients, userIdentifier);
                await _chatCommunicator.SendReadStateChangeToClients(onlineUserClients, userIdentifier);
            }
        }
    }
}