using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eaf.Notifications.Push.Configuration;
using Eaf.Notifications.Push.Providers;
using Eaf.Notifications.Push.Tests.Fakes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Notifications.Push.Tests.Providers
{
    /// <summary>
    /// Testes unitários do <see cref="GenericHttpPushProvider"/>.
    /// </summary>
    public class GenericHttpPushProviderTests
    {
        private readonly FakeHttpMessageHandler _handler;
        private readonly IHttpClientFactory _factory;

        public GenericHttpPushProviderTests()
        {
            _handler = new FakeHttpMessageHandler();
            var httpClient = new HttpClient(_handler);
            _factory = Substitute.For<IHttpClientFactory>();
            _factory.CreateClient("EafPush").Returns(httpClient);
        }

        [Fact]
        public async Task Dado_ConfiguracaoGenerica_Quando_EnviarPush_Entao_DeveMontarRequisicaoComTodosOsPlaceholders()
        {
            var options = Options.Create(new PushOptions
            {
                GenericHttp = new GenericHttpPushProviderOptions
                {
                    BaseUrl = "https://api.zenvia.com",
                    Endpoint = "/services/push",
                    AuthenticationType = "Bearer",
                    Token = "abc",
                    ContentType = "Json",
                    Template = "{\"to\":\"{{endpoint}}\",\"title\":\"{{title}}\",\"body\":\"{{body}}\",\"icon\":\"{{icon}}\",\"tag\":\"{{tag}}\",\"data\":\"{{data}}\"}"
                }
            });
            var provider = new GenericHttpPushProvider(_factory, options);
            var subscription = new PushSubscription
            {
                Endpoint = "https://fcm.example.com/token",
                P256dh = "p256dh-value",
                Auth = "auth-value"
            };
            var message = new PushNotificationMessage
            {
                Title = "Alerta",
                Body = "Você tem uma nova mensagem",
                Icon = "/icon.png",
                Tag = "chat",
                Data = "{\"chatId\":1}"
            };

            var result = await provider.SendAsync(subscription, message);

            result.Succeeded.ShouldBeTrue();
            _handler.LastRequest.ShouldNotBeNull();
            _handler.LastRequest.RequestUri.ToString().ShouldBe("https://api.zenvia.com/services/push");
            _handler.LastRequest.Headers.Authorization.Scheme.ShouldBe("Bearer");

            var body = await _handler.LastRequest.Content.ReadAsStringAsync();
            body.ShouldContain("\"to\":\"https://fcm.example.com/token\"");
            body.ShouldContain("\"title\":\"Alerta\"");
            body.ShouldContain("\"body\":\"Você tem uma nova mensagem\"");
            body.ShouldContain("\"icon\":\"/icon.png\"");
            body.ShouldContain("\"tag\":\"chat\"");
        }

        [Fact]
        public async Task Dado_RespostaErro_Quando_EnviarPush_Entao_DeveRetornarFalha()
        {
            _handler.Response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("invalid") };
            var options = Options.Create(new PushOptions
            {
                GenericHttp = new GenericHttpPushProviderOptions
                {
                    BaseUrl = "https://api.push.com",
                    Endpoint = "/send",
                    ContentType = "Json",
                    Template = "{\"to\":\"{{endpoint}}\"}"
                }
            });
            var provider = new GenericHttpPushProvider(_factory, options);

            var result = await provider.SendAsync(
                new PushSubscription { Endpoint = "https://fcm.example.com/token" },
                new PushNotificationMessage { Title = "A", Body = "B" });

            result.Succeeded.ShouldBeFalse();
            result.ErrorMessage.ShouldContain("400");
        }
    }
}
