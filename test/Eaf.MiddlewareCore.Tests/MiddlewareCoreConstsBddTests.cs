using Eaf.Middleware;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests
{
    /// <summary>
    /// Testes BDD para MiddlewareCoreConsts seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class MiddlewareCoreConstsBddTests
    {
        #region Constantes

        [Fact]
        public void Dado_MiddlewareCoreConsts_Quando_VerificarDefaultPassPhrase_Entao_DeveSerConstante()
        {
            MiddlewareCoreConsts.DefaultPassPhrase.ShouldNotBeNullOrEmpty();
            MiddlewareCoreConsts.DefaultPassPhrase.ShouldBe("gsKxGZ012HLL3MI5");
        }

        [Fact]
        public void Dado_MiddlewareCoreConsts_Quando_VerificarSecurityStampKey_Entao_DeveSerConstante()
        {
            MiddlewareCoreConsts.SecurityStampKey.ShouldBe("AspNet.Identity.SecurityStamp");
        }

        [Fact]
        public void Dado_MiddlewareCoreConsts_Quando_VerificarTokenValidityKey_Entao_DeveSerConstante()
        {
            MiddlewareCoreConsts.TokenValidityKey.ShouldBe("token_validity_key");
        }

        [Fact]
        public void Dado_MiddlewareCoreConsts_Quando_VerificarTokenValidityValue_Entao_DeveSerConstante()
        {
            MiddlewareCoreConsts.TokenValidityValue.ShouldBe("token_validity_value");
        }

        [Fact]
        public void Dado_MiddlewareCoreConsts_Quando_VerificarUserIdentifier_Entao_DeveSerConstante()
        {
            MiddlewareCoreConsts.UserIdentifier.ShouldBe("user_identifier");
        }

        #endregion
    }
}
