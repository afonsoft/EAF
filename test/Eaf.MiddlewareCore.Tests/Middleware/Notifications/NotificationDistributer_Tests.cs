using Abp.Notifications;
using Abp.Runtime.Session;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Notifications
{
    public class NotificationDistributer_Tests : EafMiddlewareTestBase
    {
        private readonly FakeNotificationDistributer _fakeNotificationDistributer;
        private readonly INotificationPublisher _publisher;

        public NotificationDistributer_Tests()
        {
            _publisher = LocalIocManager.Resolve<INotificationPublisher>();
            _fakeNotificationDistributer = (FakeNotificationDistributer)LocalIocManager.Resolve<INotificationDistributer>();
        }

        [Fact]
        public async Task Should_Distribute_Notification_Using_Custom_Distributer()
        {
            //Arrange
            var notificationData = new NotificationData();

            //Act
            await _publisher.PublishAsync("TestNotification", notificationData, severity: NotificationSeverity.Success, userIds: new[] { AbpSession.ToUserIdentifier() });

            //Assert
            _fakeNotificationDistributer.IsDistributeCalled.ShouldBeTrue();
        }
    }
}