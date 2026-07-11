using Abp;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using System;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization
{
    /// <summary>
    /// Testes BDD para AuthorizationExtensions seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class AuthorizationExtensionsBddTests
    {
        private static readonly PropertyInfo InstanceProperty = typeof(IocManager).GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);

        private static IDisposable SetIocManagerInstance(IocManager instance)
        {
            var original = IocManager.Instance;
            InstanceProperty.SetValue(null, instance, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, null, null);
            return new RestoreIocManager(original);
        }

        private sealed class RestoreIocManager : IDisposable
        {
            private readonly IocManager _original;
            public RestoreIocManager(IocManager original) => _original = original;
            public void Dispose() => InstanceProperty.SetValue(null, _original, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, null, null);
        }

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

            var iocManager = new IocManager();
            iocManager.IocContainer.Register(Component.For<ICacheManager>().Instance(cacheManager).LifestyleSingleton());

            using (SetIocManagerInstance(iocManager))
            {
                // Quando
                var result = session.GetExternalTokenInformation();

                // Então
                cache.Received(1).GetOrDefault("1@1");
                result.ShouldBe("external-token");
            }
        }
    }
}
