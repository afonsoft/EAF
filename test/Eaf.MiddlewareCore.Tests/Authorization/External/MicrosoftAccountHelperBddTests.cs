using Eaf.Middleware.Core.Authentication.External.Microsoft;
using Newtonsoft.Json.Linq;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External
{
    public class MicrosoftAccountHelperBddTests
    {
        private static JObject CriarUsuarioMicrosoft()
        {
            return JObject.FromObject(new
            {
                id = "ms-456",
                displayName = "Maria Souza",
                givenName = "Maria",
                surname = "Souza",
                mail = "maria@outlook.com",
                userPrincipalName = "maria@corp.com"
            });
        }

        [Fact]
        public void Dado_UsuarioMicrosoft_Quando_GetDisplayName_Entao_DeveRetornarNome()
        {
            var user = CriarUsuarioMicrosoft();

            MicrosoftAccountHelper.GetDisplayName(user).ShouldBe("Maria Souza");
        }

        [Fact]
        public void Dado_UsuarioMicrosoft_Quando_GetEmail_Entao_DeveRetornarMail()
        {
            var user = CriarUsuarioMicrosoft();

            MicrosoftAccountHelper.GetEmail(user).ShouldBe("maria@outlook.com");
        }

        [Fact]
        public void Dado_UsuarioSemMail_Quando_GetEmail_Entao_DeveFallbackParaUserPrincipalName()
        {
            var user = JObject.FromObject(new
            {
                userPrincipalName = "maria@corp.com"
            });

            MicrosoftAccountHelper.GetEmail(user).ShouldBe("maria@corp.com");
        }

        [Fact]
        public void Dado_UsuarioMicrosoft_Quando_GetGivenName_Entao_DeveRetornarPrimeiroNome()
        {
            var user = CriarUsuarioMicrosoft();

            MicrosoftAccountHelper.GetGivenName(user).ShouldBe("Maria");
        }

        [Fact]
        public void Dado_UsuarioMicrosoft_Quando_GetId_Entao_DeveRetornarId()
        {
            var user = CriarUsuarioMicrosoft();

            MicrosoftAccountHelper.GetId(user).ShouldBe("ms-456");
        }

        [Fact]
        public void Dado_UsuarioMicrosoft_Quando_GetSurname_Entao_DeveRetornarSobrenome()
        {
            var user = CriarUsuarioMicrosoft();

            MicrosoftAccountHelper.GetSurname(user).ShouldBe("Souza");
        }

        [Fact]
        public void Dado_Null_Quando_GetDisplayName_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => MicrosoftAccountHelper.GetDisplayName(null));
        }

        [Fact]
        public void Dado_Null_Quando_GetEmail_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => MicrosoftAccountHelper.GetEmail(null));
        }

        [Fact]
        public void Dado_Null_Quando_GetGivenName_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => MicrosoftAccountHelper.GetGivenName(null));
        }

        [Fact]
        public void Dado_Null_Quando_GetId_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => MicrosoftAccountHelper.GetId(null));
        }

        [Fact]
        public void Dado_Null_Quando_GetSurname_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => MicrosoftAccountHelper.GetSurname(null));
        }

        [Fact]
        public void Dado_UsuarioSemCampos_Quando_GetEmail_Entao_DeveRetornarNull()
        {
            var user = new JObject();

            MicrosoftAccountHelper.GetEmail(user).ShouldBeNull();
        }
    }
}
