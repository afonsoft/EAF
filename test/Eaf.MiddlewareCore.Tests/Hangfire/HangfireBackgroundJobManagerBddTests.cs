using Abp.BackgroundJobs;
using Eaf.Hangfire;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Hangfire
{
    /// <summary>
    /// Fake job sem implementar IBackgroundJob/IAsyncBackgroundJob para evitar chamada ao Hangfire.
    /// </summary>
    public class FakeJob : IBackgroundJobBase<string>
    {
    }

    /// <summary>
    /// Testes BDD para HangfireBackgroundJobManager seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class HangfireBackgroundJobManagerBddTests
    {
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
            // Dado
            var sut = new HangfireBackgroundJobManager();

            // Quando/Entao
            Should.Throw<ArgumentNullException>(() => sut.Delete(null));
        }

        [Fact]
        public void Dado_JobIdVazio_Quando_Delete_Entao_DeveLancarArgumentNullException()
        {
            var sut = new HangfireBackgroundJobManager();
            Should.Throw<ArgumentNullException>(() => sut.Delete("   "));
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

        #endregion

        #region Enqueue

        [Fact]
        public void Dado_JobSemInterfaceCompativel_Quando_Enqueue_Entao_DeveRetornarStringVazia()
        {
            // Dado
            var sut = new HangfireBackgroundJobManager();

            // Quando
            var result = sut.Enqueue<FakeJob, string>("args");

            // Então
            result.ShouldBe(string.Empty);
        }

        [Fact]
        public void Dado_JobSemInterfaceCompativelEComDelay_Quando_Enqueue_Entao_DeveRetornarStringVazia()
        {
            // Dado
            var sut = new HangfireBackgroundJobManager();

            // Quando
            var result = sut.Enqueue<FakeJob, string>("args", delay: TimeSpan.FromMinutes(1));

            // Então
            result.ShouldBe(string.Empty);
        }

        #endregion

        #region EnqueueAsync

        [Fact]
        public async Task Dado_JobSemInterfaceCompativel_Quando_EnqueueAsync_Entao_DeveRetornarStringVazia()
        {
            // Dado
            var sut = new HangfireBackgroundJobManager();

            // Quando
            var result = await sut.EnqueueAsync<FakeJob, string>("args");

            // Então
            result.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task Dado_JobSemInterfaceCompativelEComDelay_Quando_EnqueueAsync_Entao_DeveRetornarStringVazia()
        {
            // Dado
            var sut = new HangfireBackgroundJobManager();

            // Quando
            var result = await sut.EnqueueAsync<FakeJob, string>("args", delay: TimeSpan.FromMinutes(1));

            // Então
            result.ShouldBe(string.Empty);
        }

        #endregion
    }
}
