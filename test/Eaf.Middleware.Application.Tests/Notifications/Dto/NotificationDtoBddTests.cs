using Abp.Notifications;
using Eaf.Middleware.Notifications.Dto;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Notifications.Dto
{
    public class NotificationDtoBddTests
    {
        [Fact]
        public void Dado_NotificationSubscriptionDto_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var dto = new NotificationSubscriptionDto
            {
                Name = "NewUserRegistered",
                IsSubscribed = true
            };

            dto.Name.ShouldBe("NewUserRegistered");
            dto.IsSubscribed.ShouldBeTrue();
        }

        [Fact]
        public void Dado_NotificationSubscriptionWithDisplayNameDto_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var dto = new NotificationSubscriptionWithDisplayNameDto
            {
                Name = "NewUserRegistered",
                DisplayName = "Novo Usuário Registrado",
                Description = "Notifica quando um novo usuário se registra",
                IsSubscribed = false
            };

            dto.Name.ShouldBe("NewUserRegistered");
            dto.DisplayName.ShouldBe("Novo Usuário Registrado");
            dto.Description.ShouldBe("Notifica quando um novo usuário se registra");
            dto.IsSubscribed.ShouldBeFalse();
        }

        [Fact]
        public void Dado_GetNotificationSettingsOutput_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var output = new GetNotificationSettingsOutput
            {
                ReceiveNotifications = true,
                Notifications = new List<NotificationSubscriptionWithDisplayNameDto>
                {
                    new NotificationSubscriptionWithDisplayNameDto { Name = "Test", IsSubscribed = true }
                }
            };

            output.ReceiveNotifications.ShouldBeTrue();
            output.Notifications.Count.ShouldBe(1);
        }

        [Fact]
        public void Dado_UpdateNotificationSettingsInput_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var input = new UpdateNotificationSettingsInput
            {
                ReceiveNotifications = false,
                Notifications = new List<NotificationSubscriptionDto>
                {
                    new NotificationSubscriptionDto { Name = "Test", IsSubscribed = false }
                }
            };

            input.ReceiveNotifications.ShouldBeFalse();
            input.Notifications.Count.ShouldBe(1);
        }

        [Fact]
        public void Dado_GetNotificationsOutput_Quando_CriarComParametros_Entao_DeveDefinirPropriedades()
        {
            var notifications = new List<UserNotification>();
            var output = new GetNotificationsOutput(100, 5, notifications);

            output.TotalCount.ShouldBe(100);
            output.UnreadCount.ShouldBe(5);
            output.Items.ShouldBe(notifications);
        }

        [Fact]
        public void Dado_GetUserNotificationsInput_Quando_CriarNovo_Entao_StateDeveSerNull()
        {
            var input = new GetUserNotificationsInput();

            input.State.ShouldBeNull();
        }

        [Fact]
        public void Dado_GetUserNotificationsInput_Quando_DefinirState_Entao_DevePersistir()
        {
            var input = new GetUserNotificationsInput { State = UserNotificationState.Unread };

            input.State.ShouldBe(UserNotificationState.Unread);
        }
    }
}
