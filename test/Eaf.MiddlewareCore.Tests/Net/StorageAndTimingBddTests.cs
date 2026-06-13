using Eaf.Middleware;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Timing;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Net
{
    /// <summary>
    /// Testes BDD para Storage, Timing e Folder seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class StorageAndTimingBddTests
    {
        #region BinaryObject

        [Fact]
        public void Dado_BinaryObject_Quando_CriarComConstrutorPadrao_Entao_DeveGerarId()
        {
            var obj = new BinaryObject();
            obj.Id.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public void Dado_BinaryObject_Quando_CriarComParametros_Entao_DeveDefinirPropriedades()
        {
            var bytes = new byte[] { 0x48, 0x65, 0x6C };
            var obj = new BinaryObject(1, bytes, "image/png", "photo.png");

            obj.TenantId.ShouldBe(1);
            obj.Bytes.ShouldBe(bytes);
            obj.FileType.ShouldBe("image/png");
            obj.FileName.ShouldContain("photo.png");
            obj.FileName.ShouldContain("_");
            obj.Id.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public void Dado_BinaryObject_SemTenantId_Quando_Criar_Entao_TenantIdDeveSerNull()
        {
            var obj = new BinaryObject(null, new byte[] { 1 }, "text/plain", "file.txt");
            obj.TenantId.ShouldBeNull();
        }

        [Fact]
        public void Dado_BinaryObject_Quando_CriarDoisObjetos_Entao_IdsDevemSerDiferentes()
        {
            var obj1 = new BinaryObject();
            var obj2 = new BinaryObject();
            obj1.Id.ShouldNotBe(obj2.Id);
        }

        #endregion

        #region AppTimes

        [Fact]
        public void Dado_AppTimes_Quando_DefinirStartupTime_Entao_DeveArmazenar()
        {
            var times = new AppTimes { StartupTime = new DateTime(2026, 1, 1, 10, 0, 0) };
            times.StartupTime.ShouldBe(new DateTime(2026, 1, 1, 10, 0, 0));
        }

        #endregion

        #region AppFolders

        [Fact]
        public void Dado_AppFolders_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var folders = new AppFolders
            {
                ProfileImagesFolder = "/data/profiles",
                WebDataFolder = "/data/web",
                WebDownloadFolder = "/data/downloads",
                WebLogsFolder = "/data/logs",
                WebTempFolder = "/data/temp"
            };

            folders.ProfileImagesFolder.ShouldBe("/data/profiles");
            folders.WebDataFolder.ShouldBe("/data/web");
            folders.WebDownloadFolder.ShouldBe("/data/downloads");
            folders.WebLogsFolder.ShouldBe("/data/logs");
            folders.WebTempFolder.ShouldBe("/data/temp");
        }

        #endregion
    }
}
