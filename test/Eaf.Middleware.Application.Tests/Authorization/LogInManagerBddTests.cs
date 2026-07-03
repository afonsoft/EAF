using Eaf.Middleware.Authorization;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization
{
    public class LogInManagerBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarHeranca_Entao_DeveHerdarDeAbpLogInManager()
        {
            typeof(LogInManager).BaseType.Name.ShouldContain("AbpLogInManager");
        }
    }
}
