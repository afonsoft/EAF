using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Eaf.Middleware.Application.Tests.WebHooks.Dto
{
    public class WebhooksDtoCoverageTests
    {
        [Fact]
        public void ActivateWebhookSubscriptionInput_ShouldSet()
        {
            var sid = Guid.NewGuid();
            var dto = new ActivateWebhookSubscriptionInput { IsActive = true, SubscriptionId = sid };
            dto.IsActive.ShouldBeTrue();
            dto.SubscriptionId.ShouldBe(sid);
        }

        [Fact]
        public void GetAllAvailableWebhooksOutput_ShouldSet()
        {
            var dto = new GetAllAvailableWebhooksOutput
            {
                Description = "d",
                DisplayName = "dn",
                Name = "n"
            };
            dto.Description.ShouldBe("d");
            dto.DisplayName.ShouldBe("dn");
            dto.Name.ShouldBe("n");
        }

        [Fact]
        public void GetAllSendAttemptsInput_ShouldSet()
        {
            var dto = new GetAllSendAttemptsInput { SubscriptionId = "sid" };
            dto.SubscriptionId.ShouldBe("sid");
        }

        [Fact]
        public void GetAllSendAttemptsOfWebhookEventInput_ShouldSet()
        {
            var dto = new GetAllSendAttemptsOfWebhookEventInput { Id = "eid" };
            dto.Id.ShouldBe("eid");
        }

        [Fact]
        public void GetAllSendAttemptsOfWebhookEventOutput_ShouldSet()
        {
            var id = Guid.NewGuid();
            var sid = Guid.NewGuid();
            var dto = new GetAllSendAttemptsOfWebhookEventOutput
            {
                CreationTime = new DateTime(2024, 1, 1),
                Id = id,
                LastModificationTime = new DateTime(2024, 2, 2),
                Response = "r",
                ResponseStatusCode = HttpStatusCode.OK,
                WebhookSubscriptionId = sid,
                WebhookUri = "http://x"
            };
            dto.CreationTime.ShouldBe(new DateTime(2024, 1, 1));
            dto.Id.ShouldBe(id);
            dto.LastModificationTime.ShouldBe(new DateTime(2024, 2, 2));
            dto.Response.ShouldBe("r");
            dto.ResponseStatusCode.ShouldBe(HttpStatusCode.OK);
            dto.WebhookSubscriptionId.ShouldBe(sid);
            dto.WebhookUri.ShouldBe("http://x");
        }

        [Fact]
        public void GetAllSendAttemptsOutput_ShouldSet()
        {
            var id = Guid.NewGuid();
            var evId = Guid.NewGuid();
            var dto = new GetAllSendAttemptsOutput
            {
                CreationTime = new DateTime(2024, 1, 1),
                Data = "{}",
                Id = id,
                Response = "r",
                ResponseStatusCode = HttpStatusCode.InternalServerError,
                WebhookEventId = evId,
                WebhookName = "n"
            };
            dto.CreationTime.ShouldBe(new DateTime(2024, 1, 1));
            dto.Data.ShouldBe("{}");
            dto.Id.ShouldBe(id);
            dto.Response.ShouldBe("r");
            dto.ResponseStatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            dto.WebhookEventId.ShouldBe(evId);
            dto.WebhookName.ShouldBe("n");
        }

        [Fact]
        public void GetAllSubscriptionsOutput_ShouldSet()
        {
            var dto = new GetAllSubscriptionsOutput
            {
                IsActive = true,
                Webhooks = new List<string> { "w1" },
                WebhookUri = "http://a"
            };
            dto.IsActive.ShouldBeTrue();
            dto.Webhooks.Count.ShouldBe(1);
            dto.WebhookUri.ShouldBe("http://a");
        }
    }
}
