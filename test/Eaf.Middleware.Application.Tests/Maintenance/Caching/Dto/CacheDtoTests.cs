using Eaf.Middleware.Maintenance.Caching.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Maintenance.Caching.Dto
{
    public class CacheDtoTests
    {
        [Fact]
        public void ShouldSetName()
        {
            var dto = new CacheDto { Name = "n" };
            dto.Name.ShouldBe("n");
        }
    }
}
