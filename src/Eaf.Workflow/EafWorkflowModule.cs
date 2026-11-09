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
            IocManager.Register<IWorkflowUserDirectory, NullWorkflowUserDirectory>();
        }
    }
}
