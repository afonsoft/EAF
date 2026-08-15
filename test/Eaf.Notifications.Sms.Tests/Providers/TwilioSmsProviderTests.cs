using System.Net;
using System.Net.Http;
using System.Text;
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
    /// Testes unitários do <see cref="TwilioSmsProvider"/>.
    /// </summary>
    public class TwilioSmsProviderTests
    {
        private readonly FakeHttpMessageHandler _handler;
        private readonly IHttpClientFactory _factory;

        public TwilioSmsProviderTests()
        {
            _handler = new FakeHttpMessageHandler();
            var httpClient = new HttpClient(_handler);
            _factory = Substitute.For<IHttpClientFactory>();
            _factory.CreateClient("EafSms").Returns(httpClient);
        }

        [Fact]
        public async Task Dado_CredenciaisValidas_Quando_EnviarSms_Entao_DevePostarParaTwilioComBasicAuth()
        {
            var options = Options.Create(new SmsOptions
            {
                Twilio = new TwilioSmsProviderOptions
                {
                    AccountSid = "AC123",
                    AuthToken = "token",
                    From = "+15551234567"
                }
            });
            var provider = new TwilioSmsProvider(_factory, options);

            var result = await provider.SendAsync(new SmsMessage
            {
                PhoneNumber = "+5511987654321",
                Body = "Olá"
            });

            result.Succeeded.ShouldBeTrue();
            _handler.LastRequest.RequestUri.ToString().ShouldBe("https://api.twilio.com/2010-04-01/Accounts/AC123/Messages.json");
            _handler.LastRequest.Headers.Authorization.Scheme.ShouldBe("Basic");

            var body = await _handler.LastRequest.Content.ReadAsStringAsync();
            body.ShouldContain("To=%2B5511987654321");
            body.ShouldContain("From=%2B15551234567");
            body.ShouldContain("Body=Ol%C3%A1");
        }

        [Fact]
        public async Task Dado_RespostaErro_Quando_EnviarSms_Entao_DeveRetornarFalha()
        {
            _handler.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("unauthorized") };
            var options = Options.Create(new SmsOptions
            {
                Twilio = new TwilioSmsProviderOptions
                {
                    AccountSid = "AC123",
                    AuthToken = "token",
                    From = "+15551234567"
                }
            });
            var provider = new TwilioSmsProvider(_factory, options);

            var result = await provider.SendAsync(new SmsMessage { PhoneNumber = "+5511987654321", Body = "Olá" });

            result.Succeeded.ShouldBeFalse();
            result.ErrorMessage.ShouldContain("401");
        }
    }
}
