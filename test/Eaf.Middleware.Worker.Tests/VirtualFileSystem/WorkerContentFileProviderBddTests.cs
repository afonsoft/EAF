using Eaf.Middleware.Worker.VirtualFileSystem;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System.IO;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.VirtualFileSystem
{
    public class WorkerContentFileProviderBddTests
    {
        private readonly string _tempDir;

        public WorkerContentFileProviderBddTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_tempDir);
        }

        private IHostEnvironment CreateHostEnvironment()
        {
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.ContentRootPath.Returns(_tempDir);
            return hostEnvironment;
        }

        [Fact]
        public void Dado_ArquivoExistente_Quando_GetFileInfo_Entao_DeveRetornarFileInfoExistente()
        {
            // Dado
            var fileName = Path.GetRandomFileName();
            File.WriteAllText(Path.Combine(_tempDir, fileName), "conteudo");
            var provider = new WorkerContentFileProvider(CreateHostEnvironment());

            // Quando
            var fileInfo = provider.GetFileInfo(fileName);

            // Então
            fileInfo.ShouldNotBeNull();
            fileInfo.Exists.ShouldBeTrue();
        }

        [Fact]
        public void Dado_DiretorioExistente_Quando_GetDirectoryContents_Entao_DeveRetornarConteudoExistente()
        {
            // Dado
            var subDirName = Path.GetRandomFileName();
            Directory.CreateDirectory(Path.Combine(_tempDir, subDirName));
            var provider = new WorkerContentFileProvider(CreateHostEnvironment());

            // Quando
            var contents = provider.GetDirectoryContents(subDirName);

            // Então
            contents.ShouldNotBeNull();
            contents.Exists.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ArquivoInexistenteComRootPath_Quando_GetFileInfo_Entao_DeveRetornarFileInfoDoRoot()
        {
            // Dado
            var provider = new WorkerContentFileProvider(CreateHostEnvironment());

            // Quando
            var fileInfo = provider.GetFileInfo("/arquivo_inexistente.txt");

            // Então
            fileInfo.ShouldNotBeNull();
        }
    }
}
