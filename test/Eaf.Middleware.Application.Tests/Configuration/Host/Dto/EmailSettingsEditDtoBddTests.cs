using Eaf.Middleware.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    public class EmailSettingsEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new EmailSettingsEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDefaultFromDisplayName_Entao_DeveArmazenar()
        {
            var sut = new EmailSettingsEditDto();
            sut.DefaultFromDisplayName = "test_value";
            sut.DefaultFromDisplayName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSmtpDomain_Entao_DeveArmazenar()
        {
            var sut = new EmailSettingsEditDto();
            sut.SmtpDomain = "test_value";
            sut.SmtpDomain.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSmtpEnableSsl_Entao_DeveArmazenar()
        {
            var sut = new EmailSettingsEditDto();
            sut.SmtpEnableSsl = true;
            sut.SmtpEnableSsl.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSmtpHost_Entao_DeveArmazenar()
        {
            var sut = new EmailSettingsEditDto();
            sut.SmtpHost = "test_value";
            sut.SmtpHost.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSmtpPassword_Entao_DeveArmazenar()
        {
            var sut = new EmailSettingsEditDto();
            sut.SmtpPassword = "test_value";
            sut.SmtpPassword.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSmtpPort_Entao_DeveArmazenar()
        {
            var sut = new EmailSettingsEditDto();
            sut.SmtpPort = 42;
            sut.SmtpPort.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSmtpUseDefaultCredentials_Entao_DeveArmazenar()
        {
            var sut = new EmailSettingsEditDto();
            sut.SmtpUseDefaultCredentials = true;
            sut.SmtpUseDefaultCredentials.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSmtpUserName_Entao_DeveArmazenar()
        {
            var sut = new EmailSettingsEditDto();
            sut.SmtpUserName = "test_value";
            sut.SmtpUserName.ShouldBe("test_value");
        }
    }
}
