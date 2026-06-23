using Eaf.Middleware.Web.WebHooks;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.WebHooks
{
    /// <summary>
    /// Testes BDD para EafWebhookDefinitionProvider seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class EafWebhookDefinitionProviderBddTests
    {
        #region Instanciacao

        [Fact]
        public void Dado_Padrao_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new EafWebhookDefinitionProvider();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Padrao_Quando_CriarInstancia_Entao_DeveHerdarDeWebhookDefinitionProvider()
        {
            var sut = new EafWebhookDefinitionProvider();
            sut.ShouldBeAssignableTo<Abp.Webhooks.WebhookDefinitionProvider>();
        }

        #endregion
    }
}
