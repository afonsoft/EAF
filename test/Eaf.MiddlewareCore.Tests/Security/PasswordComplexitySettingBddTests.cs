using Eaf.Middleware.Security;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Security
{
    /// <summary>
    /// Testes BDD para PasswordComplexitySetting seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class PasswordComplexitySettingBddTests
    {
        [Fact]
        public void Dado_DoisSettingsIguais_Quando_Comparar_Entao_DeveRetornarTrue()
        {
            // Dado
            var setting1 = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonAlphanumeric = true,
                RequiredLength = 8
            };
            var setting2 = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonAlphanumeric = true,
                RequiredLength = 8
            };

            // Quando & Então
            setting1.Equals(setting2).ShouldBeTrue();
        }

        [Fact]
        public void Dado_DoisSettingsDiferentes_Quando_Comparar_Entao_DeveRetornarFalse()
        {
            // Dado
            var setting1 = new PasswordComplexitySetting { RequireDigit = true, RequiredLength = 8 };
            var setting2 = new PasswordComplexitySetting { RequireDigit = false, RequiredLength = 8 };

            // Quando & Então
            setting1.Equals(setting2).ShouldBeFalse();
        }

        [Fact]
        public void Dado_SettingComparadoComNull_Quando_Comparar_Entao_DeveRetornarFalse()
        {
            // Dado
            var setting = new PasswordComplexitySetting { RequiredLength = 6 };

            // Quando & Então
            setting.Equals(null).ShouldBeFalse();
        }

        [Fact]
        public void Dado_SettingComRequiredLengthDiferente_Quando_Comparar_Entao_DeveRetornarFalse()
        {
            // Dado
            var setting1 = new PasswordComplexitySetting { RequiredLength = 6 };
            var setting2 = new PasswordComplexitySetting { RequiredLength = 10 };

            // Quando & Então
            setting1.Equals(setting2).ShouldBeFalse();
        }

        [Fact]
        public void Dado_NovoSetting_Quando_Criar_Entao_DeveTerValoresPadrao()
        {
            // Dado & Quando
            var setting = new PasswordComplexitySetting();

            // Então
            setting.RequireDigit.ShouldBeFalse();
            setting.RequireLowercase.ShouldBeFalse();
            setting.RequireUppercase.ShouldBeFalse();
            setting.RequireNonAlphanumeric.ShouldBeFalse();
            setting.RequiredLength.ShouldBe(0);
        }
    }
}
