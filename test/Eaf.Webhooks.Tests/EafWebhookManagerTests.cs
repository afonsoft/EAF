using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Abp.Json;
using Abp.UI;
using Abp.Webhooks;
using Eaf.Webhooks.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Webhooks.Tests
{
    /// <summary>
    /// Testes BDD para EafWebhookManager seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class EafWebhookManagerTests
    {
        private static EafWebhookManager CriarGerenciador(IWebhookSubscriptionSecretProtector protector = null)
        {
            var configuration = Substitute.For<IWebhooksConfiguration>();
            var options = Options.Create(new EafWebhooksOptions());

            return new EafWebhookManager(
                configuration,
                Substitute.For<IWebhookSendAttemptStore>(),
                protector ?? Substitute.For<IWebhookSubscriptionSecretProtector>(),
                options);
        }

        private static IWebhookSubscriptionSecretProtector CriarProtetorSimples()
        {
            var protector = Substitute.For<IWebhookSubscriptionSecretProtector>();
            protector.Protect(Arg.Any<string>()).Returns(ci => ci.Arg<string>());
            protector.Unprotect(Arg.Any<string>()).Returns(ci => ci.Arg<string>());
            return protector;
        }

        [Fact]
        public void Dado_ConstrutorComDependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            var sut = CriarGerenciador();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_SendExactSameDataAtivado_Quando_SerializarCorpo_Entao_DeveRetornarDataOriginal()
        {
            // Dado
            var sut = CriarGerenciador();
            var args = new WebhookSenderArgs
            {
                WebhookName = "Test",
                Data = "{\"foo\":1}",
                SendExactSameData = true
            };

            // Quando
            var corpo = await sut.GetSerializedBodyAsync(args);

            // Então
            corpo.ShouldBe("{\"foo\":1}");
        }

        [Fact]
        public async Task Dado_WebhookPayload_Quando_SerializarCorpo_Entao_DeveGerarPayloadEaf()
        {
            // Dado
            var sut = CriarGerenciador();
            var creationTime = new DateTime(2026, 8, 15, 0, 0, 0, DateTimeKind.Utc);
            var payload = new WebhookPayload("id", "Test", 1)
            {
                Data = new { foo = 1 },
                CreationTimeUtc = creationTime
            };

            var args = new WebhookSenderArgs
            {
                WebhookName = "Test",
                Data = payload.ToJsonString(),
                WebhookEventId = Guid.NewGuid()
            };

            // Quando
            var corpo = await sut.GetSerializedBodyAsync(args);

            // Então
            corpo.ShouldNotBeNullOrWhiteSpace();
            corpo.ShouldContain("\"eventName\":");
            corpo.ShouldContain("\"timestamp\":");
            corpo.ShouldContain("\"payload\":");
            corpo.ShouldContain("\"foo\":1");
        }

        [Fact]
        public async Task Dado_CorpoESegredo_Quando_AssinarRequisicao_Entao_DeveAdicionarHeaderHmac256EmMinusculo()
        {
            // Dado
            var protector = CriarProtetorSimples();
            var sut = CriarGerenciador(protector);
            var corpo = "{\"eventName\":\"Test\"}";
            var segredo = "segredo-super-seguro";
            var request = new HttpRequestMessage();

            // Quando
            sut.SignWebhookRequest(request, corpo, segredo);

            // Então
            request.Headers.ShouldContain(h => h.Key == "X-Eaf-Signature-256");
            var valor = request.Headers.GetValues("X-Eaf-Signature-256").Single();
            valor.ShouldStartWith("sha256=");

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(segredo)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(corpo));
                var esperado = "sha256=" + Convert.ToHexString(hash).ToLowerInvariant();
                valor.ShouldBe(esperado);
            }

            var conteudo = await request.Content.ReadAsStringAsync();
            conteudo.ShouldBe(corpo);
        }

        [Fact]
        public void Dado_SegredoAusente_Quando_AssinarRequisicao_Entao_DeveLancarExcecao()
        {
            // Dado
            var protector = Substitute.For<IWebhookSubscriptionSecretProtector>();
            protector.Unprotect(Arg.Any<string>()).Returns(string.Empty);
            var sut = CriarGerenciador(protector);
            var request = new HttpRequestMessage();

            // Quando / Então
            Should.Throw<ArgumentException>(() => sut.SignWebhookRequest(request, "{}", "cifrado"));
        }
    }
}
