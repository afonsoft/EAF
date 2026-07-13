using Eaf.Middleware.Security;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Security
{
    /// <summary>
    /// Testes BDD para PasswordComplexitySetting seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class PasswordComplexitySettingBddTests
    {
        #region Propriedades

        [Fact]
        public void Dado_PasswordComplexitySetting_Quando_DefinirRequireDigit_Entao_DeveArmazenarCorretamente()
        {
            var setting = new PasswordComplexitySetting { RequireDigit = true };
            setting.RequireDigit.ShouldBeTrue();
        }

        [Fact]
        public void Dado_PasswordComplexitySetting_Quando_DefinirRequiredLength_Entao_DeveArmazenarCorretamente()
        {
            var setting = new PasswordComplexitySetting { RequiredLength = 12 };
            setting.RequiredLength.ShouldBe(12);
        }

        [Fact]
        public void Dado_PasswordComplexitySetting_Quando_DefinirRequireLowercase_Entao_DeveArmazenarCorretamente()
        {
            var setting = new PasswordComplexitySetting { RequireLowercase = true };
            setting.RequireLowercase.ShouldBeTrue();
        }

        [Fact]
        public void Dado_PasswordComplexitySetting_Quando_DefinirRequireNonAlphanumeric_Entao_DeveArmazenarCorretamente()
        {
            var setting = new PasswordComplexitySetting { RequireNonAlphanumeric = true };
            setting.RequireNonAlphanumeric.ShouldBeTrue();
        }

        [Fact]
        public void Dado_PasswordComplexitySetting_Quando_DefinirRequireUppercase_Entao_DeveArmazenarCorretamente()
        {
            var setting = new PasswordComplexitySetting { RequireUppercase = true };
            setting.RequireUppercase.ShouldBeTrue();
        }

        #endregion

        #region Equals

        [Fact]
        public void Dado_DoisSettingsIguais_Quando_Equals_Entao_DeveRetornarTrue()
        {
            var setting1 = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = false,
                RequireUppercase = true,
                RequiredLength = 8
            };
            var setting2 = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = false,
                RequireUppercase = true,
                RequiredLength = 8
            };
            setting1.Equals(setting2).ShouldBeTrue();
        }

        [Fact]
        public void Dado_DoisSettingsDiferentes_Quando_Equals_Entao_DeveRetornarFalse()
        {
            var setting1 = new PasswordComplexitySetting { RequireDigit = true, RequiredLength = 8 };
            var setting2 = new PasswordComplexitySetting { RequireDigit = false, RequiredLength = 8 };
            setting1.Equals(setting2).ShouldBeFalse();
        }

        [Fact]
        public void Dado_SettingComparadoComNull_Quando_Equals_Entao_DeveRetornarFalse()
        {
            var setting = new PasswordComplexitySetting();
            setting.Equals(null).ShouldBeFalse();
        }

        [Fact]
        public void Dado_DoisSettingsComRequiredLengthDiferente_Quando_Equals_Entao_DeveRetornarFalse()
        {
            var setting1 = new PasswordComplexitySetting { RequiredLength = 8 };
            var setting2 = new PasswordComplexitySetting { RequiredLength = 12 };
            setting1.Equals(setting2).ShouldBeFalse();
        }

        [Fact]
        public void Dado_SettingComparadoComOutroTipo_Quando_Equals_Entao_DeveRetornarFalse()
        {
            var setting = new PasswordComplexitySetting { RequireDigit = true };
            setting.Equals("not-a-setting").ShouldBeFalse();
        }

        [Fact]
        public void Dado_DoisSettingsIguais_Quando_GetHashCode_Entao_DeveRetornarMesmoValor()
        {
            var setting1 = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = false,
                RequireUppercase = true,
                RequiredLength = 8
            };
            var setting2 = new PasswordComplexitySetting
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = false,
                RequireUppercase = true,
                RequiredLength = 8
            };
            setting1.GetHashCode().ShouldBe(setting2.GetHashCode());
        }

        #endregion
    }
}
