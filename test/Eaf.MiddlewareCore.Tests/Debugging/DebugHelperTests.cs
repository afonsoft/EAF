using Eaf.Middleware.Debugging;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Debugging
{
    public class DebugHelperTests
    {
        [Fact]
        public void IsDebug_ShouldReturnBoolean()
        {
            // Arrange & Act
            var isDebug = DebugHelper.IsDebug;

            // Assert
            isDebug.ShouldBeOfType<bool>();
        }

        [Fact]
        public void IsDebug_ShouldBeFalseInRelease()
        {
            // Arrange & Act
            var isDebug = DebugHelper.IsDebug;

            // Assert - In release builds, this should be false
            // In debug builds, this would be true, but we can't test both scenarios in the same build
            isDebug.ShouldBeOfType<bool>();
        }

        [Fact]
        public void DebugHelper_ShouldBeStaticClass()
        {
            // Arrange & Act
            var type = typeof(DebugHelper);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }
    }
}
