using Abp.Dependency;
using Quartz;
using Quartz.Spi;
using System;

namespace Eaf.Quartz
{
    /// <summary>
    /// Fábrica de jobs do Quartz que resolve instâncias a partir do contêiner Windsor,
    /// garantindo o lifestyle escopo/Transient do ABP dentro da execução do job.
    /// </summary>
    public class EafQuartzJobFactory : IJobFactory, ISingletonDependency
    {
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="EafQuartzJobFactory"/>.
        /// </summary>
        public EafQuartzJobFactory(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <inheritdoc />
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)_iocResolver.Resolve(bundle.JobDetail.JobType);
        }

        /// <inheritdoc />
        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}
