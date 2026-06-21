using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.Webhooks;
using Eaf.Middleware.WebHooks;
using Eaf.Middleware.WebHooks.Dto;
using NSubstitute;
using Shouldly;
using System;
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

        #endregion
    }
}
