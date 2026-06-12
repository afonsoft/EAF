using Eaf.Security;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Security
{
    /// <summary>
    /// Testes BDD para EafClaimTypes seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class EafClaimTypesBddTests
    {
        [Fact]
        public void Dado_UserIdentifierClaimType_Quando_Verificar_Entao_DeveTerValorPadrao()
        {
            EafClaimTypes.UserIdentifierClaimType.ShouldBe("http://aspnetzero.com/claims/useridentifier");
        }

        [Fact]
        public void Dado_ExternalAuthProviderformation_Quando_Verificar_Entao_DeveTerValorPadrao()
        {
            EafClaimTypes.ExternalAuthProviderformation.ShouldContain("externalAuthProviderformation");
        }

        [Fact]
        public void Dado_UserIdentifierClaimType_Quando_Alterar_Entao_DeveAceitarNovoValor()
        {
            // Dado
            var original = EafClaimTypes.UserIdentifierClaimType;

            try
            {
                // Quando
                EafClaimTypes.UserIdentifierClaimType = "custom/claim/type";

                // Então
                EafClaimTypes.UserIdentifierClaimType.ShouldBe("custom/claim/type");
            }
            finally
            {
                EafClaimTypes.UserIdentifierClaimType = original;
            }
        }
    }
}
