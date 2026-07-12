using Abp.Reflection.Extensions;
using Eaf.Middleware.Web;
using Shouldly;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Net.Web
{
    public class WebContentDirectoryFinderBddTests : IDisposable
    {
        private readonly string _webHostFolder;

        public WebContentDirectoryFinderBddTests()
        {
            var coreAssemblyDirectoryPath = Path.GetDirectoryName(typeof(Eaf.Middleware.MiddlewareCoreModule).GetAssembly().Location);
            if (coreAssemblyDirectoryPath == null)
            {
                throw new InvalidOperationException("Could not find location of Eaf.Middleware.Core assembly!");
            }

            var directoryInfo = new DirectoryInfo(coreAssemblyDirectoryPath);
            while (!DirectoryContains(directoryInfo.FullName, "Eaf.sln")
                   && !DirectoryContains(directoryInfo.FullName, "Eaf.ProjectName.sln")
                   && !DirectoryContains(directoryInfo.FullName, "Web.Host.csproj"))
            {
                if (directoryInfo.Parent == null)
                {
                    throw new InvalidOperationException("Could not find content root folder!");
                }

                directoryInfo = directoryInfo.Parent;
            }

            _webHostFolder = Path.Combine(directoryInfo.FullName, $"src{Path.DirectorySeparatorChar}Eaf.Middleware.Web.Host");
        }

        private static bool DirectoryContains(string directory, string fileName)
        {
            return Directory.GetFiles(directory).Any(filePath => string.Equals(Path.GetFileName(filePath), fileName, StringComparison.OrdinalIgnoreCase));
        }

        public void Dispose()
        {
            if (Directory.Exists(_webHostFolder))
            {
                Directory.Delete(_webHostFolder, recursive: true);
            }
        }

        [Fact]
        public void Dado_ProjetoWebHostInexistente_Quando_CalculateContentRootFolder_Entao_DeveLancarExcecao()
        {
            if (Directory.Exists(_webHostFolder))
            {
                Directory.Delete(_webHostFolder, recursive: true);
            }

            var ex = Should.Throw<Exception>(() => WebContentDirectoryFinder.CalculateContentRootFolder());
            ex.Message.ShouldContain("Could not find root folder of the web project!");
        }

        [Fact]
        public void Dado_ProjetoWebHostExistente_Quando_CalculateContentRootFolder_Entao_DeveRetornarPasta()
        {
            Directory.CreateDirectory(_webHostFolder);

            var result = WebContentDirectoryFinder.CalculateContentRootFolder();

            result.ShouldBe(_webHostFolder);
        }

        [Fact]
        public void Dado_DiretorioSemArquivosEsperados_Quando_DirectoryContains_Entao_DeveRetornarFalse()
        {
            var directoryContains = typeof(WebContentDirectoryFinder).GetMethod("DirectoryContains", BindingFlags.NonPublic | BindingFlags.Static);
            directoryContains.ShouldNotBeNull();

            var tempPath = Path.GetTempPath();
            var result = directoryContains!.Invoke(null, new object[] { tempPath, "arquivo-nao-existente.txt" });

            result.ShouldBe(false);
        }

        [Fact]
        public void Dado_DiretorioComWebHostCsproj_Quando_DirectoryContains_Entao_DeveRetornarTrue()
        {
            var directoryContains = typeof(WebContentDirectoryFinder).GetMethod("DirectoryContains", BindingFlags.NonPublic | BindingFlags.Static);
            directoryContains.ShouldNotBeNull();

            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            var filePath = Path.Combine(tempDirectory, "Web.Host.csproj");
            File.WriteAllText(filePath, "<Project></Project>");

            try
            {
                var result = directoryContains!.Invoke(null, new object[] { tempDirectory, "Web.Host.csproj" });
                result.ShouldBe(true);
            }
            finally
            {
                try { Directory.Delete(tempDirectory, recursive: true); } catch { }
            }
        }
    }
}
