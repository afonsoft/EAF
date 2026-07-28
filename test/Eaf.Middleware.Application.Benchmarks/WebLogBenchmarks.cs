using BenchmarkDotNet.Attributes;
using Eaf.Middleware;
using Eaf.Middleware.IO;
using Eaf.Middleware.Logging;
using Eaf.Middleware.Storage;
using NSubstitute;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Eaf.Middleware.Application.Benchmarks
{
    /// <summary>
    /// Benchmarks for WebLogAppService tail reading.
    /// </summary>
    [ShortRunJob]
    [MemoryDiagnoser]
    public class WebLogBenchmarks
    {
        private string _logFolder = null!;
        private string _logFilePath = null!;
        private WebLogAppService _service = null!;

        /// <summary>
        /// Setup.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            _logFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_logFolder);
            _logFilePath = Path.Combine(_logFolder, "app.txt");

            var builder = new StringBuilder();
            for (int i = 0; i < 100000; i++)
            {
                builder.AppendLine($"INFO [{i:000000}] This is a sample log line used to simulate a moderately large web log file.");
            }
            File.WriteAllText(_logFilePath, builder.ToString());

            var appFolders = Substitute.For<IAppFolders>();
            appFolders.WebLogsFolder.Returns(_logFolder);

            var tempFileCache = Substitute.For<ITempFileCacheManager>();
            _service = new WebLogAppService(appFolders, tempFileCache);
        }

        /// <summary>
        /// Cleanup.
        /// </summary>
        [GlobalCleanup]
        public void Cleanup()
        {
            try
            {
                if (Directory.Exists(_logFolder))
                    Directory.Delete(_logFolder, true);
            }
            catch
            {
                // ignore cleanup errors
            }
        }

        /// <summary>
        /// Legacy tail read that loads the whole file into memory.
        /// </summary>
        [Benchmark(Baseline = true)]
        public void LegacyTailRead()
        {
            var lines = File.ReadLines(_logFilePath)
                .Reverse()
                .Take(1000)
                .ToList();
        }

        /// <summary>
        /// New tail read that uses a bounded buffer.
        /// </summary>
        [Benchmark]
        public void OptimizedTailRead()
        {
            _service.GetLatestWebLogs();
        }
    }
}
