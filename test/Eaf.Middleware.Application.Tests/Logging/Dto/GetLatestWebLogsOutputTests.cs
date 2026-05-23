using Eaf.Middleware.Logging.Dto;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Logging.Dto
{
    public class GetLatestWebLogsOutputTests
    {
        [Fact]
        public void Dado_GetLatestWebLogsOutput_Quando_Criado_Entao_LatestWebLogLinesDeveSerNulo()
        {
            var output = new GetLatestWebLogsOutput();
            output.LatestWebLogLines.ShouldBeNull();
        }

        [Fact]
        public void Dado_GetLatestWebLogsOutput_Quando_AtribuirLinhas_Entao_DevemSerRetornadas()
        {
            var lines = new List<string> { "Line 1", "Line 2", "Line 3" };
            var output = new GetLatestWebLogsOutput { LatestWebLogLines = lines };

            output.LatestWebLogLines.Count.ShouldBe(3);
            output.LatestWebLogLines[0].ShouldBe("Line 1");
        }
    }
}
