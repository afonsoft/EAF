using Eaf.Webhooks.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace Eaf.Webhooks.Tests
{
    /// <summary>
    /// Testes BDD para os protetores de segredo de assinatura de webhook.
    /// </summary>
    public class EafWebhookSecretProtectorTests
    {
        [Fact]
        public void Dado_Segredo_Quando_ProtegerComDataProtection_Entao_DeveRecuperarValorOriginal()
        {
            // Dado
            var provider = new EphemeralDataProtectionProvider();
            var options = Options.Create(new EafWebhooksOptions());
            var sut = new EafDataProtectionWebhookSecretProtector(provider, options);

            // Quando
            var cifrado = sut.Protect("meu-segredo");
            var original = sut.Unprotect(cifrado);

            // Então
            cifrado.ShouldNotBe("meu-segredo");
            original.ShouldBe("meu-segredo");
        }

        [Fact]
        public void Dado_SegredoVazio_Quando_ProtegerComDataProtection_Entao_DeveRetornarVazio()
        {
            // Dado
            var provider = new EphemeralDataProtectionProvider();
            var options = Options.Create(new EafWebhooksOptions());
            var sut = new EafDataProtectionWebhookSecretProtector(provider, options);

            // Quando / Então
            sut.Protect(null).ShouldBeNull();
            sut.Protect(string.Empty).ShouldBe(string.Empty);
            sut.Unprotect(null).ShouldBeNull();
            sut.Unprotect(string.Empty).ShouldBe(string.Empty);
        }

        [Fact]
        public void Dado_Segredo_Quando_UsarPlainProtector_Entao_DeveManterValorOriginal()
        {
            // Dado
            var sut = new EafPlainWebhookSecretProtector();

            // Quando / Então
            sut.Protect("meu-segredo").ShouldBe("meu-segredo");
            sut.Unprotect("meu-segredo").ShouldBe("meu-segredo");
            sut.Protect(null).ShouldBeNull();
            sut.Unprotect(string.Empty).ShouldBe(string.Empty);
        }
    }
}
