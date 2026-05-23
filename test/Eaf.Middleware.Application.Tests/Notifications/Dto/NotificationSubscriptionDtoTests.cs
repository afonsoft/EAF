using Eaf.Middleware.Notifications.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Notifications.Dto
{
    public class NotificationSubscriptionDtoTests
    {
        [Fact]
        public void Dado_NotificationSubscriptionDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new NotificationSubscriptionDto();

            dto.IsSubscribed.ShouldBeFalse();
            dto.Name.ShouldBeNull();
        }

        [Fact]
        public void Dado_NotificationSubscriptionDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new NotificationSubscriptionDto
            {
                IsSubscribed = true,
                Name = "NewUserRegistered"
            };

            dto.IsSubscribed.ShouldBeTrue();
            dto.Name.ShouldBe("NewUserRegistered");
        }

        [Fact]
        public void Dado_NotificationSubscriptionDto_Quando_Verificado_Entao_NameDeveConterRequiredAttribute()
        {
            var prop = typeof(NotificationSubscriptionDto).GetProperty(nameof(NotificationSubscriptionDto.Name));
            prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault().ShouldNotBeNull();
        }
    }
}
