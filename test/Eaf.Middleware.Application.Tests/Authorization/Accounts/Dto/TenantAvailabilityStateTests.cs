using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class TenantAvailabilityStateTests
    {
        [Fact]
        public void Dado_TenantAvailabilityState_Quando_Available_Entao_ValorDeveSerUm()
        {
            ((int)TenantAvailabilityState.Available).ShouldBe(1);
        }

        [Fact]
        public void Dado_TenantAvailabilityState_Quando_InActive_Entao_ValorDeveSerDois()
        {
            ((int)TenantAvailabilityState.InActive).ShouldBe(2);
        }

        [Fact]
        public void Dado_TenantAvailabilityState_Quando_NotFound_Entao_ValorDeveSerTres()
        {
            ((int)TenantAvailabilityState.NotFound).ShouldBe(3);
        }
    }
}
