using Eaf.Middleware.Security;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Net.Security
{
    /// <summary>
    /// Testes BDD para PasswordComplexitySetting seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class PasswordComplexitySettingBddTests
    {
        [Fact]
        public void Dado_DoisSettingsIguais_Quando_Equals_Entao_DeveRetornarTrue()
        {
            var a = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true,
                RequiredLength = 8
            };
            var b = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true,
                RequiredLength = 8
            };

            a.Equals(b).ShouldBeTrue();
        }

        [Fact]
        public void Dado_SettingNulo_Quando_Equals_Entao_DeveRetornarFalse()
        {
            var a = new PasswordComplexitySetting { RequireDigit = true };
            a.Equals(null).ShouldBeFalse();
        }

        [Fact]
        public void Dado_RequireDigitDiferente_Quando_Equals_Entao_DeveRetornarFalse()
        {
            var a = new PasswordComplexitySetting { RequireDigit = true };
            var b = new PasswordComplexitySetting { RequireDigit = false };
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
        public void Dado_RequireNonAlphanumericDiferente_Quando_Equals_Entao_DeveRetornarFalse()
        {
            var a = new PasswordComplexitySetting { RequireNonAlphanumeric = true };
            var b = new PasswordComplexitySetting { RequireNonAlphanumeric = false };
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
        public void Dado_RequiredLengthDiferente_Quando_Equals_Entao_DeveRetornarFalse()
        {
            var a = new PasswordComplexitySetting { RequiredLength = 6 };
            var b = new PasswordComplexitySetting { RequiredLength = 10 };
            a.Equals(b).ShouldBeFalse();
        }

        [Fact]
        public void Dado_SettingPadrao_Quando_Verificar_Entao_DeveSerFalseEZero()
        {
            var setting = new PasswordComplexitySetting();
            setting.RequireDigit.ShouldBeFalse();
            setting.RequireLowercase.ShouldBeFalse();
            setting.RequireNonAlphanumeric.ShouldBeFalse();
            setting.RequireUppercase.ShouldBeFalse();
            setting.RequiredLength.ShouldBe(0);
        }
    }
}
