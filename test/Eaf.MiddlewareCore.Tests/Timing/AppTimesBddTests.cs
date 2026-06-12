using Eaf.Middleware.Timing;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Timing
{
    /// <summary>
    /// Testes BDD para AppTimes seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AppTimesBddTests
    {
        [Fact]
        public void Dado_AppTimes_Quando_DefinirStartupTime_Entao_DeveArmazenar()
        {
            var times = new AppTimes();
            var now = DateTime.UtcNow;
            times.StartupTime = now;
            times.StartupTime.ShouldBe(now);
        }

        [Fact]
        public void Dado_AppTimes_Quando_CriarNovo_Entao_StartupTimeDeveTerValorPadrao()
        {
            var times = new AppTimes();
            times.StartupTime.ShouldBe(default(DateTime));
        }
    }
}
