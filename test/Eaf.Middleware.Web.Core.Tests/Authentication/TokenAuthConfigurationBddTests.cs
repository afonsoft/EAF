using Eaf.Middleware.Web.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shouldly;
using System.Text;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Authentication
{
    /// <summary>
    /// Testes BDD para TokenAuthConfiguration seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class TokenAuthConfigurationBddTests
    {
        #region Propriedades

        [Fact]
        public void Dado_TokenAuthConfiguration_Quando_DefinirIssuer_Entao_DeveArmazenarCorretamente()
        {
            var config = new TokenAuthConfiguration { Issuer = "EAF" };
            config.Issuer.ShouldBe("EAF");
        }

        [Fact]
        public void Dado_TokenAuthConfiguration_Quando_DefinirAudience_Entao_DeveArmazenarCorretamente()
        {
            var config = new TokenAuthConfiguration { Audience = "EAF-Client" };
            config.Audience.ShouldBe("EAF-Client");
        }

        [Fact]
        public void Dado_TokenAuthConfiguration_Quando_DefinirSecurityKey_Entao_DeveArmazenarCorretamente()
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("MySuperSecretKeyForTesting12345!"));
            var config = new TokenAuthConfiguration { SecurityKey = key };
            config.SecurityKey.ShouldBe(key);
        }

        [Fact]
        public void Dado_TokenAuthConfiguration_Quando_DefinirSigningCredentials_Entao_DeveArmazenarCorretamente()
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("MySuperSecretKeyForTesting12345!"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var config = new TokenAuthConfiguration { SigningCredentials = credentials };
            config.SigningCredentials.ShouldBe(credentials);
        }

        [Fact]
        public void Dado_TokenAuthConfiguration_Quando_CriarInstancia_Entao_PropriedadesDevemSerNulas()
        {
            var config = new TokenAuthConfiguration();
            config.Issuer.ShouldBeNull();
            config.Audience.ShouldBeNull();
            config.SecurityKey.ShouldBeNull();
            config.SigningCredentials.ShouldBeNull();
        }

        #endregion
    }
}
