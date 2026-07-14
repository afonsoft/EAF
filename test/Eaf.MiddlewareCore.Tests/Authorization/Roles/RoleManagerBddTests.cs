using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.UI;
using Abp.Zero.Configuration;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Roles
{
    /// <summary>
    /// Testes BDD para RoleManager seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class RoleManagerBddTests
    {
        [Fact]
        public async Task Dado_RoleAdminSemPermissoesObrigatorias_Quando_SetGrantedPermissionsAsync_Entao_DeveLancarExcecao()
        {
            // Dado
            var roleStore = Substitute.For<RoleStore>(
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<IRepository<Role>>(),
                Substitute.For<IRepository<RolePermissionSetting, long>>()
            );

            var localizationManager = Substitute.For<ILocalizationManager>();
            var localizationSource = Substitute.For<ILocalizationSource>();
            localizationSource.GetString(Arg.Any<string>()).Returns("test");
            localizationManager.GetSource("EafCore").Returns(localizationSource);

            var roleManager = Substitute.For<RoleManager>(new object[]
            {
                roleStore,
                new List<IRoleValidator<Role>>(),
                Substitute.For<ILookupNormalizer>(),
                new IdentityErrorDescriber(),
                Substitute.For<ILogger<RoleManager>>(),
                Substitute.For<IPermissionManager>(),
                Substitute.For<IRoleManagementConfig>(),
                Substitute.For<ICacheManager>(),
                Substitute.For<IUnitOfWorkManager>(),
                localizationManager,
                Substitute.For<IRepository<OrganizationUnit, long>>(),
                Substitute.For<IRepository<OrganizationUnitRole, long>>()
            });

            roleManager.When(x => x.SetGrantedPermissionsAsync(
                Arg.Any<Role>(),
                Arg.Any<IEnumerable<Permission>>()
            )).CallBase();

            var adminRole = new Role(null, "Admin", "Admin");
            var permissions = new List<Permission>
            {
                new Permission(MiddlewarePermissions.Pages_Administration_Roles_Edit, displayName: null)
            };

            // Quando/Então
            var exception = await Should.ThrowAsync<UserFriendlyException>(() =>
                roleManager.SetGrantedPermissionsAsync(adminRole, permissions)
            );

            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_RoleAdminComPermissoesObrigatorias_Quando_SetGrantedPermissionsAsync_Entao_DevePermitir()
        {
            // Dado
            var roleStore = Substitute.For<RoleStore>(
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<IRepository<Role>>(),
                Substitute.For<IRepository<RolePermissionSetting, long>>()
            );

            var localizationManager = Substitute.For<ILocalizationManager>();
            var localizationSource = Substitute.For<ILocalizationSource>();
            localizationSource.GetString(Arg.Any<string>()).Returns("test");
            localizationManager.GetSource("EafCore").Returns(localizationSource);

            var roleManager = Substitute.For<RoleManager>(new object[]
            {
                roleStore,
                new List<IRoleValidator<Role>>(),
                Substitute.For<ILookupNormalizer>(),
                new IdentityErrorDescriber(),
                Substitute.For<ILogger<RoleManager>>(),
                Substitute.For<IPermissionManager>(),
                Substitute.For<IRoleManagementConfig>(),
                Substitute.For<ICacheManager>(),
                Substitute.For<IUnitOfWorkManager>(),
                localizationManager,
                Substitute.For<IRepository<OrganizationUnit, long>>(),
                Substitute.For<IRepository<OrganizationUnitRole, long>>()
            });

            roleManager.When(x => x.SetGrantedPermissionsAsync(
                Arg.Any<Role>(),
                Arg.Any<IEnumerable<Permission>>()
            )).CallBase();

            var adminRole = new Role(null, "Admin", "Admin");
            var permissions = new List<Permission>
            {
                new Permission(MiddlewarePermissions.Pages_Administration_Roles_Edit, displayName: null),
                new Permission(MiddlewarePermissions.Pages_Administration_Users_ChangePermissions, displayName: null)
            };

            // Quando
            await roleManager.SetGrantedPermissionsAsync(adminRole, permissions);

            // Então
            true.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_RoleAdminApenasComPermissaoUsuario_Quando_SetGrantedPermissionsAsync_Entao_DeveLancarExcecao()
        {
            var roleStore = Substitute.For<RoleStore>(
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<IRepository<Role>>(),
                Substitute.For<IRepository<RolePermissionSetting, long>>()
            );

            var localizationManager = Substitute.For<ILocalizationManager>();
            var localizationSource = Substitute.For<ILocalizationSource>();
            localizationSource.GetString(Arg.Any<string>()).Returns("test");
            localizationManager.GetSource("EafCore").Returns(localizationSource);

            var roleManager = Substitute.For<RoleManager>(new object[]
            {
                roleStore,
                new List<IRoleValidator<Role>>(),
                Substitute.For<ILookupNormalizer>(),
                new IdentityErrorDescriber(),
                Substitute.For<ILogger<RoleManager>>(),
                Substitute.For<IPermissionManager>(),
                Substitute.For<IRoleManagementConfig>(),
                Substitute.For<ICacheManager>(),
                Substitute.For<IUnitOfWorkManager>(),
                localizationManager,
                Substitute.For<IRepository<OrganizationUnit, long>>(),
                Substitute.For<IRepository<OrganizationUnitRole, long>>()
            });

            roleManager.When(x => x.SetGrantedPermissionsAsync(
                Arg.Any<Role>(),
                Arg.Any<IEnumerable<Permission>>()
            )).CallBase();

            var adminRole = new Role(null, "Admin", "Admin");
            var permissions = new List<Permission>
            {
                new Permission(MiddlewarePermissions.Pages_Administration_Users_ChangePermissions, displayName: null)
            };

            var exception = await Should.ThrowAsync<UserFriendlyException>(() =>
                roleManager.SetGrantedPermissionsAsync(adminRole, permissions)
            );

            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_RoleNaoAdmin_Quando_SetGrantedPermissionsAsync_Entao_DevePermitirSemValidacao()
        {
            // Dado
            var roleStore = Substitute.For<RoleStore>(
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<IRepository<Role>>(),
                Substitute.For<IRepository<RolePermissionSetting, long>>()
            );

            var roleManager = Substitute.For<RoleManager>(new object[]
            {
                roleStore,
                new List<IRoleValidator<Role>>(),
                Substitute.For<ILookupNormalizer>(),
                new IdentityErrorDescriber(),
                Substitute.For<ILogger<RoleManager>>(),
                Substitute.For<IPermissionManager>(),
                Substitute.For<IRoleManagementConfig>(),
                Substitute.For<ICacheManager>(),
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<ILocalizationManager>(),
                Substitute.For<IRepository<OrganizationUnit, long>>(),
                Substitute.For<IRepository<OrganizationUnitRole, long>>()
            });

            roleManager.When(x => x.SetGrantedPermissionsAsync(
                Arg.Any<Role>(),
                Arg.Any<IEnumerable<Permission>>()
            )).CallBase();

            var userRole = new Role(null, "User", "User");
            var permissions = new List<Permission>
            {
                new Permission(MiddlewarePermissions.Pages_Administration_Users, displayName: null)
            };

            // Quando
            await roleManager.SetGrantedPermissionsAsync(userRole, permissions);

            // Então
            true.ShouldBeTrue();
        }
    }
}
