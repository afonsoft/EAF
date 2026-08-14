using Eaf.MailKit.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.MailKit.Tests
{
    public class EafMailKitConfigurationTests
    {
        [Fact]
        public void Dado_ConfiguracaoPadrao_Quando_Criar_Entao_Valores_Sao_Razoaveis()
        {
            var configuration = new EafMailKitConfiguration();

            configuration.RetryCount.ShouldBe(3);
            configuration.RetryDelayMilliseconds.ShouldBe(500);
            configuration.DisableCertificateValidation.ShouldBeFalse();
        }
    }
}
