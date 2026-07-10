using Abp.BackgroundJobs;
using Eaf.Hangfire;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Hangfire
{
    /// <summary>
    /// Job sem implementar interfaces compatíveis para testar fallback.
    /// </summary>
    public class FakeJob : IBackgroundJobBase<string>
    {
    }

    /// <summary>
    /// Job síncrono compatível com HangfireBackgroundJobManager.
    /// </summary>
    public class FakeSyncJob : IBackgroundJob<string>
    {
        public void Execute(string args)
        {
        }
    }

    /// <summary>
    /// Job assíncrono compatível com a interface Abp.
    /// </summary>
    public class FakeAbpAsyncJob : Abp.BackgroundJobs.IAsyncBackgroundJob<string>
    {
        public Task ExecuteAsync(string args)
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Job assíncrono compatível com a interface Eaf.
    /// </summary>
    public class FakeEafAsyncJob : Eaf.BackgroundJobs.IAsyncBackgroundJob<string>
    {
        public Task ExecuteAsync(string args, global::Hangfire.Server.PerformContext context, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Testes BDD para HangfireBackgroundJobManager seguindo o padrão Dado/Quando/Entao.
    /// </summary>
    public class HangfireBackgroundJobManagerBddTests : IDisposable
    {
        private readonly global::Hangfire.JobStorage _originalJobStorage;
        private readonly global::Hangfire.MemoryStorage.MemoryStorage _memoryStorage;

        public HangfireBackgroundJobManagerBddTests()
        {
            try
            {
                _originalJobStorage = global::Hangfire.JobStorage.Current;
            }
            catch
            {
                _originalJobStorage = null;
            }

            _memoryStorage = new global::Hangfire.MemoryStorage.MemoryStorage();
            global::Hangfire.JobStorage.Current = _memoryStorage;
        }

        public void Dispose()
        {
            global::Hangfire.JobStorage.Current = _originalJobStorage;
            (_memoryStorage as IDisposable)?.Dispose();
        }

        #region Instanciacao

        [Fact]
        public void Dado_Padrao_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new HangfireBackgroundJobManager();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Padrao_Quando_CriarInstancia_Entao_DeveImplementarIBackgroundJobManager()
        {
            var sut = new HangfireBackgroundJobManager();
            sut.ShouldBeAssignableTo<Abp.BackgroundJobs.IBackgroundJobManager>();
        }

        #endregion

        #region Delete

        [Fact]
        public void Dado_JobIdNullOuVazio_Quando_Delete_Entao_DeveLancarArgumentNullException()
        {
            var sut = new HangfireBackgroundJobManager();
            Should.Throw<ArgumentNullException>(() => sut.Delete(null));
        }

        [Fact]
        public void Dado_JobIdVazio_Quando_Delete_Entao_DeveLancarArgumentNullException()
        {
            var sut = new HangfireBackgroundJobManager();
            Should.Throw<ArgumentNullException>(() => sut.Delete("   "));
        }

        [Fact]
        public void Dado_JobExistente_Quando_Delete_Entao_DeveRetornarVerdadeiro()
        {
            // Dado
            var sut = new HangfireBackgroundJobManager();
            var jobId = sut.Enqueue<FakeSyncJob, string>("args");

            // Quando
            var result = sut.Delete(jobId);

            // Então
            result.ShouldBeTrue();
        }

        #endregion

        #region DeleteAsync

        [Fact]
        public void Dado_JobIdNullOuVazio_Quando_DeleteAsync_Entao_DeveLancarArgumentNullException()
        {
            var sut = new HangfireBackgroundJobManager();
            Should.Throw<ArgumentNullException>(() => sut.DeleteAsync(null).GetAwaiter().GetResult());
        }

        [Fact]
        public void Dado_JobIdVazio_Quando_DeleteAsync_Entao_DeveLancarArgumentNullException()
        {
            var sut = new HangfireBackgroundJobManager();
            Should.Throw<ArgumentNullException>(() => sut.DeleteAsync("  ").GetAwaiter().GetResult());
        }

        [Fact]
        public async Task Dado_JobExistente_Quando_DeleteAsync_Entao_DeveRetornarVerdadeiro()
        {
            // Dado
            var sut = new HangfireBackgroundJobManager();
            var jobId = sut.Enqueue<FakeSyncJob, string>("args");

            // Quando
            var result = await sut.DeleteAsync(jobId);

            // Então
            result.ShouldBeTrue();
        }

        #endregion

        #region Enqueue

        [Fact]
        public void Dado_JobSemInterfaceCompativel_Quando_Enqueue_Entao_DeveRetornarStringVazia()
        {
            var sut = new HangfireBackgroundJobManager();
            var result = sut.Enqueue<FakeJob, string>("args");
            result.ShouldBe(string.Empty);
        }

        [Fact]
        public void Dado_JobSync_Quando_Enqueue_Entao_DeveRetornarJobId()
        {
            var sut = new HangfireBackgroundJobManager();
            var result = sut.Enqueue<FakeSyncJob, string>("args");
            result.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_JobAbpAsync_Quando_Enqueue_Entao_DeveRetornarJobId()
        {
            var sut = new HangfireBackgroundJobManager();
            var result = sut.Enqueue<FakeAbpAsyncJob, string>("args");
            result.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_JobEafAsync_Quando_Enqueue_Entao_DeveRetornarJobId()
        {
            var sut = new HangfireBackgroundJobManager();
            var result = sut.Enqueue<FakeEafAsyncJob, string>("args");
            result.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_JobSyncComDelay_Quando_Enqueue_Entao_DeveRetornarJobId()
        {
            var sut = new HangfireBackgroundJobManager();
            var result = sut.Enqueue<FakeSyncJob, string>("args", delay: TimeSpan.FromMinutes(1));
            result.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_JobSemInterfaceCompativelEComDelay_Quando_Enqueue_Entao_DeveRetornarStringVazia()
        {
            var sut = new HangfireBackgroundJobManager();
            var result = sut.Enqueue<FakeJob, string>("args", delay: TimeSpan.FromMinutes(1));
            result.ShouldBe(string.Empty);
        }

        #endregion

        #region EnqueueAsync

        [Fact]
        public async Task Dado_JobSemInterfaceCompativel_Quando_EnqueueAsync_Entao_DeveRetornarStringVazia()
        {
            var sut = new HangfireBackgroundJobManager();
            var result = await sut.EnqueueAsync<FakeJob, string>("args");
            result.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task Dado_JobSync_Quando_EnqueueAsync_Entao_DeveRetornarJobId()
        {
            var sut = new HangfireBackgroundJobManager();
            var result = await sut.EnqueueAsync<FakeSyncJob, string>("args");
            result.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Dado_JobAbpAsync_Quando_EnqueueAsync_Entao_DeveRetornarJobId()
        {
            var sut = new HangfireBackgroundJobManager();
            var result = await sut.EnqueueAsync<FakeAbpAsyncJob, string>("args");
            result.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Dado_JobEafAsync_Quando_EnqueueAsync_Entao_DeveRetornarJobId()
        {
            var sut = new HangfireBackgroundJobManager();
            var result = await sut.EnqueueAsync<FakeEafAsyncJob, string>("args");
            result.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Dado_JobSyncComDelay_Quando_EnqueueAsync_Entao_DeveRetornarJobId()
        {
            var sut = new HangfireBackgroundJobManager();
            var result = await sut.EnqueueAsync<FakeSyncJob, string>("args", delay: TimeSpan.FromMinutes(1));
            result.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Dado_JobSemInterfaceCompativelEComDelay_Quando_EnqueueAsync_Entao_DeveRetornarStringVazia()
        {
            var sut = new HangfireBackgroundJobManager();
            var result = await sut.EnqueueAsync<FakeJob, string>("args", delay: TimeSpan.FromMinutes(1));
            result.ShouldBe(string.Empty);
        }

        #endregion
    }
}
