using Eaf.Middleware.Sessions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Sessions.Dto
{
    public class GetCurrentLoginInformationsOutputTests
    {
        [Fact]
        public void Dado_GetCurrentLoginInformationsOutput_Quando_Criado_Entao_PropriedadesDevemSerNulas()
        {
            var output = new GetCurrentLoginInformationsOutput();

            output.Application.ShouldBeNull();
            output.Tenant.ShouldBeNull();
            output.Theme.ShouldBeNull();
            output.User.ShouldBeNull();
        }

        [Fact]
        public void Dado_GetCurrentLoginInformationsOutput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var app = new ApplicationInfoDto { Version = "1.0" };
            var tenant = new TenantLoginInfoDto { Name = "Default" };
            var user = new UserLoginInfoDto { UserName = "admin" };

            var output = new GetCurrentLoginInformationsOutput
            {
                Application = app,
                Tenant = tenant,
                User = user
            };

            output.Application.Version.ShouldBe("1.0");
            output.Tenant.Name.ShouldBe("Default");
            output.User.UserName.ShouldBe("admin");
        }
    }
}
