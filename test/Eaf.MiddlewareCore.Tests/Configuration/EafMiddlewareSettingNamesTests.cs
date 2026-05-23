using Eaf.Middleware.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Configuration
{
    public class EafMiddlewareSettingNamesTests
    {
        [Fact]
        public void Dado_GoogleSettings_Quando_VerificarAnalytics_Entao_DeveSerCorreto()
        {
            EafMiddlewareSettingNames.Google.Analytics.ShouldBe("Eaf.Middleware.Google.Analytics");
        }

        [Fact]
        public void Dado_GoogleSettings_Quando_VerificarRecaptchaSiteKey_Entao_DeveSerCorreto()
        {
            EafMiddlewareSettingNames.Google.RecaptchaSiteKey.ShouldBe("Eaf.Middleware.Google.RecaptchaSiteKey");
        }

        [Fact]
        public void Dado_GoogleSettings_Quando_VerificarTagManager_Entao_DeveSerCorreto()
        {
            EafMiddlewareSettingNames.Google.TagManager.ShouldBe("Eaf.Middleware.Google.TagManager");
        }

        [Fact]
        public void Dado_UserManagement_Quando_VerificarIsRegisterRequired_Entao_DeveSerCorreto()
        {
            EafMiddlewareSettingNames.UserManagement.IsRegisterRequiredForLogin
                .ShouldBe("Eaf.Middleware.UserManagement.IsRegisterRequiredForLogin");
        }

        [Fact]
        public void Dado_LogDeleter_Quando_VerificarIsEnabled_Entao_DeveSerCorreto()
        {
            EafMiddlewareSettingNames.LogDeleter.IsEnabled.ShouldBe("Eaf.ExpiredEntity.LogDeleter.IsEnabled");
        }

        [Fact]
        public void Dado_LogDeleter_Quando_VerificarDeletedQuantity_Entao_DeveSerCorreto()
        {
            EafMiddlewareSettingNames.LogDeleter.DeletedQuantity.ShouldBe("Eaf.ExpiredEntity.LogDeleter.DeletedQuantity");
        }

        [Fact]
        public void Dado_LogDeleter_Quando_VerificarExpiredDays_Entao_DeveSerCorreto()
        {
            EafMiddlewareSettingNames.LogDeleter.ExpiredDays.ShouldBe("Eaf.ExpiredEntity.LogDeleter.ExpiredDays");
        }

        [Fact]
        public void Dado_LoginImpersonator_Quando_VerificarIsEnabled_Entao_DeveSerCorreto()
        {
            EafMiddlewareSettingNames.LoginImpersonator.IsEnabled.ShouldBe("Eaf.ExpiredEntity.LoginImpersonator.IsEnabled");
        }
    }
}
