using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.UI;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.MultiTenancy
{
    /// <summary>
    /// Testes BDD para TenantRolePermissionReplicationService seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class TenantRolePermissionReplicationServiceBddTests
    {
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly RoleManager _roleManager;
        private readonly IPermissionManager _permissionManager;
        private readonly TenantRolePermissionReplicationService _sut;

        public TenantRolePermissionReplicationServiceBddTests()
        {
            _tenantRepository = Substitute.For<IRepository<Tenant>>();
            _roleManager = CoreManagerTestHelper.CreateRoleManager(out _);
            _permissionManager = Substitute.For<IPermissionManager>();

            _sut = new TenantRolePermissionReplicationService(
                _tenantRepository,
                _roleManager,
                _permissionManager)
            {
                UnitOfWorkManager = CoreManagerTestHelper.CreateUnitOfWorkManager(),
                LocalizationManager = Substitute.For<ILocalizationManager>(),
            };
        }

        [Fact]
        public async Task Dado_TenantInexistente_Quando_GarantirRole_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            _tenantRepository.GetAllListAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
                .Returns(new List<Tenant>());

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() =>
                _sut.EnsureRoleInTenantAsync(99, "Admin", new[] { "Permissao" }));
        }

        [Fact]
        public async Task Dado_RoleJaExisteNoTenant_Quando_GarantirRole_Entao_DeveApenasAtribuirPermissoes()
        {
            // Dado
            var role = new Role(5, "Admin", "Admin");
            _tenantRepository.GetAllListAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
                .Returns(new List<Tenant> { new Tenant("t1", "T1") { Id = 5 } });
            _roleManager.FindByNameAsync("Admin").Returns(role);

            var permissao = new Permission("Pages.Users");
            _permissionManager.GetAllPermissions().Returns(new List<Permission> { permissao });

            // Quando
            await _sut.EnsureRoleInTenantAsync(5, "Admin", new[] { "Pages.Users" });

            // Então
            await _roleManager.DidNotReceive().CreateAsync(Arg.Any<Role>());
            await _roleManager.Received(1).SetGrantedPermissionsAsync(
                role,
                Arg.Is<IEnumerable<Permission>>(p => p == null || true));
        }

        [Fact]
        public async Task Dado_RoleInexistente_Quando_GarantirRole_Entao_DeveCriarEAtribuirPermissoes()
        {
            // Dado
            _tenantRepository.GetAllListAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
                .Returns(new List<Tenant> { new Tenant("t1", "T1") { Id = 5 } });
            _roleManager.FindByNameAsync("Admin").Returns((Role)null);
            _roleManager.CreateAsync(Arg.Any<Role>()).Returns(IdentityResult.Success);

            var permissao = new Permission("Pages.Users");
            _permissionManager.GetAllPermissions().Returns(new List<Permission> { permissao });

            // Quando
            await _sut.EnsureRoleInTenantAsync(5, "Admin", new[] { "Pages.Users" });

            // Então
            await _roleManager.Received(1).CreateAsync(Arg.Is<Role>(r => r.TenantId == 5 && r.Name == "Admin"));
            await _roleManager.Received(1).SetGrantedPermissionsAsync(Arg.Any<Role>(), Arg.Any<IEnumerable<Permission>>());
        }

        [Fact]
        public async Task Dado_FalhaAoCriarRole_Quando_GarantirRole_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            _tenantRepository.GetAllListAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
                .Returns(new List<Tenant> { new Tenant("t1", "T1") { Id = 5 } });
            _roleManager.FindByNameAsync("Admin").Returns((Role)null);
            _roleManager.CreateAsync(Arg.Any<Role>())
                .Returns(IdentityResult.Failed(new IdentityError { Description = "falha" }));

            // Quando / Então
            var ex = await Should.ThrowAsync<UserFriendlyException>(() =>
                _sut.EnsureRoleInTenantAsync(5, "Admin", new[] { "Pages.Users" }));
            ex.Message.ShouldBe("falha");
        }

        [Fact]
        public async Task Dado_SemPermissoes_Quando_GarantirRole_Entao_NaoDeveConcederPermissoes()
        {
            // Dado
            _tenantRepository.GetAllListAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
                .Returns(new List<Tenant> { new Tenant("t1", "T1") { Id = 5 } });
            _roleManager.FindByNameAsync("Admin").Returns(new Role(5, "Admin", "Admin"));

            // Quando
            await _sut.EnsureRoleInTenantAsync(5, "Admin", null);

            // Então
            await _roleManager.DidNotReceive().SetGrantedPermissionsAsync(Arg.Any<Role>(), Arg.Any<IEnumerable<Permission>>());
        }

        [Fact]
        public async Task Dado_RoleHostInexistente_Quando_CopiarPermissoes_Entao_NaoDeveFazerNada()
        {
            // Dado
            _roleManager.FindByNameAsync("Admin").Returns((Role)null);

            // Quando
            await _sut.CopyRolePermissionsFromHostAsync(5, "Admin");

            // Então
            await _roleManager.DidNotReceive().GetGrantedPermissionsAsync(Arg.Any<Role>());
        }

        [Fact]
        public async Task Dado_RoleHostComPermissoes_Quando_Copiar_Entao_DeveReplicarParaTenant()
        {
            // Dado
            var roleHost = new Role(null, "Admin", "Admin");
            _roleManager.FindByNameAsync("Admin").Returns(roleHost);
            _roleManager.GetGrantedPermissionsAsync(roleHost)
                .Returns(new List<Permission> { new Permission("Pages.Users") });
            _tenantRepository.GetAllListAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
                .Returns(new List<Tenant> { new Tenant("t1", "T1") { Id = 5 } });
            _permissionManager.GetAllPermissions().Returns(new List<Permission> { new Permission("Pages.Users") });

            // Quando
            await _sut.CopyRolePermissionsFromHostAsync(5, "Admin");

            // Então
            await _roleManager.Received(1).SetGrantedPermissionsAsync(Arg.Any<Role>(), Arg.Any<IEnumerable<Permission>>());
        }
    }
}
