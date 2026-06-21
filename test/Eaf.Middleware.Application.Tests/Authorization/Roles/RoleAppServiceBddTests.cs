using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Zero.Configuration;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Roles.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
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
    }
}
