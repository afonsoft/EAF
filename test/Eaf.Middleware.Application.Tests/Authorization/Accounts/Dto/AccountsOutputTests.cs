using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class AccountsOutputTests
    {
        [Fact]
        public void RegisterOutput_ShouldSet()
        {
            var dto = new RegisterOutput { CanLogin = true };
            dto.CanLogin.ShouldBeTrue();
        }

        [Fact]
        public void IsTenantAvailableOutput_DefaultCtor()
        {
            var dto = new IsTenantAvailableOutput();
            dto.ServerRootAddress.ShouldBeNull();
            dto.State.ShouldBe((TenantAvailabilityState)0);
            dto.TenantId.ShouldBeNull();
        }

        [Fact]
        public void IsTenantAvailableOutput_TwoArgCtor()
        {
            var dto = new IsTenantAvailableOutput(TenantAvailabilityState.Available, 5);
            dto.State.ShouldBe(TenantAvailabilityState.Available);
            dto.TenantId.ShouldBe(5);
            dto.ServerRootAddress.ShouldBeNull();
        }

        [Fact]
        public void IsTenantAvailableOutput_ThreeArgCtor()
        {
            var dto = new IsTenantAvailableOutput(TenantAvailabilityState.NotFound, null, "http://x");
            dto.State.ShouldBe(TenantAvailabilityState.NotFound);
            dto.TenantId.ShouldBeNull();
            dto.ServerRootAddress.ShouldBe("http://x");
        }

        [Fact]
        public void TenantAvailabilityState_EnumValues()
        {
            ((int)TenantAvailabilityState.Available).ShouldBe(1);
            ((int)TenantAvailabilityState.InActive).ShouldBe(2);
            ((int)TenantAvailabilityState.NotFound).ShouldBe(3);
        }
    }
}
