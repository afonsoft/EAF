using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    public class UserLoginAttemptDtoTests
    {
        [Fact]
        public void Dado_UserLoginAttemptDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new UserLoginAttemptDto();

            dto.BrowserInfo.ShouldBeNull();
            dto.ClientIpAddress.ShouldBeNull();
            dto.ClientName.ShouldBeNull();
            dto.CreationTime.ShouldBe(default(DateTime));
            dto.Result.ShouldBeNull();
            dto.TenancyName.ShouldBeNull();
            dto.UserNameOrEmail.ShouldBeNull();
        }

        [Fact]
        public void Dado_UserLoginAttemptDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var now = DateTime.UtcNow;
            var dto = new UserLoginAttemptDto
            {
                BrowserInfo = "Chrome 120",
                ClientIpAddress = "10.0.0.1",
                ClientName = "WebApp",
                CreationTime = now,
                Result = "Success",
                TenancyName = "Default",
                UserNameOrEmail = "admin@test.com"
            };

            dto.BrowserInfo.ShouldBe("Chrome 120");
            dto.ClientIpAddress.ShouldBe("10.0.0.1");
            dto.ClientName.ShouldBe("WebApp");
            dto.CreationTime.ShouldBe(now);
            dto.Result.ShouldBe("Success");
            dto.TenancyName.ShouldBe("Default");
            dto.UserNameOrEmail.ShouldBe("admin@test.com");
        }
    }
}
