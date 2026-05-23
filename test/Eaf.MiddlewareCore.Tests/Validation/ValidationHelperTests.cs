using Eaf.Middleware.Validation;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Validation
{
    public class ValidationHelperTests
    {
        [Fact]
        public void IsEmail_ValidEmail_ReturnsTrue()
        {
            // Arrange
            var validEmail = "test@example.com";

            // Act
            var result = ValidationHelper.IsEmail(validEmail);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void IsEmail_ValidEmailWithDots_ReturnsTrue()
        {
            // Arrange
            var validEmail = "test.name@example.com";

            // Act
            var result = ValidationHelper.IsEmail(validEmail);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void IsEmail_ValidEmailWithPlus_ReturnsTrue()
        {
            // Arrange
            var validEmail = "test+tag@example.com";

            // Act
            var result = ValidationHelper.IsEmail(validEmail);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void IsEmail_ValidEmailWithSubdomain_ReturnsTrue()
        {
            // Arrange
            var validEmail = "test@mail.example.com";

            // Act
            var result = ValidationHelper.IsEmail(validEmail);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void IsEmail_InvalidEmailNoAtSymbol_ReturnsFalse()
        {
            // Arrange
            var invalidEmail = "testexample.com";

            // Act
            var result = ValidationHelper.IsEmail(invalidEmail);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void IsEmail_InvalidEmailNoDomain_ReturnsFalse()
        {
            // Arrange
            var invalidEmail = "test@";

            // Act
            var result = ValidationHelper.IsEmail(invalidEmail);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void IsEmail_InvalidEmailNoLocalPart_ReturnsFalse()
        {
            // Arrange
            var invalidEmail = "@example.com";

            // Act
            var result = ValidationHelper.IsEmail(invalidEmail);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void IsEmail_NullEmail_ReturnsFalse()
        {
            // Arrange
            string nullEmail = null;

            // Act
            var result = ValidationHelper.IsEmail(nullEmail);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void IsEmail_EmptyEmail_ReturnsFalse()
        {
            // Arrange
            var emptyEmail = string.Empty;

            // Act
            var result = ValidationHelper.IsEmail(emptyEmail);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void IsEmail_WhitespaceEmail_ReturnsFalse()
        {
            // Arrange
            var whitespaceEmail = "   ";

            // Act
            var result = ValidationHelper.IsEmail(whitespaceEmail);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void EmailRegex_ShouldNotBeEmpty()
        {
            // Act
            var emailRegex = ValidationHelper.EmailRegex;

            // Assert
            emailRegex.ShouldNotBeNull();
            emailRegex.ShouldNotBeEmpty();
        }

        [Fact]
        public void ValidationHelper_ShouldBeStaticClass()
        {
            // Arrange & Act
            var type = typeof(ValidationHelper);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }
    }
}
