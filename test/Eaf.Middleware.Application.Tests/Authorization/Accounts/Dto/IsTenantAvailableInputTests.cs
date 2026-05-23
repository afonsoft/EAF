using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class IsTenantAvailableInputTests
    {
        [Fact]
        public void Dado_IsTenantAvailableInput_Quando_Criado_Entao_TenancyNameDeveSerNulo()
        {
            var input = new IsTenantAvailableInput();
            input.TenancyName.ShouldBeNull();
        }

        [Fact]
        public void Dado_IsTenantAvailableInput_Quando_AtribuirTenancyName_Entao_DeveRetornarValor()
        {
            var input = new IsTenantAvailableInput { TenancyName = "Default" };
            input.TenancyName.ShouldBe("Default");
        }

        [Fact]
        public void Dado_IsTenantAvailableInput_Quando_Verificado_Entao_TenancyNameDeveConterRequiredAttribute()
        {
            var prop = typeof(IsTenantAvailableInput).GetProperty(nameof(IsTenantAvailableInput.TenancyName));
            var attr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            attr.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_IsTenantAvailableInput_Quando_Verificado_Entao_TenancyNameDeveConterMaxLengthAttribute()
        {
            var prop = typeof(IsTenantAvailableInput).GetProperty(nameof(IsTenantAvailableInput.TenancyName));
            var attr = prop!.GetCustomAttributes(typeof(MaxLengthAttribute), false).FirstOrDefault() as MaxLengthAttribute;
            attr.ShouldNotBeNull();
            attr!.Length.ShouldBeGreaterThan(0);
        }
    }
}
