using Eaf.Middleware.Common.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Common.Dto
{
    public class CommonDtoCoverageTests
    {
        [Fact]
        public void FindUsersInput_ShouldSet()
        {
            var dto = new FindUsersInput { TenantId = 5 };
            dto.TenantId.ShouldBe(5);
        }

        [Fact]
        public void FindUsersInput_DefaultTenantIdIsNull()
        {
            var dto = new FindUsersInput();
            dto.TenantId.ShouldBeNull();
        }
    }
}
