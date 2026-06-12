using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Auditing.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de Auditoria seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AuditingDtoBddTests
    {
        #region GetAuditLogsInput

        [Fact]
        public void Dado_GetAuditLogsInput_Quando_NormalizeSemSorting_Entao_DeveDefinirExecutionTimeDESC()
        {
            var input = new GetAuditLogsInput();
            input.Normalize();
            input.Sorting.ShouldBe("AuditLog.ExecutionTime DESC");
        }

        [Fact]
        public void Dado_GetAuditLogsInput_Quando_NormalizeComUserNameSorting_Entao_DevePrefixarComUser()
        {
            var input = new GetAuditLogsInput { Sorting = "UserName ASC" };
            input.Normalize();
            input.Sorting.ShouldBe("User.UserName ASC");
        }

        [Fact]
        public void Dado_GetAuditLogsInput_Quando_NormalizeComServiceNameSorting_Entao_DevePrefixarComAuditLog()
        {
            var input = new GetAuditLogsInput { Sorting = "ServiceName" };
            input.Normalize();
            input.Sorting.ShouldBe("AuditLog.ServiceName");
        }

        [Fact]
        public void Dado_GetAuditLogsInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var start = new DateTime(2026, 1, 1);
            var end = new DateTime(2026, 12, 31);
            var input = new GetAuditLogsInput
            {
                StartDate = start,
                EndDate = end,
                UserName = "admin",
                ServiceName = "UserAppService",
                MethodName = "GetAll",
                BrowserInfo = "Chrome",
                HasException = false,
                MinExecutionDuration = 100,
                MaxExecutionDuration = 5000
            };

            input.StartDate.ShouldBe(start);
            input.EndDate.ShouldBe(end);
            input.UserName.ShouldBe("admin");
            input.ServiceName.ShouldBe("UserAppService");
            input.MethodName.ShouldBe("GetAll");
            input.BrowserInfo.ShouldBe("Chrome");
            input.HasException.ShouldBe(false);
            input.MinExecutionDuration.ShouldBe(100);
            input.MaxExecutionDuration.ShouldBe(5000);
        }

        #endregion

        #region GetEntityChangeInput

        [Fact]
        public void Dado_GetEntityChangeInput_Quando_NormalizeSemSorting_Entao_DeveDefinirChangeTimeDESC()
        {
            var input = new GetEntityChangeInput();
            input.Normalize();
            input.Sorting.ShouldBe("EntityChange.ChangeTime DESC");
        }

        [Fact]
        public void Dado_GetEntityChangeInput_Quando_NormalizeComUserNameSorting_Entao_DevePrefixarComUser()
        {
            var input = new GetEntityChangeInput { Sorting = "UserName" };
            input.Normalize();
            input.Sorting.ShouldBe("User.UserName");
        }

        [Fact]
        public void Dado_GetEntityChangeInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new GetEntityChangeInput
            {
                EntityTypeFullName = "Eaf.Middleware.Authorization.Users.User",
                UserName = "admin",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 6, 1)
            };

            input.EntityTypeFullName.ShouldBe("Eaf.Middleware.Authorization.Users.User");
            input.UserName.ShouldBe("admin");
        }

        #endregion

        #region AuditLogListDto

        [Fact]
        public void Dado_AuditLogListDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new AuditLogListDto
            {
                Id = 1,
                UserId = 100,
                UserName = "admin",
                ServiceName = "RoleAppService",
                MethodName = "CreateRole",
                Parameters = "{}",
                ExecutionTime = new DateTime(2026, 6, 1, 10, 0, 0),
                ExecutionDuration = 250,
                ClientIpAddress = "192.168.1.1",
                ClientName = "Web",
                BrowserInfo = "Chrome 120",
                Exception = null,
                CustomData = "custom",
                ImpersonatorUserId = 1,
                ImpersonatorTenantId = null
            };

            dto.Id.ShouldBe(1);
            dto.UserId.ShouldBe(100);
            dto.UserName.ShouldBe("admin");
            dto.ServiceName.ShouldBe("RoleAppService");
            dto.MethodName.ShouldBe("CreateRole");
            dto.Parameters.ShouldBe("{}");
            dto.ExecutionDuration.ShouldBe(250);
            dto.ClientIpAddress.ShouldBe("192.168.1.1");
            dto.ClientName.ShouldBe("Web");
            dto.BrowserInfo.ShouldBe("Chrome 120");
            dto.Exception.ShouldBeNull();
            dto.CustomData.ShouldBe("custom");
            dto.ImpersonatorUserId.ShouldBe(1);
            dto.ImpersonatorTenantId.ShouldBeNull();
        }

        #endregion
    }
}
