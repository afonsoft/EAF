using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class AccountsDtoCoverageTests
    {
        [Fact]
        public void ActivateEmailInput_ShouldSetProperties()
        {
            var dto = new ActivateEmailInput { c = "encrypted", ConfirmationCode = "CONF", UserId = 42 };
            dto.c.ShouldBe("encrypted");
            dto.ConfirmationCode.ShouldBe("CONF");
            dto.UserId.ShouldBe(42);
        }

        [Fact]
        public void ActivateEmailInput_Normalize_WithEmptyC_ShouldNotThrow()
        {
            var dto = new ActivateEmailInput();
            Should.NotThrow(() => dto.Normalize());
        }

        [Fact]
        public void ImpersonateInput_DefaultsAndSet()
        {
            var dto = new ImpersonateInput();
            dto.TenantId.ShouldBeNull();
            dto.UserId.ShouldBe(0);

            dto.TenantId = 1;
            dto.UserId = 99;
            dto.TenantId.ShouldBe(1);
            dto.UserId.ShouldBe(99);
        }

        [Fact]
        public void ImpersonateOutput_ShouldSetProperties()
        {
            var output = new ImpersonateOutput { ImpersonationToken = "tk", TenancyName = "tn" };
            output.ImpersonationToken.ShouldBe("tk");
            output.TenancyName.ShouldBe("tn");
        }

        [Fact]
        public void IsTenantAvailableInput_ShouldSet()
        {
            var dto = new IsTenantAvailableInput { TenancyName = "acme" };
            dto.TenancyName.ShouldBe("acme");
        }

        [Fact]
        public void CurrentTenantInfoDto_ShouldSet()
        {
            var dto = new CurrentTenantInfoDto { Name = "n", TenancyName = "tn", Id = 7 };
            dto.Name.ShouldBe("n");
            dto.TenancyName.ShouldBe("tn");
            dto.Id.ShouldBe(7);
        }

        [Fact]
        public void ResetPasswordInput_ShouldSetAndNormalizeEmptyC()
        {
            var dto = new ResetPasswordInput
            {
                AuthenticationSource = "src",
                c = "",
                Password = "pwd",
                ResetCode = "rc",
                ReturnUrl = "/home",
                SingleSignIn = "1",
                UserId = 1
            };
            dto.AuthenticationSource.ShouldBe("src");
            dto.Password.ShouldBe("pwd");
            dto.ResetCode.ShouldBe("rc");
            dto.ReturnUrl.ShouldBe("/home");
            dto.SingleSignIn.ShouldBe("1");
            dto.UserId.ShouldBe(1);
            Should.NotThrow(() => dto.Normalize());
        }

        [Fact]
        public void ResolveTenantIdInput_ShouldSet()
        {
            var dto = new ResolveTenantIdInput { c = "abc" };
            dto.c.ShouldBe("abc");
        }

        [Fact]
        public void SendEmailActivationLinkInput_ShouldSet()
        {
            var dto = new SendEmailActivationLinkInput { EmailAddress = "a@b.com" };
            dto.EmailAddress.ShouldBe("a@b.com");
        }

        [Fact]
        public void SendPasswordResetCodeInput_ShouldSet()
        {
            var dto = new SendPasswordResetCodeInput { EmailAddress = "a@b.com" };
            dto.EmailAddress.ShouldBe("a@b.com");
        }
    }
}
