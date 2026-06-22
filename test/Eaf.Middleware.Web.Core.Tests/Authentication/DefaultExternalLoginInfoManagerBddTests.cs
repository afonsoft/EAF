using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Web.Authentication;
using Microsoft.AspNetCore.Identity;
using Shouldly;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Authentication
{
    /// <summary>
    /// Testes BDD para DefaultExternalLoginInfoManager seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class DefaultExternalLoginInfoManagerBddTests
    {
        private readonly DefaultExternalLoginInfoManager _sut;

        public DefaultExternalLoginInfoManagerBddTests()
        {
            _sut = new DefaultExternalLoginInfoManager();
        }

        #region GetNameAndSurnameFromClaims

        [Fact]
        public void Dado_ClaimsComGivenNameESurname_Quando_GetNameAndSurname_Entao_DeveRetornarCorreto()
        {
            // Dado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName, "Afonso"),
                new Claim(ClaimTypes.Surname, "Silva")
            };

            // Quando
            var result = _sut.GetNameAndSurnameFromClaims(claims, new IdentityOptions());

            // Entao
            result.name.ShouldBe("Afonso");
            result.surname.ShouldBe("Silva");
        }

        [Fact]
        public void Dado_ClaimsSemGivenNameESurname_ComNameClaim_Quando_GetNameAndSurname_Entao_DeveSepararPorEspaco()
        {
            // Dado
            var options = new IdentityOptions();
            var claims = new List<Claim>
            {
                new Claim(options.ClaimsIdentity.UserNameClaimType, "Afonso Silva")
            };

            // Quando
            var result = _sut.GetNameAndSurnameFromClaims(claims, options);

            // Entao
            result.name.ShouldBe("Afonso");
            result.surname.ShouldBe(" Silva");
        }

        [Fact]
        public void Dado_ClaimsSemGivenNameESurname_ComNomeSemEspaco_Quando_GetNameAndSurname_Entao_DeveUsarMesmoValor()
        {
            // Dado
            var options = new IdentityOptions();
            var claims = new List<Claim>
            {
                new Claim(options.ClaimsIdentity.UserNameClaimType, "Afonso")
            };

            // Quando
            var result = _sut.GetNameAndSurnameFromClaims(claims, options);

            // Entao
            result.name.ShouldBe("Afonso");
            result.surname.ShouldBe("Afonso");
        }

        [Fact]
        public void Dado_ClaimsVazias_Quando_GetNameAndSurname_Entao_DeveRetornarNull()
        {
            // Dado
            var claims = new List<Claim>();

            // Quando
            var result = _sut.GetNameAndSurnameFromClaims(claims, new IdentityOptions());

            // Entao
            result.name.ShouldBeNull();
            result.surname.ShouldBeNull();
        }

        #endregion

        #region GetUserNameFromClaims

        [Fact]
        public void Dado_ClaimsComEmail_Quando_GetUserNameFromClaims_Entao_DeveRetornarParteAntesDo_Arroba()
        {
            // Dado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, "admin@test.com")
            };

            // Quando
            var result = _sut.GetUserNameFromClaims(claims);

            // Entao
            result.ShouldBe("admin");
        }

        #endregion

        #region GetUserNameFromExternalAuthUserInfo

        [Fact]
        public void Dado_ExternalAuthUserInfo_Quando_GetUserNameFromExternalAuthUserInfo_Entao_DeveRetornarParteAntesDo_Arroba()
        {
            // Dado
            var userInfo = new ExternalAuthUserInfo
            {
                EmailAddress = "user@domain.com",
                Name = "User",
                Surname = "Test",
                Provider = "Google",
                ProviderKey = "key-123"
            };

            // Quando
            var result = _sut.GetUserNameFromExternalAuthUserInfo(userInfo);

            // Entao
            result.ShouldBe("user");
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_DefaultExternalLoginInfoManager_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
            _sut.ShouldBeAssignableTo<IExternalLoginInfoManager>();
        }

        #endregion
    }
}
