using Eaf.Middleware.Sessions.Dto;
using Eaf.Middleware.UiCustomization.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Sessions.Dto
{
    public class SessionsDtoCoverageTests
    {
        [Fact]
        public void ApplicationInfoDto_ShouldSet()
        {
            var features = new Dictionary<string, bool> { ["f"] = true };
            var dto = new ApplicationInfoDto
            {
                Currency = "USD",
                CurrencySign = "$",
                Features = features,
                ReleaseDate = new DateTime(2024, 1, 1),
                TwoFactorCodeExpireSeconds = 60,
                Version = "1.0"
            };
            dto.Currency.ShouldBe("USD");
            dto.CurrencySign.ShouldBe("$");
            dto.Features.ShouldBe(features);
            dto.ReleaseDate.ShouldBe(new DateTime(2024, 1, 1));
            dto.TwoFactorCodeExpireSeconds.ShouldBe(60);
            dto.Version.ShouldBe("1.0");
        }

        [Fact]
        public void GetCurrentLoginInformationsOutput_ShouldSet()
        {
            var dto = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto(),
                Tenant = new TenantLoginInfoDto(),
                Theme = new UiCustomizationSettingsDto(),
                User = new UserLoginInfoDto()
            };
            dto.Application.ShouldNotBeNull();
            dto.Tenant.ShouldNotBeNull();
            dto.Theme.ShouldNotBeNull();
            dto.User.ShouldNotBeNull();
        }

        [Fact]
        public void TenantLoginInfoDto_ShouldSet()
        {
            var dto = new TenantLoginInfoDto
            {
                CreationTime = new DateTime(2022, 1, 1),
                Name = "n",
                TenancyName = "tn"
            };
            dto.CreationTime.ShouldBe(new DateTime(2022, 1, 1));
            dto.Name.ShouldBe("n");
            dto.TenancyName.ShouldBe("tn");
        }

        [Fact]
        public void UpdateUserSignInTokenOutput_ShouldSet()
        {
            var dto = new UpdateUserSignInTokenOutput
            {
                EncodedTenantId = "tid",
                EncodedUserId = "uid",
                SignInToken = "tok"
            };
            dto.EncodedTenantId.ShouldBe("tid");
            dto.EncodedUserId.ShouldBe("uid");
            dto.SignInToken.ShouldBe("tok");
        }

        [Fact]
        public void UserLoginInfoDto_ShouldSet()
        {
            var dto = new UserLoginInfoDto
            {
                AuthenticationSource = "Local",
                EmailAddress = "a@b.com",
                Name = "n",
                ProfilePictureId = "pid",
                Surname = "s",
                UserName = "u"
            };
            dto.AuthenticationSource.ShouldBe("Local");
            dto.EmailAddress.ShouldBe("a@b.com");
            dto.Name.ShouldBe("n");
            dto.ProfilePictureId.ShouldBe("pid");
            dto.Surname.ShouldBe("s");
            dto.UserName.ShouldBe("u");
        }
    }
}
