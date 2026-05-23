using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Auditing.Dto
{
    public class AuditLogListDtoTests
    {
        [Fact]
        public void Dado_AuditLogListDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new AuditLogListDto();

            dto.BrowserInfo.ShouldBeNull();
            dto.ClientIpAddress.ShouldBeNull();
            dto.ClientName.ShouldBeNull();
            dto.CustomData.ShouldBeNull();
            dto.Exception.ShouldBeNull();
            dto.ExecutionDuration.ShouldBe(0);
            dto.ExecutionTime.ShouldBe(default(DateTime));
            dto.ImpersonatorTenantId.ShouldBeNull();
            dto.ImpersonatorUserId.ShouldBeNull();
            dto.MethodName.ShouldBeNull();
            dto.Parameters.ShouldBeNull();
            dto.ServiceName.ShouldBeNull();
            dto.UserId.ShouldBeNull();
            dto.UserName.ShouldBeNull();
        }

        [Fact]
        public void Dado_AuditLogListDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var now = DateTime.UtcNow;
            var dto = new AuditLogListDto
            {
                BrowserInfo = "Chrome",
                ClientIpAddress = "192.168.1.1",
                ClientName = "TestClient",
                CustomData = "custom",
                Exception = "error",
                ExecutionDuration = 150,
                ExecutionTime = now,
                ImpersonatorTenantId = 1,
                ImpersonatorUserId = 2L,
                MethodName = "GetAll",
                Parameters = "{}",
                ServiceName = "UserAppService",
                UserId = 10L,
                UserName = "admin"
            };

            dto.BrowserInfo.ShouldBe("Chrome");
            dto.ClientIpAddress.ShouldBe("192.168.1.1");
            dto.ClientName.ShouldBe("TestClient");
            dto.CustomData.ShouldBe("custom");
            dto.Exception.ShouldBe("error");
            dto.ExecutionDuration.ShouldBe(150);
            dto.ExecutionTime.ShouldBe(now);
            dto.ImpersonatorTenantId.ShouldBe(1);
            dto.ImpersonatorUserId.ShouldBe(2L);
            dto.MethodName.ShouldBe("GetAll");
            dto.Parameters.ShouldBe("{}");
            dto.ServiceName.ShouldBe("UserAppService");
            dto.UserId.ShouldBe(10L);
            dto.UserName.ShouldBe("admin");
        }
    }
}
