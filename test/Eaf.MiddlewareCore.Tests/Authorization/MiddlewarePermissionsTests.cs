using Eaf.Middleware.Authorization;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization
{
    /// <summary>
    /// Testes para MiddlewarePermissions — valida constantes de permissão
    /// </summary>
    public class MiddlewarePermissionsTests
    {
        [Fact]
        public void Dado_PermissoesPadrao_Quando_Verificar_Entao_DevemTerValoresCorretos()
        {
            MiddlewarePermissions.Pages.ShouldBe("Pages");
            MiddlewarePermissions.Pages_Administration.ShouldBe("Pages.Administration");
            MiddlewarePermissions.Pages_Dashboard.ShouldBe("Pages.Dashboard");
        }

        [Fact]
        public void Dado_PermissoesDeAuditLog_Quando_Verificar_Entao_DevemTerPrefixoCorreto()
        {
            MiddlewarePermissions.Pages_Administration_AuditLogs
                .ShouldStartWith("Pages.Administration.");
        }

        [Fact]
        public void Dado_PermissoesDeUsuarios_Quando_Verificar_Entao_DevemTerHierarquia()
        {
            MiddlewarePermissions.Pages_Administration_Users.ShouldBe("Pages.Administration.Users");
            MiddlewarePermissions.Pages_Administration_Users_Create.ShouldBe("Pages.Administration.Users.Create");
            MiddlewarePermissions.Pages_Administration_Users_Edit.ShouldBe("Pages.Administration.Users.Edit");
            MiddlewarePermissions.Pages_Administration_Users_Delete.ShouldBe("Pages.Administration.Users.Delete");
            MiddlewarePermissions.Pages_Administration_Users_ChangePermissions.ShouldBe("Pages.Administration.Users.ChangePermissions");
            MiddlewarePermissions.Pages_Administration_Users_Impersonation.ShouldBe("Pages.Administration.Users.Impersonation");
        }

        [Fact]
        public void Dado_PermissoesDeRoles_Quando_Verificar_Entao_DevemTerHierarquia()
        {
            MiddlewarePermissions.Pages_Administration_Roles.ShouldBe("Pages.Administration.Roles");
            MiddlewarePermissions.Pages_Administration_Roles_Create.ShouldBe("Pages.Administration.Roles.Create");
            MiddlewarePermissions.Pages_Administration_Roles_Edit.ShouldBe("Pages.Administration.Roles.Edit");
            MiddlewarePermissions.Pages_Administration_Roles_Delete.ShouldBe("Pages.Administration.Roles.Delete");
        }

        [Fact]
        public void Dado_PermissoesDeLanguages_Quando_Verificar_Entao_DevemTerHierarquia()
        {
            MiddlewarePermissions.Pages_Administration_Languages.ShouldBe("Pages.Administration.Languages");
            MiddlewarePermissions.Pages_Administration_Languages_Create.ShouldBe("Pages.Administration.Languages.Create");
            MiddlewarePermissions.Pages_Administration_Languages_Edit.ShouldBe("Pages.Administration.Languages.Edit");
            MiddlewarePermissions.Pages_Administration_Languages_Delete.ShouldBe("Pages.Administration.Languages.Delete");
            MiddlewarePermissions.Pages_Administration_Languages_ChangeTexts.ShouldBe("Pages.Administration.Languages.ChangeTexts");
        }

        [Fact]
        public void Dado_PermissoesDeTenants_Quando_Verificar_Entao_DevemTerHierarquia()
        {
            MiddlewarePermissions.Pages_Tenants.ShouldBe("Pages.Tenants");
            MiddlewarePermissions.Pages_Tenants_Create.ShouldBe("Pages.Tenants.Create");
            MiddlewarePermissions.Pages_Tenants_Edit.ShouldBe("Pages.Tenants.Edit");
            MiddlewarePermissions.Pages_Tenants_Delete.ShouldBe("Pages.Tenants.Delete");
            MiddlewarePermissions.Pages_Tenants_ChangeFeatures.ShouldBe("Pages.Tenants.ChangeFeatures");
            MiddlewarePermissions.Pages_Tenants_Impersonation.ShouldBe("Pages.Tenants.Impersonation");
        }

        [Fact]
        public void Dado_PermissoesAdmin_Quando_Verificar_Entao_DevemIncluirHangfireESettings()
        {
            MiddlewarePermissions.Pages_Administration_HangfireDashboard.ShouldBe("Pages.Administration.HangfireDashboard");
            MiddlewarePermissions.Pages_Administration_Settings.ShouldBe("Pages.Administration.Settings");
            MiddlewarePermissions.Pages_Administration_Maintenance.ShouldBe("Pages.Administration.Maintenance");
            MiddlewarePermissions.Pages_Administration_UiCustomization.ShouldBe("Pages.Administration.UiCustomization");
        }
    }
}
