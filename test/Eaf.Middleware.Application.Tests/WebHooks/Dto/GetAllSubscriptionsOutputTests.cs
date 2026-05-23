using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.WebHooks.Dto
{
    public class GetAllSubscriptionsOutputTests
    {
        [Fact]
        public void Dado_GetAllSubscriptionsOutput_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var output = new GetAllSubscriptionsOutput();

            output.IsActive.ShouldBeFalse();
            output.Webhooks.ShouldBeNull();
            output.WebhookUri.ShouldBeNull();
        }

        [Fact]
        public void Dado_GetAllSubscriptionsOutput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var output = new GetAllSubscriptionsOutput
            {
                IsActive = true,
                Webhooks = new List<string> { "App.UserCreated", "App.UserDeleted" },
                WebhookUri = "https://example.com/webhook"
            };

            output.IsActive.ShouldBeTrue();
            output.Webhooks.Count.ShouldBe(2);
            output.WebhookUri.ShouldBe("https://example.com/webhook");
        }
    }
}
