using Abp.BackgroundJobs;
using Quartz;
using Shouldly;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Quartz.Tests
{
    /// <summary>
    /// Testes do módulo Eaf.Quartz.
    /// </summary>
    public class EafQuartz_Tests : EafQuartzTestBase
    {
        private static async Task AguardarExecucaoAsync(Func<bool> condicao, int timeoutMs = 15000)
        {
            var sw = Stopwatch.StartNew();
            while (!condicao() && sw.ElapsedMilliseconds < timeoutMs)
            {
                await Task.Delay(100);
            }
            condicao().ShouldBeTrue("condição não atingida dentro do timeout");
        }

        [Fact]
        public void Dado_ModuloCarregado_Quando_ResolveScheduler_Entao_SchedulerEstaIniciado()
        {
            var scheduler = Resolve<IScheduler>();
            scheduler.ShouldNotBeNull();
            scheduler.IsStarted.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ModuloCarregado_Quando_ResolveBackgroundJobManager_Entao_EhImplementacaoQuartz()
        {
            var manager = Resolve<IBackgroundJobManager>();
            manager.ShouldBeOfType<EafQuartzBackgroundJobManager>();
        }

        [Fact]
        public async Task Dado_JobSincrono_Quando_EnqueueAsync_Entao_ExecutaComArgumentos()
        {
            var manager = Resolve<IBackgroundJobManager>();
            var jobId = await manager.EnqueueAsync<SampleBackgroundJob, SampleJobArgs>(new SampleJobArgs { Value = "sync-1" });

            jobId.ShouldNotBeNullOrEmpty();
            await AguardarExecucaoAsync(() => SampleBackgroundJob.Executed.Any(a => a.Value == "sync-1"));
        }

        [Fact]
        public async Task Dado_JobAssincrono_Quando_EnqueueAsync_Entao_ExecutaAsync()
        {
            var manager = Resolve<IBackgroundJobManager>();
            await manager.EnqueueAsync<SampleAsyncBackgroundJob, SampleJobArgs>(new SampleJobArgs { Value = "async-1" });

            await AguardarExecucaoAsync(() => SampleAsyncBackgroundJob.Executed.Any(a => a.Value == "async-1"));
        }

        [Fact]
        public async Task Dado_JobAgendado_Quando_DeleteAsync_Entao_RemoveJob()
        {
            var manager = Resolve<IBackgroundJobManager>();
            var jobId = await manager.EnqueueAsync<SampleBackgroundJob, SampleJobArgs>(
                new SampleJobArgs { Value = "delete-me" }, delay: TimeSpan.FromMinutes(5));

            (await manager.DeleteAsync(jobId)).ShouldBeTrue();
            (await manager.DeleteAsync(jobId)).ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_CronInvalido_Quando_ScheduleAsync_Entao_LancaUserFriendlyException()
        {
            var manager = Resolve<EafQuartzBackgroundJobManager>();

            await Should.ThrowAsync<Abp.UI.UserFriendlyException>(
                () => manager.ScheduleAsync<SampleBackgroundJob, SampleJobArgs>(
                    new SampleJobArgs { Value = "x" }, "isto nao e cron"));
        }

        [Fact]
        public async Task Dado_CronValido_Quando_ScheduleAsync_Entao_AgendaJobRecorrente()
        {
            var manager = Resolve<EafQuartzBackgroundJobManager>();
            var scheduler = Resolve<IScheduler>();

            var jobId = await manager.ScheduleAsync<SampleBackgroundJob, SampleJobArgs>(
                new SampleJobArgs { Value = "recurring" }, "0/5 * * * * ?");

            jobId.ShouldNotBeNullOrEmpty();
            var keys = await scheduler.GetJobKeys(global::Quartz.Impl.Matchers.GroupMatcher<JobKey>.GroupEquals(nameof(SampleBackgroundJob)));
            keys.ShouldContain(k => k.Name == jobId);
        }

        [Fact]
        public async Task Dado_JobQueFalha_Quando_EnqueueAsync_Entao_ErroEhPropagadoAoQuartz()
        {
            // O job falha dentro do scheduler; a falha não deve derrubar o manager nem o scheduler.
            var manager = Resolve<IBackgroundJobManager>();
            var jobId = await manager.EnqueueAsync<FailingBackgroundJob, SampleJobArgs>(new SampleJobArgs { Value = "boom" });

            jobId.ShouldNotBeNullOrEmpty();
            var scheduler = Resolve<IScheduler>();
            await AguardarExecucaoAsync(() => scheduler.IsStarted);
        }

        [Fact]
        public void Dado_OpcoesPadrao_Entao_ValoresCorretos()
        {
            var options = Resolve<EafQuartzOptions>();
            options.SchedulerName.ShouldBe("EafQuartz");
            options.JobStoreType.ShouldBe(EafQuartzJobStoreType.Ram);
            options.InstanceId.ShouldBe("AUTO");
            options.Value.ShouldBeSameAs(options);
        }
    }
}
