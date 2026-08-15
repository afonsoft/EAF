using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Abp.UI;
using Eaf.Notifications.Push.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Notifications.Push.Tests
{
    /// <summary>
    /// Testes unitários do <see cref="EafPushNotificationSender"/>.
    /// </summary>
    public class EafPushNotificationSenderTests
    {
        private readonly IPushNotificationProvider _webPushProvider;
        private readonly IPushNotificationProvider _genericProvider;
        private readonly List<IPushNotificationProvider> _providers;

        public EafPushNotificationSenderTests()
        {
            _webPushProvider = Substitute.For<IPushNotificationProvider>();
            _webPushProvider.Name.Returns("WebPush");
            _genericProvider = Substitute.For<IPushNotificationProvider>();
            _genericProvider.Name.Returns("GenericHttp");
            _providers = new List<IPushNotificationProvider> { _webPushProvider, _genericProvider };
        }

        [Fact]
        public async Task Dado_ProviderGenericHttpConfigurado_Quando_EnviarPush_Entao_DeveDelegarParaProviderCorreto()
        {
            var options = Options.Create(new PushOptions { Provider = "GenericHttp" });
            var sender = new EafPushNotificationSender(options, _providers);
            var subscription = new PushSubscription { Endpoint = "https://example.com/endpoint" };
            var message = new PushNotificationMessage { Title = "Alerta", Body = "Teste" };

            _genericProvider.SendAsync(subscription, message, Arg.Any<CancellationToken>()).Returns(Task.FromResult(new PushSendResult { Succeeded = true }));

            var result = await sender.SendAsync(subscription, message);

            result.Succeeded.ShouldBeTrue();
            await _genericProvider.Received(1).SendAsync(subscription, message, Arg.Any<CancellationToken>());
            await _webPushProvider.DidNotReceive().SendAsync(Arg.Any<PushSubscription>(), Arg.Any<PushNotificationMessage>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_ProviderNaoConfigurado_Quando_EnviarPush_Entao_DeveUsarPrimeiroProviderDisponivel()
        {
            var options = Options.Create(new PushOptions());
            var sender = new EafPushNotificationSender(options, _providers);
            var subscription = new PushSubscription { Endpoint = "https://example.com/endpoint" };
            var message = new PushNotificationMessage { Title = "Alerta", Body = "Teste" };

            _webPushProvider.SendAsync(subscription, message, Arg.Any<CancellationToken>()).Returns(Task.FromResult(new PushSendResult { Succeeded = true }));

            await sender.SendAsync(subscription, message);

            await _webPushProvider.Received(1).SendAsync(subscription, message, Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Dado_ProviderInexistente_Quando_EnviarPush_Entao_DeveLancarExcecao()
        {
            var options = Options.Create(new PushOptions { Provider = "Inexistente" });
            var sender = new EafPushNotificationSender(options, _providers);

            Should.Throw<UserFriendlyException>(() => sender.SendAsync(new PushSubscription { Endpoint = "x" }, new PushNotificationMessage()));
        }

        [Fact]
        public void Dado_EndpointVazio_Quando_EnviarPush_Entao_DeveLancarExcecao()
        {
            var sender = new EafPushNotificationSender(Options.Create(new PushOptions()), _providers);

            Should.Throw<UserFriendlyException>(() => sender.SendAsync(new PushSubscription(), new PushNotificationMessage()));
        }
    }
}
