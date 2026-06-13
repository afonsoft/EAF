using Eaf.Middleware.Web.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shouldly;
using System.Text;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Authentication
{
    /// <summary>
    /// Testes BDD para TokenAuthConfiguration seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class TokenAuthConfigurationBddTests
    {
        [Fact]
        public void Dado_TokenAuthConfiguration_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("TestSecretKeyForTesting1234567890"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var config = new TokenAuthConfiguration
            {
                Issuer = "https://api.eaf.com",
                Audience = "https://app.eaf.com",
                SecurityKey = key,
                SigningCredentials = credentials
            };

            config.Issuer.ShouldBe("https://api.eaf.com");
            config.Audience.ShouldBe("https://app.eaf.com");
            config.SecurityKey.ShouldNotBeNull();
            config.SigningCredentials.ShouldNotBeNull();
        }
    }
}
