using Abp.BackgroundJobs;
using Abp.Dependency;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Eaf.Quartz.Tests
{
    /// <summary>
    /// Argumentos do job de amostra usado nos testes.
    /// </summary>
    public class SampleJobArgs
    {
        /// <summary>Obtém ou define o valor transportado para o job.</summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Job síncrono de amostra: registra execuções em um bucket estático para asserção.
    /// </summary>
    public class SampleBackgroundJob : IBackgroundJob<SampleJobArgs>, ITransientDependency
    {
        /// <summary>Execuções observadas (thread-safe).</summary>
        public static readonly ConcurrentBag<SampleJobArgs> Executed = new();

        /// <inheritdoc />
        public void Execute(SampleJobArgs args)
        {
            Executed.Add(args);
        }
    }

    /// <summary>
    /// Job assíncrono de amostra.
    /// </summary>
    public class SampleAsyncBackgroundJob : IAsyncBackgroundJob<SampleJobArgs>, ITransientDependency
    {
        /// <summary>Execuções observadas.</summary>
        public static readonly ConcurrentBag<SampleJobArgs> Executed = new();

        /// <inheritdoc />
        public Task ExecuteAsync(SampleJobArgs args)
        {
            Executed.Add(args);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Job que sempre falha, para testar o caminho de erro.
    /// </summary>
    public class FailingBackgroundJob : IBackgroundJob<SampleJobArgs>, ITransientDependency
    {
        /// <inheritdoc />
        public void Execute(SampleJobArgs args)
        {
            throw new InvalidOperationException("falha proposital");
        }
    }
}
