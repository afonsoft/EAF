using Abp.Localization;
using Abp.Net.Mail;
using Abp.Notifications;
using Eaf.Middleware;
using Eaf.Middleware.Authorization.Users;
using Eaf.Notifications;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Notifications
{
    public class EmailRealTimeNotifierBddTests
    {
        private sealed class TestUserManager : UserManager
        {
            public TestUserManager()
                : base(
                    new UserStore(null, null, null, null, null, null, null, null, null, null),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null)
            {
            }

            public override Task<User> GetUserByIdAsync(long userId)
            {
                return Task.FromResult(new User { EmailAddress = "test@example.com" });
            }
        }

        [Fact]
        public void Dado_Tipo_Quando_VerificarInterface_Entao_DeveImplementarIRealTimeNotifier()
        {
            typeof(EmailRealTimeNotifier).GetInterface(nameof(IRealTimeNotifier)).ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_NotificacaoDeMensagem_Quando_EnviarNotificacoes_Entao_DeveEnviarEmail()
        {
            var emailSender = Substitute.For<IEmailSender>();
            var notifier = new EmailRealTimeNotifier(emailSender, new TestUserManager());

            var userNotifications = new UserNotification[]
            {
                new UserNotification
                {
                    UserId = 1,
                    Notification = new TenantNotification
                    {
                        Data = new MessageNotificationData("Hello World")
                    }
                }
            };

            await notifier.SendNotificationsAsync(userNotifications);

            await emailSender.Received(1).SendAsync(
                "test@example.com",
                "You have a new notification!",
                "Hello World",
                true);
        }

        [Fact]
        public async Task Dado_NotificacaoComDataNaoMensagem_Quando_EnviarNotificacoes_Entao_NaoDeveEnviarEmail()
        {
            var emailSender = Substitute.For<IEmailSender>();
            var notifier = new EmailRealTimeNotifier(emailSender, new TestUserManager());

            var userNotifications = new UserNotification[]
            {
                new UserNotification
                {
                    UserId = 1,
                    Notification = new TenantNotification
                    {
                        Data = new LocalizableMessageNotificationData(new LocalizableString("TestKey", "EafCore"))
                    }
                }
            };

            await notifier.SendNotificationsAsync(userNotifications);

            await emailSender.DidNotReceive().SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());
        }

        [Fact]
        public async Task Dado_NotificacaoComDataNula_Quando_EnviarNotificacoes_Entao_NaoDeveEnviarEmail()
        {
            var emailSender = Substitute.For<IEmailSender>();
            var notifier = new EmailRealTimeNotifier(emailSender, new TestUserManager());

            var userNotifications = new UserNotification[]
            {
                new UserNotification
                {
                    UserId = 1,
                    Notification = new TenantNotification()
                }
            };

            await notifier.SendNotificationsAsync(userNotifications);

            await emailSender.DidNotReceive().SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());
        }

        [Fact]
        public void Dado_EmailRealTimeNotifier_Quando_VerificarUseOnlyIfRequestedAsTarget_Entao_DeveSerFalso()
        {
            var notifier = new EmailRealTimeNotifier(Substitute.For<IEmailSender>(), new TestUserManager());
            notifier.UseOnlyIfRequestedAsTarget.ShouldBeFalse();
        }
    }
}
