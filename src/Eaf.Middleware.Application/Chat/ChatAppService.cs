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
using System;
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
                    UnreadMessageCount = await GetGroupUnreadMessageCountAsync(userIdentifier)
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

        [DisableAuditing]
        [Produces("application/json", "application/json-patch+json", "text/json")]
        public async Task<ListResultDto<ChatMessageDto>> GetHistoryAsync(GetChatHistoryInput input)
        {
            var query = await _chatMessageRepository.GetAllAsync();

            if (input.ConversationId.HasValue)
            {
                query = query.Where(m => m.ConversationId == input.ConversationId.Value);
            }

            if (input.GameId.HasValue)
            {
                query = query.Where(m => m.GameId == input.GameId.Value);
            }

            if (input.MatchId.HasValue)
            {
                query = query.Where(m => m.MatchId == input.MatchId.Value);
            }

            if (!string.IsNullOrWhiteSpace(input.ContextType))
            {
                query = query.Where(m => m.ContextType == input.ContextType);
            }

            if (input.MinMessageId.HasValue)
            {
                query = query.Where(m => m.Id < input.MinMessageId.Value);
            }

            var takeCount = input.MaxResultCount ?? 100;

            var messages = await query
                .OrderByDescending(m => m.CreationTime)
                .Take(takeCount)
                .ToListAsync();

            messages.Reverse();
            var listMessages = ObjectMapper.Map<List<ChatMessageDto>>(messages);
            await SetTargetUserNamesAsync(listMessages);
            return new ListResultDto<ChatMessageDto>(listMessages);
        }

        [Produces("application/json", "application/json-patch+json", "text/json")]
        public async Task MarkReadAsync(MarkChatReadInput input)
        {
            if (input.UserId.HasValue && input.UserId.Value > 0)
            {
                await MarkAllUnreadMessagesOfUserAsRead(new MarkAllUnreadMessagesOfUserAsReadInput
                {
                    TenantId = input.TenantId,
                    UserId = input.UserId
                });
                return;
            }

            if (input.GroupId.HasValue && input.GroupId.Value > 0)
            {
                await MarkAllUnreadMessagesOfUserAsRead(new MarkAllUnreadMessagesOfUserAsReadInput
                {
                    TenantId = input.TenantId,
                    GroupId = input.GroupId
                });
                return;
            }

            if (!input.ConversationId.HasValue && !input.GameId.HasValue && !input.MatchId.HasValue)
            {
                return;
            }

            var userId = AbpSession.GetUserId();
            var tenantId = AbpSession.TenantId;
            var query = await _chatMessageRepository.GetAllAsync();
            query = query.Where(m => m.TargetUserId == userId && m.TargetTenantId == tenantId);

            if (input.ConversationId.HasValue)
            {
                query = query.Where(m => m.ConversationId == input.ConversationId.Value);
            }

            if (input.GameId.HasValue)
            {
                query = query.Where(m => m.GameId == input.GameId.Value);
            }

            if (input.MatchId.HasValue)
            {
                query = query.Where(m => m.MatchId == input.MatchId.Value);
            }

            var messages = await query
                .Where(m => m.ReadState == ChatMessageReadState.Unread)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.ChangeReadState(ChatMessageReadState.Read);
            }
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
            if (messages == null || messages.Count == 0)
                return;

            var userIds = messages.Select(m => m.TargetUserId).Distinct().ToList();
            var userNames = new Dictionary<long, string>();

            if (UserManager != null)
            {
                try
                {
                    userNames = await UserManager.Users
                        .AsNoTracking()
                        .Where(u => userIds.Contains(u.Id))
                        .ToDictionaryAsync(u => u.Id, u => u.Name);
                }
                catch (Exception)
                {
                    // Ignora falhas do IQueryable para tentar carregamento individual.
                }

                if (userNames.Count == 0)
                {
                    foreach (var id in userIds)
                    {
                        try
                        {
                            var user = await UserManager.GetUserByIdAsync(id);
                            if (user != null)
                                userNames[id] = user.Name;
                        }
                        catch
                        {
                            // Usuário não encontrado: nome permanecerá vazio.
                        }
                    }
                }
            }

            foreach (var message in messages)
            {
                message.TargetUserName = userNames.TryGetValue(message.TargetUserId, out var name) ? name : "";
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
            const int batchSize = 1000;

            var firstDirectionQuery = (await _chatMessageRepository.GetAllAsync())
                .Where(m =>
                    m.UserId == userId &&
                    m.TargetTenantId == input.TenantId &&
                    m.TargetUserId == input.UserId &&
                    m.ReadState == ChatMessageReadState.Unread);

            var firstDirectionCount = await MarkMessagesAsReadInBatchesAsync(firstDirectionQuery, m => m.ChangeReadState(ChatMessageReadState.Read), batchSize);

            if (firstDirectionCount == 0)
            {
                return;
            }

            int reverseDirectionCount;
            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                var reverseDirectionQuery = (await _chatMessageRepository.GetAllAsync())
                    .Where(m =>
                        m.UserId == input.UserId &&
                        m.TargetTenantId == tenantId &&
                        m.TargetUserId == userId &&
                        m.ReceiverReadState == ChatMessageReadState.Unread);

                reverseDirectionCount = await MarkMessagesAsReadInBatchesAsync(reverseDirectionQuery, m => m.ChangeReceiverReadState(ChatMessageReadState.Read), batchSize);
            }

            if (firstDirectionCount == 0 && reverseDirectionCount == 0)
            {
                return;
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
            const int batchSize = 1000;

            var firstDirectionQuery = (await _chatMessageRepository.GetAllAsync())
                .Where(m =>
                    m.UserId == 0 &&
                    m.TargetTenantId == input.TenantId &&
                    m.TargetUserId == userId &&
                    m.ReadState == ChatMessageReadState.Unread);

            var messagesCount = await MarkMessagesAsReadInBatchesAsync(firstDirectionQuery, m => m.ChangeReadState(ChatMessageReadState.Read), batchSize);

            if (messagesCount == 0)
            {
                return;
            }

            int reverseMessagesCount;
            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                var reverseDirectionQuery = (await _chatMessageRepository.GetAllAsync())
                    .Where(m =>
                        m.UserId == 0 &&
                        m.TargetTenantId == tenantId &&
                        m.TargetUserId == userId &&
                        m.ReadState == ChatMessageReadState.Unread);

                reverseMessagesCount = await MarkMessagesAsReadInBatchesAsync(reverseDirectionQuery, m => m.ChangeReceiverReadState(ChatMessageReadState.Read), batchSize);
            }

            if (messagesCount == 0 && reverseMessagesCount == 0)
            {
                return;
            }

            var userIdentifier = AbpSession.ToUserIdentifier();

            var onlineUserClients = await _onlineClientManager.GetAllByUserIdAsync(userIdentifier);
            if (onlineUserClients.Any())
            {
                await _chatCommunicator.SendAllUnreadMessagesOfUserReadToClients(onlineUserClients, userIdentifier);
                await _chatCommunicator.SendReadStateChangeToClients(onlineUserClients, userIdentifier);
            }
        }

        private async Task<int> MarkMessagesAsReadInBatchesAsync(IQueryable<ChatMessage> query, Action<ChatMessage> markAsRead, int batchSize)
        {
            int totalMarked = 0;

            while (true)
            {
                var batch = await query.Take(batchSize).ToListAsync();
                if (batch.Count == 0)
                {
                    break;
                }

                for (int i = 0; i < batch.Count; i++)
                {
                    markAsRead(batch[i]);
                }

                totalMarked += batch.Count;

                if (batch.Count < batchSize)
                {
                    break;
                }
            }

            return totalMarked;
        }

        private async Task<int> GetGroupUnreadMessageCountAsync(UserIdentifier userIdentifier)
        {
            var query = (await _chatMessageRepository.GetAllAsync())
                .Where(cm =>
                    cm.ReadState == ChatMessageReadState.Unread &&
                    cm.TenantId == userIdentifier.TenantId &&
                    cm.TargetTenantId == userIdentifier.TenantId &&
                    cm.Side == ChatSide.Receiver);

            var unreadAsSender = await query
                .CountAsync(cm => cm.UserId == userIdentifier.UserId && cm.TargetUserId == 0);

            var unreadAsReceiver = await query
                .CountAsync(cm => cm.UserId == 0 && cm.TargetUserId == userIdentifier.UserId);

            return unreadAsSender + unreadAsReceiver;
        }
    }
}