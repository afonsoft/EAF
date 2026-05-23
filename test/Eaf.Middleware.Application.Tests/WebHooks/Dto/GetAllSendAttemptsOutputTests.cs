using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using System;
using System.Net;
using Xunit;

namespace Eaf.Middleware.Application.Tests.WebHooks.Dto
{
    public class GetAllSendAttemptsOutputTests
    {
        [Fact]
        public void Dado_GetAllSendAttemptsOutput_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var output = new GetAllSendAttemptsOutput();

            output.CreationTime.ShouldBe(default(DateTime));
            output.Data.ShouldBeNull();
            output.Id.ShouldBe(Guid.Empty);
            output.Response.ShouldBeNull();
            output.ResponseStatusCode.ShouldBeNull();
            output.WebhookEventId.ShouldBe(Guid.Empty);
            output.WebhookName.ShouldBeNull();
        }

        [Fact]
        public void Dado_GetAllSendAttemptsOutput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var now = DateTime.UtcNow;
            var id = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            var output = new GetAllSendAttemptsOutput
            {
                CreationTime = now,
                Data = "{\"userId\": 1}",
                Id = id,
                Response = "OK",
                ResponseStatusCode = HttpStatusCode.OK,
                WebhookEventId = eventId,
                WebhookName = "App.UserCreated"
            };

            output.CreationTime.ShouldBe(now);
            output.Data.ShouldBe("{\"userId\": 1}");
            output.Id.ShouldBe(id);
            output.Response.ShouldBe("OK");
            output.ResponseStatusCode.ShouldBe(HttpStatusCode.OK);
            output.WebhookEventId.ShouldBe(eventId);
            output.WebhookName.ShouldBe("App.UserCreated");
        }
    }
}
