using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    public class ImpersonateModelTests
    {
        [Fact]
        public void Dado_ImpersonateModel_Quando_Criado_Entao_PropriedadesDevemSerAtribuidas()
        {
            var model = new ImpersonateModel
            {
                TenantId = 5,
                UserId = 42
            };

            model.TenantId.ShouldBe(5);
            model.UserId.ShouldBe(42);
        }

        [Fact]
        public void Dado_ImpersonateModel_Quando_TenantIdNulo_Entao_DevePermitirNulo()
        {
            var model = new ImpersonateModel
            {
                TenantId = null,
                UserId = 1
            };

            model.TenantId.ShouldBeNull();
        }

        [Fact]
        public void Dado_ImpersonateModel_Quando_UserId_Entao_DeveConterRangeAttribute()
        {
            var prop = typeof(ImpersonateModel).GetProperty(nameof(ImpersonateModel.UserId));
            var rangeAttr = prop!.GetCustomAttributes(typeof(RangeAttribute), false).Cast<RangeAttribute>().FirstOrDefault();

            rangeAttr.ShouldNotBeNull();
            rangeAttr!.Minimum.ShouldBe(1);
            rangeAttr.Maximum.ShouldBe(long.MaxValue);
        }
    }
}
