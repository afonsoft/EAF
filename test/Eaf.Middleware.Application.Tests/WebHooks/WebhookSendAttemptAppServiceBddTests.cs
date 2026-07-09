using Abp.Application.Services.Dto;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Abp.Webhooks;
using Abp.Webhooks.BackgroundWorker;
using Eaf.Middleware.WebHooks;
using Eaf.Middleware.WebHooks.Dto;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.WebHooks
{
    /// <summary>
    /// Testes BDD para WebhookSendAttemptAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class WebhookSendAttemptAppServiceBddTests
    {
        private readonly IWebhookSendAttemptStore _webhookSendAttemptStore;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IWebhookEventAppService _webhookEventAppService;
        private readonly IWebhookSubscriptionManager _webhookSubscriptionManager;
        private readonly IRepository<WebhookSubscriptionInfo, Guid> _subscriptionRepository;
        private readonly WebhookSendAttemptAppService _sut;

        public WebhookSendAttemptAppServiceBddTests()
        {
            _webhookSendAttemptStore = Substitute.For<IWebhookSendAttemptStore>();
            _backgroundJobManager = Substitute.For<IBackgroundJobManager>();
            _webhookEventAppService = Substitute.For<IWebhookEventAppService>();
            _webhookSubscriptionManager = Substitute.For<IWebhookSubscriptionManager>();
            _subscriptionRepository = Substitute.For<IRepository<WebhookSubscriptionInfo, Guid>>();

            _sut = new WebhookSendAttemptAppService(
                _webhookSendAttemptStore,
                _backgroundJobManager,
                _webhookEventAppService,
                _webhookSubscriptionManager,
                _subscriptionRepository
            );
        }

        #region Construtor

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region GetAllSendAttempts

        [Fact]
        public async Task Dado_SubscriptionIdVazio_Quando_GetAllSendAttempts_Entao_DeveLancarExcecao()
        {
            // Dado
            var input = new GetAllSendAttemptsInput
            {
                SubscriptionId = "",
                MaxResultCount = 10,
                SkipCount = 0
            };

            // Quando / Então
            await Should.ThrowAsync<ArgumentNullException>(() => _sut.GetAllSendAttempts(input));
        }

        [Fact]
        public async Task Dado_SubscriptionIdNulo_Quando_GetAllSendAttempts_Entao_DeveLancarExcecao()
        {
            // Dado
            var input = new GetAllSendAttemptsInput
            {
                SubscriptionId = null,
                MaxResultCount = 10,
                SkipCount = 0
            };

            // Quando / Então
            await Should.ThrowAsync<ArgumentNullException>(() => _sut.GetAllSendAttempts(input));
        }

        [Fact]
        public async Task Dado_SubscriptionIdValido_Quando_GetAllSendAttempts_Entao_DeveRetornarPaginaMapeada()
        {
            // Dado
            var subscriptionId = Guid.NewGuid();
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            _sut.AbpSession = abpSession;

            var attempt = new WebhookSendAttempt
            {
                Id = Guid.NewGuid(),
                WebhookEventId = Guid.NewGuid(),
                WebhookSubscriptionId = subscriptionId,
                Response = "OK",
                ResponseStatusCode = System.Net.HttpStatusCode.OK
            };

            var pagedResult = new PagedResultDto<WebhookSendAttempt>(
                1,
                new List<WebhookSendAttempt> { attempt }
            );

            _webhookSendAttemptStore
                .GetAllSendAttemptsBySubscriptionAsPagedListAsync(
                    abpSession.TenantId,
                    subscriptionId,
                    10,
                    0)
                .Returns(pagedResult);

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper
                .Map<List<GetAllSendAttemptsOutput>>(Arg.Any<object>())
                .Returns(callInfo =>
                {
                    var source = callInfo.Arg<object>();
                    var items = (source as IEnumerable<WebhookSendAttempt>)!;
                    return new List<GetAllSendAttemptsOutput>(
                        items.Select(x => new GetAllSendAttemptsOutput { Id = x.Id })
                    );
                });
            _sut.ObjectMapper = objectMapper;

            var input = new GetAllSendAttemptsInput
            {
                SubscriptionId = subscriptionId.ToString(),
                MaxResultCount = 10,
                SkipCount = 0
            };

            // Quando
            var result = await _sut.GetAllSendAttempts(input);

            // Então
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(1);
            result.Items.Count.ShouldBe(1);
            result.Items[0].Id.ShouldBe(attempt.Id);
            await _webhookSendAttemptStore
                .Received(1)
                .GetAllSendAttemptsBySubscriptionAsPagedListAsync(
                    abpSession.TenantId,
                    subscriptionId,
                    10,
                    0);
        }

        #endregion

        #region GetAllSendAttemptsOfWebhookEvent

        [Fact]
        public async Task Dado_IdVazio_Quando_GetAllSendAttemptsOfWebhookEvent_Entao_DeveLancarExcecao()
        {
            // Dado
            var input = new GetAllSendAttemptsOfWebhookEventInput { Id = "" };

            // Quando / Então
            await Should.ThrowAsync<ArgumentNullException>(() =>
                _sut.GetAllSendAttemptsOfWebhookEvent(input));
        }

        [Fact]
        public async Task Dado_IdNulo_Quando_GetAllSendAttemptsOfWebhookEvent_Entao_DeveLancarExcecao()
        {
            // Dado
            var input = new GetAllSendAttemptsOfWebhookEventInput { Id = null };

            // Quando / Então
            await Should.ThrowAsync<ArgumentNullException>(() =>
                _sut.GetAllSendAttemptsOfWebhookEvent(input));
        }

        [Fact]
        public async Task Dado_IdValido_Quando_GetAllSendAttemptsOfWebhookEvent_Entao_DeveMapearEAssociarWebhookUri()
        {
            // Dado
            var eventId = Guid.NewGuid();
            var subscriptionId = Guid.NewGuid();
            var webhookUri = "https://example.com/webhook";

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            _sut.AbpSession = abpSession;

            var attempt = new WebhookSendAttempt
            {
                Id = Guid.NewGuid(),
                WebhookEventId = eventId,
                WebhookSubscriptionId = subscriptionId
            };

            var attemptList = new List<WebhookSendAttempt> { attempt };

            _webhookSendAttemptStore
                .GetAllSendAttemptsByWebhookEventIdAsync(abpSession.TenantId, eventId)
                .Returns(attemptList);

            var subscription = new WebhookSubscriptionInfo
            {
                Id = subscriptionId,
                WebhookUri = webhookUri
            };
            _subscriptionRepository.GetAll().Returns(new List<WebhookSubscriptionInfo> { subscription }.AsQueryable());

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper
                .Map<List<GetAllSendAttemptsOfWebhookEventOutput>>(Arg.Any<object>())
                .Returns(callInfo =>
                {
                    var source = callInfo.Arg<object>();
                    var items = (source as IEnumerable<WebhookSendAttempt>)!;
                    return new List<GetAllSendAttemptsOfWebhookEventOutput>(
                        items.Select(x => new GetAllSendAttemptsOfWebhookEventOutput
                        {
                            Id = x.Id,
                            WebhookSubscriptionId = x.WebhookSubscriptionId
                        })
                    );
                });
            _sut.ObjectMapper = objectMapper;

            var input = new GetAllSendAttemptsOfWebhookEventInput { Id = eventId.ToString() };

            // Quando
            var result = await _sut.GetAllSendAttemptsOfWebhookEvent(input);

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
            result.Items[0].Id.ShouldBe(attempt.Id);
            result.Items[0].WebhookSubscriptionId.ShouldBe(subscriptionId);
            result.Items[0].WebhookUri.ShouldBe(webhookUri);
            await _webhookSendAttemptStore
                .Received(1)
                .GetAllSendAttemptsByWebhookEventIdAsync(abpSession.TenantId, eventId);
        }

        #endregion

        #region Resend

        [Fact]
        public async Task Dado_SendAttemptIdValido_Quando_Resend_Entao_DeveEnfileirarWebhookSenderJob()
        {
            // Dado
            var eventId = Guid.NewGuid();
            var subscriptionId = Guid.NewGuid();
            var attemptId = Guid.NewGuid();

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            _sut.AbpSession = abpSession;

            var webhookSendAttempt = new WebhookSendAttempt
            {
                Id = attemptId,
                WebhookEventId = eventId,
                WebhookSubscriptionId = subscriptionId
            };

            _webhookSendAttemptStore
                .GetAsync(abpSession.TenantId, attemptId)
                .Returns(webhookSendAttempt);

            var webhookEvent = new WebhookEvent
            {
                Id = eventId,
                Data = "{\"test\":true}",
                WebhookName = "Test"
            };
            _webhookEventAppService
                .Get(eventId.ToString())
                .Returns(webhookEvent);

            var webhookSubscription = new WebhookSubscription
            {
                Id = subscriptionId,
                WebhookUri = "https://example.com/webhook",
                Headers = new Dictionary<string, string> { { "X-Key", "value" } },
                Secret = "secret"
            };
            _webhookSubscriptionManager
                .GetAsync(subscriptionId)
                .Returns(webhookSubscription);

            _backgroundJobManager
                .Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Any<WebhookSenderArgs>())
                .Returns("job-id");

            // Quando
            await _sut.Resend(attemptId.ToString());

            // Então
            await _webhookSendAttemptStore.Received(1).GetAsync(abpSession.TenantId, attemptId);
            await _webhookEventAppService.Received(1).Get(eventId.ToString());
            await _webhookSubscriptionManager.Received(1).GetAsync(subscriptionId);
            _backgroundJobManager
                .Received(1)
                .Enqueue<WebhookSenderJob, WebhookSenderArgs>(
                    Arg.Is<WebhookSenderArgs>(args =>
                        args.TenantId == abpSession.TenantId &&
                        args.WebhookEventId == eventId &&
                        args.WebhookSubscriptionId == subscriptionId &&
                        args.Data == webhookEvent.Data &&
                        args.WebhookName == webhookEvent.WebhookName &&
                        args.WebhookUri == webhookSubscription.WebhookUri &&
                        args.Headers == webhookSubscription.Headers &&
                        args.Secret == webhookSubscription.Secret &&
                        args.TryOnce));
        }

        #endregion
    }
}
