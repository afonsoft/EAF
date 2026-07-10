using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Eaf.Middleware.Friendships;
using NSubstitute;
using Shouldly;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Friendships
{
    public class FriendshipManagerBddTests
    {
        private readonly IRepository<Friendship, long> _friendshipRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly FriendshipManager _sut;

        public FriendshipManagerBddTests()
        {
            _friendshipRepository = Substitute.For<IRepository<Friendship, long>>();
            _unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _unitOfWorkManager.Current.Returns(_unitOfWork);
            _unitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            _unitOfWork.SaveChangesAsync().Returns(Task.CompletedTask);

            _sut = new FriendshipManager(_friendshipRepository)
            {
                UnitOfWorkManager = _unitOfWorkManager
            };
        }

        [Fact]
        public void Dado_Repository_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_AmigosDistintos_Quando_CreateFriendshipAsync_Entao_DeveInserirESalvar()
        {
            // Dado
            var user = new UserIdentifier(1, 10);
            var friend = new UserIdentifier(2, 20);
            var friendship = new Friendship(user, friend, "acme", "friend", null, FriendshipState.Accepted);

            // Quando
            await _sut.CreateFriendshipAsync(friendship);

            // Então
            _friendshipRepository.Received(1).Insert(friendship);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task Dado_MesmoUsuario_Quando_CreateFriendshipAsync_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var user = new UserIdentifier(1, 10);
            var friendship = new Friendship(user, user, "acme", "same", null, FriendshipState.Accepted);

            // Quando / Então
            var exception = await Should.ThrowAsync<UserFriendlyException>(
                async () => await _sut.CreateFriendshipAsync(friendship));
            exception.Message.ShouldBe("YouCannotBeFriendWithYourself");
        }

        [Fact]
        public async Task Dado_AmizadeExistente_Quando_AcceptFriendshipRequestAsync_Entao_DeveAtualizarEstadoParaAceito()
        {
            // Dado
            var user = new UserIdentifier(1, 10);
            var friend = new UserIdentifier(2, 20);
            var friendship = new Friendship(user, friend, "acme", "friend", null, FriendshipState.Blocked);
            _friendshipRepository
                .FirstOrDefaultAsync(Arg.Any<Expression<Func<Friendship, bool>>>())
                .Returns(Task.FromResult(friendship));

            // Quando
            await _sut.AcceptFriendshipRequestAsync(user, friend);

            // Então
            friendship.State.ShouldBe(FriendshipState.Accepted);
            _friendshipRepository.Received(1).Update(friendship);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task Dado_AmizadeInexistente_Quando_AcceptFriendshipRequestAsync_Entao_DeveLancarAbpException()
        {
            // Dado
            var user = new UserIdentifier(1, 10);
            var friend = new UserIdentifier(2, 20);
            _friendshipRepository
                .FirstOrDefaultAsync(Arg.Any<Expression<Func<Friendship, bool>>>())
                .Returns(Task.FromResult<Friendship>(null!));

            // Quando / Então
            var exception = await Should.ThrowAsync<AbpException>(
                async () => await _sut.AcceptFriendshipRequestAsync(user, friend));
            exception.Message.ShouldContain("Friendship does not exist between");
        }

        [Fact]
        public async Task Dado_AmizadeExistente_Quando_BanFriendAsync_Entao_DeveAtualizarEstadoParaBloqueado()
        {
            // Dado
            var user = new UserIdentifier(1, 10);
            var friend = new UserIdentifier(2, 20);
            var friendship = new Friendship(user, friend, "acme", "friend", null, FriendshipState.Accepted);
            _friendshipRepository
                .FirstOrDefaultAsync(Arg.Any<Expression<Func<Friendship, bool>>>())
                .Returns(Task.FromResult(friendship));

            // Quando
            await _sut.BanFriendAsync(user, friend);

            // Então
            friendship.State.ShouldBe(FriendshipState.Blocked);
            _friendshipRepository.Received(1).Update(friendship);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task Dado_AmizadeExistente_Quando_GetFriendshipOrNullAsync_Entao_DeveRetornarAmizade()
        {
            // Dado
            var user = new UserIdentifier(1, 10);
            var friend = new UserIdentifier(2, 20);
            var friendship = new Friendship(user, friend, "acme", "friend", null, FriendshipState.Accepted);
            _friendshipRepository
                .FirstOrDefaultAsync(Arg.Any<Expression<Func<Friendship, bool>>>())
                .Returns(Task.FromResult(friendship));

            // Quando
            var result = await _sut.GetFriendshipOrNullAsync(user, friend);

            // Então
            result.ShouldNotBeNull();
            result.FriendUserId.ShouldBe(friend.UserId);
        }

        [Fact]
        public async Task Dado_AmizadeInexistente_Quando_GetFriendshipOrNullAsync_Entao_DeveRetornarNulo()
        {
            // Dado
            var user = new UserIdentifier(1, 10);
            var friend = new UserIdentifier(2, 20);
            _friendshipRepository
                .FirstOrDefaultAsync(Arg.Any<Expression<Func<Friendship, bool>>>())
                .Returns(Task.FromResult<Friendship>(null!));

            // Quando
            var result = await _sut.GetFriendshipOrNullAsync(user, friend);

            // Então
            result.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_MesmoTenantEUsuariosDistintos_Quando_CreateFriendshipAsync_Entao_DeveInserirESalvar()
        {
            // Dado
            var user = new UserIdentifier(1, 10);
            var friend = new UserIdentifier(1, 20);
            var friendship = new Friendship(user, friend, "acme", "friend", null, FriendshipState.Accepted);

            // Quando
            await _sut.CreateFriendshipAsync(friendship);

            // Então
            _friendshipRepository.Received(1).Insert(friendship);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task Dado_AmizadeExistente_Quando_UpdateFriendshipAsync_Entao_DeveAtualizarESalvar()
        {
            // Dado
            var user = new UserIdentifier(1, 10);
            var friend = new UserIdentifier(1, 20);
            var friendship = new Friendship(user, friend, "acme", "friend", null, FriendshipState.Accepted);

            // Quando
            await _sut.UpdateFriendshipAsync(friendship);

            // Então
            _friendshipRepository.Received(1).Update(friendship);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }
    }
}
