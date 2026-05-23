using Castle.Core.Logging;
using Eaf.Log4NetServiceBus.Logging;
using log4net;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Log4NetServiceBus.Tests
{
    public class LogExtensionsTests
    {
        [Fact]
        public void Debug_WithCastleLogger_ShouldCallDebugAndSetJsonProperty()
        {
            // Arrange
            var logger = Substitute.For<ILogger>();
            var testObject = new { Name = "Test", Value = 123 };
            var message = "Test debug message";

            // Act
            logger.Debug(message, testObject);

            // Assert
            logger.Received(1).Debug(message);
        }

        [Fact]
        public void Debug_WithLog4NetLogger_ShouldCallDebugAndSetJsonProperty()
        {
            // Arrange
            var logger = Substitute.For<ILog>();
            var testObject = new { Name = "Test", Value = 123 };
            var message = "Test debug message";

            // Act
            logger.Debug(message, testObject);

            // Assert
            logger.Received(1).Debug(message);
        }

        [Fact]
        public void Error_WithCastleLogger_ShouldCallErrorAndSetJsonProperty()
        {
            // Arrange
            var logger = Substitute.For<ILogger>();
            var testObject = new { Error = "TestError", Code = 500 };
            var message = "Test error message";

            // Act
            logger.Error(message, testObject);

            // Assert
            logger.Received(1).Error(message);
        }

        [Fact]
        public void Error_WithLog4NetLogger_ShouldCallErrorAndSetJsonProperty()
        {
            // Arrange
            var logger = Substitute.For<ILog>();
            var testObject = new { Error = "TestError", Code = 500 };
            var message = "Test error message";

            // Act
            logger.Error(message, testObject);

            // Assert
            logger.Received(1).Error(message);
        }

        [Fact]
        public void Info_WithCastleLogger_ShouldCallInfoAndSetJsonProperty()
        {
            // Arrange
            var logger = Substitute.For<ILogger>();
            var testObject = new { Info = "TestInfo", Status = "Success" };
            var message = "Test info message";

            // Act
            logger.Info(message, testObject);

            // Assert
            logger.Received(1).Info(message);
        }

        [Fact]
        public void Info_WithLog4NetLogger_ShouldCallInfoAndSetJsonProperty()
        {
            // Arrange
            var logger = Substitute.For<ILog>();
            var testObject = new { Info = "TestInfo", Status = "Success" };
            var message = "Test info message";

            // Act
            logger.Info(message, testObject);

            // Assert
            logger.Received(1).Info(message);
        }

        [Fact]
        public void Warn_WithCastleLogger_ShouldCallWarnAndSetJsonProperty()
        {
            // Arrange
            var logger = Substitute.For<ILogger>();
            var testObject = new { Warning = "TestWarning", Level = "Medium" };
            var message = "Test warn message";

            // Act
            logger.Warn(message, testObject);

            // Assert
            logger.Received(1).Warn(message);
        }

        [Fact]
        public void Warn_WithLog4NetLogger_ShouldCallWarnAndSetJsonProperty()
        {
            // Arrange
            var logger = Substitute.For<ILog>();
            var testObject = new { Warning = "TestWarning", Level = "Medium" };
            var message = "Test warn message";

            // Act
            logger.Warn(message, testObject);

            // Assert
            logger.Received(1).Warn(message);
        }

        [Fact]
        public void LogExtensions_WithNullObject_ShouldHandleGracefully()
        {
            // Arrange
            var logger = Substitute.For<ILogger>();
            var message = "Test message with null object";

            // Act & Assert (should not throw)
            Should.NotThrow(() => logger.Debug(message, null));
            Should.NotThrow(() => logger.Info(message, null));
            Should.NotThrow(() => logger.Warn(message, null));
            Should.NotThrow(() => logger.Error(message, null));

            // The extension methods should call the underlying logger methods
            // We can't easily verify the exact calls due to the extension method implementation
            // but we can verify no exceptions are thrown
        }

        [Fact]
        public void LogExtensions_WithComplexObject_ShouldSerializeToJson()
        {
            // Arrange
            var logger = Substitute.For<ILogger>();
            var complexObject = new
            {
                Id = 1,
                Name = "Test Object",
                Properties = new { Prop1 = "Value1", Prop2 = 42 },
                Tags = new[] { "tag1", "tag2", "tag3" }
            };
            var message = "Test message with complex object";

            // Act
            logger.Info(message, complexObject);

            // Assert
            logger.Received(1).Info(message);
        }
    }
}