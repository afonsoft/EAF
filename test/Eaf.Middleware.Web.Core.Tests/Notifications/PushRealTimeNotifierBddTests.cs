using Abp.Domain.Repositories;
using Abp.Notifications;
using Eaf.Notifications;
using Eaf.Notifications.Push;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Notifications
{
    /// <summary>
    /// Testes BDD para PushRealTimeNotifier seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class PushRealTimeNotifierBddTests
    {
        private readonly IRepository<PushSubscription, long> _repository;
        private readonly IPushNotificationSender _pushSender;
        private readonly PushRealTimeNotifier _sut;

        public PushRealTimeNotifierBddTests()
        {
            _repository = Substitute.For<IRepository<PushSubscription, long>>();
            _pushSender = Substitute.For<IPushNotificationSender>();
            _sut = new PushRealTimeNotifier(_repository, _pushSender);
        }

        private static UserNotification CriarUserNotification(NotificationData data, long userId = 7, int? tenantId = 1)
        {
            return new UserNotification
            {
                UserId = userId,
                TenantId = tenantId,
                Notification = new TenantNotification { Data = data },
            };
        }

        [Fact]
        public void Dado_Notificador_Quando_VerificarUseOnlyIfRequestedAsTarget_Entao_DeveSerFalso()
        {
            _sut.UseOnlyIfRequestedAsTarget.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_NotificacaoPushComDados_Quando_Enviar_Entao_DeveEnviarParaCadaSubscription()
        {
            // Dado
            var subscriptions = new List<PushSubscription>
            {
                new PushSubscription { Id = 1, UserId = 7, TenantId = 1, Endpoint = "https://a" },
                new PushSubscription { Id = 2, UserId = 7, TenantId = 1, Endpoint = "https://b" },
            };
            _repository.GetAllListAsync(Arg.Any<Expression<Func<PushSubscription, bool>>>())
                .Returns(subscriptions);

            var data = new PushNotificationData
            {
                Message = "Olá",
                Icon = "icon.png",
                Data = "{}",
                Tag = "tag1",
            };
            var userNotifications = new[] { CriarUserNotification(data) };

            // Quando
            await _sut.SendNotificationsAsync(userNotifications);

            // Então
            await _pushSender.Received(2).SendAsync(
                Arg.Any<PushSubscription>(),
                Arg.Is<PushNotificationMessage>(m =>
                    m.Title == "Olá" && m.Body == "Olá" &&
                    m.Icon == "icon.png" && m.Data == "{}" && m.Tag == "tag1"));
        }

        [Fact]
        public async Task Dado_NotificacaoDeMensagemGenerica_Quando_Enviar_Entao_DeveEnviarComTituloECorpo()
        {
            // Dado
            _repository.GetAllListAsync(Arg.Any<Expression<Func<PushSubscription, bool>>>())
                .Returns(new List<PushSubscription> { new PushSubscription { Id = 1, UserId = 7, TenantId = 1 } });

            var data = new MessageNotificationData("Aviso geral");
            var userNotifications = new[] { CriarUserNotification(data) };

            // Quando
            await _sut.SendNotificationsAsync(userNotifications);

            // Então
            await _pushSender.Received(1).SendAsync(
                Arg.Any<PushSubscription>(),
                Arg.Is<PushNotificationMessage>(m => m.Title == "Aviso geral" && m.Body == "Aviso geral"));
        }

        [Fact]
        public async Task Dado_DadosDeTipoNaoSuportado_Quando_Enviar_Entao_NaoDeveConsultarRepositorio()
        {
            // Dado
            var userNotifications = new[] { CriarUserNotification(new NotificationData()) };

            // Quando
            await _sut.SendNotificationsAsync(userNotifications);

            // Então
            await _repository.DidNotReceive().GetAllListAsync(Arg.Any<Expression<Func<PushSubscription, bool>>>());
            await _pushSender.DidNotReceive().SendAsync(Arg.Any<PushSubscription>(), Arg.Any<PushNotificationMessage>());
        }

        [Fact]
        public async Task Dado_SubscriptionExpirada_Quando_EnviarFalhaComGone_Entao_DeveRemoverSubscription()
        {
            // Dado
            var subscription = new PushSubscription { Id = 9, UserId = 7, TenantId = 1 };
            _repository.GetAllListAsync(Arg.Any<Expression<Func<PushSubscription, bool>>>())
                .Returns(new List<PushSubscription> { subscription });

            var excecaoGone = new WebPush.WebPushException(
                "gone",
                new WebPush.PushSubscription("https://a", "p", "a"),
                new HttpResponseMessage(HttpStatusCode.Gone));
            _pushSender.SendAsync(Arg.Any<PushSubscription>(), Arg.Any<PushNotificationMessage>())
                .Throws(excecaoGone);

            var data = new PushNotificationData { Message = "x" };
            var userNotifications = new[] { CriarUserNotification(data) };

            // Quando
            await _sut.SendNotificationsAsync(userNotifications);

            // Então
            await _repository.Received(1).DeleteAsync(subscription);
        }

        [Fact]
        public async Task Dado_SubscriptionNaoEncontrada_Quando_EnviarFalhaComNotFound_Entao_DeveRemoverSubscription()
        {
            // Dado
            var subscription = new PushSubscription { Id = 10, UserId = 7, TenantId = 1 };
            _repository.GetAllListAsync(Arg.Any<Expression<Func<PushSubscription, bool>>>())
                .Returns(new List<PushSubscription> { subscription });

            var excecaoNotFound = new WebPush.WebPushException(
                "not found",
                new WebPush.PushSubscription("https://a", "p", "a"),
                new HttpResponseMessage(HttpStatusCode.NotFound));
            _pushSender.SendAsync(Arg.Any<PushSubscription>(), Arg.Any<PushNotificationMessage>())
                .Throws(excecaoNotFound);

            var data = new PushNotificationData { Message = "x" };
            var userNotifications = new[] { CriarUserNotification(data) };

            // Quando
            await _sut.SendNotificationsAsync(userNotifications);

            // Então
            await _repository.Received(1).DeleteAsync(subscription);
        }
    }
}
