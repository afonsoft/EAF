using Eaf.Middleware.Localization;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Localization
{
    public class FamFamFamFlagsHelperTests
    {
        [Fact]
        public void FlagClassNames_ShouldNotBeEmpty()
        {
            // Act
            var count = FamFamFamFlagsHelper.FlagClassNames.Count;

            // Assert
            count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void FlagClassNames_ShouldContainExpectedFlags()
        {
            // Act
            var containsUs = FamFamFamFlagsHelper.FlagClassNames.Contains("famfamfam-flags us");
            var containsBr = FamFamFamFlagsHelper.FlagClassNames.Contains("famfamfam-flags br");
            var containsGb = FamFamFamFlagsHelper.FlagClassNames.Contains("famfamfam-flags gb");

            // Assert
            containsUs.ShouldBeTrue();
            containsBr.ShouldBeTrue();
            containsGb.ShouldBeTrue();
        }

        [Fact]
        public void GetCountryCode_WithValidFlagName_ReturnsCountryCode()
        {
            // Arrange
            var flagName = "famfamfam-flags us";

            // Act
            var countryCode = FamFamFamFlagsHelper.GetCountryCode(flagName);

            // Assert
            countryCode.ShouldBe("us");
        }

        [Fact]
        public void GetCountryCode_WithBrazilFlag_ReturnsBr()
        {
            // Arrange
            var flagName = "famfamfam-flags br";

            // Act
            var countryCode = FamFamFamFlagsHelper.GetCountryCode(flagName);

            // Assert
            countryCode.ShouldBe("br");
        }

        [Fact]
        public void GetCountryCode_WithUnitedKingdomFlag_ReturnsGb()
        {
            // Arrange
            var flagName = "famfamfam-flags gb";

            // Act
            var countryCode = FamFamFamFlagsHelper.GetCountryCode(flagName);

            // Assert
            countryCode.ShouldBe("gb");
        }

        [Fact]
        public void GetCountryCode_WithWalesFlag_ReturnsWales()
        {
            // Arrange
            var flagName = "famfamfam-flags wales";

            // Act
            var countryCode = FamFamFamFlagsHelper.GetCountryCode(flagName);

            // Assert
            countryCode.ShouldBe("wales");
        }

        [Fact]
        public void GetCountryCode_WithScotlandFlag_ReturnsScotland()
        {
            // Arrange
            var flagName = "famfamfam-flags scotland";

            // Act
            var countryCode = FamFamFamFlagsHelper.GetCountryCode(flagName);

            // Assert
            countryCode.ShouldBe("scotland");
        }

        [Fact]
        public void GetCountryCode_WithEnglandFlag_ReturnsEngland()
        {
            // Arrange
            var flagName = "famfamfam-flags england";

            // Act
            var countryCode = FamFamFamFlagsHelper.GetCountryCode(flagName);

            // Assert
            countryCode.ShouldBe("england");
        }

        [Fact]
        public void GetCountryCode_WithCataloniaFlag_ReturnsCatalonia()
        {
            // Arrange
            var flagName = "famfamfam-flags catalonia";

            // Act
            var countryCode = FamFamFamFlagsHelper.GetCountryCode(flagName);

            // Assert
            countryCode.ShouldBe("catalonia");
        }
    }
}
