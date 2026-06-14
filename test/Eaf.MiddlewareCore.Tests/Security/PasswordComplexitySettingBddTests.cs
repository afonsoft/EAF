using Eaf.Middleware.Security;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Security
{
    public class PasswordComplexitySettingBddTests
    {
        [Fact]
        public void Dado_DoisSettingsIguais_Quando_Equals_Entao_DeveRetornarTrue()
        {
            var a = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonAlphanumeric = false,
                RequiredLength = 8
            };

            var b = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonAlphanumeric = false,
                RequiredLength = 8
            };

            a.Equals(b).ShouldBeTrue();
        }

        [Fact]
        public void Dado_SettingsDiferentes_Quando_Equals_Entao_DeveRetornarFalse()
        {
            var a = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonAlphanumeric = false,
                RequiredLength = 8
            };

            var b = new PasswordComplexitySetting
            {
                RequireDigit = false,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonAlphanumeric = false,
                RequiredLength = 8
            };

            a.Equals(b).ShouldBeFalse();
        }

        [Fact]
        public void Dado_SettingNull_Quando_Equals_Entao_DeveRetornarFalse()
        {
            var a = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequiredLength = 6
            };

            a.Equals(null).ShouldBeFalse();
        }

        [Theory]
        [InlineData(true, true, true, true, 12)]
        [InlineData(false, false, false, false, 4)]
        public void Dado_PasswordComplexitySetting_Quando_DefinirPropriedades_Entao_DevePersistir(
            bool digit, bool lower, bool upper, bool nonAlpha, int length)
        {
            var setting = new PasswordComplexitySetting
            {
                RequireDigit = digit,
                RequireLowercase = lower,
                RequireUppercase = upper,
                RequireNonAlphanumeric = nonAlpha,
                RequiredLength = length
            };

            setting.RequireDigit.ShouldBe(digit);
            setting.RequireLowercase.ShouldBe(lower);
            setting.RequireUppercase.ShouldBe(upper);
            setting.RequireNonAlphanumeric.ShouldBe(nonAlpha);
            setting.RequiredLength.ShouldBe(length);
        }

        [Fact]
        public void Dado_RequiredLengthDiferente_Quando_Equals_Entao_DeveRetornarFalse()
        {
            var a = new PasswordComplexitySetting { RequiredLength = 8 };
            var b = new PasswordComplexitySetting { RequiredLength = 12 };

            a.Equals(b).ShouldBeFalse();
        }

        [Fact]
        public void Dado_RequireLowercaseDiferente_Quando_Equals_Entao_DeveRetornarFalse()
        {
            var a = new PasswordComplexitySetting { RequireLowercase = true };
            var b = new PasswordComplexitySetting { RequireLowercase = false };

            a.Equals(b).ShouldBeFalse();
        }

        [Fact]
        public void Dado_RequireUppercaseDiferente_Quando_Equals_Entao_DeveRetornarFalse()
        {
            var a = new PasswordComplexitySetting { RequireUppercase = true };
            var b = new PasswordComplexitySetting { RequireUppercase = false };

            a.Equals(b).ShouldBeFalse();
        }

        [Fact]
        public void Dado_RequireNonAlphanumericDiferente_Quando_Equals_Entao_DeveRetornarFalse()
        {
            var a = new PasswordComplexitySetting { RequireNonAlphanumeric = true };
            var b = new PasswordComplexitySetting { RequireNonAlphanumeric = false };

            a.Equals(b).ShouldBeFalse();
        }
    }
}
