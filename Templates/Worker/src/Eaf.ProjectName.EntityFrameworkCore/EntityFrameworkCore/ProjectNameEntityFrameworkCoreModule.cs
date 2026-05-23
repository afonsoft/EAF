using Eaf.Dependency;
using Eaf.EntityFrameworkCore;
using Eaf.EntityFrameworkCore.Configuration;
using Eaf.Modules;
using Eaf.ProjectName.EntityHistory;
using Eaf.ProjectName.Migrations.Seed;
using Microsoft.Extensions.Logging;
using System;

namespace Eaf.ProjectName.EntityFrameworkCore
{
    [DependsOn(
        typeof(EafEntityFrameworkCoreModule),
        typeof(ProjectNameCoreModule)
    )]
    public class ProjectNameEntityFrameworkCoreModule : EafModule
    {
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.EafEfCore().AddDbContext<ProjectNameDbContext>(options =>
                {
                    options.DbContextOptions.EnableDetailedErrors(Configuration.Database.EnableDetailedErrors);
                    options.DbContextOptions.EnableSensitiveDataLogging(Configuration.Database.EnableSensitiveDataLogging);

                    if (Configuration.Database.EnableDetailedErrors && Configuration.IocManager.IsRegistered<ILoggerFactory>())
                    {
                        options.DbContextOptions.UseLoggerFactory(Configuration.IocManager.Resolve<ILoggerFactory>());
                    }

                    if (options.ExistingConnection != null)
                        ProjectNameDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection, Configuration.Database.IsOracleEnabled);
                    else
                        ProjectNameDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString, Configuration.Database.IsOracleEnabled);
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
    }
}