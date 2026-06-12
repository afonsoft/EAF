using Eaf.Middleware.Security.Recaptcha;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Security.Recaptcha
{
    /// <summary>
    /// Testes BDD para NullRecaptchaValidator seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class NullRecaptchaValidatorBddTests
    {
        [Fact]
        public void Dado_Instance_Quando_Acessar_Entao_DeveRetornarInstanciaSingleton()
        {
            var instance1 = NullRecaptchaValidator.Instance;
            var instance2 = NullRecaptchaValidator.Instance;
            instance1.ShouldBeSameAs(instance2);
        }

        [Fact]
        public async Task Dado_QualquerCaptchaResponse_Quando_ValidateAsync_Entao_DeveCompletarSemExcecao()
        {
            var validator = NullRecaptchaValidator.Instance;
            await validator.ValidateAsync("any-captcha-response");
        }

        [Fact]
        public async Task Dado_CaptchaResponseNull_Quando_ValidateAsync_Entao_DeveCompletarSemExcecao()
        {
            var validator = NullRecaptchaValidator.Instance;
            await validator.ValidateAsync(null);
        }

        [Fact]
        public async Task Dado_CaptchaResponseVazio_Quando_ValidateAsync_Entao_DeveCompletarSemExcecao()
        {
            var validator = NullRecaptchaValidator.Instance;
            await validator.ValidateAsync("");
        }
    }
}
