using Eaf.Middleware.Authorization.Users.Profile;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Web.Controllers;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Controllers
{
    public class ProfileControllerBddTests
    {
        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var tempFileCacheManager = Substitute.For<ITempFileCacheManager>();
            var profileAppService = Substitute.For<IProfileAppService>();
            var sut = new ProfileController(tempFileCacheManager, profileAppService);
            sut.ShouldNotBeNull();
        }
    }
}
