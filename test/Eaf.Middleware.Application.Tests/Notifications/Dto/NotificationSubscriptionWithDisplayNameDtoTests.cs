using Eaf.Middleware.Notifications.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Notifications.Dto
{
    public class NotificationSubscriptionWithDisplayNameDtoTests
    {
        [Fact]
        public void Dado_NotificationSubscriptionWithDisplayNameDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new NotificationSubscriptionWithDisplayNameDto();

            dto.Description.ShouldBeNull();
            dto.DisplayName.ShouldBeNull();
            dto.IsSubscribed.ShouldBeFalse();
            dto.Name.ShouldBeNull();
        }

        [Fact]
        public void Dado_NotificationSubscriptionWithDisplayNameDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new NotificationSubscriptionWithDisplayNameDto
            {
                Description = "Notificação de novo usuário",
                DisplayName = "Novo Usuário",
                IsSubscribed = true,
                Name = "NewUser"
            };

            dto.Description.ShouldBe("Notificação de novo usuário");
            dto.DisplayName.ShouldBe("Novo Usuário");
            dto.IsSubscribed.ShouldBeTrue();
            dto.Name.ShouldBe("NewUser");
        }

        [Fact]
        public void Dado_NotificationSubscriptionWithDisplayNameDto_Quando_Verificado_Entao_DeveHerdarNotificationSubscriptionDto()
        {
            var dto = new NotificationSubscriptionWithDisplayNameDto();
            dto.ShouldBeAssignableTo<NotificationSubscriptionDto>();
        }
    }
}
