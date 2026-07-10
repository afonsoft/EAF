using Abp.Collections;
using Abp.Zero.Configuration;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using NSubstitute;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.AzureActiveDirectory.Tests.Configuration
{
    public class EafMiddlewareAzureActiveDirectoryModuleConfigBddTests
    {
        [Fact]
        public void Dado_Construtor_Quando_CriarConfig_Entao_DeveInicializarPropriedadesFalse()
        {
            // Dado
            var zeroConfig = Substitute.For<IAbpZeroConfig>();
            zeroConfig.UserManagement.Returns(Substitute.For<IUserManagementConfig>());
            zeroConfig.UserManagement.ExternalAuthenticationSources.Returns(Substitute.For<ITypeList<object>>());

            // Quando
            var sut = new EafMiddlewareAzureActiveDirectoryModuleConfig(zeroConfig);

            // Então
            sut.IsEnabled.ShouldBeFalse();
            sut.AuthenticationSourceType.ShouldBeNull();
        }

        [Fact]
        public void Dado_TipoAutenticacao_Quando_Enable_Entao_DeveAtivarEAdicionarFonte()
        {
            // Dado
            var zeroConfig = Substitute.For<IAbpZeroConfig>();
            var userManagementConfig = Substitute.For<IUserManagementConfig>();
            var externalSources = Substitute.For<ITypeList<object>>();
            userManagementConfig.ExternalAuthenticationSources.Returns(externalSources);
            zeroConfig.UserManagement.Returns(userManagementConfig);

            var authSourceType = typeof(TestableAuthenticationSource);

            var sut = new EafMiddlewareAzureActiveDirectoryModuleConfig(zeroConfig);

            // Quando
            sut.Enable(authSourceType);

            // Então
            sut.IsEnabled.ShouldBeTrue();
            sut.AuthenticationSourceType.ShouldBe(authSourceType);
            externalSources.Received(1).Add(authSourceType);
        }

        private sealed class TestableAuthenticationSource
        {
        }
    }
}
