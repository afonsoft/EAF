using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Auditing.Dto
{
    /// <summary>
    /// Testes BDD estendidos para DTOs de Auditoria seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AuditLogDtoBddExtendedTests
    {
        #region AuditLogListDto

        [Fact]
        public void Dado_AuditLogListDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new AuditLogListDto
            {
                Id = 1,
                BrowserInfo = "Chrome 100",
                ClientIpAddress = "192.168.1.1",
                ClientName = "WorkStation-01",
                CustomData = "{\"key\":\"value\"}",
                Exception = null,
                ExecutionDuration = 250,
                ExecutionTime = new DateTime(2026, 6, 13, 12, 0, 0),
                ImpersonatorTenantId = 2,
                ImpersonatorUserId = 10,
                MethodName = "GetAll",
                Parameters = "{\"filter\":\"\"}",
                ServiceName = "UserAppService",
                UserId = 42,
                UserName = "admin"
            };

            dto.Id.ShouldBe(1);
            dto.BrowserInfo.ShouldBe("Chrome 100");
            dto.ClientIpAddress.ShouldBe("192.168.1.1");
            dto.ClientName.ShouldBe("WorkStation-01");
            dto.CustomData.ShouldNotBeNull();
            dto.Exception.ShouldBeNull();
            dto.ExecutionDuration.ShouldBe(250);
            dto.ImpersonatorTenantId.ShouldBe(2);
            dto.ImpersonatorUserId.ShouldBe(10);
            dto.MethodName.ShouldBe("GetAll");
            dto.ServiceName.ShouldBe("UserAppService");
            dto.UserId.ShouldBe(42);
            dto.UserName.ShouldBe("admin");
        }

        #endregion

        #region GetAuditLogsInput - Normalize

        [Fact]
        public void Dado_GetAuditLogsInput_SemSorting_Quando_Normalize_Entao_DeveDefinirPadrao()
        {
            var input = new GetAuditLogsInput
            {
                StartDate = DateTime.UtcNow.AddDays(-7),
                EndDate = DateTime.UtcNow
            };

            input.Normalize();
            input.Sorting.ShouldBe("AuditLog.ExecutionTime DESC");
        }

        [Fact]
        public void Dado_GetAuditLogsInput_ComSortingUserName_Quando_Normalize_Entao_DevePrefixarUser()
        {
            var input = new GetAuditLogsInput
            {
                Sorting = "UserName ASC",
                StartDate = DateTime.UtcNow.AddDays(-7),
                EndDate = DateTime.UtcNow
            };

            input.Normalize();
            input.Sorting.ShouldBe("User.UserName ASC");
        }

        [Fact]
        public void Dado_GetAuditLogsInput_ComSortingOutroCampo_Quando_Normalize_Entao_DevePrefixarAuditLog()
        {
            var input = new GetAuditLogsInput
            {
                Sorting = "ExecutionDuration DESC",
                StartDate = DateTime.UtcNow.AddDays(-7),
                EndDate = DateTime.UtcNow
            };

            input.Normalize();
            input.Sorting.ShouldBe("AuditLog.ExecutionDuration DESC");
        }

        [Fact]
        public void Dado_GetAuditLogsInput_Quando_DefinirFiltros_Entao_DeveArmazenar()
        {
            var input = new GetAuditLogsInput
            {
                HasException = true,
                MinExecutionDuration = 100,
                MaxExecutionDuration = 5000,
                ServiceName = "UserAppService",
                MethodName = "GetAll",
                BrowserInfo = "Chrome",
                UserName = "admin"
            };

            input.HasException.ShouldBe(true);
            input.MinExecutionDuration.ShouldBe(100);
            input.MaxExecutionDuration.ShouldBe(5000);
            input.ServiceName.ShouldBe("UserAppService");
            input.MethodName.ShouldBe("GetAll");
            input.BrowserInfo.ShouldBe("Chrome");
            input.UserName.ShouldBe("admin");
        }

        #endregion

        #region EntityChangeDto

        [Fact]
        public void Dado_EntityChangeDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new EntityChangeDto
            {
                Id = 100,
                ChangeTime = new DateTime(2026, 1, 1),
                ChangeType = Abp.Events.Bus.Entities.EntityChangeType.Created,
                EntityChangeSetId = 50,
                EntityId = "42",
                EntityTypeFullName = "Eaf.Middleware.Core.User",
                TenantId = 1
            };

            dto.Id.ShouldBe(100);
            dto.ChangeType.ShouldBe(Abp.Events.Bus.Entities.EntityChangeType.Created);
            dto.EntityId.ShouldBe("42");
            dto.EntityTypeFullName.ShouldBe("Eaf.Middleware.Core.User");
            dto.TenantId.ShouldBe(1);
        }

        #endregion

        #region EntityChangeListDto

        [Fact]
        public void Dado_EntityChangeListDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new EntityChangeListDto
            {
                Id = 200,
                ChangeTime = DateTime.UtcNow,
                ChangeType = Abp.Events.Bus.Entities.EntityChangeType.Updated,
                EntityChangeSetId = 50,
                EntityTypeFullName = "Eaf.Role",
                UserId = 10,
                UserName = "admin"
            };

            dto.Id.ShouldBe(200);
            dto.ChangeType.ShouldBe(Abp.Events.Bus.Entities.EntityChangeType.Updated);
            dto.EntityTypeFullName.ShouldBe("Eaf.Role");
            dto.ChangeTypeName.ShouldBe("Updated");
            dto.UserId.ShouldBe(10);
            dto.UserName.ShouldBe("admin");
        }

        #endregion

        #region EntityPropertyChangeDto

        [Fact]
        public void Dado_EntityPropertyChangeDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new EntityPropertyChangeDto
            {
                Id = 300,
                EntityChangeId = 200,
                NewValue = "new-value",
                OriginalValue = "old-value",
                PropertyName = "Name",
                PropertyTypeFullName = "System.String"
            };

            dto.Id.ShouldBe(300);
            dto.EntityChangeId.ShouldBe(200);
            dto.NewValue.ShouldBe("new-value");
            dto.OriginalValue.ShouldBe("old-value");
            dto.PropertyName.ShouldBe("Name");
            dto.PropertyTypeFullName.ShouldBe("System.String");
        }

        #endregion

        #region GetEntityChangeInput

        [Fact]
        public void Dado_GetEntityChangeInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new GetEntityChangeInput
            {
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow,
                EntityTypeFullName = "Eaf.User"
            };

            input.EntityTypeFullName.ShouldBe("Eaf.User");
        }

        #endregion
    }
}
