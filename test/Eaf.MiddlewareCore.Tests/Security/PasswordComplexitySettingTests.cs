using Eaf.Middleware.Security;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Security
{
    public class PasswordComplexitySettingTests
    {
        [Fact]
        public void Dado_DoisObjetosIguais_Quando_Comparar_Entao_DeveRetornarTrue()
        {
            var setting1 = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequiredLength = 8,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            var setting2 = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequiredLength = 8,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            setting1.Equals(setting2).ShouldBeTrue();
        }

        [Fact]
        public void Dado_ObjetosDiferentes_Quando_Comparar_Entao_DeveRetornarFalse()
        {
            var setting1 = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequiredLength = 8,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            var setting2 = new PasswordComplexitySetting
            {
                RequireDigit = false,
                RequiredLength = 8,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            setting1.Equals(setting2).ShouldBeFalse();
        }

        [Fact]
        public void Dado_Null_Quando_Comparar_Entao_DeveRetornarFalse()
        {
            var setting = new PasswordComplexitySetting { RequiredLength = 6 };
            setting.Equals(null).ShouldBeFalse();
        }

        [Fact]
        public void Dado_RequiredLengthDiferente_Quando_Comparar_Entao_DeveRetornarFalse()
        {
            var setting1 = new PasswordComplexitySetting { RequiredLength = 6 };
            var setting2 = new PasswordComplexitySetting { RequiredLength = 10 };
            setting1.Equals(setting2).ShouldBeFalse();
        }

        [Fact]
        public void Dado_RequireLowercaseDiferente_Quando_Comparar_Entao_DeveRetornarFalse()
        {
            var setting1 = new PasswordComplexitySetting { RequireLowercase = true };
            var setting2 = new PasswordComplexitySetting { RequireLowercase = false };
            setting1.Equals(setting2).ShouldBeFalse();
        }

        [Fact]
        public void Dado_RequireNonAlphanumericDiferente_Quando_Comparar_Entao_DeveRetornarFalse()
        {
            var setting1 = new PasswordComplexitySetting { RequireNonAlphanumeric = true };
            var setting2 = new PasswordComplexitySetting { RequireNonAlphanumeric = false };
            setting1.Equals(setting2).ShouldBeFalse();
        }

        [Fact]
        public void Dado_RequireUppercaseDiferente_Quando_Comparar_Entao_DeveRetornarFalse()
        {
            var setting1 = new PasswordComplexitySetting { RequireUppercase = true };
            var setting2 = new PasswordComplexitySetting { RequireUppercase = false };
            setting1.Equals(setting2).ShouldBeFalse();
        }
    }
}
