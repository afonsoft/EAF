using Abp;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Eaf.Middleware;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships.Cache;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System.IO;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Middleware
{
    public class MiddlewareWebCoreModuleIntegrationTests
    {
        [Fact]
        public void Dado_MiddlewareWebCoreModule_Quando_ExecutarCicloDeVida_Entao_DeveCompletarSemErros()
        {
            var iocManager = new IocManager();
            var tempDir = Path.GetTempPath();
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.ContentRootPath.Returns(tempDir);
            hostEnvironment.EnvironmentName.Returns("Development");

            iocManager.IocContainer.Register(
                Castle.MicroKernel.Registration.Component.For<IHostEnvironment>()
                    .Instance(hostEnvironment)
                    .LifestyleSingleton()
            );

            using var bootstrapper = Abp.AbpBootstrapper.Create<MiddlewareWebCoreModuleIntegrationTestModule>(options =>
            {
                options.IocManager = iocManager;
            });

            Should.NotThrow(() => bootstrapper.Initialize());

            bootstrapper.IocManager.IsRegistered<IUserFriendsCache>().ShouldBeTrue();
            bootstrapper.IocManager.IsRegistered<ISettingManager>().ShouldBeTrue();
            bootstrapper.IocManager.IsRegistered<IChatCommunicator>().ShouldBeTrue();
            bootstrapper.IocManager.IsRegistered<AppFolders>().ShouldBeTrue();
        }
    }
}
