using Eaf.AspNetCore.Hangfire;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Hangfire
{
    /// <summary>
    /// Testes BDD para EafHangfireAuthorizationFilter seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class EafHangfireAuthorizationFilterBddTests
    {
        #region Instanciacao

        [Fact]
        public void Dado_SemParametros_Quando_CriarInstancia_Entao_DeveInicializarComPermissoesPadrao()
        {
            var sut = new EafHangfireAuthorizationFilter();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_PermissoesCustom_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new EafHangfireAuthorizationFilter("Pages.Custom", "Pages.Admin");
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_SemParametros_Quando_CriarInstancia_Entao_DeveImplementarIDashboardAuthorizationFilter()
        {
            var sut = new EafHangfireAuthorizationFilter();
            sut.ShouldBeAssignableTo<global::Hangfire.Dashboard.IDashboardAuthorizationFilter>();
        }

        [Fact]
        public void Dado_ArrayVazio_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new EafHangfireAuthorizationFilter(System.Array.Empty<string>());
            sut.ShouldNotBeNull();
        }

        #endregion
    }
}
