using Abp.Authorization;
using Eaf.Middleware.Authorization.Permissions;
using Eaf.Middleware.Authorization.Permissions.Dto;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Permissions
{
    /// <summary>
    /// Testes BDD para PermissionAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class PermissionAppServiceBddTests
    {
        private readonly PermissionAppService _sut;

        public PermissionAppServiceBddTests()
        {
            _sut = new PermissionAppService();
        }

        #region Construtor

        [Fact]
        public void Dado_NenhumParametro_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region GetAllPermissions

        [Fact]
        public void Dado_PermissoesExistentes_Quando_GetAllPermissions_Entao_DeveRetornarComNiveis()
        {
            // Dado
            var permission1 = new Permission("Pages", displayName: null);
            var permission2 = new Permission("Pages.Admin", displayName: null);

            var permissionManager = Substitute.For<IPermissionManager>();
            permissionManager.GetAllPermissions().Returns(new List<Permission> { permission1, permission2 });
            _sut.PermissionManager = permissionManager;

            var objectMapper = Substitute.For<Abp.ObjectMapping.IObjectMapper>();
            objectMapper.Map<FlatPermissionWithLevelDto>(permission1)
                .Returns(new FlatPermissionWithLevelDto { ParentName = null });
            objectMapper.Map<FlatPermissionWithLevelDto>(permission2)
                .Returns(new FlatPermissionWithLevelDto { ParentName = "Pages" });
            _sut.ObjectMapper = objectMapper;

            // Quando
            var result = _sut.GetAllPermissions();

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Dado_SemPermissoes_Quando_GetAllPermissions_Entao_DeveRetornarListaVazia()
        {
            // Dado
            var permissionManager = Substitute.For<IPermissionManager>();
            permissionManager.GetAllPermissions().Returns(new List<Permission>());
            _sut.PermissionManager = permissionManager;

            // Quando
            var result = _sut.GetAllPermissions();

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_PermissoesSemFilhos_Quando_GetAllPermissions_Entao_DeveRetornarApenasRaiz()
        {
            // Dado
            var permission1 = new Permission("Pages.Read", displayName: null);
            var permission2 = new Permission("Pages.Write", displayName: null);

            var permissionManager = Substitute.For<IPermissionManager>();
            permissionManager.GetAllPermissions().Returns(new List<Permission> { permission1, permission2 });
            _sut.PermissionManager = permissionManager;

            var objectMapper = Substitute.For<Abp.ObjectMapping.IObjectMapper>();
            objectMapper.Map<FlatPermissionWithLevelDto>(Arg.Any<Permission>())
                .Returns(ci => new FlatPermissionWithLevelDto { ParentName = null });
            _sut.ObjectMapper = objectMapper;

            // Quando
            var result = _sut.GetAllPermissions();

            // Então
            result.Items.Count.ShouldBe(2);
        }

        #endregion
    }
}
