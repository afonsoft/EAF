using Eaf.Middleware.Core.Authentication;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization
{
    /// <summary>
    /// Testes BDD para JsonClaimMapDto seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class JsonClaimMapDtoBddTests
    {
        #region Propriedades

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirClaim_Entao_DeveArmazenar()
        {
            var sut = new JsonClaimMapDto { Claim = "email" };
            sut.Claim.ShouldBe("email");
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirKey_Entao_DeveArmazenar()
        {
            var sut = new JsonClaimMapDto { Key = "email_key" };
            sut.Key.ShouldBe("email_key");
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_CriarSemParametros_Entao_PropriedadesDevemSerNull()
        {
            var sut = new JsonClaimMapDto();
            sut.Claim.ShouldBeNull();
            sut.Key.ShouldBeNull();
        }

        [Fact]
        public void Dado_Dto_Quando_DefinirClaimEKey_Entao_AmbasDevemSerArmazenadas()
        {
            var sut = new JsonClaimMapDto
            {
                Claim = "name",
                Key = "display_name"
            };
            sut.Claim.ShouldBe("name");
            sut.Key.ShouldBe("display_name");
        }

        #endregion
    }
}
