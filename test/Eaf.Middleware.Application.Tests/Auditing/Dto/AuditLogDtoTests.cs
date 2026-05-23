using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Auditing.Dto
{
    public class AuditLogDtoTests
    {
        [Fact]
        public void Dado_AuditLogListDto_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var now = DateTime.UtcNow;
            var dto = new AuditLogListDto
            {
                Id = 1,
                BrowserInfo = "Chrome 120",
                ClientIpAddress = "192.168.1.1",
                ClientName = "WebApp",
                CustomData = "custom-data",
                Exception = null,
                ExecutionDuration = 250,
                ExecutionTime = now,
                ImpersonatorTenantId = 1,
                ImpersonatorUserId = 10,
                MethodName = "GetUsers",
                Parameters = "{\"page\":1}",
                ServiceName = "UserAppService",
                UserId = 42,
                UserName = "admin"
            };

            dto.Id.ShouldBe(1);
            dto.BrowserInfo.ShouldBe("Chrome 120");
            dto.ClientIpAddress.ShouldBe("192.168.1.1");
            dto.ClientName.ShouldBe("WebApp");
            dto.CustomData.ShouldBe("custom-data");
            dto.Exception.ShouldBeNull();
            dto.ExecutionDuration.ShouldBe(250);
            dto.ExecutionTime.ShouldBe(now);
            dto.ImpersonatorTenantId.ShouldBe(1);
            dto.ImpersonatorUserId.ShouldBe(10);
            dto.MethodName.ShouldBe("GetUsers");
            dto.Parameters.ShouldBe("{\"page\":1}");
            dto.ServiceName.ShouldBe("UserAppService");
            dto.UserId.ShouldBe(42);
            dto.UserName.ShouldBe("admin");
        }

        [Fact]
        public void Dado_AuditLogListDto_Quando_PropriedadesNull_Entao_DeveRetornarNull()
        {
            var dto = new AuditLogListDto();
            dto.UserId.ShouldBeNull();
            dto.ImpersonatorUserId.ShouldBeNull();
            dto.ImpersonatorTenantId.ShouldBeNull();
        }
    }
}
