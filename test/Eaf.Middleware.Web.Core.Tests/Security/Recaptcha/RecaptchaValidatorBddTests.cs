using Abp;
using Abp.UI;
using Eaf.Middleware.Web.Security.Recaptcha;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Owl.reCAPTCHA;
using Owl.reCAPTCHA.v3;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Security.Recaptcha
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

        [Fact]
        public async Task Dado_HttpContextNulo_Quando_ValidarAsync_Entao_DeveLancarAbpException()
        {
            // Dado
            var siteVerify = Substitute.For<IreCAPTCHASiteVerifyV3>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.Returns((HttpContext)null);

            var sut = new RecaptchaValidator(siteVerify, httpContextAccessor);

            // Quando / Então
            await Should.ThrowAsync<AbpException>(() => sut.ValidateAsync("response"));
        }

        [Fact]
        public async Task Dado_CaptchaResponseVazio_Quando_ValidarAsync_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var siteVerify = Substitute.For<IreCAPTCHASiteVerifyV3>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.Returns(new DefaultHttpContext());

            var sut = new RecaptchaValidator(siteVerify, httpContextAccessor);

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() => sut.ValidateAsync(""));
        }

        [Fact]
        public async Task Dado_CaptchaResponseValido_Quando_ValidarAsync_Entao_DeveCompletarSemExcecao()
        {
            // Dado
            var siteVerify = Substitute.For<IreCAPTCHASiteVerifyV3>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.Returns(new DefaultHttpContext());

            siteVerify.Verify(Arg.Any<reCAPTCHASiteVerifyRequest>())
                .Returns(Task.FromResult(new reCAPTCHASiteVerifyV3Response { Success = true, Score = 0.9f }));

            var sut = new RecaptchaValidator(siteVerify, httpContextAccessor);

            // Quando
            await sut.ValidateAsync("valid-response");

            // Então
            await siteVerify.Received(1).Verify(Arg.Any<reCAPTCHASiteVerifyRequest>());
        }

        [Fact]
        public async Task Dado_CaptchaResponseInvalido_Quando_ValidarAsync_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var siteVerify = Substitute.For<IreCAPTCHASiteVerifyV3>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.Returns(new DefaultHttpContext());

            siteVerify.Verify(Arg.Any<reCAPTCHASiteVerifyRequest>())
                .Returns(Task.FromResult(new reCAPTCHASiteVerifyV3Response { Success = false, Score = 0.1f }));

            var sut = new RecaptchaValidator(siteVerify, httpContextAccessor);

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() => sut.ValidateAsync("invalid-response"));
        }
    }
}
