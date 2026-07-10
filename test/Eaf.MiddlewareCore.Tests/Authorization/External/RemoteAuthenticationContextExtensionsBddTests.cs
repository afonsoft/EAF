using Eaf.Middleware.Core.Authentication.External;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Shouldly;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External
{
    /// <summary>
    /// Testes BDD para RemoteAuthenticationContextExtensions seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class RemoteAuthenticationContextExtensionsBddTests
    {
        private class TestAuthenticationHandler : IAuthenticationHandler
        {
            public Task<AuthenticateResult> AuthenticateAsync() => Task.FromResult(AuthenticateResult.NoResult());
            public Task ChallengeAsync(AuthenticationProperties properties) => Task.CompletedTask;
            public Task ForbidAsync(AuthenticationProperties properties) => Task.CompletedTask;
            public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context) => Task.CompletedTask;
        }

        private class TestRemoteAuthenticationContext : RemoteAuthenticationContext<RemoteAuthenticationOptions>
        {
            public TestRemoteAuthenticationContext(
                HttpContext context,
                AuthenticationScheme scheme,
                RemoteAuthenticationOptions options,
                AuthenticationProperties properties)
                : base(context, scheme, options, properties)
            {
            }
        }
        #region AddMappedClaims - ClaimsPrincipal

        [Fact]
        public void Dado_MappingsVazios_Quando_AddMappedClaims_Entao_NaoDeveAdicionarIdentidades()
        {
            // Dado
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("email_key", "test@test.com")
            }, "test");
            var principal = new ClaimsPrincipal(identity);
            var mappings = new List<JsonClaimMap>();

            // Quando
            principal.AddMappedClaims(mappings);

            // Entao
            principal.Identities.ShouldHaveSingleItem();
        }

        [Fact]
        public void Dado_MappingComClaimExistente_Quando_AddMappedClaims_Entao_DeveAdicionarNovaIdentidade()
        {
            // Dado
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("email_key", "user@example.com")
            }, "test");
            var principal = new ClaimsPrincipal(identity);
            var mappings = new List<JsonClaimMap>
            {
                new JsonClaimMap { Key = "email_key", Claim = "email" }
            };

            // Quando
            principal.AddMappedClaims(mappings);

            // Entao
            principal.HasClaim("email", "user@example.com").ShouldBeTrue();
        }

        [Fact]
        public void Dado_MappingComClaimInexistente_Quando_AddMappedClaims_Entao_NaoDeveAdicionarClaim()
        {
            // Dado
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("name", "John")
            }, "test");
            var principal = new ClaimsPrincipal(identity);
            var mappings = new List<JsonClaimMap>
            {
                new JsonClaimMap { Key = "nonexistent_key", Claim = "mapped_claim" }
            };

            // Quando
            principal.AddMappedClaims(mappings);

            // Entao
            principal.HasClaim(c => c.Type == "mapped_claim").ShouldBeFalse();
        }

        [Fact]
        public void Dado_MultipleMappings_Quando_AddMappedClaims_Entao_DeveMapearTodosExistentes()
        {
            // Dado
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("source_email", "a@b.com"),
                new Claim("source_name", "John")
            }, "test");
            var principal = new ClaimsPrincipal(identity);
            var mappings = new List<JsonClaimMap>
            {
                new JsonClaimMap { Key = "source_email", Claim = "email" },
                new JsonClaimMap { Key = "source_name", Claim = "name" }
            };

            // Quando
            principal.AddMappedClaims(mappings);

            // Entao
            principal.HasClaim("email", "a@b.com").ShouldBeTrue();
            principal.HasClaim("name", "John").ShouldBeTrue();
        }

        #endregion

        #region AddMappedClaims - RemoteAuthenticationContext

        [Fact]
        public void Dado_ContextoComClaimExistente_Quando_AddMappedClaims_Entao_DeveAdicionarNovaIdentidade()
        {
            // Dado
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("email_key", "user@example.com")
            }, "test");

            var context = new TestRemoteAuthenticationContext(
                new DefaultHttpContext(),
                new AuthenticationScheme("test", "Test", typeof(TestAuthenticationHandler)),
                new RemoteAuthenticationOptions(),
                new AuthenticationProperties())
            {
                Principal = new ClaimsPrincipal(identity)
            };

            var mappings = new List<JsonClaimMap>
            {
                new JsonClaimMap { Key = "email_key", Claim = "email" }
            };

            // Quando
            context.AddMappedClaims(mappings);

            // Entao
            context.Principal.HasClaim("email", "user@example.com").ShouldBeTrue();
        }

        [Fact]
        public void Dado_ContextoComMappingInexistente_Quando_AddMappedClaims_Entao_NaoDeveAdicionarClaim()
        {
            // Dado
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("name", "John")
            }, "test");

            var context = new TestRemoteAuthenticationContext(
                new DefaultHttpContext(),
                new AuthenticationScheme("test", "Test", typeof(TestAuthenticationHandler)),
                new RemoteAuthenticationOptions(),
                new AuthenticationProperties())
            {
                Principal = new ClaimsPrincipal(identity)
            };

            var mappings = new List<JsonClaimMap>
            {
                new JsonClaimMap { Key = "nonexistent_key", Claim = "mapped_claim" }
            };

            // Quando
            context.AddMappedClaims(mappings);

            // Entao
            context.Principal.HasClaim(c => c.Type == "mapped_claim").ShouldBeFalse();
        }

        #endregion
    }
}
