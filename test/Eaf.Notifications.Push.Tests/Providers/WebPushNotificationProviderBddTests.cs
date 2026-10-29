using Abp.UI;
using Eaf.Notifications.Push.Configuration;
using Eaf.Notifications.Push.Providers;
using Eaf.Notifications.Push.Tests.Fakes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Notifications.Push.Tests.Providers
{
    /// <summary>
    /// Testes BDD para WebPushNotificationProvider seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class WebPushNotificationProviderBddTests
    {
        private readonly FakeHttpMessageHandler _handler = new();
        private readonly IHttpClientFactory _httpClientFactory;

        public WebPushNotificationProviderBddTests()
        {
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _httpClientFactory.CreateClient(Arg.Any<string>())
                .Returns(_ => new HttpClient(_handler));
        }

        private WebPushNotificationProvider CriarProvider(Action<PushOptions> configurar = null)
        {
            var options = new PushOptions();
            configurar?.Invoke(options);
            return new WebPushNotificationProvider(_httpClientFactory, Options.Create(options));
        }

        private static PushSubscription CriarSubscription()
        {
            return new PushSubscription
            {
                Id = 1,
                Endpoint = "https://push.example.com/send/abc",
                P256dh = "BP256dh",
                Auth = "YXV0aA==",
            };
        }

        private static PushNotificationMessage CriarMensagem()
        {
            return new PushNotificationMessage { Title = "Título", Body = "Corpo" };
        }

        [Fact]
        public void Dado_Nome_Quando_Consultar_Entao_DeveSerWebPush()
        {
            CriarProvider().Name.ShouldBe("WebPush");
        }

        [Fact]
        public void Dado_HttpClientFactoryNulo_Quando_Construir_Entao_DeveLancarArgumentNull()
        {
            Should.Throw<ArgumentNullException>(() =>
                new WebPushNotificationProvider(null, Options.Create(new PushOptions())));
        }

        [Fact]
        public void Dado_OptionsNulo_Quando_Construir_Entao_DeveLancarArgumentNull()
        {
            Should.Throw<ArgumentNullException>(() =>
                new WebPushNotificationProvider(_httpClientFactory, null));
        }

        [Fact]
        public async Task Dado_SubscriptionNula_Quando_Enviar_Entao_DeveLancarArgumentNull()
        {
            var sut = CriarProvider();
            await Should.ThrowAsync<ArgumentNullException>(() => sut.SendAsync(null, CriarMensagem()));
        }

        [Fact]
        public async Task Dado_MensagemNula_Quando_Enviar_Entao_DeveLancarArgumentNull()
        {
            var sut = CriarProvider();
            await Should.ThrowAsync<ArgumentNullException>(() => sut.SendAsync(CriarSubscription(), null));
        }

        [Fact]
        public async Task Dado_SemConfiguracaoWebPush_Quando_Enviar_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var sut = CriarProvider(o => o.WebPush = null);

            // Quando / Então
            var ex = await Should.ThrowAsync<UserFriendlyException>(() =>
                sut.SendAsync(CriarSubscription(), CriarMensagem()));
            ex.Message.ShouldContain("WebPush");
        }

        [Fact]
        public async Task Dado_SemPublicKey_Quando_Enviar_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var sut = CriarProvider(o => o.WebPush.PublicKey = null);

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() =>
                sut.SendAsync(CriarSubscription(), CriarMensagem()));
        }

        [Fact]
        public async Task Dado_SemPrivateKey_Quando_Enviar_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var sut = CriarProvider(o =>
            {
                o.WebPush.PublicKey = "pub";
                o.WebPush.PrivateKey = null;
            });

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() =>
                sut.SendAsync(CriarSubscription(), CriarMensagem()));
        }

        [Fact]
        public async Task Dado_SemSubject_Quando_Enviar_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var sut = CriarProvider(o =>
            {
                o.WebPush.PublicKey = "pub";
                o.WebPush.PrivateKey = "priv";
                o.WebPush.Subject = null;
            });

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() =>
                sut.SendAsync(CriarSubscription(), CriarMensagem()));
        }

        [Fact]
        public async Task Dado_SubscriptionSemP256dh_Quando_Enviar_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var sut = CriarProvider(o =>
            {
                o.WebPush.PublicKey = "pub";
                o.WebPush.PrivateKey = "priv";
                o.WebPush.Subject = "mailto:a@b.com";
            });
            var subscription = CriarSubscription();
            subscription.P256dh = null;

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() =>
                sut.SendAsync(subscription, CriarMensagem()));
        }

        [Fact]
        public async Task Dado_SubscriptionSemAuth_Quando_Enviar_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var sut = CriarProvider(o =>
            {
                o.WebPush.PublicKey = "pub";
                o.WebPush.PrivateKey = "priv";
                o.WebPush.Subject = "mailto:a@b.com";
            });
            var subscription = CriarSubscription();
            subscription.Auth = null;

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() =>
                sut.SendAsync(subscription, CriarMensagem()));
        }

        [Fact]
        public async Task Dado_ConfiguracaoValidaEEndpointSucesso_Quando_Enviar_Entao_DeveRetornarSucesso()
        {
            // Dado
            var vapid = WebPush.VapidHelper.GenerateVapidKeys();
            var sut = CriarProvider(o =>
            {
                o.WebPush.PublicKey = vapid.PublicKey;
                o.WebPush.PrivateKey = vapid.PrivateKey;
                o.WebPush.Subject = "mailto:test@eaf.dev";
                o.WebPush.TimeoutSeconds = 15;
            });

            _handler.Response = new HttpResponseMessage(HttpStatusCode.Created);

            var subscription = CriarSubscriptionReal();

            // Quando
            var resultado = await sut.SendAsync(subscription, CriarMensagem());

            // Então
            resultado.Succeeded.ShouldBeTrue();
            _handler.LastRequest.ShouldNotBeNull();
            _handler.LastRequest.RequestUri.ToString().ShouldBe(subscription.Endpoint);
        }

        [Fact]
        public async Task Dado_EndpointRetornaGone_Quando_Enviar_Entao_DeveRetornarFalhaComStatus()
        {
            // Dado
            var vapid = WebPush.VapidHelper.GenerateVapidKeys();
            var sut = CriarProvider(o =>
            {
                o.WebPush.PublicKey = vapid.PublicKey;
                o.WebPush.PrivateKey = vapid.PrivateKey;
                o.WebPush.Subject = "mailto:test@eaf.dev";
            });

            _handler.Response = new HttpResponseMessage(HttpStatusCode.Gone);

            var subscription = CriarSubscriptionReal();

            // Quando
            var resultado = await sut.SendAsync(subscription, CriarMensagem());

            // Então
            resultado.Succeeded.ShouldBeFalse();
            resultado.ErrorMessage.ShouldContain("410");
        }

        private static PushSubscription CriarSubscriptionReal()
        {
            // Gera um par de chaves ECDH P-256 real no formato p256dh (ponto não comprimido)
            // e auth de 16 bytes, aceitos pela criptografia do WebPush.
            using var ecdh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
            var p = ecdh.ExportParameters(false);
            var p256dh = new byte[65];
            p256dh[0] = 0x04;
            Buffer.BlockCopy(p.Q.X, 0, p256dh, 1, 32);
            Buffer.BlockCopy(p.Q.Y, 0, p256dh, 33, 32);

            var auth = new byte[16];
            RandomNumberGenerator.Fill(auth);

            return new PushSubscription
            {
                Id = 1,
                Endpoint = "https://push.example.com/send/real",
                P256dh = Base64UrlEncode(p256dh),
                Auth = Base64UrlEncode(auth),
            };
        }

        private static string Base64UrlEncode(byte[] data)
        {
            return Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
    }
}
