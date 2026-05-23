using Abp.Notifications;
using Eaf.Middleware.Notifications.Dto;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Notifications.Dto
{
    public class NotificationsDtoCoverageTests
    {
        [Fact]
        public void GetNotificationSettingsOutput_ShouldSet()
        {
            var dto = new GetNotificationSettingsOutput
            {
                Notifications = new List<NotificationSubscriptionWithDisplayNameDto>(),
                ReceiveNotifications = true
            };
            dto.Notifications.ShouldNotBeNull();
            dto.ReceiveNotifications.ShouldBeTrue();
        }

        [Fact]
        public void GetNotificationsOutput_ShouldSet()
        {
            var list = new List<UserNotification>();
            var dto = new GetNotificationsOutput(10, 3, list);
            dto.UnreadCount.ShouldBe(3);
            dto.TotalCount.ShouldBe(10);
            dto.Items.ShouldBe(list);
        }

        [Fact]
        public void GetUserNotificationsInput_ShouldSet()
        {
            var dto = new GetUserNotificationsInput { State = UserNotificationState.Unread };
            dto.State.ShouldBe(UserNotificationState.Unread);
        }

        [Fact]
        public void NotificationSubscriptionDto_ShouldSet()
        {
            var dto = new NotificationSubscriptionDto { IsSubscribed = true, Name = "n" };
            dto.IsSubscribed.ShouldBeTrue();
            dto.Name.ShouldBe("n");
        }

        [Fact]
        public void NotificationSubscriptionWithDisplayNameDto_ShouldSet()
        {
            var dto = new NotificationSubscriptionWithDisplayNameDto
            {
                IsSubscribed = true,
                Name = "n",
                Description = "d",
                DisplayName = "dn"
            };
            dto.IsSubscribed.ShouldBeTrue();
            dto.Name.ShouldBe("n");
            dto.Description.ShouldBe("d");
            dto.DisplayName.ShouldBe("dn");
        }

        [Fact]
        public void UpdateNotificationSettingsInput_ShouldSet()
        {
            var dto = new UpdateNotificationSettingsInput
            {
                Notifications = new List<NotificationSubscriptionDto>(),
                ReceiveNotifications = true
            };
            dto.Notifications.ShouldNotBeNull();
            dto.ReceiveNotifications.ShouldBeTrue();
        }
    }
}
