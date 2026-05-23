using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Users;
using Shouldly;
using System.Security.Claims;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Authorization.Impersonation
{
    public class UserAndIdentityTests
    {
        [Fact]
        public void Dado_UserEIdentity_Quando_Criar_Entao_DeveArmazenarCorretamente()
        {
            var user = new User { Name = "Test" };
            var identity = new ClaimsIdentity("TestAuth");

            var result = new UserAndIdentity(user, identity);

            result.User.ShouldBe(user);
            result.Identity.ShouldBe(identity);
            result.User.Name.ShouldBe("Test");
            result.Identity.AuthenticationType.ShouldBe("TestAuth");
        }

        [Fact]
        public void Dado_UserAndIdentity_Quando_AlterarPropriedades_Entao_DeveAtualizar()
        {
            var user1 = new User { Name = "User1" };
            var identity1 = new ClaimsIdentity("Auth1");
            var result = new UserAndIdentity(user1, identity1);

            var user2 = new User { Name = "User2" };
            var identity2 = new ClaimsIdentity("Auth2");
            result.User = user2;
            result.Identity = identity2;

            result.User.Name.ShouldBe("User2");
            result.Identity.AuthenticationType.ShouldBe("Auth2");
        }
    }
}
