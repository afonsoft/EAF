using Abp.MultiTenancy;
using Abp.Zero.Configuration;
using Eaf.Middleware.Authorization.Roles;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Roles
{
    /// <summary>
    /// Testes BDD para AppRoleConfig seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class AppRoleConfigBddTests
    {
        #region Configure

        [Fact]
        public void Dado_RoleManagementConfig_Quando_Configure_Entao_DeveAdicionarQuatroRoles()
        {
            // Dado
            var config = Substitute.For<IRoleManagementConfig>();
            config.StaticRoles.Returns(new List<StaticRoleDefinition>());

            // Quando
            AppRoleConfig.Configure(config);

            // Entao
            config.StaticRoles.Count.ShouldBe(4);
        }

        [Fact]
        public void Dado_RoleManagementConfig_Quando_Configure_Entao_DeveConterHostAdmin()
        {
            // Dado
            var config = Substitute.For<IRoleManagementConfig>();
            config.StaticRoles.Returns(new List<StaticRoleDefinition>());

            // Quando
            AppRoleConfig.Configure(config);

            // Entao
            config.StaticRoles.ShouldContain(r => r.RoleName == StaticRoleNames.Host.Admin && r.Side == MultiTenancySides.Host);
        }

        [Fact]
        public void Dado_RoleManagementConfig_Quando_Configure_Entao_DeveConterHostUser()
        {
            var config = Substitute.For<IRoleManagementConfig>();
            config.StaticRoles.Returns(new List<StaticRoleDefinition>());
            AppRoleConfig.Configure(config);
            config.StaticRoles.ShouldContain(r => r.RoleName == StaticRoleNames.Host.User && r.Side == MultiTenancySides.Host);
        }

        [Fact]
        public void Dado_RoleManagementConfig_Quando_Configure_Entao_DeveConterTenantAdmin()
        {
            var config = Substitute.For<IRoleManagementConfig>();
            config.StaticRoles.Returns(new List<StaticRoleDefinition>());
            AppRoleConfig.Configure(config);
            config.StaticRoles.ShouldContain(r => r.RoleName == StaticRoleNames.Tenants.Admin && r.Side == MultiTenancySides.Tenant);
        }

        [Fact]
        public void Dado_RoleManagementConfig_Quando_Configure_Entao_DeveConterTenantUser()
        {
            var config = Substitute.For<IRoleManagementConfig>();
            config.StaticRoles.Returns(new List<StaticRoleDefinition>());
            AppRoleConfig.Configure(config);
            config.StaticRoles.ShouldContain(r => r.RoleName == StaticRoleNames.Tenants.User && r.Side == MultiTenancySides.Tenant);
        }

        [Fact]
        public void Dado_RoleManagementConfig_Quando_Configure_Entao_HostAdminDeveGrantAllPermissions()
        {
            var config = Substitute.For<IRoleManagementConfig>();
            config.StaticRoles.Returns(new List<StaticRoleDefinition>());
            AppRoleConfig.Configure(config);
            config.StaticRoles.ShouldContain(r => r.RoleName == StaticRoleNames.Host.Admin && r.GrantAllPermissionsByDefault);
        }

        [Fact]
        public void Dado_RoleManagementConfig_Quando_Configure_Entao_HostUserNaoDeveGrantAllPermissions()
        {
            var config = Substitute.For<IRoleManagementConfig>();
            config.StaticRoles.Returns(new List<StaticRoleDefinition>());
            AppRoleConfig.Configure(config);
            config.StaticRoles.ShouldContain(r => r.RoleName == StaticRoleNames.Host.User && !r.GrantAllPermissionsByDefault);
        }

        [Fact]
        public void Dado_RoleManagementConfig_Quando_Configure_Entao_TenantAdminDeveGrantAllPermissions()
        {
            var config = Substitute.For<IRoleManagementConfig>();
            config.StaticRoles.Returns(new List<StaticRoleDefinition>());
            AppRoleConfig.Configure(config);
            config.StaticRoles.ShouldContain(r => r.RoleName == StaticRoleNames.Tenants.Admin && r.GrantAllPermissionsByDefault);
        }

        [Fact]
        public void Dado_RoleManagementConfig_Quando_Configure_Entao_TenantUserNaoDeveGrantAllPermissions()
        {
            var config = Substitute.For<IRoleManagementConfig>();
            config.StaticRoles.Returns(new List<StaticRoleDefinition>());
            AppRoleConfig.Configure(config);
            config.StaticRoles.ShouldContain(r => r.RoleName == StaticRoleNames.Tenants.User && !r.GrantAllPermissionsByDefault);
        }

        #endregion
    }
}
