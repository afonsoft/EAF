using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Auditing.Dto
{
    public class GetAuditLogsInputTests
    {
        [Fact]
        public void Dado_GetAuditLogsInput_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var input = new GetAuditLogsInput();

            input.BrowserInfo.ShouldBeNull();
            input.HasException.ShouldBeNull();
            input.MaxExecutionDuration.ShouldBeNull();
            input.MethodName.ShouldBeNull();
            input.MinExecutionDuration.ShouldBeNull();
            input.ServiceName.ShouldBeNull();
            input.UserName.ShouldBeNull();
        }

        [Fact]
        public void Dado_GetAuditLogsInput_Quando_SortingNulo_Entao_NormalizeDeveDefinirPadrao()
        {
            var input = new GetAuditLogsInput();
            input.Normalize();
            input.Sorting.ShouldBe("AuditLog.ExecutionTime DESC");
        }

        [Fact]
        public void Dado_GetAuditLogsInput_Quando_SortingComUserName_Entao_NormalizeDeveAdicionarPrefixoUser()
        {
            var input = new GetAuditLogsInput { Sorting = "UserName ASC" };
            input.Normalize();
            input.Sorting.ShouldBe("User.UserName ASC");
        }

        [Fact]
        public void Dado_GetAuditLogsInput_Quando_SortingComOutroCampo_Entao_NormalizeDeveAdicionarPrefixoAuditLog()
        {
            var input = new GetAuditLogsInput { Sorting = "ServiceName DESC" };
            input.Normalize();
            input.Sorting.ShouldBe("AuditLog.ServiceName DESC");
        }
    }
}
