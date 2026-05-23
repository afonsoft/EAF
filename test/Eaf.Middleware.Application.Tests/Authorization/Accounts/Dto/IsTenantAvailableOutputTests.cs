using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class IsTenantAvailableOutputTests
    {
        [Fact]
        public void Dado_IsTenantAvailableOutput_Quando_CriadoSemParametros_Entao_PropriedadesDevemSerPadrao()
        {
            var output = new IsTenantAvailableOutput();

            output.State.ShouldBe(default(TenantAvailabilityState));
            output.TenantId.ShouldBeNull();
            output.ServerRootAddress.ShouldBeNull();
        }

        [Fact]
        public void Dado_IsTenantAvailableOutput_Quando_CriadoComStateETenantId_Entao_DevemSerAtribuidos()
        {
            var output = new IsTenantAvailableOutput(TenantAvailabilityState.Available, 1);

            output.State.ShouldBe(TenantAvailabilityState.Available);
            output.TenantId.ShouldBe(1);
            output.ServerRootAddress.ShouldBeNull();
        }

        [Fact]
        public void Dado_IsTenantAvailableOutput_Quando_CriadoComTodosParametros_Entao_DevemSerAtribuidos()
        {
            var output = new IsTenantAvailableOutput(TenantAvailabilityState.InActive, 2, "https://example.com");

            output.State.ShouldBe(TenantAvailabilityState.InActive);
            output.TenantId.ShouldBe(2);
            output.ServerRootAddress.ShouldBe("https://example.com");
        }

        [Fact]
        public void Dado_IsTenantAvailableOutput_Quando_CriadoComStateNotFound_Entao_TenantIdDeveSerNulo()
        {
            var output = new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);

            output.State.ShouldBe(TenantAvailabilityState.NotFound);
            output.TenantId.ShouldBeNull();
        }
    }
}
