using Eaf.Middleware.IO;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Tests.IO
{
    /// <summary>
    /// Testes BDD para AppFileHelper seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AppFileHelperBddTests
    {
        [Fact]
        public void Dado_ArquivoExistente_Quando_ReadLines_Entao_DeveRetornarLinhas()
        {
            // Dado
            var tempFile = Path.GetTempFileName();
            File.WriteAllLines(tempFile, new[] { "linha1", "linha2", "linha3" });

            try
            {
                // Quando
                var lines = AppFileHelper.ReadLines(tempFile).ToList();

                // Então
                lines.Count.ShouldBe(3);
                lines[0].ShouldBe("linha1");
                lines[1].ShouldBe("linha2");
                lines[2].ShouldBe("linha3");
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void Dado_ArquivoVazio_Quando_ReadLines_Entao_DeveRetornarVazio()
        {
            // Dado
            var tempFile = Path.GetTempFileName();

            try
            {
                // Quando
                var lines = AppFileHelper.ReadLines(tempFile).ToList();

                // Então
                lines.Count.ShouldBe(0);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void Dado_PastaComArquivos_Quando_DeleteFilesInFolderIfExists_Entao_DeveExcluirArquivosCorretos()
        {
            // Dado
            var tempDir = Path.Combine(Path.GetTempPath(), "eaf-test-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            var targetFile1 = Path.Combine(tempDir, "profile.jpg");
            var targetFile2 = Path.Combine(tempDir, "profile.png");
            var otherFile = Path.Combine(tempDir, "other.txt");
            File.WriteAllText(targetFile1, "img");
            File.WriteAllText(targetFile2, "img");
            File.WriteAllText(otherFile, "data");

            try
            {
                // Quando
                AppFileHelper.DeleteFilesInFolderIfExists(tempDir, "profile");

                // Então
                File.Exists(targetFile1).ShouldBeFalse();
                File.Exists(targetFile2).ShouldBeFalse();
                File.Exists(otherFile).ShouldBeTrue();
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void Dado_PastaVazia_Quando_DeleteFilesInFolderIfExists_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var tempDir = Path.Combine(Path.GetTempPath(), "eaf-test-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            try
            {
                // Quando/Então
                Should.NotThrow(() => AppFileHelper.DeleteFilesInFolderIfExists(tempDir, "nonexistent"));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
