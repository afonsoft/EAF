using Abp.Runtime.Caching;
using Eaf.Middleware.Storage;
using NSubstitute;
using Shouldly;
using System;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Storage
{
    /// <summary>
    /// Testes BDD para TempFileCacheManager seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class TempFileCacheManagerBddTests
    {
        private readonly ICacheManager _cacheManager;
        private readonly ICache _cache;
        private readonly TempFileCacheManager _sut;

        public TempFileCacheManagerBddTests()
        {
            _cacheManager = Substitute.For<ICacheManager>();
            _cache = Substitute.For<ICache>();
            _cacheManager.GetCache(TempFileCacheManager.TempFileCacheName).Returns(_cache);
            _sut = new TempFileCacheManager(_cacheManager);
        }

        #region GetFile

        [Fact]
        public void Dado_TokenValido_Quando_GetFile_Entao_DeveRetornarBytes()
        {
            // Dado
            var token = "file-token-123";
            var fileBytes = new byte[] { 1, 2, 3, 4 };
            _cache.Get(token, Arg.Any<Func<string, object>>()).Returns(fileBytes);

            // Quando
            var result = _sut.GetFile(token);

            // Entao
            result.ShouldBe(fileBytes);
        }

        [Fact]
        public void Dado_TokenInvalido_Quando_GetFile_Entao_DeveRetornarNull()
        {
            // Dado
            var token = "invalid-token";
            _cache.Get(token, Arg.Any<Func<string, object>>()).Returns((object)null);

            // Quando
            var result = _sut.GetFile(token);

            // Entao
            result.ShouldBeNull();
        }

        #endregion

        #region SetFile

        [Fact]
        public void Dado_TokenEConteudo_Quando_SetFile_Entao_DeveArmazenarNoCache()
        {
            // Dado
            var token = "file-token-456";
            var content = new byte[] { 10, 20, 30 };

            // Quando
            _sut.SetFile(token, content);

            // Entao
            _cache.Received(1).Set(token, content, Arg.Any<TimeSpan>());
        }

        [Fact]
        public void Dado_TokenEConteudo_Quando_SetFile_Entao_DeveUsarExpiracao5Minutos()
        {
            // Dado
            var token = "file-token-789";
            var content = new byte[] { 40, 50, 60 };

            // Quando
            _sut.SetFile(token, content);

            // Entao
            _cache.Received(1).Set(token, content, Arg.Is<TimeSpan>(ts => ts == new TimeSpan(0, 0, 5, 0)));
        }

        #endregion

        #region Constantes

        [Fact]
        public void Dado_TempFileCacheManager_Quando_VerificarCacheName_Entao_DeveSerTempFileCacheName()
        {
            TempFileCacheManager.TempFileCacheName.ShouldBe("TempFileCacheName");
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_CacheManager_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
            _sut.ShouldBeAssignableTo<ITempFileCacheManager>();
        }

        #endregion
    }
}
