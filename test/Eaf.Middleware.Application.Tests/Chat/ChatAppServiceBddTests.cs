using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using Abp.RealTime;
using Abp.Runtime.Session;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Chat.Dto;
using Eaf.Middleware.Friendships.Cache;
using Eaf.Middleware.Friendships.Dto;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Chat
{
    /// <summary>
    /// Testes BDD para ChatAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ChatAppServiceBddTests
    {
        private readonly IRepository<ChatMessage, long> _chatMessageRepository;
        private readonly IUserFriendsCache _userFriendsCache;
        private readonly IOnlineClientManager<ChatChannel> _onlineClientManager;
        private readonly IChatCommunicator _chatCommunicator;
        private readonly ChatAppService _sut;

        public ChatAppServiceBddTests()
        {
            _chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            _userFriendsCache = Substitute.For<IUserFriendsCache>();
            _onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            _chatCommunicator = Substitute.For<IChatCommunicator>();

            _sut = new ChatAppService(
                _chatMessageRepository,
                _userFriendsCache,
                _onlineClientManager,
                _chatCommunicator
            );
        }

        #region Construtor

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region GetUserChatFriendsWithSettingsAsync

        [Fact]
        public async Task Dado_UsuarioNaoLogado_Quando_GetUserChatFriendsWithSettings_Entao_DeveRetornarVazio()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns((long?)null);
            _sut.AbpSession = abpSession;

            // Quando
            var result = await _sut.GetUserChatFriendsWithSettingsAsync();

            // Então
            result.ShouldNotBeNull();
            result.Friends.ShouldBeEmpty();
        }

        [Fact]
        public async Task Dado_UsuarioLogadoSemAmigos_Quando_GetUserChatFriendsWithSettings_Entao_DeveRetornarListaVazia()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 42);
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            _sut.AbpSession = abpSession;

            var cacheItem = new UserWithFriendsCacheItem { Friends = new List<FriendCacheItem>() };
            _userFriendsCache.GetCacheItem(userIdentifier).Returns(cacheItem);

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<FriendshipDto>>(Arg.Any<object>())
                .Returns(new List<FriendshipDto>());
            _sut.ObjectMapper = objectMapper;

            var featureChecker = Substitute.For<Abp.Application.Features.IFeatureChecker>();
            featureChecker.IsEnabledAsync(1, AppFeatures.GroupChatFeature).Returns(false);
            _sut.FeatureChecker = featureChecker;

            // Quando
            var result = await _sut.GetUserChatFriendsWithSettingsAsync();

            // Então
            result.ShouldNotBeNull();
            result.Friends.ShouldNotBeNull();
            result.Friends.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Dado_UsuarioLogadoComAmigosEGroupChatAtivo_Quando_GetUserChatFriendsWithSettings_Entao_DeveRetornarAmigosEGrupo()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 42);
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            var friend = new FriendCacheItem
            {
                FriendTenantId = 1,
                FriendUserId = 10,
                FriendUserName = "amigo",
                Name = "Amigo",
                Surname = "Teste"
            };

            var cacheItem = new UserWithFriendsCacheItem
            {
                Friends = new List<FriendCacheItem> { friend }
            };
            _userFriendsCache.GetCacheItem(userIdentifier).Returns(cacheItem);

            var onlineClient = Substitute.For<IOnlineClient>();
            _onlineClientManager
                .GetAllByUserIdAsync(Arg.Any<IUserIdentifier>())
                .Returns(Task.FromResult<IReadOnlyList<IOnlineClient>>(new List<IOnlineClient> { onlineClient }));

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<FriendshipDto>>(Arg.Any<object>())
                .Returns(callInfo =>
                {
                    var source = callInfo.Arg<object>();
                    var items = (source as IEnumerable<FriendCacheItem>)!;
                    return items.Select(x => new FriendshipDto
                    {
                        FriendTenantId = x.FriendTenantId,
                        FriendUserId = x.FriendUserId,
                        FriendUserName = x.FriendUserName,
                        Name = x.Name,
                        Surname = x.Surname
                    }).ToList();
                });
            _sut.ObjectMapper = objectMapper;

            var featureChecker = Substitute.For<Abp.Application.Features.IFeatureChecker>();
            featureChecker.IsEnabledAsync(1, AppFeatures.GroupChatFeature).Returns(true);
            _sut.FeatureChecker = featureChecker;

            _chatMessageRepository.GetAll().Returns(new List<ChatMessage>().AsAsyncQueryable());

            // Quando
            var result = await _sut.GetUserChatFriendsWithSettingsAsync();

            // Então
            result.ShouldNotBeNull();
            result.Friends.Count.ShouldBe(2);
            result.Friends[0].GroupId.ShouldBe(1);
            result.Friends[1].FriendUserId.ShouldBe(friend.FriendUserId);
            result.Friends[1].IsOnline.ShouldBeTrue();
        }

        #endregion

        #region GetUserChatMessages

        [Fact]
        public async Task Dado_SemUserIdNemGroupId_Quando_GetUserChatMessages_Entao_DeveRetornarListaVazia()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            var input = new GetUserChatMessagesInput
            {
                TenantId = 1,
                UserId = null,
                GroupId = null
            };

            // Quando
            var result = await _sut.GetUserChatMessages(input);

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Dado_UserIdZero_Quando_GetUserChatMessages_Entao_DeveRetornarListaVazia()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            var input = new GetUserChatMessagesInput
            {
                TenantId = 1,
                UserId = 0,
                GroupId = 0
            };

            // Quando
            var result = await _sut.GetUserChatMessages(input);

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Dado_UsuarioLogadoComMensagensDeUsuario_Quando_GetUserChatMessages_Entao_DeveRetornarMensagensMapeadas()
        {
            // Dado
            var userId = 42L;
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(userId);
            _sut.AbpSession = abpSession;

            var message = new ChatMessage(
                new UserIdentifier(1, userId),
                new UserIdentifier(1, 10),
                ChatSide.Sender,
                "Ola",
                ChatMessageReadState.Unread,
                Guid.NewGuid(),
                ChatMessageReadState.Unread);

            _chatMessageRepository.GetAll().Returns(new List<ChatMessage> { message }.AsAsyncQueryable());

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<ChatMessageDto>>(Arg.Any<object>())
                .Returns(callInfo =>
                {
                    var source = callInfo.Arg<object>();
                    var items = (source as IEnumerable<ChatMessage>)!;
                    return items.Select(x => new ChatMessageDto
                    {
                        Id = (int)x.Id,
                        CreationTime = x.CreationTime,
                        Message = x.Message,
                        ReadState = x.ReadState,
                        ReceiverReadState = x.ReceiverReadState,
                        SharedMessageId = x.SharedMessageId?.ToString(),
                        Side = x.Side,
                        TargetTenantId = x.TargetTenantId,
                        TargetUserId = x.TargetUserId,
                        TenantId = x.TenantId,
                        UserId = x.UserId
                    }).ToList();
                });
            _sut.ObjectMapper = objectMapper;

            var input = new GetUserChatMessagesInput
            {
                TenantId = 1,
                UserId = 10,
                GroupId = null
            };

            // Quando
            var result = await _sut.GetUserChatMessages(input);

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
            result.Items[0].UserId.ShouldBe(userId);
            result.Items[0].TargetUserId.ShouldBe(10);
            result.Items[0].TargetUserName.ShouldBeEmpty();
        }

        [Fact]
        public async Task Dado_UsuarioLogadoComMensagensDeGrupo_Quando_GetUserChatMessages_Entao_DeveRetornarMensagensDeGrupoMapeadas()
        {
            // Dado
            var userId = 42L;
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(userId);
            _sut.AbpSession = abpSession;

            var message = new ChatMessage(
                new UserIdentifier(1, 2),
                new UserIdentifier(1, 0),
                ChatSide.Receiver,
                "Ola grupo",
                ChatMessageReadState.Unread,
                Guid.NewGuid(),
                ChatMessageReadState.Unread);

            _chatMessageRepository.GetAll().Returns(new List<ChatMessage> { message }.AsAsyncQueryable());

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<ChatMessageDto>>(Arg.Any<object>())
                .Returns(callInfo =>
                {
                    var source = callInfo.Arg<object>();
                    var items = (source as IEnumerable<ChatMessage>)!;
                    return items.Select(x => new ChatMessageDto
                    {
                        Id = (int)x.Id,
                        CreationTime = x.CreationTime,
                        Message = x.Message,
                        ReadState = x.ReadState,
                        ReceiverReadState = x.ReceiverReadState,
                        SharedMessageId = x.SharedMessageId?.ToString(),
                        Side = x.Side,
                        TargetTenantId = x.TargetTenantId,
                        TargetUserId = x.TargetUserId,
                        TenantId = x.TenantId,
                        UserId = x.UserId
                    }).ToList();
                });
            _sut.ObjectMapper = objectMapper;

            var input = new GetUserChatMessagesInput
            {
                TenantId = 1,
                UserId = null,
                GroupId = 1
            };

            // Quando
            var result = await _sut.GetUserChatMessages(input);

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
            result.Items[0].UserId.ShouldBe(userId);
            result.Items[0].TargetUserId.ShouldBe(2);
            result.Items[0].TargetUserName.ShouldBeEmpty();
        }

        #endregion

        #region MarkAllUnreadMessagesOfUserAsRead

        [Fact]
        public async Task Dado_UsuarioComMensagensNaoLidasDeUsuario_Quando_MarkAllUnreadMessagesOfUserAsRead_Entao_DeveMarcarComoLidaEComunicar()
        {
            // Dado
            var userId = 42L;
            var friendUserId = 10L;
            var tenantId = 1;

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(userId);
            abpSession.TenantId.Returns(tenantId);
            _sut.AbpSession = abpSession;

            var messageReceived = new ChatMessage(
                new UserIdentifier(tenantId, userId),
                new UserIdentifier(tenantId, friendUserId),
                ChatSide.Sender,
                "Ola",
                ChatMessageReadState.Unread,
                Guid.NewGuid(),
                ChatMessageReadState.Unread);

            var messageSent = new ChatMessage(
                new UserIdentifier(tenantId, friendUserId),
                new UserIdentifier(tenantId, userId),
                ChatSide.Receiver,
                "Resposta",
                ChatMessageReadState.Unread,
                Guid.NewGuid(),
                ChatMessageReadState.Unread);

            _chatMessageRepository.GetAll().Returns(new List<ChatMessage> { messageReceived, messageSent }.AsAsyncQueryable());

            var activeUnitOfWork = Substitute.For<IActiveUnitOfWork>();
            activeUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUnitOfWork);
            _sut.UnitOfWorkManager = unitOfWorkManager;

            var onlineClient = Substitute.For<IOnlineClient>();
            _onlineClientManager
                .GetAllByUserIdAsync(Arg.Any<IUserIdentifier>())
                .Returns(Task.FromResult<IReadOnlyList<IOnlineClient>>(new List<IOnlineClient> { onlineClient }));

            var input = new MarkAllUnreadMessagesOfUserAsReadInput
            {
                TenantId = tenantId,
                UserId = friendUserId
            };

            // Quando
            await _sut.MarkAllUnreadMessagesOfUserAsRead(input);

            // Então
            messageReceived.ReadState.ShouldBe(ChatMessageReadState.Read);
            messageSent.ReceiverReadState.ShouldBe(ChatMessageReadState.Read);
            _userFriendsCache.Received(1).ResetUnreadMessageCount(
                new UserIdentifier(tenantId, userId),
                new UserIdentifier(tenantId, friendUserId));
            await _chatCommunicator.Received(1).SendAllUnreadMessagesOfUserReadToClients(
                Arg.Is<IReadOnlyList<IOnlineClient>>(list => list.Count == 1),
                new UserIdentifier(tenantId, friendUserId));
            await _chatCommunicator.Received(1).SendReadStateChangeToClients(
                Arg.Is<IReadOnlyList<IOnlineClient>>(list => list.Count == 1),
                new UserIdentifier(tenantId, userId));
        }

        [Fact]
        public async Task Dado_UsuarioComMensagensNaoLidasDeGrupo_Quando_MarkAllUnreadMessagesOfUserAsRead_Entao_DeveMarcarComoLida()
        {
            // Dado
            var userId = 42L;
            var tenantId = 1;

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(userId);
            abpSession.TenantId.Returns(tenantId);
            _sut.AbpSession = abpSession;

            var message = new ChatMessage(
                new UserIdentifier(tenantId, 0),
                new UserIdentifier(tenantId, userId),
                ChatSide.Receiver,
                "Ola grupo",
                ChatMessageReadState.Unread,
                Guid.NewGuid(),
                ChatMessageReadState.Unread);

            _chatMessageRepository.GetAll().Returns(new List<ChatMessage> { message }.AsAsyncQueryable());

            var activeUnitOfWork = Substitute.For<IActiveUnitOfWork>();
            activeUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUnitOfWork);
            _sut.UnitOfWorkManager = unitOfWorkManager;

            var input = new MarkAllUnreadMessagesOfUserAsReadInput
            {
                TenantId = tenantId,
                GroupId = 1
            };

            // Quando
            await _sut.MarkAllUnreadMessagesOfUserAsRead(input);

            // Então
            message.ReadState.ShouldBe(ChatMessageReadState.Read);
            await _chatCommunicator.DidNotReceive().SendAllUnreadMessagesOfUserReadToClients(
                Arg.Any<IReadOnlyList<IOnlineClient>>(),
                Arg.Any<UserIdentifier>());
        }

        #endregion
    }
}
