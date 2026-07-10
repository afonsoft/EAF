using Abp.Webhooks;
using Eaf.Middleware.Web.WebHooks;
using NSubstitute;
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

        #region SetWebhooks

        [Fact]
        public void Dado_ContextoDeDefinicao_Quando_DefinirWebhooks_Entao_DeveAdicionarWebhookDeNovoUsuario()
        {
            var sut = new EafWebhookDefinitionProvider();
            var manager = Substitute.For<IWebhookDefinitionManager>();
            var context = Substitute.For<IWebhookDefinitionContext>();
            context.Manager.Returns(manager);

            sut.SetWebhooks(context);

            manager.Received(1).Add(Arg.Any<WebhookDefinition>());
            manager.Received().Add(Arg.Is<WebhookDefinition>(d => d.Name == EafWebHookNames.NewUserRegistered));
        }

        #endregion
    }
}
