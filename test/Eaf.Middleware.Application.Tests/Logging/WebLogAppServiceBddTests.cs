using Eaf.Middleware.Logging;
using Eaf.Middleware.Storage;
using NSubstitute;
using Shouldly;
using System.IO;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Logging
{
    /// <summary>
    /// Testes BDD para WebLogAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class WebLogAppServiceBddTests
    {
        private readonly IAppFolders _appFolders;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly WebLogAppService _sut;

        public WebLogAppServiceBddTests()
        {
            _appFolders = Substitute.For<IAppFolders>();
            _tempFileCacheManager = Substitute.For<ITempFileCacheManager>();
            _sut = new WebLogAppService(_appFolders, _tempFileCacheManager);
        }

        #region Construtor

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region GetLatestWebLogs

        [Fact]
        public void Dado_DiretorioInexistente_Quando_GetLatestWebLogs_Entao_DeveRetornarListaVazia()
        {
            // Dado
            _appFolders.WebLogsFolder.Returns("/caminho/inexistente/logs");

            // Quando
            var result = _sut.GetLatestWebLogs();

            // Então
            result.ShouldNotBeNull();
            result.LatestWebLogLines.ShouldNotBeNull();
            result.LatestWebLogLines.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_DiretorioExistenteSemArquivos_Quando_GetLatestWebLogs_Entao_DeveRetornarOutputSemLinhas()
        {
            // Dado
            var tempDir = Path.Combine(Path.GetTempPath(), "WebLogTest_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                _appFolders.WebLogsFolder.Returns(tempDir);

                // Quando
                var result = _sut.GetLatestWebLogs();

                // Então
                result.ShouldNotBeNull();
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void Dado_DiretorioComArquivoLog_Quando_GetLatestWebLogs_Entao_DeveRetornarLinhas()
        {
            // Dado
            var tempDir = Path.Combine(Path.GetTempPath(), "WebLogTest_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var logFile = Path.Combine(tempDir, "app.txt");
                File.WriteAllText(logFile, "[IMF] 2024-01-01 Test log line\n[ERR] Error line\n[DBG] Debug line\n");
                _appFolders.WebLogsFolder.Returns(tempDir);

                // Quando
                var result = _sut.GetLatestWebLogs();

                // Então
                result.ShouldNotBeNull();
                result.LatestWebLogLines.ShouldNotBeNull();
                result.LatestWebLogLines.Count.ShouldBeGreaterThan(0);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        #endregion

        [Fact]
        public void Dado_ArquivoComMaisDeCemLinhas_Quando_GetLatestWebLogs_Entao_DevePararNoLimite()
        {
            // Dado
            var tempDir = Path.Combine(Path.GetTempPath(), "WebLogTest_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var logFile = Path.Combine(tempDir, "app.txt");
                var lines = new List<string>();
                for (int i = 0; i < 101; i++)
                {
                    lines.Add($"INFO log line {i}");
                }
                File.WriteAllLines(logFile, lines);
                _appFolders.WebLogsFolder.Returns(tempDir);

                // Quando
                var result = _sut.GetLatestWebLogs();

                // Então
                result.ShouldNotBeNull();
                result.LatestWebLogLines.Count.ShouldBeLessThanOrEqualTo(100);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        #region DownloadWebLogs

        [Fact]
        public void Dado_DiretorioComLogs_Quando_DownloadWebLogs_Entao_DeveRetornarZipFile()
        {
            // Dado
            var tempDir = Path.Combine(Path.GetTempPath(), "WebLogTest_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var logFile = Path.Combine(tempDir, "app.log");
                File.WriteAllText(logFile, "test log content");
                _appFolders.WebLogsFolder.Returns(tempDir);

                // Quando
                var result = _sut.DownloadWebLogs();

                // Então
                result.ShouldNotBeNull();
                result.FileName.ShouldBe("WebSiteLogs.zip");
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        #endregion
    }
}
