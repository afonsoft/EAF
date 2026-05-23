using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class ImpersonateInputTests
    {
        [Fact]
        public void Dado_ImpersonateInput_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var input = new ImpersonateInput();
            input.TenantId.ShouldBeNull();
            input.UserId.ShouldBe(0L);
        }

        [Fact]
        public void Dado_ImpersonateInput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var input = new ImpersonateInput { TenantId = 1, UserId = 42L };
            input.TenantId.ShouldBe(1);
            input.UserId.ShouldBe(42L);
        }

        [Fact]
        public void Dado_ImpersonateInput_Quando_Verificado_Entao_UserIdDeveConterRangeAttribute()
        {
            var prop = typeof(ImpersonateInput).GetProperty(nameof(ImpersonateInput.UserId));
            var attr = prop!.GetCustomAttributes(typeof(RangeAttribute), false).FirstOrDefault() as RangeAttribute;
            attr.ShouldNotBeNull();
            attr!.Minimum.ShouldBe(1.0);
        }
    }
}
