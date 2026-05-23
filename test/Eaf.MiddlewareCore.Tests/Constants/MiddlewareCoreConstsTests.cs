using Eaf.Middleware;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Constants
{
    public class MiddlewareCoreConstsTests
    {
        [Fact]
        public void Dado_DefaultPassPhrase_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MiddlewareCoreConsts.DefaultPassPhrase.ShouldBe("gsKxGZ012HLL3MI5");
        }

        [Fact]
        public void Dado_SecurityStampKey_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MiddlewareCoreConsts.SecurityStampKey.ShouldBe("AspNet.Identity.SecurityStamp");
        }

        [Fact]
        public void Dado_TokenValidityKey_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MiddlewareCoreConsts.TokenValidityKey.ShouldBe("token_validity_key");
        }

        [Fact]
        public void Dado_TokenValidityValue_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MiddlewareCoreConsts.TokenValidityValue.ShouldBe("token_validity_value");
        }

        [Fact]
        public void Dado_UserIdentifier_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MiddlewareCoreConsts.UserIdentifier.ShouldBe("user_identifier");
        }
    }

    public class EafWebHookNamesTests
    {
        [Fact]
        public void Dado_NewUserRegistered_Quando_Verificar_Entao_DeveSerCorreto()
        {
            EafWebHookNames.NewUserRegistered.ShouldBe("WebHook.NewUserRegistered");
        }
    }

    public class MiddlewarePermissionsTests
    {
        [Fact]
        public void Dado_Pages_Quando_Verificar_Entao_DeveSerCorreto()
        {
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages.ShouldBe("Pages");
        }

        [Fact]
        public void Dado_PagesAdministration_Quando_Verificar_Entao_DeveSerCorreto()
        {
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration.ShouldBe("Pages.Administration");
        }

        [Fact]
        public void Dado_PagesAdministrationRoles_Quando_Verificar_Entao_DeveSerCorreto()
        {
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Roles.ShouldBe("Pages.Administration.Roles");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Roles_Create.ShouldBe("Pages.Administration.Roles.Create");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Roles_Edit.ShouldBe("Pages.Administration.Roles.Edit");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Roles_Delete.ShouldBe("Pages.Administration.Roles.Delete");
        }

        [Fact]
        public void Dado_PagesAdministrationUsers_Quando_Verificar_Entao_DeveSerCorreto()
        {
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Users.ShouldBe("Pages.Administration.Users");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Users_Create.ShouldBe("Pages.Administration.Users.Create");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Users_Edit.ShouldBe("Pages.Administration.Users.Edit");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Users_Delete.ShouldBe("Pages.Administration.Users.Delete");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Users_ChangePermissions.ShouldBe("Pages.Administration.Users.ChangePermissions");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Users_Impersonation.ShouldBe("Pages.Administration.Users.Impersonation");
        }

        [Fact]
        public void Dado_PagesTenants_Quando_Verificar_Entao_DeveSerCorreto()
        {
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Tenants.ShouldBe("Pages.Tenants");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Tenants_Create.ShouldBe("Pages.Tenants.Create");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Tenants_Edit.ShouldBe("Pages.Tenants.Edit");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Tenants_Delete.ShouldBe("Pages.Tenants.Delete");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Tenants_ChangeFeatures.ShouldBe("Pages.Tenants.ChangeFeatures");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Tenants_Impersonation.ShouldBe("Pages.Tenants.Impersonation");
        }

        [Fact]
        public void Dado_PagesAdministrationLanguages_Quando_Verificar_Entao_DeveSerCorreto()
        {
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Languages.ShouldBe("Pages.Administration.Languages");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Languages_Create.ShouldBe("Pages.Administration.Languages.Create");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Languages_Edit.ShouldBe("Pages.Administration.Languages.Edit");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Languages_Delete.ShouldBe("Pages.Administration.Languages.Delete");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Languages_ChangeTexts.ShouldBe("Pages.Administration.Languages.ChangeTexts");
        }

        [Fact]
        public void Dado_PagesAdministrationOutras_Quando_Verificar_Entao_DeveSerCorreto()
        {
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_AuditLogs.ShouldBe("Pages.Administration.AuditLogs");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_HangfireDashboard.ShouldBe("Pages.Administration.HangfireDashboard");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Maintenance.ShouldBe("Pages.Administration.Maintenance");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_Settings.ShouldBe("Pages.Administration.Settings");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Administration_UiCustomization.ShouldBe("Pages.Administration.UiCustomization");
            Eaf.Middleware.Authorization.MiddlewarePermissions.Pages_Dashboard.ShouldBe("Pages.Dashboard");
        }
    }
}
