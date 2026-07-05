using System.Linq;
using Eaf.AspNetCore.Hangfire.Configuration;
using Eaf.Hangfire;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Features;
using Eaf.Middleware.Identity;
using Eaf.Middleware.Web;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Extensions
{
    /// <summary>
    /// Testes BDD para as classes estáticas de extensão do Core seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class CoreStaticExtensionsBddTests
    {
        [Theory]
        [InlineData(typeof(HostingEnvironmentExtensions))]
        [InlineData(typeof(EafHostBuilderExtensions))]
        [InlineData(typeof(FeatureExtensions))]
        [InlineData(typeof(EafDisplayNameExtensions))]
        [InlineData(typeof(EafHangfireApplicationBuilderExtensions))]
        [InlineData(typeof(EafHangfireConfigurationExtensions))]
        [InlineData(typeof(IdentityRegistrar))]
        [InlineData(typeof(WebContentDirectoryFinder))]
        public void Dado_ClasseDeExtensao_Quando_VerificarTipo_Entao_DeveSerEstatica(System.Type tipo)
        {
            (tipo.IsAbstract && tipo.IsSealed).ShouldBeTrue();
        }

        [Fact]
        public void Dado_HostingEnvironmentExtensions_Quando_VerificarMetodos_Entao_DeveExporGetAppConfiguration()
        {
            typeof(HostingEnvironmentExtensions)
                .GetMethods().Count(m => m.Name == "GetAppConfiguration").ShouldBe(2);
        }

        [Fact]
        public void Dado_EafHangfireConfigurationExtensions_Quando_VerificarMetodos_Entao_DeveExporMetodosDeConfiguracao()
        {
            var tipo = typeof(EafHangfireConfigurationExtensions);
            tipo.GetMethod("UseHangfire").ShouldNotBeNull();
            tipo.GetMethod("SetExpiredHistoryEntityWoker").ShouldNotBeNull();
            tipo.GetMethod("SetExpiredAuditWoker").ShouldNotBeNull();
        }

        [Fact]
        public void Dado_IdentityRegistrar_Quando_VerificarMetodoRegister_Entao_DeveExisitir()
        {
            typeof(IdentityRegistrar).GetMethod("Register").ShouldNotBeNull();
        }

        [Fact]
        public void Dado_WebContentDirectoryFinder_Quando_VerificarMetodo_Entao_DeveExporCalculateContentRootFolder()
        {
            typeof(WebContentDirectoryFinder).GetMethod("CalculateContentRootFolder").ShouldNotBeNull();
        }
    }
}
