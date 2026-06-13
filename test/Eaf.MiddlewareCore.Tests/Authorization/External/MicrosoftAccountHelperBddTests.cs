using Eaf.Middleware.Core.Authentication.External.Microsoft;
using Newtonsoft.Json.Linq;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External
{
    /// <summary>
    /// Testes BDD para MicrosoftAccountHelper seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class MicrosoftAccountHelperBddTests
    {
        [Fact]
        public void Dado_JObjectComDisplayName_Quando_GetDisplayName_Entao_DeveRetornar()
        {
            var user = JObject.Parse("{\"displayName\":\"João Silva\"}");
            MicrosoftAccountHelper.GetDisplayName(user).ShouldBe("João Silva");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetDisplayName_Entao_DeveLancarExcecao()
        {
            Should.Throw<ArgumentNullException>(() => MicrosoftAccountHelper.GetDisplayName(null));
        }

        [Fact]
        public void Dado_JObjectComMail_Quando_GetEmail_Entao_DeveRetornarMail()
        {
            var user = JObject.Parse("{\"mail\":\"user@outlook.com\"}");
            MicrosoftAccountHelper.GetEmail(user).ShouldBe("user@outlook.com");
        }

        [Fact]
        public void Dado_JObjectSemMailComUPN_Quando_GetEmail_Entao_DeveRetornarUPN()
        {
            var user = JObject.Parse("{\"userPrincipalName\":\"user@domain.com\"}");
            MicrosoftAccountHelper.GetEmail(user).ShouldBe("user@domain.com");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetEmail_Entao_DeveLancarExcecao()
        {
            Should.Throw<ArgumentNullException>(() => MicrosoftAccountHelper.GetEmail(null));
        }

        [Fact]
        public void Dado_JObjectComGivenName_Quando_GetGivenName_Entao_DeveRetornar()
        {
            var user = JObject.Parse("{\"givenName\":\"João\"}");
            MicrosoftAccountHelper.GetGivenName(user).ShouldBe("João");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetGivenName_Entao_DeveLancarExcecao()
        {
            Should.Throw<ArgumentNullException>(() => MicrosoftAccountHelper.GetGivenName(null));
        }

        [Fact]
        public void Dado_JObjectComId_Quando_GetId_Entao_DeveRetornar()
        {
            var user = JObject.Parse("{\"id\":\"ms-id-456\"}");
            MicrosoftAccountHelper.GetId(user).ShouldBe("ms-id-456");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetId_Entao_DeveLancarExcecao()
        {
            Should.Throw<ArgumentNullException>(() => MicrosoftAccountHelper.GetId(null));
        }

        [Fact]
        public void Dado_JObjectComSurname_Quando_GetSurname_Entao_DeveRetornar()
        {
            var user = JObject.Parse("{\"surname\":\"Silva\"}");
            MicrosoftAccountHelper.GetSurname(user).ShouldBe("Silva");
        }

        [Fact]
        public void Dado_JObjectNulo_Quando_GetSurname_Entao_DeveLancarExcecao()
        {
            Should.Throw<ArgumentNullException>(() => MicrosoftAccountHelper.GetSurname(null));
        }

        [Fact]
        public void Dado_JObjectComMailEUPN_Quando_GetEmail_Entao_DevePreferirMail()
        {
            var user = JObject.Parse("{\"mail\":\"primary@ms.com\",\"userPrincipalName\":\"upn@ms.com\"}");
            MicrosoftAccountHelper.GetEmail(user).ShouldBe("primary@ms.com");
        }
    }
}
