using Eaf.Middleware.Web.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shouldly;
using System.Text;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Authentication.JwtBearer
{
    public class TokenAuthConfigurationTests
    {
        [Fact]
        public void Dado_TokenAuthConfiguration_Quando_Criado_Entao_PropriedadesDevemSerAtribuidas()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super-secret-key-1234567890-abcdef"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var config = new TokenAuthConfiguration
            {
                Audience = "eaf-app",
                Issuer = "eaf-issuer",
                SecurityKey = key,
                SigningCredentials = credentials
            };

            config.Audience.ShouldBe("eaf-app");
            config.Issuer.ShouldBe("eaf-issuer");
            config.SecurityKey.ShouldBe(key);
            config.SigningCredentials.ShouldBe(credentials);
        }

        [Fact]
        public void Dado_TokenAuthConfiguration_Quando_PadraoInicial_Entao_PropriedadesDevemSerNulas()
        {
            var config = new TokenAuthConfiguration();

            config.Audience.ShouldBeNull();
            config.Issuer.ShouldBeNull();
            config.SecurityKey.ShouldBeNull();
            config.SigningCredentials.ShouldBeNull();
        }
    }
}
