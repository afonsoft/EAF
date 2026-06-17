using Eaf.Middleware.Authorization.External.AuthZero;
using Newtonsoft.Json.Linq;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External
{
    /// <summary>
    /// Testes BDD para AuthZeroAccountHelper seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AuthZeroAccountHelperBddTests
    {
        #region GetEmail

        [Fact]
        public void Dado_JObjectComEmail_Quando_GetEmail_Entao_DeveRetornarEmail()
        {
            var user = JObject.Parse("{\"email\":\"user@authzero.com\"}");
            AuthZeroAccountHelper.GetEmail(user).ShouldBe("user@authzero.com");
        }

        [Fact]
        public void Dado_JObjectSemEmail_Quando_GetEmail_Entao_DeveRetornarVazio()
        {
            var user = JObject.Parse("{\"name\":\"Test\"}");
            AuthZeroAccountHelper.GetEmail(user).ShouldBe("");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetEmail_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => AuthZeroAccountHelper.GetEmail(null));
        }

        #endregion

        #region GetDisplayName

        [Fact]
        public void Dado_JObjectComName_Quando_GetDisplayName_Entao_DeveRetornarNome()
        {
            var user = JObject.Parse("{\"name\":\"João Silva\"}");
            AuthZeroAccountHelper.GetDisplayName(user).ShouldBe("João Silva");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetDisplayName_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => AuthZeroAccountHelper.GetDisplayName(null));
        }

        #endregion

        #region GetGivenName

        [Fact]
        public void Dado_JObjectComGivenName_Quando_GetGivenName_Entao_DeveRetornarNome()
        {
            var user = JObject.Parse("{\"given_name\":\"João\"}");
            AuthZeroAccountHelper.GetGivenName(user).ShouldBe("João");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetGivenName_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => AuthZeroAccountHelper.GetGivenName(null));
        }

        #endregion

        #region GetSurname

        [Fact]
        public void Dado_JObjectComFamilyName_Quando_GetSurname_Entao_DeveRetornarSobrenome()
        {
            var user = JObject.Parse("{\"family_name\":\"Silva\"}");
            AuthZeroAccountHelper.GetSurname(user).ShouldBe("Silva");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetSurname_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => AuthZeroAccountHelper.GetSurname(null));
        }

        #endregion

        #region GetId

        [Fact]
        public void Dado_JObjectComSub_Quando_GetId_Entao_DeveRetornarId()
        {
            var user = JObject.Parse("{\"sub\":\"auth0|12345\"}");
            AuthZeroAccountHelper.GetId(user).ShouldBe("auth0|12345");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetId_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => AuthZeroAccountHelper.GetId(null));
        }

        #endregion

        #region GetPicture

        [Fact]
        public void Dado_JObjectComPicture_Quando_GetPicture_Entao_DeveRetornarUrl()
        {
            var user = JObject.Parse("{\"picture\":\"https://cdn.auth0.com/avatar.png\"}");
            AuthZeroAccountHelper.GetPicture(user).ShouldBe("https://cdn.auth0.com/avatar.png");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetPicture_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => AuthZeroAccountHelper.GetPicture(null));
        }

        #endregion
    }
}
