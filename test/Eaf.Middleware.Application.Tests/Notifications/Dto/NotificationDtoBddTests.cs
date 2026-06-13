using Eaf.Middleware.Notifications.Dto;
using Eaf.Middleware.Logging.Dto;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Notifications.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de Notifications e Logging seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class NotificationDtoBddTests
    {
        #region NotificationSubscriptionDto

        [Fact]
        public void Dado_NotificationSubscriptionDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new NotificationSubscriptionDto
            {
                Name = "App.NewUserRegistered",
                IsSubscribed = true
            };

            dto.Name.ShouldBe("App.NewUserRegistered");
            dto.IsSubscribed.ShouldBeTrue();
        }

        #endregion

        #region NotificationSubscriptionWithDisplayNameDto

        [Fact]
        public void Dado_NotificationSubscriptionWithDisplayNameDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new NotificationSubscriptionWithDisplayNameDto
            {
                Name = "App.NewUserRegistered",
                IsSubscribed = true,
                DisplayName = "Novo Usuário Registrado",
                Description = "Notificação quando um novo usuário se registra"
            };

            dto.DisplayName.ShouldBe("Novo Usuário Registrado");
            dto.Description.ShouldBe("Notificação quando um novo usuário se registra");
            dto.Name.ShouldBe("App.NewUserRegistered");
            dto.IsSubscribed.ShouldBeTrue();
        }

        #endregion

        #region GetNotificationSettingsOutput

        [Fact]
        public void Dado_GetNotificationSettingsOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
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

        #endregion

        #region GetNotificationsOutput

        [Fact]
        public void Dado_GetNotificationsOutput_Quando_CriarComParametros_Entao_DeveDefinir()
        {
            var output = new GetNotificationsOutput(10, 3, new List<Abp.Notifications.UserNotification>());
            output.TotalCount.ShouldBe(10);
            output.UnreadCount.ShouldBe(3);
            output.Items.ShouldNotBeNull();
        }

        #endregion

        #region GetUserNotificationsInput

        [Fact]
        public void Dado_GetUserNotificationsInput_Quando_DefinirState_Entao_DeveArmazenar()
        {
            var input = new GetUserNotificationsInput
            {
                State = Abp.Notifications.UserNotificationState.Unread
            };

            input.State.ShouldBe(Abp.Notifications.UserNotificationState.Unread);
        }

        [Fact]
        public void Dado_GetUserNotificationsInput_SemState_Quando_Verificar_Entao_DeveSerNull()
        {
            var input = new GetUserNotificationsInput();
            input.State.ShouldBeNull();
        }

        #endregion

        #region UpdateNotificationSettingsInput

        [Fact]
        public void Dado_UpdateNotificationSettingsInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
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

        #endregion

        #region GetLatestWebLogsOutput

        [Fact]
        public void Dado_GetLatestWebLogsOutput_Quando_DefinirLogs_Entao_DeveArmazenar()
        {
            var output = new GetLatestWebLogsOutput
            {
                LatestWebLogLines = new List<string>
                {
                    "2026-06-13 INFO Application started",
                    "2026-06-13 ERROR NullReferenceException"
                }
            };

            output.LatestWebLogLines.Count.ShouldBe(2);
        }

        #endregion
    }
}
