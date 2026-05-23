using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Authorization.External
{
    public class JsonClaimMapTests
    {
        [Fact]
        public void Dado_JsonClaimMap_Quando_DefinirPropriedades_Entao_DeveArmazenarCorretamente()
        {
            var map = new JsonClaimMap
            {
                Claim = "email",
                Key = "mail"
            };

            map.Claim.ShouldBe("email");
            map.Key.ShouldBe("mail");
        }

        [Fact]
        public void Dado_JsonClaimMapDto_Quando_DefinirPropriedades_Entao_DeveArmazenarCorretamente()
        {
            var dto = new JsonClaimMapDto
            {
                Claim = "name",
                Key = "displayName"
            };

            dto.Claim.ShouldBe("name");
            dto.Key.ShouldBe("displayName");
        }
    }
}
