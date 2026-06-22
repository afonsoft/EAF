using Eaf.Middleware.MultiTenancy;
using Shouldly;
using System.Text.RegularExpressions;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.MultiTenancy
{
    /// <summary>
    /// Testes BDD para TenantConsts seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class TenantConstsBddTests
    {
        #region Constantes

        [Fact]
        public void Dado_TenantConsts_Quando_VerificarDefaultTenantName_Entao_DeveSerDefault()
        {
            TenantConsts.DefaultTenantName.ShouldBe("Default");
        }

        [Fact]
        public void Dado_TenantConsts_Quando_VerificarMaxNameLength_Entao_DeveSer128()
        {
            TenantConsts.MaxNameLength.ShouldBe(128);
        }

        [Fact]
        public void Dado_TenantConsts_Quando_VerificarTenancyNameRegex_Entao_DeveSerValido()
        {
            TenantConsts.TenancyNameRegex.ShouldNotBeNullOrEmpty();
        }

        #endregion

        #region Regex

        [Theory]
        [InlineData("Default", true)]
        [InlineData("tenant1", true)]
        [InlineData("my-tenant", true)]
        [InlineData("my_tenant", true)]
        [InlineData("ab", true)]
        [InlineData("1invalid", false)]
        [InlineData("a", false)]
        [InlineData("", false)]
        public void Dado_NomeTenant_Quando_ValidarComRegex_Entao_DeveRetornarEsperado(string name, bool expectedMatch)
        {
            var regex = new Regex(TenantConsts.TenancyNameRegex);
            regex.IsMatch(name).ShouldBe(expectedMatch);
        }

        #endregion
    }
}
