using Eaf.Middleware.Worker.Folders;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Folders
{
    public class WorkerAppFoldersTests
    {
        [Fact]
        public void Dado_AppFolders_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var folders = new AppFolders
            {
                ProfileImagesFolder = "/images/profiles",
                DataFolder = "/data",
                DownloadFolder = "/downloads",
                LogsFolder = "/logs",
                TempFolder = "/temp"
            };

            folders.ProfileImagesFolder.ShouldBe("/images/profiles");
            folders.DataFolder.ShouldBe("/data");
            folders.DownloadFolder.ShouldBe("/downloads");
            folders.LogsFolder.ShouldBe("/logs");
            folders.TempFolder.ShouldBe("/temp");
        }

        [Fact]
        public void Dado_AppFolders_Quando_Instanciar_Entao_PropriedadesDevemSerNull()
        {
            var folders = new AppFolders();

            folders.ProfileImagesFolder.ShouldBeNull();
            folders.DataFolder.ShouldBeNull();
            folders.DownloadFolder.ShouldBeNull();
            folders.LogsFolder.ShouldBeNull();
            folders.TempFolder.ShouldBeNull();
            folders.RootFileProvider.ShouldBeNull();
        }
    }
}
