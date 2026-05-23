using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.WebHooks.Dto
{
    public class ActivateWebhookSubscriptionInputTests
    {
        [Fact]
        public void Dado_ActivateWebhookSubscriptionInput_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var input = new ActivateWebhookSubscriptionInput();

            input.IsActive.ShouldBeFalse();
            input.SubscriptionId.ShouldBe(Guid.Empty);
        }

        [Fact]
        public void Dado_ActivateWebhookSubscriptionInput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var id = Guid.NewGuid();
            var input = new ActivateWebhookSubscriptionInput
            {
                IsActive = true,
                SubscriptionId = id
            };

            input.IsActive.ShouldBeTrue();
            input.SubscriptionId.ShouldBe(id);
        }
    }
}
