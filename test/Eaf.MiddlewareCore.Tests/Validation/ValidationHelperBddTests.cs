using Eaf.Middleware.Validation;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Validation
{
    /// <summary>
    /// Testes BDD para ValidationHelper seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ValidationHelperBddTests
    {
        [Theory]
        [InlineData("user@example.com", true)]
        [InlineData("user.name@domain.com", true)]
        [InlineData("user+tag@domain.co.uk", true)]
        [InlineData("user-name@domain.com", true)]
        [InlineData("user@domain", false)]
        [InlineData("@domain.com", false)]
        [InlineData("user@", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("plaintext", false)]
        [InlineData("user@domain.com.br", true)]
        public void Dado_Valor_Quando_IsEmail_Entao_DeveRetornarResultadoCorreto(string value, bool expected)
        {
            ValidationHelper.IsEmail(value).ShouldBe(expected);
        }

        [Fact]
        public void Dado_EmailRegex_Quando_Verificar_Entao_DeveSerConstante()
        {
            ValidationHelper.EmailRegex.ShouldNotBeNullOrEmpty();
        }
    }
}
