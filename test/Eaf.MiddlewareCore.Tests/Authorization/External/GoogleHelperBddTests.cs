using Eaf.Middleware.Core.Authentication.External.Google;
using Newtonsoft.Json.Linq;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External
{
    public class GoogleHelperBddTests
    {
        private static JObject CriarUsuarioGoogle()
        {
            return JObject.FromObject(new
            {
                id = "google-123",
                email = "usuario@gmail.com",
                name = "João Silva",
                given_name = "João",
                family_name = "Silva",
                link = "https://plus.google.com/123"
            });
        }

        [Fact]
        public void Dado_UsuarioGoogle_Quando_GetEmail_Entao_DeveRetornarEmail()
        {
            var user = CriarUsuarioGoogle();

            GoogleHelper.GetEmail(user).ShouldBe("usuario@gmail.com");
        }

        [Fact]
        public void Dado_UsuarioGoogle_Quando_GetId_Entao_DeveRetornarId()
        {
            var user = CriarUsuarioGoogle();

            GoogleHelper.GetId(user).ShouldBe("google-123");
        }

        [Fact]
        public void Dado_UsuarioGoogle_Quando_GetName_Entao_DeveRetornarNome()
        {
            var user = CriarUsuarioGoogle();

            GoogleHelper.GetName(user).ShouldBe("João Silva");
        }

        [Fact]
        public void Dado_UsuarioGoogle_Quando_GetGivenName_Entao_DeveRetornarPrimeiroNome()
        {
            var user = CriarUsuarioGoogle();

            GoogleHelper.GetGivenName(user).ShouldBe("João");
        }

        [Fact]
        public void Dado_UsuarioGoogle_Quando_GetFamilyName_Entao_DeveRetornarSobrenome()
        {
            var user = CriarUsuarioGoogle();

            GoogleHelper.GetFamilyName(user).ShouldBe("Silva");
        }

        [Fact]
        public void Dado_UsuarioGoogle_Quando_GetProfile_Entao_DeveRetornarLink()
        {
            var user = CriarUsuarioGoogle();

            GoogleHelper.GetProfile(user).ShouldBe("https://plus.google.com/123");
        }

        [Fact]
        public void Dado_Null_Quando_GetEmail_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => GoogleHelper.GetEmail(null));
        }

        [Fact]
        public void Dado_Null_Quando_GetId_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => GoogleHelper.GetId(null));
        }

        [Fact]
        public void Dado_Null_Quando_GetName_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => GoogleHelper.GetName(null));
        }

        [Fact]
        public void Dado_Null_Quando_GetGivenName_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => GoogleHelper.GetGivenName(null));
        }

        [Fact]
        public void Dado_Null_Quando_GetFamilyName_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => GoogleHelper.GetFamilyName(null));
        }

        [Fact]
        public void Dado_Null_Quando_GetProfile_Entao_DeveLancarArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => GoogleHelper.GetProfile(null));
        }

        [Fact]
        public void Dado_UsuarioSemCampos_Quando_GetEmail_Entao_DeveRetornarNull()
        {
            var user = new JObject();

            GoogleHelper.GetEmail(user).ShouldBeNull();
        }
    }
}
