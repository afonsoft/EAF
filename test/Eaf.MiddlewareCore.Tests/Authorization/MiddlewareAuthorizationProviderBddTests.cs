using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;
using Eaf.Middleware.Authorization;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Authorization
{
    /// <summary>
    /// Testes BDD para MiddlewareAuthorizationProvider seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class MiddlewareAuthorizationProviderBddTests
    {
        #region Construtores

        [Fact]
        public void Dado_MultiTenancyHabilitado_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var provider = new MiddlewareAuthorizationProvider(true);
            provider.ShouldNotBeNull();
            provider.ShouldBeAssignableTo<AuthorizationProvider>();
        }

        [Fact]
        public void Dado_MultiTenancyDesabilitado_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var provider = new MiddlewareAuthorizationProvider(false);
            provider.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_MultiTenancyConfig_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var config = Substitute.For<IMultiTenancyConfig>();
            config.IsEnabled.Returns(true);
            var provider = new MiddlewareAuthorizationProvider(config);
            provider.ShouldNotBeNull();
        }

        #endregion

        #region SetPermissions

        [Fact]
        public void Dado_ProviderComMultiTenancy_Quando_SetPermissions_Entao_DeveCriarPermissoesDePaginas()
        {
            // Dado
            var provider = new MiddlewareAuthorizationProvider(true);
            var context = Substitute.For<IPermissionDefinitionContext>();

            var pagesPermission = new Permission(
                MiddlewarePermissions.Pages,
                new FixedLocalizableString("Pages"));

            context.GetPermissionOrNull(MiddlewarePermissions.Pages).Returns((Permission)null);
            context.CreatePermission(
                MiddlewarePermissions.Pages,
                Arg.Any<ILocalizableString>(),
                Arg.Any<ILocalizableString>(),
                Arg.Any<MultiTenancySides>(),
                Arg.Any<Abp.Application.Features.IFeatureDependency>(),
                Arg.Any<System.Collections.Generic.Dictionary<string, object>>()
            ).Returns(pagesPermission);

            // Quando
            provider.SetPermissions(context);

            // Entao
            context.Received(1).CreatePermission(
                MiddlewarePermissions.Pages,
                Arg.Any<ILocalizableString>(),
                Arg.Any<ILocalizableString>(),
                Arg.Any<MultiTenancySides>(),
                Arg.Any<Abp.Application.Features.IFeatureDependency>(),
                Arg.Any<System.Collections.Generic.Dictionary<string, object>>()
            );

            pagesPermission.Children.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Dado_ProviderSemMultiTenancy_Quando_SetPermissions_Entao_DeveCriarPermissoesDePaginas()
        {
            // Dado
            var provider = new MiddlewareAuthorizationProvider(false);
            var context = Substitute.For<IPermissionDefinitionContext>();

            var pagesPermission = new Permission(
                MiddlewarePermissions.Pages,
                new FixedLocalizableString("Pages"));

            context.GetPermissionOrNull(MiddlewarePermissions.Pages).Returns((Permission)null);
            context.CreatePermission(
                MiddlewarePermissions.Pages,
                Arg.Any<ILocalizableString>(),
                Arg.Any<ILocalizableString>(),
                Arg.Any<MultiTenancySides>(),
                Arg.Any<Abp.Application.Features.IFeatureDependency>(),
                Arg.Any<System.Collections.Generic.Dictionary<string, object>>()
            ).Returns(pagesPermission);

            // Quando
            provider.SetPermissions(context);

            // Entao
            pagesPermission.Children.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Dado_PermissaoPagesJaExistente_Quando_SetPermissions_Entao_DeveReutilizarPermissaoExistente()
        {
            // Dado
            var provider = new MiddlewareAuthorizationProvider(true);
            var context = Substitute.For<IPermissionDefinitionContext>();

            var existingPermission = new Permission(
                MiddlewarePermissions.Pages,
                new FixedLocalizableString("Pages"));

            context.GetPermissionOrNull(MiddlewarePermissions.Pages).Returns(existingPermission);

            // Quando
            provider.SetPermissions(context);

            // Entao — should not call CreatePermission since it already exists
            context.DidNotReceive().CreatePermission(
                MiddlewarePermissions.Pages,
                Arg.Any<ILocalizableString>(),
                Arg.Any<ILocalizableString>(),
                Arg.Any<MultiTenancySides>(),
                Arg.Any<Abp.Application.Features.IFeatureDependency>(),
                Arg.Any<System.Collections.Generic.Dictionary<string, object>>()
            );

            existingPermission.Children.Count.ShouldBeGreaterThan(0);
        }

        #endregion
    }
}
