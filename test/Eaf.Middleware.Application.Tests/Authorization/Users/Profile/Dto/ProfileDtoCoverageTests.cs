using Abp.Web.Models;
using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Eaf.Middleware.Security;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Profile.Dto
{
    public class ProfileDtoCoverageTests
    {
        [Fact]
        public void ChangePasswordInput_ShouldSet()
        {
            var dto = new ChangePasswordInput { CurrentPassword = "old", NewPassword = "new" };
            dto.CurrentPassword.ShouldBe("old");
            dto.NewPassword.ShouldBe("new");
        }

        [Fact]
        public void CurrentUserProfileEditDto_ShouldSet()
        {
            var dto = new CurrentUserProfileEditDto
            {
                EmailAddress = "a@b.com",
                Name = "n",
                Surname = "s",
                Timezone = "UTC",
                UserName = "u"
            };
            dto.EmailAddress.ShouldBe("a@b.com");
            dto.Name.ShouldBe("n");
            dto.Surname.ShouldBe("s");
            dto.Timezone.ShouldBe("UTC");
            dto.UserName.ShouldBe("u");
        }

        [Fact]
        public void GetPasswordComplexitySettingOutput_ShouldSet()
        {
            var complexity = new PasswordComplexitySetting();
            var dto = new GetPasswordComplexitySettingOutput { Setting = complexity };
            dto.Setting.ShouldBe(complexity);
        }

        [Fact]
        public void GetProfilePictureOutput_ShouldSetViaCtor()
        {
            var dto = new GetProfilePictureOutput("picture");
            dto.ProfilePicture.ShouldBe("picture");

            dto.ProfilePicture = "new";
            dto.ProfilePicture.ShouldBe("new");
        }

        [Fact]
        public void UpdateProfilePictureInput_ShouldSet()
        {
            var dto = new UpdateProfilePictureInput
            {
                FileToken = "t",
                Height = 10,
                Width = 20,
                X = 1,
                Y = 2
            };
            dto.FileToken.ShouldBe("t");
            dto.Height.ShouldBe(10);
            dto.Width.ShouldBe(20);
            dto.X.ShouldBe(1);
            dto.Y.ShouldBe(2);
        }

        [Fact]
        public void UploadProfilePictureOutput_DefaultCtor()
        {
            var dto = new UploadProfilePictureOutput
            {
                FileName = "f",
                FileToken = "t",
                FileType = "ft",
                Height = 1,
                Width = 2
            };
            dto.FileName.ShouldBe("f");
            dto.FileToken.ShouldBe("t");
            dto.FileType.ShouldBe("ft");
            dto.Height.ShouldBe(1);
            dto.Width.ShouldBe(2);
        }

        [Fact]
        public void UploadProfilePictureOutput_CopyCtor_FromErrorInfo()
        {
            var error = new ErrorInfo { Code = 500, Details = "d", Message = "m" };
            var dto = new UploadProfilePictureOutput(error);
            dto.Code.ShouldBe(500);
            dto.Details.ShouldBe("d");
            dto.Message.ShouldBe("m");
        }
    }
}
