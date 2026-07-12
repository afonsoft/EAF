using Eaf.Middleware.Web;
using Shouldly;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Tests.Net.Web
{
    public class WebContentDirectoryFinderBddTests
    {
        [Fact]
        public void Dado_AssemblyCore_Quando_CalculateContentRootFolder_Entao_DeveLancarExcecaoSeWebHostNaoExistir()
        {
            var exception = Assert.Throws<Exception>(() => WebContentDirectoryFinder.CalculateContentRootFolder());

            exception.Message.ShouldContain("Could not find root folder of the web project");
        }

        [Fact]
        public void Dado_WebHostExistente_Quando_CalculateContentRootFolder_Entao_DeveRetornarCaminhoWebHost()
        {
            var assemblyPath = Path.GetDirectoryName(typeof(MiddlewareCoreModule).Assembly.Location);
            var directoryInfo = new DirectoryInfo(assemblyPath);
            while (directoryInfo != null && !File.Exists(Path.Combine(directoryInfo.FullName, "Eaf.sln")))
            {
                directoryInfo = directoryInfo.Parent;
            }

            directoryInfo.ShouldNotBeNull();

            var webHostFolder = Path.Combine(directoryInfo.FullName, $"src{Path.DirectorySeparatorChar}Eaf.Middleware.Web.Host");
            Directory.CreateDirectory(webHostFolder);

            try
            {
                var result = WebContentDirectoryFinder.CalculateContentRootFolder();
                result.ShouldBe(webHostFolder);
            }
            finally
            {
                try { Directory.Delete(webHostFolder, true); } catch { }
            }
        }

        [Fact]
        public void Dado_DiretorioComArquivo_Quando_DirectoryContains_Entao_DeveRetornarVerdadeiro()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            var filePath = Path.Combine(tempDirectory, "Eaf.sln");
            File.WriteAllText(filePath, string.Empty);

            try
            {
                var method = typeof(WebContentDirectoryFinder).GetMethod("DirectoryContains", BindingFlags.NonPublic | BindingFlags.Static);
                method.ShouldNotBeNull();
                var result = method!.Invoke(null, new object[] { tempDirectory, "Eaf.sln" });
                result.ShouldBe(true);
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_DiretorioVazio_Quando_DirectoryContains_Entao_DeveRetornarFalso()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var method = typeof(WebContentDirectoryFinder).GetMethod("DirectoryContains", BindingFlags.NonPublic | BindingFlags.Static);
                method.ShouldNotBeNull();
                var result = method!.Invoke(null, new object[] { tempDirectory, "Inexistente.txt" });
                result.ShouldBe(false);
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }
    }
}
