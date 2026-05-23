using Eaf.Middleware.MultiTenancy.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.MultiTenancy.Dto
{
    public class CreateTenantInputTests
    {
        [Fact]
        public void Dado_CreateTenantInput_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var input = new CreateTenantInput();

            input.AdminEmailAddress.ShouldBeNull();
            input.AdminPassword.ShouldBeNull();
            input.IsActive.ShouldBeFalse();
            input.Name.ShouldBeNull();
            input.SendActivationEmail.ShouldBeFalse();
            input.ShouldChangePasswordOnNextLogin.ShouldBeFalse();
            input.TenancyName.ShouldBeNull();
        }

        [Fact]
        public void Dado_CreateTenantInput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var input = new CreateTenantInput
            {
                AdminEmailAddress = "admin@tenant.com",
                AdminPassword = "P@ssw0rd",
                IsActive = true,
                Name = "New Tenant",
                SendActivationEmail = true,
                ShouldChangePasswordOnNextLogin = true,
                TenancyName = "NewTenant"
            };

            input.AdminEmailAddress.ShouldBe("admin@tenant.com");
            input.AdminPassword.ShouldBe("P@ssw0rd");
            input.IsActive.ShouldBeTrue();
            input.Name.ShouldBe("New Tenant");
            input.SendActivationEmail.ShouldBeTrue();
            input.ShouldChangePasswordOnNextLogin.ShouldBeTrue();
            input.TenancyName.ShouldBe("NewTenant");
        }

        [Fact]
        public void Dado_CreateTenantInput_Quando_Verificado_Entao_AdminEmailDeveConterRequiredAttribute()
        {
            var prop = typeof(CreateTenantInput).GetProperty(nameof(CreateTenantInput.AdminEmailAddress));
            prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault().ShouldNotBeNull();
        }

        [Fact]
        public void Dado_CreateTenantInput_Quando_Verificado_Entao_AdminEmailDeveConterEmailAddressAttribute()
        {
            var prop = typeof(CreateTenantInput).GetProperty(nameof(CreateTenantInput.AdminEmailAddress));
            prop!.GetCustomAttributes(typeof(EmailAddressAttribute), false).FirstOrDefault().ShouldNotBeNull();
        }

        [Fact]
        public void Dado_CreateTenantInput_Quando_Verificado_Entao_NameDeveConterRequiredAttribute()
        {
            var prop = typeof(CreateTenantInput).GetProperty(nameof(CreateTenantInput.Name));
            prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault().ShouldNotBeNull();
        }

        [Fact]
        public void Dado_CreateTenantInput_Quando_Verificado_Entao_TenancyNameDeveConterRequiredAttribute()
        {
            var prop = typeof(CreateTenantInput).GetProperty(nameof(CreateTenantInput.TenancyName));
            prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault().ShouldNotBeNull();
        }
    }
}
