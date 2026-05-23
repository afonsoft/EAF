using Abp;
using Abp.Auditing;
using Abp.Domain.Uow;
using Abp.Modules;
using Abp.Runtime.Validation;
using Castle.LoggingFacility.MsLogging;
using Castle.Windsor.MsDependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;

namespace Eaf.Middleware.Worker
{
    /// <summary>
    /// Representa a classe EafServiceCollectionExtensions.
    /// </summary>
    public static class EafServiceCollectionExtensions
    {
        /// <summary>
        /// Integrates eaf to AspNet Core.
        /// </summary>
        /// <typeparam name="TStartupModule">
        /// Startup module of the application which depends on other used modules. Should be derived
        /// from <see cref="AbpModule"/>.
        /// </typeparam>
        /// <param name="services">Services.</param>
        /// <param name="optionsAction">An action to get/modify options</param>
        /// <param name="removeConventionalInterceptors">Removes the conventional interceptors</param>
        public static IServiceProvider AddEaf<TStartupModule>(this IServiceCollection services,
            [CanBeNull] Action<AbpBootstrapperOptions> optionsAction = null,
            bool removeConventionalInterceptors = true)
            where TStartupModule : AbpModule
        {
            if (removeConventionalInterceptors)
            {
                RemoveConventionalInterceptionSelectors();
            }

            var AbpBootstrapper = AddAbpBootstrapper<TStartupModule>(services, optionsAction);
            ConfigureNetCore(services);

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(AbpBootstrapper.IocManager.IocContainer, services);

            var castleLoggerFactory = serviceProvider.GetService<Castle.Core.Logging.ILoggerFactory>();
            if (castleLoggerFactory != null)
                serviceProvider.GetRequiredService<ILoggerFactory>().AddCastleLogger(castleLoggerFactory);

            InitializeEaf(AbpBootstrapper, serviceProvider);

            return serviceProvider;
        }

        /// <summary>
        /// Integrates eaf to AspNet Core without creating a IServiceProvider.
        /// </summary>
        /// <typeparam name="TStartupModule">
        /// Startup module of the application which depends on other used modules. Should be derived
        /// from <see cref="AbpModule"/>.
        /// </typeparam>
        /// <param name="services">Services.</param>
        /// <param name="optionsAction">An action to get/modify options</param>
        /// <param name="removeConventionalInterceptors">Removes the conventional interceptors</param>
        public static void AddEafWithoutCreatingServiceProvider<TStartupModule>(this IServiceCollection services,
            [CanBeNull] Action<AbpBootstrapperOptions> optionsAction = null,
            bool removeConventionalInterceptors = true)
            where TStartupModule : AbpModule
        {
            if (removeConventionalInterceptors)
            {
                RemoveConventionalInterceptionSelectors();
            }

            var AbpBootstrapper = AddAbpBootstrapper<TStartupModule>(services, optionsAction);
            ConfigureNetCore(services);

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(AbpBootstrapper.IocManager.IocContainer, services);

            var castleLoggerFactory = serviceProvider.GetService<Castle.Core.Logging.ILoggerFactory>();
            if (castleLoggerFactory != null)
                serviceProvider.GetRequiredService<ILoggerFactory>().AddCastleLogger(castleLoggerFactory);

            InitializeEaf(AbpBootstrapper, serviceProvider);
        }

        private static AbpBootstrapper AddAbpBootstrapper<TStartupModule>(IServiceCollection services,
            Action<AbpBootstrapperOptions> optionsAction)
            where TStartupModule : AbpModule
        {
            var AbpBootstrapper = Abp.AbpBootstrapper.Create<TStartupModule>(optionsAction);

            services.AddSingleton(AbpBootstrapper);

            return AbpBootstrapper;
        }

        private static void ConfigureNetCore(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddSerilog();
                loggingBuilder.SetMinimumLevel(LogLevel.Debug);
            });

            services.AddOptions();

            services.AddHttpClient();
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
        }

        private static void InitializeEaf(AbpBootstrapper app, IServiceProvider service)
        {
            app.Initialize();
            var applicationLifetime = service.GetService<IHostApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(() => app.Dispose());
        }

        private static void RemoveConventionalInterceptionSelectors()
        {
            UnitOfWorkDefaultOptions.ConventionalUowSelectorList = new List<Func<Type, bool>>();
            AbpAuditingDefaultOptions.ConventionalAuditingSelectorList = new List<Func<Type, bool>>();
            AbpValidationDefaultOptions.ConventionalValidationSelectorList = new List<Func<Type, bool>>();
        }
    }
}