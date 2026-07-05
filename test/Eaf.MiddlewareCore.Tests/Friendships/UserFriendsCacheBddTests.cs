using Abp.Dependency;
using Eaf.Middleware.Friendships.Cache;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Friendships
{
    /// <summary>
    /// Testes BDD para UserFriendsCache seguindo o padrão Dado/Quando/Então.
    /// O construtor depende de UserStore (classe concreta não mockável),
    /// portanto validam-se características de tipo e contrato.
    /// </summary>
    public class UserFriendsCacheBddTests
    {
        [Fact]
        public void Dado_TipoUserFriendsCache_Quando_Verificar_Entao_DeveImplementarIUserFriendsCache()
        {
            typeof(IUserFriendsCache).IsAssignableFrom(typeof(UserFriendsCache)).ShouldBeTrue();
        }

        [Fact]
        public void Dado_TipoUserFriendsCache_Quando_Verificar_Entao_DeveSerSingletonDependency()
        {
            typeof(ISingletonDependency).IsAssignableFrom(typeof(UserFriendsCache)).ShouldBeTrue();
        }
    }
}
