using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.Reflection.Extensions;
using Eaf.Middleware;
using Eaf.Middleware.Application.RateLimiting;
using Eaf.Middleware.Moderation;
using Eaf.Middleware.RateLimiting;
using Eaf.ProjectName.EntityFrameworkCore;
using Eaf.ProjectName.Migrations.Seed;
using System;

namespace Eaf.ProjectName
{
    [DependsOn(
        typeof(ProjectNameEntityFrameworkCoreModule),
        typeof(MiddlewareApplicationModule),
        typeof(Eaf.DynamicEntityProperties.EafDynamicEntityPropertiesModule),
        typeof(Eaf.Workflow.EafWorkflowModule)
    )]
    public class ProjectNameApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(ProjectNameCustomDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ProjectNameApplicationModule).GetAssembly());

#if EAF_USE_SOURCE_PROJECTS
            // AutoMapper→Mapperly transition: register source-generated pairs with
            // the shared adapter (available when building against module sources).
            var generatedMapper = new Eaf.ProjectName.Mapping.ProjectNameMapperly();
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Eaf.ProjectName.Airplanes.Dtos.CreateOrEditAirplaneDto, Eaf.ProjectName.Airplanes.Airplane>(generatedMapper.ToEntity);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Eaf.ProjectName.Airplanes.Airplane, Eaf.ProjectName.Airplanes.Dtos.AirplaneDto>(generatedMapper.ToDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<System.Collections.Generic.List<Eaf.ProjectName.Airplanes.Airplane>, System.Collections.Generic.List<Eaf.ProjectName.Airplanes.Dtos.AirplaneDto>>(generatedMapper.ToDtoList);
#endif

            // Phase B: real workflow user directory overrides the module's null default.
            IocManager.IocContainer.Register(
                Castle.MicroKernel.Registration.Component
                    .For<Eaf.Workflow.Domain.IWorkflowUserDirectory>()
                    .ImplementedBy<Workflows.WorkflowUserDirectory>()
                    .IsDefault()
                    .Named("EafWorkflowUserDirectory")
                    .LifestyleTransient());
        }

        public override void PostInitialize()
        {
            IocManager.RegisterIfNot<IRateLimitManager, RateLimitManager>(DependencyLifeStyle.Transient);
            IocManager.RegisterIfNot<IModerationAuditWriter, NullModerationAuditWriter>(DependencyLifeStyle.Transient);

            using (var _connectionStringResolver = IocManager.ResolveAsDisposable<DefaultConnectionStringResolver>())
            {
                var hostConnStr = _connectionStringResolver.Object.GetNameOrConnectionString(new ConnectionStringResolveArgs(MultiTenancySides.Host));
                if (hostConnStr.IsNullOrWhiteSpace())
                {
                    Logger.Error("Configuration file should contain a connection string");
                    return;
                }

                using (var _startupConfiguration = IocManager.ResolveAsDisposable<IAbpStartupConfiguration>())
                {
                    using (var _migrator = IocManager.ResolveAsDisposable<ProjectNameDbMigrator>())
                    {
                        Logger.Info("Database migration started...");
                        try
                        {
                            _migrator.Object.CreateOrMigrateForHost(SeedHelper.SeedHostDb);
                        }
                        catch (Exception ex)
                        {
                            Logger.ErrorFormat(ex, "An error occured during hangfire migration: {0}", ex.Message);
                            Logger.Info("Canceled migrations.");
                            return;
                        }

                        Logger.Info("Database migration completed.");
                    }
                }
            }
        }
    }
}