using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Logging;
using Quartz;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eaf.Quartz
{
    /// <summary>
    /// Chave do <see cref="JobDataMap"/> que carrega os argumentos do job serializados em JSON.
    /// </summary>
    public static class EafQuartzJobKeys
    {
        /// <summary>Nome da entrada que guarda o JSON dos argumentos do job.</summary>
        public const string Arguments = "eaf.job.args";
    }

    /// <summary>
    /// Adapter que executa um <see cref="IBackgroundJobBase{TArgs}"/> do ABP dentro do Quartz.
    /// Suporta tanto jobs síncronos (<see cref="IBackgroundJob{TArgs}"/>) quanto assíncronos
    /// (<see cref="IAsyncBackgroundJob{TArgs}"/>). Os argumentos são desserializados do
    /// <see cref="JobDataMap"/> a cada execução.
    /// </summary>
    /// <typeparam name="TJob">Tipo do background job ABP.</typeparam>
    /// <typeparam name="TArgs">Tipo dos argumentos do job.</typeparam>
    public class EafQuartzJobAdapter<TJob, TArgs> : IJob, ITransientDependency
        where TJob : IBackgroundJobBase<TArgs>
    {
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="EafQuartzJobAdapter{TJob,TArgs}"/>.
        /// </summary>
        public EafQuartzJobAdapter(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <inheritdoc />
        public async Task Execute(IJobExecutionContext context)
        {
            var job = default(TJob);
            try
            {
                var json = context.MergedJobDataMap.GetString(EafQuartzJobKeys.Arguments);
                var args = json == null ? default : JsonSerializer.Deserialize<TArgs>(json);

                job = _iocResolver.Resolve<TJob>();

                if (job is IAsyncBackgroundJob<TArgs> asyncJob)
                {
                    await asyncJob.ExecuteAsync(args).ConfigureAwait(false);
                }
                else if (job is IBackgroundJob<TArgs> syncJob)
                {
                    syncJob.Execute(args);
                }
                else
                {
                    throw new Abp.AbpException(
                        $"O job {typeof(TJob).Name} não implementa IBackgroundJob nem IAsyncBackgroundJob.");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error($"Background job {typeof(TJob).Name} falhou durante a execução.", ex);
                throw new JobExecutionException(ex, refireImmediately: false);
            }
            finally
            {
                if (job != null)
                {
                    _iocResolver.Release(job);
                }
            }
        }
    }
}
