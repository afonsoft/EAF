using Eaf.Middleware.Sessions.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Sessions.Dto
{
    public class TenantLoginInfoDtoTests
    {
        [Fact]
        public void Dado_TenantLoginInfoDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new TenantLoginInfoDto();

            dto.CreationTime.ShouldBe(default(DateTime));
            dto.Name.ShouldBeNull();
            dto.TenancyName.ShouldBeNull();
        }

        [Fact]
        public void Dado_TenantLoginInfoDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var now = DateTime.UtcNow;
            var dto = new TenantLoginInfoDto
            {
                CreationTime = now,
                Name = "Default Tenant",
                TenancyName = "Default"
            };

            dto.CreationTime.ShouldBe(now);
            dto.Name.ShouldBe("Default Tenant");
            dto.TenancyName.ShouldBe("Default");
        }
    }
}
