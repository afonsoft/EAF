using Abp.Modules;
using Eaf.AspNetCore.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.OpenTelemetry.Tests.AspNetCore.Configuration
{
    public class EafOpenTelemetryModuleTests
    {
        [Fact]
        public void EafOpenTelemetryModule_ShouldBeAbpModule()
        {
            // Arrange
            var moduleType = typeof(EafOpenTelemetryModule);

            // Act & Assert
            typeof(AbpModule).IsAssignableFrom(moduleType).ShouldBeTrue();
        }

        [Fact]
        public void EafOpenTelemetryModule_ShouldBeInstantiable()
        {
            // Act
            var module = new EafOpenTelemetryModule();

            // Assert
            module.ShouldNotBeNull();
            module.ShouldBeOfType<EafOpenTelemetryModule>();
        }
    }
}
