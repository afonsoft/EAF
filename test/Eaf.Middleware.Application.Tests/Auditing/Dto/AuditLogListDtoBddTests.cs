using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Auditing
{
    public class AuditLogListDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new AuditLogListDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirBrowserInfo_Entao_DeveArmazenar()
        {
            var sut = new AuditLogListDto();
            sut.BrowserInfo = "test_value";
            sut.BrowserInfo.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirClientIpAddress_Entao_DeveArmazenar()
        {
            var sut = new AuditLogListDto();
            sut.ClientIpAddress = "test_value";
            sut.ClientIpAddress.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirClientName_Entao_DeveArmazenar()
        {
            var sut = new AuditLogListDto();
            sut.ClientName = "test_value";
            sut.ClientName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirCustomData_Entao_DeveArmazenar()
        {
            var sut = new AuditLogListDto();
            sut.CustomData = "test_value";
            sut.CustomData.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirException_Entao_DeveArmazenar()
        {
            var sut = new AuditLogListDto();
            sut.Exception = "test_value";
            sut.Exception.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirExecutionDuration_Entao_DeveArmazenar()
        {
            var sut = new AuditLogListDto();
            sut.ExecutionDuration = 42;
            sut.ExecutionDuration.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirExecutionTime_Entao_DeveArmazenar()
        {
            var sut = new AuditLogListDto();
            var dt = System.DateTime.UtcNow; sut.ExecutionTime = dt;
            sut.ExecutionTime.ShouldBe(dt);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirImpersonatorTenantId_Entao_DeveArmazenar()
        {
            var sut = new AuditLogListDto();
            sut.ImpersonatorTenantId = 42;
            sut.ImpersonatorTenantId.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirImpersonatorUserId_Entao_DeveArmazenar()
        {
            var sut = new AuditLogListDto();
            sut.ImpersonatorUserId = 100L;
            sut.ImpersonatorUserId.ShouldBe(100L);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirMethodName_Entao_DeveArmazenar()
        {
            var sut = new AuditLogListDto();
            sut.MethodName = "test_value";
            sut.MethodName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirParameters_Entao_DeveArmazenar()
        {
            var sut = new AuditLogListDto();
            sut.Parameters = "test_value";
            sut.Parameters.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirServiceName_Entao_DeveArmazenar()
        {
            var sut = new AuditLogListDto();
            sut.ServiceName = "test_value";
            sut.ServiceName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserId_Entao_DeveArmazenar()
        {
            var sut = new AuditLogListDto();
            sut.UserId = 100L;
            sut.UserId.ShouldBe(100L);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserName_Entao_DeveArmazenar()
        {
            var sut = new AuditLogListDto();
            sut.UserName = "test_value";
            sut.UserName.ShouldBe("test_value");
        }
    }
}
