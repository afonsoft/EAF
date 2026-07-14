using Abp.Configuration.Startup;
using Eaf.AspNetCore.Hangfire.Configuration;
using Eaf.Hangfire;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System;
using System.Reflection;
using System.Threading;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Hangfire
{
    public class EafHangfireApplicationBuilderExtensionsBddTests : IDisposable
    {
        private readonly JobStorage _originalJobStorage;

        public EafHangfireApplicationBuilderExtensionsBddTests()
        {
            var currentField = typeof(JobStorage).GetField("_current", BindingFlags.NonPublic | BindingFlags.Static);
            _originalJobStorage = currentField?.GetValue(null) as JobStorage ?? new MemoryStorage();
            JobStorage.Current = new MemoryStorage();
        }

        public void Dispose()
        {
            JobStorage.Current = _originalJobStorage ?? new MemoryStorage();
        }

        private static IApplicationBuilder CreateApplicationBuilder()
        {
            var app = Substitute.For<IApplicationBuilder>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            var configuration = Substitute.For<IAbpStartupConfiguration>();
            configuration.BackgroundJobs.IsJobExecutionEnabled.Returns(true);

            var hostLifetime = Substitute.For<IHostApplicationLifetime>();
            hostLifetime.ApplicationStopped.Returns(CancellationToken.None);
            hostLifetime.ApplicationStopping.Returns(CancellationToken.None);
            hostLifetime.ApplicationStarted.Returns(CancellationToken.None);

            var memoryStorage = new MemoryStorage();

            serviceProvider.GetService(typeof(IAbpStartupConfiguration)).Returns(configuration);
            serviceProvider.GetService(typeof(IGlobalConfiguration)).Returns(Substitute.For<IGlobalConfiguration>());
            serviceProvider.GetService(typeof(JobStorage)).Returns(memoryStorage);
            serviceProvider.GetService(typeof(IHostApplicationLifetime)).Returns(hostLifetime);
            serviceProvider.GetService(typeof(RouteCollection)).Returns(new RouteCollection());

            var branch = Substitute.For<IApplicationBuilder>();
            branch.ApplicationServices.Returns(serviceProvider);
            branch.Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>()).Returns(branch);
            branch.Build().Returns(Substitute.For<RequestDelegate>());

            app.ApplicationServices.Returns(serviceProvider);
            app.New().Returns(branch);
            app.Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>()).Returns(app);

            return app;
        }

        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(EafHangfireApplicationBuilderExtensions).IsAbstract.ShouldBeTrue();
            typeof(EafHangfireApplicationBuilderExtensions).IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void Dado_JobExecutionDesabilitado_Quando_UseEafHangfire_Entao_DeveRetornarSemConfigurar()
        {
            var app = Substitute.For<IApplicationBuilder>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            var configuration = Substitute.For<IAbpStartupConfiguration>();
            configuration.BackgroundJobs.IsJobExecutionEnabled.Returns(false);
            serviceProvider.GetService(typeof(IAbpStartupConfiguration)).Returns(configuration);
            app.ApplicationServices.Returns(serviceProvider);

            app.UseEafHangfire();

            app.DidNotReceive().Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>());
            app.DidNotReceive().New();
        }

        [Fact]
        public void Dado_JobExecutionHabilitado_Quando_UseEafHangfireComIsEnabledFalse_Entao_DeveRetornarSemConfigurar()
        {
            var app = CreateApplicationBuilder();

            app.UseEafHangfire(options => options.IsEnabled = false);

            app.DidNotReceive().Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>());
            app.DidNotReceive().New();
        }

        [Fact]
        public void Dado_JobExecutionHabilitado_Quando_UseEafHangfireComIsEnabledTrue_Entao_DeveConfigurarDashboardEServer()
        {
            var app = CreateApplicationBuilder();

            app.UseEafHangfire(options =>
            {
                options.IsEnabled = true;
                options.WorkerCount = 1;
                options.Queues = new[] { "default" };
            });

            app.Received(1).New();
            app.Received(1).Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>());
        }

        [Fact]
        public void Dado_JobExecutionHabilitado_Quando_UseEafHangfireComCustomOptions_Entao_DeveAplicarOverrides()
        {
            var app = CreateApplicationBuilder();

            app.UseEafHangfire(options =>
            {
                options.IsEnabled = true;
                options.WorkerCount = 1;
                options.Queues = new[] { "default" };
                options.PathMatch = "/jobs";
                options.AppPath = "/";
                options.PrefixPath = "/admin";
                options.DashboardTitle = "EAF Jobs";
            });

            app.Received(1).New();
            app.Received(1).Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>());
        }

        [Fact]
        public void Dado_JobExecutionHabilitado_Quando_UseEafHangfireSemOptions_Entao_DeveConfigurarDashboardEServer()
        {
            var app = CreateApplicationBuilder();

            app.UseEafHangfire();

            app.Received(1).New();
            app.Received(1).Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>());
        }
    }
}
