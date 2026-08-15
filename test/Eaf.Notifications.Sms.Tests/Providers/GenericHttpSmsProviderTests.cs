using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eaf.Notifications.Sms.Configuration;
using Eaf.Notifications.Sms.Providers;
using Eaf.Notifications.Sms.Tests.Fakes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Notifications.Sms.Tests.Providers
{
    /// <summary>
    /// Testes unitários do <see cref="GenericHttpSmsProvider"/>.
    /// </summary>
    public class GenericHttpSmsProviderTests
    {
        private readonly FakeHttpMessageHandler _handler;
        private readonly IHttpClientFactory _factory;

        public GenericHttpSmsProviderTests()
        {
            _handler = new FakeHttpMessageHandler();
            var httpClient = new HttpClient(_handler);
            _factory = Substitute.For<IHttpClientFactory>();
            _factory.CreateClient("EafSms").Returns(httpClient);
        }

        [Fact]
        public async Task Dado_ConfiguracaoZenvia_Quando_EnviarSms_Entao_DeveMontarRequisicaoCorretamente()
        {
            var options = Options.Create(new SmsOptions
            {
                DefaultFrom = "EAF",
                GenericHttp = new GenericHttpSmsProviderOptions
                {
                    BaseUrl = "https://api.zenvia.com",
                    Endpoint = "/services/send-sms",
                    AuthenticationType = "Basic",
                    Username = "user",
                    Password = "pass",
                    ContentType = "Json",
                    Template = "{\"sendSmsRequest\":{\"from\":\"{{from}}\",\"to\":\"{{phoneNumber}}\",\"msg\":\"{{body}}\"}}"
                }
            });
            var provider = new GenericHttpSmsProvider(_factory, options);
            var message = new SmsMessage { PhoneNumber = "+5511987654321", Body = "Código 123" };

            var result = await provider.SendAsync(message);

            result.Succeeded.ShouldBeTrue();
            _handler.LastRequest.ShouldNotBeNull();
            _handler.LastRequest.RequestUri.ToString().ShouldBe("https://api.zenvia.com/services/send-sms");

            var body = await _handler.LastRequest.Content.ReadAsStringAsync();
            body.ShouldContain("\"to\":\"+5511987654321\"");
            body.ShouldContain("\"msg\":\"Código 123\"");
            body.ShouldContain("\"from\":\"EAF\"");

            _handler.LastRequest.Headers.Authorization.Scheme.ShouldBe("Basic");
        }

        [Fact]
        public async Task Dado_AuthBearer_Quando_EnviarSms_Entao_DeveAdicionarHeaderCorreto()
        {
            var options = Options.Create(new SmsOptions
            {
                GenericHttp = new GenericHttpSmsProviderOptions
                {
                    BaseUrl = "https://api.custom-sms.com",
                    Endpoint = "/send",
                    AuthenticationType = "Bearer",
                    Token = "abc123",
                    ContentType = "Json",
                    Template = "{\"to\":\"{{phoneNumber}}\",\"text\":\"{{body}}\"}"
                }
            });
            var provider = new GenericHttpSmsProvider(_factory, options);

            await provider.SendAsync(new SmsMessage { PhoneNumber = "+5511987654321", Body = "Teste" });

            _handler.LastRequest.Headers.Authorization.Scheme.ShouldBe("Bearer");
            _handler.LastRequest.Headers.Authorization.Parameter.ShouldBe("abc123");
        }

        [Fact]
        public async Task Dado_RespostaErro_Quando_EnviarSms_Entao_DeveRetornarFalha()
        {
            _handler.Response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("invalid") };
            var options = Options.Create(new SmsOptions
            {
                GenericHttp = new GenericHttpSmsProviderOptions
                {
                    BaseUrl = "https://api.custom-sms.com",
                    Endpoint = "/send",
                    ContentType = "Json",
                    Template = "{\"to\":\"{{phoneNumber}}\"}"
                }
            });
            var provider = new GenericHttpSmsProvider(_factory, options);

            var result = await provider.SendAsync(new SmsMessage { PhoneNumber = "+5511987654321", Body = "Teste" });

            result.Succeeded.ShouldBeFalse();
            result.ErrorMessage.ShouldContain("400");
        }
    }
}
