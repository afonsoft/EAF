using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Auditing
{
    public class GetAuditLogsInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetAuditLogsInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirBrowserInfo_Entao_DeveArmazenar()
        {
            var sut = new GetAuditLogsInput();
            sut.BrowserInfo = "test_value";
            sut.BrowserInfo.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirEndDate_Entao_DeveArmazenar()
        {
            var sut = new GetAuditLogsInput();
            var dt = System.DateTime.UtcNow; sut.EndDate = dt;
            sut.EndDate.ShouldBe(dt);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirHasException_Entao_DeveArmazenar()
        {
            var sut = new GetAuditLogsInput();
            sut.HasException = true;
            sut.HasException.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirMaxExecutionDuration_Entao_DeveArmazenar()
        {
            var sut = new GetAuditLogsInput();
            sut.MaxExecutionDuration = 42;
            sut.MaxExecutionDuration.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirMethodName_Entao_DeveArmazenar()
        {
            var sut = new GetAuditLogsInput();
            sut.MethodName = "test_value";
            sut.MethodName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirMinExecutionDuration_Entao_DeveArmazenar()
        {
            var sut = new GetAuditLogsInput();
            sut.MinExecutionDuration = 42;
            sut.MinExecutionDuration.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirServiceName_Entao_DeveArmazenar()
        {
            var sut = new GetAuditLogsInput();
            sut.ServiceName = "test_value";
            sut.ServiceName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirStartDate_Entao_DeveArmazenar()
        {
            var sut = new GetAuditLogsInput();
            var dt = System.DateTime.UtcNow; sut.StartDate = dt;
            sut.StartDate.ShouldBe(dt);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserName_Entao_DeveArmazenar()
        {
            var sut = new GetAuditLogsInput();
            sut.UserName = "test_value";
            sut.UserName.ShouldBe("test_value");
        }
    }
}
