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

        [Fact]
        public void Dado_NomeGenericoComNamespace_Quando_StripNameSpace_Entao_DeveRetornarTipoGenericoSimples()
        {
            // Dado
            var genericName = "System.Collections.Generic.List`1[[System.String, System.Private.CoreLib, Version=10.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]";

            // Quando
            var result = _stripper.StripNameSpace(genericName);

            // Então
            result.ShouldContain("List");
            result.ShouldContain("<");
            result.ShouldContain(">");
        }

        [Fact]
        public void Dado_NomeSemPontos_Quando_StripNameSpace_Entao_DeveRetornarMesmo()
        {
            // Dado
            var name = "SimpleClass";

            // Quando
            var result = _stripper.StripNameSpace(name);

            // Então
            result.ShouldBe("SimpleClass");
        }

        [Fact]
        public void Dado_NomeComUmPonto_Quando_StripNameSpace_Entao_DeveRetornarUltimaParte()
        {
            // Dado
            var name = "Namespace.ClassName";

            // Quando
            var result = _stripper.StripNameSpace(name);

            // Então
            result.ShouldBe("ClassName");
        }

        [Fact]
        public void Dado_NomeGenericoComMultiplosArgumentos_Quando_StripNameSpace_Entao_DeveRetornarTiposGenericosSimples()
        {
            // Dado
            var genericName = "System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib, Version=10.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e],[System.Int32, System.Private.CoreLib, Version=10.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]";

            // Quando
            var result = _stripper.StripNameSpace(genericName);

            // Então
            result.ShouldContain("Dictionary");
            result.ShouldContain("<");
            result.ShouldContain(">");
            result.ShouldContain(",");
        }

        [Fact]
        public void Dado_NomeGenericoSemNamespaceArgumentos_Quando_StripNameSpace_Entao_DeveFecharGenerico()
        {
            // Dado
            var genericName = "System.Collections.Generic.List`1[[Foo]]";

            // Quando
            var result = _stripper.StripNameSpace(genericName);

            // Então
            result.ShouldBe("List<>");
        }
    }
}
