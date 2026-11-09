using Abp;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Eaf.Workflow.Authorization;
using Eaf.Workflow.Domain;

namespace Eaf.Workflow
{
    /// <summary>
    /// EAF workflow and approval engine module.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule), typeof(AbpAutoMapperModule))]
    public class EafWorkflowModule : AbpModule
    {
        /// <inheritdoc />
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<EafWorkflowAuthorizationProvider>();
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafWorkflowModule).GetAssembly());
            IocManager.IocContainer.Register(
                Castle.MicroKernel.Registration.Component
                    .For<IWorkflowUserDirectory>()
                    .ImplementedBy<NullWorkflowUserDirectory>()
                    .IsDefault()
                    .LifestyleTransient());
        }
        /// <inheritdoc />
        public override void PostInitialize()
        {
            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                try
                {
                    var workManager = IocManager.Resolve<Abp.Threading.BackgroundWorkers.IBackgroundWorkerManager>();
                    workManager.Add(IocManager.Resolve<WorkflowSlaWorker>());
                }
                catch (System.Exception ex)
                {
                    // No persistence layer registered (e.g. unit tests) — SLA reminders are skipped.
                    Logger.Warn($"WorkflowSlaWorker not registered: {ex.Message}");
                }
            }
        }
    }}
