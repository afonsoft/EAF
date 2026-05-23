using Eaf.Middleware.Debugging;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Net
{
    public class DebugHelperTests
    {
        [Fact]
        public void Dado_DebugHelper_Quando_VerificarIsDebug_Entao_DeveRetornarBooleano()
        {
            var isDebug = DebugHelper.IsDebug;
            isDebug.ShouldBeOneOf(true, false);
        }
    }
}
