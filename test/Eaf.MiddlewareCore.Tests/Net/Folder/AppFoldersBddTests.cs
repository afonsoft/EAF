using Eaf.Middleware;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Net.Folder
{
    /// <summary>
    /// Testes BDD para AppFolders seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AppFoldersBddTests
    {
        [Fact]
        public void Dado_AppFolders_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var folders = new AppFolders
            {
                ProfileImagesFolder = "/data/images",
                WebDataFolder = "/data/web",
                WebDownloadFolder = "/data/download",
                WebLogsFolder = "/data/logs",
                WebTempFolder = "/data/temp"
            };

            folders.ProfileImagesFolder.ShouldBe("/data/images");
            folders.WebDataFolder.ShouldBe("/data/web");
            folders.WebDownloadFolder.ShouldBe("/data/download");
            folders.WebLogsFolder.ShouldBe("/data/logs");
            folders.WebTempFolder.ShouldBe("/data/temp");
        }
    }
}
