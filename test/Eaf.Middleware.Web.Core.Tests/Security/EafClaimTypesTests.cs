using Eaf.Security;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Security
{
    public class EafClaimTypesTests
    {
        [Fact]
        public void Dado_UserIdentifierClaimType_Quando_Verificar_Entao_DeveSerCorreto()
        {
            EafClaimTypes.UserIdentifierClaimType.ShouldBe("https://aspnetzero.com/claims/useridentifier");
        }

        [Fact]
        public void Dado_ExternalAuthProviderformation_Quando_Verificar_Entao_DeveSerCorreto()
        {
            EafClaimTypes.ExternalAuthProviderformation
                .ShouldBe("https://www.aspnetboilerplate.com/identity/claims/externalAuthProviderformation");
        }

        [Fact]
        public void Dado_UserIdentifierClaimType_Quando_Alterar_Entao_DeveRefletirNovoValor()
        {
            var original = EafClaimTypes.UserIdentifierClaimType;
            try
            {
                EafClaimTypes.UserIdentifierClaimType = "custom/claim";
                EafClaimTypes.UserIdentifierClaimType.ShouldBe("custom/claim");
            }
            finally
            {
                EafClaimTypes.UserIdentifierClaimType = original;
            }
        }
    }
}
