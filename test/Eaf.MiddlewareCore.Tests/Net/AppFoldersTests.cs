using Eaf.Middleware;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Net
{
    public class AppFoldersTests
    {
        [Fact]
        public void Dado_AppFolders_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var folders = new AppFolders
            {
                ProfileImagesFolder = "/images/profiles",
                WebDataFolder = "/data",
                WebDownloadFolder = "/downloads",
                WebLogsFolder = "/logs",
                WebTempFolder = "/temp"
            };

            folders.ProfileImagesFolder.ShouldBe("/images/profiles");
            folders.WebDataFolder.ShouldBe("/data");
            folders.WebDownloadFolder.ShouldBe("/downloads");
            folders.WebLogsFolder.ShouldBe("/logs");
            folders.WebTempFolder.ShouldBe("/temp");
        }

        [Fact]
        public void Dado_AppFolders_Quando_Instanciar_Entao_PropriedadesDevemSerNull()
        {
            var folders = new AppFolders();

            folders.ProfileImagesFolder.ShouldBeNull();
            folders.WebDataFolder.ShouldBeNull();
            folders.WebDownloadFolder.ShouldBeNull();
            folders.WebLogsFolder.ShouldBeNull();
            folders.WebTempFolder.ShouldBeNull();
            folders.WebRootFileProvider.ShouldBeNull();
        }
    }
}
