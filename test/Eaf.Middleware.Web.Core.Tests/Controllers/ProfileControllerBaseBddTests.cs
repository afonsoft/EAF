using Eaf.Middleware.Authorization.Users.Profile;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Web.Controllers;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Controllers
{
    /// <summary>
    /// Testes BDD para ProfileControllerBase seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class ProfileControllerBaseBddTests
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IProfileAppService _profileAppService;

        public ProfileControllerBaseBddTests()
        {
            _tempFileCacheManager = Substitute.For<ITempFileCacheManager>();
            _profileAppService = Substitute.For<IProfileAppService>();
        }

        private sealed class TestableProfileController : ProfileControllerBase
        {
            public TestableProfileController(
                ITempFileCacheManager tempFileCacheManager,
                IProfileAppService profileAppService)
                : base(tempFileCacheManager, profileAppService)
            {
            }

            public new Microsoft.AspNetCore.Mvc.FileResult GetDefaultProfilePicture()
            {
                return GetDefaultProfilePictureInternal();
            }
        }

        #region Instanciacao

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new TestableProfileController(_tempFileCacheManager, _profileAppService);
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveHerdarDeMiddlewareControllerBase()
        {
            var sut = new TestableProfileController(_tempFileCacheManager, _profileAppService);
            sut.ShouldBeAssignableTo<MiddlewareControllerBase>();
        }

        #endregion
    }
}
