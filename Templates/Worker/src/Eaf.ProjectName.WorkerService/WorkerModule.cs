using Abp.Reflection.Extensions;
using Eaf.Middleware.Worker;
using Abp.Modules;
using Eaf.ProjectName.Application;
using Eaf.ProjectName.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace Eaf.ProjectName.WorkerService
{
    [DependsOn(
        typeof(ProjectNameCoreModule),
        typeof(MiddlewareWorkerModule)
    //typeof(ProjectNameEntityFrameworkCoreModule)
    )]
    public class WorkerModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public WorkerModule(IHostEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(WorkerModule).GetAssembly());
        }

        public override void PreInitialize()
        {
            //Set default connection string
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(ProjectNameConsts.ConnectionStringName);
        }

    }
}