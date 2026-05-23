using Eaf.Middleware.Worker;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Worker.Tests
{
    public class EafWorkerBaseTests
    {
        [Fact]
        public void EafWorkerBase_ShouldImplementIEafWorkerBase()
        {
            // Arrange & Act
            var type = typeof(EafWorkerBase);

            // Assert
            type.ShouldNotBeNull();
            typeof(IEafWorkerBase).IsAssignableFrom(type).ShouldBeTrue();
        }

        [Fact]
        public void EafWorkerBase_ShouldBeAbstract()
        {
            // Arrange & Act
            var type = typeof(EafWorkerBase);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
        }
    }
}
