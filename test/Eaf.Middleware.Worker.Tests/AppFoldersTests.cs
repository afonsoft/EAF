using Eaf.Middleware.Worker.Folders;
using Microsoft.Extensions.FileProviders;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Worker.Tests
{
    public class AppFoldersTests
    {
        [Fact]
        public void AppFolders_DefaultConstructor_ShouldInitializeCorrectly()
        {
            // Act
            var appFolders = new AppFolders();

            // Assert
            appFolders.ShouldNotBeNull();
            appFolders.ShouldBeAssignableTo<IAppFolders>();
        }

        [Fact]
        public void AppFolders_ProfileImagesFolder_ShouldBeSettable()
        {
            // Arrange
            var appFolders = new AppFolders();
            var testPath = "/test/profile-images";

            // Act
            appFolders.ProfileImagesFolder = testPath;

            // Assert
            appFolders.ProfileImagesFolder.ShouldBe(testPath);
        }

        [Fact]
        public void AppFolders_DataFolder_ShouldBeSettable()
        {
            // Arrange
            var appFolders = new AppFolders();
            var testPath = "/test/data";

            // Act
            appFolders.DataFolder = testPath;

            // Assert
            appFolders.DataFolder.ShouldBe(testPath);
        }

        [Fact]
        public void AppFolders_DownloadFolder_ShouldBeSettable()
        {
            // Arrange
            var appFolders = new AppFolders();
            var testPath = "/test/downloads";

            // Act
            appFolders.DownloadFolder = testPath;

            // Assert
            appFolders.DownloadFolder.ShouldBe(testPath);
        }

        [Fact]
        public void AppFolders_LogsFolder_ShouldBeSettable()
        {
            // Arrange
            var appFolders = new AppFolders();
            var testPath = "/test/logs";

            // Act
            appFolders.LogsFolder = testPath;

            // Assert
            appFolders.LogsFolder.ShouldBe(testPath);
        }

        [Fact]
        public void AppFolders_TempFolder_ShouldBeSettable()
        {
            // Arrange
            var appFolders = new AppFolders();
            var testPath = "/test/temp";

            // Act
            appFolders.TempFolder = testPath;

            // Assert
            appFolders.TempFolder.ShouldBe(testPath);
        }

        [Fact]
        public void AppFolders_RootFileProvider_ShouldBeSettable()
        {
            // Arrange
            var appFolders = new AppFolders();
            var fileProvider = new CompositeFileProvider();

            // Act
            appFolders.RootFileProvider = fileProvider;

            // Assert
            appFolders.RootFileProvider.ShouldBe(fileProvider);
        }

        [Fact]
        public void AppFolders_AllProperties_ShouldBeNullByDefault()
        {
            // Act
            var appFolders = new AppFolders();

            // Assert
            appFolders.ProfileImagesFolder.ShouldBeNull();
            appFolders.DataFolder.ShouldBeNull();
            appFolders.DownloadFolder.ShouldBeNull();
            appFolders.LogsFolder.ShouldBeNull();
            appFolders.TempFolder.ShouldBeNull();
            appFolders.RootFileProvider.ShouldBeNull();
        }

        [Fact]
        public void AppFolders_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange
            var appFolders1 = new AppFolders();
            var appFolders2 = new AppFolders();

            // Act
            appFolders1.DataFolder = "/path1";
            appFolders2.DataFolder = "/path2";

            // Assert
            appFolders1.DataFolder.ShouldBe("/path1");
            appFolders2.DataFolder.ShouldBe("/path2");
            appFolders1.DataFolder.ShouldNotBe(appFolders2.DataFolder);
        }

        [Fact]
        public void AppFolders_SetAllProperties_ShouldWork()
        {
            // Arrange
            var appFolders = new AppFolders();
            var fileProvider = new CompositeFileProvider();

            // Act
            appFolders.ProfileImagesFolder = "/profile";
            appFolders.DataFolder = "/data";
            appFolders.DownloadFolder = "/download";
            appFolders.LogsFolder = "/logs";
            appFolders.TempFolder = "/temp";
            appFolders.RootFileProvider = fileProvider;

            // Assert
            appFolders.ProfileImagesFolder.ShouldBe("/profile");
            appFolders.DataFolder.ShouldBe("/data");
            appFolders.DownloadFolder.ShouldBe("/download");
            appFolders.LogsFolder.ShouldBe("/logs");
            appFolders.TempFolder.ShouldBe("/temp");
            appFolders.RootFileProvider.ShouldBe(fileProvider);
        }
    }
}