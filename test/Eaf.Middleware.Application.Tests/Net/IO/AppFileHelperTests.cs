using Eaf.Middleware.IO;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Net.IO
{
    public class AppFileHelperTests
    {
        [Fact]
        public void AppFileHelper_ShouldBeStaticClass()
        {
            // Arrange & Act
            var type = typeof(AppFileHelper);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void AppFileHelper_ShouldHaveMethods()
        {
            // Arrange & Act
            var type = typeof(AppFileHelper);
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            // Assert
            methods.ShouldNotBeNull();
            methods.Length.ShouldBeGreaterThan(0);
        }
    }
}
