using System.Security.Claims;
using System.Threading.Tasks;
using Eaf.Middleware;
using Eaf.MiddlewareCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.SampleApp.Core
{
    public class UserClaimsPrincipalFactoryBddTests : EafMiddlewareTestBase
    {
        [Fact]
        public async Task Dado_UsuarioAutenticado_Quando_CreateAsync_Entao_DeveRetornarClaimsPrincipal()
        {
            var factory = LocalIocManager.Resolve<Microsoft.AspNetCore.Identity.IUserClaimsPrincipalFactory<User>>();
            var user = GetCurrentUser();

            var principal = await factory.CreateAsync(user);

            principal.ShouldNotBeNull();
            principal.Identity.ShouldNotBeNull();
            principal.Identity.IsAuthenticated.ShouldBeTrue();
            principal.Claims.ShouldContain(c => c.Type == ClaimTypes.NameIdentifier);
        }
    }
}
