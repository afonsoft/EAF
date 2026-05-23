using Eaf.Middleware.Core.Authentication.External;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Authorization.External
{
    public class ExternalLoginProviderInfoTests
    {
        [Fact]
        public void Dado_ParametrosCompletos_Quando_Criar_Entao_DeveDefinirTodasPropriedades()
        {
            var additionalParams = new Dictionary<string, string> { { "scope", "openid" } };
            var claimMappings = new List<JsonClaimMap> { new JsonClaimMap { Claim = "email", Key = "mail" } };

            var info = new ExternalLoginProviderInfo(
                "Google",
                "client-id",
                "client-secret",
                "tenant-id",
                typeof(string),
                additionalParams,
                claimMappings
            );

            info.Name.ShouldBe("Google");
            info.ClientId.ShouldBe("client-id");
            info.ClientSecret.ShouldBe("client-secret");
            info.TenantId.ShouldBe("tenant-id");
            info.ProviderApiType.ShouldBe(typeof(string));
            info.AdditionalParams.ShouldContainKey("scope");
            info.ClaimMappings.Count.ShouldBe(1);
        }

        [Fact]
        public void Dado_ParametrosOpcionaisNulos_Quando_Criar_Entao_DeveUsarListasVazias()
        {
            var info = new ExternalLoginProviderInfo(
                "Microsoft",
                "cid",
                "csecret",
                "tid",
                typeof(int)
            );

            info.AdditionalParams.ShouldNotBeNull();
            info.AdditionalParams.Count.ShouldBe(0);
            info.ClaimMappings.ShouldNotBeNull();
            info.ClaimMappings.Count.ShouldBe(0);
        }
    }
}
