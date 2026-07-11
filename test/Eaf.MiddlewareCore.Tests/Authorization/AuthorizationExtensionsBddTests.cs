using Abp;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization
{
    /// <summary>
    /// Testes BDD para AuthorizationExtensions seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class AuthorizationExtensionsBddTests
    {
        [Fact]
        public void Dado_ClasseAuthorizationExtensions_Quando_VerificarTipo_Entao_DeveSerEstatica()
        {
            var tipo = typeof(AuthorizationExtensions);
            (tipo.IsAbstract && tipo.IsSealed).ShouldBeTrue();
        }

        [Fact]
        public void Dado_SessaoSemUserId_Quando_GetExternalTokenInformation_Entao_DeveLancarAbpException()
        {
            var session = Substitute.For<IAbpSession>();
            session.UserId.Returns((long?)null);

            Should.Throw<AbpException>(() => session.GetExternalTokenInformation());
        }

        [Fact]
        public void Dado_SessaoComUserId_Quando_GetExternalTokenInformation_Entao_DeveRetornarValorDoCache()
        {
            // Dado
            var session = Substitute.For<IAbpSession>();
            session.UserId.Returns(1L);
            session.TenantId.Returns(1);

            var cache = Substitute.For<ICache>();
            cache.GetOrDefault("1@1").Returns("external-token");

            var cacheManager = Substitute.For<ICacheManager>();
            cacheManager.GetCache("ExternalTokenInformationCache").Returns(cache);

            if (!IocManager.Instance.IsRegistered<ICacheManager>())
                IocManager.Instance.IocContainer.Register(Component.For<ICacheManager>().Instance(cacheManager).LifestyleSingleton());

            // Quando
            var result = session.GetExternalTokenInformation();

            // Então
            cache.Received(1).GetOrDefault("1@1");
            result.ShouldBe("external-token");
        }
    }
}
