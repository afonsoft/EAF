using Eaf.Security;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Security
{
    public class EafClaimTypesBddTests
    {
        [Fact]
        public void Dado_EafClaimTypes_Quando_VerificarUserIdentifierClaimType_Entao_DeveEstarCorreto()
        {
            EafClaimTypes.UserIdentifierClaimType.ShouldBe("https://aspnetzero.com/claims/useridentifier");
        }

        [Fact]
        public void Dado_EafClaimTypes_Quando_VerificarExternalAuthProviderformation_Entao_DeveEstarCorreto()
        {
            EafClaimTypes.ExternalAuthProviderformation
                .ShouldBe("https://www.aspnetboilerplate.com/identity/claims/externalAuthProviderformation");
        }

        [Fact]
        public void Dado_EafClaimTypes_Quando_AlterarUserIdentifierClaimType_Entao_DeveAtualizar()
        {
            var original = EafClaimTypes.UserIdentifierClaimType;
            try
            {
                EafClaimTypes.UserIdentifierClaimType = "custom/claim/type";

                EafClaimTypes.UserIdentifierClaimType.ShouldBe("custom/claim/type");
            }
            finally
            {
                EafClaimTypes.UserIdentifierClaimType = original;
            }
        }

        [Fact]
        public void Dado_EafClaimTypes_Quando_AlterarExternalAuthProviderformation_Entao_DeveAtualizar()
        {
            var original = EafClaimTypes.ExternalAuthProviderformation;
            try
            {
                EafClaimTypes.ExternalAuthProviderformation = "custom/external/claim";

                EafClaimTypes.ExternalAuthProviderformation.ShouldBe("custom/external/claim");
            }
            finally
            {
                EafClaimTypes.ExternalAuthProviderformation = original;
            }
        }
    }
}
