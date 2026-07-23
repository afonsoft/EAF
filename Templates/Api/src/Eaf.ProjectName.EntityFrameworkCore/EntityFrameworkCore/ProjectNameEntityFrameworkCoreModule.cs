using Abp;
using Abp.Dependency;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using Eaf.ProjectName.EntityHistory;
using Eaf.ProjectName.Migrations.Seed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Eaf.ProjectName.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpZeroCoreEntityFrameworkCoreModule),
        typeof(ProjectNameCoreModule)
    )]
    public class ProjectNameEntityFrameworkCoreModule : AbpModule
    {
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                var isDevelopment = IsDevelopmentEnvironment();

                Configuration.Modules.AbpEfCore().AddDbContext<ProjectNameDbContext>(options =>
                {
                    options.DbContextOptions.EnableDetailedErrors(isDevelopment);
                    options.DbContextOptions.EnableSensitiveDataLogging(false);

                    if (isDevelopment && Configuration.IocManager.IsRegistered<ILoggerFactory>())
                    {
                        options.DbContextOptions.UseLoggerFactory(Configuration.IocManager.Resolve<ILoggerFactory>());
                    }

                    if (options.ExistingConnection != null)
                        ProjectNameDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    else
                        ProjectNameDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                });
            }

            Configuration.EntityHistory.Selectors.Add("ProjectNameEntities", EntityHistoryHelper.TrackedTypes);
            Configuration.CustomConfigProviders.Add(new EntityHistoryConfigProvider(Configuration));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ProjectNameEntityFrameworkCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            using (var scope = IocManager.CreateScope())
            {
                if (!SkipDbSeed)
                {
                    SeedHelper.SeedHostDb(IocManager);
                }
            }
        }

        private static bool IsDevelopmentEnvironment()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                ?? "Production";

            return env.Equals("Development", StringComparison.OrdinalIgnoreCase);
        }
    }
}