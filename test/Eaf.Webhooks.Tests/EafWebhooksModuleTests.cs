using Abp.Webhooks;
using Shouldly;
using Xunit;

namespace Eaf.Webhooks.Tests
{
    /// <summary>
    /// Testes BDD para o registro do módulo Eaf.Webhooks.
    /// </summary>
    public class EafWebhooksModuleTests : EafWebhooksTestBase
    {
        [Fact]
        public void Dado_ModuloInicializado_Quando_ResolverGerenciador_Entao_DeveUsarEafWebhookManager()
        {
            // Quando
            var manager = Resolve<IWebhookManager>();

            // Então
            manager.ShouldBeOfType<EafWebhookManager>();
        }

        [Fact]
        public void Dado_ModuloInicializado_Quando_ResolverSender_Entao_DeveUsarEafWebhookSender()
        {
            // Quando
            var sender = Resolve<IWebhookSender>();

            // Então
            sender.ShouldBeOfType<EafWebhookSender>();
        }

        [Fact]
        public void Dado_ModuloInicializado_Quando_ResolverSubscriptionManager_Entao_DeveUsarEafWebhookSubscriptionManager()
        {
            // Quando
            var manager = Resolve<IWebhookSubscriptionManager>();

            // Então
            manager.ShouldBeOfType<EafWebhookSubscriptionManager>();
        }
    }
}
