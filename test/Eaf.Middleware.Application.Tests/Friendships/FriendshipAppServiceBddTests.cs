using Abp;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.ObjectMapping;
using Abp.RealTime;
using Abp.Runtime.Session;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Dto;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Friendships
{
    /// <summary>
    /// Testes BDD para FriendshipAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class FriendshipAppServiceBddTests
    {
        private readonly IFriendshipManager _friendshipManager;
        private readonly IOnlineClientManager<ChatChannel> _onlineClientManager;
        private readonly IChatCommunicator _chatCommunicator;
        private readonly ITenantCache _tenantCache;
        private readonly IChatFeatureChecker _chatFeatureChecker;
        private readonly FriendshipAppService _sut;

        public FriendshipAppServiceBddTests()
        {
            _friendshipManager = Substitute.For<IFriendshipManager>();
            _onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            _chatCommunicator = Substitute.For<IChatCommunicator>();
            _tenantCache = Substitute.For<ITenantCache>();
            _chatFeatureChecker = Substitute.For<IChatFeatureChecker>();

            _sut = new FriendshipAppService(
                _friendshipManager,
                _onlineClientManager,
                _chatCommunicator,
                _tenantCache,
                _chatFeatureChecker
            );
        }

        #region Construtor

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region AcceptFriendshipRequest

        [Fact]
        public async Task Dado_SolicitacaoPendente_Quando_AcceptFriendshipRequest_Entao_DeveAceitar()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 42);
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            _onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>())
                .Returns(new List<IOnlineClient>());

            var input = new AcceptFriendshipRequestInput { TenantId = 1, UserId = 100 };

            // Quando
            await _sut.AcceptFriendshipRequest(input);

            // Então
            await _friendshipManager.Received(1)
                .AcceptFriendshipRequestAsync(
                    Arg.Is<UserIdentifier>(u => u.UserId == 42),
                    Arg.Is<UserIdentifier>(u => u.UserId == 100)
                );
        }

        [Fact]
        public async Task Dado_SolicitacaoPendenteComClientesOnline_Quando_AcceptFriendshipRequest_Entao_DeveNotificarClientes()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            var clients = new List<IOnlineClient> { Substitute.For<IOnlineClient>() };
            _onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(clients);

            var input = new AcceptFriendshipRequestInput { TenantId = 1, UserId = 100 };

            // Quando
            await _sut.AcceptFriendshipRequest(input);

            // Então
            await _chatCommunicator.Received(1)
                .SendUserStateChangeToClients(
                    clients,
                    Arg.Is<UserIdentifier>(u => u.UserId == 100),
                    FriendshipState.Accepted
                );
        }

        #endregion

        #region BlockUser

        [Fact]
        public async Task Dado_Amizade_Quando_BlockUser_Entao_DeveBanir()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 42);
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            _onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>())
                .Returns(new List<IOnlineClient>());

            var input = new BlockUserInput { TenantId = 1, UserId = 100 };

            // Quando
            await _sut.BlockUser(input);

            // Então
            await _friendshipManager.Received(1)
                .BanFriendAsync(
                    Arg.Is<UserIdentifier>(u => u.UserId == 42),
                    Arg.Is<UserIdentifier>(u => u.UserId == 100)
                );
        }

        #endregion

        #region UnblockUser

        [Fact]
        public async Task Dado_UsuarioBloqueado_Quando_UnblockUser_Entao_DeveDesbloquear()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 42);
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            _onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>())
                .Returns(new List<IOnlineClient>());

            var input = new UnblockUserInput { TenantId = 1, UserId = 100 };

            // Quando
            await _sut.UnblockUser(input);

            // Então
            await _friendshipManager.Received(1)
                .AcceptFriendshipRequestAsync(
                    Arg.Is<UserIdentifier>(u => u.UserId == 42),
                    Arg.Is<UserIdentifier>(u => u.UserId == 100)
                );
        }

        #endregion

        #region CreateFriendshipRequest

        [Fact]
        public async Task Dado_AmizadeJaExistente_Quando_CreateFriendshipRequest_Entao_DeveLancarExcecao()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 42);
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            var localizationManager = Substitute.For<Abp.Localization.ILocalizationManager>();
            _sut.LocalizationManager = localizationManager;

            var probableFriend = new UserIdentifier(1, 100);
            _friendshipManager.GetFriendshipOrNullAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>())
                .Returns(new Friendship(userIdentifier, probableFriend, "acme", "user100", null, FriendshipState.Accepted));

            var input = new CreateFriendshipRequestInput { TenantId = 1, UserId = 100 };

            // Quando / Então
            await Should.ThrowAsync<Abp.UI.UserFriendlyException>(() =>
                _sut.CreateFriendshipRequest(input));
        }

        [Fact]
        public async Task Dado_NovaSolicitacaoDeAmizade_Quando_CreateFriendshipRequest_Entao_DeveCriarAmizadeERetornarDto()
        {
            // Dado
            var userId = 42L;
            var friendId = 100L;
            var tenantId = 1;

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(tenantId);
            abpSession.UserId.Returns(userId);
            _sut.AbpSession = abpSession;

            var currentUser = new User { Id = userId, UserName = "user42", TenantId = tenantId };
            var friendUser = new User { Id = friendId, UserName = "user100", TenantId = tenantId };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync(userId.ToString()).Returns(currentUser);
            userManager.FindByIdAsync(friendId.ToString()).Returns(friendUser);
            _sut.UserManager = userManager;

            var currentUow = Substitute.For<IActiveUnitOfWork>();
            currentUow.SetTenantId(default(int?)).ReturnsForAnyArgs(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(currentUow);
            _sut.UnitOfWorkManager = unitOfWorkManager;

            _friendshipManager.GetFriendshipOrNullAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>()).Returns((Friendship?)null);
            _onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(new List<IOnlineClient>());
            _tenantCache.Get(tenantId).Returns(new TenantCacheItem { TenancyName = "acme" });

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<FriendshipDto>(Arg.Any<Friendship>()).Returns(new FriendshipDto { FriendUserId = friendId, FriendUserName = "user100" });
            _sut.ObjectMapper = objectMapper;

            var input = new CreateFriendshipRequestInput { TenantId = tenantId, UserId = friendId };

            // Quando
            var result = await _sut.CreateFriendshipRequest(input);

            // Então
            result.ShouldNotBeNull();
            result.FriendUserId.ShouldBe(friendId);
            result.FriendUserName.ShouldBe("user100");
            await _friendshipManager.Received(2).CreateFriendshipAsync(Arg.Any<Friendship>());
            await _friendshipManager.Received(2).AcceptFriendshipRequestAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>());
        }

        #endregion
    }
}
