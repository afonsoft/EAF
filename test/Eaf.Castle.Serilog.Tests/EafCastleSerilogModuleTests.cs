using Abp.Modules;
using Eaf.Castle.Logging.SerilogIntegration;
using Shouldly;
using Xunit;

namespace Eaf.Castle.Serilog.Tests
{
    public class EafCastleSerilogModuleTests
    {
        [Fact]
        public void EafCastleSerilogModule_ShouldBeAbpModule()
        {
            // Arrange
            var moduleType = typeof(EafCastleSerilogModule);

            // Act & Assert
            typeof(AbpModule).IsAssignableFrom(moduleType).ShouldBeTrue();
        }

        [Fact]
        public void EafCastleSerilogModule_ShouldBeInstantiable()
        {
            // Act
            var module = new EafCastleSerilogModule();

            // Assert
            module.ShouldNotBeNull();
            module.ShouldBeOfType<EafCastleSerilogModule>();
        }
    }
}
