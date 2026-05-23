using Eaf.Middleware.Worker.VirtualFileSystem;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Shouldly;
using System;
using System.IO;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.VirtualFileSystem
{
    public class WorkerContentFileProviderTests
    {
        private readonly string _tempDir;
        private readonly IHostEnvironment _hostEnvironment;

        public WorkerContentFileProviderTests()
        {
            _tempDir = Path.GetTempPath();
            _hostEnvironment = Substitute.For<IHostEnvironment>();
            _hostEnvironment.ContentRootPath.Returns(_tempDir);
        }

        [Fact]
        public void WorkerContentFileProvider_ShouldImplementIWorkerContentFileProvider()
        {
            // Act
            var provider = new WorkerContentFileProvider(_hostEnvironment);

            // Assert
            provider.ShouldNotBeNull();
            provider.ShouldBeAssignableTo<IWorkerContentFileProvider>();
            provider.ShouldBeAssignableTo<IFileProvider>();
        }

        [Fact]
        public void GetFileInfo_WithValidPath_ShouldReturnFileInfo()
        {
            // Arrange
            var provider = new WorkerContentFileProvider(_hostEnvironment);

            // Act
            var fileInfo = provider.GetFileInfo("test.txt");

            // Assert
            fileInfo.ShouldNotBeNull();
        }

        [Fact]
        public void GetFileInfo_WithPathNavigatingAboveRoot_ShouldReturnNotFoundFileInfo()
        {
            // Arrange
            var provider = new WorkerContentFileProvider(_hostEnvironment);

            // Act
            var fileInfo = provider.GetFileInfo("../test.txt");

            // Assert
            fileInfo.ShouldNotBeNull();
            fileInfo.Exists.ShouldBeFalse();
            fileInfo.ShouldBeOfType<NotFoundFileInfo>();
        }

        [Fact]
        public void GetDirectoryContents_WithValidPath_ShouldReturnDirectoryContents()
        {
            // Arrange
            var provider = new WorkerContentFileProvider(_hostEnvironment);

            // Act
            var contents = provider.GetDirectoryContents(".");

            // Assert
            contents.ShouldNotBeNull();
        }

        [Fact]
        public void GetDirectoryContents_WithPathNavigatingAboveRoot_ShouldReturnNotFoundDirectoryContents()
        {
            // Arrange
            var provider = new WorkerContentFileProvider(_hostEnvironment);

            // Act
            var contents = provider.GetDirectoryContents("../");

            // Assert
            contents.ShouldNotBeNull();
            contents.Exists.ShouldBeFalse();
            contents.ShouldBe(NotFoundDirectoryContents.Singleton);
        }

        [Fact]
        public void Watch_WithFilter_ShouldReturnChangeToken()
        {
            // Arrange
            var provider = new WorkerContentFileProvider(_hostEnvironment);

            // Act
            var changeToken = provider.Watch("*.txt");

            // Assert
            changeToken.ShouldNotBeNull();
            changeToken.ShouldBeOfType<CompositeChangeToken>();
        }

        [Theory]
        [InlineData("")]
        public void GetFileInfo_WithEmptyPath_ShouldThrowException(string path)
        {
            // Arrange
            var provider = new WorkerContentFileProvider(_hostEnvironment);

            // Act & Assert
            Should.Throw<System.ArgumentException>(() => provider.GetFileInfo(path));
        }

        [Fact]
        public void GetFileInfo_WithNullPath_ShouldThrowException()
        {
            // Arrange
            var provider = new WorkerContentFileProvider(_hostEnvironment);

            // Act & Assert
            Should.Throw<System.ArgumentException>(() => provider.GetFileInfo(null!));
        }

        [Theory]
        [InlineData("")]
        public void GetDirectoryContents_WithEmptyPath_ShouldThrowException(string path)
        {
            // Arrange
            var provider = new WorkerContentFileProvider(_hostEnvironment);

            // Act & Assert
            Should.Throw<System.ArgumentException>(() => provider.GetDirectoryContents(path));
        }

        [Fact]
        public void GetDirectoryContents_WithNullPath_ShouldThrowException()
        {
            // Arrange
            var provider = new WorkerContentFileProvider(_hostEnvironment);

            // Act & Assert
            Should.Throw<System.ArgumentException>(() => provider.GetDirectoryContents(null!));
        }

        [Fact]
        public void WorkerContentFileProvider_WithDifferentHostEnvironments_ShouldWork()
        {
            // Arrange
            var environments = new[] { "Development", "Production", "Staging" };

            foreach (var env in environments)
            {
                var hostEnv = Substitute.For<IHostEnvironment>();
                hostEnv.ContentRootPath.Returns(_tempDir);
                hostEnv.EnvironmentName.Returns(env);

                // Act & Assert
                Should.NotThrow(() => new WorkerContentFileProvider(hostEnv));
            }
        }
    }
}