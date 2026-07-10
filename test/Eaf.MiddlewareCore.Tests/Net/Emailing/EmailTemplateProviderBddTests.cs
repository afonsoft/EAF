using Eaf.Middleware.Net.Emailing;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Net.Emailing
{
    /// <summary>
    /// Testes BDD para EmailTemplateProvider exercitando carga e cache do template padrão.
    /// </summary>
    public class EmailTemplateProviderBddTests
    {
        [Fact]
        public void Dado_TenantId_Quando_GetDefaultTemplate_Entao_DeveRetornarTemplateComAnoAtual()
        {
            // Dado
            var provider = new EmailTemplateProvider();

            // Quando
            var result = provider.GetDefaultTemplate(1);

            // Então
            result.ShouldNotBeNullOrEmpty();
            result.ShouldNotContain("{THIS_YEAR}");
            result.ShouldContain(DateTime.Now.Year.ToString());
        }

        [Fact]
        public void Dado_Host_Quando_GetDefaultTemplate_Entao_DeveRetornarTemplateComAnoAtual()
        {
            // Dado
            var provider = new EmailTemplateProvider();

            // Quando
            var result = provider.GetDefaultTemplate(null);

            // Então
            result.ShouldNotBeNullOrEmpty();
            result.ShouldNotContain("{THIS_YEAR}");
            result.ShouldContain(DateTime.Now.Year.ToString());
        }

        [Fact]
        public void Dado_MesmoTenant_Quando_GetDefaultTemplateDuasVezes_Entao_DeveRetornarMesmoTemplate()
        {
            // Dado
            var provider = new EmailTemplateProvider();

            // Quando
            var first = provider.GetDefaultTemplate(2);
            var second = provider.GetDefaultTemplate(2);

            // Então
            first.ShouldBe(second);
        }
    }
}
