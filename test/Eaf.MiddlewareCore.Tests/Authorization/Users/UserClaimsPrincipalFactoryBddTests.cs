using Abp;
using Abp.Runtime.Security;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Tests.Helpers;
using Shouldly;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Authorization.Users
{
    /// <summary>
    /// Testes BDD para UserClaimsPrincipalFactory exercitando criação de ClaimsPrincipal.
    /// </summary>
    public class UserClaimsPrincipalFactoryBddTests
    {
        [Fact]
        public async Task Dado_UsuarioTenant_Quando_CreateAsync_Entao_DeveRetornarClaimsPrincipalComTenantId()
        {
            // Dado
            var user = new User
            {
                Id = 1,
                TenantId = 1,
                UserName = "admin",
                Name = "Admin",
                Surname = "User",
                EmailAddress = "admin@example.com"
            };
            var factory = CoreManagerTestHelper.CreateUserClaimsPrincipalFactory();

            // Quando
            var principal = await factory.CreateAsync(user);

            // Então
            principal.ShouldNotBeNull();
            principal.Identity.ShouldNotBeNull();
            ((ClaimsIdentity)principal.Identity).HasClaim(AbpClaimTypes.TenantId, "1").ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_UsuarioHost_Quando_CreateAsync_Entao_DeveRetornarClaimsPrincipal()
        {
            // Dado
            var user = new User
            {
                Id = 1,
                TenantId = null,
                UserName = "admin",
                Name = "Admin",
                Surname = "User",
                EmailAddress = "admin@example.com"
            };
            var factory = CoreManagerTestHelper.CreateUserClaimsPrincipalFactory();

            // Quando
            var principal = await factory.CreateAsync(user);

            // Então
            principal.ShouldNotBeNull();
            principal.Identity.ShouldNotBeNull();
        }
    }
}
