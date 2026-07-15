using Abp;
using Abp.Dependency;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Configuration;
using Abp.EntityHistory;
using Abp.Modules;
using Eaf.ProjectName.EntityHistory;
using Eaf.ProjectName.Migrations.Seed;
using Abp.Reflection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Eaf.ProjectName.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreModule),
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
                Configuration.Modules.AbpEfCore().AddDbContext<ProjectNameDbContext>(options =>
                {
                    options.DbContextOptions.EnableDetailedErrors(true);
                    options.DbContextOptions.EnableSensitiveDataLogging(false);

                    if (Configuration.IocManager.IsRegistered<ILoggerFactory>())
                    {
                        options.DbContextOptions.UseLoggerFactory(Configuration.IocManager.Resolve<ILoggerFactory>());
                    }

                    if (options.ExistingConnection != null)
                        ProjectNameDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    else
                        ProjectNameDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                });
            }

            Configuration.EntityHistory.Selectors.Add(new NamedTypeSelector("ProjectNameEntities", type => EntityHistoryHelper.TrackedTypes.Contains(type)));
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