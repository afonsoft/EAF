using Eaf.Middleware.Worker.VirtualFileSystem;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Worker.Tests
{
    public class PathUtilsTests
    {
        [Theory]
        [InlineData("../test", true)]
        [InlineData("../../test", true)]
        [InlineData("test/../..", true)]
        [InlineData("test/../../file", true)]
        public void PathNavigatesAboveRoot_WithPathsNavigatingAbove_ShouldReturnTrue(string path, bool expected)
        {
            // Act
            var result = PathUtils.PathNavigatesAboveRoot(path);

            // Assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData("test/path", false)]
        [InlineData("./test/path", false)]
        [InlineData("test/../path", false)]
        [InlineData("test/./path", false)]
        [InlineData("", false)]
        [InlineData(".", false)]
        public void PathNavigatesAboveRoot_WithSafePaths_ShouldReturnFalse(string path, bool expected)
        {
            // Act
            var result = PathUtils.PathNavigatesAboveRoot(path);

            // Assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData("test/../test2/file", false)]
        [InlineData("./test/../test2", false)]
        [InlineData("test/file/../other", false)]
        public void PathNavigatesAboveRoot_WithComplexButSafePaths_ShouldReturnFalse(string path, bool expected)
        {
            // Act
            var result = PathUtils.PathNavigatesAboveRoot(path);

            // Assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData("test/file/../../../unsafe", true)]
        [InlineData("test/../../file/../safe", true)]
        [InlineData("../", true)]
        [InlineData("..", true)]
        public void PathNavigatesAboveRoot_WithUnsafePaths_ShouldReturnTrue(string path, bool expected)
        {
            // Act
            var result = PathUtils.PathNavigatesAboveRoot(path);

            // Assert
            result.ShouldBe(expected);
        }

        [Fact]
        public void PathNavigatesAboveRoot_WithNullPath_ShouldThrowException()
        {
            // Act & Assert
            Should.Throw<System.ArgumentNullException>(() => PathUtils.PathNavigatesAboveRoot(null));
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public void PathNavigatesAboveRoot_WithWhitespacePath_ShouldReturnFalse(string path)
        {
            // Act
            var result = PathUtils.PathNavigatesAboveRoot(path);

            // Assert
            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData("folder/../file.txt", false)] // Resolves to file.txt
        [InlineData("folder/subfolder/../../file.txt", false)] // Resolves to file.txt
        [InlineData("/absolute/path.txt", false)] // Absolute paths don't navigate above root
        [InlineData("normal/path/file.txt", false)] // Normal nested path
        [InlineData("a/b/c/d/../../../../e", false)] // Goes back to root level, not above
        [InlineData("a/b/c/d/../../../../../e", true)] // Goes one level above root
        public void PathNavigatesAboveRoot_EdgeCases_ShouldReturnCorrectResult(string path, bool expected)
        {
            // Act
            var result = PathUtils.PathNavigatesAboveRoot(path);

            // Assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData("file.txt")]
        [InlineData("folder/file.txt")]
        [InlineData("folder/subfolder/file.txt")]
        [InlineData("a/b/c/d/e/f/g/file.txt")]
        public void PathNavigatesAboveRoot_WithDeepValidPaths_ShouldReturnFalse(string path)
        {
            // Act
            var result = PathUtils.PathNavigatesAboveRoot(path);

            // Assert
            result.ShouldBeFalse();
        }
    }
}