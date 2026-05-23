using Eaf.Middleware.Logging.Dto;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Logging.Dto
{
    public class LoggingDtoTests
    {
        [Fact]
        public void GetLatestWebLogsOutput_ShouldSet()
        {
            var dto = new GetLatestWebLogsOutput { LatestWebLogLines = new List<string> { "a", "b" } };
            dto.LatestWebLogLines.Count.ShouldBe(2);
        }
    }
}
