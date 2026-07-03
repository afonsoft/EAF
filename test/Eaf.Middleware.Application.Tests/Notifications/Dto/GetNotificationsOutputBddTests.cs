using Eaf.Middleware.Notifications.Dto;
using Shouldly;
using System.Collections.Generic;
using Abp.Notifications;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Notifications
{
    public class GetNotificationsOutputBddTests
    {
        [Fact]
        public void Dado_Parametros_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var notifications = new List<UserNotification>();
            var sut = new GetNotificationsOutput(10, 3, notifications);
            sut.ShouldNotBeNull();
            sut.UnreadCount.ShouldBe(3);
            sut.TotalCount.ShouldBe(10);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUnreadCount_Entao_DeveArmazenar()
        {
            var sut = new GetNotificationsOutput(0, 0, new List<UserNotification>());
            sut.UnreadCount = 5;
            sut.UnreadCount.ShouldBe(5);
        }
    }
}
