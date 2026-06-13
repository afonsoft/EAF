using Eaf.Middleware.Core.Authentication.External.Google;
using Newtonsoft.Json.Linq;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External
{
    /// <summary>
    /// Testes BDD para GoogleHelper seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class GoogleHelperBddTests
    {
        [Fact]
        public void Dado_JObjectComEmail_Quando_GetEmail_Entao_DeveRetornarEmail()
        {
            var user = JObject.Parse("{\"email\":\"user@gmail.com\"}");
            GoogleHelper.GetEmail(user).ShouldBe("user@gmail.com");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetEmail_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => GoogleHelper.GetEmail(null));
        }

        [Fact]
        public void Dado_JObjectComFamilyName_Quando_GetFamilyName_Entao_DeveRetornar()
        {
            var user = JObject.Parse("{\"family_name\":\"Silva\"}");
            GoogleHelper.GetFamilyName(user).ShouldBe("Silva");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetFamilyName_Entao_DeveLancarExcecao()
        {
            Should.Throw<ArgumentNullException>(() => GoogleHelper.GetFamilyName(null));
        }

        [Fact]
        public void Dado_JObjectComGivenName_Quando_GetGivenName_Entao_DeveRetornar()
        {
            var user = JObject.Parse("{\"given_name\":\"João\"}");
            GoogleHelper.GetGivenName(user).ShouldBe("João");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetGivenName_Entao_DeveLancarExcecao()
        {
            Should.Throw<ArgumentNullException>(() => GoogleHelper.GetGivenName(null));
        }

        [Fact]
        public void Dado_JObjectComId_Quando_GetId_Entao_DeveRetornar()
        {
            var user = JObject.Parse("{\"id\":\"google-123\"}");
            GoogleHelper.GetId(user).ShouldBe("google-123");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetId_Entao_DeveLancarExcecao()
        {
            Should.Throw<ArgumentNullException>(() => GoogleHelper.GetId(null));
        }

        [Fact]
        public void Dado_JObjectComName_Quando_GetName_Entao_DeveRetornar()
        {
            var user = JObject.Parse("{\"name\":\"João Silva\"}");
            GoogleHelper.GetName(user).ShouldBe("João Silva");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetName_Entao_DeveLancarExcecao()
        {
            Should.Throw<ArgumentNullException>(() => GoogleHelper.GetName(null));
        }

        [Fact]
        public void Dado_JObjectComLink_Quando_GetProfile_Entao_DeveRetornar()
        {
            var user = JObject.Parse("{\"link\":\"https://profiles.google.com/123\"}");
            GoogleHelper.GetProfile(user).ShouldBe("https://profiles.google.com/123");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetProfile_Entao_DeveLancarExcecao()
        {
            Should.Throw<ArgumentNullException>(() => GoogleHelper.GetProfile(null));
        }

        [Fact]
        public void Dado_JObjectSemCampo_Quando_GetEmail_Entao_DeveRetornarNull()
        {
            var user = JObject.Parse("{\"name\":\"test\"}");
            GoogleHelper.GetEmail(user).ShouldBeNull();
        }
    }
}
