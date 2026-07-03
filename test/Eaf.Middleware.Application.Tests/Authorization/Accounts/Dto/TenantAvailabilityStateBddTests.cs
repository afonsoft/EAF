using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Accounts
{
    public class TenantAvailabilityStateBddTests
    {
        [Fact]
        public void Dado_Enum_Quando_VerificarValores_Entao_DeveConterTodosOsValores()
        {
            TenantAvailabilityState.Available.ShouldBeOfType<TenantAvailabilityState>();
            TenantAvailabilityState.InActive.ShouldBeOfType<TenantAvailabilityState>();
        }

        [Fact]
        public void Dado_Enum_Quando_VerificarTipo_Entao_DeveSerEnum()
        {
            typeof(TenantAvailabilityState).IsEnum.ShouldBeTrue();
        }
    }
}
