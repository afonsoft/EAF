using Eaf.Middleware.Timing;
using Shouldly;
using System;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Timing
{
    public class AppTimesTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirStartupTime_Entao_DeveArmazenar()
        {
            var appTimes = new AppTimes();
            var now = DateTime.UtcNow;
            appTimes.StartupTime = now;
            appTimes.StartupTime.ShouldBe(now);
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_NaoDefinirStartupTime_Entao_DeveSerDefault()
        {
            var appTimes = new AppTimes();
            appTimes.StartupTime.ShouldBe(default);
        }
    }
}
