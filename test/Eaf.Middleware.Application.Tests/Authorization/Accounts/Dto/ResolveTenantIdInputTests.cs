using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class ResolveTenantIdInputTests
    {
        [Fact]
        public void Dado_ResolveTenantIdInput_Quando_Criado_Entao_CDeveSerNulo()
        {
            var input = new ResolveTenantIdInput();
            input.c.ShouldBeNull();
        }

        [Fact]
        public void Dado_ResolveTenantIdInput_Quando_AtribuirC_Entao_DeveRetornarValor()
        {
            var input = new ResolveTenantIdInput { c = "encrypted-data" };
            input.c.ShouldBe("encrypted-data");
        }
    }
}
