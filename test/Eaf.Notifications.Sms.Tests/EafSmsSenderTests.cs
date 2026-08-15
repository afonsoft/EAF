using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Abp.UI;
using Eaf.Notifications.Sms.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Notifications.Sms.Tests
{
    /// <summary>
    /// Testes unitários do <see cref="EafSmsSender"/>.
    /// </summary>
    public class EafSmsSenderTests
    {
        private readonly ISmsProvider _genericProvider;
        private readonly ISmsProvider _twilioProvider;
        private readonly List<ISmsProvider> _providers;

        public EafSmsSenderTests()
        {
            _genericProvider = Substitute.For<ISmsProvider>();
            _genericProvider.Name.Returns("GenericHttp");
            _twilioProvider = Substitute.For<ISmsProvider>();
            _twilioProvider.Name.Returns("Twilio");
            _providers = new List<ISmsProvider> { _genericProvider, _twilioProvider };
        }

        [Fact]
        public async Task Dado_ProviderGenericHttpConfigurado_Quando_EnviarSms_Entao_DeveDelegarParaProviderCorreto()
        {
            var options = Options.Create(new SmsOptions { Provider = "GenericHttp" });
            var sender = new EafSmsSender(options, _providers);
            var message = new SmsMessage { PhoneNumber = "+5511987654321", Body = "Teste" };

            _genericProvider.SendAsync(message, Arg.Any<CancellationToken>()).Returns(Task.FromResult(new SmsSendResult { Succeeded = true }));

            var result = await sender.SendAsync(message);

            result.Succeeded.ShouldBeTrue();
            await _genericProvider.Received(1).SendAsync(message, Arg.Any<CancellationToken>());
            await _twilioProvider.DidNotReceive().SendAsync(Arg.Any<SmsMessage>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_ProviderNaoConfigurado_Quando_EnviarSms_Entao_DeveUsarPrimeiroProviderDisponivel()
        {
            var options = Options.Create(new SmsOptions());
            var sender = new EafSmsSender(options, _providers);
            var message = new SmsMessage { PhoneNumber = "+5511987654321", Body = "Teste" };

            _genericProvider.SendAsync(message, Arg.Any<CancellationToken>()).Returns(Task.FromResult(new SmsSendResult { Succeeded = true }));

            await sender.SendAsync(message);

            await _genericProvider.Received(1).SendAsync(message, Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Dado_ProviderInexistente_Quando_EnviarSms_Entao_DeveLancarExcecao()
        {
            var options = Options.Create(new SmsOptions { Provider = "Inexistente" });
            var sender = new EafSmsSender(options, _providers);

            Should.Throw<UserFriendlyException>(() => sender.SendAsync(new SmsMessage { PhoneNumber = "123", Body = "Teste" }));
        }

        [Fact]
        public void Dado_TelefoneVazio_Quando_EnviarSms_Entao_DeveLancarExcecao()
        {
            var sender = new EafSmsSender(Options.Create(new SmsOptions()), _providers);

            Should.Throw<UserFriendlyException>(() => sender.SendAsync(new SmsMessage { PhoneNumber = "", Body = "Teste" }));
        }

        [Fact]
        public void Dado_MensagemVazia_Quando_EnviarSms_Entao_DeveLancarExcecao()
        {
            var sender = new EafSmsSender(Options.Create(new SmsOptions()), _providers);

            Should.Throw<UserFriendlyException>(() => sender.SendAsync(new SmsMessage { PhoneNumber = "+5511987654321", Body = "" }));
        }
    }
}
