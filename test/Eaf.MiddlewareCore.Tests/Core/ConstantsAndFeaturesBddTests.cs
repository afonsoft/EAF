using Eaf.Middleware;
using Eaf.Middleware.Debugging;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Core
{
    /// <summary>
    /// Testes BDD para constantes, features e helpers do Core
    /// </summary>
    public class ConstantsAndFeaturesBddTests
    {
        #region MiddlewareCoreConsts

        [Fact]
        public void Dado_MiddlewareCoreConsts_Quando_VerificarDefaultPassPhrase_Entao_DeveSerCorreto()
        {
            MiddlewareCoreConsts.DefaultPassPhrase.ShouldBe("gsKxGZ012HLL3MI5");
        }

        [Fact]
        public void Dado_MiddlewareCoreConsts_Quando_VerificarSecurityStampKey_Entao_DeveSerCorreto()
        {
            MiddlewareCoreConsts.SecurityStampKey.ShouldBe("AspNet.Identity.SecurityStamp");
        }

        [Fact]
        public void Dado_MiddlewareCoreConsts_Quando_VerificarTokenValidityKey_Entao_DeveSerCorreto()
        {
            MiddlewareCoreConsts.TokenValidityKey.ShouldBe("token_validity_key");
        }

        [Fact]
        public void Dado_MiddlewareCoreConsts_Quando_VerificarTokenValidityValue_Entao_DeveSerCorreto()
        {
            MiddlewareCoreConsts.TokenValidityValue.ShouldBe("token_validity_value");
        }

        [Fact]
        public void Dado_MiddlewareCoreConsts_Quando_VerificarUserIdentifier_Entao_DeveSerCorreto()
        {
            MiddlewareCoreConsts.UserIdentifier.ShouldBe("user_identifier");
        }

        #endregion

        #region AppFeatures

        [Fact]
        public void Dado_AppFeatures_Quando_VerificarChatFeature_Entao_DeveSerCorreto()
        {
            AppFeatures.ChatFeature.ShouldBe("App.ChatFeature");
        }

        [Fact]
        public void Dado_AppFeatures_Quando_VerificarTenantToHostChat_Entao_DeveSerCorreto()
        {
            AppFeatures.TenantToHostChatFeature.ShouldBe("App.ChatFeature.TenantToHost");
        }

        [Fact]
        public void Dado_AppFeatures_Quando_VerificarTenantToTenantChat_Entao_DeveSerCorreto()
        {
            AppFeatures.TenantToTenantChatFeature.ShouldBe("App.ChatFeature.TenantToTenant");
        }

        [Fact]
        public void Dado_AppFeatures_Quando_VerificarGroupChat_Entao_DeveSerCorreto()
        {
            AppFeatures.GroupChatFeature.ShouldBe("App.ChatFeature.GroupChat");
        }

        #endregion

        #region EafWebHookNames

        [Fact]
        public void Dado_EafWebHookNames_Quando_VerificarNewUserRegistered_Entao_DeveSerCorreto()
        {
            EafWebHookNames.NewUserRegistered.ShouldBe("WebHook.NewUserRegistered");
        }

        #endregion

        #region DebugHelper

        [Fact]
        public void Dado_DebugHelper_Quando_VerificarIsDebug_Entao_DeveRetornarBoolean()
        {
            var isDebug = DebugHelper.IsDebug;
            isDebug.ShouldBeOneOf(true, false);
        }

        #endregion

        #region AppVersionHelper

        [Fact]
        public void Dado_AppVersionHelper_Quando_ObterVersion_Entao_NaoDeveSerNulo()
        {
            var version = AppVersionHelper.Version;
            version.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void Dado_AppVersionHelper_Quando_ObterReleaseDate_Entao_DeveSerDataValida()
        {
            var date = AppVersionHelper.ReleaseDate;
            date.ShouldBeGreaterThan(new System.DateTime(2020, 1, 1));
        }

        #endregion
    }
}
