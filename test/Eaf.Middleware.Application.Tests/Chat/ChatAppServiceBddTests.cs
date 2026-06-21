using Abp;
using Abp.Domain.Repositories;
using Abp.RealTime;
using Abp.Runtime.Session;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Chat.Dto;
using Eaf.Middleware.Friendships.Cache;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
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

            var objectMapper = Substitute.For<Abp.ObjectMapping.IObjectMapper>();
            objectMapper.Map<List<Eaf.Middleware.Friendships.Dto.FriendshipDto>>(Arg.Any<object>())
                .Returns(new List<Eaf.Middleware.Friendships.Dto.FriendshipDto>());
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

        #endregion
    }
}
