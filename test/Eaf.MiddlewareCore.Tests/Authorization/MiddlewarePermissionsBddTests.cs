using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Roles;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization
{
    public class MiddlewarePermissionsBddTests
    {
        [Fact]
        public void Dado_MiddlewarePermissions_Quando_VerificarPages_Entao_DeveEstarCorreto()
        {
            MiddlewarePermissions.Pages.ShouldBe("Pages");
            MiddlewarePermissions.Pages_Administration.ShouldBe("Pages.Administration");
            MiddlewarePermissions.Pages_Dashboard.ShouldBe("Pages.Dashboard");
        }

        [Fact]
        public void Dado_MiddlewarePermissions_Quando_VerificarAuditLogs_Entao_DeveEstarCorreto()
        {
            MiddlewarePermissions.Pages_Administration_AuditLogs.ShouldBe("Pages.Administration.AuditLogs");
        }

        [Fact]
        public void Dado_MiddlewarePermissions_Quando_VerificarHangfire_Entao_DeveEstarCorreto()
        {
            MiddlewarePermissions.Pages_Administration_HangfireDashboard
                .ShouldBe("Pages.Administration.HangfireDashboard");
        }

        [Fact]
        public void Dado_MiddlewarePermissions_Quando_VerificarLanguages_Entao_DevemEstarCorretos()
        {
            MiddlewarePermissions.Pages_Administration_Languages.ShouldBe("Pages.Administration.Languages");
            MiddlewarePermissions.Pages_Administration_Languages_ChangeTexts.ShouldBe("Pages.Administration.Languages.ChangeTexts");
            MiddlewarePermissions.Pages_Administration_Languages_Create.ShouldBe("Pages.Administration.Languages.Create");
            MiddlewarePermissions.Pages_Administration_Languages_Delete.ShouldBe("Pages.Administration.Languages.Delete");
            MiddlewarePermissions.Pages_Administration_Languages_Edit.ShouldBe("Pages.Administration.Languages.Edit");
        }

        [Fact]
        public void Dado_MiddlewarePermissions_Quando_VerificarRoles_Entao_DevemEstarCorretos()
        {
            MiddlewarePermissions.Pages_Administration_Roles.ShouldBe("Pages.Administration.Roles");
            MiddlewarePermissions.Pages_Administration_Roles_Create.ShouldBe("Pages.Administration.Roles.Create");
            MiddlewarePermissions.Pages_Administration_Roles_Delete.ShouldBe("Pages.Administration.Roles.Delete");
            MiddlewarePermissions.Pages_Administration_Roles_Edit.ShouldBe("Pages.Administration.Roles.Edit");
        }

        [Fact]
        public void Dado_MiddlewarePermissions_Quando_VerificarUsers_Entao_DevemEstarCorretos()
        {
            MiddlewarePermissions.Pages_Administration_Users.ShouldBe("Pages.Administration.Users");
            MiddlewarePermissions.Pages_Administration_Users_ChangePermissions.ShouldBe("Pages.Administration.Users.ChangePermissions");
            MiddlewarePermissions.Pages_Administration_Users_Create.ShouldBe("Pages.Administration.Users.Create");
            MiddlewarePermissions.Pages_Administration_Users_Delete.ShouldBe("Pages.Administration.Users.Delete");
            MiddlewarePermissions.Pages_Administration_Users_Edit.ShouldBe("Pages.Administration.Users.Edit");
            MiddlewarePermissions.Pages_Administration_Users_Impersonation.ShouldBe("Pages.Administration.Users.Impersonation");
        }

        [Fact]
        public void Dado_MiddlewarePermissions_Quando_VerificarTenants_Entao_DevemEstarCorretos()
        {
            MiddlewarePermissions.Pages_Tenants.ShouldBe("Pages.Tenants");
            MiddlewarePermissions.Pages_Tenants_ChangeFeatures.ShouldBe("Pages.Tenants.ChangeFeatures");
            MiddlewarePermissions.Pages_Tenants_Create.ShouldBe("Pages.Tenants.Create");
            MiddlewarePermissions.Pages_Tenants_Delete.ShouldBe("Pages.Tenants.Delete");
            MiddlewarePermissions.Pages_Tenants_Edit.ShouldBe("Pages.Tenants.Edit");
            MiddlewarePermissions.Pages_Tenants_Impersonation.ShouldBe("Pages.Tenants.Impersonation");
        }

        [Fact]
        public void Dado_MiddlewarePermissions_Quando_VerificarOutras_Entao_DevemEstarCorretos()
        {
            MiddlewarePermissions.Pages_Administration_Maintenance.ShouldBe("Pages.Administration.Maintenance");
            MiddlewarePermissions.Pages_Administration_Settings.ShouldBe("Pages.Administration.Settings");
            MiddlewarePermissions.Pages_Administration_UiCustomization.ShouldBe("Pages.Administration.UiCustomization");
        }
    }

    public class StaticRoleNamesBddTests
    {
        [Fact]
        public void Dado_StaticRoleNames_Quando_VerificarHostRoles_Entao_DevemEstarCorretos()
        {
            StaticRoleNames.Host.Admin.ShouldBe("Admin");
            StaticRoleNames.Host.User.ShouldBe("User");
        }

        [Fact]
        public void Dado_StaticRoleNames_Quando_VerificarTenantRoles_Entao_DevemEstarCorretos()
        {
            StaticRoleNames.Tenants.Admin.ShouldBe("Admin");
            StaticRoleNames.Tenants.User.ShouldBe("User");
        }
    }

    public class MiddlewareCoreConstsBddTests
    {
        [Fact]
        public void Dado_MiddlewareCoreConsts_Quando_VerificarConstants_Entao_DevemEstarCorretos()
        {
            MiddlewareCoreConsts.DefaultPassPhrase.ShouldBe("gsKxGZ012HLL3MI5");
            MiddlewareCoreConsts.SecurityStampKey.ShouldBe("AspNet.Identity.SecurityStamp");
            MiddlewareCoreConsts.TokenValidityKey.ShouldBe("token_validity_key");
            MiddlewareCoreConsts.TokenValidityValue.ShouldBe("token_validity_value");
            MiddlewareCoreConsts.UserIdentifier.ShouldBe("user_identifier");
        }
    }

    public class EafWebHookNamesBddTests
    {
        [Fact]
        public void Dado_EafWebHookNames_Quando_VerificarNewUserRegistered_Entao_DeveEstarCorreto()
        {
            EafWebHookNames.NewUserRegistered.ShouldBe("WebHook.NewUserRegistered");
        }
    }
}
