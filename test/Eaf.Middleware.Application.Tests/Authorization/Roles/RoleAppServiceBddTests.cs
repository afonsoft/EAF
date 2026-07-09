using Abp;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.ObjectMapping;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Zero.Configuration;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Permissions;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Roles.Dto;
using Eaf.Middleware.Authorization.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Roles
{
    /// <summary>
    /// Testes BDD para RoleAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class RoleAppServiceBddTests
    {
        private readonly RoleManager _roleManager;
        private readonly RoleAppService _sut;

        public RoleAppServiceBddTests()
        {
            var roleStore = Substitute.For<RoleStore>(
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<IRepository<Role>>(),
                Substitute.For<IRepository<RolePermissionSetting, long>>()
            );

            _roleManager = Substitute.For<RoleManager>(
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
            );
            _sut = new RoleAppService(_roleManager);
        }

        #region Construtor

        [Fact]
        public void Dado_RoleManager_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region GetRoleForEdit - Nova Role

        [Fact]
        public async Task Dado_IdNulo_Quando_GetRoleForEdit_Entao_DeveRetornarNovaRole()
        {
            // Dado
            var permissions = new List<Permission>();
            var permissionManager = Substitute.For<IPermissionManager>();
            permissionManager.GetAllPermissions().Returns(permissions);
            _sut.PermissionManager = permissionManager;

            var objectMapper = Substitute.For<Abp.ObjectMapping.IObjectMapper>();
            objectMapper.Map<List<Eaf.Middleware.Authorization.Permissions.Dto.FlatPermissionDto>>(Arg.Any<object>())
                .Returns(new List<Eaf.Middleware.Authorization.Permissions.Dto.FlatPermissionDto>());
            _sut.ObjectMapper = objectMapper;

            // Quando
            var result = await _sut.GetRoleForEdit(new NullableIdDto());

            // Então
            result.ShouldNotBeNull();
            result.Role.ShouldNotBeNull();
            result.Permissions.ShouldNotBeNull();
            result.GrantedPermissionNames.ShouldNotBeNull();
            result.GrantedPermissionNames.Count.ShouldBe(0);
        }

        #endregion

        #region GetRoleForEdit - Role Existente

        [Fact]
        public async Task Dado_RoleExistente_Quando_GetRoleForEdit_Entao_DeveRetornarRoleComPermissoes()
        {
            // Dado
            var role = new Role(1, "Admin") { Id = 1 };
            _roleManager.GetRoleByIdAsync(1).Returns(role);

            var permissions = new List<Permission>();
            var permissionManager = Substitute.For<IPermissionManager>();
            permissionManager.GetAllPermissions().Returns(permissions);
            _sut.PermissionManager = permissionManager;

            var grantedPermissions = new List<Permission>
            {
                new Permission("Pages.Admin", displayName: null)
            };
            _roleManager.GetGrantedPermissionsAsync(role).Returns(grantedPermissions);

            var objectMapper = Substitute.For<Abp.ObjectMapping.IObjectMapper>();
            objectMapper.Map<RoleEditDto>(role)
                .Returns(new RoleEditDto { Id = 1, DisplayName = "Admin" });
            objectMapper.Map<List<Eaf.Middleware.Authorization.Permissions.Dto.FlatPermissionDto>>(Arg.Any<object>())
                .Returns(new List<Eaf.Middleware.Authorization.Permissions.Dto.FlatPermissionDto>());
            _sut.ObjectMapper = objectMapper;

            // Quando
            var result = await _sut.GetRoleForEdit(new NullableIdDto { Id = 1 });

            // Então
            result.ShouldNotBeNull();
            result.Role.DisplayName.ShouldBe("Admin");
            result.GrantedPermissionNames.ShouldContain("Pages.Admin");
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_RoleManager_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region GetRoles

        [Fact]
        public async Task Dado_RolesCadastradas_Quando_GetRoles_Entao_DeveRetornarListaMapeada()
        {
            // Dado
            var role = new Role(1, "Admin") { Id = 1, Name = "Admin" };
            _roleManager.Roles.Returns(new List<Role> { role }.AsAsyncQueryable());

            _sut.ObjectMapper = CreateObjectMapper();

            var input = new GetRolesInput { Sorting = "Name" };

            // Quando
            var result = await _sut.GetRoles(input);

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
        }

        #endregion

        #region DeleteRole

        [Fact]
        public async Task Dado_RoleComUsuario_Quando_DeleteRole_Entao_DeveRemoverUsuariosERole()
        {
            // Dado
            var role = new Role(1, "Admin") { Id = 1, Name = "Admin" };
            var user = new User { Id = 1, UserName = "admin" };

            _roleManager.GetRoleByIdAsync(1).Returns(role);

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUsersInRoleAsync(role.Name).Returns(new List<User> { user });
            userManager.RemoveFromRoleAsync(user, role.Name).Returns(IdentityResult.Success);

            _sut.UserManager = userManager;
            _roleManager.DeleteAsync(role).Returns(IdentityResult.Success);

            // Quando
            await _sut.DeleteRole(new EntityDto(1));

            // Então
            await userManager.Received(1).RemoveFromRoleAsync(user, role.Name);
            await _roleManager.Received(1).DeleteAsync(role);
        }

        #endregion

        #region CreateOrUpdateRole

        [Fact]
        public async Task Dado_NovaRole_Quando_CreateOrUpdateRole_Entao_DeveCriarRoleComPermissoes()
        {
            // Dado
            var permissionManager = Substitute.For<IPermissionManager>();
            permissionManager.GetPermissionOrNull(Arg.Any<string>()).Returns(new Permission("Pages.Test", displayName: null));

            var unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.SaveChangesAsync().Returns(Task.CompletedTask);
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(unitOfWork);

            _roleManager.CreateAsync(Arg.Any<Role>()).Returns(IdentityResult.Success);
            _roleManager.SetGrantedPermissionsAsync(Arg.Any<Role>(), Arg.Any<IEnumerable<Permission>>()).Returns(Task.CompletedTask);

            _sut.PermissionManager = permissionManager;
            _sut.UnitOfWorkManager = unitOfWorkManager;
            _sut.AbpSession = CreateAbpSession();

            // Quando
            await _sut.CreateOrUpdateRole(new CreateOrUpdateRoleInput
            {
                Role = new RoleEditDto { DisplayName = "Admin", IsDefault = false },
                GrantedPermissionNames = new List<string>()
            });

            // Então
            await _roleManager.Received(1).CreateAsync(Arg.Any<Role>());
            await _roleManager.Received(1).SetGrantedPermissionsAsync(Arg.Any<Role>(), Arg.Any<IEnumerable<Permission>>());
        }

        [Fact]
        public async Task Dado_RoleExistente_Quando_CreateOrUpdateRole_Entao_DeveAtualizarRoleComPermissoes()
        {
            // Dado
            var role = new Role(1, "Admin") { Id = 1, Name = "Admin" };
            _roleManager.GetRoleByIdAsync(1).Returns(role);

            var permissionManager = Substitute.For<IPermissionManager>();
            permissionManager.GetPermissionOrNull(Arg.Any<string>()).Returns(new Permission("Pages.Test", displayName: null));

            var unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.SaveChangesAsync().Returns(Task.CompletedTask);
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(unitOfWork);

            _roleManager.SetGrantedPermissionsAsync(role, Arg.Any<IEnumerable<Permission>>()).Returns(Task.CompletedTask);

            _sut.PermissionManager = permissionManager;
            _sut.UnitOfWorkManager = unitOfWorkManager;
            _sut.AbpSession = CreateAbpSession();

            // Quando
            await _sut.CreateOrUpdateRole(new CreateOrUpdateRoleInput
            {
                Role = new RoleEditDto { Id = 1, DisplayName = "Admin Updated", IsDefault = false },
                GrantedPermissionNames = new List<string>()
            });

            // Então
            role.DisplayName.ShouldBe("Admin Updated");
            await _roleManager.Received(1).SetGrantedPermissionsAsync(role, Arg.Any<IEnumerable<Permission>>());
        }

        #endregion

        #region Helpers

        private IObjectMapper CreateObjectMapper()
        {
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<RoleListDto>>(Arg.Any<object>()).Returns(ci =>
            {
                var source = ci.Arg<object>();
                var count = source is System.Collections.IEnumerable e ? e.Cast<object>().Count() : 1;
                var list = new List<RoleListDto>();
                for (int i = 0; i < count; i++)
                {
                    list.Add(new RoleListDto());
                }
                return list;
            });
            objectMapper.Map<RoleEditDto>(Arg.Any<object>()).Returns(new RoleEditDto());
            objectMapper.Map<List<Eaf.Middleware.Authorization.Permissions.Dto.FlatPermissionDto>>(Arg.Any<object>()).Returns(new List<Eaf.Middleware.Authorization.Permissions.Dto.FlatPermissionDto>());
            return objectMapper;
        }

        private IAbpSession CreateAbpSession()
        {
            var session = Substitute.For<IAbpSession>();
            session.TenantId.Returns((int?)null);
            return session;
        }

        #endregion
    }
}
