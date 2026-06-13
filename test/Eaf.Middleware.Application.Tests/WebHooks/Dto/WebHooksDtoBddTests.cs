using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Eaf.Middleware.Application.Tests.WebHooks.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de WebHooks seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class WebHooksDtoBddTests
    {
        #region ActivateWebhookSubscriptionInput

        [Fact]
        public void Dado_ActivateWebhookSubscriptionInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var id = Guid.NewGuid();
            var input = new ActivateWebhookSubscriptionInput
            {
                SubscriptionId = id,
                IsActive = true
            };

            input.SubscriptionId.ShouldBe(id);
            input.IsActive.ShouldBeTrue();
        }

        #endregion

        #region GetAllAvailableWebhooksOutput

        [Fact]
        public void Dado_GetAllAvailableWebhooksOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new GetAllAvailableWebhooksOutput
            {
                Name = "App.NewUserRegistered",
                DisplayName = "Novo Usuário Registrado",
                Description = "Evento disparado quando um novo usuário se registra"
            };

            dto.Name.ShouldBe("App.NewUserRegistered");
            dto.DisplayName.ShouldBe("Novo Usuário Registrado");
            dto.Description.ShouldBe("Evento disparado quando um novo usuário se registra");
        }

        #endregion

        #region GetAllSendAttemptsInput

        [Fact]
        public void Dado_GetAllSendAttemptsInput_Quando_DefinirSubscriptionId_Entao_DeveArmazenar()
        {
            var input = new GetAllSendAttemptsInput { SubscriptionId = "sub-001" };
            input.SubscriptionId.ShouldBe("sub-001");
        }

        #endregion

        #region GetAllSendAttemptsOutput

        [Fact]
        public void Dado_GetAllSendAttemptsOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var id = Guid.NewGuid();
            var dto = new GetAllSendAttemptsOutput
            {
                Id = id,
                Data = "{\"userId\":42}",
                Response = "OK",
                ResponseStatusCode = HttpStatusCode.OK,
                CreationTime = new DateTime(2026, 6, 12),
                WebhookName = "App.Event",
                WebhookEventId = Guid.NewGuid()
            };

            dto.Id.ShouldBe(id);
            dto.Data.ShouldBe("{\"userId\":42}");
            dto.ResponseStatusCode.ShouldBe(HttpStatusCode.OK);
        }

        #endregion

        #region GetAllSendAttemptsOfWebhookEventInput

        [Fact]
        public void Dado_GetAllSendAttemptsOfWebhookEventInput_Quando_DefinirId_Entao_DeveArmazenar()
        {
            var input = new GetAllSendAttemptsOfWebhookEventInput { Id = "evt-001" };
            input.Id.ShouldBe("evt-001");
        }

        #endregion

        #region GetAllSendAttemptsOfWebhookEventOutput

        [Fact]
        public void Dado_GetAllSendAttemptsOfWebhookEventOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new GetAllSendAttemptsOfWebhookEventOutput
            {
                Id = Guid.NewGuid(),
                Response = "Error",
                ResponseStatusCode = HttpStatusCode.InternalServerError,
                CreationTime = DateTime.UtcNow
            };

            dto.ResponseStatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        }

        #endregion

        #region GetAllSubscriptionsOutput

        [Fact]
        public void Dado_GetAllSubscriptionsOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new GetAllSubscriptionsOutput
            {
                WebhookUri = "https://webhook.site/test",
                IsActive = true,
                Webhooks = new List<string> { "App.Event1", "App.Event2" }
            };

            dto.WebhookUri.ShouldBe("https://webhook.site/test");
            dto.IsActive.ShouldBeTrue();
            dto.Webhooks.Count.ShouldBe(2);
        }

        #endregion
    }
}
