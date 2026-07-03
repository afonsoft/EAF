using Eaf.Middleware.Security.Recaptcha;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Owl.reCAPTCHA.v3;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Security.Recaptcha
{
    public class RecaptchaValidatorBddTests
    {
        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var siteVerify = Substitute.For<IreCAPTCHASiteVerifyV3>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();

            var sut = new RecaptchaValidator(siteVerify, httpContextAccessor);
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Classe_Quando_VerificarConstanteRecaptchaResponseKey_Entao_DeveSerCorreta()
        {
            RecaptchaValidator.RecaptchaResponseKey.ShouldBe("g-recaptcha-response");
        }
    }
}
