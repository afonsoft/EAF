using Abp;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;

namespace Eaf.Quartz
{
    /// <summary>
    /// Módulo EAF que registra um scheduler Quartz.NET como implementação alternativa
    /// de <see cref="Abp.BackgroundJobs.IBackgroundJobManager"/>.
    /// Opt-in: usar em vez do módulo Hangfire, nunca junto.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class EafQuartzModule : AbpModule
    {
        private IScheduler _scheduler;

        /// <summary>
        /// Pré-inicializa o módulo registrando as opções padrão.
        /// </summary>
        public override void PreInitialize()
        {
            if (!IocManager.IsRegistered<EafQuartzOptions>())
            {
                IocManager.IocContainer.Register(
                    Component.For<EafQuartzOptions, Microsoft.Extensions.Options.IOptions<EafQuartzOptions>>()
                             .ImplementedBy<EafQuartzOptions>()
                             .LifestyleSingleton());
            }
        }

        /// <summary>
        /// Inicializa o módulo registrando tipos por convenção.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafQuartzModule).GetAssembly());

            // Adapters de job são open generics — convenção não registra; registro explícito.
            IocManager.IocContainer.Register(
                Castle.MicroKernel.Registration.Component
                    .For(typeof(EafQuartzJobAdapter<,>))
                    .LifestyleTransient());
        }

        /// <summary>
        /// Pós-inicializa criando o scheduler e associando a fábrica Windsor.
        /// O scheduler só inicia quando o primeiro job é agendado (lazy start).
        /// </summary>
        public override void PostInitialize()
        {
            var options = IocManager.Resolve<EafQuartzOptions>();
            var props = new NameValueCollection
            {
                ["quartz.scheduler.instanceName"] = options.SchedulerName ?? "EafQuartz",
                ["quartz.scheduler.instanceId"] = options.InstanceId ?? "AUTO",
            };

            switch (options.JobStoreType)
            {
                case EafQuartzJobStoreType.Ado:
                    props["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
                    props["quartz.jobStore.dataSource"] = "default";
                    props["quartz.dataSource.default.provider"] = options.AdoProvider ?? "SqlServer";
                    props["quartz.dataSource.default.connectionString"] = options.ConnectionString ?? string.Empty;
                    break;
                case EafQuartzJobStoreType.Mongo:
                    // Mongo job store requires the Quartz.Plugins.MongoDB extension package;
                    // keep the type configurable via properties without hard dependency.
                    props["quartz.jobStore.type"] = "Quartz.Impl.MongoDB.JobStore, Quartz.Plugins.MongoDB";
                    props["quartz.jobStore.connectionString"] = options.ConnectionString ?? string.Empty;
                    break;
                case EafQuartzJobStoreType.Ram:
                default:
                    props["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz";
                    break;
            }

            var factory = new StdSchedulerFactory(props);
            _scheduler = factory.GetScheduler().GetAwaiter().GetResult();
            _scheduler.JobFactory = IocManager.Resolve<EafQuartzJobFactory>();

            IocManager.IocContainer.Register(
                Component.For<IScheduler>().Instance(_scheduler).IsDefault().LifestyleSingleton());

            // Registra o manager Quartz como implementação default de IBackgroundJobManager
            // (substitui o InMemoryBackgroundJobStore do ABP quando o módulo é usado).
            IocManager.IocContainer.Register(
                Castle.MicroKernel.Registration.Component
                    .For<Abp.BackgroundJobs.IBackgroundJobManager>()
                    .ImplementedBy<EafQuartzBackgroundJobManager>()
                    .IsDefault()
                    .Named("EafQuartzBackgroundJobManager")
                    .LifestyleSingleton());

            // Resolve e inicia o worker (IBackgroundJobManager : IBackgroundWorker).
            IocManager.Resolve<EafQuartzBackgroundJobManager>().Start();
        }

        /// <summary>
        /// Encerra o scheduler no shutdown do módulo.
        /// </summary>
        public override void Shutdown()
        {
            if (_scheduler != null && _scheduler.IsStarted)
            {
                _scheduler.Shutdown(waitForJobsToComplete: true).GetAwaiter().GetResult();
            }
        }
    }
}
