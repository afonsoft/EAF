using Eaf.Middleware.Storage;
using Eaf.Middleware.Web.Controllers;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Controllers
{
    /// <summary>
    /// Testes BDD para FileController seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class FileControllerBddTests
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly FileController _sut;

        public FileControllerBddTests()
        {
            _tempFileCacheManager = Substitute.For<ITempFileCacheManager>();
            _binaryObjectManager = Substitute.For<IBinaryObjectManager>();
            _sut = new FileController(_tempFileCacheManager, _binaryObjectManager);
        }

        #region Instanciacao

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_TempFileCacheManager_Quando_CriarInstancia_Entao_DeveAceitarDependencia()
        {
            var tempManager = Substitute.For<ITempFileCacheManager>();
            var binaryManager = Substitute.For<IBinaryObjectManager>();
            var controller = new FileController(tempManager, binaryManager);
            controller.ShouldNotBeNull();
        }

        #endregion
    }
}
