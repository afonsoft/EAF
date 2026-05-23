using Abp.Events.Bus.Entities;
using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Auditing.Dto
{
    public class AuditingDtoCoverageTests
    {
        [Fact]
        public void AuditLogListDto_ShouldSetAll()
        {
            var now = DateTime.UtcNow;
            var dto = new AuditLogListDto
            {
                Id = 1,
                BrowserInfo = "b",
                ClientIpAddress = "ip",
                ClientName = "c",
                CustomData = "cd",
                Exception = "ex",
                ExecutionDuration = 123,
                ExecutionTime = now,
                ImpersonatorTenantId = 2,
                ImpersonatorUserId = 3,
                MethodName = "m",
                Parameters = "p",
                ServiceName = "svc",
                UserId = 4,
                UserName = "u"
            };
            dto.Id.ShouldBe(1);
            dto.BrowserInfo.ShouldBe("b");
            dto.ClientIpAddress.ShouldBe("ip");
            dto.ClientName.ShouldBe("c");
            dto.CustomData.ShouldBe("cd");
            dto.Exception.ShouldBe("ex");
            dto.ExecutionDuration.ShouldBe(123);
            dto.ExecutionTime.ShouldBe(now);
            dto.ImpersonatorTenantId.ShouldBe(2);
            dto.ImpersonatorUserId.ShouldBe(3);
            dto.MethodName.ShouldBe("m");
            dto.Parameters.ShouldBe("p");
            dto.ServiceName.ShouldBe("svc");
            dto.UserId.ShouldBe(4);
            dto.UserName.ShouldBe("u");
        }

        [Fact]
        public void EntityChangeDto_ShouldSetAll()
        {
            var now = DateTime.UtcNow;
            var dto = new EntityChangeDto
            {
                Id = 1,
                ChangeTime = now,
                ChangeType = EntityChangeType.Updated,
                EntityChangeSetId = 10,
                EntityEntry = new { x = 1 },
                EntityId = "42",
                EntityTypeFullName = "T",
                TenantId = 5
            };
            dto.ChangeTime.ShouldBe(now);
            dto.ChangeType.ShouldBe(EntityChangeType.Updated);
            dto.EntityChangeSetId.ShouldBe(10);
            dto.EntityEntry.ShouldNotBeNull();
            dto.EntityId.ShouldBe("42");
            dto.EntityTypeFullName.ShouldBe("T");
            dto.TenantId.ShouldBe(5);
        }

        [Fact]
        public void EntityChangeListDto_ChangeTypeName_ShouldReflectEnum()
        {
            var dto = new EntityChangeListDto { ChangeType = EntityChangeType.Created };
            dto.ChangeTypeName.ShouldBe(EntityChangeType.Created.ToString());
        }

        [Fact]
        public void EntityChangeListDto_ShouldSetAll()
        {
            var now = DateTime.UtcNow;
            var dto = new EntityChangeListDto
            {
                Id = 1,
                ChangeTime = now,
                ChangeType = EntityChangeType.Deleted,
                EntityChangeSetId = 2,
                EntityTypeFullName = "T",
                UserId = 3,
                UserName = "u"
            };
            dto.ChangeTime.ShouldBe(now);
            dto.ChangeType.ShouldBe(EntityChangeType.Deleted);
            dto.EntityChangeSetId.ShouldBe(2);
            dto.EntityTypeFullName.ShouldBe("T");
            dto.UserId.ShouldBe(3);
            dto.UserName.ShouldBe("u");
        }

        [Fact]
        public void EntityPropertyChangeDto_ShouldSetAll()
        {
            var dto = new EntityPropertyChangeDto
            {
                Id = 1,
                EntityChangeId = 2,
                NewValue = "new",
                OriginalValue = "old",
                PropertyName = "pn",
                PropertyTypeFullName = "pt",
                TenantId = 3
            };
            dto.EntityChangeId.ShouldBe(2);
            dto.NewValue.ShouldBe("new");
            dto.OriginalValue.ShouldBe("old");
            dto.PropertyName.ShouldBe("pn");
            dto.PropertyTypeFullName.ShouldBe("pt");
            dto.TenantId.ShouldBe(3);
        }

        [Fact]
        public void GetAuditLogsInput_Normalize_Defaults()
        {
            var dto = new GetAuditLogsInput();
            dto.Normalize();
            dto.Sorting.ShouldBe("AuditLog.ExecutionTime DESC");
        }

        [Fact]
        public void GetAuditLogsInput_Normalize_WithUserNameSorting_PrefixesUser()
        {
            var dto = new GetAuditLogsInput { Sorting = "UserName ASC" };
            dto.Normalize();
            dto.Sorting.ShouldBe("User.UserName ASC");
        }

        [Fact]
        public void GetAuditLogsInput_Normalize_WithOtherSorting_PrefixesAuditLog()
        {
            var dto = new GetAuditLogsInput { Sorting = "Method" };
            dto.Normalize();
            dto.Sorting.ShouldBe("AuditLog.Method");
        }

        [Fact]
        public void GetAuditLogsInput_ShouldSetAllProperties()
        {
            var start = DateTime.UtcNow.AddDays(-1);
            var end = DateTime.UtcNow;
            var dto = new GetAuditLogsInput
            {
                BrowserInfo = "b",
                EndDate = end,
                HasException = true,
                MaxExecutionDuration = 500,
                MethodName = "m",
                MinExecutionDuration = 1,
                ServiceName = "s",
                StartDate = start,
                UserName = "u"
            };
            dto.BrowserInfo.ShouldBe("b");
            dto.EndDate.ShouldBe(end);
            dto.HasException.ShouldBe(true);
            dto.MaxExecutionDuration.ShouldBe(500);
            dto.MethodName.ShouldBe("m");
            dto.MinExecutionDuration.ShouldBe(1);
            dto.ServiceName.ShouldBe("s");
            dto.StartDate.ShouldBe(start);
            dto.UserName.ShouldBe("u");
        }

        [Fact]
        public void GetEntityChangeInput_Normalize_DefaultsAndBranches()
        {
            var dto = new GetEntityChangeInput();
            dto.Normalize();
            dto.Sorting.ShouldBe("EntityChange.ChangeTime DESC");

            dto.Sorting = "UserName";
            dto.Normalize();
            dto.Sorting.ShouldBe("User.UserName");

            dto.Sorting = "ChangeTime";
            dto.Normalize();
            dto.Sorting.ShouldBe("EntityChange.ChangeTime");
        }

        [Fact]
        public void GetEntityTypeChangeInput_Normalize_DefaultsAndBranches()
        {
            var dto = new GetEntityTypeChangeInput();
            dto.Normalize();
            dto.Sorting.ShouldBe("EntityChange.ChangeTime DESC");

            dto.Sorting = "UserName";
            dto.Normalize();
            dto.Sorting.ShouldBe("User.UserName");

            dto.Sorting = "Field";
            dto.Normalize();
            dto.Sorting.ShouldBe("EntityChange.Field");
        }

        [Fact]
        public void GetEntityChangeInput_ShouldSet()
        {
            var dto = new GetEntityChangeInput
            {
                EndDate = DateTime.UtcNow,
                EntityTypeFullName = "T",
                StartDate = DateTime.UtcNow.AddDays(-1),
                UserName = "u"
            };
            dto.EntityTypeFullName.ShouldBe("T");
            dto.UserName.ShouldBe("u");
        }

        [Fact]
        public void GetEntityTypeChangeInput_ShouldSet()
        {
            var dto = new GetEntityTypeChangeInput { EntityId = "1", EntityTypeFullName = "T" };
            dto.EntityId.ShouldBe("1");
            dto.EntityTypeFullName.ShouldBe("T");
        }
    }
}
