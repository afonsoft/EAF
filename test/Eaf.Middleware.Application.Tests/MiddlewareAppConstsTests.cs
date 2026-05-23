using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests
{
    public class MiddlewareAppConstsTests
    {
        [Fact]
        public void DefaultPageSize_ShouldBe30()
        {
            // Arrange & Act
            var defaultPageSize = MiddlewareAppConsts.DefaultPageSize;

            // Assert
            defaultPageSize.ShouldBe(30);
        }

        [Fact]
        public void LocalizationSourceName_ShouldBeEafCore()
        {
            // Arrange & Act
            var localizationSourceName = MiddlewareAppConsts.LocalizationSourceName;

            // Assert
            localizationSourceName.ShouldBe("EafCore");
        }

        [Fact]
        public void MaxPageSize_ShouldBe300000()
        {
            // Arrange & Act
            var maxPageSize = MiddlewareAppConsts.MaxPageSize;

            // Assert
            maxPageSize.ShouldBe(300000);
        }

        [Fact]
        public void MaxProfilPictureBytesUserFriendlyValue_ShouldBe5()
        {
            // Arrange & Act
            var maxProfilPictureBytes = MiddlewareAppConsts.MaxProfilPictureBytesUserFriendlyValue;

            // Assert
            maxProfilPictureBytes.ShouldBe(5);
        }

        [Fact]
        public void ResizedMaxProfilPictureBytesUserFriendlyValue_ShouldBe1024()
        {
            // Arrange & Act
            var resizedMaxProfilPictureBytes = MiddlewareAppConsts.ResizedMaxProfilPictureBytesUserFriendlyValue;

            // Assert
            resizedMaxProfilPictureBytes.ShouldBe(1024);
        }

        [Fact]
        public void SystemProvider_ShouldBeSystem()
        {
            // Arrange & Act
            var systemProvider = MiddlewareAppConsts.SystemProvider;

            // Assert
            systemProvider.ShouldBe("System");
        }

        [Fact]
        public void Theme2_ShouldBeTheme2()
        {
            // Arrange & Act
            var theme2 = MiddlewareAppConsts.Theme2;

            // Assert
            theme2.ShouldBe("theme2");
        }

        [Fact]
        public void Theme3_ShouldBeTheme3()
        {
            // Arrange & Act
            var theme3 = MiddlewareAppConsts.Theme3;

            // Assert
            theme3.ShouldBe("theme3");
        }

        [Fact]
        public void Theme4_ShouldBeTheme4()
        {
            // Arrange & Act
            var theme4 = MiddlewareAppConsts.Theme4;

            // Assert
            theme4.ShouldBe("theme4");
        }

        [Fact]
        public void ThemeDefault_ShouldBeDefault()
        {
            // Arrange & Act
            var themeDefault = MiddlewareAppConsts.ThemeDefault;

            // Assert
            themeDefault.ShouldBe("default");
        }

        [Fact]
        public void DefaultPageSize_ShouldBeLessThanMaxPageSize()
        {
            // Arrange & Act
            var defaultPageSize = MiddlewareAppConsts.DefaultPageSize;
            var maxPageSize = MiddlewareAppConsts.MaxPageSize;

            // Assert
            defaultPageSize.ShouldBeLessThan(maxPageSize);
        }
    }
}
