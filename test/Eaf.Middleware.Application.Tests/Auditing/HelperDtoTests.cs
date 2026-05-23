using Eaf.Middleware.Auditing;
using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Auditing
{
    public class HelperDtoTests
    {
        [Fact]
        public void AuditLogAndUser_ShouldSet()
        {
            var dto = new AuditLogAndUser();
            dto.AuditLog.ShouldBeNull();
            dto.User.ShouldBeNull();
        }

        [Fact]
        public void EntityChangeAndUser_ShouldSet()
        {
            var dto = new EntityChangeAndUser();
            dto.EntityChange.ShouldBeNull();
            dto.User.ShouldBeNull();
        }

        [Fact]
        public void GetEntityChangeInput_ShouldSet()
        {
            var dto = new GetEntityChangeInput
            {
                EndDate = new System.DateTime(2024, 1, 1),
                EntityTypeFullName = "e",
                StartDate = new System.DateTime(2023, 1, 1),
                UserName = "u"
            };
            dto.EndDate.ShouldBe(new System.DateTime(2024, 1, 1));
            dto.EntityTypeFullName.ShouldBe("e");
            dto.StartDate.ShouldBe(new System.DateTime(2023, 1, 1));
            dto.UserName.ShouldBe("u");
        }
    }
}
