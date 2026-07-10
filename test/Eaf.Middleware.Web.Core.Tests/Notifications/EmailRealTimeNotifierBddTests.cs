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

            emailSender.Received(1).Send(
                "test@example.com",
                "You have a new notification!",
                "Hello World",
                true);
        }
    }
}
