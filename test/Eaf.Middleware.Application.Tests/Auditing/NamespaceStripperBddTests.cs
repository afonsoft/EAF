using Eaf.Middleware.Auditing;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Auditing
{
    /// <summary>
    /// Testes BDD para NamespaceStripper seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class NamespaceStripperBddTests
    {
        private readonly NamespaceStripper _stripper = new();

        [Fact]
        public void Dado_NomeCompletoComNamespace_Quando_StripNameSpace_Entao_DeveRetornarApenasNome()
        {
            // Dado
            var fullName = "Eaf.Middleware.Authorization.Users.UserAppService";

            // Quando
            var result = _stripper.StripNameSpace(fullName);

            // Então
            result.ShouldBe("UserAppService");
        }

        [Fact]
        public void Dado_NomeSemNamespace_Quando_StripNameSpace_Entao_DeveRetornarOMesmo()
        {
            // Dado
            var simpleName = "UserAppService";

            // Quando
            var result = _stripper.StripNameSpace(simpleName);

            // Então
            result.ShouldBe("UserAppService");
        }

        [Fact]
        public void Dado_StringVazia_Quando_StripNameSpace_Entao_DeveRetornarVazio()
        {
            // Quando
            var result = _stripper.StripNameSpace("");

            // Então
            result.ShouldBe("");
        }

        [Fact]
        public void Dado_StringNull_Quando_StripNameSpace_Entao_DeveRetornarNull()
        {
            // Quando
            var result = _stripper.StripNameSpace(null);

            // Então
            result.ShouldBeNull();
        }
    }
}
