using Abp.Collections;
using Abp.Zero.Configuration;
using Eaf.Middleware.Ldap.Configuration;
using NSubstitute;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Ldap.Tests.Configuration
{
    public class EafMiddlewareLdapModuleConfigBddTests
    {
        [Fact]
        public void Dado_Configuracao_Quando_Enable_Entao_DeveAtivarComTipoInformado()
        {
            var middlewareConfig = Substitute.For<IAbpZeroConfig>();
            var userManagementConfig = Substitute.For<IUserManagementConfig>();
            var typeList = new TypeList();
            userManagementConfig.ExternalAuthenticationSources.Returns(typeList);
            middlewareConfig.UserManagement.Returns(userManagementConfig);

            var config = new EafMiddlewareLdapModuleConfig(middlewareConfig);
            var authType = typeof(string);

            config.Enable(authType);

            config.IsEnabled.ShouldBeTrue();
            config.AuthenticationSourceType.ShouldBe(authType);
            typeList.ShouldContain(authType);
        }

        [Fact]
        public void Dado_Configuracao_Quando_Enable_Entao_UseNovellProviderDeveRefletirOS()
        {
            var middlewareConfig = Substitute.For<IAbpZeroConfig>();
            var userManagementConfig = Substitute.For<IUserManagementConfig>();
            userManagementConfig.ExternalAuthenticationSources.Returns(new TypeList());
            middlewareConfig.UserManagement.Returns(userManagementConfig);

            var config = new EafMiddlewareLdapModuleConfig(middlewareConfig);

            config.Enable(typeof(string));

            config.UseNovellProvider.ShouldBe(!OperatingSystem.IsWindows());
        }
    }
}
