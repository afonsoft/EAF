using Eaf.WebHooks;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.WebHooks
{
    public class EafWebhookReceiverBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarAbstrata_Entao_DeveSerAbstrata()
        {
            typeof(EafWebHookReceiver).IsAbstract.ShouldBeTrue();
        }
    }
}
