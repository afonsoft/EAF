using Abp.Domain.Repositories;
using Eaf.Middleware.Friendships;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Friendships
{
    public class FriendshipManagerBddTests
    {
        [Fact]
        public void Dado_Repository_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var friendshipRepository = Substitute.For<IRepository<Friendship, long>>();
            var sut = new FriendshipManager(friendshipRepository);
            sut.ShouldNotBeNull();
        }
    }
}
