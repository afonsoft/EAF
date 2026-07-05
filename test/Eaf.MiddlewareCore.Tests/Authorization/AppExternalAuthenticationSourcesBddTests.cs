using Eaf.Middleware.Authorization.AzureActiveDirectory;
using Eaf.Middleware.Authorization.Ldap;
using Eaf.Middleware.AzureActiveDirectory.Authentication;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Ldap.Authentication;
using Eaf.Middleware.Ldap.Configuration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization
{
    /// <summary>
    /// Testes BDD para as fontes de autenticação externas (AAD e LDAP) seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class AppExternalAuthenticationSourcesBddTests
    {
        [Fact]
        public void Dado_Dependencias_Quando_CriarAppAzureActiveDirectoryAuthenticationSource_Entao_DeveDefinirName()
        {
            var settings = Substitute.For<IAzureActiveDirectorySettings>();
            var config = Substitute.For<IEafMiddlewareAzureActiveDirectoryModuleConfig>();

            var source = new AppAzureActiveDirectoryAuthenticationSource(settings, config);

            source.ShouldNotBeNull();
            source.Name.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarAppLdapAuthenticationSource_Entao_DeveDefinirNameLdap()
        {
            var settings = Substitute.For<ILdapSettings>();
            var config = Substitute.For<IEafMiddlewareLdapModuleConfig>();

            var source = new AppLdapAuthenticationSource(settings, config);

            source.Name.ShouldBe(LdapAuthenticationSource<global::Eaf.Middleware.MultiTenancy.Tenant, global::Eaf.Middleware.Authorization.Users.User>.SourceName);
        }
    }
}
