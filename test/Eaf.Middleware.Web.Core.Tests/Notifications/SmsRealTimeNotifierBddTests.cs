using Abp.Notifications;
using Eaf.Notifications;
using Eaf.Notifications.Sms;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Notifications
{
    /// <summary>
    /// Testes BDD para SmsRealTimeNotifier seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class SmsRealTimeNotifierBddTests
    {
        private readonly ISmsSender _smsSender;
        private readonly SmsRealTimeNotifier _sut;

        public SmsRealTimeNotifierBddTests()
        {
            _smsSender = Substitute.For<ISmsSender>();
            _sut = new SmsRealTimeNotifier(_smsSender);
        }

        private static UserNotification CriarUserNotification(NotificationData data)
        {
            return new UserNotification
            {
                UserId = 7,
                TenantId = 1,
                Notification = new TenantNotification { Data = data },
            };
        }

        [Fact]
        public void Dado_Notificador_Quando_VerificarUseOnlyIfRequestedAsTarget_Entao_DeveSerFalso()
        {
            _sut.UseOnlyIfRequestedAsTarget.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_NotificacaoSmsComTelefone_Quando_Enviar_Entao_DeveEnviarSms()
        {
            // Dado
            var data = new SmsNotificationData
            {
                Message = "Código 1234",
                PhoneNumber = "+5511999999999",
                From = "EAF",
            };
            var userNotifications = new[] { CriarUserNotification(data) };

            // Quando
            await _sut.SendNotificationsAsync(userNotifications);

            // Então
            await _smsSender.Received(1).SendAsync(Arg.Is<SmsMessage>(m =>
                m.PhoneNumber == "+5511999999999" &&
                m.Body == "Código 1234" &&
                m.From == "EAF"));
        }

        [Fact]
        public async Task Dado_NotificacaoSmsSemTelefone_Quando_Enviar_Entao_NaoDeveEnviar()
        {
            // Dado
            var data = new SmsNotificationData { Message = "sem telefone", PhoneNumber = null };
            var userNotifications = new[] { CriarUserNotification(data) };

            // Quando
            await _sut.SendNotificationsAsync(userNotifications);

            // Então
            await _smsSender.DidNotReceive().SendAsync(Arg.Any<SmsMessage>());
        }

        [Fact]
        public async Task Dado_NotificacaoDeOutroTipo_Quando_Enviar_Entao_NaoDeveEnviar()
        {
            // Dado
            var userNotifications = new[] { CriarUserNotification(new NotificationData()) };

            // Quando
            await _sut.SendNotificationsAsync(userNotifications);

            // Então
            await _smsSender.DidNotReceive().SendAsync(Arg.Any<SmsMessage>());
        }
    }
}
