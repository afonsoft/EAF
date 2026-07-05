using System.Threading.Tasks;
using Abp.Net.Mail;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Configuration.Host.Dto;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration
{
    /// <summary>
    /// Testes BDD para SettingsAppServiceBase seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class SettingsAppServiceBaseBddTests
    {
        private sealed class TestableSettingsAppService : SettingsAppServiceBase
        {
            public TestableSettingsAppService(IEmailSender emailSender) : base(emailSender)
            {
            }
        }

        [Fact]
        public void Dado_TipoSettingsAppServiceBase_Quando_Verificar_Entao_DeveSerAbstrato()
        {
            typeof(SettingsAppServiceBase).IsAbstract.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_Input_Quando_SendTestEmail_Entao_DeveEnviarEmailParaEndereco()
        {
            var emailSender = Substitute.For<IEmailSender>();
            var sut = new TestableSettingsAppService(emailSender);
            var input = new SendTestEmailInput { EmailAddress = "teste@exemplo.com" };

            await sut.SendTestEmail(input);

            await emailSender.Received(1).SendAsync("teste@exemplo.com", "TestEmail_Subject", "TestEmail_Body");
        }
    }
}
