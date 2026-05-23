using Eaf.Middleware.Security.Recaptcha;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Security.Recaptcha
{
    public class NullRecaptchaValidatorTests
    {
        [Fact]
        public void Instance_IsSingleton()
        {
            NullRecaptchaValidator.Instance.ShouldNotBeNull();
            NullRecaptchaValidator.Instance.ShouldBeSameAs(NullRecaptchaValidator.Instance);
        }

        [Fact]
        public async System.Threading.Tasks.Task ValidateAsync_ReturnsCompletedTask()
        {
            await NullRecaptchaValidator.Instance.ValidateAsync("any");
        }
    }
}
