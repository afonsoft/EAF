using Eaf.Middleware.Core.Authentication.External;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Users;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External
{
    /// <summary>
    /// Testes BDD para classes de autenticação externa do Core seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class CoreExternalAuthBddTests
    {
        #region ExternalAuthUserInfo

        [Fact]
        public void Dado_ExternalAuthUserInfo_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ExternalAuthUserInfo
            {
                Name = "João",
                Surname = "Silva",
                EmailAddress = "joao@acme.com",
                Provider = "Google",
                ProviderKey = "google-123",
                Picture = "https://example.com/photo.jpg",
                AccessCode = "access-code-xyz"
            };

            dto.Name.ShouldBe("João");
            dto.Surname.ShouldBe("Silva");
            dto.EmailAddress.ShouldBe("joao@acme.com");
            dto.Provider.ShouldBe("Google");
            dto.ProviderKey.ShouldBe("google-123");
            dto.Picture.ShouldBe("https://example.com/photo.jpg");
            dto.AccessCode.ShouldBe("access-code-xyz");
        }

        #endregion

        #region ExternalLoginProviderInfo

        [Fact]
        public void Dado_ExternalLoginProviderInfo_Quando_CriarComParametros_Entao_DeveDefinirValores()
        {
            var provider = new ExternalLoginProviderInfo(
                "Google",
                "client-id",
                "client-secret",
                "tenant-1",
                typeof(object));

            provider.Name.ShouldBe("Google");
            provider.ClientId.ShouldBe("client-id");
            provider.ClientSecret.ShouldBe("client-secret");
            provider.TenantId.ShouldBe("tenant-1");
            provider.ProviderApiType.ShouldBe(typeof(object));
            provider.AdditionalParams.ShouldNotBeNull();
            provider.AdditionalParams.Count.ShouldBe(0);
            provider.ClaimMappings.ShouldNotBeNull();
            provider.ClaimMappings.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_ExternalLoginProviderInfo_Quando_CriarComParametrosAdicionais_Entao_DeveArmazenar()
        {
            var additionalParams = new Dictionary<string, string>
            {
                { "scope", "openid profile email" }
            };
            var claimMappings = new List<JsonClaimMap>
            {
                new JsonClaimMap { Claim = "name", Key = "displayName" }
            };

            var provider = new ExternalLoginProviderInfo(
                "Microsoft",
                "ms-client",
                "ms-secret",
                "tenant-2",
                typeof(string),
                additionalParams,
                claimMappings);

            provider.AdditionalParams.Count.ShouldBe(1);
            provider.AdditionalParams["scope"].ShouldBe("openid profile email");
            provider.ClaimMappings.Count.ShouldBe(1);
            provider.ClaimMappings[0].Claim.ShouldBe("name");
            provider.ClaimMappings[0].Key.ShouldBe("displayName");
        }

        #endregion

        #region JsonClaimMap

        [Fact]
        public void Dado_JsonClaimMap_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var map = new JsonClaimMap
            {
                Claim = "email",
                Key = "emailAddress"
            };

            map.Claim.ShouldBe("email");
            map.Key.ShouldBe("emailAddress");
        }

        #endregion

        #region UserAndIdentity

        [Fact]
        public void Dado_UserAndIdentity_Quando_CriarComUsuarioEIdentity_Entao_DeveArmazenar()
        {
            var user = new User { UserName = "admin", EmailAddress = "admin@acme.com" };
            var identity = new ClaimsIdentity("Bearer");

            var result = new UserAndIdentity(user, identity);

            result.User.UserName.ShouldBe("admin");
            result.Identity.AuthenticationType.ShouldBe("Bearer");
        }

        #endregion

        #region EafUserToken

        [Fact]
        public void Dado_EafUserToken_Quando_Criar_Entao_DeveInicializar()
        {
            var token = new EafUserToken();
            token.ShouldNotBeNull();
        }

        #endregion
    }
}
