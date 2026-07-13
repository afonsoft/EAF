using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Eaf.Middleware.Friendships;
using NSubstitute;
using Shouldly;
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Friendships
{
    public class FriendshipManagerBddTests
    {
        private static FriendshipManager CriarSut(
            IRepository<Friendship, long>? friendshipRepository = null,
            Friendship? friendshipToReturn = null)
        {
            var repository = friendshipRepository ?? Substitute.For<IRepository<Friendship, long>>();

            repository.FirstOrDefaultAsync(Arg.Any<Expression<Func<Friendship, bool>>>())
                .Returns(Task.FromResult<Friendship>(friendshipToReturn!));

            repository.InsertAsync(Arg.Any<Friendship>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<Friendship>()));
            repository.UpdateAsync(Arg.Any<Friendship>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<Friendship>()));

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            activeUow.SaveChangesAsync().Returns(Task.FromResult(0));

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var manager = new FriendshipManager(repository)
            {
                UnitOfWorkManager = unitOfWorkManager
            };

            return manager;
        }

        private static Friendship CriarFriendship(
            long userId = 1,
            int? tenantId = null,
            long friendUserId = 2,
            int? friendTenantId = null,
            FriendshipState state = FriendshipState.Accepted)
        {
            var user = new UserIdentifier(tenantId, userId);
            var friend = new UserIdentifier(friendTenantId, friendUserId);
            return new Friendship(
                user,
                friend,
                "tenant",
                "friendUser",
                null,
                state);
        }

        [Fact]
        public async Task Dado_AmizadeValida_Quando_CreateFriendshipAsync_Entao_DeveInserirNoRepositorio()
        {
            var repository = Substitute.For<IRepository<Friendship, long>>();
            var sut = CriarSut(repository, friendshipToReturn: null);
            var friendship = CriarFriendship(userId: 1, friendUserId: 2);

            await sut.CreateFriendshipAsync(friendship);

            await repository.Received(1).InsertAsync(Arg.Is<Friendship>(f => f.UserId == 1 && f.FriendUserId == 2));
        }

        [Fact]
        public async Task Dado_UsuarioTentandoSerAmigoDeSiMesmo_Quando_CreateFriendshipAsync_Entao_DeveLancarUserFriendlyException()
        {
            var sut = CriarSut();
            var friendship = CriarFriendship(userId: 1, tenantId: 1, friendUserId: 1, friendTenantId: 1);

            var ex = await Should.ThrowAsync<UserFriendlyException>(async () => await sut.CreateFriendshipAsync(friendship));
            ex.Message.ShouldBe("YouCannotBeFriendWithYourself");
        }

        [Fact]
        public async Task Dado_AmizadeExistente_Quando_AcceptFriendshipRequestAsync_Entao_DeveAlterarEstadoParaAccepted()
        {
            var friendship = CriarFriendship(state: FriendshipState.Blocked);
            var sut = CriarSut(friendshipToReturn: friendship);

            await sut.AcceptFriendshipRequestAsync(
                new UserIdentifier(null, 1),
                new UserIdentifier(null, 2));

            friendship.State.ShouldBe(FriendshipState.Accepted);
        }

        [Fact]
        public async Task Dado_AmizadeNaoExistente_Quando_AcceptFriendshipRequestAsync_Entao_DeveLancarAbpException()
        {
            var sut = CriarSut(friendshipToReturn: null);

            var ex = await Should.ThrowAsync<AbpException>(async () => await sut.AcceptFriendshipRequestAsync(
                new UserIdentifier(null, 1),
                new UserIdentifier(null, 2)));
            ex.Message.ShouldContain("Friendship does not exist");
        }

        [Fact]
        public async Task Dado_AmizadeExistente_Quando_BanFriendAsync_Entao_DeveAlterarEstadoParaBlocked()
        {
            var friendship = CriarFriendship(state: FriendshipState.Accepted);
            var sut = CriarSut(friendshipToReturn: friendship);

            await sut.BanFriendAsync(
                new UserIdentifier(null, 1),
                new UserIdentifier(null, 2));

            friendship.State.ShouldBe(FriendshipState.Blocked);
        }

        [Fact]
        public async Task Dado_AmizadeNaoExistente_Quando_BanFriendAsync_Entao_DeveLancarAbpException()
        {
            var sut = CriarSut(friendshipToReturn: null);

            var ex = await Should.ThrowAsync<AbpException>(async () => await sut.BanFriendAsync(
                new UserIdentifier(null, 1),
                new UserIdentifier(null, 2)));
            ex.Message.ShouldContain("Friendship does not exist");
        }

        [Fact]
        public async Task Dado_Amizade_Quando_UpdateFriendshipAsync_Entao_DeveAtualizarNoRepositorio()
        {
            var repository = Substitute.For<IRepository<Friendship, long>>();
            var sut = CriarSut(repository, friendshipToReturn: null);
            var friendship = CriarFriendship();

            await sut.UpdateFriendshipAsync(friendship);

            await repository.Received(1).UpdateAsync(Arg.Is<Friendship>(f => f.UserId == friendship.UserId));
        }

        [Fact]
        public async Task Dado_Amizade_Quando_GetFriendshipOrNullAsync_Entao_DeveRetornarAmizade()
        {
            var friendship = CriarFriendship();
            var sut = CriarSut(friendshipToReturn: friendship);

            var result = await sut.GetFriendshipOrNullAsync(
                new UserIdentifier(null, 1),
                new UserIdentifier(null, 2));

            result.ShouldNotBeNull();
            result!.UserId.ShouldBe(1);
            result.FriendUserId.ShouldBe(2);
        }

        [Fact]
        public void Dado_ChaveLocalizada_Quando_LComArgs_Entao_DeveRetornarTextoFormatado()
        {
            var sut = new TestableFriendshipManager(Substitute.For<IRepository<Friendship, long>>());
            var result = sut.LocalizeComArgs("TestKey", "arg1");
            result.ShouldBe("TestKey");
        }

        [Fact]
        public void Dado_ChaveLocalizada_Quando_LComCultura_Entao_DeveRetornarChave()
        {
            var sut = new TestableFriendshipManager(Substitute.For<IRepository<Friendship, long>>());
            var result = sut.LocalizeComCultura("TestKey", CultureInfo.InvariantCulture);
            result.ShouldBe("TestKey");
        }

        public class TestableFriendshipManager : FriendshipManager
        {
            public TestableFriendshipManager(IRepository<Friendship, long> friendshipRepository)
                : base(friendshipRepository)
            {
            }

            public string LocalizeComArgs(string name, params object[] args)
            {
                return L(name, args);
            }

            public string LocalizeComCultura(string name, CultureInfo culture)
            {
                return L(name, culture);
            }
        }
    }
}
