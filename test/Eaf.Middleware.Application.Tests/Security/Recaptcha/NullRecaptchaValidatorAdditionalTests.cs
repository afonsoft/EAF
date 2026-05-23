using Eaf.Middleware.Security.Recaptcha;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Security.Recaptcha
{
    public class NullRecaptchaValidatorAdditionalTests
    {
        [Fact]
        public void Dado_NullRecaptchaValidator_Quando_AcessarInstance_Entao_DeveRetornarSingleton()
        {
            var instance1 = NullRecaptchaValidator.Instance;
            var instance2 = NullRecaptchaValidator.Instance;

            instance1.ShouldBeSameAs(instance2);
        }

        [Fact]
        public void Dado_NullRecaptchaValidator_Quando_Verificado_Entao_DeveImplementarIRecaptchaValidator()
        {
            NullRecaptchaValidator.Instance.ShouldBeAssignableTo<IRecaptchaValidator>();
        }

        [Fact]
        public async Task Dado_NullRecaptchaValidator_Quando_ValidateAsync_Entao_NaoDeveLancarExcecao()
        {
            await NullRecaptchaValidator.Instance.ValidateAsync("any-token");
            await NullRecaptchaValidator.Instance.ValidateAsync(null);
            await NullRecaptchaValidator.Instance.ValidateAsync(string.Empty);
        }
    }
}
