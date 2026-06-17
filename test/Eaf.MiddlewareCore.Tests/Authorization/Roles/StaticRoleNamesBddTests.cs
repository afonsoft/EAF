using Eaf.Middleware.Authorization.Roles;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Roles
{
    /// <summary>
    /// Testes BDD para StaticRoleNames seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class StaticRoleNamesBddTests
    {
        [Fact]
        public void Dado_HostAdmin_Quando_Verificar_Entao_DeveSerAdmin()
        {
            StaticRoleNames.Host.Admin.ShouldBe("Admin");
        }

        [Fact]
        public void Dado_HostUser_Quando_Verificar_Entao_DeveSerUser()
        {
            StaticRoleNames.Host.User.ShouldBe("User");
        }

        [Fact]
        public void Dado_TenantsAdmin_Quando_Verificar_Entao_DeveSerAdmin()
        {
            StaticRoleNames.Tenants.Admin.ShouldBe("Admin");
        }

        [Fact]
        public void Dado_TenantsUser_Quando_Verificar_Entao_DeveSerUser()
        {
            StaticRoleNames.Tenants.User.ShouldBe("User");
        }
    }
}
